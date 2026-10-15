using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Accessories;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 放射虫（Radiolarian），亡碧湖（Death Jade Lake）浅水区的远程射手：在水中持续发射水弹，
/// 仇恨目标贴近到四格时改成一次穿身冲刺，随后拉开距离回到远程。
/// Design row: 放射虫, heading block id Z1hzdRHyToqCUHxN6qScwKeGnVd,
/// behavior block TdwNdpn2XoFla9x8OUKc8AQanrd, drop block PAord6A0NomliPxRzQlcw0wEnMc
/// (D-28 full implementation).
/// Approved artwork is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the migration is adding
/// Radiolarian.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset
/// path is Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/Radiolarian.png.
/// </summary>
public class Radiolarian : ModNPC
{
	// --- the design's own numbers ------------------------------------------------------------------

	/// <summary>
	/// 当仇恨目标靠近时（4格）: the range, in tiles, at which the creature stops firing and commits to
	/// the dash. The design's own number, named so no bare literal sits in the comparison.
	/// </summary>
	public const float DashTriggerTiles = 4f;

	/// <summary>冲刺AI的冷却为300帧: the design's own dash cooldown.</summary>
	public const int DashCooldownFrames = 300;

	/// <summary>伤害 35（远程）: the value the water bolt carries (the melee 45 stays on <c>NPC.damage</c>).</summary>
	private const int RangedDamage = 35;

	/// <summary>Pixels per tile, used to turn <see cref="DashTriggerTiles"/> into a squared pixel range.</summary>
	private const float PixelsPerTile = 16f;

	/// <summary>
	/// The squared pixel form of <see cref="DashTriggerTiles"/>, so the trigger compares distances
	/// without a square root (no client value is read while doing it, D-55).
	/// </summary>
	private const float DashTriggerRangeSquared = DashTriggerTiles * DashTriggerTiles * PixelsPerTile * PixelsPerTile;

	// --- conservative defaults (the design gives no number; recorded in 04-DEVIATIONS.md section 10)

	/// <summary>在水中时连续发射水弹: the gap between water bolts (D-54 default).</summary>
	private const int FireInterval = 60;

	/// <summary>水中持续发射: the range it stops closing at and opens fire from (D-54 default).</summary>
	private const float FireRange = 32f * 16f;

	/// <summary>The leash beyond which the target is given up and the creature drifts back (D-54 default).</summary>
	private const float GiveUpFactor = 1.6f;

	/// <summary>水上/水下的慢速巡游速度, in pixels per tick (D-54 default).</summary>
	private const float CruiseSpeed = 1.6f;

	/// <summary>尝试冲刺撞击穿过目标: the dash speed, in pixels per tick (D-54 default).</summary>
	private const float DashSpeed = 10f;

	/// <summary>冲刺撞击穿过目标: the dash's own duration, in ticks (D-54 default).</summary>
	private const int DashDuration = 24;

	/// <summary>然后拉开距离后重新使用远程: the disengage phase's duration and speed (D-54 defaults).</summary>
	private const int DisengageDuration = 45;

	/// <summary>拉开距离: the retreat speed used during <see cref="RadiolarianState.Disengaging"/> (D-54 default).</summary>
	private const float DisengageSpeed = 4.2f;

	/// <summary>The water bolt's launch speed, in pixels per tick (D-54 default).</summary>
	private const float WaterBoltSpeed = 7.5f;

	/// <summary>Out of water the creature sinks back into the lake (D-54 default).</summary>
	private const float SinkSpeed = 2.4f;

	/// <summary>
	/// The bounded window, in tiles, of the shallow-water probe: the liquid column above the spawn tile
	/// must reach its dry surface inside this many tiles for the spawn to count as 浅水区.
	/// </summary>
	private const int MaxShallowScanTiles = 16;

