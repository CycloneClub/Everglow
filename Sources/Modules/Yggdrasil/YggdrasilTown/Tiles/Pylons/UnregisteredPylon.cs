using Everglow.Commons.Templates.Pylon;
using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Map;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Pylons;

public class UnregisteredPylon : EverglowPylonBase<UnregisteredPylonTileEntity>
{
	public override void SetStaticDefaults()
	{
		AddMapEntry(new Color(175, 0, 0));
		base.SetStaticDefaults();
	}

	public override bool CanPlacePylon() => true;

	public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) => true;

	public override bool ValidTeleportCheck_AnyDanger(TeleportPylonInfo pylonInfo) => true;

	public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) => true;

	public override void ValidTeleportCheck_DestinationPostCheck(TeleportPylonInfo destinationPylonInfo, ref bool destinationPylonValid, ref string errorKey) => base.ValidTeleportCheck_DestinationPostCheck(destinationPylonInfo, ref destinationPylonValid, ref errorKey);

	public override void ValidTeleportCheck_NearbyPostCheck(TeleportPylonInfo nearbyPylonInfo, ref bool destinationPylonValid, ref bool anyNearbyValidPylon, ref string errorKey) => base.ValidTeleportCheck_NearbyPostCheck(nearbyPylonInfo, ref destinationPylonValid, ref anyNearbyValidPylon, ref errorKey);

	public override void ModifyTeleportationPosition(TeleportPylonInfo destinationPylonInfo, ref Vector2 teleportationPosition) => base.ModifyTeleportationPosition(destinationPylonInfo, ref teleportationPosition);

	public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
	{
		DrawModPylon(spriteBatch, i, j, CrystalTexture, CrystalHighlightTexture, new Vector2(0, DefaultVerticalOffset), new Color(0.2f, 0.0f, 0.0f, 0), new Color(0.9f, 0f, 0f, 0f), CrystalVerticalFrameCount, true, PylonSparkDustType);
		Lighting.AddLight(i, j, 1f, 0f, 0f);
	}

	public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
	{
	}

	public override void DrawModPylon(SpriteBatch spriteBatch, int i, int j, Asset<Texture2D> CrystalTexture, Asset<Texture2D> CrystalHighlightTexture, Vector2 crystalOffset, Color pylonShadowColor, Color dustColor, int crystalVerticalFrameCount, bool animation = true, int dustType = 43)
	{
		// Gets offscreen vector for different lighting modes
		var offscreenVector = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			offscreenVector = Vector2.Zero;
		}

		// Double check that the tile exists
		var point = new Point(i, j);
		Tile tile = Main.tile[point.X, point.Y];
		if (tile == null || !tile.HasTile)
		{
			return;
		}

		var tileData = TileObjectData.GetTileData(tile);

		// Calculate frame based on vanilla counters in order to line up the animation
		int frameY = animation ? Main.tileFrameCounter[TileID.TeleportationPylon] / crystalVerticalFrameCount : 0;

		// Frame our modded crystal sheet accordingly for proper drawing
		Rectangle crystalFrame = CrystalTexture.Frame(1, crystalVerticalFrameCount, 0, frameY);
		Rectangle smartCursorGlowFrame = CrystalHighlightTexture.Frame(1, crystalVerticalFrameCount, 0, frameY);

		// I have no idea what is happening here; but it fixes the frame bleed issue. All I know is that the vertical sinusoidal motion has something to with it.
		// If anyone else has a clue as to why, please do tell. - MutantWafflez
		crystalFrame.Height -= 1;
		smartCursorGlowFrame.Height -= 1;

		// Calculate positional variables for actually drawing the crystal
		Vector2 origin = crystalFrame.Size() / 2f;
		var tileOrigin = new Vector2(tileData.CoordinateFullWidth / 2f, tileData.CoordinateFullHeight / 2f);
		Vector2 crystalPosition = point.ToWorldCoordinates(tileOrigin.X - 2f, tileOrigin.Y) + crystalOffset;

		// Calculate additional drawing positions with a sine wave movement
		float sinusoidalOffset = animation ? (float)Math.Sin(Main.GlobalTimeWrappedHourly * (Math.PI * 2) / 5) : 0;
		Vector2 drawingPosition = crystalPosition + offscreenVector + new Vector2(0f, sinusoidalOffset * 4f);

		// Do dust drawing
		Rectangle dustBox = Utils.CenteredRectangle(crystalPosition, crystalFrame.Size());
		GenerateDust(dustBox, dustType, dustColor, 0.1f);

		// Get color value and draw the the crystal
		Color color = Lighting.GetColor(point.X, point.Y);
		color = Color.Lerp(color, Color.White, 0.8f);
		spriteBatch.Draw(CrystalTexture.Value, drawingPosition - Main.screenPosition, crystalFrame, color * 0.7f, 0f, origin, 1f, SpriteEffects.None, 0f);

		Texture2D glow = ModAsset.UnregisteredPylon_Crystal_glow.Value;
		spriteBatch.Draw(glow, drawingPosition - Main.screenPosition, crystalFrame, new Color(1f, 1f, 1f, 0), 0f, origin, 1f, SpriteEffects.None, 0f);

		// Draw the shadow effect for the crystal
		float scale = animation ? (float)Math.Sin(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f) / 1f) * 0.2f + 0.8f : 0.8f;
		Color shadowColor = pylonShadowColor * scale;
		for (float shadowPos = 0f; shadowPos < 1f; shadowPos += 1f / 6f)
		{
			spriteBatch.Draw(CrystalTexture.Value, drawingPosition - Main.screenPosition + ((float)Math.PI * 2f * shadowPos).ToRotationVector2() * (6f + sinusoidalOffset * 2f), crystalFrame, shadowColor, 0f, origin, 1f, SpriteEffects.None, 0f);
		}

		// Interpret smart cursor outline color & draw it
		int selectionLevel = 0;
		if (Main.InSmartCursorHighlightArea(point.X, point.Y, out bool actuallySelected))
		{
			selectionLevel = 1;
			if (actuallySelected)
			{
				selectionLevel = 2;
			}
		}

		if (selectionLevel == 0)
		{
			return;
		}

		int averageBrightness = (color.R + color.G + color.B) / 3;

		if (averageBrightness <= 10)
		{
			return;
		}

		Color selectionGlowColor = Colors.GetSelectionGlowColor(selectionLevel == 2, averageBrightness);
		spriteBatch.Draw(CrystalHighlightTexture.Value, drawingPosition - Main.screenPosition, smartCursorGlowFrame, selectionGlowColor, 0f, origin, 1f, SpriteEffects.None, 0f);
	}

	public override void GenerateDust(Rectangle dustBox, int dustType, Color dustColor, float chance)
	{
		if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)) && Main.rand.NextFloat() < chance)
		{
			int numForDust = Dust.NewDust(dustBox.TopLeft(), dustBox.Width, dustBox.Height, dustType, 0f, 0f, 254, dustColor, 0.5f);
			Dust obj = Main.dust[numForDust];
			obj.velocity *= 0.1f;
			Main.dust[numForDust].velocity.Y -= 0.2f;
		}
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		Texture2D tex = ModAsset.UnregisteredPylon_glow.Value;
		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(1f, 1f, 1f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
	}

	public override bool RightClick(int i, int j)
	{
		var tile = Main.tile[i, j];
		var topLeftTile = new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
		for (int x = 0; x < 3; x++)
		{
			for (int y = 0; y < 4; y++)
			{
				var checkPoint = new Point(topLeftTile.X + x, topLeftTile.Y + y);
				var checkTile = WorldGenMisc.SafeGetTile(checkPoint);
				if (checkTile.TileType == Type)
				{
					checkTile.TileType = (ushort)ModContent.TileType<HolographicPylon>();
					checkTile.TileFrameX = (short)(x * 18);
					checkTile.TileFrameY = (short)(y * 18);
				}
				if (x == 0 && y == 0)
				{
					TileEntity tileEntityOld;
					TileEntity.ByPosition.TryGetValue(new Point16(checkPoint.X, checkPoint.Y), out tileEntityOld);
					if (tileEntityOld is not null)
					{
						ModContent.GetInstance<UnregisteredPylonTileEntity>().Kill(checkPoint.X, checkPoint.Y);
						TileEntity.PlaceEntityNet(checkPoint.X, checkPoint.Y, ModContent.TileEntityType<HolographicPylonTileEntity>());
					}
				}
				if (x == 1 && y == 3)
				{
					UnlockVFX(checkPoint.X, checkPoint.Y);
				}
			}
		}
		return false;
	}

	public void UnlockVFX(int x, int y)
	{
		Vector2 worldPos = new Point(x, y).ToWorldCoordinates();
		for (int i = 0; i < 6 + 10; i++)
		{
			var vel = new Vector2(0, -Main.rand.NextFloat(3.6f, 6.4f));
			Vector2 pos = new Vector2(0, Main.rand.NextFloat(0f, 22.4f)).RotatedByRandom(MathHelper.TwoPi);
			pos.Y *= 0.1f;
			var dust = new AvariceSuccessDust
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = worldPos + pos,
				maxTime = Main.rand.Next(60, 120),
				scale = Main.rand.NextFloat(1f, 2f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
		for (int i = 0; i < 12; i++)
		{
			var vel = new Vector2(0, -Main.rand.NextFloat(1.6f, 6.4f));
			Vector2 pos = new Vector2(0, Main.rand.NextFloat(0f, 50.4f)).RotatedByRandom(MathHelper.TwoPi);
			pos.Y *= 0.1f;
			var cube = new AvariceSuccessCube
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = worldPos + pos,
				maxTime = Main.rand.Next(60, 120),
				scale = Main.rand.NextFloat(10f, 20f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 20.93f), 0.06f },
			};
			Ins.VFXManager.Add(cube);
		}
	}
}
