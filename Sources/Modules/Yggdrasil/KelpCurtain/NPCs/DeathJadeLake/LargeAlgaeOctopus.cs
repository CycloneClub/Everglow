using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 大型覆藻章鱼（Large Algae Octopus），亡碧湖（Death Jade Lake）水底的抓取型小型伏击者：
/// 只在水底刷新，抓住猎物施加束缚与窒息，每 180 帧放出一波四团墨水云，残血时逃离玩家并喷墨阻挡。
/// Design row: 大型覆藻章鱼, heading block id AKGxdQG2foyNqBxGXjOcomLVnBb,
/// behavior block ZyROdHeOZoAGLsxHnTUcIK7unSb, drop block Hco0dB4t1oqGjbx1nXRczxm4nsg
/// (D-28 full implementation).
/// Approved artwork is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the migration is adding
/// LargeAlgaeOctopus.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime
/// asset path is Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/LargeAlgaeOctopus.png.
/// </summary>
public class LargeAlgaeOctopus : ModNPC
{
	// --- the design's own numbers ------------------------------------------------------------------

	/// <summary>每过180帧放出一波...墨水云: the design's own ink-wave interval, in frames.</summary>
	public const int InkWaveInterval = 180;

	/// <summary>每60帧施加2秒: the design's own grab debuff cadence, in frames.</summary>
	public const int GrabInterval = 60;

	/// <summary>每60帧施加2秒: the design's own debuff duration, 2 s = 120 ticks.</summary>
	public const int GrabDebuffTicks = 120;

	/// <summary>放出一波（4个...）: the design's own number of ink clouds per wave.</summary>
	public const int InkCloudsPerWave = 4;

	// --- conservative defaults (the design gives no number; recorded in 04-DEVIATIONS.md section 10)

	/// <summary>
	/// 若猎物为玩家且自身残血: the life fraction below which the creature stops fighting and runs. The
	/// design says 残血 without a number, so 40% is used and recorded as a D-54 default.
	/// </summary>
	private const float LowLifeFraction = 0.4f;

	/// <summary>伤害 50（近战）60（墨水）: the value the shared ink cloud carries for this owner.</summary>
	private const int InkDamage = 60;

	/// <summary>无法在远处被观察（隐身）: how close the prey must be for this creature to be seen.</summary>
	private const float ObservationRangeTiles = 32f;

	/// <summary>尝试抓住猎物: the leash beyond which the creature gives the chase up (D-54 default).</summary>
	private const float StalkRange = 72f * 16f;

	/// <summary>尝试抓住猎物: the range at which the grab begins (D-54 default).</summary>
	private const float GrabRange = 6f * 16f;

	/// <summary>冲刺速度非常快: the dash ranges and speed (D-54 defaults; faster than 覆藻章鱼's 9f).</summary>
	private const float DashRange = 14f * 16f;

	/// <summary>冲刺速度非常快: the dash speed, in pixels per tick (D-54 default).</summary>
	private const float DashSpeed = 16f;

	/// <summary>冲刺速度非常快: the dash's own duration, in ticks (D-54 default).</summary>
	private const int DashDuration = 26;

	/// <summary>The gap between two dashes (D-54 default; the design gives no cadence).</summary>
	private const int DashCooldownFrames = 150;

	/// <summary>The launch speed of one ink cloud of a wave, in pixels per tick (D-54 default).</summary>
	private const float InkCloudSpeed = 4.5f;

	/// <summary>不断喷射滞留墨水云: the gap between two flee ink clouds, in ticks (D-54 default).</summary>
	private const int FleeInkInterval = 24;

	/// <summary>会尝试逃离玩家: the retreat speed used while fleeing (D-54 default).</summary>
	private const float FleeSpeed = 5f;

	/// <summary>接近猎物 with the same stealth presentation as 覆藻章鱼: the stalk speed (D-54 default).</summary>
	private const float StalkSpeed = 2.4f;

	/// <summary>The slow aimless drift used while the creature is still hidden (D-54 default).</summary>
	private const float DriftSpeed = 1f;

	/// <summary>只有在水底才会刷新: the vertical speed the creature uses to settle onto the floor.</summary>
	private const float FloorSeekSpeed = 2.2f;

	/// <summary>The bounded window, in tiles, of the downward floor probe (the 碧灵鮟鱇 shape).</summary>
	private const int MaxFloorScanTiles = 24;

	/// <summary>The fully stealthed alpha: for an NPC, 0 is opaque and 255 is invisible.</summary>
	private const int StealthAlpha = 255;

	/// <summary>The observable alpha: fully visible inside the design's observation range.</summary>
	private const int RevealedAlpha = 0;

