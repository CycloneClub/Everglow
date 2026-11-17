using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.Commons.Mechanics.CustomBackground;

public abstract class UnderBackgroundBackgroundSlideBase : BackgroundSlideBase
{
	public List<Point> BgTiles = new List<Point>();

	public int SamplingUnit = 3;

	public override void Draw()
	{
		List<Vertex2D> bars = new List<Vertex2D>();
		BackgroundHigherPerformanceHelper.Add_TileBgVertice(this, BgTiles, bars);
		DrawVertexBackground(this, PrimitiveType.TriangleStrip, bars);
	}
}
