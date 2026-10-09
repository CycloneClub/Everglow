namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class CrimsonBlood : ModDust
{
	public override bool Update(Dust dust)
	{
		dust.scale *= 0.99f;
		dust.velocity.Y += 0.25f;
		if (Collision.SolidCollision(dust.position, 0, 0))
		{
			dust.active = false;
		}

		return true;
	}
}
