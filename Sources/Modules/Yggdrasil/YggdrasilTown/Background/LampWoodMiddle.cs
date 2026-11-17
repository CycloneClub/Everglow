using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class LampWoodMiddle : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.LampWoodMiddle.Value;
		Distance = 18f;
		UseColorStyle = BackgroundColorStyle.White;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override bool CanActive()
	{
		return ModContent.GetInstance<LampWoodForest>().IsBiomeActive(Main.LocalPlayer);
	}
}
