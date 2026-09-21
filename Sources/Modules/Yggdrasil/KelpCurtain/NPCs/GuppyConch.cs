using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Accessories;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs;

/// <summary>
/// 格普螺（Guppy Conch），森雨幽谷（Valley of Lush and Moist）的中立巨型蜗牛。
/// Design row: 格普螺, heading block id K2TldVE5woR5xQxjJA8cGswqnnd,
/// stats table IZAjdqbGQoNuYzxnEJGcpTEEnQc, behavior FsnSd54BHoDUP9xGftGceO7Ynmg,
/// drop IqEbdZxOKo0CoHxSqmgcen4anCg (D-28 full implementation).
/// This file sits beside the already-approved GuppyConch.png, so tML's default texture
/// resolution finds the art with no Texture override, no handwritten asset path and no asset move.
/// </summary>
public class GuppyConch : ModNPC
{
	/// <summary>
	/// The design's crawl speed: the row asks for a snail that "缓慢地在地面上爬行", so the value is
	/// kept well below the standard walk speed instead of being derived from a vanilla AI.
	/// </summary>
	private const float CrawlSpeed = 0.4f;

	/// <summary>
	/// The design gives no shell duration, so a conservative two-second retract window is used and
	/// recorded in 03-DEVIATIONS.md section 4 (D-34).
	/// </summary>
	private const int ShellDuration = 120;

	/// <summary>
	/// The two local states of the design row (D-31): a slow crawl and the hit-triggered shell
	/// retract. No vanilla <c>aiStyle</c> provides a hit-triggered retract state, so the class
	/// writes its own <c>AI()</c> and stores the state in <c>NPC.ai[0]</c>.
	/// </summary>
	private enum GuppyConchState
	{
		Crawling = 0,
		Shelled = 1,
	}

