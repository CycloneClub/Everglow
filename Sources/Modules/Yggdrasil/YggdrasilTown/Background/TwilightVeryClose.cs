using Everglow.Commons.Mechanics.CustomBackground;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class TwilightVeryClose : TwilightBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.TwilightVeryClose.Value;
		UseColorStyle = BackgroundColorStyle.Sky;
		Distance = 2f;
	}
}
