using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class LampWoodClose_Middle : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.LampWoodClose_Middle.Value;
		Distance = 6f;
		UseColorStyle = BackgroundColorStyle.White;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override bool CanActive()
	{
		return ModContent.GetInstance<LampWoodForest>().IsBiomeActive(Main.LocalPlayer);
	}
}
