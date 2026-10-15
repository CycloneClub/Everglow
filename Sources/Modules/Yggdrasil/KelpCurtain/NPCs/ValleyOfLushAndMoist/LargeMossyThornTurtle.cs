using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist;

/// <summary>
/// 大型荆棘苔龟（Large Mossy Thorn Turtle），森雨幽谷（Valley of Lush and Moist）的 Mini Boss，
/// 背上长满石笋和苔藓的大王八。
/// Design row: 大型荆棘苔龟, heading block id doxcnzBmJS1yk7hq4csTiRt20ut,
/// behavior block TyxWdwlaMoE4mhxYWfEcM9gtnUg, drop block E3X5dKd17ohqDExLK0dcZpVCnyh.
/// 生命 1000 / 伤害 50/100/150（接触）85/160/240（旋转）70/140/200（震荡波）55/100/150（巨石）/
/// 防御 10（正常）999（缩壳）/ 击退抗性 免疫击退 / 减伤 20 / 免疫 中毒、困惑 / 钱币（铜）1金 = 10000.
/// <para>
/// <b>Four-state machine (OQ5/D-47, 04-DEVIATIONS.md section 8).</b> The design row is complete enough for
/// the full implementation, so no state-3 blocker is recorded:
/// </para>
/// <list type="number">
/// <item>缩壳 (<see cref="LargeMossyThornTurtleState.Retracted"/>) - defence 999, a 20% reflect of the
/// incoming damage clamped to 2-20, exiting after more than 100 frames without taking damage or after more
/// than 320 frames in the shell, then into state 2.</item>
/// <item>伸出头震击地板 (<see cref="LargeMossyThornTurtleState.Slamming"/>) - a floor slam that emits
/// <see cref="LargeMossyThornTurtle_Shockwave"/>, repeated once after 120 frames, then 120 more frames to
/// state 3.</item>
/// <item>缩壳飞天下坠 (<see cref="LargeMossyThornTurtleState.AerialSlam"/>) - the flight ignores tile
/// collision, lands with one more floor slam, drops three evenly spaced
/// <see cref="LargeMossyThornTurtle_Boulder"/> from above the screen, then state 4.</item>
/// <item>伸头远遁 (<see cref="LargeMossyThornTurtleState.Retreating"/>) - head out, crawling away from the
/// player at 2 tiles/s for 240 frames, then back to state 1 with its back to the player.</item>
/// </list>
/// <para>
/// <b>The defence hazard is closed structurally (T-04-48).</b> The design's 防御 999 is assigned in exactly
/// one place - the <see cref="EnterState"/> helper - which writes 999 for the two retracted states and
/// restores the normal value from <c>NPC.defDefense</c> on every other entry. The flight's target-loss and
/// timeout paths both end in <see cref="EnterState"/>, so no exit path can leave the mini-boss permanently
/// invulnerable.
/// </para>
/// <para>
/// <b>Damage progressions take their first value (the Phase 3 rule).</b> The design's slash-separated cells
/// resolve to 50 contact / 85 rotated contact / 70 shockwave / 55 boulder here; the contact escalation is
/// carried by the state through <see cref="PostAI"/>, and the shockwave and boulder values are passed to
/// their projectiles at spawn.
/// </para>
/// <para>
/// <b>Reflect locality (D-55, the Phase 3 荆棘苔龟 precedent).</b> tML invokes <c>OnHitByItem</c> for the
/// damaging player on their own client, so a netmode guard there would make the reflect dead code. The
/// reflect therefore lives in <see cref="OnHitByItem"/> / <see cref="OnHitByProjectile"/>, is gated on the
/// retracted states and on <c>player.whoAmI == Main.myPlayer</c> so exactly one client applies it, is never
/// written in <c>ModifyIncomingHit</c>, and lets <c>Player.Hurt</c> perform its normal client-to-server
/// hurt sync.
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. When the art arrives the D-49 migration is adding
/// LargeMossyThornTurtle.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime
/// asset path is Everglow/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/LargeMossyThornTurtle.png.
/// </para>
/// </summary>
public class LargeMossyThornTurtle : ModNPC
{
	/// <summary>Pixels per tile, so a world-space distance or speed can be expressed in the design's tiles.</summary>
	private const float PixelsPerTile = 16f;

