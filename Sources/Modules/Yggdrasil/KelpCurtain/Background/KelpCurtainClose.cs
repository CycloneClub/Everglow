using Everglow.Commons.Utilities.BackgroundHelper;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class KelpCurtainClose : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.KelpCurtainClose.Value;
		Distance = 1 / 0.35f;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.InModBiome<KelpCurtainBiome>();
	}
}
