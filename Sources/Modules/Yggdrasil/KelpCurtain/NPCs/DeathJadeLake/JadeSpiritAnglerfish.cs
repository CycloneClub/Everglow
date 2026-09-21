using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Pets;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 碧灵鮟鱇（Jade Spirit Anglerfish），亡碧湖（Death Jade Lake）水底的隐身伏击者。
/// Design row: 碧灵鮟鱇, heading block id RbODdEzlYo9XtHxQ6z7cmc5Wn2q,
/// stats table GfdvdtB0MooyyFxY1ZmchFxinlb, behavior E0aLdZ0tmoEvhTxQvJNcYFeEnxe,
/// drop GMYGdtpeZo4oHMx05TdcHK0WnGd (D-28 full implementation).
/// Approved artwork is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the migration is adding
/// JadeSpiritAnglerfish.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime
/// asset path is Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/JadeSpiritAnglerfish.png.
/// </summary>
public class JadeSpiritAnglerfish : ModNPC
{
	/// <summary>
	/// 破隐第一次冲刺速度非常快: the reveal dash's speed, far above the 随后正常速度 follow-up speed.
	/// The design gives no number, so a conservative aquatic dash speed is used (D-54 default).
	/// </summary>
	private const float DashSpeed = 11f;

	/// <summary>随后正常速度追随猎物: the normal follow speed (D-54 default).</summary>
	private const float ChaseSpeed = 3.4f;

	/// <summary>尝试回到水底: the slow swim used while drifting back to the lake floor.</summary>
	private const float ReturnSpeed = 2.2f;

	/// <summary>伤害 60（出动）: the reveal dash's contact damage (the design's 出动 value).</summary>
	private const int RevealDashDamage = 60;

	/// <summary>
	/// 接近后主动攻击玩家: the range at which the hidden creature reveals itself. The design only says it
	/// cannot be observed from a distance (无法在远处被观察), so 26 tiles is a conservative default
	/// recorded in 04-DEVIATIONS.md section 10 (D-54).
	/// </summary>
	private const float RevealRange = 26f * 16f;

	/// <summary>
	/// The prey must leave this multiple of <see cref="RevealRange"/> before the creature gives up and
	/// fades back into stealth, so a player hovering on the boundary cannot make it flicker.
	/// </summary>
	private const float RevealGiveUpFactor = 1.5f;

	/// <summary>The fully stealthed alpha: for an NPC, 0 is opaque and 255 is invisible.</summary>
	private const int StealthAlpha = 255;

	/// <summary>The revealed alpha: fully visible while hunting.</summary>
	private const int RevealedAlpha = 0;

	/// <summary>Per-tick alpha change, so revealing and 逐渐隐身 are gradual rather than instant.</summary>
	private const int AlphaStep = 4;

	/// <summary>The reveal dash's duration, in ticks (D-54 default).</summary>
	private const int DashDuration = 45;

	/// <summary>
	/// 破隐第一次冲刺 - the dash belongs to the first moment of a reveal, so it is put on a cooldown and a
	/// creature that re-stealths and re-reveals cannot dash again immediately (D-54 default).
	/// </summary>
	private const int DashCooldownTicks = 300;

	/// <summary>How close to the floor the creature must be to settle back into the idle state.</summary>
	private const float FloorSettleDistance = 3f * 16f;

	/// <summary>The bounded window, in tiles, of the downward floor probe.</summary>
	private const int MaxFloorScanTiles = 24;

