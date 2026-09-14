namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

/// <summary>
/// 若干酒类 - identity-only shell (D-18). The design row carries no 伤害 / 击退 / 暴击 /
/// 使用时间 / 其他数值 / 价格 / 稀有度 / 效果 cells; its only populated cell is the 描述
/// 只喝了一两口就丢进去了, so no consumable buff is declared and the class is an identity
/// only. The approved artwork (贴图) is absent from the repository (D-13).
/// </summary>
public class AlcoholicDrinks : ModItem
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
