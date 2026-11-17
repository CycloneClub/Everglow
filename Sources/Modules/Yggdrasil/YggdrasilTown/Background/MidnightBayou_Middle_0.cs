using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class MidnightBayou_Middle_0 : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.MidnightBayou_Middle_0.Value;
		Distance = 10f;
		UseColorStyle = BackgroundColorStyle.White;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public override bool CanActive()
	{
		return MidnightBayouBiome.InMidnightBayou(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f);
	}
}