	/// <summary>
	/// The engine's tick rate, so the design's per-second crawl speeds can be written as 格/s: 1 格/s is
	/// <c>PixelsPerTile / TicksPerSecond</c> pixels per tick.
	/// </summary>
	private const float TicksPerSecond = 60f;

	/// <summary>
	/// 当玩家对王八造成伤害且王八生命值不高于900时，进入敌对状态: the life threshold the hostility gate reads.
	/// </summary>
	private const int HostileLifeThreshold = 900;

	/// <summary>当超过100帧没有受到伤害: the retracted state's no-damage exit, in frames.</summary>
	private const int RetractNoDamageFrames = 100;

	/// <summary>或者缩壳时间超过320帧后: the retracted state's hard exit, in frames.</summary>
	private const int RetractHardFrames = 320;

	/// <summary>间隔120帧后重复1次: the gap between the two state-2 floor slams, in frames.</summary>
	private const int SlamRepeatFrames = 120;

	/// <summary>间隔120帧后进入状态3: the gap between the second slam and state 3, in frames.</summary>
	private const int SlamToAerialFrames = 120;

	/// <summary>从上方屏幕外均匀落下3颗巨石: the boulder-rain count.</summary>
	private const int BoulderCount = 3;

	/// <summary>爬行240帧: the retreat's fixed duration, in frames.</summary>
	private const int RetreatFrames = 240;

	/// <summary>
	/// 1格/s: the passive crawl speed. The design calls it 非常非常缓慢, and this is the literal
	/// per-second value rather than a vanilla-derived walk speed.
	/// </summary>
	private const float PassiveSpeed = PixelsPerTile / TicksPerSecond;

	/// <summary>2格/s: the retreat speed (王八飞快地爬的速度).</summary>
	private const float RetreatSpeed = PixelsPerTile * 2f / TicksPerSecond;

	/// <summary>999（缩壳）: the design's literal retracted defence, unbounded in tML so it needs no clamp.</summary>
	private const int RetractedDefense = 999;

	/// <summary>减伤 20: incoming damage on the open-shell state is scaled to 80%.</summary>
	private const float DamageReduction = 0.8f;

	/// <summary>玩家如果攻击会受到自身攻击伤害的20%反伤: the reflect ratio.</summary>
	private const float ReflectDamageRatio = 0.2f;

	/// <summary>取值范围为2~20: the reflect clamp's lower bound.</summary>
	private const float ReflectMinDamage = 2f;

	/// <summary>取值范围为2~20: the reflect clamp's upper bound.</summary>
	private const float ReflectMaxDamage = 20f;

	/// <summary>震荡波 70/140/200: the normal-state shockwave damage, passed to the projectile at spawn.</summary>
	private const int ShockwaveDamage = 70;

	/// <summary>巨石 55/100/150: the normal-state boulder damage, passed to the projectile at spawn.</summary>
	private const int BoulderDamage = 55;

	/// <summary>
	/// 旋转 85/160/240: the rotated-contact damage (the first value of the progression, the Phase 3 rule),
	/// re-asserted from the synced state in <see cref="PostAI"/> because <c>NPC.damage</c> is not part of the
	/// NPC net message.
	/// </summary>
	private const int RotatedContactDamage = 85;

	/// <summary>The state-3 launch speed, in pixels per tick (conservative default, D-54).</summary>
	private const float FlightLaunchSpeed = 9f;

	/// <summary>The state-3 fall acceleration, in pixels per tick squared (conservative default, D-54).</summary>
	private const float FlightGravity = 0.4f;

