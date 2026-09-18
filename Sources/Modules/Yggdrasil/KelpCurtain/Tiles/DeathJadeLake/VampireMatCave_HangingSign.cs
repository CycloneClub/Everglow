using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class VampireMatCave_HangingSign : ShapeDataTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		TotalWidth = 5;
		TotalHeight = 14;
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Height = 14;
		TileObjectData.newTile.Width = 5;
		TileObjectData.newTile.CoordinateHeights = new int[14];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 2);
		TileObjectData.newTile.Origin = new Point16(2, 0);
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<VampireMatCave_HangingSign_Dust>();
		AddMapEntry(new Color(61, 34, 48));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.tile[i, j - 1].TileType != Type && Main.tile[i, j].TileFrameY == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawSign(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	private void DrawSign(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var lastOffset = new Vector2(0, -8);
		for (int j = 0; j < 6; j++)
		{
			var offsetPos = tilePos + new Point(0, j);
			var tile = Main.tile[offsetPos];
			ushort type = tile.TileType;

			// 回声涂料
			if (!TileDrawing.IsVisible(tile))
			{
				continue;
			}

			int paint = Main.tile[offsetPos].TileColor;
			Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.VampireMatCave_HangingSign_Path, type, 1, paint, tileDrawing);
			tex ??= ModAsset.VampireMatCave_HangingSign.Value;

			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind(offsetPos.X, offsetPos.Y, 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle(offsetPos.X, offsetPos.Y, tileDrawing._sunflowerWindCounter);
			}

			int totalPushTime = 140;
			float pushForcePerFrame = 0.96f;
			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(offsetPos.X, offsetPos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			if (j == 5)
			{
				highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(offsetPos.X - 2, offsetPos.Y, 5, 9, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			}
			windCycle -= highestWindGridPushComplex;
			float rotation = windCycle * 0.21f;
			if (j < 5)
			{
				rotation -= lastOffset.X / (22f + j * 3f);
			}
			else
			{
				rotation += lastOffset.X / (22f + j * 3f) * 0.25f;
			}
			var tileLight = Lighting.GetColor(offsetPos);

			// 支持发光涂料
			tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(offsetPos.X, offsetPos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
			tileLight = tileDrawing.DrawTiles_GetLightOverride(offsetPos.X, offsetPos.Y, tile, type, 0, 0, tileLight);

			var origin = new Vector2(8, 0);
			var frame = Rectangle.emptyRectangle;
			switch (j)
			{
				case 0:
					frame = new Rectangle(124, 0, 16, 16);
					break;
				case 1:
					frame = new Rectangle(124, 18, 16, 16);
					break;
				case 2:
					frame = new Rectangle(124, 36, 16, 20);
					break;
				case 3:
					frame = new Rectangle(124, 58, 16, 20);
					break;
				case 4:
					frame = new Rectangle(124, 80, 16, 14);
					break;
				case 5:
					frame = new Rectangle(94, 96, 80, 160);
					origin = new Vector2(40, 0);
					break;
			}
			var drawPos = drawCenterPos + lastOffset;
			var tileSpriteEffect = SpriteEffects.None;
			spriteBatch.Draw(tex, drawPos, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
			if (j == 5)
			{
				frame.X += 80;
				spriteBatch.Draw(tex, drawPos, frame, tileLight * 3, rotation, origin, 1f, tileSpriteEffect, 0f);
			}
			lastOffset += new Vector2(0, frame.Height - 4).RotatedBy(rotation);
		}
	}
}
