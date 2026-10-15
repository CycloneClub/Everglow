using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class VampireMatCave_BoardSign : ShapeDataTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		TotalWidth = 5;
		TotalHeight = 9;
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Height = 9;
		TileObjectData.newTile.Width = 5;
		TileObjectData.newTile.CoordinateHeights = new int[9];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 2);
		TileObjectData.newTile.Origin = new Point16(2, 8);
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<VampireMatCave_BoardSign_Dust>();
		AddMapEntry(new Color(119, 116, 62));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.tile[i, j + 1].TileType != Type && Main.tile[i, j].TileFrameX == 36)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawSign(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	private void DrawSign(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		drawCenterPos += new Vector2(1, 10);
		var tile = TileUtils.SafeGetTile(tilePos);

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[tilePos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.VampireMatCave_BoardSign_Path, Type, 1, paint, tileDrawing);
		tex ??= ModAsset.VampireMatCave_BoardSign.Value;

		var tileLight = Lighting.GetColor(tilePos);

		// 支持发光涂料
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, Type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y, tile, Type, 0, 0, tileLight);

		var frame = new Rectangle(148, 16, 70, 146);
		TileUtils.VertexDraw_Grid(drawCenterPos, frame, new Vector2(35, 146), tex, spriteBatch, 0);

		Vector2 kelpPos = drawCenterPos + new Vector2(-14, -50);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int y = 0; y <= 3; y++)
		{
			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind(tilePos.X - 1, tilePos.Y - 2 + y, 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle(tilePos.X - 1, tilePos.Y - 2 + y, tileDrawing._sunflowerWindCounter);
			}
			int totalPushTime = 140;
			float pushForcePerFrame = 0.96f;
			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X - 1, tilePos.Y - 2 + y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			windCycle *= y * 0.2f;
			AddWorldVertex(bars, kelpPos + new Vector2(windCycle * 10 - 11, y * 16), new Vector2(92, 106 + y * 16), tex);
			AddWorldVertex(bars, kelpPos + new Vector2(windCycle * 10 + 11, y * 16), new Vector2(114, 106 + y * 16), tex);
		}
		if (bars.Count > 2)
		{
			spriteBatch.GraphicsDevice.Textures[0] = tex;
			spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		kelpPos = drawCenterPos + new Vector2(12, -50);
		bars = new List<Vertex2D>();
		for (int y = 0; y <= 3; y++)
		{
			float windCycle = 0;
			if (tileDrawing.InAPlaceWithWind(tilePos.X + 1, tilePos.Y - 2 + y, 1, 1))
			{
				windCycle = tileDrawing.GetWindCycle(tilePos.X + 1, tilePos.Y - 2 + y, tileDrawing._sunflowerWindCounter);
			}
			int totalPushTime = 140;
			float pushForcePerFrame = 0.96f;
			float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X + 1, tilePos.Y - 2 + y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
			windCycle += highestWindGridPushComplex;
			windCycle *= y * 0.2f;
			AddWorldVertex(bars, kelpPos + new Vector2(windCycle * 10 - 10, y * 16), new Vector2(126, 106 + y * 16), tex);
			AddWorldVertex(bars, kelpPos + new Vector2(windCycle * 10 + 10, y * 16), new Vector2(146, 106 + y * 16), tex);
		}
		if (bars.Count > 2)
		{
			spriteBatch.GraphicsDevice.Textures[0] = tex;
			spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	private void AddWorldVertex(List<Vertex2D> bars, Vector2 worldPos, Vector2 coord, Texture2D tex)
	{
		bars.Add(worldPos, Lighting.GetColor((worldPos + Main.screenPosition).ToTileCoordinates()), new Vector3(coord / tex.Size(), 0));
	}
}
