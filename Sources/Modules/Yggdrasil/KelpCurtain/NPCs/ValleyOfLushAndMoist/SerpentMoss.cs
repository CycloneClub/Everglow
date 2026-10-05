using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist;

/// <summary>
/// 蛇行苔（Serpent Moss），森雨幽谷（Valley of Lush and Moist）的束缚型伏击植物。
/// Design row: 蛇行苔, heading block id C8fRdR6uhoYxOKxrWzDc8IHwnud,
/// behavior block RmwmdtUCwoMPI1xMVQrclFS4nuh (敌对生物，出生后伪装成环境植物，在玩家接近（不低于2格）后，
/// 束缚玩家并持续造成伤害。束缚玩家后，每60帧对玩家施加60帧束缚并造成15伤害，每次造成伤害后，有33%概率对玩家造成
/// 15秒中毒), 生命 100 / 伤害 15 / 防御 10 / 击退抗性 免疫击退 / 免疫 中毒、困惑 / 钱币（铜）2银 = 200.
/// <para>
/// <b>Scope (D-46).</b> The trigger, the bind and the damage cadence are built in full. The design's 伪装 -
/// moss that is indistinguishable from an environmental plant after it spawns - belongs to the Valley
/// disguised-hazard visual system, which does not exist in this repository (04-DEVIATIONS.md section 7
/// lists it as an unimplemented system). That presentation is therefore a D-46 blocker naming that system
/// and this file, and no disguise trick is faked here: a half-built alpha trick would be indistinguishable
/// from a rendering bug.
/// </para>
/// <para>
/// <b>Debuff mapping (04-DEVIATIONS.md section 10).</b> The design's 束缚 has no vanilla member of its own,
/// so it maps to the engine's own immobilise debuff <c>BuffID.Webbed</c> (id 149) - the member this class
/// applies. The same register maps 窒息 to <c>BuffID.Suffocation</c> (id 68), but this row's design carries
/// no 窒息 clause, so that member is not applied here. The mapping in that register is the authority, so
/// this class stands alone and needs no same-wave sibling to have landed first. The bind cadence is never
/// stored on a <c>Player</c>: the per-NPC timer lives in <c>NPC.localAI[0]</c> (Pitfall 6).
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. When the art arrives the D-49 migration is adding SerpentMoss.png
/// beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset path is
/// Everglow/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SerpentMoss.png.
/// </para>
/// </summary>
public class SerpentMoss : ModNPC
{
	/// <summary>在玩家接近（不低于2格）后: the bind trigger radius, in tiles.</summary>
	private const float BindTriggerTiles = 2f;

	/// <summary>Pixels per tile, so a world-space distance can be compared with the design's tile range.</summary>
	private const float PixelsPerTile = 16f;

	/// <summary>每60帧: the bind cadence, in frames.</summary>
	private const int BindIntervalFrames = 60;

	/// <summary>施加60帧束缚: the already-mapped 束缚 duration, in ticks.</summary>
	private const int BindDebuffTicks = 60;

	/// <summary>造成15伤害: the per-cadence bind damage.</summary>
	private const int BindDamage = 15;

	/// <summary>
	/// 有33%概率对玩家造成15秒中毒: 15 s = 900 ticks, rolled 1 in 3.
	/// </summary>
	private const int PoisonTicks = 900;

	/// <summary>The 33% poison denominator.</summary>
	private const int PoisonChanceDenominator = 3;

	/// <summary>The <c>NPC.localAI[1]</c> value meaning "no player is currently bound".</summary>
	private const int NoBoundPlayer = -1;

