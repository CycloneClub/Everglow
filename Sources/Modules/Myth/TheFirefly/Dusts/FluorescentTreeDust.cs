namespace Everglow.Myth.TheFirefly.Dusts;

public class FluorescentTreeDust : ModDust
{
	public override bool Update(Dust dust)
	{
		return true;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		if (dust.frame.Y == 0)
		{
			return new Color(255, 255, 255, 0);
		}

		return base.GetAlpha(dust, lightColor);
	}
}
