using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class Union_Y_PlatformRed : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileTable[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.Platforms[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		DustType = ModContent.DustType<Union_Y_PlatformRed_Dust>();
		MinPick = 300;
		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

		DustType = DustID.DynastyWood;
		AdjTiles = new int[] { TileID.Platforms };

		// Placement
		TileObjectData.newTile.CoordinateHeights = new[] { 16 };
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.StyleMultiplier = 27;
		TileObjectData.newTile.StyleWrapLimit = 27;
		TileObjectData.newTile.UsesCustomCanPlace = false;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(204, 38, 52));
	}

	public override bool CanExplode(int i, int j) => false;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		resetFrame = false;
		return false;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];

		var texture = ModAsset.Union_Y_Stairs.Value;
		var offsetScreen = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			offsetScreen = Vector2.Zero;
		}
		int tfX = tile.TileFrameX - 900;
		int tfY = tile.TileFrameY - 900;
		if (!Ins.VisualQuality.High)
		{
			Vector2 drawPos = new Point(i, j).ToWorldCoordinates() - Main.screenPosition + offsetScreen;
			Rectangle frame = new Rectangle(tfX, tfY, 16, 16);
			spriteBatch.Draw(texture, drawPos, frame, Lighting.GetColor(i, j), 0, frame.Size() * 0.5f, 1, SpriteEffects.None, 0);
		}
		else
		{
			Vector2 drawPos = new Point(i, j).ToWorldCoordinates() - Main.screenPosition + offsetScreen;
			Color lightColor = Lighting.GetColor(i, j);
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					Rectangle frame = new Rectangle(tfX + x * 6, tfY + y * 6, 6, 6);
					Vector2 offset = new Vector2(x, y) * 6;
					if (x == 2)
					{
						frame.Width = 4;
					}
					if (y == 2 && tfY != 19 * 18)
					{
						frame.Height = 4;
					}

					Color offsetColor = Lighting.GetColor(i + x - 1, j + y - 1);
					Color drawColor = Color.Lerp(offsetColor, lightColor, 0.5f);
					spriteBatch.Draw(texture, drawPos + offset - new Vector2(8), frame, drawColor, 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
				}
			}
		}
		return false;
	}
}
