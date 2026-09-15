using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class FluorescentHydraTree : ModTile, ITileFluentlyDrawn
{
	public const int MaxLength = 12;

	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = false;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<FluorescentHydraWoodDust>();

		AddMapEntry(new Color(134, 183, 201));
		HitSound = SoundID.Dig;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		Tile bottomTile = TileUtils.SafeGetTile(i, j + 1);
		Tile tile = Main.tile[i, j];
		if (tile.HasTile && tile.TileType == Type)
		{
			if (!bottomTile.HasTile || (bottomTile.TileType != Type && !Main.tileSolid[bottomTile.type] && !Main.tileSolidTop[bottomTile.type]))
			{
				int deltaY = 0;
				while (true)
				{
					Tile topTile = TileUtils.SafeGetTile(i, j - deltaY);
					if (topTile.HasTile && topTile.TileType == Type && j - deltaY > 0)
					{
						WorldGen.KillTile(i, j - deltaY, false, false, true);
					}
					else
					{
						break;
					}
					deltaY++;
				}
			}
		}
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		for (int k = 0; k < 2; k++)
		{
			Vector2 pos = new Point(i, j).ToWorldCoordinates();
			var d = Dust.NewDustDirect(pos - new Vector2(20, 40) + new Vector2(4), 40, 50, type);
			d.noGravity = true;
		}
		return false;
	}

	public override void RandomUpdate(int i, int j)
	{
		var tile = Main.tile[i, j];
		var tile2 = Main.tile[i, j - 1];

		if (tile2.TileType != tile.TileType && !tile2.HasTile)
		{
			int length = 0;
			int maxLengthHere = MaxLength - TileUtils.GetFixedRandomNumber(tile) % 3;
			while (TileUtils.SafeGetTile(i, j + length).TileType == Type)
			{
				length++;
				if (length >= maxLengthHere + 1)
				{
					break;
				}
			}
			if (length <= maxLengthHere)
			{
				tile2.TileType = Type;
				tile2.HasTile = true;
			}
		}

		int topY = j;
		for (int y = 0; y < 12; y++)
		{
			if (TileUtils.SafeGetTile(i, j - y).TileType != Type)
			{
				topY = j - y + 1;
				break;
			}
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		Lighting.AddLight(i, j, 0.4f, 0.7f, 0.7f);
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawFluorescentHydraTree(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of FluorescentHydra blossom
	/// </summary>
	private void DrawFluorescentHydraTree(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		int toBottom = 0;
		int toTop = 0;
		var tile = TileUtils.SafeGetTile(tilePos);
		for (int j = 1; j < MaxLength; j++)
		{
			toBottom = j;
			var checkTile = TileUtils.SafeGetTile(tilePos.X, tilePos.Y + j);
			if (checkTile.type != Type)
			{
				break;
			}
		}
		for (int j = 1; j < MaxLength; j++)
		{
			toTop = j;
			var checkTile = TileUtils.SafeGetTile(tilePos.X, tilePos.Y - j);
			if (checkTile.type != Type)
			{
				break;
			}
		}
		Texture2D tex = ModAsset.FluorescentHydraTree.Value;

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[tilePos].TileColor;
		tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.FluorescentHydraTree_Path, Type, 1, paint, tileDrawing);
		tex ??= ModAsset.FluorescentHydraTree.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(tilePos.X, tilePos.Y, 1, 1))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter) * 0.25f;
		}

		int totalPushTime = 140;
		float pushForcePerFrame = 0.96f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(tilePos.X, tilePos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex * 0.25f;
		float rotation = windCycle;

		var tileLight = Lighting.GetColor(tilePos);

		// 支持发光涂料
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, Type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y, tile, Type, 0, 0, tileLight);
		Rectangle frame = Rectangle.emptyRectangle;
		Vector2 origin = Vector2.zeroVector;

		if (toTop == 1)
		{
			int style = TileUtils.GetFixedRandomNumber(tile) % 1;
			frame = new Rectangle(74, 2, 162, 140);
			origin = new Vector2(79, 124);
			rotation *= 0.5f;
			// switch (style)
			// {
			// case 0:
			// frame = new Rectangle(14, 4, 340, 212);
			// origin = new Vector2(167, 212);
			// break;
			// }
		}
		if (toTop > 1 && toBottom > 3)
		{
			int style = TileUtils.GetFixedRandomNumber(tile) % 4;
			frame = new Rectangle(40, 18 + 18 * style, 22, 16);
			origin = new Vector2(11, 16);
			rotation *= 0;
		}
		if (toBottom == 1)
		{
			int style = TileUtils.GetFixedRandomNumber(tile) % 2;
			frame = new Rectangle(34 * style, 90, 34, 56);
			origin = new Vector2(17, 48);
			rotation *= 0;
		}
		if (frame != Rectangle.Empty)
		{
			VertexDrawTree(drawCenterPos, frame, origin, tex, spriteBatch, rotation);
			VertexDrawTree_glow(drawCenterPos, frame, origin, ModAsset.FluorescentHydraTree_glow.Value, spriteBatch, rotation);
		}
	}

	private void VertexDrawTree(Vector2 drawCenterPos, Rectangle frame, Vector2 origin, Texture2D tex, SpriteBatch spriteBatch, float rotation = 0)
	{
		var drawPos = drawCenterPos;
		List<Vertex2D> bars = new List<Vertex2D>();
		Vector2 pos = drawPos + Main.screenPosition;
		Vector2 offset0 = (new Vector2(0, 0) - origin).RotatedBy(rotation);
		Vector2 offset1 = (new Vector2(frame.Width, 0) - origin).RotatedBy(rotation);
		Vector2 offset2 = (new Vector2(0, frame.Height) - origin).RotatedBy(rotation);
		Vector2 offset3 = (new Vector2(frame.Width, frame.Height) - origin).RotatedBy(rotation);

		AddLightColorVertex(bars, pos + offset0, new Vector3(new Vector2(frame.X, frame.Y) / tex.Size(), 0));
		AddLightColorVertex(bars, pos + offset1, new Vector3(new Vector2(frame.X + frame.Width, frame.Y) / tex.Size(), 0));
		AddLightColorVertex(bars, pos + offset2, new Vector3(new Vector2(frame.X, frame.Y + frame.Height) / tex.Size(), 0));
		AddLightColorVertex(bars, pos + offset3, new Vector3(new Vector2(frame.X + frame.Width, frame.Y + frame.Height) / tex.Size(), 0));
		if (bars.Count > 2)
		{
			spriteBatch.GraphicsDevice.Textures[0] = tex;
			spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	private void VertexDrawTree_glow(Vector2 drawCenterPos, Rectangle frame, Vector2 origin, Texture2D tex, SpriteBatch spriteBatch, float rotation = 0)
	{
		var drawPos = drawCenterPos;
		float glowFade = 0.25f;
		Vector2 pos = drawPos;
		Vector2 offset0 = (new Vector2(0, 0) - origin).RotatedBy(rotation);
		Vector2 offset1 = (new Vector2(frame.Width, 0) - origin).RotatedBy(rotation);
		Vector2 offset2 = (new Vector2(0, frame.Height) - origin).RotatedBy(rotation);
		Vector2 offset3 = (new Vector2(frame.Width, frame.Height) - origin).RotatedBy(rotation);
		List<Vertex2D> bars = new List<Vertex2D>
		{
			{ pos + offset0, new Color(1f, 1f, 1f, 0) * glowFade, new Vector3(new Vector2(frame.X, frame.Y) / tex.Size(), 0) },
			{ pos + offset1, new Color(1f, 1f, 1f, 0) * glowFade, new Vector3(new Vector2(frame.X + frame.Width, frame.Y) / tex.Size(), 0) },
			{ pos + offset2, new Color(1f, 1f, 1f, 0) * glowFade, new Vector3(new Vector2(frame.X, frame.Y + frame.Height) / tex.Size(), 0) },
			{ pos + offset3, new Color(1f, 1f, 1f, 0) * glowFade, new Vector3(new Vector2(frame.X + frame.Width, frame.Y + frame.Height) / tex.Size(), 0) },
		};
		if (bars.Count > 2)
		{
			spriteBatch.GraphicsDevice.Textures[0] = tex;
			spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	private void AddLightColorVertex(List<Vertex2D> bars, Vector2 worldPos, Vector3 coord)
	{
		Color drawC = Lighting.GetColor(worldPos.ToTileCoordinates());
		drawC.A = 150;
		bars.Add(worldPos - Main.screenPosition, drawC, coord);
	}
}
