namespace Everglow.Myth.TheTusk;

public class TuskModPlayer : ModPlayer
{
	public int Shake;
	public int MinaShake;
	public float ShakeStrength;

	public float screenShake;

	public static void ScreenShake(float i, Vector2 center, int dis = 2000)
	{
		if (Main.netMode == NetmodeID.Server)
		{
			return;
		}

		foreach (Player p in Main.player)
		{
			if ((p.Center - center).Length() < dis)
			{
				p.GetModPlayer<TuskModPlayer>().screenShake = i;
			}
		}
	}

	public static void ScreenShake(Player p, float i)
	{
		p.GetModPlayer<TuskModPlayer>().screenShake = i;
	}

	public override void ModifyScreenPosition()
	{
		if (screenShake > 0)
		{
			Main.screenPosition += Main.rand.NextVector2Unit() * screenShake;
			screenShake--;
		}
		else
		{
			screenShake = 0;
		}
	}
}