	/// <summary>Per-tick alpha change, so 隐身 and its release are gradual rather than instant.</summary>
	private const int AlphaStep = 5;

	/// <summary>
	/// The five states of the design's behaviour (D-31): hidden on the lake floor, 接近猎物, 尝试抓住猎物
	/// with its repeating 束缚与窒息, 冲刺速度非常快, and the 残血 flee that sprays lingering ink. No vanilla
	/// <c>aiStyle</c> provides this bottom-dwelling ambusher, so the class owns its <c>AI()</c>.
	/// </summary>
	private enum LargeAlgaeOctopusState
	{
		Lurking = 0,
		Stalking = 1,
		Grabbing = 2,
		Dashing = 3,
		Fleeing = 4,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private LargeAlgaeOctopusState State
	{
		get => (LargeAlgaeOctopusState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames left before the next ink wave (每过180帧).</summary>
	private int InkWaveTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[1]</c>: the frames left before the next grab (每60帧施加).</summary>
	private int GrabTimer
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[2]</c>: the frames left before the next dash is allowed.</summary>
	private int DashCooldown
	{
		get => (int)NPC.localAI[2];
		set => NPC.localAI[2] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[3]</c>: the dash's remaining ticks.</summary>
	private int DashTimer
	{
		get => (int)NPC.localAI[3];
		set => NPC.localAI[3] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[4]</c>: the frames left before the next flee ink cloud.</summary>
	private int FleeInkTimer
	{
		get => (int)NPC.localAI[4];
		set => NPC.localAI[4] = value;
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

		// 免疫 中毒、困惑 (the design row's 免疫 cell): exactly these two immunities.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override void SetDefaults()
	{
		// The grab/wave/flee cycle has no vanilla aiStyle analogue, so the class owns a small custom AI()
		// (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative large-octopus hit box is used and recorded as a
		// D-54 default in 04-DEVIATIONS.md section 10.
		NPC.width = 64;
		NPC.height = 56;

		NPC.lifeMax = 360; // 生命 360
		NPC.life = 360;

		// 伤害 50（近战）60（墨水）: the melee value is the contact damage, while the 60 travels on the
		// shared AlgaeOctopus_InkCloud projectile.
		NPC.damage = 50;
		NPC.defDamage = 50;

		NPC.defense = 12; // 防御 12
		NPC.defDefense = 12;
		NPC.knockBackResist = 0.8f; // 击退抗性 20 -> 1 - 0.20
		NPC.value = 4500; // 钱币（铜） 45银 = 4500 copper, a 1:1 transpose (NPC.value is copper)

		// 类型 稀有: the design's own 类型 cell maps to the LightPurple rarity (the Phase 3 巨树人
		// precedent, 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.LightPurple;

		// 只有在水底才会刷新, 水生生物: it swims along the floor rather than walking.
		NPC.noGravity = true;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 大型覆藻章鱼 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// It starts fully hidden, because it spawns outside the design's observation range by definition
		// (无法在远处被观察). Deterministic, so the server and its clients agree before the first sync.
		NPC.alpha = StealthAlpha;
		InkWaveTimer = InkWaveInterval;
		GrabTimer = GrabInterval;
		DashCooldown = 0;
		DashTimer = 0;
		FleeInkTimer = FleeInkInterval;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The state machine runner. The nearest player is the design's prey: every distance is computed from
	/// the synced <c>NPC.Center</c> and that player's centre, never from a camera or screen value (D-55),
	/// and the stealth presentation is the same distance-driven <c>NPC.alpha</c> as 覆藻章鱼's.
	/// </summary>
	public override void AI()
	{
		NPC.TargetClosest(false);
		Player prey = null;
		if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
		{
			prey = Main.player[NPC.target];
		}

		bool hasTarget = prey is not null && prey.active && !prey.dead;
		Vector2 targetCenter = hasTarget ? prey.Center : NPC.Center;
		float distance = hasTarget ? Vector2.Distance(NPC.Center, targetCenter) : float.MaxValue;

		// 水生生物: it swims rather than walks.
		NPC.noGravity = true;

		int desiredAlpha = ComputeStealthAlpha(distance);
		NPC.alpha = StepAlpha(NPC.alpha, desiredAlpha, AlphaStep);

		if (Main.netMode != NetmodeID.MultiplayerClient && DashCooldown > 0)
		{
			DashCooldown--;
		}

		// 每过180帧放出一波...: the wave runs whenever the creature is engaged. The flee branch sprays its
		// own lingering ink instead, and a hidden creature is not yet attacking.
		if (State != LargeAlgaeOctopusState.Fleeing && State != LargeAlgaeOctopusState.Lurking)
		{
			UpdateInkWave(targetCenter);
		}

		switch (State)
		{
			case LargeAlgaeOctopusState.Grabbing:
				UpdateGrabbing(prey, hasTarget, distance);
				break;
			case LargeAlgaeOctopusState.Dashing:
				UpdateDashing();
				break;
			case LargeAlgaeOctopusState.Fleeing:
				UpdateFleeing(targetCenter, hasTarget);
				break;
			case LargeAlgaeOctopusState.Stalking:
				UpdateStalking(targetCenter, hasTarget, distance);
				break;
			default:
				UpdateLurking(hasTarget, distance);
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// A faint ink smear trails the hidden creature. Client-only work, so it sits behind the
	/// dedicated-server guard (D-35/D-55) and reuses an existing Kelp Curtain dust rather than creating
	/// a new dust class (D-51).
	/// </summary>
	public override void PostAI()
	{
		if (Main.dedServ)
		{
			return;
		}

		if (Main.rand.NextBool(6))
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkLakeBottomMudDust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.5f, 0.9f);
		}
	}

	/// <summary>
	/// 减伤 15 (the design row's 减伤 cell): every incoming hit is scaled to 85%. Applied through
	/// <c>FinalDamage</c> so it composes with the engine's own defence and difficulty scaling instead of
	/// competing with it.
	/// </summary>
	/// <param name="modifiers">The hit modifiers being resolved.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
		modifiers.FinalDamage *= 0.85f; // 减伤 15
	}

	/// <summary>
	/// The death burst. Client-only work, so it sits behind the dedicated-server guard (D-35/D-55) and
	/// reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	/// <param name="hit">The killing hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life > 0 || Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 10; i++)
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkLakeBottomMudDust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.9f, 1.5f);
		}
	}

	/// <summary>
	/// 只有在水底才会刷新，权重是全部水生生物最低, and only ever inside Yggdrasil (BIO-06).
	/// <c>NPCSpawnManager.EditSpawnPool</c> returns early outside the subworld, so this per-creature gate
	/// is the real isolation, and <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe
	/// layer predicate because the hook runs in single player or on the server only, where the client
	/// camera is zero (D-52/D-55).
	/// <para>
	/// The water-bottom test is the design's own 只有在水底 condition, kept distinct from an ordinary water
	/// spawn (Pitfall 6, the 碧灵鮟鱇 precedent). A region-level 亡碧湖 sub-biome predicate does not exist
	/// yet (Phase 5-6 terrain work) and is deliberately NOT approximated with a coordinate range; the gap
	/// is recorded in 04-DEVIATIONS.md section 5 (D-52/D-53).
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

		if (!spawnInfo.Water || !KelpCurtainSpawnConditions.IsWaterBottom(spawnInfo))
		{
			return 0f;
		}

		return KelpCurtainSpawnConditions.RareWaterBottomWeight;
	}

	/// <summary>
	/// 死亡后掉落2~4 软体甲壳碎片与3~6亡碧膏，武器与饰品掉落待定: none of the three has a ModItem in the
	/// repository, so no rule is written and no item type is referenced here (D-58, Pitfall 2): a
	/// <c>ModContent.ItemType&lt;X&gt;()</c> for an absent <c>X</c> would fail the whole mod build. The
	/// blockers live in 04-DEVIATIONS.md section 6.2 and in the biology matrix instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}

	/// <summary>
	/// 无法在远处被观察（隐身）: the alpha scale between the fully observable and the fully hidden state,
	/// the same presentation 覆藻章鱼 uses. Below the observation range the creature fades in linearly;
	/// at or beyond it, it is invisible.
	/// </summary>
	/// <param name="distance">The distance to the synced target centre, or <see cref="float.MaxValue"/>.</param>
	/// <returns>The alpha to approach, clamped to 0-255.</returns>
	private static int ComputeStealthAlpha(float distance)
	{
		float observationRange = ObservationRangeTiles * 16f;
		if (distance >= observationRange)
		{
			return StealthAlpha;
		}

		return (int)MathHelper.Clamp(distance / observationRange * StealthAlpha, RevealedAlpha, StealthAlpha);
	}

	/// <summary>The per-tick alpha approach, so entering and leaving stealth are gradual.</summary>
	/// <param name="current">The current alpha.</param>
	/// <param name="target">The alpha to approach.</param>
	/// <param name="step">The maximum change per tick.</param>
	/// <returns>The next alpha value.</returns>
	private static int StepAlpha(int current, int target, int step)
	{
		if (current < target)
		{
			return Math.Min(current + step, target);
		}

		return Math.Max(current - step, target);
	}

	/// <summary>True when its own life has fallen into the design's 残血 band.</summary>
	/// <returns>True when the creature should run rather than fight.</returns>
	private bool IsLowLife()
	{
		return NPC.life < NPC.lifeMax * LowLifeFraction;
	}

	/// <summary>Hidden and drifting along the lake floor while nothing is inside the observation range.</summary>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateLurking(bool hasTarget, float distance)
	{
		// 只有在水底: it hugs the floor while it waits.
		HoldNearFloor();
		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * DriftSpeed, 0.02f);

		if (hasTarget && IsLowLife())
		{
			EnterState(LargeAlgaeOctopusState.Fleeing);
			return;
		}

		if (hasTarget && distance <= ObservationRangeTiles * 16f)
		{
			EnterState(LargeAlgaeOctopusState.Stalking);
		}
	}

