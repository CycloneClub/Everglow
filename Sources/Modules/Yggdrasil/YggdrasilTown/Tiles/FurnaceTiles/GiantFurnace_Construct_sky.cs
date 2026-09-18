using Everglow.Commons.Utilities.BackgroundHelper;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class GiantFurnace_Construct_sky : BackgroundSlideBase
{
	public Point TileAnchor;

	public List<Point> BgTiles = new List<Point>();

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.GiantFurnace_Construct_sky.Value;
		Distance = 3;
		UseColorStyle = 2;
		LayerPriority = 1;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override void Update()
	{
		base.Update();
		//if(Main.mouseLeft && Main.mouseLeftRelease)
		//{
		//	Distance = 3;
		//	WorldAnchor.Y = 320000;
		//	Main.NewText(WorldAnchor.Y);
		//}
	}

	public override void Draw()
	{
		var bars = new List<Vertex2D>();
		BackgroundHigherPerformanceHelper.Add_TileBgVertice(this, BgTiles, bars, 10);
		DrawVertexBackground(this, PrimitiveType.TriangleStrip, bars);
	}

	public override bool CanActive()
	{
		return TileUtils.SafeGetTile(TileAnchor).TileType == ModContent.TileType<YggdrasilCommandBlock>();
	}
}
