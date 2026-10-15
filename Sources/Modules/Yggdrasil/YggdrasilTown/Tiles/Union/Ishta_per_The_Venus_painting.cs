using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class Ishta_per_The_Venus_painting : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new int[12];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;

		TileObjectData.addTile(Type);
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(96, 96, 96));
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		for (int x = 0; x < 6; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				int h = i + x - 5;
				int v = j + y - 7;
				spriteBatch.Draw(texture, new Vector2(h, v) * 16 - Main.screenPosition + zero, new Rectangle(x * 18, y * 18, 16, 16), Lighting.GetColor(h, v), 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
			}
		}
	}
}
