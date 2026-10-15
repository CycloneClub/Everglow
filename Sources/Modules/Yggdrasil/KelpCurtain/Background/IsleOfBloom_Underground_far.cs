using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain.Biomes;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class IsleOfBloom_Underground_far : BackgroundSlideBase
{

	public List<Point> BgTiles = new List<Point>();

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.IsleOfBloom_Underground_far.Value;
		Distance = 2.4f;
		UseColorStyle = 1;
		LayerPriority = 2;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		var bars = new List<Vertex2D>();
		BackgroundHigherPerformanceHelper.Add_TileBgVertice(this, BgTiles, bars);
		DrawVertexBackground(this, PrimitiveType.TriangleStrip, bars);
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.InModBiome<IsleOfBloomBiome>();
	}
}
