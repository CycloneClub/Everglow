using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 装甲虾（Armored Shrimp），亡碧湖（Death Jade Lake）水中的被动群居虾。
/// Design row: 装甲虾, heading block id Z94rdc53uoe4Wyx7TRPck6g5nQb,
/// behavior block ELQgdcR30oJFwAxNBgjcmXmQnXd (D-28 full implementation).
/// The design's 伤害, 击退抗性, 减伤, 免疫 and 钱币 cells are all empty, so the creature is a pure
/// passive critter: no contact damage, no target and no loot.
/// <para>
/// <b>Group spawn (assumption, D-54).</b> 在水里成群（2~5只）刷新，然后成群移动 - tML has no group-spawn
/// primitive, so the leader creates the rest of its shoal in <see cref="OnSpawn(IEntitySource)"/> on the
/// authoritative side and every created member carries a follower marker in <c>NPC.ai[1]</c>, which is
/// exactly what stops the group from growing without bound (T-04-17).
/// </para>
/// <para>
/// <b>Capture blocker (D-58).</b> The design says 可被捕捉, but no catch item for this creature exists
/// anywhere in the repository, so this class writes no catch-item type, no <c>Main.npcCatchable</c> flag
/// and no <c>NPCID.Sets.CountsAsCritter</c> entry (04-DEVIATIONS.md section 6.2 and section 13).
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback (D-48). The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the migration is adding
/// ArmoredShrimp.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset
/// path is Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ArmoredShrimp.png.
/// </para>
/// </summary>
public class ArmoredShrimp : ModNPC
{
	/// <summary>在水里成群（2~5只）: the design's lower group bound.</summary>
	private const int MinGroupSize = 2;

	/// <summary>在水里成群（2~5只）: the design's upper group bound.</summary>
	private const int MaxGroupSize = 5;

	/// <summary>
	/// The follower marker written into <c>NPC.ai[1]</c>. It is passed through the <c>ai1</c> parameter of
	/// <c>NPC.NewNPC</c>, so the member already reads it during its own <see cref="OnSpawn(IEntitySource)"/>
	/// and never starts a second group (T-04-17).
	/// </summary>
	private const float FollowerMarker = 1f;

	/// <summary>How far, at most, a created member is placed from its leader, in pixels (D-54 default).</summary>
	private const float GroupSpreadPixels = 48f;

	/// <summary>成群移动: the shoal's drift speed, in pixels per tick (D-54 default).</summary>
	private const float ShoalSpeed = 0.55f;

	/// <summary>The slower idle drift used by a creature with nothing to shoal with (D-54 default).</summary>
	private const float DriftSpeed = 0.3f;

	/// <summary>只有些许的上下摆动: the vertical bob's amplitude, in pixels per tick (D-54 default).</summary>
	private const float BobAmplitude = 0.22f;

	/// <summary>只有些许的上下摆动: the vertical bob's angular speed (D-54 default).</summary>
	private const float BobSpeed = 0.05f;

	/// <summary>How often the server re-rolls the shoal state and the wander heading, in frames (D-54 default).</summary>
	private const int WanderInterval = 150;

	/// <summary>成群移动: the greatest gap a follower tolerates before steering back toward a shoal mate (D-54 default).</summary>
	private const float ShoalCohesionRange = 6f * 16f;

	/// <summary>
	/// The two states of the design's behaviour (D-31): 成群移动 while a same-type neighbour is close
	/// enough to shoal with, and a slower lone drift when there is none. No vanilla <c>aiStyle</c>
	/// provides an aquatic critter shoal, so the class owns its <c>AI()</c>.
	/// </summary>
	private enum ArmoredShrimpState
	{
		Shoaling = 0,
		Drifting = 1,
	}

