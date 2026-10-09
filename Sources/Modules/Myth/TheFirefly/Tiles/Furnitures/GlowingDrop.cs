using Everglow.Commons.TileHelper;
using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly;
using Everglow.Myth.TheFirefly.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles.Furnitures;

public class GlowingDrop : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoFail[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.CanBeSleptIn[Type] = true;
		TileID.Sets.InteractibleByNPCs[Type] = true;
		TileID.Sets.IsValidSpawnPoint[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

		DustType = ModContent.DustType<BlueGlow>();
		AdjTiles = new int[] { TileID.Chandeliers };

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorBottom = default;
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
	}
	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
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
			r = 0.1f;
			g = 0.5f;
			b = 1.2f;
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
			Adx = 46; // 改了下贴图，所以是46
		}

		DrawLanternPiece(0 + Adx, 40, (int)((Math.Sin(pos.X + pos.Y) * 100 + 100) % 26), -10, pos + new Point(-1, 0), pos, drawCenterPos, spriteBatch, tileDrawing);
		DrawLanternPiece(12 + Adx, 32, (int)((Math.Sin(pos.X + pos.Y) * 100 + 100) % 14), -4, pos + new Point(0, 1), pos + new Point(-1, 0), drawCenterPos, spriteBatch, tileDrawing);
		DrawLanternPiece(24 + Adx, 46, (int)((Math.Sin(pos.X + pos.Y) * 100 + 100) % 22), 14, pos, pos + new Point(-1, 0), drawCenterPos, spriteBatch, tileDrawing);
		DrawLanternPiece(34 + Adx, 26, (int)((Math.Sin(pos.X + pos.Y) * 100 + 100) % 14), 6, pos + new Point(1, 0), pos + new Point(-1, 0), drawCenterPos, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// 绘制灯的一个小Piece
	/// </summary>
	private void DrawLanternPiece(int frameX, int frameHeight, int frameY, int offsetX, Point tilePos, Point paintPos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		// 回声涂料	
		if (!TileDrawing.IsVisible(Main.tile[paintPos]))
		{
			return;
		}

		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		int paint = Main.tile[paintPos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.Tiles_GlowingDrop_Path, type, frameX, paint, tileDrawing);
		tex ??= ModAsset.Tiles_GlowingDrop.Value;
		var frame = new Rectangle(frameX, frameY, 10, frameHeight);

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

		float swayCoefficient = 0.12f;
		float rotation = -windCycle * swayCoefficient;
		var origin = new Vector2(6, 0);
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(offsetX, -2), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
	}
}
