using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 剧毒蟾蜍（Toxic Toad），亡碧湖（Death Jade Lake）岸边的两栖伏击者。
/// Design row: 剧毒蟾蜍, heading block id KicYd5bbPo4psdxHl1ScX90Bnpf,
/// behavior block GjwedzoyPouXssx66zpcBbo5nWd, drop block GftbdkuVuovYXoxpZm7cAE4Mndb (D-28 full implementation).
/// Approved artwork is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the migration is adding
/// ToxicToad.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset
/// path is Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/ToxicToad.png.
/// </summary>
public class ToxicToad : ModNPC
{
	// --- aggro preference (会主动攻击蝾螈与水蛞蝓，对玩家仇恨低于这些生物) -------------------------

	/// <summary>
	/// 蝾螈与水蛞蝓: the preference range for the two prey species, in pixels. Inside this range a live
	/// 幽光蝾螈 or 水蛞蝓 outranks the player as the toad's target (D-54 default; the design gives no number).
	/// </summary>
	private const float PreyRange = 40f * 16f;

	/// <summary>
	/// 对玩家仇恨低于这些生物: how often the prey scan runs, in frames. Re-scanning on a timer keeps the
	/// cost off every tick while still reacting within half a second (D-54 default).
	/// </summary>
	private const int RetargetInterval = 30;

	/// <summary>毒泡泡 attack range: inside this range the toad stops and fires instead of walking (D-54 default).</summary>
	private const float FireRange = 30f * 16f;

	/// <summary>The aggro range from which the toad starts approaching a target at all (D-54 default).</summary>
	private const float AggroRange = 50f * 16f;

	/// <summary>通过发射毒泡泡攻击: the gap between bubbles (D-54 default: the design gives no cadence).</summary>
	private const int FireInterval = 90;

	/// <summary>在地表生成: the walking speed used on dry land (D-54 default).</summary>
	private const float WalkSpeed = 1.1f;

	/// <summary>落入水中的话会游泳: the swim speed used once it is in liquid (D-54 default).</summary>
	private const float SwimSpeed = 1.9f;

	/// <summary>A small hop so a lip of ground does not strand the toad (D-54 default).</summary>
	private const float HopSpeedY = 4.2f;

	/// <summary>The poison bubble's launch speed, in pixels per tick (D-54 default).</summary>
	private const float BubbleSpeed = 6.5f;

	/// <summary>
	/// 不论何种方式都会有75%概率造成10秒中毒: 10 s = 600 ticks. The design's 75% branch is
	/// <c>Main.rand.NextBool(3, 4)</c> (3 in 4), the documented "X out of Y" overload; the one-argument
	/// <c>NextBool(n)</c> is 1 in n, not n-1 in n.
	/// </summary>
	private const int PoisonTicks = 600;

	/// <summary>剩下25%概率造成7秒酸性毒液: 7 s = 420 ticks of the mapped <c>BuffID.Venom</c>.</summary>
	private const int VenomTicks = 420;

	/// <summary>接触后造成10伤害: the death cloud's contact damage (the design's own number).</summary>
	private const int PoisonCloudDamage = 10;

