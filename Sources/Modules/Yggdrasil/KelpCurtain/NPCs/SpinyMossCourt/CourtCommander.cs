using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt;

/// <summary>
/// 王庭号令者（Court Commander）——刺苔庭园（Spiny Moss Court）王庭的号令单位。
/// Design row: 王庭号令者, region h1 block YCYwdeYPEomoMVxotFOcE8gZnUh (刺苔庭园),
/// heading block id M0h6dc7myo8FQTxx1nQcYb2Xn4b, stats table YVgWdPMdToFnOTxZXKJcGgDqnDf,
/// behaviour block id PQU5dPXNQo1pnyxAWWpchZ5wnkg, drop block id OB6sdTaOPoUc70x3zeWcKrHzn2b.
/// <para>
/// 生成权重较低，每次生成时与初次使用权杖时，如果身边没有枯木活化士兵，生成1~3个随机品种枯木活化士兵: the commander
/// spawns at a low land weight, summons 1-3 randomly chosen soldier variants when it spawns and once per aggro entry
/// on the first staff use (only while no soldier is within <see cref="SearchRadiusTiles"/>), wanders aimlessly
/// while unaggroed, raises its staff when a player enters its range, and then keeps at least
/// <see cref="RetreatTiles"/> tiles from that player while still dealing contact damage. The summon is limited to
/// one per aggro entry by the named <see cref="HasSummonedThisAggro"/> flag, and it is created with
/// <c>NPC.NewNPC</c> on the authoritative side only (D-55).
/// </para>
/// <para>
/// D-46 BLOCKER - the Spiny Moss Court morale/command system does not exist. The design's 使附近所有的活化士兵战斗意志
/// 高涨 buff application (意志高涨的活化士兵防御力+4, 攻击+35%, 移速+15%), its break-on-commander-death rule and the
/// morale-gated 干涸心脏 drop are NOT implemented: only the staff-raise the design describes as the visible tell is
/// built. The affected files are the four soldier variants this class summons
/// (<c>AnimatedWitherbarkSoldier.cs</c>, <c>AnimatedWitherbarkSoldierRanged.cs</c>,
/// <c>AnimatedWitherbarkSoldierSpell.cs</c>, <c>AnimatedWitherbarkHound.cs</c>), which expose their defence/damage/
/// speed as named members so a later phase applies the buff without reworking any AI (04-DEVIATIONS.md sections 7, 13).
/// </para>
/// <para>
/// Approved artwork is missing from the repository, so the class requests the shared
/// <c>Commons.ModAsset.White_Mod</c> fallback. The <c>SpinyMossCourt</c> region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so the D-49 migration is adding <c>CourtCommander.png</c> beside
/// this .cs and deleting the <c>Texture</c> override.
/// </para>
/// </summary>
public class CourtCommander : ModNPC
{
	/// <summary>
	/// 生成权重较低: the low land weight, below the D-54 land band because the design makes the commander rarer than
	/// the courtyard's soldier units (04-DEVIATIONS.md section 10).
	/// </summary>
	private const float CommanderWeight = 0.5f;

	/// <summary>漫无目的地游荡: the slow patrol pace used while the commander has no aggro (D-54 default).</summary>
	private const float WanderSpeed = 1f;

	/// <summary>The pace used while keeping the design's safe distance (D-54 default).</summary>
	private const float RetreatSpeed = 2.2f;

	/// <summary>
	/// 当玩家进入仇恨范围后: the range at which the commander raises its staff. The design gives no number, so a
	/// conservative courtyard radius is used (D-54 default).
	/// </summary>
	private const float AggroRange = 30f * 16f;

	/// <summary>
	/// 高举权杖: how long the staff stays raised, which is the design's visible tell (D-54 default).
	/// </summary>
	private const int StaffRaiseFrames = 60;

	/// <summary>
	/// 尝试与玩家保持安全距离（不低于10格）: exactly the design's ten tiles.
	/// </summary>
	private const float RetreatTiles = 10f;

