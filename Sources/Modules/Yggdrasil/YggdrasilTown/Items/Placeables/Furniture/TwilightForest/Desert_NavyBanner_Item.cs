using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.TwilightForest;

public class Desert_NavyBanner_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Desert_NavyBanner>());
		Item.width = 12;
		Item.height = 28;
		Item.value = 1000;
	}
}
