using Everglow.Commons.Utilities.BackgroundHelper;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class KelpCurtainSky : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.KelpCurtainSky.Value;
		Distance = float.PositiveInfinity;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.InModBiome<KelpCurtainBiome>();
	}
}
