using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using SubworldLibrary;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs;

/// <summary>
/// 叶飞棍（Verdant Rods），亡碧湖（Death Jade Lake）与森雨幽谷（Valley of Lush and Moist）的中立飞棍。
/// Design row: 叶飞棍, heading block id RPs8db7IMof8ilxWLPNccOtPnBe,
/// stats table EU6pdMiwLoZxqNxi0KDcnceonBb, behavior CFCDdFU78ozdhSxpAQsc64btnyd,
/// drop H2dQdrngYoGevPxYaDlc5oganOg (D-28 full implementation).
/// This file sits beside the already-approved VerdantRods.png (54x432, an eight-frame vertical
/// strip), so tML's default texture resolution finds the art with no Texture override, no
/// handwritten asset path and no asset move.
/// </summary>
public class VerdantRods : ModNPC
{
	/// <summary>
	/// The design gives no suffocation cadence, so a conservative 30-tick interval with a small
	/// drain is used and recorded in 03-DEVIATIONS.md section 4 (D-34).
	/// </summary>
	private const int SuffocationInterval = 30;

	/// <summary>
	/// The per-tick suffocation drain. Deliberately small: the design says the rod suffocates in
	/// water like a vanilla land creature but can fly out, so the drain must never kill it.
	/// </summary>
	private const int SuffocationDamage = 2;

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[0]</c>: the ticks elapsed while the rod is submerged.
	/// </summary>
	private int SuffocationTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>
	/// The point the hover AI drifts toward, mirroring the <c>DarkGlimmeringRods</c> precedent.
	/// It starts near the creature and is re-aimed at the closest player's centre so the rod
	/// circles nearby instead of hovering in place.
	/// </summary>
	public Vector2 TargetPos = Vector2.Zero;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	public override void SetStaticDefaults()
	{
		// The approved sprite is 54x432; 432 / 54 = 8, so it is an eight-frame vertical strip,
		// matching the existing DarkGlimmeringRods eight-frame precedent.
		Main.npcFrameCount[NPC.type] = 8;
		NPCSpawnManager.RegisterNPC(Type);

		// 免疫 中毒 (the design row's 免疫 cell): the poisoned debuff is the only immunity.
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
	}

