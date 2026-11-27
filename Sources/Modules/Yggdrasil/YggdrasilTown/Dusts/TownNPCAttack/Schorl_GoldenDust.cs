namespace Everglow.Yggdrasil.YggdrasilTown.Dusts.TownNPCAttack;

public class Schorl_GoldenDust : ModDust
{
	public override bool Update(Dust dust)
	{
		dust.velocity = dust.velocity.RotatedBy(Noise(dust) * 0.2f);
		dust.velocity *= 0.99f;
		dust.scale *= 0.95f;
		dust.position += dust.velocity;
		if (dust.scale < 0.1f)
		{
			dust.active = false;
		}
		Lighting.AddLight(dust.position + new Vector2(4), new Vector3(0.4f * dust.scale, 0.44f * dust.scale, 0));
		return false;
	}

	public float Noise(Dust dust)
	{
		float value = 0;
		for (int i = 0; i < 8; i++)
		{
			float x0 = dust.position.X - dust.velocity.X * 10;
			float y0 = dust.position.Y - dust.velocity.Y * 10;
			value += MathF.Sin(x0 * 0.5f * MathF.Pow(2, i) + (float)Main.timeForVisualEffects * 0.02f * i % 3) / MathF.Pow(2, i);
			value -= MathF.Sin(y0 * 0.5f * MathF.Pow(2, i) - (float)Main.timeForVisualEffects * 0.03f * i % 5) / MathF.Pow(2, i);
		}
		return value;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(1f, 1f, 1f, 0f);
	}
}
