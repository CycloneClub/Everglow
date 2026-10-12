using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

/// <summary>
/// 爆弹水母（Bomb Jellyfish，小号），亡碧湖（Death Jade Lake）水中悬停的被动水母。
/// Design row: 爆弹水母, heading block id LDXndBembo98qsxPzlLcPt0An9d,
/// behavior blocks OwnZdVZaJogXNSxk6M7cSF2enHW / WxyJdoRyXo0cFNxkaGac67tNnde (D-28 full implementation).
/// 生命 10, 免疫击退; the design's 伤害, 防御, 减伤, 免疫 and 钱币 cells are all empty.
/// <para>
/// <b>Stat variant (OQ2).</b> The design's 小/大 pair has different life and a different death blast, and
/// <c>SetDefaults</c> runs once per NPC <i>type</i>, so the two cannot share one class. This is the row's
/// primary class (its <c>internal_name</c>); 大 ships as the sibling
/// <see cref="LargeBombJellyfish"/> and neither class ever mutates <c>NPC.lifeMax</c> at runtime (Pitfall 5).
/// </para>
/// <para>
/// <b>Only threat (OQ4).</b> 死亡后自爆，造成30（小）伤害: the creature itself deals no contact damage, so
/// its whole threat is the death blast, delivered by the shared <see cref="BombJellyfish_Explosion"/>
/// projectile rather than being blocked (04-DEVIATIONS.md section 7 item 9).
/// </para>
/// <para>
/// <b>Capture blocker (D-58).</b> The design says 可被捕捉, but no catch item for this creature exists
/// anywhere in the repository, so no catch-item type, no <c>Main.npcCatchable</c> flag and no
/// <c>NPCID.Sets.CountsAsCritter</c> entry are written here.
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the class requests the shared
/// Commons.ModAsset.White_Mod fallback (D-48). The DeathJadeLake region subfolder is what tML's default
/// (namespace-derived) texture resolution expects, so when the art arrives the migration is adding
/// BombJellyfish.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime asset
/// path is Everglow/Yggdrasil/KelpCurtain/NPCs/DeathJadeLake/BombJellyfish.png.
/// </para>
/// </summary>
public class BombJellyfish : ModNPC
{
	/// <summary>死亡后自爆，造成30: the 小 variant's death-blast damage (the design's own number).</summary>
	private const int DeathBlastDamage = 30;

	/// <summary>只会有些许的上下摆动: the hover bob's speed, in pixels per tick (D-54 default).</summary>
	private const float BobSpeed = 0.14f;

	/// <summary>
	/// How long one leg of the up/down bob lasts, in frames (D-54 default). The design only says 些许的
	/// 上下摆动 with no cadence, so a slow two-second leg keeps the motion a hover rather than a bounce.
	/// </summary>
	private const int BobLegFrames = 120;

	/// <summary>
	/// The two legs of the design's 些许的上下摆动 (D-31). No vanilla <c>aiStyle</c> provides a hover that
	/// neither moves nor drifts, so the class owns its <c>AI()</c>.
	/// </summary>
	private enum BombJellyfishState
	{
		Rising = 0,
		Sinking = 1,
	}

