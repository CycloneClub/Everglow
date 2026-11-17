using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public abstract class TwilightBackgroundBase : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		UseColorStyle = BackgroundColorStyle.White;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override bool CanActive()
	{
		TwilightForsetAndRelic biome = ModContent.GetInstance<TwilightForsetAndRelic>();
		if (!biome.IsBiomeActive(Main.LocalPlayer))
		{
			return false;
		}
		return biome.IsTheCenterRelic || this is TwilightSky || this is TwilightVeryClose || this is TwilightVeryClose_Glow;
	}
}
