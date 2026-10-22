namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class TwilightMiddleFar : TwilightBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.TwilightMiddleFar.Value;
		Distance = 30f;
	}
}
