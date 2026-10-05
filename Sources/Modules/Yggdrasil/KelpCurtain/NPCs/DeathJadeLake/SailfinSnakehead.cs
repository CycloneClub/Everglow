using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 帆鳍鳢（Sailfin Snakehead），亡碧湖（Death Jade Lake）水中的中立游猎鱼。
/// Design row: 帆鳍鳢, heading block id BJVjdHX2FoW4MLxOk6ocVw82nUd,
/// stats table BiiNdL85hoJC6axx5lbcy5Zsnnf, behavior block UUgpdyHQjopoSHx7wCzczqxwnPc,
/// drop block QLkTdqtHoowbKexZttGc71byn5e (D-28 full implementation).
/// 生命 80/140/200 (the first value is taken, the Phase 3 rule), 伤害 30, 防御 12, 击退抗性 20,
/// 钱币 5银 = 500. The design's 减伤 and 免疫 cells are empty.
/// <para>
/// <b>The aggro trigger is a conservative reading (D-54).</b> The design only says 获得仇恨时通过快速游动来撞击
/// and never names a trigger, so this class reads 获得仇恨 as "aggro on taking damage, refreshed by every
/// further hit and bounded by <see cref="AggroWindowFrames"/>" - a bounded window rather than a permanent
/// aggro, so the neutral creature cannot silently become a permanent aggressor (T-04-22). Plan 04-09 lists
/// this reading in 04-DEVIATIONS.md section 10.
/// </para>
/// <para>
/// <b>Foe rules.</b> 会远离蝾螈 and 主动攻击蛞蝓: a nearby 幽光蝾螈 (<see cref="GlowSalamander"/>) outranks
/// every other consideration and the creature retreats from it, while a nearby 水蛞蝓 (<see cref="RiverSlug"/>)
/// is its own prey and is rammed. Both species are named through <c>ModContent.NPCType&lt;...&gt;</c>, never a
/// string or name-based lookup. As 04-DEVIATIONS.md section 7 records for the whole phase, tML has no
/// NPC-versus-NPC damage hook the design could use, so the design's inter-creature hostility is modelled as
/// the target-and-charge behaviour and the contact damage between two creatures is not dealt.
/// </para>
/// <para>
/// <b>Loot blocker (D-58).</b> The design's 死亡后 33%概率掉落1 亡碧膏 names a material with no ModItem
/// anywhere in the repository, so no rule is written and no item type is referenced here.
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback (D-48). The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the migration is adding
/// SailfinSnakehead.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset
/// path is Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/SailfinSnakehead.png.
/// </para>
/// </summary>
public class SailfinSnakehead : ModNPC
{
	/// <summary>
	/// 获得仇恨: how long the creature stays hostile after it is damaged, in frames (5 s). The design gives no
	/// window, so this is the conservative reading recorded as a D-54 default in 04-DEVIATIONS.md section 10.
	/// </summary>
	public const int AggroWindowFrames = 300;

	/// <summary>闲置时匀速在水中游动: the constant cruise speed, in pixels per tick (D-54 default).</summary>
	private const float CruiseSpeed = 2.2f;

	/// <summary>获得仇恨时通过快速游动来撞击: the ram speed, in pixels per tick (D-54 default).</summary>
	private const float ChargeSpeed = 6.5f;

	/// <summary>会远离蝾螈: the retreat speed, in pixels per tick (D-54 default).</summary>
	private const float RetreatSpeed = 3.4f;

	/// <summary>A drift back toward the water when the creature is out of it, in pixels per tick (D-54 default).</summary>
	private const float ReturnToWaterSpeed = 1.6f;

	/// <summary>
	/// The recovery between two rams, in frames (D-54 default). Without it a charge would glue the creature to
	/// its target, so the design's 撞击 reads as repeated blows rather than a permanent push.
	/// </summary>
	private const int ChargeCooldownTicks = 40;

	/// <summary>会远离蝾螈: the range inside which a 幽光蝾螈 displaces every other consideration (D-54 default).</summary>
	private const float SalamanderFleeRange = 30f * 16f;

	/// <summary>主动攻击蛞蝓: the range of the 水蛞蝓 prey scan (D-54 default).</summary>
	private const float PreyRange = 40f * 16f;

	/// <summary>
	/// The leash range of the player charge, in pixels (D-54 default). The aggro window already bounds the
	/// charge in time; this keeps a provoked creature from being dragged out of its own lake across the map.
	/// </summary>
	private const float LeashRange = 50f * 16f;

