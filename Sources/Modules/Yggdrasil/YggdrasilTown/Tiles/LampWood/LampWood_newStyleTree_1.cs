using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Drawing;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

public class LampWood_newStyleTree_1 : ShapeDataTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.tileAxe[Type] = true;
		DustType = ModContent.DustType<LampWood_Dust>();
		AddMapEntry(new Color(49, 41, 96));
	}

	public override void PostSetDefaults()
	{
		base.PostSetDefaults();
		MultiItem = true;
		TileID.Sets.IsATreeTrunk[Type] = true;
		CustomItemType = ModContent.ItemType<LampWood_Wood>();
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}

	public override void CustomDropItem(int i, int j)
	{
		if (CustomItemType > 0)
		{
			Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(), new Item(CustomItemType, 1));
			if (Main.rand.NextBool(6))
			{
				Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(), new Item(ModContent.ItemType<LampFruit>(), 1));
			}
			if (Main.rand.NextBool(16))
			{
				int maxBorer = Main.rand.Next(4, 8);
				for (int h = 0; h < maxBorer; h++)
				{
					NPC.NewNPCDirect(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates() + new Vector2(0, h * 12).RotatedByRandom(MathHelper.TwoPi), ModContent.NPCType<LampFruitBorer>());
				}
			}
		}
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 16) - screenPosition;
		Point SwayHitboxPos(int addX, int addY) => new Point(pos.X + addX, pos.Y + addY);
		Point PaintPos(int addX, int addY) => new Point(pos.X + addX, pos.Y + addY);

		var drawInfo = new BasicDrawInfo()
		{
			DrawCenterPos = drawCenterPos,
			SpriteBatch = spriteBatch,
			TileDrawing = tileDrawing,
		};

		// trunk_2
		DrawBranchPiece(new Rectangle(638, 274, 44, 92), 0, SwayHitboxPos(1, 0), PaintPos(1, 0), new Vector2(24, 92), new Vector2(4, -42), 2, 4, drawInfo);

		// trunk_1
		DrawBranchPiece(new Rectangle(328, 514, 64, 114), 0, SwayHitboxPos(1, 0), PaintPos(1, 0), new Vector2(32, 70), new Vector2(6, -10), 2, 4, drawInfo);

		// trunk_0
		DrawBranchPiece(new Rectangle(22, 552, 128, 76), 0, SwayHitboxPos(1, 0), PaintPos(1, 0), new Vector2(0, 38), new Vector2(-50, 64), 2, 4, drawInfo);

		Vector2 branch1Move = GetInfoRotatedMoved(0.030f, SwayHitboxPos(-3, -10), 3, 3, new Vector2(-32, -90), drawInfo);

		// branch_1
		// back
		// leaf_0
		DrawBranchPiece(new Rectangle(408, 268, 44, 90), -0.037f, SwayHitboxPos(-4, -10), PaintPos(-3, -10), new Vector2(38, 2), new Vector2(-50, -180) + branch1Move, 2, 4, drawInfo);

		// leaf_1
		DrawBranchPiece(new Rectangle(540, 240, 54, 76), -0.045f, SwayHitboxPos(-3, -10), PaintPos(-3, -10), new Vector2(0, 0), new Vector2(-40, -196) + branch1Move, 3, 3, drawInfo);

		// subbranch_0
		DrawBranchPiece(new Rectangle(632, 162, 74, 92), 0.030f, SwayHitboxPos(-4, -10), PaintPos(3, -10), new Vector2(72, 90), new Vector2(-14, -80), 4, 6, drawInfo);

		// flower
		DrawBranchPiece(new Rectangle(572, 64, 88, 70), 0.072f, SwayHitboxPos(-6, -14), PaintPos(3, -10), new Vector2(60, 68), new Vector2(-44, -172) + branch1Move, 6, 4, drawInfo, true);
		Lighting.AddLight(drawInfo.DrawCenterPos + new Vector2(-44, -172) + branch1Move + Main.screenPosition, 0.5f, 0.4f, 0);

		// front

		// leaf_2
		DrawBranchPiece(new Rectangle(364, 24, 78, 98), -0.067f, SwayHitboxPos(-8, -10), PaintPos(-3, -10), new Vector2(77, 4), new Vector2(-55, -176) + branch1Move, 5, 6, drawInfo);

		// leaf_3
		DrawBranchPiece(new Rectangle(468, 18, 88, 78), -0.075f, SwayHitboxPos(-2, -11), PaintPos(-3, -10), new Vector2(0, 20), new Vector2(-46, -170) + branch1Move, 5, 4, drawInfo);

		// leaf_4
		DrawBranchPiece(new Rectangle(544, 152, 40, 68), -0.127f, SwayHitboxPos(-2, -10), PaintPos(-3, -10), new Vector2(0, 2), new Vector2(-42, -168) + branch1Move, 3, 3, drawInfo);

		// leaf_6
		DrawBranchPiece(new Rectangle(382, 142, 54, 98), -0.155f, SwayHitboxPos(-6, -10), PaintPos(-3, -10), new Vector2(52, 2), new Vector2(-48, -176) + branch1Move, 3, 5, drawInfo);

		// leaf_5
		DrawBranchPiece(new Rectangle(480, 150, 44, 100), -0.175f, SwayHitboxPos(-3, -9), PaintPos(-3, -10), new Vector2(12, 2), new Vector2(-46, -166) + branch1Move, 3, 6, drawInfo);

		Vector2 branch2Move = GetInfoRotatedMoved(0.042f, SwayHitboxPos(2, -8), 2, 5, new Vector2(41, -87), drawInfo);

		// branch_2
		// back
		// leaf_5
		DrawBranchPiece(new Rectangle(190, 134, 40, 74), -0.052f, SwayHitboxPos(4, -7), PaintPos(3, -8), new Vector2(0, 2), new Vector2(58, -142) + branch2Move, 2, 2, drawInfo);

		// leaf_4
		DrawBranchPiece(new Rectangle(84, 144, 40, 68), -0.042f, SwayHitboxPos(1, -7), PaintPos(3, -8), new Vector2(38, 1), new Vector2(50, -143) + branch2Move, 2, 3, drawInfo);

		// subbranch_0
		DrawBranchPiece(new Rectangle(246, 74, 46, 88), 0.042f, SwayHitboxPos(2, -8), PaintPos(3, -8), new Vector2(0, 87), new Vector2(12, -51), 2, 5, drawInfo);

		// flower
		DrawBranchPiece(new Rectangle(238, 14, 70, 58), 0.082f, SwayHitboxPos(2, -10), PaintPos(3, -8), new Vector2(34, 56), new Vector2(56, -140) + branch2Move, 3, 4, drawInfo, true);
		Lighting.AddLight(drawInfo.DrawCenterPos + new Vector2(56, -140) + branch2Move + Main.screenPosition, 0.5f, 0.4f, 0);

		// front
		// leaf_2
		DrawBranchPiece(new Rectangle(182, 56, 62, 64), -0.127f, SwayHitboxPos(3, -8), PaintPos(3, -8), new Vector2(3, 7), new Vector2(59, -135) + branch2Move, 2, 3, drawInfo);

		// leaf_3
		DrawBranchPiece(new Rectangle(46, 116, 28, 44), -0.141f, SwayHitboxPos(2, -7), PaintPos(3, -8), new Vector2(27, 1), new Vector2(55, -143) + branch2Move, 2, 2, drawInfo);

		// leaf_0
		DrawBranchPiece(new Rectangle(56, 30, 62, 76), -0.165f, SwayHitboxPos(0, -8), PaintPos(3, -8), new Vector2(61, 15), new Vector2(51, -129) + branch2Move, 3, 3, drawInfo);

		// leaf_1
		DrawBranchPiece(new Rectangle(138, 84, 42, 82), -0.172f, SwayHitboxPos(4, -8), PaintPos(3, -8), new Vector2(22, 0), new Vector2(56, -140) + branch2Move, 3, 4, drawInfo);

		Vector2 branch3Move = GetInfoRotatedMoved(0.032f, SwayHitboxPos(3, -3), 4, 3, new Vector2(54, -40), drawInfo);

		// branch_3

		// back
		// leaf_5
		DrawBranchPiece(new Rectangle(192, 524, 32, 86), -0.083f, SwayHitboxPos(3, -4), PaintPos(5, -4), new Vector2(31, 9), new Vector2(75, -67) + branch3Move, 2, 6, drawInfo);

		// subbranch_0
		DrawBranchPiece(new Rectangle(18, 448, 58, 44), 0.032f, SwayHitboxPos(3, -3), PaintPos(5, -4), new Vector2(0, 87), new Vector2(24, 21), 4, 3, drawInfo);

		// flower
		DrawBranchPiece(new Rectangle(52, 396, 68, 52), 0.072f, SwayHitboxPos(4, -6), PaintPos(5, -4), new Vector2(20, 52), new Vector2(78, -66) + branch3Move, 3, 4, drawInfo, true);
		Lighting.AddLight(drawInfo.DrawCenterPos + new Vector2(78, -66) + branch3Move + Main.screenPosition, 0.5f, 0.4f, 0);

		// front
		// leaf_0
		DrawBranchPiece(new Rectangle(130, 406, 74, 98), -0.087f, SwayHitboxPos(0, -4), PaintPos(5, -4), new Vector2(73, 11), new Vector2(73, -67) + branch3Move, 5, 6, drawInfo);

		// leaf_1
		DrawBranchPiece(new Rectangle(348, 404, 68, 92), -0.070f, SwayHitboxPos(6, -4), PaintPos(5, -4), new Vector2(0, 9), new Vector2(80, -67) + branch3Move, 4, 6, drawInfo);

		// leaf_2
		DrawBranchPiece(new Rectangle(206, 432, 40, 62), -0.160f, SwayHitboxPos(3, -3), PaintPos(5, -4), new Vector2(39, 1), new Vector2(77, -67) + branch3Move, 2, 3, drawInfo);

		// leaf_4
		DrawBranchPiece(new Rectangle(304, 428, 42, 76), -0.171f, SwayHitboxPos(6, -3), PaintPos(5, -4), new Vector2(0, 0), new Vector2(80, -66) + branch3Move, 2, 5, drawInfo);

		// leaf_3
		DrawBranchPiece(new Rectangle(254, 434, 50, 112), -0.151f, SwayHitboxPos(4, -3), PaintPos(5, -4), new Vector2(21, 0), new Vector2(79, -66) + branch3Move, 3, 7, drawInfo);

		Vector2 branch4Move = GetInfoRotatedMoved(0.032f, SwayHitboxPos(-5, -1), 5, 3, new Vector2(-89, -54), drawInfo);

		// branch_4

		// back
		// leaf_0
		DrawBranchPiece(new Rectangle(192, 348, 24, 48), -0.063f, SwayHitboxPos(-7, 0), PaintPos(5, -4), new Vector2(24, 0), new Vector2(-86, -22) + branch4Move, 2, 3, drawInfo);

		// leaf_1
		DrawBranchPiece(new Rectangle(256, 322, 34, 78), -0.045f, SwayHitboxPos(-4, 0), PaintPos(5, -4), new Vector2(0, 0), new Vector2(-82, -20) + branch4Move, 2, 5, drawInfo);

		// subbranch_0
		DrawBranchPiece(new Rectangle(50, 292, 98, 64), 0.026f, SwayHitboxPos(-5, -1), PaintPos(-5, -1), new Vector2(94, 53), new Vector2(8, 29), 5, 3, drawInfo);

		// flower
		DrawBranchPiece(new Rectangle(26, 236, 44, 58), 0.091f, SwayHitboxPos(-7, -4), PaintPos(-5, -1), new Vector2(23, 54), new Vector2(-87, -26) + branch4Move, 3, 4, drawInfo, true);
		Lighting.AddLight(drawInfo.DrawCenterPos + new Vector2(-87, -26) + branch4Move + Main.screenPosition, 0.5f, 0.4f, 0);

		// front
		// leaf_2
		DrawBranchPiece(new Rectangle(154, 250, 54, 82), -0.075f, SwayHitboxPos(-8, -1), PaintPos(5, -4), new Vector2(52, 7), new Vector2(-84, -21) + branch4Move, 3, 5, drawInfo);

		// leaf_3
		DrawBranchPiece(new Rectangle(260, 248, 54, 68), -0.079f, SwayHitboxPos(-4, -1), PaintPos(5, -4), new Vector2(2, 12), new Vector2(-78, -22) + branch4Move, 3, 4, drawInfo);

		// leaf_4
		DrawBranchPiece(new Rectangle(216, 260, 26, 68), -0.079f, SwayHitboxPos(-6, 0), PaintPos(5, -4), new Vector2(23, 0), new Vector2(-77, -24) + branch4Move, 2, 4, drawInfo);
	}

	/// <summary>
	/// 绘制一个树枝和叶子
	/// </summary>
	private void DrawBranchPiece(Rectangle frame, float swayStrength, Point tilePos, Point paintPos, Vector2 drawOrigin, Vector2 offset, int pushWidth, int pushHeight, BasicDrawInfo drawInfo, bool withGlow = false)
	{
		// 是否在调试
		bool adjusting = false;
		var drawCenterPos = drawInfo.DrawCenterPos;
		var spriteBatch = drawInfo.SpriteBatch;
		var tileDrawing = drawInfo.TileDrawing;

		var tile = Main.tile[tilePos];
		ushort type = Type;

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[paintPos].TileColor;
		int textureStyle = 0;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.LampWood_newStyleTree_1_leave_Path, type, textureStyle, paint, tileDrawing);
		tex ??= ModAsset.LampWood_newStyleTree_1_leave.Value;

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
			frame.X += 68;
			frame.Y += 780;

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
		if (tile.TileFrameX == 144 && tile.TileFrameY == 252)
		{
			// var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			// if (Main.drawToScreen)
			// zero = Vector2.Zero;
			// Texture2D tex = ModAsset.LampWood_newStyleTree_1_preview.Value;
			// spriteBatch.Draw(tex, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(-128, -224), null, Lighting.GetColor(i, j) * 0.5f, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0f);
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
	}
}
