using Everglow.Commons.TileHelper;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class LampLotus : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateWidth = 28;
		TileObjectData.addTile(Type);
		DustType = DustID.SpookyWood;

		AddMapEntry(new Color(81, 110, 255));
		HitSound = SoundID.Grass;
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		var tile = Main.tile[i, j];
		var tile2 = Main.tile[i, j - 1];

		if (tile2.TileType != tile.TileType && !tile2.HasTile)
		{
			int length = 0;
			while (Main.tile[i, j + length].TileType == tile.TileType)
			{
				length++;
			}
			if (length <= 4)
			{
				tile2.TileType = (ushort)ModContent.TileType<LampLotus>();
				tile2.HasTile = true;
				tile2.TileFrameX = (short)(Main.rand.Next(8) * 18);
			}
		}
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 0.0f;
		g = 0.6f;
		b = 1.3f;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (Main.tile[i, j + 1].TileType != tile.TileType)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawLotusPiece(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of lotus
	/// </summary>
	private void DrawLotusPiece(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		Vector2 lastOffset = new Vector2(0, 8);
		for (int j = 0; j < 30; j++)
		{
			if (tilePos.Y - j < 21)
			{
				return;
			}
			var tile = Main.tile[tilePos + new Point(0, -j)];
			if (!(tile.TileType == Type && tile.HasTile))
			{
				return;
			}
			var tileUp = Main.tile[tilePos + new Point(0, -j - 1)];
			bool lastTile = false;
			if (!(tileUp.TileType == Type && tileUp.HasTile))
			{
				lastTile = true;
			}
			ushort type = tile.TileType;
			var frame = new Rectangle(5, 62, 18, 18);
			if (lastTile)
			{
				frame = new Rectangle(28 * tile.TileFrameX / 18, 42, 28, 28);
			}

			// 回声涂料
			if (!TileDrawing.IsVisible(tile))
			{
				continue;
			}

			int paint = Main.tile[tilePos].TileColor;
			Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.LampLotus_Path, type, 1, paint, tileDrawing);
			tex ??= ModAsset.LampLotus.Value;

			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
			}

			int totalPushTime = 140;
			float pushForcePerFrame = 0.96f;
			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y - j, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			float rotation = windCycle * 0.21f;

			var tileLight = Lighting.GetColor(tilePos);

			// 支持发光涂料
			tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y - j, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
			tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y - j, tile, type, 0, 0, tileLight);

			var origin = new Vector2(9, 18);
			if (lastTile)
			{
				origin = new Vector2(14, 28);
			}

			var tileSpriteEffect = SpriteEffects.None;
			spriteBatch.Draw(tex, drawCenterPos + lastOffset, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
			if (lastTile)
			{
				frame.Y -= 37;
				spriteBatch.Draw(tex, drawCenterPos + lastOffset, frame, new Color(1f, 1f, 1f, 0), rotation, origin, 1f, tileSpriteEffect, 0f);
			}
			lastOffset += new Vector2(0, -16).RotatedBy(rotation);
		}
	}
}