	/// <summary>
	/// The three states of the design's behaviour (D-31): nothing aggroed, closing the distance, and
	/// stopped at the design's firing distance. No vanilla <c>aiStyle</c> provides a ranged amphibian
	/// ambusher, so the class owns its <c>AI()</c>.
	/// </summary>
	private enum ToxicToadState
	{
		Idle = 0,
		Approaching = 1,
		Firing = 2,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private ToxicToadState State
	{
		get => (ToxicToadState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.ai[1]</c>: the index in <c>Main.npc</c> of the 蝾螈 / 水蛞蝓 the toad is
	/// preferring over the player, or <c>-1</c> when there is none.
	/// </summary>
	private int PreyIndex
	{
		get => (int)NPC.ai[1];
		set => NPC.ai[1] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames left before the next poison bubble.</summary>
	private int FireTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[1]</c>: the frames left before the prey list is re-scanned.</summary>
	private int RetargetTimer
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = value;
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

		// 免疫 中毒、酸性毒液 (the design row's 免疫 cell): exactly these two. 酸性毒液 maps to the vanilla
		// BuffID.Venom, because BuffID.AcidVenom does not exist in this tML build and creating a debuff
		// would need an unapproved icon (04-DEVIATIONS.md section 10).
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
	}

	public override void SetDefaults()
	{
		// The ranged amphibian ambusher has no vanilla aiStyle analogue, so the class owns a small
		// custom AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative toad-sized box is used (D-54 default).
		NPC.width = 46;
		NPC.height = 32;

		NPC.lifeMax = 80; // 生命 80
		NPC.life = 80;
		NPC.damage = 25; // 伤害 25（All）
		NPC.defDamage = 25;
		NPC.defense = 8; // 防御 8
		NPC.defDefense = 8;
		NPC.knockBackResist = 0.2f; // 击退抗性 80 -> 1 - 0.80
		NPC.value = 200; // 钱币（铜） 2银 = 200 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 剧毒蟾蜍 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// Deterministic starting timers, because SetDefaults runs on every side: no Main.rand is consumed
		// here, so the server and its clients agree before the first sync arrives.
		FireTimer = FireInterval;
		RetargetTimer = RetargetInterval;
		PreyIndex = -1;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// 两栖生物: 在地表生成 but 落入水中的话会游泳, so gravity is switched off while the toad is in liquid.
	/// The target is re-scanned on a timer and the state is derived from the synced distances on every
	/// side, while the written state stays authoritative (D-55).
	/// </summary>
	public override void AI()
	{
		bool inLiquid = IsInLiquid();
		NPC.noGravity = inLiquid;

		UpdateRetarget();

		// 对玩家仇恨低于这些生物: a live 幽光蝾螈 / 水蛞蝓 inside the preference range wins; the player is
		// only used when neither creature is available.
		Vector2 targetCenter;
		float distance;
		bool hasTarget;
		if (TryGetPrey(out Vector2 preyCenter, out float preyDistance) && preyDistance <= PreyRange)
		{
			targetCenter = preyCenter;
			distance = preyDistance;
			hasTarget = true;
		}
		else if (TryGetPlayer(out Vector2 playerCenter, out float playerDistance))
		{
			targetCenter = playerCenter;
			distance = playerDistance;
			hasTarget = true;
		}
		else
		{
			targetCenter = NPC.Center;
			distance = float.MaxValue;
			hasTarget = false;
		}

		ToxicToadState state;
		if (!hasTarget || distance > AggroRange)
		{
			state = ToxicToadState.Idle;
		}
		else if (distance <= FireRange)
		{
			state = ToxicToadState.Firing;
		}
		else
		{
			state = ToxicToadState.Approaching;
		}

		if (State != state)
		{
			EnterState(state);
		}

		switch (state)
		{
			case ToxicToadState.Firing:
				// 不会主动近战: the toad plants itself at the design's firing distance and only shoots.
				NPC.velocity.X *= 0.9f;
				NPC.velocity.Y *= inLiquid ? 0.9f : 1f;
				TryFireBubble(targetCenter);
				break;
			case ToxicToadState.Approaching:
				MoveToward(targetCenter, inLiquid);
				break;
			default:
				NPC.velocity.X *= 0.9f;
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// 剧毒蟾蜍: the faint toxic sheen it carries even before it dies. Client-only work, so it sits behind
	/// the dedicated-server guard (D-35/D-55) and reuses an existing Kelp Curtain dust rather than
	/// creating a new dust class (D-51).
	/// </summary>
	public override void PostAI()
	{
		if (Main.dedServ)
		{
			return;
		}

		if (Main.rand.NextBool(8))
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<JadeLakeGreenAlgaeDust>(), 0f, -0.4f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
		}
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (behavior block GjwedzoyPouXssx66zpcBbo5nWd), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// 不论何种方式都会有75%概率造成10秒中毒，剩下25%概率造成7秒酸性毒液: every contact hit, because the toad
	/// 不会主动近战 but 会造成近战伤害. tML documents <c>ModNPC.OnHitPlayer</c> as "Called on the local
	/// client only", so this is deliberately NOT wrapped in <c>Main.netMode != NetmodeID.MultiplayerClient</c>:
	/// such a guard would make the design's debuff dead code in multiplayer. 酸性毒液 maps to the vanilla
	/// <c>BuffID.Venom</c> (04-DEVIATIONS.md section 10).
	/// </summary>
	/// <param name="target">The player the toad touched.</param>
	/// <param name="hurtInfo">The resolved hit.</param>
	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		if (Main.rand.NextBool(3, 4))
		{
			// 75% -> 10 s (600 ticks) of 中毒.
			target.AddBuff(BuffID.Poisoned, PoisonTicks);
		}
		else
		{
			// the remaining 25% -> 7 s (420 ticks) of 酸性毒液.
			target.AddBuff(BuffID.Venom, VenomTicks);
		}
	}

	/// <summary>
	/// 死亡后爆炸，产生一小团剧毒云: one cloud is spawned when the toad dies, from the creature's own AI
	/// source and only on the authoritative side (D-55), and only existing Kelp Curtain dust is emitted
	/// on the client (D-35).
	/// </summary>
	/// <param name="hit">The killing hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				// 持续3秒: the cloud carries its own 180-frame lifetime and its own damage.
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ToxicToad_PoisonCloud>(), PoisonCloudDamage, 0f);
				NPC.netUpdate = true;
			}

			if (!Main.dedServ)
			{
				for (int i = 0; i < 8; i++)
				{
					int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<JadeLakeGreenAlgaeDust>(), 0f, -0.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.3f);
				}
			}
		}
	}

	/// <summary>
	/// 在地表生成, and only ever inside Yggdrasil (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c> returns
	/// early outside the subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// The land test is the design's own 在地表生成 condition (Pitfall 6).
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The conservative land weight, or <c>0f</c> outside the design's context.</returns>
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

