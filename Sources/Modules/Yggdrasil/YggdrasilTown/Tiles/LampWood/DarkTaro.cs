using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

public class DarkTaro : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			20,
		};
		TileObjectData.newTile.CoordinateWidth = 96;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<LampGrassDust>();
		AddMapEntry(new Color(51, 41, 96));
		HitSound = SoundID.Grass;
	}
	// public override void PlaceInWorld(int i, int j, Item item)
	// {
	// short num = (short)Main.rand.Next(0, 3);
	// Main.tile[i, j].TileFrameX = (short)(num * 72);
	// Main.tile[i, j + 1].TileFrameX = (short)(num * 72);
	// Main.tile[i, j + 2].TileFrameX = (short)(num * 72);
	// }

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY == 36)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 16) - screenPosition;
		Rectangle Frame(int y) => new Rectangle(tile.TileFrameX / 72 * 100, y, 100, 92);
		Point SwayHitboxPos(int addX) => new Point(pos.X + addX, pos.Y);
		Point PaintPos(int addY) => new Point(pos.X, pos.Y - addY);

		var drawInfo = new BasicDrawInfo()
		{
			DrawCenterPos = drawCenterPos,
			SpriteBatch = spriteBatch,
			TileDrawing = tileDrawing,
		};

		DrawShrubPiece(Frame(0), 0.1f, SwayHitboxPos(0), PaintPos(1), drawInfo);
		DrawShrubPiece(Frame(96), 0.12f, SwayHitboxPos(0), PaintPos(1), drawInfo);
		DrawShrubPiece(Frame(192), 0.104f, SwayHitboxPos(-1), PaintPos(2), drawInfo);
		DrawShrubPiece(Frame(288), 0.075f, SwayHitboxPos(-1), PaintPos(2), drawInfo);
		DrawShrubPiece(Frame(384), 0.053f, SwayHitboxPos(1), PaintPos(3), drawInfo);
		DrawShrubPiece(Frame(480), 0, SwayHitboxPos(-1), PaintPos(2), drawInfo);
	}
	/// <summary>
	/// 绘制灌木的一个小Piece
	/// </summary>
	private void DrawShrubPiece(Rectangle frame, float swayStrength, Point tilePos, Point paintPos, BasicDrawInfo drawInfo, Color? specialColor = null)
	{
		var drawCenterPos = drawInfo.DrawCenterPos;
		var spriteBatch = drawInfo.SpriteBatch;
		var tileDrawing = drawInfo.TileDrawing;

		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[paintPos].TileColor;
		int textureStyle = tile.TileFrameX + frame.Y * 50;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.DarkTaro_Path, type, textureStyle, paint, tileDrawing);
		tex ??= ModAsset.DarkTaro.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
		}

		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		float rotation = windCycle * swayStrength;

		// 颜色
		Color tileLight = Lighting.GetColor(tilePos);
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(paintPos.X, paintPos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(paintPos.Y, paintPos.X, tile, type, 0, 0, tileLight);

		var origin = new Vector2(47, 95);
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(0, 6), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
	}
}
