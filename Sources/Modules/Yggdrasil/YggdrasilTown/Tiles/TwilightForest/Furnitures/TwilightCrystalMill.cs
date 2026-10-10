using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Furnitures;

public class TwilightCrystalMill : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileTable[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
		DustType = ModContent.DustType<TwilightCrystalDust>();

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
		TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(40, 80, 148));
		TileID.Sets.RoomNeeds.CountsAsTable[Type] = true;
	}

	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		if (++frameCounter >= 4)
		{
			frameCounter = 0;
			frame = ++frame % 11;
		}
	}

	public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY < 72)
		{
			frameYOffset = Main.tileFrame[type] * 72;
		}
		else
		{
			frameYOffset = 720;
		}
	}

	public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
	{
		if (Main.gamePaused || !Main.instance.IsActive)
		{
			return;
		}
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		Tile tile = Main.tile[i, j];
		int addFrX = 0;
		int addFrY = 0;
		TileLoader.SetAnimationFrame(Type, i, j, ref addFrX, ref addFrY); // calculates the animation offsets
		if (tile.TileFrameY + addFrY >= 756)
		{
			r = 0.3f;
			g = 0.6f;
			b = 1.1f;
		}
		if (tile.TileFrameY + addFrY >= 36 && tile.TileFrameY + addFrY <= 54)
		{
			r = 0f;
			g = 0.2f;
			b = 0.3f;
		}
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		Color color = new Color(255, 255, 255, 0);

		Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		int width = 16;
		int offsetY = 0;
		int height = 16;
		short frameX = tile.TileFrameX;
		short frameY = tile.TileFrameY;
		int addFrX = 0;
		int addFrY = 0;

		TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY); // calculates the draw offsets
		TileLoader.SetAnimationFrame(Type, i, j, ref addFrX, ref addFrY); // calculates the animation offsets

		Rectangle drawRectangle = new Rectangle(tile.TileFrameX, tile.TileFrameY + addFrY, 16, 16);

		// The flame is manually drawn separate from the tile texture so that it can be drawn at full brightness.
		Main.spriteBatch.Draw(ModAsset.TwilightCrystalMill_glow.Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + offsetY) + zero, drawRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
	}
}
