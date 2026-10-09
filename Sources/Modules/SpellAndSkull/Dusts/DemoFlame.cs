namespace Everglow.SpellAndSkull.Dusts;

public class DemoFlame : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.velocity *= 0.1f;
		dust.noGravity = true;
		dust.noLight = false;
		dust.scale *= 1f;
		dust.alpha = 0;
	}

	public override bool Update(Dust dust)
	{
		dust.fadeIn -= 0.3f;
		dust.position += dust.velocity;
		dust.rotation += 0.4f;
		dust.velocity *= 0.99f;
		dust.scale *= 0.995f;

		// Lighting.AddLight(dust.position, 1f * dust.scale, 0.28f * dust.scale, 0.68f);
		if (dust.scale < 0.15f)
			dust.active = false;
		if (dust.fadeIn < 0)
			dust.active = false;

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
					v0 = dust.velocity * 0.9f;

					break;
				}
			}
			dust.alpha = 120;
			dust.velocity = v0;
		}
		if (dust.alpha == 120)
			dust.velocity *= 0.9f;
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float k0 = dust.fadeIn / 12f;
		float k1 = 0.3f;
		if (k0 > k1)
		{
			float k2 = (k0 - k1) / (1 - k1);
			float k3 = Math.Min(k2 * k2 * 1.6f, 0.4f);
			return new Color?(new Color(k3, 0, (float)Math.Sqrt(k2) * 2, 1 - k2));
		}
		else
		{
			return new Color?(new Color(0, 0, 0, k0 / k1));
		}
	}
}