		return KelpCurtainSpawnConditions.LandWeight;
	}

	/// <summary>
	/// 死亡后33%概率掉落1 毒腺，1%/2%概率掉落 1 牛黄. Neither 毒腺 nor 牛黄 has a ModItem in the
	/// repository, so no rule is written and no item type is referenced here (D-58, Pitfall 2): a
	/// <c>ModContent.ItemType&lt;X&gt;()</c> for an absent <c>X</c> would fail the whole mod build. The
	/// blocker lives in 04-DEVIATIONS.md section 6 and in the biology matrix instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}

	/// <summary>
	/// 对玩家仇恨低于这些生物: refreshes the preferred prey index on a timer, on the authoritative side
	/// only, because it consumes no randomness but does drive shared movement state (D-55).
	/// </summary>
	private void UpdateRetarget()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		RetargetTimer--;
		if (RetargetTimer > 0)
		{
			return;
		}

		RetargetTimer = RetargetInterval;
		int prey = FindPreferredPrey();
		if (prey != PreyIndex)
		{
			PreyIndex = prey;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// The nearest live 幽光蝾螈 or 水蛞蝓 inside <see cref="PreyRange"/>, or <c>-1</c>. The two species are
	/// named through <c>ModContent.NPCType&lt;...&gt;</c> so the preference survives a rename of either
	/// class (never a string or name-based lookup).
	/// </summary>
	/// <returns>The index in <c>Main.npc</c>, or <c>-1</c>.</returns>
	private int FindPreferredPrey()
	{
		int best = -1;
		float bestDistance = PreyRange;
		int glowSalamander = ModContent.NPCType<GlowSalamander>();
		int riverSlug = ModContent.NPCType<RiverSlug>();
		for (int i = 0; i < Main.maxNPCs; i++)
		{
			NPC other = Main.npc[i];
			if (!other.active || other.life <= 0 || other.friendly || other.whoAmI == NPC.whoAmI)
			{
				continue;
			}

			if (other.type != glowSalamander && other.type != riverSlug)
			{
				continue;
			}

			float distance = Vector2.Distance(NPC.Center, other.Center);
			if (distance < bestDistance)
			{
				bestDistance = distance;
				best = i;
			}
		}

		return best;
	}

	/// <summary>The centre of the preferred prey creature, when one is still alive.</summary>
	/// <param name="center">The prey's centre.</param>
	/// <param name="distance">The distance to it, or <see cref="float.MaxValue"/>.</param>
	/// <returns>True when a live prey creature exists.</returns>
	private bool TryGetPrey(out Vector2 center, out float distance)
	{
		center = default;
		distance = float.MaxValue;
		int index = PreyIndex;
		if (index < 0 || index >= Main.maxNPCs)
		{
			return false;
		}

		NPC prey = Main.npc[index];
		if (!prey.active || prey.life <= 0)
		{
			return false;
		}

		center = prey.Center;
		distance = Vector2.Distance(NPC.Center, prey.Center);
		return true;
	}

	/// <summary>The nearest live player, used only when no preferred prey is available (D-55: never a camera value).</summary>
	/// <param name="center">The player's centre.</param>
	/// <param name="distance">The distance to it, or <see cref="float.MaxValue"/>.</param>
	/// <returns>True when a live player exists.</returns>
	private bool TryGetPlayer(out Vector2 center, out float distance)
	{
		NPC.TargetClosest(false);
		center = default;
		distance = float.MaxValue;
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
		distance = Vector2.Distance(NPC.Center, target.Center);
		return true;
	}

	/// <summary>Moves toward the target: a swim while submerged, a walk with a small hop on land.</summary>
	/// <param name="targetCenter">The point to close on.</param>
	/// <param name="inLiquid">Whether the toad is currently in liquid.</param>
	private void MoveToward(Vector2 targetCenter, bool inLiquid)
	{
		Vector2 direction = (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		if (inLiquid)
		{
			// 落入水中的话会游泳: no gravity, no drowning, just a swim toward the target.
			NPC.velocity = Vector2.Lerp(NPC.velocity, direction * SwimSpeed, 0.06f);
			return;
		}

		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, direction.X * WalkSpeed, 0.08f);
		if (NPC.collideX && NPC.collideY)
		{
			// A lip of ground it cannot walk over: a small hop, so it is not stranded.
			NPC.velocity.Y = -HopSpeedY;
		}
	}

	/// <summary>
	/// 通过发射毒泡泡攻击: launches one bubble at the target on the design's fire cadence. The bubble is
	/// spawned and the timer consumed on the authoritative side only, from <c>NPC.GetSource_FromAI()</c>
	/// so the damage is attributed to the creature (D-55).
	/// </summary>
	/// <param name="targetCenter">The point the bubble is aimed at.</param>
	private void TryFireBubble(Vector2 targetCenter)
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
		Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * BubbleSpeed, ModContent.ProjectileType<ToxicToad_PoisonBubble>(), NPC.damage, 0f);
		NPC.netUpdate = true;
	}

	/// <summary>True when the toad's centre tile holds liquid (it 落入水中 and is swimming).</summary>
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

	/// <summary>The design's state transition, written only on the authoritative side with an <c>NPC.netUpdate</c> (D-55).</summary>
	/// <param name="state">The state to enter.</param>
	private void EnterState(ToxicToadState state)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = state;
		NPC.netUpdate = true;
	}
}
