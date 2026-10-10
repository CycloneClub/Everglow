namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class LeafMagic : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.customData = Main.rand.NextFloat(0f, 20f);
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
		if ((float)dust.customData < 60f)
		{
			dust.frame = new Rectangle(0, 0, 8, 8);
		}
		else if ((float)dust.customData < 75f)
		{
			dust.frame = new Rectangle(0, 10, 8, 8);
		}
		else
		{
			dust.frame = new Rectangle(0, 20, 8, 8);
		}
		if ((float)dust.customData > 90f)
		{
			dust.active = false;
		}
		dust.velocity *= 0.85f;
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
		float value = (float)dust.customData / 60f;
		if ((float)dust.customData > 75f)
		{
			value = ((float)dust.customData - 75f) / 15f;
			var lightColor = Lighting.GetColor(dust.position.ToTileCoordinates()).ToVector4();
			var baseColor = new Color(107, 84, 70, 250).ToVector4();
			lightColor *= baseColor;
			return Color.Lerp(new Color(lightColor.X, lightColor.Y, lightColor.Z, 250), new Color(0, 0, 0, 0), value);
		}
		else if ((float)dust.customData > 60f)
		{
			value = ((float)dust.customData - 60f) / 15f;
			var lightColor = Lighting.GetColor(dust.position.ToTileCoordinates()).ToVector4();
			var baseColor = new Color(107, 84, 70, 250).ToVector4();
			lightColor *= baseColor;
			return Color.Lerp(new Color(114, 88, 43, 0), new Color(lightColor.X, lightColor.Y, lightColor.Z, 250), value);
		}
		else
		{
			return Color.Lerp(new Color(0, 255, 28, 0), new Color(114, 88, 43, 0), value);
		}
	}
}