	/// <summary>
	/// The five states of the design's stealth cycle: 只会在水底刷新 while hidden, 破隐 on approach, the
	/// first high-damage dash, the normal-speed follow that 逐渐隐身, and 尝试回到水底 once fully stealthed.
	/// </summary>
	private enum JadeSpiritAnglerfishState
	{
		Idle = 0,
		Revealed = 1,
		Dashing = 2,
		Chasing = 3,
		Returning = 4,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private JadeSpiritAnglerfishState State
	{
		get => (JadeSpiritAnglerfishState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[0]</c>: this class's own stealth/aggro timer. It counts the
	/// reveal dash's remaining ticks while dashing and is reset at every state change.
	/// </summary>
	private int StealthTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[1]</c>: the reveal dash's cooldown, counted down on the
	/// authoritative side while the creature follows its prey.
	/// </summary>
	private int DashCooldown
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = (int)value;
	}

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	// Approved artwork is missing from the repository (D-48). The shared fallback keeps the class
	// loadable, and the empty beside-.png is asserted by the phase's gate until the art arrives.
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
		// The design's stealth/reveal/dash/return cycle has no vanilla aiStyle analogue, so the class
		// owns a small custom AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative mid-sized aquatic box is used and recorded as
		// a D-54 default in 04-DEVIATIONS.md section 10.
		NPC.width = 46;
		NPC.height = 28;

		NPC.lifeMax = 150; // 生命 150
		NPC.life = 150;
		NPC.defense = 8; // 防御 8
		NPC.knockBackResist = 0.8f; // 击退抗性 20% -> 1 - 0.20
		NPC.value = 500; // 钱币（铜） 5银 = 500 copper, a 1:1 transpose (NPC.value is copper)

		// 伤害 60（出动）30（正常）: the normal value is pinned as the base and PostAI swaps in 60 while
		// dashing from the synced state, so server and clients always agree.
		NPC.damage = 30;
		NPC.defDamage = 30;
		NPC.defDefense = 8;

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, section 10).
		NPC.rarity = ItemRarityID.White;

