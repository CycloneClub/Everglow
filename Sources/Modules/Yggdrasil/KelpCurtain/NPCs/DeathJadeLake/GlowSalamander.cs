using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 幽光蝾螈（Glow Salamander），亡碧湖（Death Jade Lake）水下的发光美西螈。
/// Design row: 幽光蝾螈, heading block id AnOud27JsouukSxerW4cQ3zWnwb,
/// behavior block Dfkidik2VoSGuwxq8YKcjUVpnRg (D-28 full implementation).
/// <para>
/// 拥有三种颜色变种（灰蓝色、粉色、褐色）: the variant index exists in a synced <c>NPC.localAI[]</c> slot, but it is
/// <b>inert until approved art arrives</b> - with the shared <c>Commons.ModAsset.White_Mod</c> fallback
/// there is no sprite to tint, so nothing on screen changes with the variant today. It exists now so the
/// D-49 migration can map the three colour sheets onto frames without reworking this class (OQ2).
/// </para>
/// <para>
/// Approved artwork is missing from the repository, so the class requests the shared fallback texture.
/// The DeathJadeLake region subfolder is what tML's default (namespace-derived) texture resolution
/// expects, so when the art arrives the migration is adding GlowSalamander.png beside this .cs and
/// deleting the Texture override (D-48/D-49); the runtime asset path is
/// Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/GlowSalamander.png.
/// </para>
/// </summary>
public class GlowSalamander : ModNPC
{
	/// <summary>
	/// 每次出水后最多持续60秒: the moisture budget, in frames (60 s * 60). It only drains while the creature
	/// is out of water.
	/// </summary>
	private const int MaxMoistureTicks = 3600;

	/// <summary>剩余10秒时会尝试爬回最近的水: the returning instinct threshold, in frames (10 s * 60).</summary>
	private const int ReturningMoistureTicks = 600;

	/// <summary>潮湿值清零后每30帧获得1秒窒息: the pulse interval, in frames.</summary>
	private const int SuffocationInterval = 30;

	/// <summary>1秒窒息: the suffocation debuff's duration, in frames.</summary>
	private const int SuffocationTicks = 60;

	/// <summary>
	/// While the budget drains, the clients' copy is re-sent at this cadence so every side derives the
	/// same moisture state (the array is only synced on a net update).
	/// </summary>
	private const int MoistureSyncInterval = 30;

	/// <summary>会主动攻击所有发现的水下生物和蛞蝓: the range of the prey scan, in pixels (D-54 default).</summary>
	private const float PreyRange = 40f * 16f;

	/// <summary>主动远离蟾蜍: the range inside which a 剧毒蟾蜍 displaces every other consideration (D-54 default).</summary>
	private const float FleeRange = 30f * 16f;

	/// <summary>How often the prey and toad scans re-run, in frames (D-54 default).</summary>
	private const int RetargetInterval = 30;

	/// <summary>在水中冲刺: the dash speed, in pixels per tick (D-54 default).</summary>
	private const float DashSpeed = 5.5f;

	/// <summary>在水中冲刺: the dash's length, in frames (D-54 default).</summary>
	private const int DashDuration = 20;

	/// <summary>The gap between dashes, in frames, so the creature does not stick to its prey (D-54 default).</summary>
	private const int DashCooldownTicks = 120;

	/// <summary>在水中冲刺: the swim speed used between dashes (D-54 default).</summary>
	private const float SwimSpeed = 2.6f;

	/// <summary>剩余10秒时会尝试爬回最近的水: the crawl speed used on land (D-54 default).</summary>
	private const float CrawlSpeed = 1f;

	/// <summary>A small hop so a lip of ground does not strand the creature (D-54 default).</summary>
	private const float HopSpeedY = 4f;

	/// <summary>The bounded horizontal window, in tiles, of the crawl-back-to-water search.</summary>
	private const int LandWaterScanTiles = 30;

	/// <summary>The bounded vertical window, in tiles, of the same search, so water just below a lip counts.</summary>
	private const int LandWaterScanDepth = 4;

