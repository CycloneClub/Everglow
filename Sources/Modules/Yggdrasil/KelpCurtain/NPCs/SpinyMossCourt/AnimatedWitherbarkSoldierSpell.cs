using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using SubworldLibrary;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt;

/// <summary>
/// 枯木活化士兵（法术）（Animated Witherbark Soldier, Spell）——刺苔庭园（Spiny Moss Court）王庭的施法单位。
/// Design row: 枯木活化士兵, region h1 block YCYwdeYPEomoMVxotFOcE8gZnUh (刺苔庭园),
/// heading block id S4Jadlq0colTtbx8UagceDb7nCq, stats table NNe6dSQqxoxXllxT9PvcVmApn1c,
/// behaviour block ids BYhvdnXwrogK3Jx1z35cwgS7nhg / MGYedPrhioUFFdxP5XrcMpnxnMd,
/// drop block id KzhDde2Q7o3Te6xUDPsclRNqnCW.
/// <para>
/// This is a <b>sibling class</b> of the row whose <c>internal_name</c> is <c>AnimatedWitherbarkSoldier</c>
/// (the 近战 variant): the design gives 普通（法术）its own stat row (生命 55 / 伤害 24 / 防御 0 / 击退抗性 40 /
/// 免疫 中毒、困惑 / 1银50铜), and <c>SetDefaults</c> runs once per NPC type, so a stat variant must never be
/// faked by mutating <c>NPC.lifeMax</c> at runtime (OQ2, 04-DEVIATIONS.md section 2).
/// </para>
/// <para>
/// 法术敌怪会固定间隔使用一次法术，然后随机传送（原版法师AI），法术为向前喷洒三束受重力影响的粒子: the class
/// clones the vanilla caster <c>NPCID.DarkCaster</c> for its collider/sound bases and pins the design's 24 / 0
/// over them, then owns the mage loop so the design's own spell is what is cast - a fixed interval, three
/// <c>AnimatedWitherbarkSoldier_SpellBeam</c> projectiles sprayed forward with gravity, and then the vanilla
/// mage's random teleport. <c>NPC.aiStyle</c> is deliberately reset to <c>-1</c>: the vanilla caster AI fires the
/// engine's own caster projectile on its own cadence and cannot be made to spray the design's three beams, so
/// only the clone's bases are inherited (recorded in 04-DEVIATIONS.md section 10).
/// </para>
/// <para>
/// Approved artwork is missing from the repository, so the class requests the shared
/// <c>Commons.ModAsset.White_Mod</c> fallback. The <c>SpinyMossCourt</c> region subfolder is what tML's
/// default (namespace-derived) texture resolution expects, so the D-49 migration is adding
/// <c>AnimatedWitherbarkSoldierSpell.png</c> beside this .cs and deleting the <c>Texture</c> override.
/// </para>
/// <para>
/// D-46 BLOCKER - the Spiny Moss Court morale/command system does not exist, so the design's 意志高涨 bonus
/// (防御力+4, 攻击+35%, 移速+15%) and the 干涸心脏 morale-gated drop are NOT implemented. The morale seam is the
/// named members below, which a later phase raises without reworking this AI (04-DEVIATIONS.md sections 7, 13).
/// </para>
/// </summary>
public class AnimatedWitherbarkSoldierSpell : ModNPC
{
	/// <summary>
	/// 在默认情况下是中立状态: the proximity half of the provocation reading. The design names no number, so a
	/// conservative courtyard-guard radius is used (D-54 default, 04-DEVIATIONS.md section 10).
	/// </summary>
	private const float ProvokeRange = 10f * 16f;

	/// <summary>漫无目的地游荡: the slow patrol pace used while the creature is still 中立 (D-54 default).</summary>
	private const float WanderSpeed = 0.8f;

	/// <summary>固定间隔使用一次法术: the ticks between two casts (D-54 default; the design gives no number).</summary>
	private const int CastInterval = 150;

	/// <summary>
	/// 向前喷洒三束受重力影响的粒子: exactly three beams per cast, sprayed into a forward cone.
	/// </summary>
	private const int SpellBeamCount = 3;

	/// <summary>The half-angle of the three-beam spray, in radians (D-54 default).</summary>
	private const float SpellSpreadRadians = 0.20f;

	/// <summary>向前喷洒: the beams' launch speed (D-54 default).</summary>
	private const float SpellBeamSpeed = 9f;

