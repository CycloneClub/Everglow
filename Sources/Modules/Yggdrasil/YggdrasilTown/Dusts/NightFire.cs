namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class NightFire : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.customData = Main.rand.NextFloat(0f, 4f);
	}

	public override bool Update(Dust dust)
	{
		if (dust.customData is float)
		{
			dust.customData = (float)dust.customData + 1;
		}
		if (dust.scale < 0.3f)
		{
			dust.active = false;
		}
		Lighting.AddLight(dust.position + new Vector2(4), GetColor(dust).ToVector3() * dust.scale * 0.5f);
		dust.position += dust.velocity;
		if ((float)dust.customData > 10f)
		{
			dust.scale += 0.003f;
			dust.velocity += new Vector2(0, 0.1f).RotatedBy(Noise(dust));
		}
		if ((float)dust.customData > 70f)
		{
			dust.active = false;
		}
		dust.velocity *= 0.75f;
		return false;
	}

	public float Noise(Dust dust)
	{
		float value = 0;
		for (int i = 0; i < 8; i++)
		{
			value += MathF.Sin(dust.position.X * 0.05f * MathF.Pow(2, i) + (float)Main.time * 0.02f * i % 3) / MathF.Pow(2, i);
			value += MathF.Sin(dust.position.Y * 0.05f * MathF.Pow(2, i) - (float)Main.time * 0.03f * i % 5) / MathF.Pow(2, i);
		}
		return value;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return GetColor(dust);
	}

	public Color GetColor(Dust dust)
	{
		float value = (float)dust.customData / 12f;
		if ((float)dust.customData > 20f)
		{
			value = ((float)dust.customData - 20f) / 50f;
			return Color.Lerp(new Color(37, 81, 114, 250), new Color(0, 0, 0, 0), value);
		}
		else if ((float)dust.customData > 12f)
		{
			value = ((float)dust.customData - 12f) / 8f;
			return Color.Lerp(new Color(27, 146, 186, 80), new Color(37, 81, 114, 250), value);
		}
		else
		{
			return Color.Lerp(new Color(227, 255, 153, 0), new Color(27, 146, 186, 80), value);
		}
	}
}
