namespace Everglow.Myth.TheFirefly.Dusts;

public class FireButterflyShimmer : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.noLight = false;
		dust.scale *= 0.5f;
		dust.alpha = 0;
		dust.frame = new Rectangle(0, 0, 8, 9);
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += 0.6f;
		if (dust.color.R > 30)
		{
			dust.color.R -= 10;
			dust.scale += 0.05f;
		}
		if (Main.rand.NextBool(6))
		{
			if (dust.frame.Y < 18)
			{
				dust.frame.Y += 9;
			}
			else
			{
				dust.frame.Y = 0;
			}
		}
		if (Collision.SolidCollision(dust.position, 8, 8))
		{
			Vector2 v0 = dust.velocity;
			int T = 0;
			while (Collision.SolidCollision(dust.position + v0, 8, 8))
			{
				T++;
				v0 = v0.RotatedByRandom(6.283);
				if (T > 10)
				{
					v0 *= -1;
					break;
				}
			}
			dust.velocity = v0;
		}
		dust.scale *= 0.98f;
		dust.velocity *= 0.94f;
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		if (dust.frame.Y == 9)
		{
			return new Color(60, 120, 255, 255) * Main.rand.NextFloat(0.1f, 10f);
		}
		if (dust.frame.Y == 18)
		{
			return new Color(45, 155, 255, 60) * Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.1f, 20f));
		}
		return lightColor;
	}
}
