namespace Everglow.Myth.TheTusk.Dusts;

public class BloodBall : ModDust
{
	public override void OnSpawn(Dust dust)
	{
	}

	public override bool Update(Dust dust)
	{
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}

		dust.scale *= 0.98f;
		dust.position += dust.velocity;
		return false;
	}
}
