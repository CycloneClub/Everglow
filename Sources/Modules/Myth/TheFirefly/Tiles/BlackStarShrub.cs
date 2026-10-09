using Everglow.Commons.TileHelper;
using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class BlackStarShrub : ModTile, ITileFluentlyDrawn
{
	internal struct BasicDrawInfo
	{
		internal Vector2 DrawCenterPos;
		internal SpriteBatch SpriteBatch;
		internal TileDrawing TileDrawing;
	}

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
		TileObjectData.newTile.CoordinateWidth = 72;
		TileObjectData.addTile(Type);
		DustType = 191;
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(84, 172, 255), modTranslation);
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		short num = (short)Main.rand.Next(0, 6);
		Main.tile[i, j].TileFrameX = (short)(num * 72);
		Main.tile[i, j + 1].TileFrameX = (short)(num * 72);
		Main.tile[i, j + 2].TileFrameX = (short)(num * 72);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY == 32)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 16) - screenPosition;
		Rectangle Frame(int y) => new Rectangle(tile.TileFrameX, y, 72, 56);
		Point SwayHitboxPos(int addX) => new Point(pos.X + addX, pos.Y);
		Point PaintPos(int addY) => new Point(pos.X, pos.Y - addY);

		var drawInfo = new BasicDrawInfo()
		{
			DrawCenterPos = drawCenterPos,
			SpriteBatch = spriteBatch,
			TileDrawing = tileDrawing,
		};

		DrawShrubPiece(Frame(0), 0.1f, SwayHitboxPos(0), PaintPos(1), drawInfo);
		DrawShrubPiece(Frame(58), 0.12f, SwayHitboxPos(0), PaintPos(1), drawInfo);
		DrawShrubPiece(Frame(114), 0.104f, SwayHitboxPos(-1), PaintPos(2), drawInfo);
		DrawShrubPiece(Frame(170), 0.075f, SwayHitboxPos(-1), PaintPos(2), drawInfo);
		DrawShrubPiece(Frame(226), 0.049f, SwayHitboxPos(-1), PaintPos(2), drawInfo);
		DrawShrubPiece(Frame(450), 0.104f, SwayHitboxPos(-1), PaintPos(2), drawInfo, new Color(0.27f, 0.27f, 0.27f, 0));
		DrawShrubPiece(Frame(506), 0.075f, SwayHitboxPos(-1), PaintPos(2), drawInfo, new Color(0.27f, 0.27f, 0.27f, 0));
		DrawShrubPiece(Frame(562), 0.049f, SwayHitboxPos(-1), PaintPos(2), drawInfo, new Color(0.27f, 0.27f, 0.27f, 0));
		DrawShrubPiece(Frame(282), 0.094f, SwayHitboxPos(1), PaintPos(0), drawInfo);
		DrawShrubPiece(Frame(338), 0.024f, SwayHitboxPos(1), PaintPos(0), drawInfo);
		DrawShrubPiece(Frame(394), 0.013f, SwayHitboxPos(1), PaintPos(0), drawInfo);
	}

	private static double _oldWindCounter = 0;
	private static double _ooldWindCounter = 0;

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
			return;

		int paint = Main.tile[paintPos].TileColor;
		int textureStyle = tile.TileFrameX + frame.Y * 50;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.BlackStarShrubDraw_Path, type, textureStyle, paint, tileDrawing);
		tex ??= ModAsset.BlackStarShrubDraw.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);

		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		float rotation = windCycle * swayStrength;

		// 角速度计算，源码乱抄与瞎写代码的结合，用于抖花粉
		float factor = tilePos.X + tilePos.Y / 100;
		float radiusNow = (float)Math.Cos(tileDrawing._sunflowerWindCounter * Math.PI * 2.0 + (double)factor);
		float radiusPrev = (float)Math.Cos(_ooldWindCounter * Math.PI * 2.0 + (double)factor);
		float angularVelocity = highestWindGridPushComplex + radiusNow - radiusPrev;

		// 颜色
		Color tileLight = Lighting.GetColor(tilePos);
		// 万象写的specialColor特效，似乎是摇曳越强越亮
		if (specialColor is not null)
		{
			float maxC = Math.Max(specialColor.Value.R / 255f, Math.Abs(rotation * 6));
			maxC = Math.Clamp(maxC, 0, 1);
			tileLight = new Color(maxC, maxC, maxC, 0);
		}
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(paintPos.X, paintPos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(paintPos.Y, paintPos.X, tile, type, 0, 0, tileLight);

		var origin = new Vector2(36, 56);
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(0, 6), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);

		// 抖落蓝色荧光花粉
		if (Math.Abs(angularVelocity) > 0.04 && Main.rand.NextBool(Math.Clamp((int)(100 - Math.Abs(angularVelocity) * 30f), 1, 900)))
		{
			var dustSpawnOffset = new Vector2(-6, -40);
			var dustSpawnPos = drawCenterPos + dustSpawnOffset.RotatedBy(rotation) + Main.screenPosition;
			var dustVelocity = new Vector2(-1f, 0f).RotatedBy(angularVelocity) * 0.7f;
			if (angularVelocity > 0)
				dustVelocity = -dustVelocity;
			var d = Dust.NewDustDirect(dustSpawnPos, 14, 16, ModContent.DustType<BlueParticleDark>(), Alpha: 150);
			var d2 = Dust.NewDustDirect(dustSpawnPos, 14, 16, ModContent.DustType<BlueParticle>(), Alpha: 150);
			d.scale = Main.rand.NextFloat() * 0.2f + 0.2f;
			d2.scale = Main.rand.NextFloat() * 0.2f + 0.3f;
			d.velocity = dustVelocity;
			d2.velocity = dustVelocity;
		}

		_ooldWindCounter = _oldWindCounter;
		_oldWindCounter = tileDrawing._sunflowerWindCounter;
	}
}
