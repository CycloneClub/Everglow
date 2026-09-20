using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 覆藻章鱼（Algae Octopus），亡碧湖（Death Jade Lake）的隐身伏击软体动物：无法在远处被观察，
/// 贴近后快速冲刺并以墨水云骚扰，接触时施加缓慢，死亡时爆出一团墨水。
/// Design row: 覆藻章鱼, heading block id K8PmdSvk3okdbbxo7vYc4flKnKd,
/// behavior block WwyUddu8SooiTDxy6tPcrgGEn3b, drop block (in behaviour) (D-28 full implementation).
/// Approved artwork is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the migration is adding
/// AlgaeOctopus.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset
/// path is Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/AlgaeOctopus.png.
/// </summary>
public class AlgaeOctopus : ModNPC
{
	/// <summary>
	/// 无法在远处被观察（隐身）: the range, in tiles, inside which the creature becomes observable. The
	/// design gives no number, so a conservative aquatic ambush range is used and recorded as a D-54
	/// default in 04-DEVIATIONS.md section 10.
	/// </summary>
	public const float ObservationRangeTiles = 24f;

	/// <summary>The leash beyond which the creature gives the chase up and fades back out (D-54 default).</summary>
	private const float StalkRange = 56f * 16f;

	/// <summary>接近后主动攻击: the range at which the creature commits to its dash (D-54 default).</summary>
	private const float DashRange = 10f * 16f;

	/// <summary>自身能快速冲刺: the dash speed, in pixels per tick (D-54 default).</summary>
	private const float DashSpeed = 9f;

	/// <summary>快速冲刺: the dash's own duration, in ticks (D-54 default).</summary>
	private const int DashDuration = 30;

	/// <summary>The gap between two dashes (D-54 default; the design gives no cadence).</summary>
	private const int DashCooldownFrames = 150;

	/// <summary>留下墨水云: the gap, in ticks, between two ink puffs left along a dash (D-54 default).</summary>
	private const int InkTrailInterval = 10;

	/// <summary>The starting ink timer: one interval, so the first puff lands one interval into the dash.</summary>
	private const int InkIntervalReset = InkTrailInterval;

	/// <summary>伤害 15（近战）25（墨水）: the value the shared ink cloud carries for this owner.</summary>
	private const int InkDamage = 25;

	/// <summary>对玩家施加缓慢: the duration of the contact Slow, in ticks (D-54 default).</summary>
	private const int SlowTicks = 300;

	/// <summary>The fully stealthed alpha: for an NPC, 0 is opaque and 255 is invisible.</summary>
	private const int StealthAlpha = 255;

	/// <summary>The observable alpha: fully visible inside the design's observation range.</summary>
	private const int RevealedAlpha = 0;

	/// <summary>Per-tick alpha change, so 隐身 and its release are gradual rather than instant.</summary>
	private const int AlphaStep = 6;

	/// <summary>接近后: the swim speed used while closing on the prey (D-54 default).</summary>
	private const float StalkSpeed = 2.6f;

	/// <summary>The slow aimless drift used while the creature is still hidden (D-54 default).</summary>
	private const float DriftSpeed = 1.2f;

