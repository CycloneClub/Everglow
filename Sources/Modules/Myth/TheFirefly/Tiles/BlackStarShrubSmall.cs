using Everglow.Commons.TileHelper;
using Everglow.Myth.TheFirefly.Dusts;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class BlackStarShrubSmall : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			18,
		};
		TileObjectData.newTile.CoordinateWidth = 48;
		TileObjectData.addTile(Type);
		DustType = DustID.SpookyWood;
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(81, 110, 255), modTranslation);
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		short num = (short)Main.rand.Next(0, 6);
		Main.tile[i, j].TileFrameX = (short)(num * 48);
		Main.tile[i, j + 1].TileFrameX = (short)(num * 48);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY == 16)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 16) - screenPosition;
		Rectangle Frame(int y) => new Rectangle(tile.TileFrameX, y + 2, 48, 34);
		Point SwayHitboxPos(int addX) => new Point(pos.X + addX, pos.Y);
		Point PaintPos(int addY) => new Point(pos.X, pos.Y - addY);

		var drawInfo = new BlackStarShrub.BasicDrawInfo()
		{
			DrawCenterPos = drawCenterPos,
			SpriteBatch = spriteBatch,
			TileDrawing = tileDrawing,
		};

		DrawShrubPiece(Frame(2), 0.025f, SwayHitboxPos(0), PaintPos(0), drawInfo);
		DrawShrubPiece(Frame(38), 0.1f, SwayHitboxPos(0), PaintPos(0), drawInfo);
		DrawShrubPiece(Frame(218), 0.1f, SwayHitboxPos(0), PaintPos(0), drawInfo, new Color(0.27f, 0.27f, 0.27f, 0));
		DrawShrubPiece(Frame(72), 0.03f, SwayHitboxPos(-1), PaintPos(1), drawInfo);
		DrawShrubPiece(Frame(110), 0.024f, SwayHitboxPos(1), PaintPos(1), drawInfo);
		DrawShrubPiece(Frame(146), 0.011f, SwayHitboxPos(1), PaintPos(1), drawInfo);
		DrawShrubPiece(Frame(182), 0.013f, SwayHitboxPos(-1), PaintPos(1), drawInfo);
	}

	private static double _oldWindCounter = 0;
	private static double _ooldWindCounter = 0;

	/// <summary>
	/// 绘制灌木的一个小Piece
	/// </summary>
	private void DrawShrubPiece(Rectangle frame, float swayStrength, Point tilePos, Point paintPos, BlackStarShrub.BasicDrawInfo drawInfo, Color? specialColor = null)
	{
		var drawCenterPos = drawInfo.DrawCenterPos;
		var spriteBatch = drawInfo.SpriteBatch;
		var tileDrawing = drawInfo.TileDrawing;

		// 回声涂料
		if (!TileDrawing.IsVisible(Main.tile[tilePos]))
		{
			return;
		}

		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		int paint = Main.tile[paintPos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.BlackStarShrubSmallDraw_Path, type, frame.X + frame.Y, paint, tileDrawing);
		tex ??= ModAsset.BlackStarShrubSmallDraw.Value;

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

		// 角速度计算，源码乱抄与瞎写代码的结合，用于抖花粉
		float factor = tilePos.X + tilePos.Y / 100;
		float radiusNow = (float)Math.Cos(tileDrawing._sunflowerWindCounter * Math.PI * 2.0 + (double)factor);
		float radiusPrev = (float)Math.Cos(_ooldWindCounter * Math.PI * 2.0 + (double)factor);
		float angularVelocity = highestWindGridPushComplex + radiusNow - radiusPrev;

		// 支持发光涂料
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

		var origin = new Vector2(24, 36);
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(0, 8), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);

		// 抖落蓝色荧光花粉
		if (Math.Abs(angularVelocity) > 0.04 && Main.rand.NextBool(Math.Clamp((int)(100 - Math.Abs(angularVelocity) * 40f), 1, 900)))
		{
			var dustSpawnOffset = new Vector2(-6, -30);
			var dustSpawnPos = drawCenterPos + dustSpawnOffset.RotatedBy(rotation) + Main.screenPosition;
			var dustVelocity = new Vector2(-1f, 0f).RotatedBy(angularVelocity) * 0.7f;
			if (angularVelocity > 0)
			{
				dustVelocity = -dustVelocity;
			}

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
