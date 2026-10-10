namespace Everglow.Myth.Acytaea.Dusts;

public class RedEffect2 : ModDust
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
		return Main.rand.NextBool(50) ? new Color?(new Color(dust.scale / 0.7f, 0, 0, 1 - dust.scale)) : new Color?(new Color(1f, 1f, 1f, 0));
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += 0.1f;
		dust.scale *= 0.96f;
		dust.velocity *= 0.95f;
		Lighting.AddLight(dust.position, dust.scale * 2, 0f, 0f);
		if (dust.scale < 0.15f)
		{
			dust.active = false;
		}

		return false;
	}
}
