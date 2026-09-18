using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.TwilightForest;

public class DragonChest_DarkDragonRoom_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<DragonChest_DarkDragonRoom>());
		Item.width = 32;
		Item.height = 26;
		Item.value = 5000;
	}
}
