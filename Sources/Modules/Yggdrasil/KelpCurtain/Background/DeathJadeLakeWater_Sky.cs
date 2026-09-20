using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain.Biomes;

namespace Everglow.Yggdrasil.KelpCurtain.Background;

public class DeathJadeLakeWater_Sky : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.DeathJadeLakeWater_Sky.Value;
		Distance = 5000;
		LayerPriority = 1;
		Shader = Effects.XClamp_YWrap_Shader;
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
