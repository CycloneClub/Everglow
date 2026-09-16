using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 水黾（Water Strider），亡碧湖（Death Jade Lake）唯一只在水面上出现的陆生滑行者。
/// Design row: 水黾, heading block id Gca8ddqq9on2lhxraxRcBNkmn6d,
/// stats table SqdydxBlPoASZtx5drwc9mZcnfe, behavior Frl7dwvbGoXf8sxSJozc3QfCnF2 (D-28 full implementation).
/// Approved artwork is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the migration is adding
/// WaterStrider.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset
/// path is Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/WaterStrider.png.
/// </summary>
public class WaterStrider : ModNPC
{
	/// <summary>
	/// 以一小段一小段的无目的的水上冲刺作为移动方式（60~200帧随机一次）: the lower bound of the aimless
	/// dash interval, in frames, used while no player is nearby (the design's own number).
	/// </summary>
	private const int DashIntervalFarMin = 60;

	/// <summary>60~200帧随机一次: the upper bound of the aimless dash interval.</summary>
	private const int DashIntervalFarMax = 200;

	/// <summary>
	/// 如果附近有玩家 ... 冲刺的频率会提高一点（45~150帧随机一次）: the tightened lower bound used while a
	/// player is inside <see cref="NearbyPlayerRange"/>.
	/// </summary>
	private const int DashIntervalNearMin = 45;

	/// <summary>45~150帧随机一次: the tightened upper bound.</summary>
	private const int DashIntervalNearMax = 150;

	/// <summary>
	/// One dash's own length. The design gives only the interval between dashes and calls each burst
	/// 一小段 (a short segment), so the burst itself is kept to a handful of frames (D-54 default).
	/// </summary>
	private const int DashBurstTicks = 18;

	/// <summary>水上冲刺: the dash speed, in pixels per tick (D-54 default: a skim, not a charge).</summary>
	private const float DashSpeed = 4.2f;

	/// <summary>如果附近有玩家: the range at which the dashes start aiming at the player (D-54 default).</summary>
	private const float NearbyPlayerRange = 24f * 16f;

	/// <summary>小跳: the horizontal hop speed used to get back to water from land (D-54 default).</summary>
	private const float LandHopSpeedX = 2.4f;

	/// <summary>小跳: the small vertical hop impulse used on land (D-54 default).</summary>
	private const float LandHopSpeedY = 4.6f;

	/// <summary>游回水面: the upward swim speed used to return from the lake bed (D-54 default).</summary>
	private const float SwimUpSpeed = 2.6f;

	/// <summary>The bounded horizontal window, in tiles, of the land-to-water search.</summary>
	private const int LandWaterScanTiles = 30;

	/// <summary>The bounded vertical window, in tiles, of the same search, so water just below a lip counts.</summary>
	private const int LandWaterScanDepth = 4;