	/// <summary>
	/// The three states of the design's behaviour (D-31): hidden and drifting, 接近后 actively stalking,
	/// and 自身能快速冲刺. No vanilla <c>aiStyle</c> provides a stealth ambusher, so the class owns its
	/// <c>AI()</c>.
	/// </summary>
	private enum AlgaeOctopusState
	{
		Lurking = 0,
		Stalking = 1,
		Dashing = 2,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private AlgaeOctopusState State
	{
		get => (AlgaeOctopusState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames left before the next dash is allowed.</summary>
	private int DashCooldown
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[1]</c>: the frames left before the next ink puff of a dash.</summary>
	private int InkTimer
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[2]</c>: the dash's remaining ticks.</summary>
	private int DashTimer
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
		// The stealth/approach/dash cycle has no vanilla aiStyle analogue, so the class owns a small
		// custom AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative octopus-sized hit box is used and recorded as a
		// D-54 default in 04-DEVIATIONS.md section 10.
		NPC.width = 40;
		NPC.height = 40;

		NPC.lifeMax = 70; // 生命 70
		NPC.life = 70;

		// 伤害 15（近战）25（墨水）: the melee value is the contact damage, while the 25 travels on the
		// shared AlgaeOctopus_InkCloud projectile.
		NPC.damage = 15;
		NPC.defDamage = 15;

		NPC.defense = 12; // 防御 12
		NPC.defDefense = 12;
		NPC.knockBackResist = 0.9f; // 击退抗性 10 -> 1 - 0.10
		NPC.value = 500; // 钱币（铜） 5银 = 500 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, section 10).
		NPC.rarity = ItemRarityID.White;

		// 水生生物: the water column carries it, so gravity is released.
		NPC.noGravity = true;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 覆藻章鱼 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// It starts fully hidden, because it spawns outside the design's observation range by definition
		// (无法在远处被观察). Deterministic, so the server and its clients agree before the first sync.
		NPC.alpha = StealthAlpha;
		DashCooldown = 0;
		InkTimer = InkIntervalReset;
		DashTimer = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The state machine runner. The nearest player is the design's prey and the stealth is a function of
	/// the distance between the synced <c>NPC.Center</c> and that player's centre, never of a camera or
	/// screen value (D-55).
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

		// 水生生物: it swims rather than walks.
		NPC.noGravity = true;

		// 无法在远处被观察（隐身）: the alpha rises with the distance to the prey, so the creature is
		// invisible beyond the design's observation range and opaque up close.
		int desiredAlpha = ComputeStealthAlpha(distance);
		NPC.alpha = StepAlpha(NPC.alpha, desiredAlpha, AlphaStep);

		if (Main.netMode != NetmodeID.MultiplayerClient && DashCooldown > 0)
		{
			DashCooldown--;
		}

		switch (State)
		{
			case AlgaeOctopusState.Dashing:
				UpdateDashing();
				break;
			case AlgaeOctopusState.Stalking:
				UpdateStalking(targetCenter, hasTarget, distance);
				break;
			default:
				UpdateLurking(hasTarget, distance);
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// 接近后: a faint ink smear trails the hidden creature. Client-only work, so it sits behind the
	/// dedicated-server guard (D-35/D-55) and reuses an existing Kelp Curtain dust rather than creating
	/// a new dust class (D-51).
	/// </summary>
	public override void PostAI()
	{
		if (Main.dedServ)
		{
			return;
		}

		if (Main.rand.NextBool(8))
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<KelpWaterDrop>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.5f, 0.9f);
		}
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (behavior block WwyUddu8SooiTDxy6tPcrgGEn3b), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	/// <param name="modifiers">The hit modifiers being resolved.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// 对玩家施加缓慢: every contact hit applies the design's Slow. tML documents <c>ModNPC.OnHitPlayer</c>
	/// as "Called on the local client only", so this is deliberately NOT wrapped in
	/// <c>Main.netMode != NetmodeID.MultiplayerClient</c>: such a guard would make the design's debuff dead
	/// code in multiplayer (the 剧毒蟾蜍 / 荆棘苔龟 precedent). The design gives no duration, so a
	/// conservative 5 s (300 ticks) is used and recorded as a D-54 default in 04-DEVIATIONS.md section 10.
	/// </summary>
	/// <param name="target">The player the creature touched.</param>
	/// <param name="hurtInfo">The resolved hit.</param>
	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Slow, SlowTicks);
	}

	/// <summary>
	/// 死亡后爆出墨水云: one cloud is spawned at the creature's position, from the creature's own AI source
	/// and only on the authoritative side (D-55). The ink cloud's damage and its 2 s of 黑暗 are
	/// implemented here; only its *lingering rendering* is an effect blocker recorded in
	/// 04-DEVIATIONS.md section 7 (OQ4), which is why no client VFX class is created for it.
	/// </summary>
	/// <param name="hit">The killing hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life > 0)
		{
			return;
		}

		SpawnInkPuff(NPC.Center);

