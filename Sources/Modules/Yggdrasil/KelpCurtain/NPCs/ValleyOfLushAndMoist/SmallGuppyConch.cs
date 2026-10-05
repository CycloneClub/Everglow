using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist;

/// <summary>
/// 小格普螺（Small Guppy Conch），森雨幽谷（Valley of Lush and Moist）的被动陆生蜗牛。
/// Design row: 小格普螺, heading block id BY1nd7O1Xokz4dxb8Mlc2MrVnwd,
/// behavior block MGMFdA62foF8gix4hqYcHHIpnhb (被动生物，可以被捕获。), 类型 (普通),
/// 生命 20 / 防御 5 / 减伤 5. The design's 伤害, 击退抗性, 免疫 and 钱币 cells are all empty.
/// It is the small sibling of the Phase 3 格普螺 (<c>GuppyConch</c>) in the same family, so the
/// slow land crawl and the water-rejecting spawn gate are mirrored from that class.
/// <para>
/// <b>Capture blocker (D-58).</b> The design says 可以被捕获, but no catch item exists for this creature
/// anywhere in the repository, so this class writes no catch-item type, no catchable flag and no
/// critter-count set entry. It is therefore not capturable until the missing catch item is authored
/// (04-DEVIATIONS.md section 6.2 and section 13, item scope).
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback. When the art arrives the D-49 migration is adding
/// SmallGuppyConch.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset
/// path is Everglow/Yggdrasil/KelpCurtain/NPCs/ValleyOfLushAndMoist/SmallGuppyConch.png.
/// </para>
/// </summary>
public class SmallGuppyConch : ModNPC
{
	/// <summary>
	/// The design's 缓慢 crawl, in pixels per tick: 小格普螺 is the small sibling of 格普螺
	/// (<c>GuppyConch.CrawlSpeed = 0.4f</c>), so it is deliberately slower than that class.
	/// </summary>
	private const float CrawlSpeed = 0.25f;

	/// <summary>
	/// The crawl leg of the crawl/rest rhythm. The design gives no cadence, so conservative defaults are
	/// used and recorded as a D-54 entry (04-DEVIATIONS.md section 10).
	/// </summary>
	private const int CrawlFrames = 240;

	/// <summary>
	/// The rest leg of the crawl/rest rhythm: the snail pauses in place, which is the only visible
	/// behaviour its single design sentence leaves room for (D-54 default).
	/// </summary>
	private const int RestFrames = 120;

	/// <summary>
	/// The two local states of the design row: <see cref="Crawling"/> is the 缓慢 land crawl and
	/// <see cref="Resting"/> is the short pause between two crawl legs. The design supplies no shell
	/// state and no defence switch, so unlike its sibling <c>GuppyConch</c> this class never raises
	/// <c>NPC.defense</c>. No vanilla <c>aiStyle</c> provides a crawl/rest land snail, so the class owns
	/// its <c>AI()</c> (D-31).
	/// </summary>
	private enum SmallGuppyConchState
	{
		Crawling = 0,
		Resting = 1,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private SmallGuppyConchState State
	{
		get => (SmallGuppyConchState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[0]</c>: the ticks remaining in the current crawl or rest leg.
	/// It is a per-NPC timer, never a field on a <c>Player</c> (Pitfall 6).
	/// </summary>
	private int StateTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
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

		// The design row's 免疫 cell is empty, so no specific debuff immunity is declared.
	}

	public override void SetDefaults()
	{
		// No vanilla aiStyle provides a crawl/rest land snail, so the class owns its AI (D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a small critter-sized box is used and is the first value to
		// revisit when the art arrives (D-54).
		NPC.width = 24;
		NPC.height = 14;

		NPC.lifeMax = 20; // 生命 20
		NPC.life = 20;

		// The design's 伤害 cell is empty, so this passive creature deals no contact damage at all.
		NPC.damage = 0;
		NPC.defDamage = 0;

		NPC.defense = 5; // 防御 5
		NPC.defDefense = 5;

		// The design's 击退抗性 cell is empty, so the engine's small-critter default is kept rather than
		// a design-derived value being invented (D-54).
		NPC.knockBackResist = 0.8f;

		// The design's 钱币 cell is empty, so the creature carries no coin value.
		NPC.value = 0;

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field (the API-name correction
		// recorded in 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		NPC.friendly = false;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 可以被捕获 in the design, but no catch item for this creature exists in the repository, so the
		// capture is a blocked item-scope dependency (D-58): no catch-item type reference, no catchable
		// flag and no critter-count set entry are written here.
		NPC.catchItem = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// Seeds the first crawl leg, so a freshly spawned snail crawls before it pauses and the state timer
	/// never starts at the zero that would otherwise read as "rest immediately" (D-55).
	/// </summary>
	/// <param name="source">The spawn source.</param>
	public override void OnSpawn(IEntitySource source)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			StateTimer = CrawlFrames;
		}
	}

	/// <summary>
	/// The slow land crawl and the short rest between two crawl legs. The creature is passive: it never
	/// acquires a target and never attacks, so the only damage it can ever deal is its (zero) contact
	/// damage. The walk direction reverses at a wall or at an unwalkable ledge, exactly as 格普螺 does.
	/// Both legs are timed on the authoritative side only, and every transition goes through
	/// <see cref="EnterState"/> with an <c>NPC.netUpdate</c>, so a client reads the synced state instead
	/// of predicting one (D-55).
	/// </summary>
	public override void AI()
	{
		// A deterministic heading: no TargetClosest() is called, because this passive creature must never
		// set NPC.target (its sibling 格普螺 uses the target only as a facing helper, and this row does
		// not need even that).
		if (NPC.direction == 0)
		{
			NPC.direction = 1;
		}

		if (State == SmallGuppyConchState.Resting)
		{
			// The rest leg: the snail holds still and does not turn, so the pause reads as one snail
			// pausing rather than a creature pacing.
			NPC.velocity.X = 0f;
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				StateTimer--;
				if (StateTimer <= 0)
				{
					EnterState(SmallGuppyConchState.Crawling);
				}
			}

			return;
		}

		// Reverse at a wall or when the tile ahead of the leading edge is not walkable, so the crawl turns
		// around instead of pushing into terrain forever (the 格普螺 behaviour).
		if (NPC.collideX || !HasFloorAhead())
		{
			NPC.direction *= -1;
		}

		NPC.spriteDirection = NPC.direction;
		NPC.velocity.X = CrawlSpeed * NPC.direction;

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			StateTimer--;
			if (StateTimer <= 0)
			{
				EnterState(SmallGuppyConchState.Resting);
			}
		}
	}

