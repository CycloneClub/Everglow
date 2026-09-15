using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Accessories;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs;

/// <summary>
/// 巨树人（Giant Tree Man），刺苔庭园（Spiny Moss Court）的稀有敌怪，一株死去的巨大树人的残骸。
/// Design row: 巨树人, region 刺苔庭园 / Spiny Moss Court, heading block id
/// Pm9jdOm1Zo9Tc4xy4lncGRtbnJh, stats table DLQGdWm3koGgwPxfI0qcU53gnec,
/// behaviour blocks Zm3ZdguPboviV1xCmcmcSIEznZd and Jmakdj0iIoqBkexBLoEcYHNUn1b (the
/// defence-zero vulnerability clause), drop block P1G9d2RrZovO8yxfXNucPNPInBh.
/// This file sits beside the already-approved GiantDandelion.png (214x263), so tML's default
/// texture resolution finds the art with no Texture override, no handwritten asset path and no
/// asset move.
/// </summary>
public class GiantDandelion : ModNPC
{
	/// <summary>The design's close range, in tiles: inside it the creature commits to the smash.</summary>
	private const float SmashRangeTiles = 5f;

	/// <summary>The design's mid range, in tiles: inside it the creature approaches and throws boulders.</summary>
	private const float BoulderRangeTiles = 15f;

	/// <summary>
	/// The aggro radius, in tiles: the 巨树人 only leaves its no-aggro Idle wander once a live
	/// player is inside this range, and the locomotion states fall back to Idle once the player
	/// escapes it. The design row gives no radius (its 有仇恨 clause is the only mention), so a
	/// conservative 30 tiles is used and recorded in 03-DEVIATIONS.md (D-34). It is wider than the
	/// 15-tile chase threshold, so every designed range — the 5-tile smash, the 5-15 tile mid-range
	/// and the beyond-15-tile faster chase — stays reachable.
	/// </summary>
	private const float AggroRangeTiles = 30f;

	/// <summary>
	/// The design's state-1 charge: slowly raising the arm and smashing the ground takes 120 ticks
	/// in total (behaviour block Zm3ZdguPboviV1xCmcmcSIEznZd).
	/// </summary>
	private const int SmashWindUpTicks = 120;

	/// <summary>
	/// The design's fixed 300-tick immobile recovery after the smash, with the fist stuck in the
	/// floor. During it 防御 is 0 and incoming damage is scaled to 150% (clause block
	/// Jmakdj0iIoqBkexBLoEcYHNUn1b).
	/// </summary>
	private const int SmashRecoveryTicks = 300;

	/// <summary>The design's mid-range backwards arm raise, also 120 ticks.</summary>
	private const int BoulderWindUpTicks = 120;

	/// <summary>
	/// The fast forward arm swing that brings the boulder up and releases it. The design gives no
	/// duration, so a conservative 20 ticks is used and recorded in 03-DEVIATIONS.md section 4
	/// (D-34).
	/// </summary>
	private const int BoulderThrowTicks = 20;

	/// <summary>The design's 120-tick arm recovery after the mid-range throw.</summary>
	private const int BoulderRecoverTicks = 120;

	/// <summary>
	/// The design's minimum gap between the mid-range state's two attacks (至少240帧的间隔).
	/// </summary>
	private const int MidRangeAttackGapTicks = 240;

	/// <summary>Idle wander speed, in pixels per tick.</summary>
	private const float WanderSpeed = 0.5f;

	/// <summary>5-15 tile approach speed, in pixels per tick.</summary>
	private const float ApproachSpeed = 1.5f;

	/// <summary>Beyond-15-tile chase speed, in pixels per tick: faster than the approach.</summary>
	private const float ChaseSpeed = 3.2f;

	/// <summary>
	/// The design's 伤害 cell: the shockwave row (50 at the normal difficulty scale), passed to
	/// the projectile at spawn. The melee contact value 70 is <c>NPC.damage</c>.
	/// </summary>
	private const int ShockwaveDamage = 50;

	/// <summary>The design's 伤害 cell: the boulder row (60 at the normal difficulty scale).</summary>
	private const int BoulderDamage = 60;

	/// <summary>The ground shockwave's travel speed, in pixels per tick (D-34).</summary>
	private const float ShockwaveSpeed = 6f;

	/// <summary>The thrown boulder's launch speed, in pixels per tick (D-34).</summary>
	private const float BoulderSpeed = 9f;

	/// <summary>防御 30（正常）.</summary>
	private const int NormalDefense = 30;

	/// <summary>防御 0（后摇）. </summary>
	private const int VulnerableDefense = 0;