		// An aquatic swimmer: it must not be pulled down by gravity. Tile collision stays on, as on the
		// Phase 3 aquatic creatures, so it cannot swim through the lake bed.
		NPC.noGravity = true;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 碧灵鮟鱇 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// 伤害 60（出动）30（正常）: the design's contact damage depends on the reveal dash, and the state
	/// lives in the synced <c>NPC.ai[0]</c>, so both the server and every client restore the right value
	/// here (the <c>MossyThornTurtle</c> precedent) instead of relying on the unsynced damage field.
	/// </summary>
	public override void PostAI()
	{
		NPC.damage = State == JadeSpiritAnglerfishState.Dashing ? RevealDashDamage : NPC.defDamage;
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (stats table GfdvdtB0MooyyFxY1ZmchFxinlb), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// The state machine runner. The nearest player is the design's prey; the creature's stealth is a
	/// visual state derived from the synced <c>NPC.ai[0]</c>, so every side computes the same alpha.
	/// </summary>
	public override void AI()
	{
		UpdateLamp();

		NPC.TargetClosest(false);
		Player target = null;
		if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
		{
			target = Main.player[NPC.target];
		}

		bool hasTarget = target is not null && target.active && !target.dead;
		float distance = hasTarget ? Vector2.Distance(NPC.Center, target.Center) : float.MaxValue;

		switch (State)
		{
			case JadeSpiritAnglerfishState.Idle:
				UpdateIdle(hasTarget, distance);
				break;
			case JadeSpiritAnglerfishState.Revealed:
				UpdateRevealed(hasTarget, distance);
				break;
			case JadeSpiritAnglerfishState.Dashing:
				UpdateDashing();
				break;
			case JadeSpiritAnglerfishState.Chasing:
				UpdateChasing(hasTarget, target, distance);
				break;
			default:
				UpdateReturning();
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// 隐身但是会亮出绿色的灯 ... 完全隐身后（除了灯）: the green lamp is the design's constant, showing even
	/// when the creature is fully stealthed. It is client-only work, so the whole block sits behind the
	/// dedicated-server guard (D-35/D-55) and reuses an existing Kelp Curtain dust rather than creating
	/// a new dust class.
	/// </summary>
	private void UpdateLamp()
	{
		if (Main.dedServ)
		{
			return;
		}

		Lighting.AddLight(NPC.Center, 0.10f, 0.70f, 0.40f);
		if (Main.rand.NextBool(5))
		{
			int dust = Dust.NewDust(NPC.Center, 0, 0, ModContent.DustType<KelpWaterDrop>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.1f);
		}
	}

	/// <summary>
	/// 无法在远处被观察: while hidden the creature holds the full stealth alpha and hovers on the lake
	/// floor, and it reveals itself as soon as the prey is inside the design's approach range.
	/// </summary>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateIdle(bool hasTarget, float distance)
	{
		NPC.alpha = StepAlpha(NPC.alpha, StealthAlpha, AlphaStep);

		int floorTileY = FindFloorTileY();
		if (floorTileY >= 0)
		{
			float distanceToFloor = floorTileY * 16f - NPC.Bottom.Y;
			if (distanceToFloor <= FloorSettleDistance)
			{
				// Settled on the lake bed: hover in place with a slow vertical bob only.
				NPC.velocity.Y = MathF.Sin((float)Main.time * 0.03f + NPC.whoAmI) * 0.12f;
				NPC.velocity.X *= 0.97f;
			}
			else
			{
				// Still above the floor: sink back down (the tail of 尝试回到水底).
				NPC.velocity.Y = Math.Min(ReturnSpeed, distanceToFloor * 0.05f);
				NPC.velocity.X *= 0.98f;
			}
		}
		else
		{
			NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.03f, ReturnSpeed);
			NPC.velocity.X *= 0.98f;
		}

		if (hasTarget && distance <= RevealRange)
		{
			EnterReveal();
		}
	}

	/// <summary>
	/// 破隐: the reveal window. The stealth alpha falls away quickly while the prey is re-checked, and
	/// once the creature is fully visible it either opens with the design's high-damage dash or, while
	/// the dash is still cooling down, settles straight into the normal follow.
	/// </summary>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateRevealed(bool hasTarget, float distance)
	{
		NPC.alpha = StepAlpha(NPC.alpha, RevealedAlpha, AlphaStep * 3);
		NPC.velocity *= 0.92f;

		if (!hasTarget || distance > RevealRange * RevealGiveUpFactor)
		{
			// The prey left before the reveal completed: fade back into stealth.
			EnterIdle();
			return;
		}

		if (NPC.alpha > RevealedAlpha)
		{
			return;
		}

		if (DashCooldown <= 0)
		{
			EnterDash();
		}
		else
		{
			EnterChase();
		}
	}

	/// <summary>
	/// 破隐第一次冲刺: the dash itself. The high damage rides on the synced state through
	/// <see cref="PostAI"/>, and the dash ends on its own timer on the authoritative side.
	/// </summary>
	private void UpdateDashing()
	{
		NPC.alpha = RevealedAlpha;

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		StealthTimer--;
		if (StealthTimer <= 0)
		{
			EnterChase();
		}
	}

	/// <summary>
	/// 随后正常速度追随猎物并逐渐隐身（除了灯）: the follow. The creature tracks the prey at the normal
	/// speed while its stealth alpha climbs back, and once it is fully stealthed it 失去仇恨 and
	/// returns to the floor.
	/// </summary>
	/// <param name="hasTarget">Whether a live player target exists.</param>
	/// <param name="target">The player target, or <c>null</c>.</param>
	/// <param name="distance">The distance to that target, or <see cref="float.MaxValue"/>.</param>
	private void UpdateChasing(bool hasTarget, Player target, float distance)
	{
		NPC.alpha = StepAlpha(NPC.alpha, StealthAlpha, AlphaStep);

		if (Main.netMode != NetmodeID.MultiplayerClient && DashCooldown > 0)
		{
			DashCooldown--;
		}

		Vector2 aim = Vector2.UnitX * NPC.direction;
		if (hasTarget)
		{
			aim = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		}

		NPC.velocity = Vector2.Lerp(NPC.velocity, aim * ChaseSpeed, 0.05f);

		if (NPC.alpha >= StealthAlpha)
		{
			EnterReturning();
		}
	}

	/// <summary>
	/// 尝试回到水底: fully stealthed and out of aggro, the creature swims back down to the lake floor and
	/// settles into <see cref="JadeSpiritAnglerfishState.Idle"/> once it gets there.
	/// </summary>
	private void UpdateReturning()
	{
		NPC.alpha = StealthAlpha;

		int floorTileY = FindFloorTileY();
		if (floorTileY >= 0)
		{
			float distanceToFloor = floorTileY * 16f - NPC.Bottom.Y;
			if (distanceToFloor <= FloorSettleDistance)
			{
				EnterIdle();
				return;
			}

			NPC.velocity.Y = Math.Min(ReturnSpeed, distanceToFloor * 0.05f);
		}
		else
		{
			NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.05f, ReturnSpeed);
		}

		NPC.velocity.X *= 0.98f;
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

	/// <summary>破隐, written only on the authoritative side with an <c>NPC.netUpdate</c> (D-55).</summary>
	private void EnterReveal()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = JadeSpiritAnglerfishState.Revealed;
		StealthTimer = 0;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// The one high-damage opening charge. The direction is taken from the prey so the dash commits to
	/// the target rather than to the previous drift. Written on the authoritative side only (D-55).
	/// </summary>
	private void EnterDash()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		Vector2 direction = Vector2.UnitX * NPC.direction;
		if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
		{
			Player target = Main.player[NPC.target];
			if (target.active && !target.dead)
			{
				direction = (target.Center - NPC.Center).SafeNormalize(direction);
			}
		}

		NPC.velocity = direction * DashSpeed;
		State = JadeSpiritAnglerfishState.Dashing;
		StealthTimer = DashDuration;
		NPC.netUpdate = true;
	}

	/// <summary>随后正常速度追随猎物, written only on the authoritative side (D-55).</summary>
	private void EnterChase()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = JadeSpiritAnglerfishState.Chasing;
		DashCooldown = DashCooldownTicks;
		NPC.netUpdate = true;
	}