	/// <summary>
	/// 灰蓝色、粉色、褐色: the three colour variants' tints. See the class comment - the palette is inert with
	/// the shared fallback texture and exists so the D-49 migration and a future glow can pick the right
	/// colour for the variant index the instance already carries.
	/// </summary>
	private static readonly Color[] VariantColors =
	[
		new Color(120, 150, 175), // 灰蓝色 GreyBlue
		new Color(230, 160, 190), // 粉色 Pink
		new Color(150, 115, 90), // 褐色 Brown
	];

	/// <summary>
	/// 拥有三种颜色变种: the variant slot, so the D-49 migration can select a colour sheet (OQ2).
	/// </summary>
	private enum GlowSalamanderVariant
	{
		GreyBlue = 0,
		Pink = 1,
		Brown = 2,
	}

	/// <summary>
	/// The four states of the design's behaviour (D-31): the underwater dash-and-melee, the dash itself,
	/// the 剩余10秒 crawl back to water, and the 潮湿值清零 suffocation state. No vanilla <c>aiStyle</c>
	/// provides a moisture-gated amphibian, so the class owns its <c>AI()</c>.
	/// </summary>
	private enum GlowSalamanderState
	{
		Swimming = 0,
		Dashing = 1,
		Returning = 2,
		Suffocating = 3,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private GlowSalamanderState State
	{
		get => (GlowSalamanderState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.ai[1]</c>: the frames left in the current dash.</summary>
	private int DashTimer
	{
		get => (int)NPC.ai[1];
		set => NPC.ai[1] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.ai[2]</c>: the index in <c>Main.npc</c> of the underwater creature or 蛞蝓
	/// this salamander is hunting, or <c>-1</c>.
	/// </summary>
	private int PreyIndex
	{
		get => (int)NPC.ai[2];
		set => NPC.ai[2] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.ai[3]</c>: the index in <c>Main.npc</c> of the nearest 剧毒蟾蜍 it has to
	/// 主动远离, or <c>-1</c>.
	/// </summary>
	private int ToadIndex
	{
		get => (int)NPC.ai[3];
		set => NPC.ai[3] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[0]</c>: 潮湿值, the per-creature moisture budget. It is
	/// deliberately per-NPC state and never stored on a <c>Player</c> (Pitfall 6).
	/// </summary>
	private int Moisture
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[1]</c>: the colour variant slot drawn at spawn (OQ2).</summary>
	private int VariantIndex
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

	/// <summary>Named wrapper over <c>NPC.localAI[3]</c>: the frames left before the prey/toad scans re-run.</summary>
	private int RetargetTimer
	{
		get => (int)NPC.localAI[3];
		set => NPC.localAI[3] = value;
	}

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	// Approved artwork is missing from the repository (D-48). The shared fallback keeps the class
	// loadable, and the beside-.png the phase's gate asserts arrives with the D-49 migration.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		// No approved sprite exists, so there is nothing to slice - but 拥有三种颜色变种（灰蓝色、粉色、褐色）,
		// so whichever sheet the D-49 migration lands must carry the variant frames the synced
		// NPC.localAI[1] slot selects (OQ2).
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// 免疫 中毒 (the design row's 免疫 cell): the poisoned debuff is the only immunity.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
	}

	public override void SetDefaults()
	{
		// The moisture cycle and the underwater dash have no vanilla aiStyle analogue, so the class owns
		// a small custom AI() (the documented case where a local AI() is warranted, D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative amphibian box is used (D-54 default).
		NPC.width = 40;
		NPC.height = 24;

		NPC.lifeMax = 120; // 生命 120
		NPC.life = 120;
		NPC.damage = 30; // 伤害 30（All）
		NPC.defDamage = 30;
		NPC.defense = 4; // 防御 4
		NPC.defDefense = 4;
		NPC.knockBackResist = 0.8f; // 击退抗性 20 -> 1 - 0.20
		NPC.value = 200; // 钱币（铜） 2银 = 200 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field and defaults to White, so this
		// records the empty cell (the Phase 3 NPC.rare -> NPC.rarity correction, 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 幽光蝾螈 is not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// Deterministic starting values, because SetDefaults runs on every side: no Main.rand is consumed
		// here, so server and clients agree before the first sync (the variant is drawn in OnSpawn).
		VariantIndex = (int)GlowSalamanderVariant.GreyBlue;
		Moisture = MaxMoistureTicks;
		DashTimer = 0;
		DashCooldown = 0;
		RetargetTimer = RetargetInterval;
		PreyIndex = -1;
		ToadIndex = -1;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// 拥有三种颜色变种（灰蓝色、粉色、褐色）: the variant is drawn once, on the authoritative side, and the
	/// moisture budget starts full. 在水下生成 means the creature is already in water, so nothing drains
	/// before the first tick.
	/// </summary>
	/// <param name="source">The engine's spawn source.</param>
	public override void OnSpawn(IEntitySource source)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		VariantIndex = Main.rand.Next(VariantColors.Length);
		Moisture = MaxMoistureTicks;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 拥有三种颜色变种 ... 拥有潮湿值: the variant slot and the moisture budget are this class's own
	/// authoritative state. tML syncs <c>NPC.localAI[]</c> with every net update; they are sent explicitly
	/// here as well so the cross-side contract is written down rather than implied by the array the class
	/// happens to use (D-55).
	/// </summary>
	/// <param name="writer">The message writer.</param>
	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(VariantIndex);
		writer.Write(Moisture);
	}

	/// <summary>Receives the variant slot and the moisture budget sent by <see cref="SendExtraAI"/> (D-55).</summary>
	/// <param name="reader">The message reader.</param>
	public override void ReceiveExtraAI(BinaryReader reader)
	{
		VariantIndex = reader.ReadInt32();
		Moisture = reader.ReadInt32();
	}

	/// <summary>
	/// 在水下生成 ... 拥有潮湿值: the moisture budget is drained and refilled on the authoritative side only,
	/// and the state is derived from the synced budget on every side (D-55).
	/// </summary>
	public override void AI()
	{
		bool inLiquid = IsInLiquid();
		NPC.noGravity = inLiquid;

		UpdateMoisture(inLiquid);
		UpdateSuffocation(inLiquid);
		UpdateRetarget();

		if (Main.netMode != NetmodeID.MultiplayerClient && DashCooldown > 0)
		{
			DashCooldown--;
		}

		// 潮湿值清零 -> 窒息; 剩余10秒 -> 爬回最近的水; otherwise the underwater dash-and-melee.
		GlowSalamanderState state;
		if (Moisture <= 0)
		{
			state = GlowSalamanderState.Suffocating;
		}
		else if (!inLiquid && Moisture <= ReturningMoistureTicks)
		{
			state = GlowSalamanderState.Returning;
		}
		else if (DashTimer > 0)
		{
			state = GlowSalamanderState.Dashing;
		}
		else
		{
			state = GlowSalamanderState.Swimming;
		}

		if (State != state)
		{
			EnterState(state);
		}

		switch (state)
		{
			case GlowSalamanderState.Dashing:
				UpdateDashing();
				break;
			case GlowSalamanderState.Swimming:
				UpdateSwimming(inLiquid);
				break;
			default:
				UpdateReturning(inLiquid);
				break;
		}

		UpdateFacing();
	}

	/// <summary>
	/// 幽光 (the design's own name): a faint luminescence in the creature's variant colour plus an existing
	/// Kelp Curtain dust that already emits its own light. Client-only work, so the whole block sits behind
	/// the dedicated-server guard (D-35/D-55); no dust class is created (D-51).
	/// </summary>
	public override void PostAI()
	{
		if (Main.dedServ)
		{
			return;
		}

		Color tint = VariantColors[Math.Clamp(VariantIndex, 0, VariantColors.Length - 1)];
		Lighting.AddLight(NPC.Center, tint.ToVector3() * 0.45f);

		if (Main.rand.NextBool(6))
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<NoctilucentFluoriteLump_Dust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.5f, 0.9f);
		}
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (behavior block Dfkidik2VoSGuwxq8YKcjUVpnRg), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// 只会造成接触伤害，但是会给予10秒中毒: every contact hit applies 10 s (600 ticks) of 中毒. tML documents
	/// <c>ModNPC.OnHitPlayer</c> as "Called on the local client only", so this is deliberately NOT wrapped
	/// in <c>Main.netMode != NetmodeID.MultiplayerClient</c>: such a guard would make the design's debuff
	/// dead code in multiplayer.
	/// </summary>
	/// <param name="target">The player the creature touched.</param>
	/// <param name="hurtInfo">The resolved hit.</param>
	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Poisoned, 600);
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

		for (int i = 0; i < 6; i++)
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<NoctilucentFluoriteLump_Dust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1.1f);
		}
	}