	/// <summary>
	/// The single state-transition helper, so the crawl/rest switch is written in exactly one place and is
	/// synced with <c>NPC.netUpdate</c>. Only reached from the authoritative side (D-55); a client reads
	/// the synced <c>NPC.ai[0]</c> / <c>NPC.localAI[0]</c> back instead of predicting a transition.
	/// </summary>
	/// <param name="nextState">The state to enter.</param>
	private void EnterState(SmallGuppyConchState nextState)
	{
		if (State == nextState)
		{
			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			State = nextState;
			StateTimer = nextState == SmallGuppyConchState.Resting ? RestFrames : CrawlFrames;
			NPC.netUpdate = true;
		}
	}

	/// <summary>
	/// True when the tile just below and ahead of the leading edge is solid, i.e. the crawler can keep
	/// going. A world-edge position reports true so the engine boundary handles it.
	/// </summary>
	/// <returns>True when the crawler has a floor ahead of it.</returns>
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

	/// <summary>
	/// The design's 减伤 5: incoming damage is scaled to 95%. The design carries no second state with its
	/// own reduction and this class has no defence switch, so the scale is unconditional.
	/// </summary>
	/// <param name="modifiers">The hit's modifiers; only <c>FinalDamage</c> is touched.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
		modifiers.FinalDamage *= 0.95f; // 减伤 5
	}

	/// <summary>
	/// The death dust. Client-only work, so the whole body sits behind the dedicated-server guard
	/// (D-35/D-55) and reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	/// <param name="hit">The resolved hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0 && !Main.dedServ)
		{
			for (int i = 0; i < 6; i++)
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
	/// Yggdrasil, so this per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because the
	/// hook runs in single player or on the server only, where the client camera is zero (D-52/D-55).
	/// <para>
	/// The creature is a land crawler, so the water spawn flag is rejected explicitly exactly as 格普螺
	/// rejects it, and the shared dry-land predicate (which folds in the same rejection plus the
	/// liquid-tile test) is the second gate. The design supplies no weight, so the conservative D-54 land
	/// band value is used. The 森雨幽谷 region refinement is a Phases 5-6 predicate gap
	/// (04-DEVIATIONS.md section 5).
	/// </para>
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>The conservative land weight, or <c>0f</c> outside the design's context.</returns>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		if (spawnInfo.Water || !KelpCurtainSpawnConditions.IsDryLand(spawnInfo))
		{
			return 0f;
		}

		return KelpCurtainSpawnConditions.LandWeight;
	}

	/// <summary>
	/// The design names no loot for this row (its drop block is absent), so the table is deliberately
	/// empty with no type reference at all (the VerdantRods precedent, D-57/D-58). The design's
	/// 可以被捕获 capture is the row's only item-scope dependency and it is recorded as a blocker rather
	/// than a reference.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
