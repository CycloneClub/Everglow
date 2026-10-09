namespace Everglow.Myth.TheFirefly.Dusts;

public class FireflyPylonDust : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.noLight = false;
		dust.scale *= 0.5f;
		dust.alpha = 0;
		dust.velocity.Y = -1f;
	}

	public override bool Update(Dust dust)
	{
		Dust dust2 = dust;
		dust.position += dust.velocity;
		dust2.rotation += 0.1f * dust.scale;
		if (dust.color.R > 30)
		{
			dust.color.R -= 10;
			dust.scale += 0.05f;
		}
		Color color2 = Lighting.GetColor((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f));
		dust.scale *= 0.98f;
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}

		Lighting.AddLight((int)dust.position.X / 16, (int)dust.position.Y / 16, 0, 0, 0.6f * dust.scale);
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color?(new Color(0.5f, 0.5f, 0.5f, 0.5f));
	}
}