	/// <summary>
	/// The two local states of the design row: <see cref="Camouflaged"/> is 伪装成环境植物 (the idle state that
	/// waits for a player to come close) and <see cref="Binding"/> is the 束缚 loop. No vanilla <c>aiStyle</c>
	/// provides a stationary binding hazard, so the class owns its <c>AI()</c> (D-31).
	/// </summary>
	private enum SerpentMossState
	{
		Camouflaged = 0,
		Binding = 1,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private SerpentMossState State
	{
		get => (SerpentMossState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[0]</c>: the frames since the last bind tick, so no bare numeric
	/// index is scattered through the class (and never a field on the <c>Player</c>, Pitfall 6).
	/// </summary>
	private int BindTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[1]</c>: the index of the player currently bound, or
	/// <see cref="NoBoundPlayer"/>, so the release test does not re-scan every player each frame.
	/// </summary>
	private int BoundPlayerIndex
	{
		get => (int)NPC.localAI[1];
		set => NPC.localAI[1] = (int)value;
	}

	/// <summary>
	/// The point the stationary hazard came to rest at, so it keeps its spawn point instead of drifting and
	/// returning to where a shove left it (D-55).
	/// </summary>
	public Vector2 AnchorPosition;

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
		// The stationary bind loop has no vanilla aiStyle analogue, so the class owns its AI().
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a conservative ground-clinging box is used and is the first
		// value to revisit when the art arrives (D-54).
		NPC.width = 40;
		NPC.height = 40;

		NPC.lifeMax = 100; // 生命 100
		NPC.life = 100;
		NPC.damage = 15; // 伤害 15
		NPC.defDamage = 15;
		NPC.defense = 10; // 防御 10
		NPC.defDefense = 10;
		NPC.knockBackResist = 0f; // 击退抗性 免疫击退
		NPC.value = 200; // 钱币（铜） 2银 = 200 copper, a 1:1 transpose (NPC.value is copper)

		// Conservative default: the design's 稀有度 cell is empty, and NPC.rarity is the engine's only
		// NPC rarity field (the NPC.rare -> NPC.rarity correction, 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.friendly = false;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// Not 可被捕捉, so no catch item, no Main.npcCatchable and no CountsAsCritter.
		NPC.catchItem = 0;

		// The bind index starts empty so GetBoundPlayer() never dereferences player 0 by accident.
		BoundPlayerIndex = NoBoundPlayer;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	public override void OnSpawn(IEntitySource source)
	{
		AnchorPosition = NPC.Center;
		BoundPlayerIndex = NoBoundPlayer;
	}

	/// <summary>
	/// The bind machine: a player who comes within 2 tiles is bound, and they then take 60 ticks of the
	/// mapped 束缚 plus 15 damage every 60 frames, with the design's 33% 15-second 中毒 roll after each
	/// damage tick. The bind is released when the bound player leaves the trigger radius. Everything here
	/// is authoritative: the state lives in the synced <c>NPC.ai[0]</c>, the writes happen only on the
	/// server or in single player, and the client reads the synced state instead of predicting it (D-55).
	/// </summary>
	public override void AI()
	{
		// A stationary hazard: it never walks off its anchor.
		NPC.velocity.X = 0f;

		if (State == SerpentMossState.Camouflaged && NPC.velocity.Y == 0f && NPC.collideY)
		{
			// The rest position becomes the anchor the release path returns to.
			AnchorPosition = NPC.Center;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			UpdateBind();
		}
	}

	/// <summary>
	/// 在玩家接近（不低于2格）后，束缚玩家并持续造成伤害: the bind machine, run on the authoritative side only, so
	/// the 束缚 / 中毒 buffs and the 15 damage are applied where they are authoritative for the bound player
	/// and the synced <c>NPC.ai[0]</c> state is written exactly once per transition (D-55).
	/// </summary>
	private void UpdateBind()
	{
		Player boundPlayer = GetBoundPlayer();
		if (boundPlayer is not null && Vector2.Distance(NPC.Center, boundPlayer.Center) > BindTriggerTiles * PixelsPerTile)
		{
			// The bound player left the trigger radius: release the bind.
			BoundPlayerIndex = NoBoundPlayer;
			EnterState(SerpentMossState.Camouflaged);
			return;
		}

		if (boundPlayer is null)
		{
			boundPlayer = FindPlayerInTrigger();
			if (boundPlayer is not null)
			{
				// 在玩家接近（不低于2格）后，束缚玩家: enter the bind loop.
				BoundPlayerIndex = boundPlayer.whoAmI;
				EnterState(SerpentMossState.Binding);
			}
		}

		if (State != SerpentMossState.Binding || boundPlayer is null)
		{
			return;
		}

		BindTimer++;
		if (BindTimer < BindIntervalFrames)
		{
			return;
		}

		BindTimer = 0;

		// 每60帧对玩家施加60帧束缚并造成15伤害: 束缚 is the mapped BuffID.Webbed (04-DEVIATIONS.md section 10).
		boundPlayer.AddBuff(BuffID.Webbed, BindDebuffTicks);
		boundPlayer.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), BindDamage, boundPlayer.direction);

		// 每次造成伤害后，有33%概率对玩家造成15秒中毒: a 1-in-3 roll, i.e. Main.rand.NextBool(3).
		if (Main.rand.NextBool(PoisonChanceDenominator))
		{
			boundPlayer.AddBuff(BuffID.Poisoned, PoisonTicks);
		}

		NPC.netUpdate = true;
	}