	/// <summary>
	/// The three states of the design's behaviour (D-31): 闲置时匀速在水中游动, the 获得仇恨 charge, and the
	/// 会远离蝾螈 retreat. No vanilla <c>aiStyle</c> provides a neutral water creature with an avoid rule, so
	/// the class owns its <c>AI()</c>.
	/// </summary>
	private enum SailfinSnakeheadState
	{
		Cruising = 0,
		Charging = 1,
		Retreating = 2,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private SailfinSnakeheadState State
	{
		get => (SailfinSnakeheadState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over the synced <c>NPC.ai[1]</c>: the remaining frames of the 获得仇恨 window. It is a
	/// per-NPC timer and never a field on a <c>Player</c> (Pitfall 6). It must ride the synced array:
	/// every side reads it through <see cref="HasAggro"/> to choose its AI branch, and
	/// <c>NPC.localAI[]</c> never reaches a client (D-55).
	/// </summary>
	private int AggroTimer
	{
		get => (int)NPC.ai[1];
		set => NPC.ai[1] = value;
	}

	/// <summary>
	/// Named wrapper over the synced <c>NPC.ai[2]</c>: the frames left before the next 撞击 is allowed.
	/// <see cref="RamToward"/> reads it on every side to pick the swim speed, so it cannot live in the
	/// unsynchronised <c>NPC.localAI[]</c> (D-55).
	/// </summary>
	private int ChargeCooldown
	{
		get => (int)NPC.ai[2];
		set => NPC.ai[2] = value;
	}

	/// <summary>True while the design's 获得仇恨 window is open.</summary>
	private bool HasAggro => AggroTimer > 0;

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
		// The neutral cruise/charge/retreat cycle has no vanilla aiStyle analogue, so the class owns its
		// AI (D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative elongated aquatic box is used and is the first
		// value to revisit when the art arrives (D-54).
		NPC.width = 48;
		NPC.height = 22;

		// 生命 80/140/200: the progression takes its first value (the Phase 3 rule).
		NPC.lifeMax = 80;
		NPC.life = 80;

		NPC.damage = 30; // 伤害 30（All）
		NPC.defDamage = 30;
		NPC.defense = 12; // 防御 12
		NPC.defDefense = 12;
		NPC.knockBackResist = 0.8f; // 击退抗性 20 -> 1 - 0.20

		// 钱币（铜） 5银 = 500 copper, a 1:1 transpose (NPC.value is copper).
		NPC.value = 500;

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field (the API-name correction
		// recorded in 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		// 水生生物: it must not be pulled down by gravity. Tile collision stays on, as on the phase's other
		// aquatic creatures, so it cannot swim through the lake bed.
		NPC.noGravity = true;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 帆鳍鳢 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// Deterministic starting values, because SetDefaults runs on every side: no Main.rand is consumed
		// here, so the server and its clients agree before the first sync arrives.
		State = SailfinSnakeheadState.Cruising;
		AggroTimer = 0;
		ChargeCooldown = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The design's three behaviours: 闲置时匀速在水中游动 while nothing provokes it, 获得仇恨时通过快速游动来
	/// 撞击 while the aggro window is open (or while a 水蛞蝓 is in range), and 会远离蝾螈 whenever a 幽光蝾螈
	/// is close. Every state is derived from synced state on each side, while the writes stay authoritative
	/// (D-55).
	/// </summary>
	public override void AI()
	{
		bool inLiquid = IsInLiquid();
		NPC.noGravity = true;

		UpdateTimers();

		// Each candidate target is resolved with a pre-seeded local, so a short-circuited scan still leaves
		// its centre definite.
		Vector2 salamanderCenter = default;
		Vector2 preyCenter = default;
		Vector2 playerCenter = default;

		// 会远离蝾螈: a nearby 幽光蝾螈 outranks both the 蛞蝓 prey and the player.
		bool seeSalamander = TryGetNearestCreature(ModContent.NPCType<GlowSalamander>(), SalamanderFleeRange, out salamanderCenter);

		// 主动攻击蛞蝓: its own prey is attacked whether or not the player provoked it.
		bool seePrey = !seeSalamander && TryGetNearestCreature(ModContent.NPCType<RiverSlug>(), PreyRange, out preyCenter);

		// 获得仇恨时通过快速游动来撞击: the player is a target only while the bounded window is open.
		bool seePlayer = !seeSalamander && !seePrey && HasAggro && TryGetLeashedPlayer(out playerCenter);

		SailfinSnakeheadState state;
		if (seeSalamander)
		{
			state = SailfinSnakeheadState.Retreating;
		}
		else if (seePrey || seePlayer)
		{
			state = SailfinSnakeheadState.Charging;
		}
		else
		{
			state = SailfinSnakeheadState.Cruising;
		}

		if (State != state)
		{
			EnterState(state);
		}

		switch (state)
		{
			case SailfinSnakeheadState.Retreating:
				MoveAwayFrom(salamanderCenter, inLiquid);
				break;
			case SailfinSnakeheadState.Charging:
				RamToward(seePrey ? preyCenter : playerCenter, inLiquid);
				break;
			default:
				Cruise(inLiquid);
				break;
		}

		UpdateFacing();
		UpdateTrail();
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (behavior block UUgpdyHQjopoSHx7wCzczqxwnPc), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	/// <param name="modifiers">The hit's modifiers; deliberately untouched.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// 获得仇恨: the design names no trigger, so the conservative reading (D-54) is "aggro when damaged,
	/// refreshed by every further hit, bounded by <see cref="AggroWindowFrames"/>". The window is written
	/// from this documented on-hit hook - never from <see cref="ModifyIncomingHit"/>, which tML documents as
	/// being for <c>HitModifiers</c> properties only - and only on the authoritative side (D-55).
	/// </summary>
	/// <param name="hit">The resolved hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life > 0)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				// Every further hit refreshes the window rather than extending it without bound.
				AggroTimer = AggroWindowFrames;
				NPC.netUpdate = true;
			}

			return;
		}

		if (!Main.dedServ)
		{
			for (int i = 0; i < 8; i++)
			{
				int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<KelpWaterDrop>(), 0f, -0.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.3f);
			}
		}
	}

