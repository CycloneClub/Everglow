using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DecayingWoodCourt;

public class WitherWoodBrazier : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileFlame[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoFail[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

		DustType = ModContent.DustType<WitherWoodDust>(); // You should set a kind of dust manually.
		AdjTiles = new int[] { TileID.Chandeliers };

		// Placement - Standard Chandelier Setup Below
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Origin = new Point16(1, 0);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.StyleHorizontal = false;
		TileObjectData.newTile.StyleLineSkip = 2;
		TileObjectData.newTile.DrawYOffset = -2;
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
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
			r = 1.5f;
			g = 0.8f;
			b = 2.250f;
		}
		else
		{
			r = 0f;
			g = 0f;
			b = 0f;
		}
	}

	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		if (++frameCounter >= 4)
		{
			frameCounter = 0;
			frame = ++frame % 16;
		}
	}

	public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
	{
		frameYOffset = Main.tileFrame[type] * 54;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		Chandelier3x3FluentDraw(screenPosition, pos, spriteBatch, tileDrawing);
	}

	public void Chandelier3x3FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		int left = Main.tile[pos].TileFrameX / 18;
		left %= 3;
		left = pos.X - left;
		int top = pos.Y - Main.tile[pos].TileFrameY / 18;
		HangingObjectFluentDraw(screenPosition, pos, spriteBatch, tileDrawing, new Point(left, top), 0, 0.11f);
	}

	/// <summary>
	/// 用于悬挂类物块的FluentDraw，用于带摆动绘制，适配油漆
	/// </summary>
	/// <param name="screenPosition">屏幕坐标</param>
	/// <param name="pos">该格物块的物块坐标</param>
	/// <param name="spriteBatch">批量雪碧</param>
	/// <param name="tileDrawing">TileDrawing工具类实例</param>
	/// <param name="topLeft">物块整体左上角的坐标</param>
	/// <param name="swayOffset">用于旗帜类物块，摇曳时底部物块会有类似“卷起来”的效果，对于吊灯应直接设置为0</param>
	/// <param name="swayStrength">风力和推力引起的旋转幅度系数。</param>
	public static void HangingObjectFluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Point topLeft, float swayOffset = -4f, float swayStrength = 0.15f)
	{
		var tile = Main.tile[pos];
		var tileData = TileObjectData.GetTileData(tile.type, 0);

		if (!TileDrawing.IsVisible(tile) || tileData is null)
		{
			return;
		}

		// 油漆
		Texture2D tex = tileDrawing.GetTileDrawTexture(tile, pos.X, pos.Y);

		short tileFrameX = tile.frameX;
		short tileFrameY = tile.frameY;

		// 用于风速、推力等一系列判定的物块坐标，通常来说是挂在墙上的那一格（这边是origin格）
		int topTileX = topLeft.X + tileData.Origin.X;
		int topTileY = topLeft.Y + tileData.Origin.Y;
		int sizeX = tileData.Width;
		int sizeY = tileData.Height;

		int offsetY = tileData.DrawYOffset;

		// 锤子是这样的
		if (WorldGen.IsBelowANonHammeredPlatform(topTileX, topTileY))
		{
			offsetY -= 8;
		}

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(topLeft.X, topLeft.Y, sizeX, sizeY))
		{
			windCycle = tileDrawing.GetWindCycle(topTileX, topTileY, tileDrawing._sunflowerWindCounter);
		}

		// 普通源码罢了
		int totalPushTime = 60;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(topTileX, topTileY, sizeX, sizeY, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;

		// 适配发光涂料
		int addFrX = 0;
		int addFrY = 0;
		short frameX = tile.TileFrameX;
		short frameY = tile.TileFrameY;
		int width = 16;
		int height = 16;
		TileLoader.SetDrawPositions(pos.X, pos.Y, ref width, ref offsetY, ref height, ref frameX, ref frameY); // calculates the draw offsets
		TileLoader.SetAnimationFrame(tile.TileType, pos.X, pos.Y, ref addFrX, ref addFrY); // calculates the animation offsets

		Rectangle rectangle = new Rectangle(tileFrameX + addFrX, tileFrameY + addFrY, 16, 16);
		Color tileLight = Lighting.GetColor(pos);
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(pos.X, pos.Y, tile, tile.type, tileFrameX, tileFrameY, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(pos.Y, pos.X, tile, tile.type, tileFrameX, tileFrameY, tileLight);

		// 基础的坐标
		Vector2 center = new Vector2(topTileX, topTileY).ToWorldCoordinates(autoAddY: 0) - screenPosition;
		Vector2 offset = new Vector2(0f, offsetY);
		center += offset;

		// heightStrength是用于旗帜类物块的，根据高度来决定该格物块的摇曳力度
		float heightStrength = (pos.Y - topLeft.Y + 1) / (float)sizeY;
		if (heightStrength == 0f)
		{
			heightStrength = 0.1f;
		}

		// 计算绘制坐标和origin，原版代码
		Vector2 tileCoordPos = pos.ToWorldCoordinates(0, 0) - screenPosition;
		tileCoordPos += offset;

		// 用于旗帜
		float swayCorrection = Math.Abs(windCycle) * swayOffset * heightStrength;
		Vector2 finalOrigin = center - tileCoordPos;
		Vector2 finalDrawPos = center + new Vector2(0, swayCorrection);

		// 旋转角度
		if (swayOffset == 0f)
		{
			heightStrength = 1f;
		}

		float rotation = -windCycle * swayStrength * heightStrength;

		// 绘制
		ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)pos.Y << 32 | (uint)pos.X); // Don't remove any casts.
		spriteBatch.Draw(tex, finalDrawPos, rectangle, tileLight, rotation, finalOrigin, 1f, SpriteEffects.None, 0f);
		rectangle.X += 108;
		for (int k = 0; k < 7; k++)
		{
			float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
			float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

			spriteBatch.Draw(tex, finalDrawPos + new Vector2(xx, yy), rectangle, new Color(100, 100, 100, 60), rotation, finalOrigin, 1f, SpriteEffects.None, 0f);
		}

		int frequency = 60;
		if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)) && Main.rand.NextBool(frequency) && rectangle.X == 126 && rectangle.Y % 54 == 36)
		{
			Rectangle dustBox = Utils.CenteredRectangle(new Vector2(pos.X * 16 + 8, pos.Y * 16 + 4) - new Vector2(4, 4), new Vector2(16, 8));
			Dust dust = Dust.NewDustDirect(dustBox.TopLeft(), dustBox.Width, dustBox.Height, ModContent.DustType<WitherWoodTorchDust>(), 0f, 0f, 254, default, Main.rand.NextFloat(0.75f, 0.95f));
			dust.velocity.X *= 0.1f;
			dust.velocity.Y = -2.4f;
		}

		// 有火的话绘制火
		if (TileLoader.GetTile(tile.type) is not ITileFlameData tileFlame)
		{
			return;
		}
	}
}
