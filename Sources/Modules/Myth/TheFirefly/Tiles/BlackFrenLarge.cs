using Everglow.Commons.TileHelper;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class BlackFrenLarge : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			18,
		};
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		DustType = 191;
		AddMapEntry(new Color(11, 11, 11));
		HitSound = SoundID.Grass;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX % 54 == 0 && tile.TileFrameY % 36 == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		FurnitureUtils.MultiTileGrassFluentDraw(screenPosition, tileDrawing, spriteBatch, pos, ModAsset.BlackFrenGlow.Value);
	}
}
