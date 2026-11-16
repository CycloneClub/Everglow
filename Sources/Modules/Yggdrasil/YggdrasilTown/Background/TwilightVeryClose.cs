namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class TwilightVeryClose : TwilightBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.TwilightVeryClose.Value;
		Distance = 2f;
	}
}
