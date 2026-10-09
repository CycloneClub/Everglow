namespace Everglow.Commons.MEAC;

public class MEACPlayer : ModPlayer
{
	public bool isUsingMeleeProj = false;
	public override void PreUpdate()
	{
		if (isUsingMeleeProj)
		{
			Player.itemAnimation = 2;
		}
	}
}
