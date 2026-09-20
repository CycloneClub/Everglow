namespace Everglow.Yggdrasil;

public class YggdrasilWorldSystem : ModSystem
{
	public override void PostUpdateEverything()
	{
		if (YggdrasilWorld.InYggdrasil)
		{
			YggdrasilWorld.YggdrasilTimer++;

			if (Main.bloodMoon)
			{
				Main.bloodMoon = false;
			}
			if (Main.slimeRain)
			{
				Main.slimeRain = false;
			}
		}
		base.PostUpdateEverything();
	}
}
