using Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.GravelStore;

public class BakeryGlassTable_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BakeryGlassTable>());
		Item.width = 26;
		Item.height = 22;
	}
}