	/// <summary>
	/// The three states of the design's behaviour (D-31): the surface skim with its dash cadence, the
	/// short burst itself, and the recovery used by both 移动到了陆地上 (小跳回最近的水面) and 出现在水底
	/// (游回水面). No vanilla <c>aiStyle</c> provides this cadence, so the class owns its <c>AI()</c>.
	/// </summary>
	private enum WaterStriderState
	{
		Skating = 0,
		Dashing = 1,
		Returning = 2,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private WaterStriderState State
	{
		get => (WaterStriderState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[0]</c>: the single dash timer. While skating it counts down to
	/// the next dash; while dashing it counts down the burst's remaining ticks.
	/// </summary>
	private int DashTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[1]</c>: the chosen dash heading, stored as a rotation in
	/// radians so the heading the authoritative side chose survives the round trip.
	/// </summary>
	private float DashAngle
	{
		get => NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[2]</c>: 如果没有就随机选择一个方向一直跳 - the single horizontal
	/// direction the creature keeps hopping in when no water is inside its search window.
	/// </summary>
	private float LandHopDirection
	{
		get => NPC.localAI[2];
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

		// 免疫 中毒、潮湿 (the design row's 免疫 cell): exactly these two immunities. The design's 潮湿
		// maps to the vanilla BuffID.Wet, verified present in this tML build (04-DEVIATIONS.md section 10).
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Wet] = true;
	}

	public override void SetDefaults()
	{
		// The dash cadence and the two recovery branches have no vanilla aiStyle analogue, so the class
		// owns a small custom AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// 体型接近史莱姆 (the design's own comparison): no sprite exists to measure, so the hit box takes
		// the slime proportion the design names and is recorded as a D-54 default.
		NPC.width = 44;
		NPC.height = 30;

		NPC.lifeMax = 45; // 生命 45
		NPC.life = 45;
		NPC.damage = 20; // 伤害 20
		NPC.defDamage = 20;
		NPC.defense = 10; // 防御 10
		NPC.defDefense = 10;
		NPC.knockBackResist = 0.6f; // 击退抗性 40 -> 1 - 0.40
		NPC.value = 80; // 钱币（铜） 80, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 水黾 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// 水面上: the surface film carries the creature while it is in liquid; on land it falls and hops
		// (both toggles are re-applied from world state in AI()).
		NPC.noGravity = true;

		// A deterministic starting interval, because SetDefaults runs on every side: no Main.rand is
		// consumed here, so the server and its clients agree on the first dash's timing.
		DashTimer = DashIntervalFarMin;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The designer's three-way split of the lake: the surface band the creature skates on, dry land,
	/// and below the surface band. Only the first runs the dash cadence; the other two both route
	/// through <see cref="WaterStriderState.Returning"/>. The branch is derived from synced world state
	/// on every side while the written state stays authoritative (D-55).
	/// </summary>
	public override void AI()
	{
		bool inLiquid = IsInLiquid();
		bool underwater = IsUnderwater();
		bool hasTarget = TryGetTarget(out float distance);

		WaterStriderState state;
		if (!inLiquid || underwater)
		{
			// 移动到了陆地上 / 出现在水底: both leave the surface band.
			state = WaterStriderState.Returning;
			if (State != WaterStriderState.Returning)
			{
				EnterReturning();
			}
		}
		else if (State == WaterStriderState.Returning)
		{
			// 回到水面: the surface band is reached again, so the design's skate cadence resumes.
			EnterSkating(hasTarget && distance <= NearbyPlayerRange);
			state = WaterStriderState.Skating;
		}
		else
		{
			state = State;
		}

		switch (state)
		{
			case WaterStriderState.Dashing:
				UpdateDashing(hasTarget, distance);
				break;
			case WaterStriderState.Returning:
				UpdateReturning();
				break;
			default:
				UpdateSkating(hasTarget, distance);
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// 只会在水面上刷新: a small surface ripple behind the skater. This is client-only work, so it sits
	/// behind the dedicated-server guard (D-35/D-55) and reuses an existing Kelp Curtain dust rather
	/// than creating a new dust class (D-51).
	/// </summary>
	public override void PostAI()
	{
		if (Main.dedServ || !IsInLiquid())
		{
			return;
		}

		if (Main.rand.NextBool(4))
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<KelpWaterDrop>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
		}
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (stats table SqdydxBlPoASZtx5drwc9mZcnfe), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// The design row names no drop at all, so no rule is added. 毒腺, 牛黄 and 软体甲壳碎片 have no class
	/// in the repository, so no item type is referenced here: a <c>ModContent.ItemType&lt;X&gt;()</c> for an
	/// absent <c>X</c> would fail the whole mod build (D-58, Pitfall 2).
	/// </summary>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}

	/// <summary>
	/// 只会在水面上刷新, and only ever inside Yggdrasil (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c>
	/// returns early outside the subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// The surface test is the design's own 水面上 condition, kept distinct from an ordinary water spawn
	/// (Pitfall 6).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The conservative water weight, or <c>0f</c> outside the design's context.</returns>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		if (!KelpCurtainSpawnConditions.IsWaterSurface(spawnInfo))
		{
			return 0f;
		}

		return KelpCurtainSpawnConditions.WaterWeight;
	}

	/// <summary>
	/// The surface skim between dashes. The creature rides the top liquid tile, so a small vertical
	/// correction holds it inside the surface band while its horizontal momentum bleeds off.
	/// </summary>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateSkating(bool hasTarget, float distance)
	{
		NPC.noGravity = true;

		// 只会在水面上: the surface band IS the creature's current liquid tile, so its centre is the
		// hold point and the correction only cancels drift out of the band.
		int centerTileY = (int)(NPC.Center.Y / 16f);
		float surfaceBandY = centerTileY * 16f + 8f;
		NPC.velocity.Y = MathHelper.Clamp((surfaceBandY - NPC.Center.Y) * 0.2f, -2f, 2f);

		// Between dashes the creature coasts: a dash carries it, then the film slows it down.
		NPC.velocity.X *= 0.94f;

		if (NPC.collideX)
		{
			// A wall at the end of the dash: turn the skater around instead of pressing into it.
			NPC.direction *= -1;
		}

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		bool playerNearby = hasTarget && distance <= NearbyPlayerRange;
		DashTimer--;
		if (DashTimer <= 0)
		{
			// 无目的的水上冲刺 / 向玩家冲刺: with a player inside the near range the burst aims at the
			// player and the next interval band tightens to 45~150; otherwise the heading is random.
			Vector2 direction;
			if (playerNearby)
			{
				Player target = Main.player[NPC.target];
				direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
			}
			else
			{
				direction = Vector2.UnitX * (Main.rand.NextBool() ? 1f : -1f);
			}

			DashAngle = direction.ToRotation();
			NPC.velocity = direction * DashSpeed;
			State = WaterStriderState.Dashing;
			DashTimer = DashBurstTicks;
			NPC.netUpdate = true;
		}
	}

	/// <summary>一小段: the dash burst itself, ended on its own timer on the authoritative side.</summary>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateDashing(bool hasTarget, float distance)
	{
		NPC.noGravity = true;

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		DashTimer--;
		if (DashTimer <= 0)
		{
			// The burst is over: pick the next interval from the band the current situation implies.
			State = WaterStriderState.Skating;
			DashTimer = PickDashInterval(hasTarget && distance <= NearbyPlayerRange);
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// The recovery state shared by both of the design's "if it somehow ends up elsewhere" clauses:
	/// 小跳 back to the nearest water from land, or 游 back up from below the surface band.
	/// </summary>
	private void UpdateReturning()
	{
		if (!IsInLiquid())
		{
			// 如果因为各种原因移动到了陆地上，会尝试小跳回最近的水面.
			NPC.noGravity = false;
			UpdateLandRecovery();
			return;
		}

		// 如果因为各种原因出现在水底，会正常游回水面.
		NPC.noGravity = true;
		NPC.velocity.X *= 0.95f;
		NPC.velocity.Y = -SwimUpSpeed;
	}

	/// <summary>
	/// 小跳回最近的水面（如果没有就随机选择一个方向一直跳）: a small hop towards the nearest water surface,
	/// or - when no water sits inside the bounded search window - repeated hops in one direction that
	/// was chosen once and is then kept.
	/// </summary>
	private void UpdateLandRecovery()
	{
		int direction = FindNearestLiquidDirection();
		if (direction == 0)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient && LandHopDirection == 0f)
			{
				// 随机选择一个方向一直跳: chosen once on the authoritative side and kept thereafter.
				LandHopDirection = Main.rand.NextBool() ? 1f : -1f;
			}

			direction = LandHopDirection >= 0f ? 1 : -1;
		}

		if (NPC.collideY || NPC.velocity.Y == 0f)
		{
			// 小跳: enough to clear a lip of ground, never a flight.
			NPC.velocity.Y = -LandHopSpeedY;
		}

		NPC.velocity.X = LandHopSpeedX * direction;
		NPC.direction = direction;
	}

	/// <summary>
	/// The bounded horizontal search for the nearest liquid at (or just below) the creature's own
	/// height: the 最近的水面 the land recovery hops towards.
	/// </summary>
	/// <returns><c>1</c> or <c>-1</c> for the side the water is on, or <c>0</c> when neither side has any.</returns>
	private int FindNearestLiquidDirection()
	{
		int tileX = (int)(NPC.Center.X / 16f);
		int tileY = (int)(NPC.Center.Y / 16f);
		for (int offset = 1; offset <= LandWaterScanTiles; offset++)
		{
			if (HasLiquidColumn(tileX + offset, tileY))
			{
				return 1;
			}

			if (HasLiquidColumn(tileX - offset, tileY))
			{
				return -1;
			}
		}

		return 0;
	}

	/// <summary>True when any tile in the bounded window under <paramref name="tileY"/> holds liquid.</summary>
	private static bool HasLiquidColumn(int tileX, int tileY)
	{
		for (int depth = 0; depth <= LandWaterScanDepth; depth++)
		{
			if (HasLiquid(tileX, tileY + depth))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>True when that tile is in the world and holds liquid. A world-edge tile reports false.</summary>
	private static bool HasLiquid(int tileX, int tileY)
	{
		if (!WorldGen.InWorld(tileX, tileY, 1))
		{
			return false;
		}

		return Main.tile[tileX, tileY].LiquidAmount > 0;
	}

	/// <summary>True when the creature's centre tile holds liquid, i.e. it is in the lake at all.</summary>
	private bool IsInLiquid()
	{
		return HasLiquid((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f));
	}

	/// <summary>
	/// 出现在水底: the surface band is the top liquid tile, so liquid in the tile directly above the
	/// creature means it is below that band and has to swim back up.
	/// </summary>
	private bool IsUnderwater()
	{
		int tileX = (int)(NPC.Center.X / 16f);
		int tileY = (int)(NPC.Center.Y / 16f);
		return HasLiquid(tileX, tileY) && HasLiquid(tileX, tileY - 1);
	}

	/// <summary>
	/// 如果附近有玩家: the nearest live player, used only to decide whether dashes aim at them and
	/// whether the interval band tightens. Never a camera or screen value (D-55).
	/// </summary>
	/// <param name="distance">The distance to that player, or <see cref="float.MaxValue"/>.</param>
	/// <returns>True when a live player was found.</returns>
	private bool TryGetTarget(out float distance)
	{
		NPC.TargetClosest(false);
		Player target = null;
		if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
		{
			target = Main.player[NPC.target];
		}

		if (target is null || !target.active || target.dead)
		{
			distance = float.MaxValue;
			return false;
		}

		distance = Vector2.Distance(NPC.Center, target.Center);
		return true;
	}

	/// <summary>
	/// Keeps the facing in step with the direction of travel. There is no approved sprite yet, so this
	/// only prepares the state the arriving art will read (the D-49 migration is art-only).
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
	/// The next dash interval. 60~200帧随机一次 with no player nearby, 45~150帧随机一次 while a player is
	/// close. Consumes <c>Main.rand</c>, so it is only ever called from an authoritative block (D-55).
	/// </summary>
	/// <param name="playerNearby">Whether a player is inside <see cref="NearbyPlayerRange"/>.</param>
	/// <returns>The interval, in frames, until the next dash.</returns>
	private static int PickDashInterval(bool playerNearby)
	{
		return playerNearby
			? Main.rand.Next(DashIntervalNearMin, DashIntervalNearMax + 1)
			: Main.rand.Next(DashIntervalFarMin, DashIntervalFarMax + 1);
	}

	/// <summary>移动到了陆地上 / 出现在水底, written only on the authoritative side with an <c>NPC.netUpdate</c> (D-55).</summary>
	private void EnterReturning()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = WaterStriderState.Returning;
		NPC.velocity = Vector2.Zero;
		NPC.netUpdate = true;
	}

	/// <summary>回到水面: the skate cadence resumes, written only on the authoritative side (D-55).</summary>
	/// <param name="playerNearby">Whether a player is inside <see cref="NearbyPlayerRange"/>.</param>
	private void EnterSkating(bool playerNearby)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = WaterStriderState.Skating;
		DashTimer = PickDashInterval(playerNearby);
		NPC.netUpdate = true;
	}
}
