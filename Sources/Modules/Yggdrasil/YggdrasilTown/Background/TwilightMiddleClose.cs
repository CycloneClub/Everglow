namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class TwilightMiddleClose : TwilightBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.TwilightMiddleClose.Value;
		Distance = 10f;
	}
}
