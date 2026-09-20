using Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.GravelStore;

public class BakeryShelf_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BakeryShelf>());
		Item.width = 68;
		Item.height = 56;
	}
}