	/// <summary>
	/// 接近猎物: the creature stalks its prey along the floor and hands over to the grab or the dash once
	/// it is inside the matching range.
	/// </summary>
	/// <param name="targetCenter">The target's centre, or the creature's own when there is none.</param>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateStalking(Vector2 targetCenter, bool hasTarget, float distance)
	{
		if (!hasTarget || distance > StalkRange)
		{
			EnterState(LargeAlgaeOctopusState.Lurking);
			return;
		}

		if (IsLowLife())
		{
			EnterState(LargeAlgaeOctopusState.Fleeing);
			return;
		}

		Vector2 direction = (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		NPC.velocity = Vector2.Lerp(NPC.velocity, direction * StalkSpeed, 0.05f);

		if (distance <= GrabRange)
		{
			EnterState(LargeAlgaeOctopusState.Grabbing);
			return;
		}

		if (distance <= DashRange && DashCooldown <= 0)
		{
			EnterDash(targetCenter);
		}
	}

	/// <summary>
	/// 尝试抓住猎物施加束缚与窒息（每60帧施加2秒）: the grab window. The creature holds position against its
	/// prey and re-applies the pair of timed debuffs on the design's cadence.
	/// </summary>
	/// <param name="prey">The grabbed player, or <c>null</c>.</param>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateGrabbing(Player prey, bool hasTarget, float distance)
	{
		NPC.velocity.X *= 0.9f;
		NPC.velocity.Y *= 0.9f;

		if (!hasTarget || distance > GrabRange * 1.5f)
		{
			EnterState(LargeAlgaeOctopusState.Stalking);
			return;
		}

		TryGrab(prey);
	}

	/// <summary>
	/// 每60帧施加2秒 of 束缚与窒息. The debuffs are timed only (120 ticks, re-applied every 60 frames) and
	/// never a persistent state, so a player can never be left permanently held even if the creature dies
	/// mid-grab (T-04-27). <para>
	/// Sync path (recorded choice, T-04-27): applied on the authoritative side - the same path the 04-02
	/// 幽光蝾螈 suffocation implementation uses (a server-side <c>AddBuff</c> that carries its own net
	/// sync) - and deliberately NOT through the client-only <c>OnHitPlayer</c> hook, because the design
	/// repeats the debuff on a 60-frame cadence rather than only on contact damage.
	/// </para>
	/// </summary>
	/// <param name="prey">The player being held.</param>
	private void TryGrab(Player prey)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || prey is null || !prey.active || prey.dead)
		{
			return;
		}

