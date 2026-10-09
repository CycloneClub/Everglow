namespace Everglow.Yggdrasil.KelpCurtain.Dusts;

public class GeyserBudPollen : ModDust
{
	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.velocity.Y -= 0.01f;
		if (dust.customData is float k)
		{
			dust.velocity.X -= (dust.position.X - k) * 0.002f;
		}
		dust.velocity *= 0.99f;
		Lighting.AddLight(dust.position, new Vector3(0.21f, 0.45f, 0.32f) * dust.scale);
		dust.scale *= 0.992f;
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		Color c0 = lightColor;
		c0.A = 120;
		return c0;
	}
}
