namespace Everglow.Myth.TheFirefly.Dusts;

public class BlueEffect : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.velocity *= 1f;
		dust.noGravity = true;
		dust.noLight = true;
		dust.alpha = 0;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color?(new Color(dust.velocity.Length() / 10f, dust.velocity.Length() / 10f, dust.velocity.Length() / 10f, 0));
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += 0.1f;
		dust.scale *= 0.95f;
		dust.velocity *= 0.95f;
		Lighting.AddLight(dust.position, dust.color.R * 0.0f, dust.color.G * 0.0f, 1f);
		if (dust.scale < 0.15f)
		{
			dust.active = false;
		}

		return false;
	}
}
