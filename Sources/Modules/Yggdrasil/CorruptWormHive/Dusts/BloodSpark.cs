namespace Everglow.Yggdrasil.CorruptWormHive.Dusts;

public class BloodSpark : ModDust
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
		dust.rotation += 1f;
		dust.velocity *= 0.95f;
		dust.alpha += 6;
		if (dust.alpha > 254)
		{
			dust.active = false;
		}

		dust.scale = (float)(-Math.Cos(dust.alpha / 127.5 * Math.PI) + 1) * dust.color.R / 200f;
		Lighting.AddLight(dust.position, dust.scale / dust.color.R * 100f, 0, 0);
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(0.4f, 0f, 0f, 0.9f);
	}
}