	/// <summary>
	/// 在水下生成, and only ever inside Yggdrasil (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c> returns
	/// early outside the subworld, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// The underwater test is the design's own 在水下生成 condition (Pitfall 6).
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
	/// The design names 毒腺 and 牛黄 (the same two absent materials 剧毒蟾蜍 drops), and neither has a ModItem
	/// in the repository, so no rule is written and no item type is referenced here (D-58, Pitfall 2). The
	/// blocker lives in 04-DEVIATIONS.md section 6 and in the biology matrix instead.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}

	/// <summary>
	/// 每次出水后最多持续60秒: drains the budget while the creature is out of water, and 回到水中后 ... 恢复潮湿值
	/// plus 移除窒息 once it is back in. Written on the authoritative side only (D-55).
	/// </summary>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	private void UpdateMoisture(bool inLiquid)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		if (inLiquid)
		{
			if (Moisture < MaxMoistureTicks)
			{
				Moisture = MaxMoistureTicks;
				NPC.netUpdate = true;
			}

			if (TryClearSuffocation())
			{
				NPC.netUpdate = true;
			}

			return;
		}

		if (Moisture > 0)
		{
			Moisture--;
			if (Moisture % MoistureSyncInterval == 0)
			{
				// Keep the clients' copy of the budget in step while it drains: the array is only sent on
				// a net update, and every side derives the moisture state from it (D-55).
				NPC.netUpdate = true;
			}
		}
	}

	/// <summary>
	/// 潮湿值清零后每30帧获得1秒窒息: at zero moisture, out of water, the creature takes a one-second
	/// suffocation pulse every 30 frames. Applied on the authoritative side only (D-55).
	/// </summary>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	private void UpdateSuffocation(bool inLiquid)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || inLiquid || Moisture > 0)
		{
			return;
		}

		if (Main.GameUpdateCount % SuffocationInterval != 0)
		{
			return;
		}

		NPC.AddBuff(BuffID.Suffocation, SuffocationTicks);
		NPC.netUpdate = true;
	}

	/// <summary>回到水中后移除窒息: drops any suffocation pulse still ticking from the dry stretch.</summary>
	/// <returns>True when at least one suffocation instance was removed.</returns>
	private bool TryClearSuffocation()
	{
		bool cleared = false;
		for (int i = 0; i < NPC.buffType.Length; i++)
		{
			if (NPC.buffType[i] == BuffID.Suffocation)
			{
				NPC.DelBuff(i);
				cleared = true;
				i--;
			}
		}

		return cleared;
	}

	/// <summary>
	/// 会主动攻击所有发现的水下生物和蛞蝓 / 主动远离蟾蜍: refreshes the prey and toad indices on a timer, on
	/// the authoritative side only (D-55).
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
		int toad = FindNearestToad();
		if (prey != PreyIndex || toad != ToadIndex)
		{
			PreyIndex = prey;
			ToadIndex = toad;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// 会主动攻击所有发现的水下生物和蛞蝓: the nearest live creature inside <see cref="PreyRange"/> that is either
	/// a 水蛞蝓 (<c>RiverSlug</c>) or any other creature sitting in liquid. Other 幽光蝾螈 and 剧毒蟾蜍 are
	/// never prey - the toad is the creature this one flees.
	/// </summary>
	/// <returns>The index in <c>Main.npc</c>, or <c>-1</c>.</returns>
	private int FindPreferredPrey()
	{
		int best = -1;
		float bestDistance = PreyRange;
		int riverSlug = ModContent.NPCType<RiverSlug>();
		int toadType = ModContent.NPCType<ToxicToad>();
		for (int i = 0; i < Main.maxNPCs; i++)
		{
			NPC other = Main.npc[i];
			if (!other.active || other.life <= 0 || other.friendly || other.whoAmI == NPC.whoAmI)
			{
				continue;
			}

			if (other.type == Type || other.type == toadType)
			{
				continue;
			}

			bool isPrey = other.type == riverSlug || IsTileLiquid((int)(other.Center.X / 16f), (int)(other.Center.Y / 16f));
			if (!isPrey)
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

	/// <summary>主动远离蟾蜍: the nearest live 剧毒蟾蜍 inside <see cref="FleeRange"/>, or <c>-1</c>.</summary>
	/// <returns>The index in <c>Main.npc</c>, or <c>-1</c>.</returns>
	private int FindNearestToad()
	{
		int best = -1;
		float bestDistance = FleeRange;
		int toadType = ModContent.NPCType<ToxicToad>();
		for (int i = 0; i < Main.maxNPCs; i++)
		{
			NPC other = Main.npc[i];
			if (!other.active || other.life <= 0 || other.whoAmI == NPC.whoAmI || other.type != toadType)
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

	/// <summary>
	/// 在水中冲刺并近战攻击: closes on the prey with a dash whenever the cooldown allows one, flees a toad
	/// first, and otherwise swims or crawls toward whatever it is hunting.
	/// </summary>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	private void UpdateSwimming(bool inLiquid)
	{
		if (TryGetNpc(ToadIndex, out Vector2 toadCenter, out float toadDistance) && toadDistance <= FleeRange)
		{
			// 主动远离蟾蜍: a toad outranks every other target.
			MoveAwayFrom(toadCenter, inLiquid);
			return;
		}

		if (TryGetNpc(PreyIndex, out Vector2 preyCenter, out float preyDistance) && preyDistance <= PreyRange)
		{
			if (TryDashAt(preyCenter, inLiquid))
			{
				return;
			}

			MoveToward(preyCenter, inLiquid);
			return;
		}

		if (TryGetPlayer(out Vector2 playerCenter, out float playerDistance) && playerDistance <= PreyRange)
		{
			if (TryDashAt(playerCenter, inLiquid))
			{
				return;
			}

			MoveToward(playerCenter, inLiquid);
			return;
		}

		// Nothing to chase: idle drift, faster damping on land.
		NPC.velocity *= inLiquid ? 0.96f : 0.9f;
	}

	/// <summary>在水中冲刺: starts a dash when the cooldown allows it and the creature is in water (D-55).</summary>
	/// <param name="targetCenter">The point to dash at.</param>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	/// <returns>True when a dash was started.</returns>
	private bool TryDashAt(Vector2 targetCenter, bool inLiquid)
	{
		if (!inLiquid || DashCooldown > 0 || Main.netMode == NetmodeID.MultiplayerClient)
		{
			return false;
		}

		EnterDash(targetCenter);
		return true;
	}

	/// <summary>The dash itself, ended on its own timer on the authoritative side.</summary>
	private void UpdateDashing()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		DashTimer--;
		if (DashTimer <= 0)
		{
			DashCooldown = DashCooldownTicks;
			EnterState(GlowSalamanderState.Swimming);
		}
	}

	/// <summary>
	/// 剩余10秒时会尝试爬回最近的水（没有则朝着一个方向爬行）and the 潮湿值清零 state share this crawl: it walks
	/// toward the nearest liquid, or keeps walking in its current direction when there is none.
	/// </summary>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	private void UpdateReturning(bool inLiquid)
	{
		if (inLiquid)
		{
			// 回到水中: the crawl back is over; AI refills 潮湿值 and removes 窒息 on the next tick.
			NPC.velocity *= 0.9f;
			return;
		}

		int direction = FindNearestLiquidDirection();
		if (direction == 0)
		{
			// 没有则朝着一个方向爬行: keep the current heading instead of picking a new one.
			direction = NPC.direction == 0 ? 1 : NPC.direction;
		}

		if (NPC.collideY || NPC.velocity.Y == 0f)
		{
			// A small hop, so a lip of ground does not strand the creature on the way back.
			NPC.velocity.Y = -HopSpeedY;
		}

		NPC.velocity.X = CrawlSpeed * direction;
		NPC.direction = direction;
	}

	/// <summary>Moves toward the target: a swim when submerged, a crawl with a small hop on land.</summary>
	/// <param name="targetCenter">The point to close on.</param>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	private void MoveToward(Vector2 targetCenter, bool inLiquid)
	{
		Vector2 direction = (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		if (inLiquid)
		{
			NPC.velocity = Vector2.Lerp(NPC.velocity, direction * SwimSpeed, 0.06f);
			return;
		}

		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, direction.X * CrawlSpeed, 0.08f);
		if (NPC.collideX && NPC.collideY)
		{
			NPC.velocity.Y = -HopSpeedY;
		}
	}

	/// <summary>主动远离蟾蜍: swims (or crawls) directly away from the threat.</summary>
	/// <param name="threatCenter">The point to move away from.</param>
	/// <param name="inLiquid">Whether the creature is currently in liquid.</param>
	private void MoveAwayFrom(Vector2 threatCenter, bool inLiquid)
	{
		Vector2 direction = (NPC.Center - threatCenter).SafeNormalize(Vector2.UnitX * -NPC.direction);
		Vector2 desired = direction * (inLiquid ? SwimSpeed : CrawlSpeed);
		NPC.velocity = Vector2.Lerp(NPC.velocity, desired, inLiquid ? 0.08f : 0.1f);
	}

	/// <summary>The centre of a tracked NPC, when the slot still holds a live creature.</summary>
	/// <param name="index">The index in <c>Main.npc</c>, or <c>-1</c>.</param>
	/// <param name="center">The creature's centre.</param>
	/// <param name="distance">The distance to it, or <see cref="float.MaxValue"/>.</param>
	/// <returns>True when a live creature exists at that index.</returns>
	private bool TryGetNpc(int index, out Vector2 center, out float distance)
	{
		center = default;
		distance = float.MaxValue;
		if (index < 0 || index >= Main.maxNPCs)
		{
			return false;
		}

		NPC other = Main.npc[index];
		if (!other.active || other.life <= 0)
		{
			return false;
		}

		center = other.Center;
		distance = Vector2.Distance(NPC.Center, other.Center);
		return true;
	}

	/// <summary>The nearest live player, used only when no creature target is available (D-55: never a camera value).</summary>
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

	/// <summary>The bounded horizontal search for the nearest liquid at (or just below) the creature's height.</summary>
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
			if (IsTileLiquid(tileX, tileY + depth))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>True when that tile is in the world and holds liquid. A world-edge tile reports false.</summary>
	private static bool IsTileLiquid(int tileX, int tileY)
	{
		if (!WorldGen.InWorld(tileX, tileY, 1))
		{
			return false;
		}

		return Main.tile[tileX, tileY].LiquidAmount > 0;
	}

	/// <summary>True when the creature's centre tile holds liquid, i.e. 在水下.</summary>
	private bool IsInLiquid()
	{
		return IsTileLiquid((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f));
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

	/// <summary>在水中冲刺: starts a dash at the target, written on the authoritative side only (D-55).</summary>
	/// <param name="targetCenter">The point to dash at.</param>
	private void EnterDash(Vector2 targetCenter)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		Vector2 direction = (targetCenter - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
		NPC.velocity = direction * DashSpeed;
		DashTimer = DashDuration;
		State = GlowSalamanderState.Dashing;
		NPC.netUpdate = true;
	}

	/// <summary>The design's state transition, written only on the authoritative side with an <c>NPC.netUpdate</c> (D-55).</summary>
	/// <param name="state">The state to enter.</param>
	private void EnterState(GlowSalamanderState state)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		State = state;
		NPC.netUpdate = true;
	}
}
