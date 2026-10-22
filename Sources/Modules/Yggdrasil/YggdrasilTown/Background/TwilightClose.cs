namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class TwilightClose : TwilightBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.TwilightClose.Value;
		Distance = 5f;
	}
}