	/// <summary>
	/// The four states of the design's behaviour (D-31): the drifting range-keeping swim, the sustained
	/// 连续发射水弹 window, the 冲刺撞击穿过目标 itself and the 拉开距离 that follows it. No vanilla
	/// <c>aiStyle</c> provides this ranged-then-charge aquatic pattern, so the class owns its <c>AI()</c>.
	/// </summary>
	private enum RadiolarianState
	{
		Ranging = 0,
		Firing = 1,
		Dashing = 2,
		Disengaging = 3,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private RadiolarianState State
	{
		get => (RadiolarianState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames left before the next water bolt.</summary>
	private int FireTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[1]</c>: the design's 300-frame dash cooldown.</summary>
	private int DashCooldown
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[2]</c>: one timer for both movement phases - the dash's
	/// remaining ticks while <see cref="RadiolarianState.Dashing"/> and the retreat's remaining ticks
	/// while <see cref="RadiolarianState.Disengaging"/>.
	/// </summary>
	private int ManeuverTimer
	{
		get => (int)NPC.localAI[2];
		set => NPC.localAI[2] = value;
	}

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	// Approved artwork is missing from the repository (D-48). The shared fallback keeps the class
	// loadable, and the beside-.png the phase's gate asserts arrives with the D-49 migration.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		// No approved sprite exists, so there is nothing to slice: the whole texture is one frame.
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// 免疫 困惑 (the design row's 免疫 cell): exactly this one immunity.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override void SetDefaults()
	{
		// The ranged-then-charge pattern has no vanilla aiStyle analogue, so the class owns a small
		// custom AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative 大型水生虫 hit box is used and recorded as a
		// D-54 default in 04-DEVIATIONS.md section 10.
		NPC.width = 52;
		NPC.height = 34;

		NPC.lifeMax = 180; // 生命 180/300/500 -> the first value (the Phase 3 rule)
		NPC.life = 180;

		// 伤害 35（远程）45（近战）: the melee value is the contact damage, while the 35 travels on the
		// projectile. PostAI re-asserts the melee value from the synced state.
		NPC.damage = 45;
		NPC.defDamage = 45;

		NPC.defense = 15; // 防御 15
		NPC.defDefense = 15;
		NPC.knockBackResist = 0.2f; // 击退抗性 80 -> 1 - 0.80
		NPC.value = 1000; // 钱币（铜） 10银 = 1000 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, section 10).
		NPC.rarity = ItemRarityID.White;

		// 水生生物: the water column carries it, so gravity is released (re-applied in AI(), D-55).
		NPC.noGravity = true;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 放射虫 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// Deterministic starting values, because SetDefaults runs on every side: no Main.rand is consumed
		// here, so the server and its clients agree before the first sync arrives.
		FireTimer = FireInterval;
		DashCooldown = 0;
		ManeuverTimer = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// 伤害 35（远程）45（近战）: the design's contact damage belongs to the melee branch, and the state
	/// lives in the synced <c>NPC.ai[0]</c>, so both the server and every client restore the melee value
	/// here (the <c>MossyThornTurtle</c> precedent) instead of relying on the unsynced damage field.
	/// </summary>
	public override void PostAI()
	{
		NPC.damage = NPC.defDamage;

		// 结构抽象的大型水生虫类: a faint water trail. Client-only work, so it sits behind the
		// dedicated-server guard (D-35/D-55) and reuses an existing Kelp Curtain dust rather than
		// creating a new dust class (D-51).
		if (Main.dedServ || !IsInLiquid())
		{
			return;
		}

		if (Main.rand.NextBool(5))
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<KelpWaterDrop>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1.1f);
		}
	}

	/// <summary>
	/// 减伤 10 (the design row's 减伤 cell): every incoming hit is scaled to 90%. Applied through
	/// <c>FinalDamage</c> so it composes with the engine's own defence and difficulty scaling instead of
	/// competing with it.
	/// </summary>
	/// <param name="modifiers">The hit modifiers being resolved.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
		modifiers.FinalDamage *= 0.9f; // 减伤 10
	}

