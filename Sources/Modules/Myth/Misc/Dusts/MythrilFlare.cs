namespace Everglow.Myth.Misc.Dusts;

public class MythrilFlare : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.noLight = false;
		dust.scale *= 1f;
		dust.alpha = 0;
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += 0.6f;
		dust.scale *= 0.98f;
		dust.velocity *= 0.99f;

		if (Collision.SolidCollision(dust.position, 0, 0))
		{
			Vector2 v0 = dust.velocity;
			int t = 0;
			while (Collision.SolidCollision(dust.position + v0, 0, 0))
			{
				t++;
				v0 = v0.RotatedByRandom(6.283);
				if (t > 10)
				{
					v0 = dust.velocity * 0.9f;
					break;
				}
			}
			dust.velocity = v0;
			dust.scale *= 0.9f;
		}
		else
		{
			dust.velocity += new Vector2(0, 0.005f * dust.scale * dust.scale);
		}
		if (dust.scale < 0.15f)
		{
			dust.active = false;
		}

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(0.1f, 0.7f, 0.6f, 0.4f);
	}
}
