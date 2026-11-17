using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class LampWoodSky : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.LampWoodSky.Value;
		Distance = float.PositiveInfinity;
		UseColorStyle = BackgroundColorStyle.White;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override bool CanActive()
	{
		return ModContent.GetInstance<LampWoodForest>().IsBiomeActive(Main.LocalPlayer);
	}
}
