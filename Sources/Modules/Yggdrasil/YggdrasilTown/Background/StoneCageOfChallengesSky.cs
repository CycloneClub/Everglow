namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class StoneCageOfChallengesSky : StoneCageOfChallengesBackgroundBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.StoneCageOfChallengesSky.Value;
		Distance = float.PositiveInfinity;
	}
}
