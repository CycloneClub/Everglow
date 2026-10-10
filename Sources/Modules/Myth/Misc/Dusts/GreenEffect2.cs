namespace Everglow.Myth.Misc.Dusts;

public class GreenEffect2 : ModDust
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
		if (Main.rand.Next(100) > 2)
		{
			return new Color?(new Color(dust.scale / 4.7f, dust.scale / 1.2f, dust.scale / 3f, 1 - dust.scale));
		}
		else
		{
			return new Color?(new Color(1f, 1f, 1f, 0));
		}
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += 0.1f;
		dust.scale *= 0.96f;
		dust.velocity *= 0.95f;
		float scale = dust.scale;
		Lighting.AddLight(dust.position, 0f, dust.scale * 2, 0f);
		if (dust.scale < 0.15f)
		{
			dust.active = false;
		}

		return false;
	}
}