	/// <summary>
	/// The design's three-range state machine (D-31: no vanilla <c>aiStyle</c> provides a
	/// charge/smash/recovery machine, so the class writes its own <c>AI()</c> and stores the state
	/// in <c>NPC.ai[0]</c>).
	/// </summary>
	private enum GiantDandelionState
	{
		/// <summary>No aggro: wandering with no target.</summary>
		Idle = 0,

		/// <summary>5-15 tiles: closing in while the mid-range attack is on cooldown.</summary>
		Approach = 1,

		/// <summary>Within 5 tiles: the 120-tick arm raise that ends in the ground smash.</summary>
		SmashWindUp = 2,

		/// <summary>The 300-tick immobile recovery, defence 0 and 150% incoming damage.</summary>
		SmashRecovery = 3,

		/// <summary>The mid-range 120-tick backwards arm raise.</summary>
		BoulderWindUp = 4,

		/// <summary>The fast forward arm swing that releases the boulder.</summary>
		BoulderThrow = 5,

		/// <summary>The 120-tick arm recovery after the mid-range throw.</summary>
		BoulderRecover = 6,

		/// <summary>Beyond 15 tiles with aggro: closing in faster than the approach.</summary>
		Chase = 7,
	}

	/// <summary>
	/// Named wrapper over <c>NPC.ai[0]</c> (the state machine) so no bare numeric index is
	/// scattered through the class.
	/// </summary>
	private GiantDandelionState State
	{
		get => (GiantDandelionState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the ticks remaining in the current timed state.</summary>
	private int StateTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[1]</c>: the mid-range attack cooldown, so two boulder
	/// attacks are at least <see cref="MidRangeAttackGapTicks"/> ticks apart.
	/// </summary>
	private int MidRangeAttackCooldown
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	public override void SetStaticDefaults()
	{
		// The approved sprite (214x263) is a single frame: textureHeight / textureWidth is not an
		// integer of at least 2, so the whole texture is one frame (the derived value is recorded
		// in 03-BIOLOGY.json as frame_count).
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// The design row's 免疫 cell is empty, so no specific debuff immunity is declared. The
		// 击退抗性 免疫击退 cell is expressed as NPC.knockBackResist = 0f in SetDefaults instead.
	}

	public override void SetDefaults()
	{
		// No vanilla aiStyle has a three-range charge/smash/recovery machine, so the class owns its
		// AI (D-31).
		NPC.aiStyle = -1;

		// Sprite-derived extents: 巨树人 has a single-frame 214x263 texture.
		NPC.width = 214;
		NPC.height = 263;

		// 生命 500 / 900 / 1300 is the difficulty progression; the normal-state value is the first.
		NPC.lifeMax = 500;
		NPC.life = 500;

		// 伤害 70 / 140 / 180 melee; the normal-state value is the first.
		NPC.damage = 70;
		NPC.defense = 30; // 防御 30（正常）/ 0（后摇）
		NPC.knockBackResist = 0f; // 击退抗性 免疫击退

		// 钱币（铜） 2金50银 = 25000 copper; NPC.value is documented in copper coins, so the design
		// value transposes 1:1 with no conversion (the 100x reading 250000 is rejected).
		NPC.value = 25000;

		// Conservative default: this row's stats table is headerless with six cells and its leading
		// cell reads 稀有; the design's 稀有度 column is empty in the sibling tables and absent here,
		// so ItemRarityID.LightPurple is a reading of that leading cell recorded in
		// 03-DEVIATIONS.md section 4, not a design-exact value. NPC.rarity is the engine's only NPC
		// rarity field (Lifeform Analyzer).
		NPC.rarity = ItemRarityID.LightPurple;

		NPC.friendly = false;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// Not 可被捕捉: only 水蛞蝓, 装甲虾 and 爆弹水母 are, so no catch item, no
		// Main.npcCatchable and no NPCID.Sets.CountsAsCritter is set here.
		NPC.catchItem = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The state machine runs on the authoritative side only (single player / server, D-35); the
	/// per-state motion below is applied on every side from the synced state, so a client never
	/// decides a transition or a spawn.
	/// </summary>
	public override void AI()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			RunStateMachine();
		}

		ApplyStateMotion();
	}

	/// <summary>
	/// The design's range selection, state timers and transitions. Only called on the authoritative
	/// side.
	/// </summary>
	private void RunStateMachine()
	{
		if (MidRangeAttackCooldown > 0)
		{
			MidRangeAttackCooldown--;
		}

		// The target is acquired or refreshed only while the creature is not locked in the
		// post-smash recovery, during which it is immobile with its fist in the floor.
		if (State != GiantDandelionState.SmashRecovery)
		{
			NPC.TargetClosest();
		}

		if (!IsTargetValid())
		{
			EnterState(GiantDandelionState.Idle);
			return;
		}

		Player target = Main.player[NPC.target];
		float distance = Vector2.Distance(NPC.Center, target.Center);

		if (StateTimer > 0)
		{
			StateTimer--;
		}

		// The design's state (0) is a no-aggro wander (行尸走肉般游荡在刺苔庭院的沼泽中) and its chase
		// clause is explicitly 有仇恨 (has aggro), so the creature only hunts within the aggro radius.
		// A committed smash or mid-range attack always completes, so the gate covers only the three
		// locomotion states, which fall back to Idle once the player escapes the radius.
		if ((State is GiantDandelionState.Idle or GiantDandelionState.Approach or GiantDandelionState.Chase)
			&& distance > AggroRangeTiles * 16f)
		{
			if (State != GiantDandelionState.Idle)
			{
				EnterState(GiantDandelionState.Idle);
			}

			return;
		}

		switch (State)
		{
			case GiantDandelionState.Idle:
			case GiantDandelionState.Approach:
			case GiantDandelionState.Chase:
				SelectLocomotionState(distance);
				break;

			case GiantDandelionState.SmashWindUp:
				if (StateTimer <= 0)
				{
					Smash();
				}
				break;

			case GiantDandelionState.SmashRecovery:
				if (StateTimer <= 0)
				{
					ExitSmashRecovery(target, distance);
				}
				break;

			case GiantDandelionState.BoulderWindUp:
				if (StateTimer <= 0)
				{
					EnterState(GiantDandelionState.BoulderThrow);
				}
				break;

			case GiantDandelionState.BoulderThrow:
				if (StateTimer <= 0)
				{
					ThrowBoulder(target, armMidRangeCooldown: true);
					EnterState(GiantDandelionState.BoulderRecover);
				}
				break;

			case GiantDandelionState.BoulderRecover:
				if (StateTimer <= 0)
				{
					EnterState(distance <= BoulderRangeTiles * 16f
						? GiantDandelionState.Approach
						: GiantDandelionState.Chase);
				}
				break;
		}
	}

	/// <summary>
	/// Picks the locomotion state for the current range: inside 5 tiles the smash, inside 15 the
	/// approach (which may interrupt itself with the mid-range boulder attack once the design's
	/// 240-tick gap has elapsed), beyond 15 the faster chase.
	/// </summary>
	private void SelectLocomotionState(float distance)
	{
		if (distance <= SmashRangeTiles * 16f)
		{
			EnterState(GiantDandelionState.SmashWindUp);
			return;
		}

		if (distance <= BoulderRangeTiles * 16f)
		{
			if (MidRangeAttackCooldown <= 0)
			{
				EnterState(GiantDandelionState.BoulderWindUp);
				return;
			}

			if (State != GiantDandelionState.Approach)
			{
				EnterState(GiantDandelionState.Approach);
			}
			return;
		}

		if (State != GiantDandelionState.Chase)
		{
			EnterState(GiantDandelionState.Chase);
		}
	}

	/// <summary>
	/// Sets the state and its duration. The defence swing (防御 30 -> 0 -> 30) is owned here so
	/// every exit from <see cref="GiantDandelionState.SmashRecovery"/> restores the normal value,
	/// whichever state follows. Only reached from the authoritative side, and the change is always
	/// synced with <c>NPC.netUpdate</c> (D-35).
	/// </summary>
	private void EnterState(GiantDandelionState next)
	{
		if (State == GiantDandelionState.SmashRecovery && next != GiantDandelionState.SmashRecovery)
		{
			NPC.defense = NormalDefense; // 防御 30（正常）restored on leaving the vulnerability window
		}

		State = next;

		switch (next)
		{
			case GiantDandelionState.SmashWindUp:
				StateTimer = SmashWindUpTicks;
				break;

			case GiantDandelionState.SmashRecovery:
				StateTimer = SmashRecoveryTicks;
				NPC.defense = VulnerableDefense; // 防御 0（后摇）
				break;

			case GiantDandelionState.BoulderWindUp:
				StateTimer = BoulderWindUpTicks;
				break;

			case GiantDandelionState.BoulderThrow:
				StateTimer = BoulderThrowTicks;
				break;

			case GiantDandelionState.BoulderRecover:
				StateTimer = BoulderRecoverTicks;
				break;

			default:
				StateTimer = 0;
				break;
		}

		NPC.netUpdate = true;
	}

	/// <summary>
	/// The end of the 120-tick charge: the design's ground smash that produces a high-damage,
	/// large-radius shockwave with roughly a 2-tile-high hit box, followed by the fixed 300-tick
	/// recovery with the fist stuck in the floor.
	/// </summary>
	private void Smash()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			// The wave is spawned on the authoritative side and synced, never duplicated per client.
			int direction = NPC.direction == 0 ? 1 : NPC.direction;
			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(ShockwaveSpeed * direction, 0f), ModContent.ProjectileType<GiantDandelion_Shockwave>(), ShockwaveDamage, 0f);
		}

		EnterState(GiantDandelionState.SmashRecovery);
	}

	/// <summary>
	/// The end of the 300-tick recovery: the design's arm yank that flings a boulder forward,
	/// then a repeat only while the player is still inside the smash range (otherwise the creature
	/// resumes closing in).
	/// </summary>
	private void ExitSmashRecovery(Player target, float distance)
	{
		ThrowBoulder(target, armMidRangeCooldown: false);

		if (distance <= SmashRangeTiles * 16f)
		{
			EnterState(GiantDandelionState.SmashWindUp);
			return;
		}

		EnterState(distance <= BoulderRangeTiles * 16f
			? GiantDandelionState.Approach
			: GiantDandelionState.Chase);
	}

	/// <summary>
	/// The design's throw, used both by the mid-range swing and by the post-recovery yank: a
	/// boulder is flung toward the current target. The 240-tick gap applies only to the two attacks
	/// of the mid-range state (这个状态下两段攻击间隔不低于240帧), so the cooldown is armed only when
	/// <paramref name="armMidRangeCooldown"/> is set by the mid-range path; the post-smash yank is a
	/// different attack and must not suppress the next mid-range boulder.
	/// </summary>
	/// <param name="target">The player to fling the boulder toward.</param>
	/// <param name="armMidRangeCooldown">
	/// True only from the mid-range <see cref="GiantDandelionState.BoulderThrow"/> path; the
	/// post-recovery yank passes false.
	/// </param>
	private void ThrowBoulder(Player target, bool armMidRangeCooldown)
	{
		if (armMidRangeCooldown)
		{
			MidRangeAttackCooldown = MidRangeAttackGapTicks;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient && target is not null)
		{
			// The boulder is spawned on the authoritative side and synced, never duplicated per client.
			int direction = NPC.direction == 0 ? 1 : NPC.direction;
			Vector2 armPosition = NPC.Center + new Vector2(NPC.width * 0.35f * direction, -NPC.height * 0.25f);
			Vector2 velocity = (target.Center - armPosition).SafeNormalize(new Vector2(direction, 0f)) * BoulderSpeed;
			Projectile.NewProjectile(NPC.GetSource_FromAI(), armPosition, velocity, ModContent.ProjectileType<GiantDandelion_Boulder>(), BoulderDamage, 0f);
		}
	}

	/// <summary>
	/// The per-state motion, applied on every side from the synced state: the smash and boulder
	/// states hold the creature still or slow it down, the locomotion states walk, approach or
	/// chase. Gravity and tile collision are the engine's, exactly as they are for 格普螺.
	/// </summary>
	private void ApplyStateMotion()
	{
		Player target = GetTarget();

		switch (State)
		{
			case GiantDandelionState.SmashWindUp:
				// Slowly raising the arm: the creature barely moves while it charges.
				NPC.velocity.X *= 0.8f;
				FaceTarget(target);
				break;

			case GiantDandelionState.SmashRecovery:
				// 拳头插在地里: completely immobile for the whole 300 ticks.
				NPC.velocity.X = 0f;
				break;

			case GiantDandelionState.BoulderWindUp:
			case GiantDandelionState.BoulderRecover:
				NPC.velocity.X *= 0.85f;
				FaceTarget(target);
				break;

			case GiantDandelionState.BoulderThrow:
				NPC.velocity.X *= 0.9f;
				FaceTarget(target);
				break;

			case GiantDandelionState.Idle:
				WanderSlowly();
				break;

			case GiantDandelionState.Approach:
				MoveToward(target, ApproachSpeed);
				break;

			case GiantDandelionState.Chase:
				MoveToward(target, ChaseSpeed);
				break;
		}
	}

	/// <summary>
	/// The design's no-aggro wander: a slow walk that reverses at a wall or at an unwalkable ledge.
	/// Deterministic, so the server and every client agree without extra state.
	/// </summary>
	private void WanderSlowly()
	{
		if (NPC.direction == 0)
		{
			NPC.direction = 1;
		}

		if (NPC.collideX || !HasFloorAhead())
		{
			NPC.direction *= -1;
		}

		NPC.spriteDirection = NPC.direction;
		NPC.velocity.X = WanderSpeed * NPC.direction;
	}

	/// <summary>Walks toward the target at the state's speed, falling back to a wander when the target is gone.</summary>
	private void MoveToward(Player target, float speed)
	{
		if (target is null)
		{
			WanderSlowly();
			return;
		}

		NPC.direction = target.Center.X >= NPC.Center.X ? 1 : -1;
		NPC.spriteDirection = NPC.direction;
		NPC.velocity.X = speed * NPC.direction;
	}

	/// <summary>Faces the target when one exists, so the charge and the throw read as aimed.</summary>
	private void FaceTarget(Player target)
	{
		if (target is null)
		{
			return;
		}

		NPC.direction = target.Center.X >= NPC.Center.X ? 1 : -1;
		NPC.spriteDirection = NPC.direction;
	}

	/// <summary>
	/// True when the tile just below and ahead of the leading edge is solid, i.e. the walker can
	/// keep going. A world-edge position reports true so the engine boundary handles it.
	/// </summary>
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

	/// <summary>The current target, or null when the synced <c>NPC.target</c> slot does not hold a live player.</summary>
	private Player GetTarget()
	{
		if (!IsTargetValid())
		{
			return null;
		}
		return Main.player[NPC.target];
	}

	/// <summary>
	/// True when <c>NPC.target</c> indexes a live, non-dead player. <c>NPC.target</c> uses the
	/// engine's "no target" sentinel, which is outside the player array, so the slot is bounds
	/// checked before it is read.
	/// </summary>
	private bool IsTargetValid()
	{
		int index = NPC.target;
		if (index < 0 || index >= Main.player.Length)
		{
			return false;
		}

		Player player = Main.player[index];
		return player.active && !player.dead;
	}

	/// <summary>
	/// The design's 后摇 vulnerability clause (block id Jmakdj0iIoqBkexBLoEcYHNUn1b): while the
	/// fist is sunk in the floor the creature takes 150% damage. Only the multiplier lives here;
	/// the 防御 30 -> 0 switch is owned by the state transition in <see cref="EnterState"/> so it
	/// stays server-authoritative and is not counted twice.
	/// </summary>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
		if (State == GiantDandelionState.SmashRecovery)
		{
			modifiers.FinalDamage *= 1.5f; // 后摇受到150%的伤害
		}
	}

	/// <summary>
	/// The death dust, reusing the KelpCurtain dust 河流蛞蝓 already emits rather than creating a
	/// new one. The whole burst sits inside <c>!Main.dedServ</c> so it never runs on a dedicated
	/// server (D-35).
	/// </summary>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0 && !Main.dedServ)
		{
			for (int i = 0; i < 8; i++)
			{
				int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LichenSlime>(), 2 * hit.HitDirection, -2f);
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
	}

	/// <summary>
	/// Subworld-only spawning (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c> returns early outside
	/// Yggdrasil, so the per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because
	/// this hook runs in single player or on the server only, where the client camera is zero.
	/// This creature is a land walker, so submerged spawn tiles and water spawns are rejected. The
	/// design calls it 稀有 and supplies no weight, so a deliberately low 0.25f (band 0.1f-0.5f,
	/// lower than every other tranche creature) is used (D-34).
	/// </summary>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		if (spawnInfo.Water)
		{
			return 0f;
		}

		int tileX = spawnInfo.SpawnTileX;
		int tileY = spawnInfo.SpawnTileY;
		if (WorldGen.InWorld(tileX, tileY, 1) && Main.tile[tileX, tileY].LiquidAmount > 0)
		{
			return 0f;
		}

		return 0.25f;
	}

	/// <summary>
	/// The design's 一定掉落 drops: 巨树之臂, 硬化枯木心脏 and 巨石弹射装置 are all guaranteed, and
	/// all three are Phase 1-2 items that already exist (D-37). The design's 4~6 枯木碎块 has no
	/// repository ModItem, so no rule references it — that gap is a blocker in 03-BIOLOGY.json and
	/// 03-DEVIATIONS.md section 5, not a type reference (D-39).
	/// </summary>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ArmOfGiantTree>(), 1, 1, 1));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HardenedWitherbarkHeart>(), 1, 1, 1));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BoulderCatapult>(), 1, 1, 1));
	}
}