	/// <summary>
	/// The state machine runner. The nearest player is the design's 仇恨目标 and every distance is
	/// computed from the synced <c>NPC.Center</c> and that player's centre, never from a camera or screen
	/// value (D-55).
	/// </summary>
	public override void AI()
	{
		NPC.TargetClosest(false);
		Player target = null;
		if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
		{
			target = Main.player[NPC.target];
		}

		bool hasTarget = target is not null && target.active && !target.dead;
		Vector2 targetCenter = hasTarget ? target.Center : NPC.Center;
		float distance = hasTarget ? Vector2.Distance(NPC.Center, targetCenter) : float.MaxValue;

		// 当仇恨目标靠近时（4格）: the squared form, so the trigger needs no square root.
		float distanceSquared = hasTarget ? Vector2.DistanceSquared(NPC.Center, targetCenter) : float.MaxValue;

		// 水生生物: it swims rather than walks, and tile collision keeps it inside the lake bed.
		NPC.noGravity = true;

		if (Main.netMode != NetmodeID.MultiplayerClient && DashCooldown > 0)
		{
			DashCooldown--;
		}

		bool inLiquid = IsInLiquid();

		switch (State)
		{
			case RadiolarianState.Dashing:
				UpdateDashing();
				break;
			case RadiolarianState.Disengaging:
				UpdateDisengaging(inLiquid, targetCenter, hasTarget);
				break;
			case RadiolarianState.Firing:
				UpdateFiring(inLiquid, targetCenter, hasTarget, distance, distanceSquared);
				break;
			default:
				UpdateRanging(inLiquid, targetCenter, hasTarget, distance);
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// The drifting range-keeping swim between engagements: the creature closes on a distant target and
	/// hands over to <see cref="RadiolarianState.Firing"/> once it reaches the design's firing band.
	/// </summary>
	/// <param name="inLiquid">Whether the creature is submerged.</param>
	/// <param name="targetCenter">The target's centre, or the creature's own when there is none.</param>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateRanging(bool inLiquid, Vector2 targetCenter, bool hasTarget, float distance)
	{
		if (inLiquid)
		{
			Vector2 direction = Vector2.UnitX * NPC.direction;
			if (hasTarget)
			{
				direction = (targetCenter - NPC.Center).SafeNormalize(direction);
			}

			NPC.velocity = Vector2.Lerp(NPC.velocity, direction * CruiseSpeed, 0.05f);
		}
		else
		{
			// 水生生物: out of the water it cannot fire, so it sinks back into the lake instead.
			NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.1f, SinkSpeed);
			NPC.velocity.X *= 0.97f;
		}

		if (hasTarget && distance <= FireRange)
		{
			EnterState(RadiolarianState.Firing);
		}
	}

	/// <summary>
	/// 在水中时连续发射水弹进行攻击: the sustained-fire window. The creature plants itself, fires on the
	/// design's cadence, and hands over to the dash the moment the target reaches four tiles.
	/// </summary>
	/// <param name="inLiquid">Whether the creature is submerged.</param>
	/// <param name="targetCenter">The target's centre, or the creature's own when there is none.</param>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	/// <param name="distanceSquared">The squared distance, used for the dash trigger.</param>
	private void UpdateFiring(bool inLiquid, Vector2 targetCenter, bool hasTarget, float distance, float distanceSquared)
	{
		NPC.velocity.X *= 0.92f;
		NPC.velocity.Y *= 0.92f;

		if (!hasTarget || distance > FireRange * GiveUpFactor)
		{
			EnterState(RadiolarianState.Ranging);
			return;
		}

		if (distanceSquared <= DashTriggerRangeSquared && DashCooldown <= 0)
		{
			// 当仇恨目标靠近时（4格）会尝试冲刺撞击穿过目标.
			EnterDash(targetCenter);
			return;
		}

		if (!inLiquid)
		{
			// 在水中时 and only then: out of the water the ranged attack is unavailable.
			EnterState(RadiolarianState.Ranging);
			return;
		}

		TryFireWaterBolt(targetCenter);
	}

	/// <summary>
	/// 冲刺撞击穿过目标: the dash itself, ended on its own timer on the authoritative side. The dash
	/// commits to the direction chosen by <see cref="EnterDash"/>, so it passes through rather than
	/// tracking the target mid-flight.
	/// </summary>
	private void UpdateDashing()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		ManeuverTimer--;
		if (ManeuverTimer > 0)
		{
			return;
		}

		// 然后拉开距离后重新使用远程: the charge is spent, so the retreat begins and the design's
		// 300-frame cooldown starts.
		State = RadiolarianState.Disengaging;
		ManeuverTimer = DisengageDuration;
		DashCooldown = DashCooldownFrames;
		NPC.netUpdate = true;
	}

	/// <summary>然后拉开距离: the retreat that reopens the firing gap, ending back in the ranged state.</summary>
	/// <param name="inLiquid">Whether the creature is submerged.</param>
	/// <param name="targetCenter">The target's centre, or the creature's own when there is none.</param>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	private void UpdateDisengaging(bool inLiquid, Vector2 targetCenter, bool hasTarget)
	{
		if (inLiquid)
		{
			Vector2 away = Vector2.UnitX * -NPC.direction;
			if (hasTarget)
			{
				away = (NPC.Center - targetCenter).SafeNormalize(away);
			}

			NPC.velocity = Vector2.Lerp(NPC.velocity, away * DisengageSpeed, 0.08f);
		}
		else
		{
			NPC.velocity.X *= 0.96f;
		}

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		ManeuverTimer--;
		if (ManeuverTimer <= 0)
		{
			EnterState(RadiolarianState.Ranging);
		}
	}

	/// <summary>
	/// 在水中时连续发射水弹: launches one water bolt at the target on the design's cadence. The bolt is
	/// spawned and the timer consumed on the authoritative side only, from <c>NPC.GetSource_FromAI()</c>
	/// so the damage is attributed to the creature and the bolt is never duplicated per client (D-55).
	/// </summary>
	/// <param name="targetCenter">The point the bolt is aimed at.</param>
	private void TryFireWaterBolt(Vector2 targetCenter)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		FireTimer--;
		if (FireTimer > 0)
		{
			return;
		}