	/// <summary>The state-3 terminal fall speed, in pixels per tick (conservative default, D-54).</summary>
	private const float FlightMaxFallSpeed = 14f;

	/// <summary>
	/// The state-3 hard cap, in frames: the flight always lands within this window even if it never finds a
	/// floor beneath it, so no bounded state can soft-lock the mini-boss (T-04-53).
	/// </summary>
	private const int FlightMaxFrames = 180;

	/// <summary>
	/// The horizontal gap between two boulder-rain columns, in tiles: three boulders at -10 / 0 / +10 tiles,
	/// i.e. 均匀落下 (conservative default, D-54).
	/// </summary>
	private const float BoulderSpreadTiles = 10f;

	/// <summary>
	/// How far above the landing point the boulders are spawned, in tiles: 从上方屏幕外, spelled as a world
	/// distance rather than a screen value, because a screen value is client-only and this spawn is
	/// server-authoritative (D-55, conservative default, D-54).
	/// </summary>
	private const float BoulderSpawnHeightTiles = 30f;

	/// <summary>The boulder rain's initial downward speed, in pixels per tick (conservative default, D-54).</summary>
	private const float BoulderFallSpeed = 6f;

	/// <summary>
	/// The design's four-state Mini Boss machine. No vanilla <c>aiStyle</c> provides it, so the class owns
	/// its <c>AI()</c> and stores the state in <c>NPC.ai[0]</c> (D-31).
	/// </summary>
	private enum LargeMossyThornTurtleState
	{
		/// <summary>0: the passive, non-hostile ground crawl at 1 格/s.</summary>
		Passive = 0,

		/// <summary>1: 缩壳 - defence 999, the clamped 20% reflect and the two exit timers.</summary>
		Retracted = 1,

		/// <summary>2: 伸出头 - the floor slam, its repeat and the hand-off to state 3.</summary>
		Slamming = 2,

		/// <summary>3: 缩壳飞天下坠 - the collision-ignoring flight, the landing slam and the boulder rain.</summary>
		AerialSlam = 3,