	/// <summary>
	/// 死亡后 33%概率掉落1 亡碧膏: 亡碧膏 has no ModItem in the repository, so no rule is written and no item
	/// type is referenced here (D-58, Pitfall 2). The blocker lives in 04-DEVIATIONS.md section 6.2 and in
	/// the biology matrix instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}

	/// <summary>
	/// Runs the two per-NPC timers on every side so the synced <c>NPC.ai[]</c> counters stay in step; only
	/// the authoritative side broadcasts the window closing (D-55). The 获得仇恨 window is the bound that
	/// keeps a provoked creature from staying hostile forever (T-04-22).
	/// </summary>
	private void UpdateTimers()
	{
		bool changed = false;
		if (AggroTimer > 0)
		{
			AggroTimer--;
			changed = AggroTimer == 0;
		}

		if (ChargeCooldown > 0)
		{
			ChargeCooldown--;
		}

		if (changed && Main.netMode != NetmodeID.MultiplayerClient)
		{
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// 闲置时匀速在水中游动: an untargeted creature swims at a constant speed along its facing, turning around
	/// at a wall instead of pushing into terrain forever. Nothing here reads a player, so the cruise never
	/// turns into a chase (T-04-22).
	/// </summary>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	private void Cruise(bool inLiquid)
	{
		if (NPC.collideX)
		{
			NPC.direction *= -1;
		}

		NPC.velocity.X = CruiseSpeed * NPC.direction;

		// 水生生物 out of the water: drift back down instead of hovering over the lake (D-54 default).
		NPC.velocity.Y = inLiquid ? 0f : ReturnToWaterSpeed;
	}

	/// <summary>
	/// 获得仇恨时通过快速游动来撞击: a fast ram toward the target while the cooldown allows it, and a normal
	/// speed close-in while it recovers. The cooldown is consumed on the authoritative side only (D-55).
	/// </summary>
	/// <param name="targetCenter">The point to ram.</param>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	private void RamToward(Vector2 targetCenter, bool inLiquid)
	{
		Vector2 direction = (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		float speed = ChargeCooldown > 0 ? CruiseSpeed : ChargeSpeed;
		NPC.velocity = Vector2.Lerp(NPC.velocity, direction * speed, inLiquid ? 0.08f : 0.05f);

		if (Main.netMode != NetmodeID.MultiplayerClient && ChargeCooldown <= 0)
		{
			ChargeCooldown = ChargeCooldownTicks;
			NPC.netUpdate = true;
		}
	}

	/// <summary>会远离蝾螈: swims (or drifts) directly away from the threat, at the retreat speed.</summary>
	/// <param name="threatCenter">The point to move away from.</param>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	private void MoveAwayFrom(Vector2 threatCenter, bool inLiquid)
	{
		Vector2 direction = (NPC.Center - threatCenter).SafeNormalize(Vector2.UnitX * -NPC.direction);
		Vector2 desired = new(direction.X * RetreatSpeed, inLiquid ? direction.Y * RetreatSpeed : -ReturnToWaterSpeed);
		NPC.velocity = Vector2.Lerp(NPC.velocity, desired, 0.08f);
	}

	/// <summary>
	/// The nearest live creature of the given type inside <paramref name="range"/>. The type comes from
	/// <c>ModContent.NPCType&lt;...&gt;</c>, so a rename of either species is caught at compile time (never a
	/// string or name-based lookup).
	/// </summary>
	/// <param name="type">The species' NPC type.</param>
	/// <param name="range">The search range, in pixels.</param>
	/// <param name="center">The found creature's centre.</param>
	/// <returns>True when a live creature of that type is inside the range.</returns>
	private bool TryGetNearestCreature(int type, float range, out Vector2 center)
	{
		center = default;
		float bestDistance = range;
		for (int i = 0; i < Main.maxNPCs; i++)
		{
			NPC other = Main.npc[i];
			if (!other.active || other.life <= 0 || other.whoAmI == NPC.whoAmI || other.type != type)
			{
				continue;
			}

			float distance = Vector2.Distance(NPC.Center, other.Center);
			if (distance < bestDistance)
			{
				bestDistance = distance;
				center = other.Center;
			}
		}

		return bestDistance < range;
	}

	/// <summary>
	/// The nearest live player, but only while it stays inside <see cref="LeashRange"/> (D-54 default: the
	/// design gives no leash length). Uses the entity's own player target, never a camera value (D-52/D-55).
	/// </summary>
	/// <param name="center">The player's centre.</param>
	/// <returns>True when a live player is inside the leash range.</returns>
	private bool TryGetLeashedPlayer(out Vector2 center)
	{
		NPC.TargetClosest(false);
		center = default;
		Player target = null;
		if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
		{
			target = Main.player[NPC.target];
		}

		if (target is null || !target.active || target.dead)
		{
			return false;
		}

		center = target.Center;
		return Vector2.Distance(NPC.Center, target.Center) <= LeashRange;
	}

	/// <summary>True when the creature's centre tile holds liquid, i.e. 在水中.</summary>
	/// <returns>True when the centre tile holds liquid.</returns>
	private bool IsInLiquid()
	{
		int tileX = (int)(NPC.Center.X / 16f);
		int tileY = (int)(NPC.Center.Y / 16f);
		if (!WorldGen.InWorld(tileX, tileY, 1))
		{
			return false;
		}

		return Main.tile[tileX, tileY].LiquidAmount > 0;
	}

	/// <summary>
	/// 在水中游动: a faint water trail so the swimmer reads as a creature under water. Client-only work, so it
	/// sits behind the dedicated-server guard (D-35/D-55) and reuses an existing Kelp Curtain dust rather
	/// than creating a new dust class (D-51).
	/// </summary>
	private void UpdateTrail()
	{
		if (Main.dedServ || !Main.rand.NextBool(10))
		{
			return;
		}

		int dust = Dust.NewDust(NPC.Center, 0, 0, ModContent.DustType<KelpWaterDrop>(), 0f, 0f);
		Main.dust[dust].noGravity = true;
		Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
	}

	/// <summary>
	/// Keeps the facing in step with the direction of travel. There is no approved sprite yet, so this only
	/// prepares the state the arriving art will read (the D-49 migration is art-only).
	/// </summary>
	private void UpdateFacing()
	{
		if (MathF.Abs(NPC.velocity.X) > 0.05f)
		{
			NPC.direction = NPC.velocity.X > 0f ? 1 : -1;
		}

		NPC.spriteDirection = NPC.direction;
	}

	/// <summary>
	/// The design's state transition, written only on the authoritative side with an <c>NPC.netUpdate</c>
	/// (D-55).
	/// </summary>
	/// <param name="state">The state to enter.</param>
	private void EnterState(SailfinSnakeheadState state)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = state;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 在水中刷新, and only ever inside Yggdrasil (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c> returns
	/// early outside the subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the hook
	/// runs in single player or on the server only, where the client camera is zero (D-52/D-55). The water
	/// test is the design's own 水生生物 condition (Pitfall 6).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The conservative open-water weight, or <c>0f</c> outside the design's context.</returns>
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