		FireTimer = FireInterval;
		Vector2 direction = (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * WaterBoltSpeed, ModContent.ProjectileType<Radiolarian_WaterBolt>(), RangedDamage, 0f);
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 尝试冲刺撞击穿过目标: commits the charge. The direction is taken from the prey so the dash goes
	/// through it rather than stopping short. Written on the authoritative side only (D-55).
	/// </summary>
	/// <param name="targetCenter">The point the dash commits to.</param>
	private void EnterDash(Vector2 targetCenter)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		Vector2 direction = (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		NPC.velocity = direction * DashSpeed;
		State = RadiolarianState.Dashing;
		ManeuverTimer = DashDuration;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// True when the creature is 在水中: the engine's own wet flag plus the liquid read of the occupied
	/// tile, so a dry tick inside a drained pocket is not mistaken for a submerged one.
	/// </summary>
	/// <returns>True when the creature's centre tile holds liquid.</returns>
	private bool IsInLiquid()
	{
		return NPC.wet && HasLiquid((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f));
	}

	/// <summary>True when that tile is in the world and holds liquid. A world-edge tile reports false.</summary>
	/// <param name="tileX">The tile's X coordinate.</param>
	/// <param name="tileY">The tile's Y coordinate.</param>
	/// <returns>True when the tile holds liquid.</returns>
	private static bool HasLiquid(int tileX, int tileY)
	{
		if (!WorldGen.InWorld(tileX, tileY, 1))
		{
			return false;
		}

		return Main.tile[tileX, tileY].LiquidAmount > 0;
	}

	/// <summary>
	/// 在浅水区生成: the shallow test derives the surface the way <c>KelpCurtainSpawnConditions</c> does
	/// (a dry tile inside a bounded upward probe), because <see cref="NPCSpawnInfo"/> exposes no depth
	/// flag. The probe is bounded, so the world surface above an ocean is never mistaken for a lake bed.
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context; only its player-independent fields are read.</param>
	/// <returns>True when the liquid column above the spawn tile ends inside the probe window.</returns>
	private static bool IsShallowWater(NPCSpawnInfo spawnInfo)
	{
		int tileX = spawnInfo.SpawnTileX;
		for (int offset = 1; offset <= MaxShallowScanTiles; offset++)
		{
			int tileY = spawnInfo.SpawnTileY - offset;
			if (!WorldGen.InWorld(tileX, tileY, 1))
			{
				return false;
			}

			if (Main.tile[tileX, tileY].LiquidAmount <= 0)
			{
				// The liquid column reaches its surface inside the probe: this is a shallow-water spawn.
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// 在浅水区生成权重最低, and only ever inside Yggdrasil (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c>
	/// returns early outside the subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// <para>
	/// Region blocker (D-52/D-53): the design also places this row inside 亡碧湖's own sub-region, and the
	/// region-level sub-biome / tile predicate for it does not exist yet (Phase 5-6 terrain work). It is
	/// deliberately NOT approximated with a coordinate range: the shallow-water band is the part of the
	/// habitat this phase can express, and the region gap is recorded in 04-DEVIATIONS.md section 5.
	/// </para>
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The lowest aquatic weight of the phase, or <c>0f</c> outside the design's context.</returns>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		if (!spawnInfo.Water || !IsShallowWater(spawnInfo))
		{
			return 0f;
		}

		return KelpCurtainSpawnConditions.RareWaterBottomWeight;
	}

	/// <summary>
	/// 6.7%概率掉落 放射状甲壳: the design's one implemented drop, wired as the denominator 15 (6.7% -> 15,
	/// 04-DEVIATIONS.md section 6.1) against the Phase 1 item <c>RadialCarapace</c> (D-57). 软体甲壳碎片
	/// (2~5) and 亡碧膏 (1~2) are named by the same drop block but have no ModItem in the repository, so
	/// they get no rule and no type reference at all (D-58, Pitfall 2): a
	/// <c>ModContent.ItemType&lt;X&gt;()</c> for an absent <c>X</c> would fail the whole mod build. The
	/// blocker lives in 04-DEVIATIONS.md section 6.2 and in the biology matrix instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RadialCarapace>(), 15, 1, 1));
	}

	/// <summary>Keeps the facing in step with the direction of travel, also while idling.</summary>
	private void UpdateFacing()
	{
		if (MathF.Abs(NPC.velocity.X) > 0.05f)
		{
			NPC.direction = NPC.velocity.X > 0f ? 1 : -1;
		}

		NPC.spriteDirection = NPC.direction;
	}

	/// <summary>The design's state transition, written only on the authoritative side with an <c>NPC.netUpdate</c> (D-55).</summary>
	/// <param name="state">The state to enter.</param>
	private void EnterState(RadiolarianState state)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = state;
		NPC.netUpdate = true;
	}
}
