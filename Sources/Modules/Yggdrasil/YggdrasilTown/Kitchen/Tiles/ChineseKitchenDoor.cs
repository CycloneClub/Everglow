using Everglow.Commons.TileHelper;
using Everglow.SubSpace;
using Everglow.SubSpace.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Kitchen.Tiles;

public class ChineseKitchenDoor : RoomDoorTile
{
	public void BuildChineseStyleKitchenGen()
	{
		var mapIO = new MapIO(5, 5);

		mapIO.Read(ModIns.Mod.GetFileStream(ModAsset.ChineseStyleKitchen_Whole_Path));

		var it = mapIO.GetEnumerator();
		while (it.MoveNext())
		{
			WorldGen.SquareTileFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
			WorldGen.SquareWallFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
		}
	}

	public override bool RightClick(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		Point point = new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
		RoomManager.EnterNextLevelRoom(point, default, BuildChineseStyleKitchenGen);
		return false;
	}
}