	/// <summary>
	/// <c>true</c> on every creature a leader created. A follower never rolls a group of its own - the
	/// marker is the design's answer to tML having no group-spawn primitive, and it is what keeps
	/// 成群（2~5只） from cascading (T-04-17).
	/// </summary>
	private bool IsGroupFollower
	{
		get => NPC.ai[1] >= FollowerMarker;
		set => NPC.ai[1] = value ? FollowerMarker : 0f;
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private ArmoredShrimpState State
	{
		get => (ArmoredShrimpState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames left before the next wander re-roll.</summary>
	private int WanderTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[1]</c>: the heading (<c>1</c> or <c>-1</c>) the shoal drifts along.</summary>
	private int Heading
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	/// <summary>
	/// How many leaders are currently creating their shoal. The <c>ai1</c> marker is the primary mechanism
	/// and is what reaches the member's own <see cref="OnSpawn(IEntitySource)"/> through
	/// <c>NPC.NewNPC</c>'s <c>ai1</c> parameter; this counter is the belt-and-braces guard, so a member can
	/// never roll a group even if the engine assigned the <c>ai[]</c> values in a different order
	/// (T-04-17).
	/// </summary>
	private static int groupCreationDepth;

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	// Approved artwork is missing from the repository (D-48). The shared fallback keeps the class
	// loadable, and the phase's gate asserts the override until the art arrives.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		// No approved sprite exists, so there is nothing to slice: the whole texture is one frame.
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// The design row's 免疫 cell is empty, so no specific debuff immunity is declared.
	}

	public override void SetDefaults()
	{
		// The shoal drift has no vanilla aiStyle analogue, so the class owns its AI (D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a small critter-sized box is used and is the first value to
		// revisit when the art arrives (D-54).
		NPC.width = 22;
		NPC.height = 14;

		NPC.lifeMax = 10; // 生命 10
		NPC.life = 10;

		// The design's 伤害 cell is empty, so this passive creature deals no contact damage at all.
		NPC.damage = 0;
		NPC.defDamage = 0;

		NPC.defense = 4; // 防御 4
		NPC.defDefense = 4;

		// The design's 击退抗性 cell is empty, so the small-critter default used by the phase's other
		// passive creature (小格普螺) is kept rather than a design-derived value being invented (D-54).
		NPC.knockBackResist = 0.8f;

		// The design's 钱币 cell is empty, so the creature carries no coin value.
		NPC.value = 0;

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field (the API-name correction
		// recorded in 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		// 水生生物: it must not be pulled down by gravity. Tile collision stays on, as on the phase's
		// other aquatic creatures, so it cannot drift through the lake bed.
		NPC.noGravity = true;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 可被捕捉 in the design, but no catch item for this creature exists in the repository, so the
		// capture is a blocked item-scope dependency (D-58): no catch-item type reference, no catchable
		// flag and no critter-count set entry are written here.
		NPC.catchItem = 0;

		// Deterministic starting values, because SetDefaults runs on every side: the follower marker the
		// engine passes through NPC.NewNPC's ai1 parameter is applied after this method, so nothing here
		// can clobber a member's marker.
		State = ArmoredShrimpState.Shoaling;
		IsGroupFollower = false;
		WanderTimer = WanderInterval;
		Heading = 1;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// 在水里成群（2~5只）刷新: the leader rolls the design's group size and creates the remaining members
	/// around itself, and every created member is marked as a follower so it never starts a group of its
	/// own. The whole body runs on the authoritative side, because <c>NPC.NewNPC</c> must never be called
	/// on a multiplayer client (D-55).
	/// </summary>
	/// <param name="source">The engine's spawn source.</param>
	public override void OnSpawn(IEntitySource source)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			CreateShoal();
		}
	}

	/// <summary>
	/// The leader's share of 在水里成群（2~5只）: it rolls the design's group size and creates the remaining
	/// members around itself. Only ever reached on the authoritative side, because <c>NPC.NewNPC</c> must
	/// never be called on a multiplayer client (D-55).
	/// </summary>
	private void CreateShoal()
	{
		if (IsGroupFollower || groupCreationDepth > 0)
		{
			// A member created by a leader never starts a further group: that is the whole point of the
			// 2~5 bound (2 is the lower bound, 5 the upper one) and it is re-asserted here so the synced
			// marker is correct no matter which side the engine applied the ai[] values from.
			IsGroupFollower = true;
			NPC.netUpdate = true;
			return;
		}

		int groupSize = Main.rand.Next(MinGroupSize, MaxGroupSize + 1);
		int memberCount = groupSize - 1;
		groupCreationDepth++;
		try
		{
			for (int i = 0; i < memberCount; i++)
			{
				Vector2 offset = new(
					Main.rand.NextFloat(-GroupSpreadPixels, GroupSpreadPixels),
					Main.rand.NextFloat(-GroupSpreadPixels, GroupSpreadPixels));

				// ai1 carries the follower marker, so the member reads it during its own OnSpawn.
				// The X/Y pair is the engine's centred spawn position, exactly as its own summons pass it.
				NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + offset.X), (int)(NPC.Center.Y + offset.Y), Type, 0, 0f, FollowerMarker);
			}
		}
		finally
		{
			groupCreationDepth--;
		}

		NPC.netUpdate = true;
	}

