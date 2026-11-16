using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class SteamBoiler : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLighted[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.Width = 6;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			16,
		};
		TileObjectData.newTile.Origin = new Point16(2, 4);
		TileObjectData.addTile(Type);
		DustType = DustID.Copper;
		AddMapEntry(new Color(135, 105, 86));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX == 36 && tile.TileFrameY == 36)
		{
			r = 0.93f;
			g = 0.23f;
			b = 0.01f;
		}
		else
		{
			r = 0f;
			g = 0f;
			b = 0f;
		}
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		var offset = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			offset = Vector2.zeroVector;
		}
		var drawPos = new Point(i, j).ToWorldCoordinates() + offset - Main.screenPosition;
		Texture2D tex = ModAsset.SteamBoiler_Glow.Value;
		spriteBatch.Draw(tex, drawPos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(1f, 1f, 1f, 0), 0, new Vector2(8), 1f, SpriteEffects.None, 0);
		base.PostDraw(i, j, spriteBatch);
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 6, 5);
	}
}
