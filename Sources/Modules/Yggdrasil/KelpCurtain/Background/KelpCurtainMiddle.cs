using Everglow.Commons.Utilities.BackgroundHelper;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class KelpCurtainMiddle : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.KelpCurtainMiddle.Value;
		Distance = 1 / 0.15f;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.InModBiome<KelpCurtainBiome>();
	}
}