		GrabTimer--;
		if (GrabTimer > 0)
		{
			return;
		}

		GrabTimer = GrabInterval;

		// 束缚 maps to the vanilla BuffID.Webbed and 窒息 to BuffID.Suffocation (both verified present in
		// this tML build, 04-DEVIATIONS.md section 10).
		prey.AddBuff(BuffID.Webbed, GrabDebuffTicks);
		prey.AddBuff(BuffID.Suffocation, GrabDebuffTicks);
		NPC.netUpdate = true;
	}

	/// <summary>冲刺速度非常快: the dash itself, ended on its own timer on the authoritative side.</summary>
	private void UpdateDashing()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		DashTimer--;
		if (DashTimer > 0)
		{
			return;
		}

		State = LargeAlgaeOctopusState.Stalking;
		DashCooldown = DashCooldownFrames;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 若猎物为玩家且自身残血，会尝试逃离玩家并不断喷射滞留墨水云进行阻挡: the retreat. Ink is sprayed behind
	/// the creature on the authoritative side, and the branch ends when the prey is lost.
	/// </summary>
	/// <param name="targetCenter">The target's centre, or the creature's own when there is none.</param>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	private void UpdateFleeing(Vector2 targetCenter, bool hasTarget)
	{
		Vector2 away = Vector2.UnitX * -NPC.direction;
		if (hasTarget)
		{
			away = (NPC.Center - targetCenter).SafeNormalize(away);
		}

		NPC.velocity = Vector2.Lerp(NPC.velocity, away * FleeSpeed, 0.08f);

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		if (!hasTarget)
		{
			// No prey left to run from: the creature settles back onto the floor.
			State = LargeAlgaeOctopusState.Lurking;
			NPC.netUpdate = true;
			return;
		}

		FleeInkTimer--;
		if (FleeInkTimer > 0)
		{
			return;
		}

		FleeInkTimer = FleeInkInterval;
		SpawnInkCloud(away);
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 每过180帧放出一波（4个，主方向为猎物方向，剩下3个随机）四向扩散的墨水云: one full wave. The first cloud is
	/// aimed at the prey and the remaining three take random directions, all from the one shared
	/// <c>AlgaeOctopus_InkCloud</c> class carrying this owner's 60（墨水）. Written on the authoritative side
	/// only (D-55).
	/// </summary>
	/// <param name="targetCenter">The prey's centre, used for the wave's primary direction.</param>
	private void UpdateInkWave(Vector2 targetCenter)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		InkWaveTimer--;
		if (InkWaveTimer > 0)
		{
			return;
		}

		InkWaveTimer = InkWaveInterval;

		for (int i = 0; i < InkCloudsPerWave; i++)
		{
			// 主方向为猎物方向，剩下3个随机.
			Vector2 direction = i == 0
				? (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction)
				: Main.rand.NextVector2Circular(1f, 1f).SafeNormalize(Vector2.UnitX * NPC.direction);
			SpawnInkCloud(direction);
		}

		NPC.netUpdate = true;
	}

