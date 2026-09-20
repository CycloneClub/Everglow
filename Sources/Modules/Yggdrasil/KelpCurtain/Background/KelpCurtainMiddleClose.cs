using Everglow.Commons.Utilities.BackgroundHelper;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class KelpCurtainMiddleClose : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.KelpCurtainMiddleClose.Value;
		Distance = 4;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.InModBiome<KelpCurtainBiome>();
	}
}