	/// <summary>
	/// Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.
	/// </summary>
	private GuppyConchState State
	{
		get => (GuppyConchState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[0]</c>: the remaining ticks of the shell retract window.
	/// </summary>
	private int ShellTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	public override void SetStaticDefaults()
	{
		// The approved sprite (114x58) is a single frame: textureHeight / textureWidth is below 1,
		// so the whole texture is one frame (recording the derived value, not a design value).
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// 免疫 困惑 (the design row's 免疫 cell): the confused debuff is the only immunity.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override void SetDefaults()
	{
		// No vanilla aiStyle provides a hit-triggered shell retract, so the class owns its AI
		// (D-31 permits a local AI() where no vanilla style fits).
		NPC.aiStyle = -1;

		// Sprite-derived extents: 格普螺 has a single-frame 114x58 texture.
		NPC.width = 114;
		NPC.height = 58;

		NPC.lifeMax = 300;
		NPC.life = 300;
		NPC.damage = 25;
		NPC.defense = 10; // 防御 10（正常）/ 20（缩壳）
		NPC.knockBackResist = 0.15f; // 击退抗性 85% -> 1 - 0.85

		// 钱币（铜） 4银 = 400 copper; NPC.value is documented in copper coins, so the design
		// value transposes 1:1 with no conversion.
		NPC.value = 400;

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no
		// design rarity exists (03-DEVIATIONS.md section 4). NPC.rarity is the engine's only NPC
		// rarity field (Lifeform Analyzer) and defaults to ItemRarityID.White, so this no-op records
		// the design's empty cell.
		NPC.rarity = ItemRarityID.White;

		NPC.friendly = false;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// Not 可被捕捉: only 水蛞蝓, 装甲虾 and 爆弹水母 are, so no catch item, no
		// Main.npcCatchable and no NPCID.Sets.CountsAsCritter is set here.
		NPC.catchItem = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// The slow ground crawl plus the shell retract window. The creature is neutral: it never
	/// initiates an attack, so the closest player is consulted only to keep the sprite facing a
	/// sensible way (and only while choosing a new heading); its only damage is contact damage.
	/// </summary>
	public override void AI()
	{
		if (State == GuppyConchState.Shelled)
		{
			// While retracted the crawl velocity stays zero and the timer runs on the server only.
			NPC.velocity.X = 0f;
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				ShellTimer--;
				if (ShellTimer <= 0)
				{
					State = GuppyConchState.Crawling;
					NPC.defense = 10; // 防御 10（正常）
					NPC.netUpdate = true;
				}
			}
			return;
		}

		// Facing helper only: the design row never attacks, so TargetClosest is used to orient the
		// sprite when the crawler has to pick a heading, never to drive an attack.
		if (NPC.direction == 0 || NPC.collideX || !HasFloorAhead())
		{
			NPC.TargetClosest();
		}

		// Reverse at a wall or when the tile ahead of the leading edge is not walkable, so the slow
		// crawl turns around instead of pushing into terrain forever.
		if (NPC.collideX || !HasFloorAhead())
		{
			NPC.direction *= -1;
		}

		NPC.spriteDirection = NPC.direction;
		NPC.velocity.X = CrawlSpeed * NPC.direction;
	}

	/// <summary>
	/// True when the tile just below and ahead of the leading edge is solid, i.e. the crawler can
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

	/// <summary>
	/// 缩壳: enters the shell state on the server, freezing the crawl and raising 防御 to 20.
	/// Idempotent, so a second hit inside the retract window never resets the timer.
	/// </summary>
	private void EnterShell()
	{
		if (State == GuppyConchState.Shelled)
		{
			return;
		}

		State = GuppyConchState.Shelled;
		ShellTimer = ShellDuration;
		NPC.defense = 20; // 防御 20（缩壳）
		NPC.velocity = Vector2.Zero;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// The design row's 减伤 cells: 15（正常） and 30（缩壳）, i.e. 0.85 and 0.70 of the incoming
	/// damage. Only <c>FinalDamage</c> is scaled: the defence switch is applied to <c>NPC.defense</c>
	/// at the state transition in <see cref="HitEffect"/> so the value stays server-authoritative and
	/// is not counted twice. The hit that triggers the retraction is still resolved with the
	/// normal-state 0.85 scale; every following hit lands while Shelled and takes the 0.70 scale.
	/// </summary>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
		if (State == GuppyConchState.Shelled)
		{
			modifiers.FinalDamage *= 0.70f; // 减伤 30%（缩壳）
		}
		else
		{
			modifiers.FinalDamage *= 0.85f; // 减伤 15%（正常）
		}
	}

	/// <summary>
	/// The hit-triggered shell retract and the death dust. The transition lives here rather than in
	/// <see cref="ModifyIncomingHit"/> because tML documents that hook as "ONLY ... properties of the
	/// HitModifiers" with side effects belonging to the OnHit hooks; <c>HitEffect</c> is the
	/// documented on-hit hook and is called on the server as well, so the transition is applied once
	/// on the authoritative side (<c>Main.netMode != NetmodeID.MultiplayerClient</c>) with an
	/// <c>NPC.netUpdate</c> and the client prediction follows the synced state.
	/// </summary>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0)
		{
			if (!Main.dedServ)
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
			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			EnterShell();
		}
	}

	/// <summary>
	/// Subworld-only spawning (BIO-06). <c>NPCSpawnManager.EditSpawnPool</c> returns early outside
	/// Yggdrasil, so the per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because
	/// this hook runs in single player or on the server only, where the client camera is zero. This
	/// creature is a land crawler, so submerged spawn tiles and water spawns are rejected. The
	/// design supplies no weight, so a conservative 0.75f is used (band 0.5f-2f, D-34).
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

		return 0.75f;
	}

	/// <summary>
	/// 11% for 1 格普螺外壳 (the denominator is 9); the item was implemented in Phase 1. The design
	/// row's other drop, 0~2 软体甲壳碎片, has no repository ModItem, so no rule is written for it
	/// (D-37/D-39) and the blocker is recorded in 03-BIOLOGY.json instead of a type reference.
	/// </summary>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GuppyShell>(), 9, 1, 1));
	}
}