	/// <summary>
	/// The single state-transition helper, so the design's 伪装/束缚 switch is written in exactly one place
	/// and only on the authoritative side with an <c>NPC.netUpdate</c> (D-55). Going back to
	/// <see cref="SerpentMossState.Camouflaged"/> clears the bind timer and returns the stationary hazard to
	/// its anchor.
	/// </summary>
	/// <param name="nextState">The state to enter.</param>
	private void EnterState(SerpentMossState nextState)
	{
		if (State == nextState)
		{
			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			State = nextState;
			if (nextState == SerpentMossState.Camouflaged)
			{
				BindTimer = 0;
				NPC.Center = AnchorPosition;
				NPC.velocity = Vector2.Zero;
			}

			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// The player this creature is currently binding, or <c>null</c> when nobody is bound or the bound
	/// player has died or left.
	/// </summary>
	/// <returns>The bound player, or <c>null</c>.</returns>
	private Player GetBoundPlayer()
	{
		int index = BoundPlayerIndex;
		if (index < 0 || index >= Main.maxPlayers)
		{
			return null;
		}

		Player player = Main.player[index];
		return player.active && !player.dead ? player : null;
	}

	/// <summary>
	/// 在玩家接近（不低于2格）后: the closest live player inside the 2-tile trigger, or <c>null</c>. The scan
	/// reads only synced player state, never the local player or a camera value (D-52/D-55).
	/// </summary>
	/// <returns>The player to bind, or <c>null</c>.</returns>
	private Player FindPlayerInTrigger()
	{
		float triggerRange = BindTriggerTiles * PixelsPerTile;
		for (int i = 0; i < Main.maxPlayers; i++)
		{
			Player player = Main.player[i];
			if (!player.active || player.dead)
			{
				continue;
			}

			if (Vector2.Distance(NPC.Center, player.Center) <= triggerRange)
			{
				return player;
			}
		}

		return null;
	}

	/// <summary>
	/// The hit and death dust. Client-only work, so the whole body sits behind the dedicated-server guard
	/// (D-35/D-55) and reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	/// <param name="hit">The resolved hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (Main.dedServ)
		{
			return;
		}

		int dustCount = NPC.life <= 0 ? 6 : 2;
		for (int i = 0; i < dustCount; i++)
		{
			int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<KelpMoss_dust>(), 2 * hit.HitDirection, -2f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
		}
	}

	/// <summary>
	/// Subworld-only spawning (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c> returns early outside
	/// Yggdrasil, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// The hazard is a ground plant, so the shared dry-land condition is used and the design supplies no
	/// weight (the conservative land band, D-54). The 森雨幽谷 region refinement is a Phases 5-6 predicate
	/// gap (04-DEVIATIONS.md section 5).
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
	/// The design row names no drop, so the table is deliberately empty with no type reference
	/// (the VerdantRods precedent, D-57/D-58).
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
