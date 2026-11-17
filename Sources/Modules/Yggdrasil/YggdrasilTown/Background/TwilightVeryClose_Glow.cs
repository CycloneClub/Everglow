namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class TwilightVeryClose_Glow : TwilightBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.TwilightVeryClose_Glow.Value;
		UseColorStyle = 3;
		Distance = 2f;
	}
}
