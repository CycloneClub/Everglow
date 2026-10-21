using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public abstract class TwilightBackgroundBase : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		UseColorStyle = 2;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override bool CanActive()
	{
		return ModContent.GetInstance<TwilightForsetAndRelic>().IsBiomeActive(Main.LocalPlayer);
	}
}
