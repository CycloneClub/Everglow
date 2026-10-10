using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class HangingSkyLantern : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoFail[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

		DustType = DustID.DynastyWood;
		AdjTiles = new int[] { TileID.Chandeliers };

		// Placement

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorBottom = default;
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
		TileObjectData.newTile.Origin = new Point16(1, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(135, 103, 90));
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 3, 3);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX < 54)
		{
			r = 0.8f;
			g = 0.75f;
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
		if (tile.TileFrameX % 54 == 18 && tile.TileFrameY == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 0) - screenPosition;
		int Adx = 0;
		if (tile.TileFrameX > 54)
		{
			Adx = 70; // 改了下贴图，所以是70
		}

		DrawLanternPiece(42 + Adx, 58, 0.15f, -2, pos, pos, drawCenterPos, spriteBatch, tileDrawing);
		DrawLanternPiece(56 + Adx, 44, 0.11f, -4, pos, pos + new Point(-1, 0), drawCenterPos, spriteBatch, tileDrawing);
		DrawLanternPiece(28 + Adx, 40, 0.13f, 2, pos, pos + new Point(1, 0), drawCenterPos, spriteBatch, tileDrawing);
		DrawLanternPiece(14 + Adx, 44, 0.09f, 8, pos + new Point(1, 0), pos + new Point(1, 1), drawCenterPos, spriteBatch, tileDrawing);
		DrawLanternPiece(0 + Adx, 48, 0.09f, -8, pos + new Point(-1, 0), pos + new Point(-1, 1), drawCenterPos, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// 绘制灯的一个小Piece
	/// </summary>
	/// <param name="frameX">卷筒纸在贴图中的帧的X坐标</param>
	/// <param name="frameHeight">卷筒纸的高度</param>
	/// <param name="swayCoefficient">摇摆系数，为了让各卷筒纸摇晃不完全一致而设置的</param>
	/// <param name="offsetX">绘制偏移</param>
	/// <param name="tilePos">用于进行摇晃和风速判定的物块的坐标</param>
	/// <param name="paintPos">用于应用漆的物块的坐标，让五个卷筒纸都可以获得不同的漆</param>
	/// <param name="drawCenterPos">绘制中心的坐标（各个卷筒纸共享一个值）</param>
	/// <param name="spriteBatch">批量雪碧（各个卷筒纸共享一个值）</param>
	/// <param name="tileDrawing">原版TileDrawing类的实例，有很多好用的方法（各个卷筒纸共享一个值）</param>
	private void DrawLanternPiece(int frameX, int frameHeight, float swayCoefficient, int offsetX, Point tilePos, Point paintPos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		// 回声涂料
		if (!TileDrawing.IsVisible(Main.tile[paintPos]))
		{
			return;
		}

		if (!TileDrawing.IsVisible(Main.tile[paintPos]))
		{
			return;
		}

		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		int paint = Main.tile[paintPos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.HangingSkyLantern_Depart_Path, type, frameX, paint, tileDrawing);
		tex ??= ModAsset.HangingSkyLantern_Depart.Value;
		var frame = new Rectangle(frameX, 0, 12, frameHeight);

		int sizeX = 1;
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
		Color tileLight = Lighting.GetColor(tilePos);
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.Y, tilePos.X, tile, type, 0, 0, tileLight);

		float rotation = -windCycle * swayCoefficient;
		var origin = new Vector2(6, 0);
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(offsetX, -2), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
	}
}
