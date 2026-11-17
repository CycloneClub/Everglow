using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Yggdrasil.KelpCurtain.Biomes;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class IsleOfBloom_Underground_Close : BackgroundSlideBase
{
	public List<Point> BgTiles = new List<Point>();

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.IsleOfBloom_Underground_Close.Value;
		Distance = 1.15f;
		UseColorStyle = BackgroundColorStyle.TileMap;
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
