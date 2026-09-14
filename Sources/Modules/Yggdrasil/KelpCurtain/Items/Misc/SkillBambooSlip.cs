namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

/// <summary>
/// 技能竹简 - identity-only shell (D-18/D-19). The design row carries no 伤害 / 击退 / 暴击 /
/// 使用时间 / 其他数值 / 价格 / 稀有度 / 效果 cells; its 描述 reads 提交给NPC后学习 (learned after
/// submission to an NPC), so the item belongs to the skill system, which is not implemented in
/// this phase, and the class is an identity only. Placed in Items/Misc/ beside the quest-item
/// precedent <see cref="ForestBreath"/>. The approved artwork (贴图) is absent from the repository (D-13).
/// </summary>
public class SkillBambooSlip : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Miscs;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
	}
}