		/// <summary>4: 伸出头远遁 - the 240-frame retreat away from the player at 2 格/s.</summary>
		Retreating = 4,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private LargeMossyThornTurtleState State
	{
		get => (LargeMossyThornTurtleState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames elapsed in the current state.</summary>
	private int StateTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[1]</c>: the elapsed-in-state tick at which the next state-2 floor
	/// slam fires. It starts at 0 (so the first slam is immediate) and advances by
	/// <see cref="SlamRepeatFrames"/> after each slam.
	/// </summary>
	private int SlamTimer
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[2]</c>: the frames since the last damage, which is what 超过100帧
	/// 没有受到伤害 measures. It is a per-NPC timer, never a field on a <c>Player</c> (Pitfall 6).
	/// </summary>
	private int NoDamageTimer
	{
		get => (int)NPC.localAI[2];
		set => NPC.localAI[2] = value;
	}

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

		// 免疫 中毒、困惑 (the design row's 免疫 cell): exactly these two immunities.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override void SetDefaults()
	{
		// No vanilla aiStyle provides this four-state Mini Boss machine, so the class owns its AI (D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative large Mini Boss box is used and is the first value
		// to revisit when the art arrives (D-54).
		NPC.width = 110;
		NPC.height = 80;

		NPC.lifeMax = 1000; // 生命 1000
		NPC.life = 1000;

		NPC.damage = 50; // 伤害 50（正常接触）
		NPC.defDamage = 50;

		NPC.defense = 10; // 防御 10（正常）
		NPC.defDefense = 10;

		NPC.knockBackResist = 0f; // 击退抗性 免疫击退

		NPC.value = 10000; // 钱币（铜） 1金 = 10000 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty, so no design rarity exists. NPC.rarity is
		// the engine's only NPC rarity field (the API-name correction, 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.friendly = false;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// Not 可被捕捉 (the design gives no capture clause for this row), so no catch item and no catchable
		// or critter-count flag is written here.
		NPC.catchItem = 0;

		// The passive form 不会穿墙, so the engine's default tile collision and gravity are kept; only the
		// state-3 flight turns them off, and it turns them back on through EnterState on every exit path.
		NPC.noTileCollide = false;
		NPC.noGravity = false;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The state machine runs on the authoritative side only (D-55); the per-state motion below is then
	/// applied on every side from the synced state, so a client never decides a transition or spawns a
	/// projectile.
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
	/// The design's four states, their exact frame budgets and their transitions. Only called on the
	/// authoritative side, and every transition goes through <see cref="EnterState"/>.
	/// </summary>
	private void RunStateMachine()
	{
		switch (State)
		{
			case LargeMossyThornTurtleState.Passive:
				// 并不会主动攻击玩家: the passive crawl has no transition of its own. The hostility gate is
				// damage-driven and lives in HitEffect (当玩家对王八造成伤害且王八生命值不高于900时).
				break;

			case LargeMossyThornTurtleState.Retracted:
				// 1、缩壳: the shell exits either when it has gone more than 100 frames without being hit or
				// when it has held the shell for more than 320 frames.
				StateTimer++;
				NoDamageTimer++;
				if (NoDamageTimer > RetractNoDamageFrames || StateTimer > RetractHardFrames)
				{
					// 伸出头，进入状态2.
					EnterState(LargeMossyThornTurtleState.Slamming);
				}
				break;

			case LargeMossyThornTurtleState.Slamming:
				// 2、伸出头，震击地板: two slams 120 frames apart, then 120 more frames to state 3.
				StateTimer++;
				if (StateTimer >= SlamRepeatFrames + SlamToAerialFrames)
				{
					EnterState(LargeMossyThornTurtleState.AerialSlam);
					break;
				}

				if (StateTimer >= SlamTimer)
				{
					// The first slam fires on this state's first authoritative tick, the second 120 later.
					Slam();
					SlamTimer += SlamRepeatFrames;
				}
				break;

			case LargeMossyThornTurtleState.AerialSlam:
				// 3、缩壳，飞天下坠，但是无视物块碰撞: the flight is driven here because tile collision is off,
				// so the landing is found by probing rather than by the engine.
				StateTimer++;
				NPC.velocity.Y += FlightGravity;
				if (NPC.velocity.Y > FlightMaxFallSpeed)
				{
					NPC.velocity.Y = FlightMaxFallSpeed;
				}

				if ((NPC.velocity.Y > 0f && HasFloorBelow()) || StateTimer >= FlightMaxFrames)
				{
					LandFromFlight();
				}
				break;

			case LargeMossyThornTurtleState.Retreating:
				// 4、伸出头，向着远离玩家的方向以2格/s爬行240帧.
				StateTimer++;
				if (StateTimer >= RetreatFrames)
				{
					// 然后背对玩家回到状态1.
					EnterState(LargeMossyThornTurtleState.Retracted);
				}
				break;
		}
	}

	/// <summary>
	/// Sets the state and everything the state owns: the defence switch (999 while retracted, the design's
	/// normal 10 on every other entry), the flight's tile-collision and gravity flags, the state timers and
	/// the entry velocity. Owning the defence here - and nowhere else - is what makes state 4 reachable
	/// without starting at 999 and what closes T-04-48 on every exit path, including a target lost mid-flight.
	/// Only reached from the authoritative side, and the change is always synced with <c>NPC.netUpdate</c>
	/// (D-55).
	/// </summary>
	/// <param name="next">The state to enter.</param>
	private void EnterState(LargeMossyThornTurtleState next)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || State == next)
		{
			return;
		}

		State = next;

		// 防御 999（缩壳） for the two retracted states, 防御 10（正常） everywhere else, including a target
		// lost mid-flight: this is the single place where the design's defence switch is written, so
		// NPC.defense = 999 can never leak into state 4 (T-04-48).
		bool retracted = next is LargeMossyThornTurtleState.Retracted or LargeMossyThornTurtleState.AerialSlam;
		NPC.defense = retracted ? RetractedDefense : NPC.defDefense;

		// Only the state-3 flight ignores tile collision; every other entry restores both flags.
		NPC.noTileCollide = next == LargeMossyThornTurtleState.AerialSlam;
		NPC.noGravity = next == LargeMossyThornTurtleState.AerialSlam;

		StateTimer = 0;

		switch (next)
		{
			case LargeMossyThornTurtleState.Retracted:
				SlamTimer = 0;
				NoDamageTimer = 0;
				NPC.velocity = Vector2.Zero;
				break;

			case LargeMossyThornTurtleState.Slamming:
				// The slam schedule restarts at 0, so the first slam of this state is immediate.
				SlamTimer = 0;
				NPC.velocity = Vector2.Zero;
				break;

			case LargeMossyThornTurtleState.AerialSlam:
				NPC.velocity = GetFlightLaunchVelocity();
				break;

			case LargeMossyThornTurtleState.Retreating:
				NPC.direction = GetRetreatDirection();
				NPC.spriteDirection = NPC.direction;
				NPC.velocity.X = RetreatSpeed * NPC.direction;
				break;

			default:
				break;
		}

		NPC.netUpdate = true;
	}

	/// <summary>
	/// The per-state motion, applied on every side from the synced state. The walk states are deterministic
	/// functions of the synced state and the synced target slot, so a client reproduces them without
	/// deciding anything; the flight is server-driven and a client follows the synced NPC velocity and
	/// position instead of re-applying gravity a second time. The flight's <c>noTileCollide</c> /
	/// <c>noGravity</c> flags are re-derived here on every side too, because they are not part of the NPC
	/// net message: without this a client keeps gravity and tile collision through the state-3 dive and
	/// snaps back on the next transition (D-55).
	/// </summary>
	private void ApplyStateMotion()
	{
		// 无视物块碰撞 and no gravity for the dive only, re-asserted on every side from the synced State.
		bool flying = State == LargeMossyThornTurtleState.AerialSlam;
		NPC.noTileCollide = flying;
		NPC.noGravity = flying;

		switch (State)
		{
			case LargeMossyThornTurtleState.Retracted:
			case LargeMossyThornTurtleState.Slamming:
				// 缩壳 and 震击地板 are stationary.
				NPC.velocity.X = 0f;
				break;

			case LargeMossyThornTurtleState.AerialSlam:
				// The flight is authoritative only; the client reads the synced velocity/position.
				break;

			case LargeMossyThornTurtleState.Retreating:
				NPC.direction = GetRetreatDirection();
				NPC.spriteDirection = NPC.direction;
				NPC.velocity.X = RetreatSpeed * NPC.direction;
				break;

			default:
				CrawlPassively();
				break;
		}
	}

	/// <summary>
	/// 正常形态下...会在地面上非常非常缓慢（1格/s）地爬行，并不会主动攻击玩家: the passive ground crawl. It never
	/// acquires a target and reverses at a wall or at an unwalkable ledge, exactly as the family's 格普螺
	/// crawler does, so the passive form cannot leave its ledge or push into terrain forever.
	/// </summary>
	private void CrawlPassively()
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
		NPC.velocity.X = PassiveSpeed * NPC.direction;
	}

