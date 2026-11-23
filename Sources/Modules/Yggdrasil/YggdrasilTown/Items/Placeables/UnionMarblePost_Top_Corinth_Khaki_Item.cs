using Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class UnionMarblePost_Top_Corinth_Khaki_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<UnionMarblePost_Top_Khaki>());
		Item.width = 42;
		Item.height = 34;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
	}
}
