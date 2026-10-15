using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.TwilightForest;

public class RubyDarkGoldChest_BloodyChurch_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<RubyDarkGoldChest_BloodyChurch>());
		Item.width = 32;
		Item.height = 26;
		Item.value = 5000;
	}
}
