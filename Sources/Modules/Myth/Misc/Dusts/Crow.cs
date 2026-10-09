namespace Everglow.Myth.Misc.Dusts;

public class Crow : ModDust
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
		dust.position += dust.velocity;
		dust.rotation += 0.4f;
		dust.velocity *= 0.99f;
		dust.scale *= 0.9f;
		float scale = dust.scale;
		// Lighting.AddLight(dust.position, 1f * dust.scale, 0.28f * dust.scale, 0.68f);
		if (dust.scale < 0.15f)
			dust.active = false;
		return false;
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		if (dust.scale > 1.5f)
			return new Color?(new Color(255, 255, 255, 255));
		else
		{
			return new Color?(new Color((dust.scale - 0.25f) / 1.25f, (dust.scale - 0.25f) / 1.25f, (dust.scale - 0.25f) / 1.25f, (dust.scale - 0.25f) / 1.25f));
		}
	}
}