	/// <summary>完全隐身后失去仇恨并尝试回到水底, written only on the authoritative side (D-55).</summary>
	private void EnterReturning()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = JadeSpiritAnglerfishState.Returning;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// Fully stealthed and idle on the floor again. The state change stays authoritative; the alpha
	/// keeps its current value so the creature does not pop back into view (D-55).
	/// </summary>
	private void EnterIdle()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = JadeSpiritAnglerfishState.Idle;
		StealthTimer = 0;
		NPC.netUpdate = true;
	}

	/// <summary>The per-tick alpha approach, so revealing and re-stealthing are gradual.</summary>
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

	/// <summary>
	/// The tile Y of the first solid tile under the creature's centre, or <c>-1</c> when no floor is
	/// inside the bounded probe window. This is the 水底 the creature settles onto and returns to.
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

	/// <summary>
	/// 只会在水底刷新, and only ever inside Yggdrasil (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c>
	/// returns early outside the subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// The water-bottom test is the design's own condition, kept distinct from an ordinary water spawn.
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The conservative rare / water-bottom weight, or <c>0f</c> outside the design's context.</returns>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		if (!KelpCurtainSpawnConditions.IsWaterBottom(spawnInfo))
		{
			return 0f;
		}

		return KelpCurtainSpawnConditions.RareWaterBottomWeight;
	}

	/// <summary>
	/// 死亡后有25%概率掉落 1 肉食性提灯，12.5%概率掉落1 灵灯. Both items were implemented in Phase 1
	/// (D-57), and 25% / 12.5% are the denominators 4 and 8. The design names no other drop, so no other
	/// rule is written.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MeatLantern>(), 4, 1, 1));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Photophore>(), 8, 1, 1));
	}
}
