namespace Everglow.Myth.OmniElementItems.Dusts;

public class GreenParticle : ModDust
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
		dust.velocity *= 0.95f;
		dust.alpha++;
		Lighting.AddLight(dust.position, 0, 0, (float)((255 - dust.alpha) * 0.0015f));
		if (dust.alpha > 254)
		{
			dust.active = false;
		}

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float k = (255 - dust.alpha) / 255f;
		float k2 = (float)Math.Sqrt(k);
		if (dust.rotation > 3.14159)
		{
			if (dust.scale > 0.6f)
				return new Color?(new Color(0f * k * k, 0.9f * k2, 0.1f * k, 0f));
			else
			{
				return new Color?(new Color(0f * k * k, 0.9f * k2, 0.1f * k, 0));
			}
		}
		else
		{
			return new Color?(new Color(1.3f * k2, 1.3f * k2, 1.3f * k2, 0f));
		}
	}
}
