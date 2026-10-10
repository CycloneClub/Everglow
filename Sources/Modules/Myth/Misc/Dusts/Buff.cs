namespace Everglow.Myth.Misc.Dusts;

public class Buff : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 15, 15);
		dust.alpha = 0;
		dust.rotation = 0;
	}

	public override bool Update(Dust dust)
	{
		// Move the dust based on its velocity and reduce its size to then remove it, as the 'return false;' at the end will prevent vanilla logic.
		dust.position += dust.velocity;
		dust.velocity *= 0.95f;
		dust.alpha += 3;

		if (dust.alpha > 253)
		{
			dust.active = false;
		}
		/*if(Main.tile[(int)((dust.position.X + dust.velocity.X * 6) / 16f), (int)((dust.position.Y + dust.velocity.Y * 6) / 16f)].CollisionType == 0)
	{
		dust.velocity *= -1;
	}
	if (Main.tile[(int)((dust.position.X + dust.velocity.X * 6) / 16f), (int)((dust.position.Y + dust.velocity.Y * 6) / 16f)].CollisionType == 1)
	{
		dust.velocity.X *= -1;
	}
	if (Main.tile[(int)((dust.position.X + dust.velocity.X * 6) / 16f), (int)((dust.position.Y + dust.velocity.Y * 6) / 16f)].CollisionType == 2)
	{
		dust.velocity.Y *= -1;
	}*/
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		double Deep = Math.Sqrt((255 - dust.alpha) / 255d) * 2;
		return new Color?(new Color((int)(dust.color.R * Deep), (int)(dust.color.G * Deep), (int)(dust.color.B * Deep), 0));
	}
}
