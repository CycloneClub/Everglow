using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain.Biomes;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class VampireMatCaveWall : BackgroundSlideBase
{

	public List<Point> BgTiles = new List<Point>();

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.VampireMatCaveWall.Value;
		Distance = 1f;
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
		return (Main.LocalPlayer.Center - KelpCurtainGeneration.VampireMatCaveCenter).Length() < new Vector2(Main.screenWidth, Main.screenHeight).Length() / 2f + 60 * 16;
	}
}