	/// <summary>
	/// The tolerance around <see cref="RetreatTiles"/> so the commander does not oscillate on the boundary
	/// (D-54 default).
	/// </summary>
	private const float RetreatSlackTiles = 2f;

	/// <summary>
	/// 如果身边没有枯木活化士兵: the commander summons only while fewer than this many soldiers stand within
	/// <see cref="SearchRadiusTiles"/>.
	/// </summary>
	private const int MinSoldiersNearby = 1;

	/// <summary>生成1~3个随机品种枯木活化士兵: the design's own lower bound.</summary>
	private const int SummonMin = 1;

	/// <summary>生成1~3个随机品种枯木活化士兵: the design's own upper bound.</summary>
	private const int SummonMax = 3;

	/// <summary>
	/// 身边: the radius the "are any soldiers near me" test uses. The design gives no number, so a conservative
	/// courtyard radius is used (D-54 default).
	/// </summary>
	private const float SearchRadiusTiles = 30f;

	/// <summary>How far from the commander a summoned soldier appears, in pixels (D-54 default).</summary>
	private const float SummonSpreadPixels = 6f * 16f;

	/// <summary>
	/// How long the commander keeps spacing itself from a player who has left its range before it gives up the
	/// aggro and returns to wandering (D-54 default).
	/// </summary>
	private const int AggroGiveUpFrames = 180;

