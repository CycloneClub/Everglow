using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class AbandonedFishingNet : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<AbandonedLakeWreckDust>();
		AddMapEntry(new Color(109, 92, 75));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return base.PreDraw(i, j, spriteBatch);
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawFluctuatingFishingNet(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of lotus
	/// </summary>
	private void DrawFluctuatingFishingNet(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[tilePos];
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}
		int paint = Main.tile[tilePos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.AbandonedFishingNet_Path, tile.TileType, 1, paint, tileDrawing);
		tex ??= ModAsset.JadeLakeGreenAlgae.Value;
		Rectangle frame = new Rectangle(2, 50, 48, 18);
		int dir = 1;
		if (tile.TileFrameX >= 54)
		{
			dir = -1;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int k = 0; k <= 8; k++)
		{
			float value = k / 8f;
			float offsetY = (float)Math.Sin((float)Main.time * 0.04f + tilePos.Y * 0.5f + k * 0.25f) * 6 * Math.Clamp(k / 3f, 0, 1) + 16;
			Vector2 offset = new Vector2(value * frame.Width * dir, offsetY);
			Vector2 drawPos = drawCenterPos + offset;
			Color lightColor = Lighting.GetColor((drawCenterPos + Main.screenPosition).ToTileCoordinates());
			float coordX = (float)(Utils.Lerp(frame.X, frame.X + frame.Width, value) / tex.Width);
			bars.Add(drawPos + new Vector2(0, frame.Height * 0.5f), lightColor, new Vector3(coordX, (float)(frame.Y + frame.Height) / tex.Height, 0));
			bars.Add(drawPos + new Vector2(0, -frame.Height * 0.5f), lightColor, new Vector3(coordX, (float)frame.Y / tex.Height, 0));
		}
		if (bars.Count > 2)
		{
			spriteBatch.GraphicsDevice.Textures[0] = tex;
			spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
