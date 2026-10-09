using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class SideHangingLantern_White : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLighted[Type] = true;
		DustType = DustID.DynastyWood;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
		};

		TileObjectData.newAlternate.Alternates = new List<TileObjectData>();
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinateWidth = 16;

		TileObjectData.newTile.AnchorBottom = new AnchorData(0, 0, 0);
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
		TileObjectData.addAlternate(1);
		TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(135, 103, 90));
	}
	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY < 54 && tile.TileFrameY > 0)
		{
			r = 0.8f;
			g = 0.6f;
			b = 0.4f;
		}
		else
		{
			r = 0f;
			g = 0f;
			b = 0f;
		}
	}
	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwireStyleVertical(i, j, Type, 2, 3);
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX % 36 == 0 && tile.TileFrameY % 54 == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 0) - screenPosition;
		int offXByDir = -3;
		if (tile.TileFrameX < 36)
		{
			DrawLanternPiece(new Rectangle(0, 0, 36, 54), 0, 3, 0, pos, pos + new Point(0, 0), drawCenterPos, spriteBatch, tileDrawing);
		}
		else
		{
			DrawLanternPiece(new Rectangle(36, 0, 36, 54), 0, 3, 0, pos, pos + new Point(0, 0), drawCenterPos, spriteBatch, tileDrawing);
			offXByDir = -5;
		}

		if (tile.TileFrameY >= 54)
		{
			DrawLanternPiece(new Rectangle(40, 110, 22, 32), 0.16f, 12 + offXByDir, 8, pos + new Point(0, 1), pos + new Point(0, 1), drawCenterPos, spriteBatch, tileDrawing);
		}
		else
		{
			DrawLanternPiece(new Rectangle(6, 110, 22, 32), 0.16f, 12 + offXByDir, 8, pos + new Point(0, 1), pos + new Point(0, 1), drawCenterPos, spriteBatch, tileDrawing);
		}
		// 这种白色的灯纸比较浑浊,免去glow
	}
	/// <summary>
	/// 画侧挂灯
	/// </summary>
	/// <param name="frame"></param>
	/// <param name="swayCoefficient"></param>
	/// <param name="offsetX"></param>
	/// <param name="offsetY"></param>
	/// <param name="tilePos"></param>
	/// <param name="paintPos"></param>
	/// <param name="drawCenterPos"></param>
	/// <param name="spriteBatch"></param>
	/// <param name="tileDrawing"></param>
	/// <param name="color"></param>
	private void DrawLanternPiece(Rectangle frame, float swayCoefficient, int offsetX, int offsetY, Point tilePos, Point paintPos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing, Color color = new Color())
	{
		// 回声涂料	
		if (!TileDrawing.IsVisible(Main.tile[paintPos]))
		{
			return;
		}

		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		int paint = Main.tile[paintPos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.SideHangingLantern_White_Draw_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.SideHangingLantern_White_Draw.Value;

		int sizeX = 2;
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
		Color tileLight;
		if (color != new Color())
		{
			tileLight = color;
		}
		else
		{
			tileLight = Lighting.GetColor(tilePos);
		}
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.Y, tilePos.X, tile, type, 0, 0, tileLight);

		float rotation = -windCycle * swayCoefficient;
		var origin = new Vector2(11, 0);
		var tileSpriteEffect = SpriteEffects.None;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(offsetX, offsetY), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
	}
}
