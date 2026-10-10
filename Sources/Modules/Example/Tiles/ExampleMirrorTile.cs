using Everglow.Commons.Utilities;
using Everglow.Example.Items;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Example.Tiles;

public class ExampleMirrorTile : ModTile
{
	internal const int Width = 6;
	internal const int Height = 8;

	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Width = Width;
		TileObjectData.newTile.Height = Height;
		TileObjectData.newTile.Origin = new Point16(Width / 2, Height - 1);
		TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, Height).ToArray();
		TileObjectData.addTile(Type);

		DustType = DustID.Glass;
		RegisterItemDrop(ModContent.ItemType<ExampleMirrorItem>());
		AddMapEntry(new Color(160, 185, 205), Language.GetText(ExampleMirrorItem.TextKey + "Name"));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.dedServ)
		{
			return false;
		}

		// 保留旧版家具的内部名称供已有存档加载；新物品改为放置背景墙。
		Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition + offset;
		Tile tile = Main.tile[i, j];
		int x = tile.TileFrameX / 18;
		int y = tile.TileFrameY / 18;
		Rectangle bounds = new((int)position.X, (int)position.Y, 16, 16);
		Texture2D pixel = Commons.ModAsset.White.Value;
		spriteBatch.Draw(pixel, bounds, new Color(60, 80, 95));
		Color frameColor = new(175, 195, 210);
		if (x == 0)
		{
			spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, 3, 16), frameColor);
		}
		if (x == Width - 1)
		{
			spriteBatch.Draw(pixel, new Rectangle(bounds.Right - 3, bounds.Y, 3, 16), frameColor);
		}
		if (y == 0)
		{
			spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, 16, 3), frameColor);
		}
		if (y == Height - 1)
		{
			spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - 3, 16, 3), frameColor);
		}
		return false;
	}
}
