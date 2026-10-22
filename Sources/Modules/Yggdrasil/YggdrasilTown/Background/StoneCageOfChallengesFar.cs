namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class StoneCageOfChallengesFar : StoneCageOfChallengesBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.StoneCageOfChallengesFar.Value;
		Distance = 30f;
	}
}
