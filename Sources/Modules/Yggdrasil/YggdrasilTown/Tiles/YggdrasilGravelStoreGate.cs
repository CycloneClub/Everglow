using Everglow.Commons.TileHelper;
using Everglow.SubSpace;
using Everglow.SubSpace.Tiles;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilGravelStoreGate : RoomDoorTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		TileUtils.DefaultToMultiTileWall(5, 5);
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(86, 62, 44));
	}

	public void BuildPlayerRoomGen()
	{
		var mapIO = new MapIO(130, 130);

		mapIO.Read(ModIns.Mod.GetFileStream(ModAsset.GravelStore114x36_Path));

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
				ushort typeChange = (ushort)ModContent.TileType<GravelStoreCommandBlock>();
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

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		//var zero = new Vector2(Main.offScreenRange);
		//if (Main.drawToScreen)
		//{
		//	zero = Vector2.Zero;
		//}
		//var color = new Color(55, 55, 55, 0);
		//var tile = Main.tile[i, j];
		//int frameX = tile.TileFrameX;
		//int frameY = tile.TileFrameY;
		//Texture2D glow = ModAsset.YggdrasilGravelStoreGate.Value;
		//spriteBatch.Draw(glow, new Vector2(i, j) * 16 - Main.screenPosition + zero, new Rectangle(frameX, frameY, 16, 18), color, 0f, Vector2.zeroVector, 1f, SpriteEffects.None, 0f);
	}
}
