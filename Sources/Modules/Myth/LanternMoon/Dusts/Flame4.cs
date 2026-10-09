namespace Everglow.Myth.LanternMoon.Dusts;

public class Flame4 : ModDust
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
		dust.rotation += 0.1f;
		dust.scale *= 0.98f;
		dust.velocity *= 0.97f;
		dust.velocity += new Vector2(Main.windSpeedCurrent / 14f * (float)Main.rand.NextFloat(0.85f, 1.15f), -0.1f);
		Lighting.AddLight(dust.position, dust.color.R * 0.0005f, dust.color.G * 0.0005f, dust.color.B * 0.0005f);
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}

		return false;
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		if (dust.scale > 0.8f)
		{
			return new Color?(new Color(1, 0.5f, 0f, 0.1f));
		}
		else
		{
			return new Color?(new Color(dust.scale / 0.8f, dust.scale * dust.scale / 1.28f, 0, (0.8f - dust.scale) / 0.8f * 0.9f + 0.1f));
		}
	}
}
