using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class StreetLantern : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLighted[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 6;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			18,
		};
		TileObjectData.newTile.CoordinateWidth = 48;
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.Origin = new Point16(0, 5);
		TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
		// The following 3 lines are needed if you decide to add more styles and stack them vertically
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = false;

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); // Facing right will use the second texture style
		TileObjectData.newTile.Origin = new Point16(0, 5);
		TileObjectData.addTile(Type);

		DustType = DustID.DynastyWood;
		AddMapEntry(new Color(151, 31, 32));
	}
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX < 48 && tile.TileFrameY % 108 > 0 && tile.TileFrameY % 108 < 54)
		{
			r = 1.45f;
			g = 0.15f;
			b = 0.0f;
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
		FurnitureUtils.LightHitwire(i, j, Type, 1, 6, 48);
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX % 48 == 0 && tile.TileFrameY % 108 == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 0) - screenPosition;
		int recX = 0;
		int recY = 0;
		int offX = -4;
		int offXofLantern = 0;
		int frameYAdd = 0;
		if (tile.TileFrameY >= 108)
		{
			offXofLantern = 10;
			recY = 132;
			frameYAdd = 30;
		}
		if (tile.TileFrameX >= 48)
		{
			recX = 48;
			DrawLanternPiece(new Rectangle(108, 62 + frameYAdd, 32, 28), 0.06f, offX + offXofLantern, 8, pos + new Point(0, 1), pos + new Point(0, 1), drawCenterPos, spriteBatch, tileDrawing);
		}
		else
		{

			DrawLanternPiece(new Rectangle(108, 2 + frameYAdd, 32, 28), 0.06f, offX + offXofLantern, 8, pos + new Point(0, 1), pos + new Point(0, 1), drawCenterPos, spriteBatch, tileDrawing);
			DrawLanternPiece(new Rectangle(108, 122 + frameYAdd, 32, 28), 0.06f, offX + offXofLantern, 8, pos + new Point(0, 1), pos + new Point(0, 1), drawCenterPos, spriteBatch, tileDrawing);
			DrawLanternPiece(new Rectangle(108, 176 + frameYAdd, 32, 28), 0.06f, offX + offXofLantern, 10, pos + new Point(0, 1), pos + new Point(0, 1), drawCenterPos, spriteBatch, tileDrawing, new Color(1f, 1f, 1f, 0));
		}


		DrawLanternPiece(new Rectangle(recX, recY, 48, 108), 0, offX - 4, -10, pos + new Point(0, 0), pos + new Point(0, 0), drawCenterPos, spriteBatch, tileDrawing);
	}
	/// <summary>
	/// 画灯笼柱
	/// </summary>
	/// <param name="frame"></param>
	/// <param name="swayCoefficient"></param>
	/// <param name="offsetX"></param>
	/// <param name="offsetY"></param>
	/// <param name="tilePos"></param>
	/// <param name="paintPos"></param>
	/// <param name="drawCenterPos"></param>
	/// <param name="spriteBatch"></param>
	/// <param name="tileDrawing"></param>
	/// <param name="color"></param>
	private void DrawLanternPiece(Rectangle frame, float swayCoefficient, int offsetX, int offsetY, Point tilePos, Point paintPos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Color color = new Color())
	{
		// 回声涂料	
		if (!TileDrawing.IsVisible(Main.tile[paintPos]))
		{
			return;
		}

		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		int paint = Main.tile[paintPos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.StreetLantern_Draw_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.StreetLantern_Draw.Value;

		int sizeX = 2;
		int sizeY = 2;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, sizeX, sizeY))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
		}

		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, sizeX, sizeY, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;

		// 支持发光涂料
		Color tileLight;
		if (color != new Color())
		{
			tileLight = color;
		}
		else
		{
			tileLight = Lighting.GetColor(tilePos);
		}
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.Y, tilePos.X, tile, type, 0, 0, tileLight);

		float rotation = -windCycle * swayCoefficient;
		var origin = new Vector2(16, 0);
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(offsetX, offsetY), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
	}
}
