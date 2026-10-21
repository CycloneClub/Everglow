namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class TwilightSky : TwilightBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.TwilightSky.Value;
		Distance = float.PositiveInfinity;
	}
}