	/// <summary>
	/// 像原版王八一样飞天下坠: the state-3 launch - up and slightly toward the current target, so the dive
	/// reads as aimed. The target slot is the synced one, never the local player (D-52/D-55).
	/// </summary>
	/// <returns>The launch velocity.</returns>
	private Vector2 GetFlightLaunchVelocity()
	{
		Player target = GetTarget();
		float horizontalDirection = target is not null
			? Math.Sign(target.Center.X - NPC.Center.X)
			: (NPC.direction == 0 ? 1 : NPC.direction);
		if (horizontalDirection == 0f)
		{
			horizontalDirection = 1f;
		}

		return new Vector2(horizontalDirection * FlightLaunchSpeed * 0.5f, -FlightLaunchSpeed);
	}

	/// <summary>
	/// 向着远离玩家的方向: the retreat direction - away from the current target, falling back to the current
	/// facing when no target is live. Reads only the synced <c>NPC.target</c> slot and the synced player
	/// array, never the local player (D-52).
	/// </summary>
	/// <returns>1 to retreat right, -1 to retreat left.</returns>
	private int GetRetreatDirection()
	{
		Player target = GetTarget();
		if (target is null)
		{
			return NPC.direction == 0 ? 1 : NPC.direction;
		}

		return NPC.Center.X >= target.Center.X ? 1 : -1;
	}

