using Everglow.Commons.Utilities.BackgroundHelper;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class GiantFurnace_Construct_far : BackgroundSlideBase
{
	public Point TileAnchor;

	public List<Point> BgTiles = new List<Point>();

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.GiantFurnace_Construct_far.Value;
		Distance = 1.5f;
		UseColorStyle = 2;
		LayerPriority = 1;
		Shader = Effects.XClamp_YClamp_Shader;
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		var bars = new List<Vertex2D>();
		BackgroundHigherPerformanceHelper.Add_TileBgVertice(this, BgTiles, bars, 3);
		DrawVertexBackground(this, PrimitiveType.TriangleStrip, bars);
	}

	public override bool CanActive()
	{
		return TileUtils.SafeGetTile(TileAnchor).TileType == ModContent.TileType<YggdrasilCommandBlock>();
	}
}