	public override void SetDefaults()
	{
		// No vanilla aiStyle provides this neutral hover-and-circle behaviour, so the class owns
		// its AI() (D-31 permits a local AI() where no vanilla style fits).
		NPC.aiStyle = -1;

		// One 54x54 frame of the eight-frame strip.
		NPC.width = 54;
		NPC.height = 54;

		NPC.lifeMax = 100;
		NPC.life = 100;
		NPC.damage = 30;
		NPC.defense = 4;
		NPC.knockBackResist = 0.8f; // 击退抗性 20% -> 1 - 0.20

		// 钱币（铜） 2银 = 200 copper; NPC.value is documented in copper coins, so the design
		// value transposes 1:1 with no conversion.
		NPC.value = 200;

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no
		// design rarity exists (03-DEVIATIONS.md section 4). NPC.rarity is the engine's only NPC
		// rarity field (Lifeform Analyzer) and defaults to ItemRarityID.White, so this no-op records
		// the design's empty cell.
		NPC.rarity = ItemRarityID.White;

		NPC.noGravity = true;
		NPC.friendly = false;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// Not 可被捕捉: only 水蛞蝓, 装甲虾 and 爆弹水母 are, so no catch item, no
		// Main.npcCatchable and no NPCID.Sets.CountsAsCritter is set here.
		NPC.catchItem = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	public override void OnSpawn(IEntitySource source)
	{
		// A point near the creature, mirroring DarkGlimmeringRods; the AI re-aims it at the
		// closest player so the rod circles nearby.
		TargetPos = NPC.Center + new Vector2(Main.rand.Next(-210, 210), Main.rand.Next(-210, -50));
	}

	/// <summary>
	/// The eight-frame strip animation. The engine calls this on the server and on clients, so the
	/// animation needs no <c>Main.dedServ</c> guard (D-35 covers dust/gore/VFX, not frames).
	/// </summary>
	public override void FindFrame(int frameHeight)
	{
		NPC.frameCounter = (NPC.frameCounter + 0.15f) % Main.npcFrameCount[NPC.type];
		NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
	}

	/// <summary>
	/// The neutral hover: drift toward the orbiting target with a sine wobble, damp the velocity,
	/// rotate with the horizontal speed (the DarkGlimmeringRods precedent), and represent the
	/// design's water suffocation. The creature never melees on purpose, so there is no attack
	/// branch: <c>TargetClosest(false)</c> only selects the player the rod circles around.
	/// </summary>
	public override void AI()
	{
		float timeValue = (float)(Main.time * 0.014f);

		NPC.TargetClosest(false);
		if (NPC.target >= 0 && NPC.target < Main.maxPlayers)
		{
			Player target = Main.player[NPC.target];
			if (target.active && !target.dead)
			{
				TargetPos = target.Center;
			}
		}

		// 在水里会像原版陆生生物一样窒息, but it flies back out. Both conditions are checked so a
		// thin film of liquid does not trigger the drain.
		bool submerged = NPC.wet && IsCenterTileLiquid();
		if (submerged)
		{
			// Bias the hover target upward so the rod climbs out of the water ("可以正常飞出来").
			TargetPos.Y -= 220f;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			if (submerged)
			{
				SuffocationTimer++;
				if (SuffocationTimer >= SuffocationInterval)
				{
					SuffocationTimer = 0;
					if (NPC.life > 1)
					{
						// Never below 1 life: the rod suffocates and escapes, it does not drown.
						NPC.life = Math.Max(1, NPC.life - SuffocationDamage);
						NPC.netUpdate = true;
					}
				}
			}
			else
			{
				SuffocationTimer = 0;
			}
		}

		NPC.velocity *= 0.97f;
		if (MathF.Abs(NPC.velocity.X) > 0.05f)
		{
			NPC.direction = NPC.velocity.X > 0f ? 1 : -1;
		}
		NPC.spriteDirection = NPC.direction;
		NPC.rotation = MathF.Log(MathF.Abs(NPC.velocity.X) + 1f) * 0.2f * NPC.direction * 0.05f + NPC.rotation * 0.95f;

		Vector2 aimTarget = TargetPos + new Vector2(160f * MathF.Sin(timeValue * 2f + NPC.whoAmI), -40f + 30f * MathF.Sin(timeValue * 0.15f + NPC.whoAmI));

		// 偶尔会点水: when a water source sits just below, let the drift dip toward its surface.
		// Purely cosmetic, computed from synced world state so no client-only value is written.
		if (!submerged && TryFindLiquidSurfaceBelow(out float liquidSurfaceY))
		{
			float dip = 0.5f + 0.5f * MathF.Sin(timeValue * 3f + NPC.whoAmI);
			aimTarget.Y = MathHelper.Lerp(aimTarget.Y, liquidSurfaceY, dip * 0.5f);
		}

		Vector2 toAim = aimTarget - NPC.Center - NPC.velocity;
		if (toAim.Length() > 50f)
		{
			NPC.velocity += Vector2.Normalize(toAim) * 0.12f;
		}
	}

	/// <summary>
	/// True when the tile at the creature's centre carries liquid, the second half of the design's
	/// submerged test.
	/// </summary>
	private bool IsCenterTileLiquid()
	{
		int tileX = (int)(NPC.Center.X / 16f);
		int tileY = (int)(NPC.Center.Y / 16f);
		return WorldGen.InWorld(tileX, tileY, 1) && Main.tile[tileX, tileY].LiquidAmount > 0;
	}

	/// <summary>
	/// Finds a liquid surface within a few tiles below the rod for the "occasionally skims water"
	/// flavour. Returns false when no liquid is close.
	/// </summary>
	private bool TryFindLiquidSurfaceBelow(out float surfaceY)
	{
		surfaceY = 0f;
		int tileX = (int)(NPC.Center.X / 16f);
		int startTileY = (int)(NPC.Center.Y / 16f);
		int endTileY = Math.Min(startTileY + 6, Main.maxTilesY - 1);
		for (int tileY = startTileY; tileY <= endTileY; tileY++)
		{
			if (!WorldGen.InWorld(tileX, tileY, 1))
			{
				break;
			}
			if (Main.tile[tileX, tileY].LiquidAmount > 0)
			{
				surfaceY = tileY * 16f;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 50% chance of 中毒 for 10 s (600 ticks) on the players this creature damages. tML documents
	/// <c>ModNPC.OnHitPlayer</c> as "Called on the local client only", so this is deliberately NOT
	/// wrapped in <c>Main.netMode != NetmodeID.MultiplayerClient</c>: such a guard would make the
	/// documented 50% poison dead code in multiplayer. The hook runs once on the client that owns
	/// the hit player, and <c>Player.AddBuff</c> performs its own hurt-path client -> server buff
	/// sync, exactly as <c>LargeBloodLanternGhost</c> and <c>LeafcutterAnt</c> do.
	/// </summary>
	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		if (Main.rand.NextBool(2))
		{
			target.AddBuff(BuffID.Poisoned, 600);
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0 && !Main.dedServ)
		{
			for (int i = 0; i < 6; i++)
			{
				int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<YggdrasilCyatheaLeafDust>(), 2 * hit.HitDirection, -2f);
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
	/// this hook runs in single player or on the server only, where the client camera is zero. The
	/// design supplies no weight, so a conservative 1f is used (band 0.5f-2f, D-34). A submerged
	/// spawn tile is rejected: this is an open-air flying creature. The per-region refinement
	/// (亡碧湖 vs 森雨幽谷) is a recorded blocker because no regional biome predicate exists yet.
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

		return 1f;
	}

	/// <summary>
	/// The design row's two drops are both absent from the repository — 33% 飞棍毛发 and 11% 毒腺 —
	/// so the table is deliberately empty with no type reference (D-37/D-39). Writing a
	/// <c>ModContent.ItemType&lt;...&gt;()</c> for either would be a compile error that breaks the
	/// whole mod build; the blocker is recorded in 03-BIOLOGY.json instead.
	/// </summary>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
