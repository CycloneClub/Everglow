namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

/// <summary>
/// 桃花纸鸢（风筝） - identity-only misc shell (D-18). The design row carries no
/// 伤害 / 价格 / 稀有度 / 效果 cells, and its 描述 cell reads 宝箱的副掉落，非核心物品 (a non-core
/// chest side drop), so no gameplay behaviour is defined yet and none is invented.
/// Approved artwork is absent from the repository.
/// </summary>
public class PeachBlossomKite : ModItem
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
