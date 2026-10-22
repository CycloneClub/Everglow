using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Commons.Vertex;

namespace Everglow.Example.BgSlides;

public class ExampleUnderGroundBg : UnderBackgroundBackgroundSlideBase
{
	public int TimeLeft = 600;

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = Commons.ModAsset.Noise_Wave.Value;
		Distance = 5f;
		UseColorStyle = 0;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override bool AllowMultiple => true;

	public override int MaxInstanceNumber => 10000;

	public override void Update()
	{
		base.Update();
		TimeLeft--;
	}

	public override bool CanActive()
	{
		return TimeLeft > 0;
	}
}
