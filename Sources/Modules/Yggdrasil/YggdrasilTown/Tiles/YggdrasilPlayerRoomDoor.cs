using Everglow.Commons.TileHelper;
using Everglow.SubSpace;
using Everglow.SubSpace.Tiles;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilPlayerRoomDoor : RoomDoorTile
{
	public void BuildPlayerRoomGen()
	{
		var mapIO = new MapIO(100, 110);

		mapIO.Read(ModIns.Mod.GetFileStream(ModAsset.MapIOs_107x60PlayerHouse_Path));

		var it = mapIO.GetEnumerator();
		while (it.MoveNext())
		{
			WorldGen.SquareTileFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
			WorldGen.SquareWallFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
		}

		for (int x = 20; x < 22; x++)
		{
			for (int y = 20; y < 23; y++)
			{
				Tile tile = TileUtils.SafeGetTile(x, y);
				tile.WallType = 1;
				ushort typeChange = (ushort)ModContent.TileType<PlayerRoomCommandBlock>();
				if (y == 22)
				{
					typeChange = 0;
				}
				else
				{
					tile.TileFrameX = (short)((x - 20) * 18);
					tile.TileFrameY = (short)((y - 20) * 18);
				}
				tile.TileType = typeChange;
				tile.HasTile = true;
			}
		}
	}

	public override bool RightClick(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		var point = new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
		RoomManager.EnterNextLevelRoom(point, new Point(150, 150), BuildPlayerRoomGen);
		return false;
	}
}
