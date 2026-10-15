using Everglow.Commons.TileHelper;
using Everglow.SubSpace;
using Everglow.SubSpace.Tiles;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Kitchen.Tiles;

public class CanteenDoor : RoomDoorTile
{
	public void BuildCanteenGen()
	{
		var mapIO = new MapIO(12, 120);

		mapIO.Read(ModIns.Mod.GetFileStream(ModAsset.MapIOs_273x66KitchenRestaurant_Path));

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
				ushort typeChange = (ushort)ModContent.TileType<CanteenCommandBlock>();
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
		Point point = new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
		RoomManager.EnterNextLevelRoom(point, new Point(175, 150), BuildCanteenGen);
		return false;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		spriteBatch.Draw(texture, new Vector2(i, j) * 16 - Main.screenPosition + zero, new Rectangle(tile.TileFrameX + 90, tile.TileFrameY, 16, 16), new Color(0.4f, 0.4f, 0.4f, 0), 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
		base.PostDraw(i, j, spriteBatch);
	}
}
