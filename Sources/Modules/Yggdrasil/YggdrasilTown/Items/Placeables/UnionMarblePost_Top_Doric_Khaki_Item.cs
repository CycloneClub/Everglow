using Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class UnionMarblePost_Top_Doric_Khaki_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.width = 42;
		Item.height = 34;
		Item.maxStack = Item.CommonMaxStack;
		Item.createTile = ModContent.TileType<UnionMarblePost_Top_Khaki>();
		Item.placeStyle = 2;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 15;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = true;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
	}
}
