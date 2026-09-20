using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class MidnightBayou_Close : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.MidnightBayou_Close.Value;
		Distance = 6f;
		UseColorStyle = 2;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override bool CanActive()
	{
		return MidnightBayouBiome.InMidnightBayou(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f);
	}
}
