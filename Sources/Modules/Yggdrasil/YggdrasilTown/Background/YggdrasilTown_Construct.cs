using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class YggdrasilTown_Construct : BackgroundSlideBase
{
	public Point TileAnchor;

	public List<Point> BgTiles = new List<Point>();

	public List<List<Point>> AreaTiles = [];

	public List<Rectangle> WindowFrames = new List<Rectangle>();

	public List<Rectangle> GlowingLogos = new List<Rectangle>();

	public override void SetDefaults()
	{
		base.SetDefaults();
		for (int k = 0; k < 3; k++)
		{
			int offsetX = (TileAnchor - new Vector2(260, -464).ToTileCoordinates()).X;
			var area = BgTiles
		   .Where(t => t.X >= (k - 0.5f) * 140 - 210 + offsetX && t.X < (k + 1.5f) * 140 - 210 + offsetX)
		   .ToList();
			AreaTiles.Add(area);
		}
		WindowFrames = new List<Rectangle>
		{
			new Rectangle(5344, 1104, 48, 64),
			new Rectangle(5424, 1104, 48, 64),
			new Rectangle(5720, 832, 38, 48),
			new Rectangle(6088, 910, 38, 50),
			new Rectangle(6088, 1056, 38, 48),
			new Rectangle(5760, 1168, 48, 64),
			new Rectangle(5888, 1216, 48, 64),
			new Rectangle(4928, 960, 48, 48),
			new Rectangle(4928, 1056, 48, 48),
			new Rectangle(4928, 1152, 48, 48),
			new Rectangle(4928, 1248, 48, 48),
			new Rectangle(4744, 736, 40, 48),
			new Rectangle(4744, 848, 40, 48),
			new Rectangle(4744, 960, 40, 48),
			new Rectangle(4744, 1072, 40, 48),
			new Rectangle(4744, 1184, 40, 48),
			new Rectangle(4320, 1056, 18, 48),
			new Rectangle(4384, 1056, 32, 48),
			new Rectangle(4462, 1056, 18, 48),
			new Rectangle(4368, 1200, 48, 64),
			new Rectangle(4208, 1152, 48, 64),
			new Rectangle(4128, 1152, 48, 64),
			new Rectangle(4032, 1104, 48, 64),
			new Rectangle(4032, 1216, 32, 48),
			new Rectangle(4032, 1296, 32, 48),
			new Rectangle(3968, 1104, 48, 64),
			new Rectangle(3968, 1216, 24, 48),
			new Rectangle(3968, 1296, 24, 48),
			new Rectangle(3872, 1152, 48, 64),
			new Rectangle(3794, 1152, 48, 64),
			new Rectangle(3832, 960, 40, 32),
			new Rectangle(3832, 864, 40, 48),
			new Rectangle(3720, 752, 38, 48),
			new Rectangle(3320, 824, 32, 40),
			new Rectangle(2880, 1136, 32, 48),
			new Rectangle(2880, 928, 32, 48),
			new Rectangle(2784, 928, 32, 48),
			new Rectangle(2784, 1040, 32, 48),
			new Rectangle(2504, 1328, 40, 64),
			new Rectangle(2120, 992, 40, 48),
			new Rectangle(2072, 720, 40, 48),
			new Rectangle(1520, 1056, 32, 30),
			new Rectangle(1064, 848, 38, 48),
			new Rectangle(1008, 912, 14, 30),
			new Rectangle(1008, 992, 14, 32),
			new Rectangle(1008, 1120, 14, 32),
			new Rectangle(608, 880, 48, 48),
			new Rectangle(608, 976, 48, 48),
			new Rectangle(608, 1072, 48, 48),

			// new Rectangle(430, 1232, 18, 48),
			// new Rectangle(174, 1232, 18, 48),
		};

		GlowingLogos = new List<Rectangle>
		{
			new Rectangle(2032, 1040, 462, 236),
			new Rectangle(5358, 850, 228, 254),
		};
		Texture = ModAsset.YggdrasilTown_Construct.Value;
		Distance = 1;
		UseColorStyle = 1;
		LayerPriority = 0;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		var topLeft = TileAnchor.ToWorldCoordinates() + new Vector2(260, -464) + new Vector2(-210, -89) * 16;
		int index = (int)((Main.screenPosition.X + 480 + Main.screenWidth * 0.5f - topLeft.X) / 16 / 140);
		index = Math.Clamp(index, 0, 2);
		var bars = new List<Vertex2D>();
		if (Main.screenWidth <= 2560)
		{
			if (AreaTiles.Count > 0)
			{
				BackgroundHigherPerformanceHelper.Add_TileBgVertice(this, AreaTiles[index], bars, 3);
				DrawVertexBackground(this, PrimitiveType.TriangleStrip, bars);
			}
		}
		else
		{
			BackgroundHigherPerformanceHelper.Add_TileBgVertice(this, BgTiles, bars, 3);
			DrawVertexBackground(this, PrimitiveType.TriangleStrip, bars);
		}
		foreach (var rectangle in WindowFrames)
		{
			if ((Main.time + TileUtils.GetFixedRandomNumber_SingleSeed(rectangle.X - rectangle.Y)) * 10 % 10240 > 5120)
			{
				if (VFXManager.InScreen(rectangle.Center() + topLeft, rectangle.Size().Length()))
				{
					bars = new List<Vertex2D>();
					AddWindow_Vertex_Frame(bars, rectangle);
					DrawVertexBackground(ModAsset.YggdrasilTown_Construct_LightedWindows.Value, PrimitiveType.TriangleStrip, bars);
				}
			}
		}
		foreach (var rectangle in GlowingLogos)
		{
			if (VFXManager.InScreen(rectangle.Center() + topLeft, rectangle.Size().Length()))
			{
				bars = new List<Vertex2D>();
				AddGlowingLogo_Vertex_Frame(bars, rectangle);
				DrawVertexBackground(ModAsset.YggdrasilTown_Construct_LightedWindows.Value, PrimitiveType.TriangleStrip, bars);
			}
		}
	}

	public void AddGlowingLogo_Vertex_Frame(List<Vertex2D> bars, Rectangle frame)
	{
		Texture2D tex = ModAsset.YggdrasilTown_Construct_LightedWindows.Value;
		var topLeft = TileAnchor.ToWorldCoordinates() + new Vector2(248, -472) + new Vector2(-210, -89) * 16 - Main.screenPosition;
		var color = new Color(0.5f, 0.5f, 0.5f, 0);
		var lightColor = new Vector3(1f, 1f, 1f);

		var pos0 = topLeft + frame.TopLeft();
		var pos1 = topLeft + frame.TopRight();
		var pos2 = topLeft + frame.BottomLeft();
		var pos3 = topLeft + frame.BottomRight();

		Lighting.AddLight(pos0 + Main.screenPosition, lightColor);
		Lighting.AddLight(pos1 + Main.screenPosition, lightColor);
		Lighting.AddLight(pos2 + Main.screenPosition, lightColor);
		Lighting.AddLight(pos3 + Main.screenPosition, lightColor);

		Lighting.AddLight((pos0 + pos1) * 0.5f + Main.screenPosition, lightColor);
		Lighting.AddLight((pos0 + pos2) * 0.5f + Main.screenPosition, lightColor);
		Lighting.AddLight((pos2 + pos3) * 0.5f + Main.screenPosition, lightColor);
		Lighting.AddLight((pos3 + pos1) * 0.5f + Main.screenPosition, lightColor);
		Lighting.AddLight((pos1 + pos2) * 0.5f + Main.screenPosition, lightColor);

		bars.Add(pos0, color, new Vector3(frame.TopLeft() / tex.Size(), 0));
		bars.Add(pos1, color, new Vector3(frame.TopRight() / tex.Size(), 0));

		bars.Add(pos2, color, new Vector3(frame.BottomLeft() / tex.Size(), 0));
		bars.Add(pos3, color, new Vector3(frame.BottomRight() / tex.Size(), 0));
	}

	public void AddWindow_Vertex_Frame(List<Vertex2D> bars, Rectangle frame)
	{
		Texture2D tex = ModAsset.YggdrasilTown_Construct_LightedWindows.Value;
		var topLeft = TileAnchor.ToWorldCoordinates() + new Vector2(248, -472) + new Vector2(-210, -89) * 16 - Main.screenPosition;
		var color = new Color(0.5f, 0.5f, 0.5f, 0);
		var lightColor = new Vector3(1f, 0.6f, 0.1f);
		if (frame.X is 2880 or 2784)
		{
			lightColor = new Vector3(0.5f, 1f, 0.2f);
		}

		var pos0 = topLeft + frame.TopLeft();
		var pos1 = topLeft + frame.TopRight();
		var pos2 = topLeft + frame.BottomLeft();
		var pos3 = topLeft + frame.BottomRight();

		Lighting.AddLight(pos0 + Main.screenPosition, lightColor);
		Lighting.AddLight(pos1 + Main.screenPosition, lightColor);
		Lighting.AddLight(pos2 + Main.screenPosition, lightColor);
		Lighting.AddLight(pos3 + Main.screenPosition, lightColor);

		bars.Add(pos0, color, new Vector3(frame.TopLeft() / tex.Size(), 0));
		bars.Add(pos1, color, new Vector3(frame.TopRight() / tex.Size(), 0));

		bars.Add(pos2, color, new Vector3(frame.BottomLeft() / tex.Size(), 0));
		bars.Add(pos3, color, new Vector3(frame.BottomRight() / tex.Size(), 0));
	}

	public override bool CanActive()
	{
		// return false;
		return TileUtils.SafeGetTile(TileAnchor).TileType == ModContent.TileType<YggdrasilCommandBlock>();
	}
}
