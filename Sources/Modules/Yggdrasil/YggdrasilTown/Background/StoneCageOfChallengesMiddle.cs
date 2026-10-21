namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class StoneCageOfChallengesMiddle : StoneCageOfChallengesBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.StoneCageOfChallengesMiddle.Value;
		Distance = 20f;
	}
}