	/// <summary>
	/// 成群移动: the shoal drifts along a shared heading, a follower steers loosely back toward a shoal
	/// mate so the group holds together, and the creature 不会逃离捕食者 - it never flees and never
	/// acquires a target, so this is the whole of its behaviour. The heading re-roll is authoritative and
	/// rides the synced <c>NPC.localAI[]</c> arrays (D-55).
	/// </summary>
	public override void AI()
	{
		UpdateWander();

		if (IsGroupFollower && TryGetNearestShoalMate(out Vector2 mateCenter))
		{
			// Loose cohesion only: a follower far enough from its nearest shoal mate turns back toward it,
			// and otherwise keeps drifting, so the group reads as a loosely coupled shoal rather than a
			// rigid formation.
			float deltaX = mateCenter.X - NPC.Center.X;
			if (MathF.Abs(deltaX) > ShoalCohesionRange)
			{
				Heading = deltaX > 0f ? 1 : -1;
			}
		}

		float speed = State == ArmoredShrimpState.Shoaling ? ShoalSpeed : DriftSpeed;
		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Heading * speed, 0.04f);

		// 不会移动 away: only 些许的上下摆动, so the vertical motion stays a small periodic bob.
		NPC.velocity.Y = MathF.Sin((float)Main.time * BobSpeed + NPC.whoAmI) * BobAmplitude;

		if (NPC.collideX)
		{
			// A wall or the lake edge: turn the shoal around instead of pushing into terrain forever.
			Heading = -Heading;
		}

		NPC.direction = Heading >= 0 ? 1 : -1;
		NPC.spriteDirection = NPC.direction;

		UpdateWaterTrail();
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (behavior block ELQgdcR30oJFwAxNBgjcmXmQnXd), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	/// <param name="modifiers">The hit's modifiers; deliberately untouched.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// 死亡后掉落1 软体甲壳碎片: 软体甲壳碎片 has no ModItem in the repository, so no rule is written and
	/// no item type is referenced here (D-58, Pitfall 2): a <c>ModContent.ItemType&lt;X&gt;()</c> for an
	/// absent <c>X</c> would fail the whole mod build. The blocker lives in 04-DEVIATIONS.md section 6.2
	/// and in the biology matrix instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}

	/// <summary>
	/// 成群移动: re-rolls the shoal state and, on a turn, the heading on the authoritative side only, and
	/// pushes the new values to the clients through the synced <c>NPC.ai[]</c> / <c>NPC.localAI[]</c>
	/// arrays (D-55).
	/// </summary>
	private void UpdateWander()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			WanderTimer--;
			if (WanderTimer <= 0)
			{
				WanderTimer = WanderInterval;
				bool hasMate = TryGetNearestShoalMate(out _);
				State = hasMate ? ArmoredShrimpState.Shoaling : ArmoredShrimpState.Drifting;
				if (Main.rand.NextBool(2))
				{
					Heading = -Heading;
				}

				NPC.netUpdate = true;
			}
		}
	}

	/// <summary>
	/// The nearest live creature of this same type except this one, used for the shoal's loose cohesion.
	/// The type is resolved through <c>Type</c>, never a string or name-based lookup.
	/// </summary>
	/// <param name="mateCenter">The shoal mate's centre.</param>
	/// <returns>True when a live shoal mate exists.</returns>
	private bool TryGetNearestShoalMate(out Vector2 mateCenter)
	{
		mateCenter = default;
		float bestDistance = float.MaxValue;
		for (int i = 0; i < Main.maxNPCs; i++)
		{
			NPC other = Main.npc[i];
			if (!other.active || other.life <= 0 || other.whoAmI == NPC.whoAmI || other.type != Type)
			{
				continue;
			}

			float distance = Vector2.Distance(NPC.Center, other.Center);
			if (distance < bestDistance)
			{
				bestDistance = distance;
				mateCenter = other.Center;
			}
		}

		return bestDistance < float.MaxValue;
	}

	/// <summary>
	/// 水生生物: a faint water trail so the drifting shoal reads as a creature in water. Client-only work,
	/// so it sits behind the dedicated-server guard (D-35/D-55) and reuses an existing Kelp Curtain dust
	/// rather than creating a new dust class (D-51).
	/// </summary>
	private void UpdateWaterTrail()
	{
		if (Main.dedServ || !Main.rand.NextBool(12))
		{
			return;
		}

		int dust = Dust.NewDust(NPC.Center, 0, 0, ModContent.DustType<KelpWaterDrop>(), 0f, 0f);
		Main.dust[dust].noGravity = true;
		Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
	}

	/// <summary>
	/// 在水里成群 ... 刷新, and only ever inside Yggdrasil (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c>
	/// returns early outside the subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// The water test is the design's own 在水里 condition (Pitfall 6).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The conservative water weight, or <c>0f</c> outside the design's context.</returns>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		if (!spawnInfo.Water)
		{
			return 0f;
		}

		return KelpCurtainSpawnConditions.WaterWeight;
	}
}