	/// <summary>
	/// Spawns one cloud of the shared <c>AlgaeOctopus_InkCloud</c> from <c>NPC.GetSource_FromAI()</c>, so
	/// the damage is attributed to the creature and the cloud is never duplicated per client (D-55). Its
	/// lingering *rendering* is the recorded effect blocker (04-DEVIATIONS.md section 7); its damage and
	/// its 2 s of 黑暗 are the shared projectile's.
	/// </summary>
	/// <param name="direction">The direction the cloud is launched in.</param>
	private void SpawnInkCloud(Vector2 direction)
	{
		Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * InkCloudSpeed, ModContent.ProjectileType<AlgaeOctopus_InkCloud>(), InkDamage, 0f, ai0: InkDamage);
	}

	/// <summary>
	/// The bounded downward floor probe (the 碧灵鮟鱇 shape): the creature holds itself just above the
	/// first solid tile under its centre, which is what makes the stalk bottom-anchored.
	/// </summary>
	private void HoldNearFloor()
	{
		int floorTileY = FindFloorTileY();
		if (floorTileY < 0)
		{
			NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.05f, FloorSeekSpeed);
			return;
		}

		float distanceToFloor = floorTileY * 16f - NPC.Bottom.Y;
		NPC.velocity.Y = MathHelper.Clamp(distanceToFloor * 0.05f, -FloorSeekSpeed, FloorSeekSpeed);
	}

	/// <summary>
	/// The tile Y of the first solid tile under the creature's centre, or <c>-1</c> when no floor is
	/// inside the bounded probe window.
	/// </summary>
	/// <returns>The floor tile's Y, or <c>-1</c>.</returns>
	private int FindFloorTileY()
	{
		int tileX = (int)(NPC.Center.X / 16f);
		int startTileY = (int)((NPC.Bottom.Y + 8f) / 16f);
		for (int offset = 0; offset <= MaxFloorScanTiles; offset++)
		{
			int tileY = startTileY + offset;
			if (!WorldGen.InWorld(tileX, tileY, 1))
			{
				return -1;
			}

			if (Main.tile[tileX, tileY].HasTile)
			{
				return tileY;
			}
		}

		return -1;
	}

	/// <summary>冲刺速度非常快: commits the dash along the direction to the prey, written authoritatively (D-55).</summary>
	/// <param name="targetCenter">The point the dash commits to.</param>
	private void EnterDash(Vector2 targetCenter)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		Vector2 direction = (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		NPC.velocity = direction * DashSpeed;
		State = LargeAlgaeOctopusState.Dashing;
		DashTimer = DashDuration;
		NPC.netUpdate = true;
	}

	/// <summary>Keeps the facing in step with the direction of travel.</summary>
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
	private void EnterState(LargeAlgaeOctopusState state)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = state;
		NPC.netUpdate = true;
	}
}
