using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

public class WaveLeafFlower4x7 : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.Width = 7;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			16,
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<LampGrassDust>();
		AddMapEntry(new Color(30, 39, 77));
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}
	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 16) - screenPosition;
		Point SwayHitboxPos(int addX, int addY) => new Point(pos.X + addX, pos.Y + addY);
		Point PaintPos(int addX, int addY) => new Point(pos.X + addX, pos.Y + addY);

		var drawInfo = new BasicDrawInfo()
		{
			DrawCenterPos = drawCenterPos,
			SpriteBatch = spriteBatch,
			TileDrawing = tileDrawing,
		};
		Tile tile = Main.tile[pos];

		Vector2 move0 = GetInfoRotatedMoved(0.018f, SwayHitboxPos(1, -1), 1, 2, new Vector2(0, -16), drawInfo);
		Vector2 move1 = GetInfoRotatedMoved(0.048f, SwayHitboxPos(1, -2), 1, 2, new Vector2(0, -62), drawInfo);
		Vector2 move2 = GetInfoRotatedMoved(0.068f, SwayHitboxPos(1, -3), 1, 2, new Vector2(0, -52), drawInfo);
		// back
		DrawLeaf(new Rectangle(0, 444, 64, 30), 0.198f, SwayHitboxPos(1, -6), PaintPos(1, -6), new Vector2(32, 30), new Vector2(24, -56) + move0 + move1 + move2, 1, 2, drawInfo);
		DrawLeaf(new Rectangle(0, 400, 64, 16), 0.068f, SwayHitboxPos(1, -3), PaintPos(1, -3), new Vector2(32, 16), new Vector2(24, -52) + move0 + move1, 1, 2, drawInfo);
		DrawLeaf(new Rectangle(0, 334, 64, 62), 0.048f, SwayHitboxPos(1, -2), PaintPos(1, -2), new Vector2(32, 62), new Vector2(24, 2) + move0, 1, 2, drawInfo);
		DrawLeaf(new Rectangle(0, 198, 64, 52), 0.018f, SwayHitboxPos(1, -1), PaintPos(1, -1), new Vector2(32, 52), new Vector2(24, 16), 1, 2, drawInfo);
	}
	/// <summary>
	/// 绘制一个树枝和叶子
	/// </summary>
	private void DrawLeaf(Rectangle frame, float swayStrength, Point tilePos, Point paintPos, Vector2 drawOrigin, Vector2 offset, int pushWidth, int pushHeight, BasicDrawInfo drawInfo, bool withGlow = false)
	{
		// 是否在调试
		bool adjusting = false;
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
		int textureStyle = 1;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.WaveLeafFlower4x7_preview_Path, type, textureStyle, paint, tileDrawing);
		tex ??= ModAsset.WaveLeafFlower4x7_preview.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, pushWidth, pushHeight))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
		}

		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, pushWidth, pushHeight, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		float rotation = windCycle * swayStrength;
		if (adjusting)
		{
			rotation = 0;
		}

		// 颜色
		Color tileLight = Lighting.GetColor(tilePos);
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(paintPos.X, paintPos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(paintPos.Y, paintPos.X, tile, type, 0, 0, tileLight);

		var tileSpriteEffect = SpriteEffects.None;
		float adjustingColor = 1;
		if (adjusting)
		{
			adjustingColor = (MathF.Sin((float)Main.timeForVisualEffects * 0.15f) + 1) * 0.5f + 0.1f;
		}
		spriteBatch.Draw(tex, drawCenterPos + offset, frame, tileLight * adjustingColor, rotation, drawOrigin, 1f, tileSpriteEffect, 0f);
		if (withGlow)
		{
			frame.X += 170;
			frame.Y += 620;

			frame.X -= 30;
			frame.Y -= 30;
			frame.Width += 60;
			frame.Height += 60;

			spriteBatch.Draw(tex, drawCenterPos + offset, frame, new Color(1f, 1f, 1f, 0) * adjustingColor, rotation, drawOrigin + new Vector2(30), 1f, tileSpriteEffect, 0f);
		}
	}
	/// <summary>
	/// 绘制一个树枝和叶子
	/// </summary>
	private Vector2 GetInfoRotatedMoved(float swayStrength, Point tilePos, int pushWidth, int pushHeight, Vector2 bone, BasicDrawInfo drawInfo)
	{
		// 是否在调试
		bool adjusting = false;

		var tileDrawing = drawInfo.TileDrawing;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, pushWidth, pushHeight))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
		}

		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, pushWidth, pushHeight, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		float rotation = windCycle * swayStrength;
		if (adjusting)
		{
			rotation = 0;
		}
		return bone.RotatedBy(rotation) - bone;
	}
	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 108)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
	}
}
