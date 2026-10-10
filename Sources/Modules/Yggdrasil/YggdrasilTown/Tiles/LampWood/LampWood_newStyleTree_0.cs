using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Drawing;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

public class LampWood_newStyleTree_0 : ShapeDataTile, ITileFluentlyDrawn
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
			if (Main.rand.NextBool(360))
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
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 16) - screenPosition;
		Point SwayHitboxPos(int addX, int addY) => new Point(pos.X + addX, pos.Y + addY);
		Point PaintPos(int addX, int addY) => new Point(pos.X + addX, pos.Y + addY);

		var drawInfo = new BasicDrawInfo()
		{
			DrawCenterPos = drawCenterPos,
			SpriteBatch = spriteBatch,
			TileDrawing = tileDrawing,
		};

		// back Leaves

		// branch_1_leaf_5
		DrawBranchPiece(new Rectangle(698, 104, 30, 106), -0.045f, SwayHitboxPos(2, -4), PaintPos(2, -4), new Vector2(9, 8), new Vector2(9, -92), 1, 5, drawInfo);

		// branch_1_leaf_4
		DrawBranchPiece(new Rectangle(732, 48, 80, 130), -0.047f, SwayHitboxPos(2, -4), PaintPos(1, -4), new Vector2(1, 15), new Vector2(5, -83), 5, 7, drawInfo);

		// branch_1_leaf_8
		DrawBranchPiece(new Rectangle(600, 194, 16, 36), -0.034f, SwayHitboxPos(2, -4), PaintPos(1, -3), new Vector2(36, 0), new Vector2(22, -80), 1, 2, drawInfo);

		// branch_1_leaf_7
		DrawBranchPiece(new Rectangle(558, 164, 36, 46), -0.028f, SwayHitboxPos(2, -4), PaintPos(-1, -3), new Vector2(36, 0), new Vector2(-10, -86), 1, 1, drawInfo);

		// branch_1_leaf_6
		DrawBranchPiece(new Rectangle(512, 150, 30, 40), -0.042f, SwayHitboxPos(2, -4), PaintPos(0, -3), new Vector2(30, 0), new Vector2(-2, -82), 1, 2, drawInfo);

		// branch_2_leaf_0
		DrawBranchPiece(new Rectangle(40, 56, 24, 54), -0.104f, SwayHitboxPos(-2, -1), PaintPos(-1, -1), new Vector2(21, 6), new Vector2(-42, -44), 1, 1, drawInfo);

		// branch_2_leaf_1
		DrawBranchPiece(new Rectangle(108, 68, 24, 72), -0.109f, SwayHitboxPos(-2, -1), PaintPos(-1, -1), new Vector2(0, 0), new Vector2(-40, -46), 1, 1, drawInfo);

		// branches
		// trunk_0
		DrawBranchPiece(new Rectangle(178, 12, 88, 114), -0.0f, SwayHitboxPos(-2, -1), PaintPos(0, 1), new Vector2(9, 113), new Vector2(-31, 27), 1, 1, drawInfo);

		// trunk_1
		DrawBranchPiece(new Rectangle(169, 456, 56, 94), -0.0f, SwayHitboxPos(-2, -1), PaintPos(-1, 6), new Vector2(34, 93), new Vector2(-28, 95), 1, 1, drawInfo);

		// root_0
		DrawBranchPiece(new Rectangle(0, 498, 102, 52), -0.0f, SwayHitboxPos(-2, -1), PaintPos(0, 9), new Vector2(52, 51), new Vector2(0, 129), 1, 1, drawInfo);

		// branch_1_flower_2
		DrawBranchPiece(new Rectangle(330, 214, 50, 54), 0.067f, SwayHitboxPos(-2, -6), PaintPos(1, -4), new Vector2(49, 53), new Vector2(-5, -93), 3, 3, drawInfo, true);

		// branch_1_flower_3
		DrawBranchPiece(new Rectangle(42, 244, 42, 48), 0.071f, SwayHitboxPos(0, -7), PaintPos(1, -4), new Vector2(23, 47), new Vector2(-3, -93), 3, 3, drawInfo, true);

		Vector2 rotMoved = GetInfoRotatedMoved(0.037f, SwayHitboxPos(1, 2), 4, 3, new Vector2(59, -33), drawInfo);

		// branch_4_leaf_3
		DrawBranchPiece(new Rectangle(138, 214, 26, 56), -0.057f, SwayHitboxPos(1, 4), PaintPos(2, 4), new Vector2(21, 0), new Vector2(15, 34) + rotMoved, 1, 3, drawInfo);

		// branch_4_leaf_4
		DrawBranchPiece(new Rectangle(194, 214, 26, 58), -0.062f, SwayHitboxPos(3, 4), PaintPos(2, 4), new Vector2(2, 6), new Vector2(28, 36) + rotMoved, 2, 2, drawInfo);

		// branch_4_flower_0
		DrawBranchPiece(new Rectangle(516, 264, 92, 72), 0.037f, SwayHitboxPos(1, 2), PaintPos(2, 4), new Vector2(0, 67), new Vector2(-36, 71), 4, 3, drawInfo, true);

		// branch_4_leaf_0
		DrawBranchPiece(new Rectangle(120, 150, 38, 56), -0.117f, SwayHitboxPos(1, 5), PaintPos(2, 4), new Vector2(37, 3), new Vector2(23, 41) + rotMoved, 1, 2, drawInfo);

		// branch_4_leaf_1
		DrawBranchPiece(new Rectangle(208, 150, 44, 58), -0.122f, SwayHitboxPos(4, 4), PaintPos(2, 4), new Vector2(0, 5), new Vector2(26, 39) + rotMoved, 2, 4, drawInfo);

		// branch_4_leaf_2
		DrawBranchPiece(new Rectangle(168, 152, 32, 60), -0.176f, SwayHitboxPos(2, 5), PaintPos(2, 4), new Vector2(15, 0), new Vector2(23, 42) + rotMoved, 2, 3, drawInfo);

		// branch_1_flower_0
		DrawBranchPiece(new Rectangle(272, 74, 88, 106), 0.07f, SwayHitboxPos(-2, -10), PaintPos(1, -4), new Vector2(49, 105), new Vector2(-1, -87), 5, 6, drawInfo, true);

		// branch_1_flower_1
		DrawBranchPiece(new Rectangle(412, 20, 90, 96), 0.06f, SwayHitboxPos(1, -10), PaintPos(1, -4), new Vector2(13, 95), new Vector2(-1, -87), 5, 5, drawInfo, true);

		// front leaves
		// branch_2_leaf_2
		DrawBranchPiece(new Rectangle(16, 6, 42, 48), -0.204f, SwayHitboxPos(-3, -1), PaintPos(-1, -1), new Vector2(41, 7), new Vector2(-39, -47), 1, 1, drawInfo);

		// branch_2_leaf_3
		DrawBranchPiece(new Rectangle(104, 4, 60, 58), -0.164f, SwayHitboxPos(-1, -1), PaintPos(-1, -1), new Vector2(0, 10), new Vector2(-42, -44), 1, 1, drawInfo);

		// branch_2_leaf_4
		DrawBranchPiece(new Rectangle(70, 8, 32, 64), -0.112f, SwayHitboxPos(-2, -1), PaintPos(-1, -1), new Vector2(16, 1), new Vector2(-46, -47), 1, 1, drawInfo);

		// branch_1_leaf_2
		DrawBranchPiece(new Rectangle(814, 16, 124, 124), -0.186f, SwayHitboxPos(2, -4), PaintPos(1, -4), new Vector2(0, 14), new Vector2(12, -86), 8, 7, drawInfo);

		// branch_1_leaf_3
		DrawBranchPiece(new Rectangle(610, 126, 28, 48), -0.326f, SwayHitboxPos(2, -4), PaintPos(1, -4), new Vector2(2, 2), new Vector2(8, -86), 2, 2, drawInfo);

		// branch_1_leaf_1
		DrawBranchPiece(new Rectangle(512, 22, 82, 96), -0.162f, SwayHitboxPos(-4, -5), PaintPos(1, -4), new Vector2(81, 25), new Vector2(-1, -81), 6, 5, drawInfo);

		// branch_1_leaf_0
		DrawBranchPiece(new Rectangle(606, 14, 84, 108), -0.172f, SwayHitboxPos(1, -6), PaintPos(1, -4), new Vector2(3, 51), new Vector2(3, -75), 6, 6, drawInfo);

		// branch_3_trunk_0
		DrawBranchPiece(new Rectangle(76, 352, 34, 60), 0, SwayHitboxPos(-6, -2), PaintPos(-3, 3), new Vector2(24, 59), new Vector2(-40, 75), 5, 4, drawInfo);

		// branch_3_leaf_4
		DrawBranchPiece(new Rectangle(312, 378, 18, 54), -0.107f, SwayHitboxPos(-3, 2), PaintPos(-3, 3), new Vector2(6, 9), new Vector2(-64, 11), 1, 4, drawInfo);

		// branch_3_leaf_5
		DrawBranchPiece(new Rectangle(250, 376, 24, 64), -0.113f, SwayHitboxPos(-2, 3), PaintPos(-3, 3), new Vector2(0, 0), new Vector2(-64, 12), 1, 2, drawInfo);

		// branch_3_flower_0
		DrawBranchPiece(new Rectangle(368, 344, 82, 68), 0.027f, SwayHitboxPos(-6, -1), PaintPos(-3, 3), new Vector2(64, 67), new Vector2(-58, 15), 5, 4, drawInfo, true);

		// branch_3_leaf_0
		DrawBranchPiece(new Rectangle(116, 300, 74, 82), -0.152f, SwayHitboxPos(-7, 2), PaintPos(-3, 3), new Vector2(73, 15), new Vector2(-63, 15), 4, 5, drawInfo);

		// branch_3_leaf_3
		DrawBranchPiece(new Rectangle(128, 394, 42, 66), -0.161f, SwayHitboxPos(-5, 3), PaintPos(-3, 3), new Vector2(41, 0), new Vector2(-61, 14), 2, 4, drawInfo);

		// branch_3_leaf_1
		DrawBranchPiece(new Rectangle(196, 316, 38, 98), -0.162f, SwayHitboxPos(-4, 4), PaintPos(-3, 3), new Vector2(20, 0), new Vector2(-66, 16), 2, 5, drawInfo);

		// branch_3_leaf_2
		DrawBranchPiece(new Rectangle(256, 318, 44, 52), -0.158f, SwayHitboxPos(-2, 4), PaintPos(-3, 3), new Vector2(0, 7), new Vector2(-62, 17), 2, 2, drawInfo);
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
		ushort type = tile.TileType;

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[paintPos].TileColor;
		int textureStyle = 1;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.LampWood_newStyleTree_0_leave_Path, type, textureStyle, paint, tileDrawing);
		tex = ModAsset.LampWood_newStyleTree_0_leave.Value;

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
		if (tile.TileFrameX == 144 && tile.TileFrameY == 198)
		{
			// var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			// if (Main.drawToScreen)
			// zero = Vector2.Zero;
			// Texture2D tex = ModAsset.LampWood_newStyleTree_0_preview.Value;
			// spriteBatch.Draw(tex, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(-128, -176), null, Lighting.GetColor(i, j) * 0.5f, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0f);
			Lighting.AddLight(i - 4, j - 1, 0.5f, 0.4f, 0);
			Lighting.AddLight(i + 1, j - 9, 0.8f, 0.64f, 0);
			Lighting.AddLight(i + 3, j + 2, 0.5f, 0.4f, 0);
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
	}
}
