namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class TwilightFar : TwilightBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.TwilightFar.Value;
		Distance = 40f;
	}
}
