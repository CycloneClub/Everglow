using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class PylonSquare_Chain : BackgroundSlideBase
{
	public Point TileAnchor;

	public List<Point> BgTiles = [];

	public List<Rectangle> GlowFrames = [];

	public override bool AllowMultiple => true;

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.PylonSquare_Chain.Value;
		Distance = 1;
		UseColorStyle = 1;
		LayerPriority = 0;
		Shader = Effects.XClamp_YWrap_Shader;
	}

	public override void Update()
	{
		base.Update();
		//if (Texture == ModAsset.PylonSquare_Chain_Flip.Value)
		//{
		//	Point center = YggdrasilTownGeneration.YggdrasilTownTopLeft + new Point(1270, 68);
		//	WorldAnchor = center.ToWorldCoordinates() + new Vector2(48, 370);
		//	BgTiles = TileUtils.GetAABBAreaOfTile(center.X, center.Y, 5, 120);
		//}
	}

	public override void Draw()
	{
		var bars = new List<Vertex2D>();
		BackgroundHigherPerformanceHelper.Add_TileBgVertice(this, BgTiles, bars, 1);
		DrawVertexBackground(this, PrimitiveType.TriangleStrip, bars);
	}

	public override bool CanActive()
	{
		return TileUtils.SafeGetTile(TileAnchor).TileType == ModContent.TileType<YggdrasilCommandBlock>();
	}
}
