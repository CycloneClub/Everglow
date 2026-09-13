namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

/// <summary>
/// 森林之息 - quest item (任务道具) procured from the underwater-maze chest
/// (水下迷宫宝箱物品); its consumer is Phase 6 quest content.
/// </summary>
public class ForestBreath : ModItem
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
