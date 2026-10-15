using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain.Biomes;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class DeathJadeLakeWater_5 : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.DeathJadeLakeWater_5.Value;
		Distance = 48f;
		LayerPriority = 1;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.InModBiome<DeathJadeLakeBiome>();
	}

	public override void Draw()
	{
		DeathJadeLakeBackground.DrawBackground(this);
	}
}
