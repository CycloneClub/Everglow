using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class FurnaceCopperGear_Large : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Origin = new(0, 0);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, };

		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;

		TileObjectData.addTile(Type);
		DustType = DustID.Copper;
		AddMapEntry(new Color(114, 81, 64));
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		resetFrame = false;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		if (tile.Slope != SlopeType.Solid || tile.halfBrick())
		{
			return true;
		}
		var texture = ModAsset.FurnaceCopperGear_Large_Gear.Value;
		var offsetScreen = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			offsetScreen = Vector2.Zero;
		}
		Vector2 drawPos = new Point(i, j).ToWorldCoordinates() - Main.screenPosition + offsetScreen;
		int style = tile.TileFrameX / 18;
		var frame = new Rectangle(style * 48, 0, 48, 48);
		float timeValue = (float)Main.time / 100f + MathF.Sin(i + j) * 6;
		int rotDir = (int)(((i + j) % 2 - 0.5f) * 2);

		spriteBatch.Draw(texture, drawPos, frame, Lighting.GetColor(i, j), timeValue * rotDir, frame.Size() * 0.5f, 1, SpriteEffects.None, 0);
		return false;
	}
}