	/// <summary>
	/// 震击地板，产生范围的震荡波: one floor slam, spawned on the authoritative side and synced so a client
	/// never duplicates it (D-55).
	/// </summary>
	private void Slam()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<LargeMossyThornTurtle_Shockwave>(), ShockwaveDamage, 0f);
		}
	}

	/// <summary>
	/// 落地后震击一次地板，然后从上方屏幕外均匀落下3颗巨石，然后进入状态4: the landing slam, the three-boulder
	/// rain and the hand-off to the retreat. The descent zeroes the flight velocity before the slams so the
	/// mini-boss does not keep sinking while it attacks.
	/// </summary>
	private void LandFromFlight()
	{
		NPC.velocity = Vector2.Zero;

		// 落地后震击一次地板.
		Slam();

		// 然后从上方屏幕外均匀落下3颗巨石.
		SpawnBoulderRain();

		// 然后进入状态4.
		EnterState(LargeMossyThornTurtleState.Retreating);
	}

	/// <summary>
	/// 从上方屏幕外均匀落下3颗巨石: three boulders dropped from a fixed world height above the landing point
	/// at three evenly spaced columns. The height is a world distance rather than a screen value, because a
	/// screen coordinate is client-only and this spawn is server-authoritative (D-52/D-55). Every boulder is
	/// spawned under the same authoritative guard.
	/// </summary>
	private void SpawnBoulderRain()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		float centerX = NPC.Center.X;
		float spawnY = NPC.Center.Y - BoulderSpawnHeightTiles * PixelsPerTile;
		float middleIndex = (BoulderCount - 1) / 2f;

		for (int i = 0; i < BoulderCount; i++)
		{
			float offsetTiles = (i - middleIndex) * BoulderSpreadTiles;
			Vector2 position = new Vector2(centerX + offsetTiles * PixelsPerTile, spawnY);
			Projectile.NewProjectile(NPC.GetSource_FromAI(), position, new Vector2(0f, BoulderFallSpeed), ModContent.ProjectileType<LargeMossyThornTurtle_Boulder>(), BoulderDamage, 0f);
		}
	}

	/// <summary>
	/// True when a solid tile sits just below the mini-boss, which is how the tile-collision-ignoring flight
	/// finds its landing. The centre and both bottom edges are sampled, so a single hole under one column
	/// does not let the flight fall through the floor. A world-edge position reports true so the engine
	/// boundary handles it.
	/// </summary>
	/// <returns>True when the flight has reached a floor.</returns>
	private bool HasFloorBelow()
	{
		int probeTileY = (int)((NPC.Bottom.Y + 8f) / PixelsPerTile);
		int leftTileX = (int)(NPC.Left.X / PixelsPerTile);
		int centerTileX = (int)(NPC.Center.X / PixelsPerTile);
		int rightTileX = (int)(NPC.Right.X / PixelsPerTile);

		return ProbeSolidTile(leftTileX, probeTileY)
			|| ProbeSolidTile(centerTileX, probeTileY)
			|| ProbeSolidTile(rightTileX, probeTileY);
	}

	/// <summary>True when that tile is outside the world (the boundary case) or holds a solid tile.</summary>
	/// <param name="tileX">The tile column to probe.</param>
	/// <param name="tileY">The tile row to probe.</param>
	/// <returns>True when the flight should treat the column as landed.</returns>
	private static bool ProbeSolidTile(int tileX, int tileY)
	{
		if (!WorldGen.InWorld(tileX, tileY, 1))
		{
			return true;
		}

		return Main.tile[tileX, tileY].HasTile;
	}

	/// <summary>
	/// True when the tile just below and ahead of the leading edge is solid, i.e. the passive crawler can
	/// keep going. A world-edge position reports true so the engine boundary handles it.
	/// </summary>
	/// <returns>True when the crawler has a floor ahead of it.</returns>
	private bool HasFloorAhead()
	{
		int checkTileX = (int)((NPC.Center.X + (NPC.width / 2f + 8f) * NPC.direction) / PixelsPerTile);
		int checkTileY = (int)((NPC.Bottom.Y + 8f) / PixelsPerTile);
		if (!WorldGen.InWorld(checkTileX, checkTileY, 1))
		{
			return true;
		}
		return Main.tile[checkTileX, checkTileY].HasTile;
	}

	/// <summary>The current target, or <c>null</c> when the synced <c>NPC.target</c> slot does not hold a live player.</summary>
	/// <returns>The target player, or <c>null</c>.</returns>
	private Player GetTarget()
	{
		if (!IsTargetValid())
		{
			return null;
		}
		return Main.player[NPC.target];
	}

	/// <summary>
	/// True when <c>NPC.target</c> indexes a live, non-dead player. <c>NPC.target</c> uses the engine's "no
	/// target" sentinel, which is outside the player array, so the slot is bounds checked before it is read.
	/// </summary>
	/// <returns>True when the target slot holds a live player.</returns>
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
	/// 减伤 20: incoming damage is scaled to 80%. The design's 减伤 applies on the open-shell form, and the
	/// reflect is deliberately <b>not</b> written here - this hook may only change <c>HitModifiers</c>
	/// properties, and tML does not invoke the on-hit hooks beside it on the server (D-55).
	/// </summary>
	/// <param name="modifiers">The hit's modifiers; only <c>FinalDamage</c> is touched.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
		modifiers.FinalDamage *= DamageReduction; // 减伤 20
	}

	/// <summary>
	/// The authoritative on-hit reaction: the state-1 no-damage timer is reset (超过100帧没有受到伤害) and the
	/// minimal hostility gate opens (当玩家对王八造成伤害且王八生命值不高于900时，进入敌对状态). It lives here rather
	/// than in <c>ModifyIncomingHit</c> because side effects belong to the OnHit hooks, and it is guarded so
	/// the transition is written once on the authoritative side with an <c>NPC.netUpdate</c> (D-55).
	/// </summary>
	/// <param name="hit">The resolved hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0)
		{
			// The death dust. Client-only work, so the whole burst sits behind the dedicated-server guard
			// (D-35/D-55) and reuses an existing Kelp Curtain dust rather than creating a new dust class.
			if (!Main.dedServ)
			{
				for (int i = 0; i < 8; i++)
				{
					int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LichenSlime>(), 2 * hit.HitDirection, -2f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].scale = Main.rand.NextFloat(0.9f, 1.4f);
				}
			}
			return;
		}

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		if (State == LargeMossyThornTurtleState.Retracted)
		{
			// 当超过100帧没有受到伤害: any hit restarts the no-damage window.
			NoDamageTimer = 0;
		}
		else if (State == LargeMossyThornTurtleState.Passive && NPC.life <= HostileLifeThreshold)
		{
			// 进入敌对状态: the passive form becomes hostile only once it is damaged at or below 900 life.
			EnterState(LargeMossyThornTurtleState.Retracted);
		}
	}

	/// <summary>
	/// The reflect: 此时玩家如果攻击会受到自身攻击伤害的20%反伤，取值范围为2~20, applied while the shell is closed
	/// (states 1 and 3) to the attacking player, exactly once, on the client that dealt the damage. tML does
	/// not invoke this hook on the server, so a netmode guard here would make the reflect dead code (D-55).
	/// </summary>
	/// <param name="player">The player who dealt the melee damage.</param>
	/// <param name="item">The item used.</param>
	/// <param name="hit">The resolved hit.</param>
	/// <param name="damageDone">The damage the attack dealt.</param>
	public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		ReflectDamageOn(player, damageDone);
	}

	/// <summary>
	/// The same reflect for a ranged hit, so a projectile attacker is answered too. The attacker is derived
	/// from the synced projectile owner and the once-only <c>Main.myPlayer</c> guard is applied identically.
	/// </summary>
	/// <param name="projectile">The projectile that dealt the damage.</param>
	/// <param name="hit">The resolved hit.</param>
	/// <param name="damageDone">The damage the projectile dealt.</param>
	public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		int owner = projectile.owner;
		if (owner < 0 || owner >= Main.maxPlayers)
		{
			return;
		}

		ReflectDamageOn(Main.player[owner], damageDone);
	}

	/// <summary>
	/// The shared reflect body: 玩家如果攻击会受到自身攻击伤害的20%反伤，取值范围为2~20 while the shell is closed.
	/// The <c>Main.myPlayer</c> guard keeps the reflect to exactly one client, and <c>Player.Hurt</c> then
	/// performs its normal client-to-server hurt sync (T-04-51).
	/// </summary>
	/// <param name="player">The attacking player.</param>
	/// <param name="damageDone">The damage the attack dealt.</param>
	private void ReflectDamageOn(Player player, int damageDone)
	{
		if (State is not (LargeMossyThornTurtleState.Retracted or LargeMossyThornTurtleState.AerialSlam))
		{
			return;
		}

		if (player is null || player.whoAmI != Main.myPlayer)
		{
			return;
		}

		float reflect = MathHelper.Clamp(damageDone * ReflectDamageRatio, ReflectMinDamage, ReflectMaxDamage);
		player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), (int)reflect, player.direction);
	}

	/// <summary>
	/// Re-asserts the design's state-dependent contact damage after the engine has run. <c>NPC.damage</c> is
	/// not part of the NPC net message, so the value is recomputed from the synced <c>NPC.ai[0]</c> on every
	/// side instead of being assigned once at the transition (the Phase 3 荆棘苔龟 precedent). Contact damage
	/// is 50 normally and 85 while the shell is spinning through its dive (旋转 85/160/240, the first value,
	/// the Phase 3 rule). The defence switch is deliberately not repeated here - it is owned by
	/// <see cref="EnterState"/> so it stays authoritative and is not counted twice.
	/// </summary>
	public override void PostAI()
	{
		NPC.damage = State == LargeMossyThornTurtleState.AerialSlam ? RotatedContactDamage : NPC.defDamage;
	}

	/// <summary>
	/// Subworld-only spawning (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c> returns early outside Yggdrasil,
	/// so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the hook
	/// runs in single player or on the server only, where the client camera is zero (D-52/D-55). The creature
	/// is a land walker, so the shared dry-land condition is used, and 非常稀有的MiniBoss takes the phase's
	/// mini-boss weight (at most 0.1f, D-54). The 森雨幽谷 region refinement is a Phases 5-6 predicate gap
	/// (04-DEVIATIONS.md section 5).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The mini-boss weight, or <c>0f</c> outside the design's context.</returns>
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

		return KelpCurtainSpawnConditions.MiniBossWeight;
	}

	/// <summary>
	/// The design's drop block is 死亡掉落物暂定 (TBD), so the table is deliberately empty with no type
	/// reference at all (D-57/D-58): guessing an item type would break the whole mod build, and the drop gap
	/// is recorded as a blocker in 03-BIOLOGY.json and 04-DEVIATIONS.md section 6.2 instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
