using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class Town_Sky : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.Town_Sky.Value;
		Distance = float.PositiveInfinity;
		UseColorStyle = 2;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override bool CanActive()
	{
		return YggdrasilTownCentralSystem.InYggdrasilTown(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f);
	}
}
