using Everglow.Myth.TheTusk.Items;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheTusk.Tiles;

public class StrangeTuskStone1 : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileLavaDeath[Type] = false;
		Main.tileFrameImportant[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 7;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			16,
			20,
		};
		TileObjectData.newTile.CoordinateWidth = 64;
		TileObjectData.addTile(Type);
		DustType = 1;
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(100, 90, 90), modTranslation);
		HitSound = SoundID.DD2_SkeletonHurt;
	}
	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}
	public override void NearbyEffects(int i, int j, bool closer)
	{
	}
	public override bool CanExplode(int i, int j)
	{
		return false;
	}
	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
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

		Player player = Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 0, 0)];
		int C = 255 - (int)(player.Center - new Vector2(i * 16, j * 16)).Length();
		if (C < 0)
		{
			C = 0;
		}

		spriteBatch.Draw(ModContent.Request<Texture2D>("Everglow/Myth/TheTusk/Tiles/StrangeTuskStoneGlow").Value, new Vector2(i * 16 - 24 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(0, tile.TileFrameY, 64, 18), new Color(C, C, C, 0), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
	}
}
