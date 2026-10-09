namespace Everglow.Myth.Misc.Dusts.Fragrans;

public class Fragrans : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		// dust.frame = new Rectangle(0, 0, 15, 15);
		// If our texture had 3 different dust on top of each other (a 30x90 pixel image), we might do this:
		dust.frame = new Rectangle(0, Main.rand.Next(8) * 14, 12, 14);
		dust.alpha = 0;
		dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
	}

	public override bool Update(Dust dust)
	{
		// Move the dust based on its velocity and reduce its size to then remove it, as the 'return false;' at the end will prevent vanilla logic.
		if (dust.velocity.Y > -0.12f)
		{
			dust.velocity.Y += 0.01f;
		}

		if (dust.velocity.Y < -0.3f)
		{
			dust.velocity.Y *= 0.98f;
		}

		dust.rotation += Main.rand.NextFloat(-0.17f, 0.17f);
		dust.velocity.X += Main.rand.NextFloat(-0.07f, 0.07f) + Main.windSpeedCurrent / 30f;
		dust.velocity.Y += Main.rand.NextFloat(-0.03f, 0.03f);
		if (Math.Abs(dust.velocity.X) > 1.7f)
		{
			dust.velocity.X *= 0.98f;
		}

		dust.position += dust.velocity;

		if ((int)(Main.time / 5d) % 5 == (int)dust.position.X % 5 && !(Collision.SolidCollision(dust.position - Vector2.One * 5f, 10, 10) && dust.fadeIn == 0f))
		{
			if (dust.frame.Y < 98)
			{
				dust.frame.Y += 14;
			}
			else
			{
				dust.frame.Y = 0;
			}
		}

		if (dust.alpha > 245)
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
		if (Collision.SolidCollision(dust.position - Vector2.One * 5f, 10, 10) && dust.fadeIn == 0f)
		{
			dust.alpha += 5;
			dust.velocity *= 0.25f;
		}
		return false;
	}
}