	/// <summary>The distance at which the provoked state falls back to 中立 (D-54 default).</summary>
	private const float LeashRange = 70f * 16f;

	/// <summary>随机传送: the horizontal radius of the vanilla mage's teleport search, in tiles (D-54 default).</summary>
	private const int TeleportRangeTiles = 20;

	/// <summary>随机传送: the vertical radius of the teleport search, in tiles (D-54 default).</summary>
	private const int TeleportVerticalTiles = 8;

	/// <summary>随机传送: how many candidate tiles are tried before the creature stays where it is (D-54 default).</summary>
	private const int TeleportAttempts = 40;

	/// <summary>The free tiles a teleport destination needs above its floor.</summary>
	private const int ClearanceTiles = 3;

	/// <summary>
	/// The states of the design's mage loop: the 中立 patrol, the fixed-interval cast, and the 随机传送 that
	/// follows every cast (原版法师AI).
	/// </summary>
	private enum AnimatedWitherbarkSoldierSpellState
	{
		Patrolling = 0,
		Casting = 1,
		Teleporting = 2,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private AnimatedWitherbarkSoldierSpellState State
	{
		get => (AnimatedWitherbarkSoldierSpellState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the ticks left before the next cast.</summary>
	private int CastTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>
	/// The neutral flag, kept in a named wrapper over the synced <c>NPC.ai[1]</c> (1 once provoked, 0 while
	/// the design's 中立 default still holds). It must ride the synced array: every side reads it through
	/// <see cref="IsNeutral"/> to choose its AI branch, and <c>NPC.localAI[]</c> never reaches a client
	/// (D-55).
	/// </summary>
	private int ProvokedFlag
	{
		get => (int)NPC.ai[1];
		set => NPC.ai[1] = value;
	}

	/// <summary>
	/// 意志高涨 防御力+4: the morale/command seam (D-46). The system that would raise this does not exist, so
	/// it stays at its neutral value (0) and a later phase raises it without reworking this AI.
	/// </summary>
	public float MoraleDefenseBonus;

	/// <summary>意志高涨 攻击+35%: the damage multiplier a later morale system sets. The neutral value is 1.</summary>
	public float MoraleDamageScale = 1f;

	/// <summary>意志高涨 移速+15%: the speed multiplier a later morale system sets. The neutral value is 1.</summary>
	public float MoraleSpeedScale = 1f;

	/// <summary>
	/// A named neutral-state member so a later morale/command system can flip the creature and read the
	/// design's 中立 default (D-46).
	/// </summary>
	public bool IsNeutral => ProvokedFlag == 0;

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

		// 免疫 中毒、困惑 (the design row's 免疫 cell for this variant): exactly these two immunities.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override void SetDefaults()
	{
		// 原版法师AI: the vanilla caster's bases are inherited, but its AI is not - the engine's caster fires its
		// own projectile on its own cadence, so the design's three-beam spray and the design's 中立 default are
		// owned locally (D-31; 04-DEVIATIONS.md section 10).
		NPC.CloneDefaults(NPCID.DarkCaster);
		NPC.aiStyle = -1;

		// The clone supplies the caster's collider; a conservative humanoid soldier box then overrides it
		// (D-54 default, because no sprite exists to measure).
		NPC.width = 30;
		NPC.height = 46;

		NPC.lifeMax = 55; // 生命 55
		NPC.life = 55;
		NPC.damage = 24; // 伤害 24
		NPC.defDamage = 24;
		NPC.defense = 0; // 防御 0
		NPC.defDefense = 0;
		NPC.knockBackResist = 0.6f; // 击退抗性 40 -> 1 - 0.40
		NPC.value = 150; // 钱币（铜） 1银50铜 = 150 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 枯木活化士兵 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter. The clone
		// above may have set a catch item, so it is explicitly cleared.
		NPC.catchItem = 0;

		// Deterministic starting values, because SetDefaults runs on every side: no Main.rand is consumed here,
		// so the server and its clients agree before the first sync arrives.
		CastTimer = CastInterval;
		ProvokedFlag = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The provoked-stat re-assertion and the morale seam. 意志高涨 (D-46) is a blocker, so the morale members
	/// keep their neutral values and this is a no-op today; it exists so a later morale/command system can raise
	/// defence and damage without this class being reworked, and so the defence is restored from the bases on
	/// every side (<c>NPC.defense</c> is not part of the NPC net message, the <c>MossyThornTurtle</c> precedent).
	/// </summary>
	public override void PostAI()
	{
		NPC.defense = NPC.defDefense + (int)MoraleDefenseBonus;
		NPC.damage = (int)(NPC.defDamage * MoraleDamageScale);
	}

	/// <summary>
	/// The design's mage loop, and the 中立 default: while unprovoked the creature only patrols, so no spell is
	/// cast and no teleport happens before the design's own 中立 is broken.
	/// </summary>
	public override void AI()
	{
		if (IsNeutral)
		{
			UpdatePatrolling();
			UpdateFacing();
			return;
		}

		switch (State)
		{
			case AnimatedWitherbarkSoldierSpellState.Casting:
				UpdateCasting();
				break;
			case AnimatedWitherbarkSoldierSpellState.Teleporting:
				UpdateTeleporting();
				break;
			default:
				UpdateWaiting();
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (stats table NNe6dSQqxoxXllxT9PvcVmApn1c), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	/// <param name="modifiers">The hit modifiers being resolved.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// Damage taken is the primary provocation reading, so a non-lethal hit breaks the design's 中立. The
	/// transition is written on the authoritative side only, with an <c>NPC.netUpdate</c> (D-55).
	/// </summary>
	/// <param name="hit">The resolved hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0)
		{
			// Death dust only, and only on a client: the dedicated server has no graphics access (D-35/D-55).
			if (!Main.dedServ)
			{
				for (int i = 0; i < 6; i++)
				{
					int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<WitherWoodDust>(), 2 * hit.HitDirection, -2f);
					if (Main.rand.NextBool())
					{
						Main.dust[dust].noGravity = true;
						Main.dust[dust].scale = 1.2f * NPC.scale;
					}
					else
					{
						Main.dust[dust].scale = 0.7f * NPC.scale;
					}
				}
			}

			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			ProvokedFlag = 1;
			State = AnimatedWitherbarkSoldierSpellState.Patrolling;
			CastTimer = CastInterval;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// The idle patrol of the design's 中立 default: a slow walk that reverses at a wall or a ledge, with no
	/// player targeting at all until the design's 中立 is broken.
	/// </summary>
	private void UpdatePatrolling()
	{
		if (TryGetPlayer(out _, out float distance) && distance <= ProvokeRange)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				ProvokedFlag = 1;
				State = AnimatedWitherbarkSoldierSpellState.Patrolling;
				CastTimer = CastInterval;
				NPC.netUpdate = true;
			}

			return;
		}

		NPC.velocity.X = WanderSpeed * NPC.direction;

		if (NPC.collideX || !HasFloorAhead())
		{
			NPC.direction *= -1;
			NPC.velocity.X = WanderSpeed * NPC.direction;
		}
	}

	/// <summary>
	/// 固定间隔使用一次法术: hold position, count the interval on the authoritative side only and hand over to the
	/// cast when it expires (D-55).
	/// </summary>
	private void UpdateWaiting()
	{
		NPC.velocity.X *= 0.9f;

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		CastTimer--;
		if (CastTimer > 0)
		{
			return;
		}

		State = AnimatedWitherbarkSoldierSpellState.Casting;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 向前喷洒三束受重力影响的粒子: spawns exactly <see cref="SpellBeamCount"/>
	/// <c>AnimatedWitherbarkSoldier_SpellBeam</c> projectiles into a forward cone from the creature's own AI source,
	/// on the authoritative side only (D-55), then moves on to the vanilla mage's 随机传送.
	/// </summary>
	private void UpdateCasting()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			Vector2 forward = Vector2.UnitX * NPC.direction;
			if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
			{
				Player target = Main.player[NPC.target];
				if (target.active && !target.dead)
				{
					forward = (target.Center - NPC.Center).SafeNormalize(forward);
				}
			}

			float baseRotation = forward.ToRotation();
			for (int i = 0; i < SpellBeamCount; i++)
			{
				// Three evenly spread rays of the forward cone: -spread, 0, +spread.
				float offset = SpellSpreadRadians * (i - (SpellBeamCount - 1) / 2f);
				Vector2 velocity = Vector2.UnitX.RotatedBy(baseRotation + offset) * SpellBeamSpeed;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<AnimatedWitherbarkSoldier_SpellBeam>(), NPC.damage, 0f);
			}

			State = AnimatedWitherbarkSoldierSpellState.Teleporting;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// 然后随机传送（原版法师AI）: pick a standable tile near the prey and move there, then wait out the next
	/// <see cref="CastInterval"/>. The search consumes randomness and moves the creature, so it runs on the
	/// authoritative side only, with an <c>NPC.netUpdate</c> (D-55), and the teleport dust is client-only (D-35).
	/// </summary>
	private void UpdateTeleporting()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		if (TryGetPlayer(out Player target, out _))
		{
			TeleportNear(target);
		}

		State = AnimatedWitherbarkSoldierSpellState.Patrolling;
		CastTimer = CastInterval;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// The vanilla mage's random reposition: up to <see cref="TeleportAttempts"/> candidate tiles around the prey
	/// are tested for a standable spot, and the creature stays where it is when none works - it never teleports
	/// into terrain.
	/// </summary>
	/// <param name="target">The prey the teleport is centred on.</param>
	private void TeleportNear(Player target)
	{
		int centerTileX = (int)(target.Center.X / 16f);
		int centerTileY = (int)(target.Center.Y / 16f);
		for (int attempt = 0; attempt < TeleportAttempts; attempt++)
		{
			int tileX = centerTileX + Main.rand.Next(-TeleportRangeTiles, TeleportRangeTiles + 1);
			int tileY = centerTileY + Main.rand.Next(-TeleportVerticalTiles, TeleportVerticalTiles + 1);
			if (!IsStandableSpot(tileX, tileY))
			{
				continue;
			}

			if (!Main.dedServ)
			{
				for (int i = 0; i < 12; i++)
				{
					int dust = Dust.NewDust(new Vector2(tileX * 16f, tileY * 16f), 16, 16, ModContent.DustType<WitherWoodDust>(), 0f, 0f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
				}
			}

			// The candidate tile is the lowest free tile of the clearance stack, so centring the body on
			// it would bury the feet 15 px into the solid floor IsStandableSpot required. Bottom-align
			// instead: the feet rest on the floor the check validated, inside the cleared stack above it.
			NPC.Center = new Vector2(tileX * 16f + 8f, (tileY + 1) * 16f - NPC.height / 2f);
			NPC.velocity = Vector2.Zero;
			NPC.netUpdate = true;
			return;
		}
	}

	/// <summary>
	/// True when a tile can hold a humanoid: a solid floor directly beneath it and
	/// <see cref="ClearanceTiles"/> free tiles above it (this tile and the two above it). The teleport
	/// bottom-aligns the body on that floor.
	/// </summary>
	/// <param name="tileX">The candidate tile X.</param>
	/// <param name="tileY">The candidate tile Y.</param>
	/// <returns>True when the spot is standable.</returns>
	private bool IsStandableSpot(int tileX, int tileY)
	{
		if (!WorldGen.InWorld(tileX, tileY, 2))
		{
			return false;
		}

		if (!Main.tile[tileX, tileY + 1].HasTile)
		{
			return false;
		}

		for (int offset = 0; offset < ClearanceTiles; offset++)
		{
			if (Main.tile[tileX, tileY - offset].HasTile)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>True when the tile just below and ahead of the leading edge is solid, or the world edge is reached.</summary>
	/// <returns>True when the creature can keep walking.</returns>
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

	/// <summary>The nearest live player, used only by the provocation test and the cast's aim.</summary>
	/// <param name="player">The player the creature is aware of, or <c>null</c>.</param>
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
	/// The courtyard unit's land spawn gate. The design gives no per-variant weight and makes the 近战 variant the
	/// most common one, so this variant returns the low end of the D-54 land band. Only ever inside Yggdrasil and
	/// inside the Kelp Curtain layer (BIO-06): <c>NPCSpawnManager.EditSpawnPool</c> returns early outside the
	/// subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the hook runs
	/// in single player or on the server only, where the client camera is zero (D-52/D-55). The design's 刺苔庭园
	/// is an above-water courtyard, so the land condition is its own 地表 reading (Pitfall 6).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The land weight, or <c>0f</c> outside the design's context.</returns>
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

		return 0.75f;
	}

	/// <summary>
	/// 除了犬，均掉落1~2 枯木碎块 ... 干涸心脏: neither 枯木碎块 nor 干涸心脏 has a ModItem in the repository, so
	/// no rule is written and no item type is referenced here (D-58, Pitfall 2): a
	/// <c>ModContent.ItemType&lt;X&gt;()</c> for an absent <c>X</c> would fail the whole mod build. Only the 犬
	/// variant wires a drop. The blockers live in 04-DEVIATIONS.md section 6 and in the biology matrix instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