	/// <summary>
	/// The three states of the design's behaviour: 漫无目的地游荡, 高举权杖 (which is also the summon moment) and
	/// 尝试与玩家保持安全距离（不低于10格）.
	/// </summary>
	private enum CourtCommanderState
	{
		Wandering = 0,
		Rallying = 1,
		KeepingDistance = 2,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private CourtCommanderState State
	{
		get => (CourtCommanderState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames the staff has been raised.</summary>
	private int RallyTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[1]</c>: the frames the aggro has been unclaimed.</summary>
	private int DistanceTimer
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[2]</c>: 1 once this aggro entry has already used the design's summon, so
	/// 初次使用权杖时 can never summon twice for one aggro (the flag the design's wording requires).
	/// </summary>
	private int HasSummonedThisAggro
	{
		get => (int)NPC.localAI[2];
		set => NPC.localAI[2] = value;
	}

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	// Approved artwork is missing from the repository (D-48). The shared fallback keeps the class loadable,
	// and the beside-.png this class's region folder expects arrives with the D-49 migration.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		// No approved sprite exists, so there is nothing to slice: the whole texture is one frame.
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// The design's commander table (YVgWdPMdToFnOTxZXKJcGgDqnDf) names no 免疫 cell at all, so this class
		// declares no specific debuff immunity.
	}

	public override void SetDefaults()
	{
		// The design's wander / staff-raise / keep-ten-tiles behaviour has no vanilla aiStyle analogue, so the class
		// owns its AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative humanoid commander box is used (D-54 default).
		NPC.width = 34;
		NPC.height = 50;

		NPC.lifeMax = 100; // 生命 100
		NPC.life = 100;
		NPC.damage = 25; // 伤害 25（接触）
		NPC.defDamage = 25;
		NPC.defense = 4; // 防御 4
		NPC.defDefense = 4;
		NPC.knockBackResist = 0.5f; // 击退抗性 50 -> 1 - 0.50
		NPC.value = 250; // 钱币（铜） 2银50铜 = 250 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is absent and 普通 is its 类型 cell, so no design rarity
		// exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this records the
		// empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 王庭号令者 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// Deterministic starting values, because SetDefaults runs on every side: no Main.rand is consumed here, so
		// the server and its clients agree before the first sync arrives.
		RallyTimer = 0;
		DistanceTimer = 0;
		HasSummonedThisAggro = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// 每次生成时 ... 如果身边没有枯木活化士兵，生成1~3个随机品种枯木活化士兵: the spawn-time half of the design's summon.
	/// It runs on the authoritative side only, because <c>NPC.NewNPC</c> must not be called on a multiplayer client
	/// (D-55).
	/// </summary>
	/// <param name="source">The entity source the spawn came from.</param>
	public override void OnSpawn(IEntitySource source)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		NPC.direction = Main.rand.NextBool() ? 1 : -1;
		NPC.spriteDirection = NPC.direction;
		TrySummonIfAlone();
		NPC.netUpdate = true;
	}

	/// <summary>
	/// The design row's 减伤 cell is absent (stats table YVgWdPMdToFnOTxZXKJcGgDqnDf), so no <c>FinalDamage</c>
	/// scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	/// <param name="modifiers">The hit modifiers being resolved.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// 高举权杖 when a player enters its range, then 尝试与玩家保持安全距离（不低于10格） while still dealing contact
	/// damage. Every written transition is authoritative and sets <c>NPC.netUpdate</c> (D-55).
	/// </summary>
	public override void AI()
	{
		switch (State)
		{
			case CourtCommanderState.Rallying:
				UpdateRallying();
				break;
			case CourtCommanderState.KeepingDistance:
				UpdateKeepingDistance();
				break;
			default:
				UpdateWandering();
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// 本身在没有仇恨的情况下只会漫无目的地游荡: the aimless patrol, with no player targeting other than the
	/// aggro-range test that raises the staff.
	/// </summary>
	private void UpdateWandering()
	{
		NPC.velocity.X = WanderSpeed * NPC.direction;

		if (NPC.collideX || !HasFloorAhead())
		{
			NPC.direction *= -1;
			NPC.velocity.X = WanderSpeed * NPC.direction;
		}

		if (TryGetPlayer(out _, out float distance) && distance <= AggroRange)
		{
			EnterRallying();
		}
	}

	/// <summary>
	/// 高举权杖，使附近所有的活化士兵战斗意志高涨: the staff goes up as the design's visible tell (drawn with an existing
	/// Kelp Curtain dust inside the dedicated-server guard, D-35/D-55). 每次生成时与初次使用权杖时, 如果身边没有枯木活化
	/// 士兵, 生成1~3个随机品种枯木活化士兵 - the summon fires once for this aggro entry, and only while no soldier is
	/// already near. The 意志高涨 buff application itself is the D-46 blocker and is deliberately absent.
	/// </summary>
	private void UpdateRallying()
	{
		NPC.velocity.X *= 0.9f;

		if (!Main.dedServ && Main.rand.NextBool(2))
		{
			Vector2 staffTip = NPC.Center + new Vector2(0f, -NPC.height * 0.75f);
			int dust = Dust.NewDust(staffTip, 8, 8, ModContent.DustType<WitherWoodDust>(), 0f, -1.2f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
		}

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		if (RallyTimer == 0 && HasSummonedThisAggro == 0)
		{
			// 初次使用权杖时: the one summon of this aggro entry.
			TrySummonIfAlone();
			HasSummonedThisAggro = 1;
			NPC.netUpdate = true;
		}

		RallyTimer++;
		if (RallyTimer >= StaffRaiseFrames)
		{
			State = CourtCommanderState.KeepingDistance;
			RallyTimer = 0;
			DistanceTimer = 0;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// 随后尝试与玩家保持安全距离（不低于10格），但是能造成接触伤害: steer away whenever the nearest player is inside
	/// <see cref="RetreatTiles"/>, close back in when the gap is too wide, and give the aggro up after
	/// <see cref="AggroGiveUpFrames"/> frames with no player inside the range. Contact damage stays the creature's
	/// own <c>defDamage</c>, so walking into it still hurts.
	/// </summary>
	private void UpdateKeepingDistance()
	{
		if (!TryGetPlayer(out Player target, out float distance) || distance > AggroRange)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				DistanceTimer++;
				if (DistanceTimer >= AggroGiveUpFrames)
				{
					EnterWandering();
				}
			}

			NPC.velocity.X *= 0.95f;
			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient && DistanceTimer != 0)
		{
			DistanceTimer = 0;
			NPC.netUpdate = true;
		}

		float retreat = RetreatTiles * 16f;
		float slack = RetreatSlackTiles * 16f;
		float direction = 0f;
		if (distance < retreat)
		{
			// 不低于10格: back away from the player.
			direction = NPC.Center.X >= target.Center.X ? 1f : -1f;
		}
		else if (distance > retreat + slack)
		{
			// Keep the player inside its own aggro range rather than letting the gap grow forever.
			direction = target.Center.X >= NPC.Center.X ? 1f : -1f;
		}

		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, direction * RetreatSpeed, 0.06f);
		if (NPC.velocity.Y == 0f && (NPC.collideX || !HasFloorAhead()))
		{
			NPC.velocity.Y = -5.5f;
		}
	}

	/// <summary>
	/// 如果身边没有枯木活化士兵，生成1~3个随机品种枯木活化士兵: count the soldier variants inside
	/// <see cref="SearchRadiusTiles"/> and, while none is there, create <see cref="SummonMin"/> to
	/// <see cref="SummonMax"/> randomly chosen variants through <c>NPC.NewNPC</c>. The whole body runs on the
	/// authoritative side only, because <c>NPC.NewNPC</c> must never be called on a multiplayer client (D-55).
	/// </summary>
	private void TrySummonIfAlone()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		if (CountSoldiersNearby() >= MinSoldiersNearby)
		{
			return;
		}

		int count = Main.rand.Next(SummonMin, SummonMax + 1);
		for (int i = 0; i < count; i++)
		{
			Vector2 position = NPC.Center + new Vector2(
				Main.rand.NextFloat(-SummonSpreadPixels, SummonSpreadPixels),
				Main.rand.NextFloat(-SummonSpreadPixels, 0f));

			// X is the spawn centre and Y the spawn feet, exactly as the engine's own summons pass them.
			NPC.NewNPC(NPC.GetSource_FromAI(), (int)position.X, (int)position.Y, RandomSoldierVariantType());
		}

		NPC.netUpdate = true;
	}

	/// <summary>
	/// The four soldier variant types this commander may summon, resolved through <c>ModContent.NPCType&lt;...&gt;</c>
	/// so a rename of any class is caught at compile time (never a string or name-based lookup).
	/// </summary>
	/// <returns>The index in the summon order.</returns>
	private int RandomSoldierVariantType()
	{
		return Main.rand.Next(4) switch
		{
			0 => ModContent.NPCType<AnimatedWitherbarkSoldier>(),
			1 => ModContent.NPCType<AnimatedWitherbarkSoldierRanged>(),
			2 => ModContent.NPCType<AnimatedWitherbarkSoldierSpell>(),
			_ => ModContent.NPCType<AnimatedWitherbarkHound>(),
		};
	}

	/// <summary>
	/// 身边没有枯木活化士兵: how many of the four soldier variants stand inside
	/// <see cref="SearchRadiusTiles"/> right now.
	/// </summary>
	/// <returns>The number of nearby soldier variants.</returns>
	private int CountSoldiersNearby()
	{
		int soldier = ModContent.NPCType<AnimatedWitherbarkSoldier>();
		int ranged = ModContent.NPCType<AnimatedWitherbarkSoldierRanged>();
		int spell = ModContent.NPCType<AnimatedWitherbarkSoldierSpell>();
		int hound = ModContent.NPCType<AnimatedWitherbarkHound>();
		float searchRadius = SearchRadiusTiles * 16f;
		int found = 0;
		for (int i = 0; i < Main.maxNPCs; i++)
		{
			NPC other = Main.npc[i];
			if (!other.active || other.life <= 0 || other.whoAmI == NPC.whoAmI)
			{
				continue;
			}

			if (other.type != soldier && other.type != ranged && other.type != spell && other.type != hound)
			{
				continue;
			}

			if (Vector2.Distance(NPC.Center, other.Center) <= searchRadius)
			{
				found++;
			}
		}

		return found;
	}

	/// <summary>True when the tile just below and ahead of the leading edge is solid, or the world edge is reached.</summary>
	/// <returns>True when the commander can keep walking.</returns>
	private bool HasFloorAhead()
	{
		int checkTileX = (int)((NPC.Center.X + (NPC.width / 2f + 8f) * NPC.direction) / 16f);
		int checkTileY = (int)((NPC.Bottom.Y + 8f) / 16f);
		if (!WorldGen.InWorld(checkTileX, checkTileY, 1))
		{
			return true;
		}

		return Main.tile[checkTileX, checkTileY].HasTile;
	}

	/// <summary>The nearest live player, used only by the aggro test and the spacing rule (D-55: never a camera value).</summary>
	/// <param name="player">The player the commander is aware of, or <c>null</c>.</param>
	/// <param name="distance">The distance to that player, or <see cref="float.MaxValue"/>.</param>
	/// <returns>True when a live player exists.</returns>
	private bool TryGetPlayer(out Player player, out float distance)
	{
		player = null;
		distance = float.MaxValue;
		NPC.TargetClosest(false);
		if (NPC.target < 0 || NPC.target >= Main.maxPlayers)
		{
			return false;
		}

		Player candidate = Main.player[NPC.target];
		if (candidate is null || !candidate.active || candidate.dead)
		{
			return false;
		}

		player = candidate;
		distance = Vector2.Distance(NPC.Center, candidate.Center);
		return true;
	}

	/// <summary>Keeps the facing in step with the direction of travel (the arriving art reads this).</summary>
	private void UpdateFacing()
	{
		if (MathF.Abs(NPC.velocity.X) > 0.05f)
		{
			NPC.direction = NPC.velocity.X > 0f ? 1 : -1;
		}

		NPC.spriteDirection = NPC.direction;
	}

	/// <summary>
	/// 当玩家进入仇恨范围后，高举权杖: enters the staff-raise state and re-arms the design's once-per-aggro summon,
	/// written on the authoritative side with an <c>NPC.netUpdate</c> (D-55).
	/// </summary>
	private void EnterRallying()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || State == CourtCommanderState.Rallying)
		{
			return;
		}

		State = CourtCommanderState.Rallying;
		RallyTimer = 0;
		DistanceTimer = 0;
		HasSummonedThisAggro = 0;
		NPC.velocity.X = 0f;
		NPC.netUpdate = true;
	}

