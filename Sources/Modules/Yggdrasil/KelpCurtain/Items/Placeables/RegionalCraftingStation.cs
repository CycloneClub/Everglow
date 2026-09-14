namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

/// <summary>
/// 区域放置物品制作台 - identity-only shell (D-18/D-19), a plain <see cref="ModItem"/> with no
/// placement tile. The design 描述 reads 宝箱的副掉落，非核心物品 (a non-core chest side drop);
/// its 伤害 / 击退 / 暴击 / 使用时间 / 其他数值 / 价格 / 稀有度 / 效果 cells are empty. The item
/// belongs to the regional-crafting system, which is not implemented in this phase, and the
/// placement tile it will need does not exist either, so no ModTile, no Item.createTile and no
/// DefaultToPlaceableTile call is added — a tile without its system would be half-built.
/// The approved artwork (贴图) is absent from the repository (D-13).
/// </summary>
public class RegionalCraftingStation : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

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
