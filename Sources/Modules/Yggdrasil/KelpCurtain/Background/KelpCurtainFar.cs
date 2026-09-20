using Everglow.Commons.Utilities.BackgroundHelper;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class KelpCurtainFar : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.KelpCurtainFar.Value;
		Distance = 10f;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.InModBiome<KelpCurtainBiome>();
	}
}
