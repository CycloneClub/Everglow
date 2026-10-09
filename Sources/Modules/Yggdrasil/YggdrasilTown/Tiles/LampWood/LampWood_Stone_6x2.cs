using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

public class LampWood_Stone_6x2 : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;

		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 6;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<TwilightStone_Dust>();
		AddMapEntry(new Color(39, 50, 52));
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return base.PreDraw(i, j, spriteBatch);
	}
	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		spriteBatch.Draw(ModAsset.LampWood_Stone_6x2_glow.Value, new Vector2(i, j) * 16 - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(1f, 1f, 1f, 0), 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
	}
}
