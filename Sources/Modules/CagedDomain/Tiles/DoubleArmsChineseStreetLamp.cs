using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using MonoMod.Utils.Interop;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class DoubleArmsChineseStreetLamp : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLighted[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			18,
		};
		TileObjectData.newTile.CoordinateWidth = 18;
		TileObjectData.newTile.Origin = new Point16(0, 4);
		TileObjectData.addTile(Type);
		DustType = DustID.DynastyWood;
		AddMapEntry(new Color(135, 103, 90));
	}
	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 3, 5);
	}
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX < 54 && tile.TileFrameY <= 18)
		{
			r = 1.15f;
			g = 0.55f;
			b = 0.0f;
		}
		else
		{
			r = 0f;
			g = 0f;
			b = 0f;
		}
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX % 54 == 0 && tile.TileFrameY == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 0) - screenPosition;

		DrawLanternPiece(new Rectangle(0, 0, 54, 78), 0, new Vector2(16, 82), pos, pos + new Point(0, 4), drawCenterPos, new Vector2(27, 78), spriteBatch, tileDrawing);

		float firstSway = 0.12f;
		float secondSway = 0.24f;
		float thirdSway = 0.48f;
		int addRecX = 0;
		if (tile.TileFrameX >= 54)
		{
			addRecX = 18;
		}

		DrawLanternPiece(new Rectangle(2 + addRecX, 80, 14, 10), firstSway, new Vector2(0, 18), pos + new Point(0, 1), pos + new Point(0, 1), drawCenterPos, new Vector2(7, 0), spriteBatch, tileDrawing);
		Vector2 firstOffsetLeft = new Vector2(0, 10).RotatedBy(GetWindRot(pos + new Point(0, 1), 1, 1, firstSway, tileDrawing));
		DrawLanternPiece(new Rectangle(4 + addRecX, 90, 10, 10), secondSway, new Vector2(0, 18) + firstOffsetLeft, pos + new Point(0, 2), pos + new Point(0, 2), drawCenterPos, new Vector2(5, 0), spriteBatch, tileDrawing);
		DrawLanternPiece(new Rectangle(6 + addRecX, 100, 6, 8), thirdSway, new Vector2(0, 18) + firstOffsetLeft + new Vector2(0, 10).RotatedBy(GetWindRot(pos + new Point(0, 2), 1, 1, secondSway, tileDrawing)), pos + new Point(0, 3), pos + new Point(0, 3), drawCenterPos, new Vector2(3, 0), spriteBatch, tileDrawing);

		DrawLanternPiece(new Rectangle(2 + addRecX, 80, 14, 10), firstSway, new Vector2(32, 18), pos + new Point(2, 1), pos + new Point(2, 1), drawCenterPos, new Vector2(7, 0), spriteBatch, tileDrawing);
		Vector2 firstOffsetRight = new Vector2(0, 10).RotatedBy(GetWindRot(pos + new Point(2, 1), 1, 1, firstSway, tileDrawing));
		DrawLanternPiece(new Rectangle(4 + addRecX, 90, 10, 10), secondSway, new Vector2(32, 18) + firstOffsetRight, pos + new Point(2, 2), pos + new Point(2, 2), drawCenterPos, new Vector2(5, 0), spriteBatch, tileDrawing);
		DrawLanternPiece(new Rectangle(6 + addRecX, 100, 6, 8), thirdSway, new Vector2(32, 18) + firstOffsetRight + new Vector2(0, 10).RotatedBy(GetWindRot(pos + new Point(2, 2), 1, 1, secondSway, tileDrawing)), pos + new Point(2, 3), pos + new Point(2, 3), drawCenterPos, new Vector2(3, 0), spriteBatch, tileDrawing);

		if (tile.TileFrameX < 54)
		{
			addRecX = 36;
			DrawLanternPiece(new Rectangle(2 + addRecX, 80, 14, 10), firstSway, new Vector2(0, 18), pos + new Point(0, 1), pos + new Point(0, 1), drawCenterPos, new Vector2(7, 0), spriteBatch, tileDrawing, new Color(1f, 0.5f, 0f, 0));
			DrawLanternPiece(new Rectangle(4 + addRecX, 90, 10, 10), secondSway, new Vector2(0, 18) + firstOffsetLeft, pos + new Point(0, 2), pos + new Point(0, 1), drawCenterPos, new Vector2(5, 0), spriteBatch, tileDrawing, new Color(1f, 0.5f, 0f, 0));

			DrawLanternPiece(new Rectangle(2 + addRecX, 80, 14, 10), firstSway, new Vector2(32, 18), pos + new Point(2, 1), pos + new Point(0, 1), drawCenterPos, new Vector2(7, 0), spriteBatch, tileDrawing, new Color(1f, 0.5f, 0f, 0));
			DrawLanternPiece(new Rectangle(4 + addRecX, 90, 10, 10), secondSway, new Vector2(32, 18) + firstOffsetRight, pos + new Point(2, 2), pos + new Point(0, 1), drawCenterPos, new Vector2(5, 0), spriteBatch, tileDrawing, new Color(1f, 0.5f, 0f, 0));
		}
	}
	public float GetWindRot(int x, int y, int width, int height, float swayCoefficient, TileDrawing tileDrawing)
	{
		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(x, y, width, height))
		{
			windCycle = tileDrawing.GetWindCycle(x, y, tileDrawing._sunflowerWindCounter);
		}

		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(x, y, width, height, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		return -windCycle * swayCoefficient;
	}
	public float GetWindRot(Point pos, int width, int height, float swayCoefficient, TileDrawing tileDrawing)
	{
		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(pos.X, pos.Y, width, height))
		{
			windCycle = tileDrawing.GetWindCycle(pos.X, pos.Y, tileDrawing._sunflowerWindCounter);
		}

		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(pos.X, pos.Y, width, height, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		return -windCycle * swayCoefficient;
	}
	/// <summary>
	/// 画侧挂灯
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
	private void DrawLanternPiece(Rectangle frame, float swayCoefficient, Vector2 offset, Point tilePos, Point paintPos, Vector2 drawCenterPos, Vector2 origin, SpriteBatch spriteBatch, TileDrawing tileDrawing, Color color = new Color())
	{
		// 回声涂料	
		if (!TileDrawing.IsVisible(Main.tile[paintPos]))
		{
			return;
		}

		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		int paint = Main.tile[paintPos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.DoubleArmsChineseStreetLamp_Draw_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.DoubleArmsChineseStreetLamp_Draw.Value;

		int sizeX = 1;
		int sizeY = 1;

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
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + offset, frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
	}
}
