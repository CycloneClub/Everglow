namespace Everglow.Myth.TheTusk.Dusts;

public class RedBlood : ModDust
{
	// private float Ome = 0;
	public override void OnSpawn(Dust dust)
	{
	}

	public override bool Update(Dust dust)
	{
		dust.scale *= 0.99f;
		dust.velocity.Y += 0.25f;
		if (Main.tile[(int)(dust.position.X / 16f), (int)(dust.position.Y / 16f)].LiquidAmount > 0)
			dust.active = false;
		if (Collision.SolidCollision(dust.position, 0, 0))
			dust.active = false;
		return true;
	}
}
