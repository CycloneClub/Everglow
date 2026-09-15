using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Accessories;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs;

/// <summary>
/// 荆棘苔龟（Thorn Mossy Tortoise），森雨幽谷（Valley of Lush and Moist）的普通敌怪。
/// Design row: 荆棘苔龟（Thorn Mossy Tortoise）, region h1 WxatdnICsojUQoxS6VXcXeYgnwd,
/// heading block id GMtsdqCSxo6IbWx0Vlhcdt8gnMS, stats table Cqvwdoc54o8vsWx3mIpcoKpinVg.
/// This file sits beside the already-approved MossyThornTurtle.png, so tML's default texture
/// resolution finds the art with no Texture override, no handwritten asset path and no asset move.
/// </summary>
public class MossyThornTurtle : ModNPC
{
	/// <summary>
	/// The state values of the vanilla <c>AI_039_Tortoise</c> behaviour installed by
	/// <see cref="NPC.CloneDefaults(int)"/> with <see cref="NPCID.GiantTortoise"/>:
	/// 0 walking, 1 retracting, 3 the spinning leap, 4 landing/recovering. These are vanilla
	/// constants read back from the cloned AI — a hit in any state resets them to 0.
	/// </summary>
	private enum TortoiseState
	{
		Walking = 0,
		Retracting = 1,
		Spinning = 3,
		Recovering = 4,
	}

	/// <summary>
	/// Named wrapper over <c>NPC.ai[0]</c> (the vanilla tortoise state) so no bare numeric index is
	/// scattered through the class.
	/// </summary>
	private TortoiseState State => (TortoiseState)NPC.ai[0];

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.NPCs;

	public override void SetStaticDefaults()
	{
		// The approved sprite (84x46) is a single frame: textureHeight / textureWidth is not an
		// integer of at least 1, so the whole texture is one frame.
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);

		// The design row's 免疫 cell is empty, so no specific debuff immunity is declared.
	}

	public override void SetDefaults()
	{
		NPC.CloneDefaults(NPCID.GiantTortoise); // 原版王八AI (D-31)

		NPC.width = 84;
		NPC.height = 46;

		NPC.lifeMax = 140;
		NPC.life = 140;
		NPC.damage = 50;
		NPC.defense = 10;
		NPC.knockBackResist = 0.3f; // 击退抗性 70% -> 1 - 0.70

		// The templated vanilla AI restores damage = defDamage; defense = defDefense; while walking
		// and escalates from those bases while spinning, so the design's normal-state values must be
		// pinned here instead of being left at the vanilla Giant Tortoise's own bases.
		NPC.defDamage = 50;
		NPC.defDefense = 10;

		// 钱币（铜） 2银50铜 = 250 copper; NPC.value is documented in copper coins, so the design
		// value transposes 1:1 with no conversion.
		NPC.value = 250;

		// Conservative default: the design's 稀有度 cell is empty and 普通 is its 类型 cell, so no
		// design rarity exists (03-DEVIATIONS.md section 4). NPC.rarity is the engine's only NPC
		// rarity field (the Lifeform Analyzer rarity) and defaults to ItemRarityID.White, so this is
		// a no-op that records the design's empty cell.
		NPC.rarity = ItemRarityID.White;

		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;

		// Not 可被捕捉: only 水蛞蝓, 装甲虾 and 爆弹水母 are, so nothing the clone inherits is kept.
		NPC.catchItem = 0;

		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	/// <summary>
	/// Re-asserts the design row's defence and contact damage after both the vanilla AI and
	/// <c>AI()</c> have run. The cloned vanilla AI escalates its spin damage to <c>defDamage * 2</c>
	/// (100) and its spin defence to <c>defDefense * 2</c>, but the design row gives
	/// 防御 10（正常）/20（旋转/缩壳） and 伤害 50（正常）/75（旋转）, so the spin values are expressed
	/// as ratios of the <c>defDamage</c>/<c>defDefense</c> bases that <see cref="SetDefaults"/> pins.
	/// </summary>
	public override void PostAI()
	{
		if (State == TortoiseState.Spinning)
		{
			NPC.damage = (int)(NPC.defDamage * 1.5f); // 75
			NPC.defense = NPC.defDefense * 2; // 20
		}
		else
		{
			NPC.damage = NPC.defDamage; // 50
			NPC.defense = NPC.defDefense; // 10
		}
	}

	/// <summary>
	/// The design row's 减伤 cell is empty (stats table Cqvwdoc54o8vsWx3mIpcoKpinVg), so no
	/// <c>FinalDamage</c> scaling is applied here. The state-dependent defence (10 -> 20) flows
	/// through <c>NPC.defense</c> in <see cref="PostAI"/> so it uses the normal defence formula
	/// instead of being counted twice.
	/// </summary>
	public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
	{
	}

	/// <summary>
	/// The design row's melee reflect: 在旋转过程中, a player melee hit is answered with 1~10 反伤
	/// (10:1 conversion of the incoming damage, capped at 10). tML calls this hook on the client doing
	/// the damage, so the reflect is applied exactly once there and <c>Player.Hurt</c> performs its
	/// normal client -> server hurt sync (D-35 locality note in 03-DEVIATIONS.md).
	/// </summary>
	public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		if (State != TortoiseState.Spinning)
		{
			return;
		}

		if (player.whoAmI != Main.myPlayer)
		{
			return;
		}

		float reflect = MathHelper.Clamp(damageDone / 10f, 1f, 10f);
		player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), (int)reflect, player.direction);
	}

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
	/// Yggdrasil, so the per-creature gate is the real isolation, and
	/// <see cref="KelpCurtainBiome.IsKelpCurtainLayer"/> is the server-safe layer predicate because
	/// this hook runs in single player or on the server only, where the client camera is zero.
	/// The design supplies no weight, so a conservative value in the 1f-3f band is used (D-34).
	/// </summary>
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>() || !KelpCurtainBiome.IsKelpCurtainLayer(spawnInfo.Player))
		{
			return 0f;
		}

		return 1.5f;
	}

	/// <summary>
	/// 死亡后有5%概率掉落1 荆棘龟壳 — the design row names only this drop, so exactly one rule is
	/// added (5% is the denominator 20). The item was implemented in Phase 1 (D-37).
	/// </summary>
	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThornTurtleShell>(), 20, 1, 1));
	}
}
