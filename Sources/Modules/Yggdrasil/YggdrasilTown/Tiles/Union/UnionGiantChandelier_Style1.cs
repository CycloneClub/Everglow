using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class UnionGiantChandelier_Style1 : ShapeDataTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		CustomItemType = ModContent.ItemType<UnionGiantChandelier_Item>();
		DustType = ModContent.DustType<UnionGiantChandelier_Dust>();
		TotalWidth = 8;
		TotalHeight = 7;

		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = true;

		AdjTiles = new int[] { TileID.Chandeliers };

		TileObjectData.newTile.Width = 8;
		TileObjectData.newTile.Height = 7;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = new int[7];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new(3, 0);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorBottom = default;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(218, 199, 186));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX is >= 0 and < 144)
		{
			if (tile.TileFrameY == 18 || tile.TileFrameY == 72)
			{
				r = 1f;
				g = 1f;
				b = 0.9f;
			}
		}
		r *= 4;
		g *= 4;
		b *= 4;
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 8, 7);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (fail)
		{
			return;
		}
		var thisTile = Main.tile[i, j];
		int x0 = i - thisTile.TileFrameX / 18;
		int y0 = j - thisTile.TileFrameY / 18;
		int times = 1;
		for (int x = 0; x < TotalWidth; x++)
		{
			for (int y = 0; y < TotalHeight; y++)
			{
				var tile = Main.tile[x0 + x, y0 + y];
				if (tile.TileFrameX == x * 18 && tile.TileFrameY == y * 18)
				{
					if (tile.TileType == Type && PixelHasTile[x, y] >= 200)
					{
						times++;
						tile.HasTile = false;
						Dust dust = Dust.NewDustDirect(new Vector2(x0 + x, y0 + y) * 16, 16, 16, DustType, 0, 0, 0, default, 1);
						dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 8, 8);

						// if (tile.TileFrameY >= 36)
						// {
						// glassDust = true;
						// }
						// int max = glassDust ? 5 : 1;
						// for (int a = 0; a < max; a++)
						// {
						// Dust dust = Dust.NewDustDirect(new Vector2(x0 + x, y0 + y) * 16, 16, 16, DustType, 0, 0, 0, default, 1);
						// dust.frame = new Rectangle(glassDust ? 10 : 0, Main.rand.Next(3) * 10, 8, 8);
						// }
					}
				}
			}
		}
		if (!MultiItem)
		{
			CustomDropItem(i, j);
		}
		SoundEngine.PlaySound(HitSound, new Vector2(i * 16, j * 16));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		int left = Main.tile[pos].TileFrameX / 18;
		left %= 8;
		left = pos.X - left;
		int top = pos.Y - Main.tile[pos].TileFrameY / 18;
		HangingObjectFluentDraw(screenPosition, pos, spriteBatch, tileDrawing, new Point(left, top));
	}

	/// <summary>
	/// 绘制巨形吊灯
	/// </summary>
	/// <param name="swayCoefficient">撞钟的摇摆系数</param>
	/// <param name="offsetX">绘制偏移</param>
	/// <param name="tilePos">用于进行摇晃和风速判定的物块的坐标</param>
	/// <param name="paintPos">用于应用漆的物块的坐标</param>
	/// <param name="drawCenterPos">绘制中心的坐标</param>
	/// <param name="spriteBatch">合批绘制</param>
	/// <param name="tileDrawing">原版TileDrawing类的实例，有很多好用的方法</param>
	public void HangingObjectFluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Point topLeft, float swayOffset = -4f, float swayStrength = 0.15f)
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
		short tileFrameY = (short)(tile.frameY + 126);

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
		int totalPushTime = 240;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(topTileX, topTileY, sizeX, sizeY, totalPushTime, pushForcePerFrame, 5, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;

		// 适配发光涂料
		Rectangle rectangle = new Rectangle(tileFrameX, tileFrameY, 16, 16);
		Color tileLight = Lighting.GetColor(pos);
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(pos.X, pos.Y, tile, tile.type, tileFrameX, tileFrameY, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(pos.Y, pos.X, tile, tile.type, tileFrameX, tileFrameY, tileLight);

		// 基础的坐标
		Vector2 center = new Vector2(topTileX, topTileY).ToWorldCoordinates(autoAddY: 0) - screenPosition;
		Vector2 offset = new Vector2(0f, offsetY);
		center += offset;

		// heightStrength是用于旗帜类物块的，根据高度来决定该格物块的摇曳力度,这里是吊灯,所以扭曲度与Y无关
		float heightStrength = 0.3f;

		// 计算绘制坐标和origin，原版代码
		Vector2 tileCoordPos = pos.ToWorldCoordinates(0, 0) - screenPosition;
		tileCoordPos += offset;

		float swayCorrection = Math.Abs(windCycle) * swayOffset * heightStrength;
		Vector2 finalOrigin = center - tileCoordPos;
		Vector2 finalDrawPos = center + new Vector2(0, swayCorrection);

		// 旋转角度
		if (swayOffset == 0f)
		{
			heightStrength = 1f;
		}

		float rotation = -windCycle * swayStrength * heightStrength;
		spriteBatch.Draw(tex, finalDrawPos, rectangle, tileLight, rotation, finalOrigin, 1f, SpriteEffects.None, 0f);
		if (tileFrameY - 126 == 54)
		{
			int style = (tileFrameX % 144) / 18;
			var frame = new Rectangle(style * 12, 252, 12, 46);
			if (tileFrameX >= 144)
			{
				frame.X += 144;
			}
			float windCycle2 = 0;
			if (tileDrawing.InAPlaceWithWind(topLeft.X + style, topLeft.Y + 3, 1, 2))
			{
				windCycle2 = tileDrawing.GetWindCycle(topLeft.X + style, topLeft.Y + 3, tileDrawing._sunflowerWindCounter);
			}
			int totalPushTime2 = 60;
			float pushForcePerFrame2 = 2f;
			float highestWindGridPushComplex2 = tileDrawing.GetHighestWindGridPushComplex(topTileX, topTileY, sizeX, sizeY, totalPushTime2, pushForcePerFrame2, 3, swapLoopDir: true);
			windCycle2 += highestWindGridPushComplex2;
			float rotation2 = windCycle2 * 0.1f;
			Vector2 anchorPos = finalDrawPos + new Vector2(8, 0);
			if (tileFrameX % 144 == 0)
			{
				anchorPos += new Vector2(-28, 66).RotatedBy(rotation);
				spriteBatch.Draw(tex, anchorPos, frame, tileLight, rotation2, new Vector2(5, 0), 1f, SpriteEffects.None, 0f);
			}

			// 18 and 36 inverse frame because of perspective relation.
			if (tileFrameX % 144 == 36)
			{
				anchorPos += new Vector2(-16, 66).RotatedBy(rotation);
				frame = new Rectangle((style - 1) * 12, 252, 12, 46);
				if (tileFrameX >= 144)
				{
					frame.X += 144;
				}
				spriteBatch.Draw(tex, anchorPos, frame, tileLight, rotation2, new Vector2(5, 0), 1f, SpriteEffects.None, 0f);
			}
			if (tileFrameX % 144 == 18)
			{
				anchorPos += new Vector2(-10, 66).RotatedBy(rotation);
				frame = new Rectangle((style + 1) * 12, 252, 12, 46);
				if (tileFrameX >= 144)
				{
					frame.X += 144;
				}
				spriteBatch.Draw(tex, anchorPos, frame, tileLight, rotation2, new Vector2(5, 0), 1f, SpriteEffects.None, 0f);
			}
			if (tileFrameX % 144 == 72)
			{
				anchorPos += new Vector2(10, 66).RotatedBy(rotation);
				spriteBatch.Draw(tex, anchorPos, frame, tileLight, rotation2, new Vector2(5, 0), 1f, SpriteEffects.None, 0f);
			}
			if (tileFrameX % 144 == 54)
			{
				anchorPos += new Vector2(0, 66).RotatedBy(rotation);
				spriteBatch.Draw(tex, anchorPos, frame, tileLight, rotation2, new Vector2(6, 0), 1f, SpriteEffects.None, 0f);
			}

			if (tileFrameX % 144 == 90)
			{
				anchorPos += new Vector2(16, 66).RotatedBy(rotation);
				spriteBatch.Draw(tex, anchorPos, frame, tileLight, rotation2, new Vector2(5, 0), 1f, SpriteEffects.None, 0f);
			}
			if (tileFrameX % 144 == 108)
			{
				anchorPos += new Vector2(28, 66).RotatedBy(rotation);
				spriteBatch.Draw(tex, anchorPos, frame, tileLight, rotation2, new Vector2(5, 0), 1f, SpriteEffects.None, 0f);
			}
		}
	}
}