	/// <summary>The return to 漫无目的地游荡 once the aggro is given up, written authoritatively (D-55).</summary>
	private void EnterWandering()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || State == CourtCommanderState.Wandering)
		{
			return;
		}

		State = CourtCommanderState.Wandering;
		RallyTimer = 0;
		DistanceTimer = 0;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 生成权重较低: the commander's low land weight inside the layer. Only ever inside Yggdrasil and inside the Kelp
	/// Curtain layer (BIO-06): <c>NPCSpawnManager.EditSpawnPool</c> returns early outside the subworld, so this
	/// per-creature gate is the real isolation, and <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the
	/// server-safe layer predicate because the hook runs in single player or on the server only, where the client
	/// camera is zero (D-52/D-55). The design's 刺苔庭园 is an above-water courtyard, so the land condition is its
	/// own 地表 reading (Pitfall 6).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The low land weight, or <c>0f</c> outside the design's context.</returns>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		if (!KelpCurtainSpawnConditions.IsDryLand(spawnInfo))
		{
			return 0f;
		}

		return CommanderWeight;
	}

	/// <summary>
	/// 掉落2~4 枯木碎块，50%概率掉落1 干涸心脏: neither 枯木碎块 nor 干涸心脏 has a ModItem in the repository, so no
	/// rule is written and no item type is referenced here (D-58, Pitfall 2): a
	/// <c>ModContent.ItemType&lt;X&gt;()</c> for an absent <c>X</c> would fail the whole mod build. The blockers live
	/// in 04-DEVIATIONS.md section 6 and in the biology matrix instead; the 干涸心脏 half also depends on the
	/// unimplemented morale/command system (D-46).
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
