using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Walls.TwilightForest;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class GreenRelicBrick_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Bakery_Scene>());
		Item.width = 16;
		Item.height = 16;
	}

	public override void HoldItem(Player player)
	{
		// Green brick runner.
		Item.DefaultToPlaceableTile(ModContent.TileType<Bakery_Scene>());
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			//Point point = Main.MouseWorld.ToTileCoordinates();
			//var checkTiles = TileUtils.BFSContinueTile(point, true, 1024);
			//foreach (var tile in checkTiles)
			//{
			//	if (tile.TileType == TileID.GreenDungeonBrick)
			//	{
			//		tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick>();
			//	}
			//}
		}
	}
}
