namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class StoneCageOfChallengesClose : StoneCageOfChallengesBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.StoneCageOfChallengesClose.Value;
		Distance = 12f;
	}
}
