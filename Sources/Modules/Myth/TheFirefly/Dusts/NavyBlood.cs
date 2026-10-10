namespace Everglow.Myth.TheFirefly.Dusts;

public class NavyBlood : ModDust
{
	// private float Ome = 0;
	public override void OnSpawn(Dust dust)
	{
	}

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
