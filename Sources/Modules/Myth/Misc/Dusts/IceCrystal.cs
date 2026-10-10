namespace Everglow.Myth.Misc.Dusts;

public class IceCrystal : ModDust
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
		dust.rotation += Main.rand.NextFloat(-0.3f, 0.3f);
		dust.scale *= 0.98f;
		dust.velocity *= 0.99f;
		dust.velocity = dust.velocity.RotatedBy((dust.color.G - 120) / 2550f);
		if (Main.rand.NextBool(6) && !Main.gamePaused)
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
			dust.velocity += new Vector2(Main.windSpeedCurrent * 0.05f, 0.005f * dust.scale * dust.scale);
		}
		if (dust.scale < 0.15f)
		{
			dust.active = false;
		}

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float light = (lightColor.R + lightColor.G + lightColor.B) / 765f;
		Color c0 = Color.Lerp(lightColor, new Color(light, light, light, 1), 0.9f);
		if (dust.frame.Y == 18)
		{
			return c0 * 1.2f;
		}
		if (dust.frame.Y == 9)
		{
			return c0 * 0.4f;
		}
		return c0 * 0.2f;
	}
}

/// <summary>
/// 此种会受到更多的重力影响
/// </summary>
public class IceCrystal2 : ModDust
{
	public override string Texture => "Everglow/" + ModAsset.IceCrystal_Path;

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
		dust.rotation += Main.rand.NextFloat(-0.3f, 0.3f);
		dust.scale *= 0.98f;
		dust.velocity *= 0.99f;
		if (Main.rand.NextBool(6) && !Main.gamePaused)
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
			dust.scale *= 0.6f;
		}
		else
		{
			dust.velocity += new Vector2(Main.windSpeedCurrent * 0.02f, 0.25f * dust.scale * dust.scale);
		}
		if (dust.scale < 0.15f)
		{
			dust.active = false;
		}

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float light = (lightColor.R + lightColor.G + lightColor.B) / 765f;
		Color c0 = Color.Lerp(lightColor, new Color(light, light, light, 1), 0.9f);
		if (dust.frame.Y == 18)
		{
			return c0 * 1.2f;
		}
		if (dust.frame.Y == 9)
		{
			return c0 * 0.4f;
		}
		return c0 * 0.2f;
	}
}