	/// <summary>Named wrapper over <c>NPC.ai[0]</c> so no bare numeric index is scattered through the class.</summary>
	private BombJellyfishState State
	{
		get => (BombJellyfishState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	/// <summary>Named wrapper over <c>NPC.localAI[0]</c>: the frames left in the current bob leg.</summary>
	private int BobTimer
	{
		get => (int)NPC.localAI[0];
		set => NPC.localAI[0] = value;
	}

	/// <summary>
	/// Named wrapper over <c>NPC.localAI[1]</c>: set once on the authoritative side when the death blast has
	/// been spawned, so 死亡后自爆 happens exactly once even if the engine reports the creature's death more
	/// than once.
	/// </summary>
	private bool HasDetonated
	{
		get => NPC.localAI[1] >= 1f;
		set => NPC.localAI[1] = value ? 1f : 0f;
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

		// The design row's 免疫 cell is empty, so no specific debuff immunity is declared.
	}

	public override void SetDefaults()
	{
		// A motionless hover has no vanilla aiStyle analogue, so the class owns its AI (D-31).
		NPC.aiStyle = -1;

		// No sprite exists to measure, so a small jellyfish box is used and is the first value to revisit
		// when the art arrives (D-54).
		NPC.width = 26;
		NPC.height = 26;

		NPC.lifeMax = 10; // 生命 10（小）
		NPC.life = 10;

		// The design's 伤害 cell is empty: the creature's only threat is its 死亡后自爆, so its contact
		// damage is 0 and no design damage value is invented.
		NPC.damage = 0;
		NPC.defDamage = 0;

		// The design's 防御 cell is empty, so a small aquatic default is used instead of a design-derived
		// value (D-54).
		NPC.defense = 2;
		NPC.defDefense = 2;

		// 免疫击退: the design's 击退抗性 cell reads 免疫击退, i.e. full knockback immunity.
		NPC.knockBackResist = 0f;

		// The design's 钱币 cell is empty, so the creature carries no coin value.
		NPC.value = 0;

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no design
		// rarity exists. NPC.rarity is the engine's only NPC rarity field (the API-name correction
		// recorded in 04-DEVIATIONS.md section 10).
		NPC.rarity = ItemRarityID.White;

		// 在水中 ... 悬停: it holds its position instead of sinking, and tile collision stays on so it cannot
		// hover out through the lake bed.
		NPC.noGravity = true;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// 可被捕捉 in the design, but no catch item for this creature exists in the repository, so the
		// capture is a blocked item-scope dependency (D-58): no catch-item type reference, no catchable
		// flag and no critter-count set entry are written here.
		NPC.catchItem = 0;

		// Deterministic starting values, because SetDefaults runs on every side: no Main.rand is consumed
		// here, so the server and its clients agree before the first sync arrives.
		State = BombJellyfishState.Rising;
		BobTimer = BobLegFrames;
		HasDetonated = false;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// 在水中单独刷新并且悬停，只会有些许的上下摆动但是不会移动: the hover never travels horizontally and
	/// only alternates between the two gentle vertical legs. The leg switch is authoritative and rides the
	/// synced <c>NPC.ai[]</c> / <c>NPC.localAI[]</c> arrays (D-55).
	/// </summary>
	public override void AI()
	{
		UpdateBobLeg();

		float direction = State == BombJellyfishState.Sinking ? 1f : -1f;

		// 不会移动: any residual horizontal velocity is damped out, so a current can never carry it away.
		NPC.velocity.X *= 0.9f;
		NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, direction * BobSpeed, 0.05f);

		UpdateGlow();
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (behavior block OwnZdVZaJogXNSxk6M7cSF2enHW), so no
	/// <c>FinalDamage</c> scaling is applied here. The omission is the design's, not the plan's.
	/// </summary>
	/// <param name="modifiers">The hit's modifiers; deliberately untouched.</param>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// 死亡后自爆，造成30（小）伤害: one <see cref="BombJellyfish_Explosion"/> is spawned from the creature's
	/// own AI source on the authoritative side only (D-55), so one blast appears per death and is never
	/// duplicated per client. The design's damage travels to the projectile as its damage parameter, and
	/// the missing capture item stays a blocker rather than a reference (D-58).
	/// </summary>
	/// <param name="hit">The killing hit.</param>
	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life > 0)
		{
			return;
		}

		if (Main.netMode != NetmodeID.MultiplayerClient && !HasDetonated)
		{
			HasDetonated = true;

			// ai0 carries the design's blast damage so the explosion's own radius reads a named ai[] value
			// instead of a per-class constant (04-DEVIATIONS.md section 7 item 9).
			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BombJellyfish_Explosion>(), DeathBlastDamage, 0f, -1, DeathBlastDamage);
			NPC.netUpdate = true;
		}

		if (!Main.dedServ)
		{
			for (int i = 0; i < 8; i++)
			{
				int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<KelpWaterDrop>(), 0f, -0.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.3f);
			}
		}
	}

	/// <summary>
	/// The design names no loot for this row (its drop cell is empty), so the table is deliberately empty
	/// with no type reference at all (the VerdantRods precedent, D-57/D-58). The row's only item-scope
	/// dependency is the absent capture item, which is recorded as a blocker rather than a reference.
	/// </summary>
	/// <param name="npcLoot">The loot table being built.</param>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}

	/// <summary>
	/// 只会有些许的上下摆动: alternates the two bob legs on the authoritative side only and pushes the new
	/// value to the clients through the synced arrays (D-55).
	/// </summary>
	private void UpdateBobLeg()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}

		BobTimer--;
		if (BobTimer > 0)
		{
			return;
		}

		BobTimer = BobLegFrames;
		State = State == BombJellyfishState.Rising ? BombJellyfishState.Sinking : BombJellyfishState.Rising;
		NPC.netUpdate = true;
	}

	/// <summary>
	/// 在水中 ... 悬停: a faint drift of water dust so a motionless creature still reads as alive.
	/// Client-only work, so it sits behind the dedicated-server guard (D-35/D-55) and reuses an existing
	/// Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	private void UpdateGlow()
	{
		if (Main.dedServ || !Main.rand.NextBool(10))
		{
			return;
		}

		int dust = Dust.NewDust(NPC.Center, 0, 0, ModContent.DustType<KelpWaterDrop>(), 0f, 0f);
		Main.dust[dust].noGravity = true;
		Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
	}
}
