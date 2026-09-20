using Everglow.Commons.TileHelper;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.GravelStore;

public class BakeryHangingLamp : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoFail[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

		DustType = DustID.BorealWood; // You should set a kind of dust manually.
		AdjTiles = new int[] { TileID.HangingLanterns };

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
		};
		TileObjectData.newTile.CoordinateWidth = 36;
		TileObjectData.newTile.DrawYOffset = -2;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.DrawYOffset = -10;
		TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
		TileObjectData.addAlternate(0);
		TileObjectData.addTile(Type); // addTile一定要放在addAlternate后面

		AddMapEntry(new Color(175, 115, 63));
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 1, 4);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX < 18)
		{
			r = 1.200f;
			g = 1.200f;
			b = 1.00f;
			Lighting.AddLight(i + 1, j, r, g, b);
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
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var tileData = TileObjectData.GetTileData(tile.type, 0);

		if (!TileDrawing.IsVisible(tile) || tileData is null)
		{
			return;
		}
		float swayOffset = 0f;
		int top = pos.Y - Main.tile[pos].TileFrameY / 18;
		Point topLeft = new Point(pos.X, top);

		// Paints
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
		Rectangle rectangle = new Rectangle(tileFrameX, tileFrameY, 34, 16);
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
		if (tile.TileFrameY >= 36)
		{
			heightStrength = 2f;
		}

		float swayStrength = 0.1f;

		float rotation = -windCycle * swayStrength * heightStrength;

		// 绘制
		spriteBatch.Draw(tex, finalDrawPos, rectangle, tileLight, rotation, finalOrigin, 1f, SpriteEffects.None, 0f);

		// 有火的话绘制火
		if (TileLoader.GetTile(tile.type) is not ITileFlameData tileFlame)
		{
			return;
		}
	}
}
