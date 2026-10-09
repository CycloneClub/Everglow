using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Dusts;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles.Furnitures;

public class GlowWoodSink : ModTile
{
	public override void SetStaticDefaults()
	{
		TileID.Sets.CountsAsWaterSource[Type] = true;

		Main.tileSolid[Type] = false;
		Main.tileLavaDeath[Type] = false;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
		TileObjectData.addTile(Type);

		DustType = ModContent.DustType<BlueGlow>();
		AdjTiles = new int[] { Type };

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
	}
	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}
	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D tex = ModAsset.GlowWoodSinkGlow.Value;
		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(0.8f, 0.8f, 0.8f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);

		base.PostDraw(i, j, spriteBatch);
	}
}
