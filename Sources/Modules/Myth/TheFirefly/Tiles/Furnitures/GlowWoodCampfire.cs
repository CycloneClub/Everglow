using Everglow.Myth.Common;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles.Furnitures;

public class GlowWoodCampfire : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.IsValidSpawnPoint[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

		DustType = ModContent.DustType<Dusts.BlueToPurpleSpark>();
		AdjTiles = new int[] { TileID.Campfire };

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2); // this style already takes care of direction for us
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
		TileObjectData.newTile.DrawYOffset = 3;
		TileObjectData.addTile(Type);

		AnimationFrameHeight = 36;

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		frameCounter++;
		if (frameCounter >= 6)
		{
			frame = (frame + 1) % 8;
			frameCounter = 0;
		}
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX < 54)
		{
			type = ModContent.DustType<Dusts.BlueToPurpleSpark>();
			return true;
		}
		else
		{
			return false;
		}
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Tile tile = Main.tile[i, j];
		Player player = Main.LocalPlayer;
		if (player != null && !player.dead && player.active && tile.TileFrameX < 54)
		{
			Main.SceneMetrics.HasCampfire = true;
		}

		if (tile.TileFrameX < 54 && tile.TileFrameY == 18)
		{
			int frequency = 24;
			if (tile.TileFrameX == 18)
			{
				frequency = 8;
			}

			if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)) && Main.rand.NextBool(frequency))
			{
				Rectangle dustBox = Utils.CenteredRectangle(new Vector2(i * 16, j * 16), new Vector2(16, 16));
				int numForDust = Dust.NewDust(dustBox.TopLeft(), dustBox.Width, dustBox.Height, ModContent.DustType<Dusts.BlueToPurpleSpark>(), 0f, 0f, 254, default, Main.rand.NextFloat(0.95f, 1.75f));
				Dust obj = Main.dust[numForDust];
				obj.velocity *= 0.4f;
				Main.dust[numForDust].velocity.Y -= 0.8f;
			}
		}
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX < 54)
		{
			if (tile.TileFrameY == 18)
			{
				r = 0.1f;
				g = 0.1f;
				b = 1.1f;
			}
			if (tile.TileFrameY == 0 && tile.TileFrameX == 18)
			{
				r = 0.6f;
				g = 0f;
				b = 0.3f;
			}
		}
		else
		{
			r = 0f;
			g = 0f;
			b = 0f;
		}
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 3, 2);
	}

	public override bool RightClick(int i, int j)
	{
		int tileX = 3;
		int tileY = 2;
		Tile tile = Main.tile[i, j];
		int x = i - tile.TileFrameX / 18 % tileX;
		int y = j - tile.TileFrameY / 18 % tileY;
		for (int m = x; m < x + tileX; m++)
		{
			for (int n = y; n < y + tileY; n++)
			{
				if (!tile.HasTile)
				{
					continue;
				}

				if (tile.TileType == Type)
				{
					tile = Main.tile[m, n];
					if (tile.TileFrameX < 18 * tileX)
					{
						tile = Main.tile[m, n];
						tile.TileFrameX += (short)(18 * tileX);
					}
					else
					{
						tile = Main.tile[m, n];
						tile.TileFrameX -= (short)(18 * tileX);
					}
				}
			}
		}
		return true;
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
	{
		return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<Items.Furnitures.GlowWoodCampfire>();
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D tex = ModAsset.GlowWoodCampfireGlow.Value;
		int frameYOffset = Main.tileFrame[Type] * AnimationFrameHeight;
		if (tile.TileFrameX < 54)
		{
			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 + 3) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, 16), new Color(0.8f, 0.8f, 0.8f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
		}

		if (tile.TileFrameX == 18 && tile.TileFrameY % 36 == 0)
		{
			Color cTile = Lighting.GetColor(i, j - 1);
			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 - 16 + 3) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX + 54, tile.TileFrameY + frameYOffset, 16, 16), cTile, 0, new Vector2(0), 1, SpriteEffects.None, 0);
			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16 - 16 + 3) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX + 54, tile.TileFrameY + frameYOffset, 16, 16), new Color(0.8f, 0.8f, 0.8f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
		}
	}
}