		if (Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 8; i++)
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<DarkLakeBottomMudDust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.9f, 1.4f);
		}
	}

	/// <summary>
	/// 水生生物，敌对生物，也会在森雨幽谷刷新, and only ever inside Yggdrasil (BIO-06).
	/// <c>NPCSpawnManager.EditSpawnPool</c> returns early outside the subworld, so this per-creature gate
	/// is the real isolation, and <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe
	/// layer predicate because the hook runs in single player or on the server only, where the client
	/// camera is zero (D-52/D-55).
	/// <para>
	/// Region blocker (D-52/D-53): the design's 森雨幽谷 habitat has no region-level sub-biome or tile
	/// predicate yet (Phase 5-6 terrain work). It is deliberately NOT approximated with a coordinate
	/// range: only the Kelp Curtain water spawn is expressed here, and the region gap is recorded in
	/// 04-DEVIATIONS.md section 5.
	/// </para>
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

	/// <summary>
	/// 死亡后掉落1 软体甲壳碎片与 1 亡碧膏: neither material has a ModItem in the repository, so no rule is
	/// written and no item type is referenced here (D-58, Pitfall 2): a
	/// <c>ModContent.ItemType&lt;X&gt;()</c> for an absent <c>X</c> would fail the whole mod build. The
	/// blocker lives in 04-DEVIATIONS.md section 6.2 and in the biology matrix instead.
	/// <para>
	/// Target-rule note: the design also has this creature attack 其他水生生物（除了鮟鱇）. tML provides no
	/// NPC-versus-NPC aggro hook and this plan adds no cross-plan creature type reference, so only the
	/// player target is taken from the engine (NPC.TargetClosest) and the inter-creature hostility is the
	/// unmodelled system already recorded in 04-DEVIATIONS.md section 7 / section 13.
	/// </para>
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}

	/// <summary>
	/// 无法在远处被观察（隐身）: the alpha scale between the fully observable and the fully hidden state.
	/// Below the observation range the creature fades in linearly; at or beyond it, it is invisible.
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

	/// <summary>Hidden and drifting while nothing has come inside the design's observation range.</summary>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateLurking(bool hasTarget, float distance)
	{
		NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitX * NPC.direction * DriftSpeed, 0.02f);

		if (hasTarget && distance <= ObservationRangeTiles * 16f)
		{
			EnterState(AlgaeOctopusState.Stalking);
		}
	}

	/// <summary>
	/// 接近后主动攻击: the creature stalks its prey and commits to the dash once it is inside the dash
	/// range and the dash is off cooldown.
	/// </summary>
	/// <param name="targetCenter">The target's centre, or the creature's own when there is none.</param>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateStalking(Vector2 targetCenter, bool hasTarget, float distance)
	{
		if (!hasTarget || distance > StalkRange)
		{
			EnterState(AlgaeOctopusState.Lurking);
			return;
		}

		Vector2 direction = (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		NPC.velocity = Vector2.Lerp(NPC.velocity, direction * StalkSpeed, 0.06f);

		if (distance <= DashRange && DashCooldown <= 0)
		{
			EnterDash(targetCenter);
		}
	}

	/// <summary>
	/// 自身能快速冲刺并留下墨水云: the dash itself. The ink trail is emitted and the dash ended on the
	/// authoritative side only, so the puffs are never duplicated per client (D-55).
	/// </summary>
	private void UpdateDashing()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		InkTimer--;
		if (InkTimer <= 0)
		{
			InkTimer = InkTrailInterval;
			SpawnInkPuff(NPC.Center);
		}

		DashTimer--;
		if (DashTimer <= 0)
		{
			State = AlgaeOctopusState.Stalking;
			DashCooldown = DashCooldownFrames;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// 自身能快速冲刺: commits the dash along the direction to the prey, so the charge goes through rather
	/// than tracking mid-flight. Written on the authoritative side only (D-55).
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
		State = AlgaeOctopusState.Dashing;
		DashTimer = DashDuration;
		InkTimer = InkIntervalReset;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 留下墨水云 / 死亡后爆出墨水云: spawns one shared <c>AlgaeOctopus_InkCloud</c>, carrying this
	/// owner's 25（墨水）through the projectile's <c>ai[0]</c>. Spawned from <c>NPC.GetSource_FromAI()</c>
	/// and only on the authoritative side (D-55); the cloud's lingering *rendering* is the recorded effect
	/// blocker (04-DEVIATIONS.md section 7), its damage and Darkness are implemented.
	/// </summary>
	/// <param name="position">Where the cloud is left behind.</param>
	private void SpawnInkPuff(Vector2 position)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		Vector2 velocity = Main.rand.NextVector2Circular(1.2f, 1.2f);
		Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<AlgaeOctopus_InkCloud>(), InkDamage, 0f, ai0: InkDamage);
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
	private void EnterState(AlgaeOctopusState state)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = state;
		NPC.netUpdate = true;
	}
}
