namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

/// <summary>
/// 竹节步符 - identity-only accessory shell (D-18). The design row carries no
/// 伤害 / 价格 / 稀有度 / 效果 cells, so no gameplay behaviour can be transcribed and none is
/// invented; the design 描述 cell reads 饰品 (accessory), which selects the Accessories
/// category. Approved artwork is absent from the repository.
/// </summary>
public class BambooStepTalisman : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.accessory = true;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Blue;
	}
}
