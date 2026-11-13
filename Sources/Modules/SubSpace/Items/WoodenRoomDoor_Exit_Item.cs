using Everglow.SubSpace.Tiles;
using SubworldLibrary;

namespace Everglow.SubSpace.Items;

public class WoodenRoomDoor_Exit_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<WoodenRoomDoor_Exit>());
		Item.width = 50;
		Item.height = 46;
	}

	public override bool CanUseItem(Player player)
	{
		return true;
	}
}
