using Everglow.Commons.Templates.Pylon;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Pylons;

public class TownGatePylon : EverglowPylonBase<TownGatePylonTileEntity>
{
	public override void SetStaticDefaults()
	{
		AddMapEntry(new Color(60, 61, 118));
		MinPick = int.MaxValue;
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
		DrawModPylon(spriteBatch, i, j, CrystalTexture, CrystalHighlightTexture, new Vector2(0, DefaultVerticalOffset), Color.White * 0.1f, new Color(0.1f, 0.1f, 0.2f, 0.5f), CrystalVerticalFrameCount, true, PylonSparkDustType);
		Lighting.AddLight(i, j, 0.6f, 0.3f, 0.4f);
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
		color = Color.Lerp(color, Color.White, 0.2f);
		spriteBatch.Draw(CrystalTexture.Value, drawingPosition - Main.screenPosition, crystalFrame, color * 0.7f, 0f, origin, 1f, SpriteEffects.None, 0f);

		Texture2D glow = ModAsset.TownGatePylon_Crystal_glow.Value;
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

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		Texture2D tex = ModAsset.HolographicPylon_glow.Value;
		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(1f, 1f, 1f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
	}
}
