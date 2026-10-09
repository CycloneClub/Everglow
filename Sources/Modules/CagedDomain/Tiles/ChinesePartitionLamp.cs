using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class ChinesePartitionLamp : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
		Main.tileLighted[Type] = true;
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.Origin = new Point16(0, 3);
		// The following 3 lines are needed if you decide to add more styles and stack them vertically
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = false;

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); // Facing right will use the second texture style
		TileObjectData.newTile.Origin = new Point16(0, 3);
		TileObjectData.addTile(Type);

		DustType = DustID.DynastyWood;
		AddMapEntry(new Color(135, 103, 90));
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 3, 4);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX < 54)
		{
			r = 0.8f;
			g = 0.6f;
			b = 0.4f;
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
		if (tile.TileFrameX % 54 == 18 && tile.TileFrameY % 72 == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 0) - screenPosition;
		int frameYAdd = 0;
		int offXAdd = 0;
		if (tile.TileFrameY == 72)
		{
			frameYAdd = 72;
			offXAdd = 14;
		}

		DrawLanternPiece(new Rectangle(4, 6 + frameYAdd, 40, 64), 0, -7, 0, pos + new Point(-1, 0), pos + new Point(0, 0), drawCenterPos, spriteBatch, tileDrawing);
		if (tile.TileFrameX >= 54)
		{
			frameYAdd += 32;
		}
		DrawLanternPiece(new Rectangle(52, 6 + frameYAdd, 26, 28), 0.06f, -7 + offXAdd, 4, pos + new Point(0, 1), pos + new Point(0, 1), drawCenterPos, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// 画屏风灯
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
	private void DrawLanternPiece(Rectangle frame, float swayCoefficient, int offsetX, int offsetY, Point tilePos, Point paintPos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Color color = default(Color))
	{
		// 回声涂料
		if (!TileDrawing.IsVisible(Main.tile[paintPos]))
		{
			return;
		}

		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		int paint = Main.tile[paintPos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.ChinesePartitionLamp_Draw_Path, type, frame.X, paint, tileDrawing);
		tex ??= ModAsset.ChinesePartitionLamp_Draw.Value;

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
		if (color != default(Color))
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
		var origin = new Vector2(13, 0);
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(offsetX, offsetY), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
	}
}
