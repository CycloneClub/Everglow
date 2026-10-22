namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class TwilightMiddle : TwilightBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.TwilightMiddle.Value;
		Distance = 16f;
	}
}
