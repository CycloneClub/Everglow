namespace Everglow.Myth.Misc.Dusts;

public class IceScale : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 15, 15);
		dust.alpha = 0;
		dust.dustIndex = (int)(dust.scale * 300); //用旋转角度存尺寸极值
	}

	public override bool Update(Dust dust)
	{
		dust.alpha += 24;
		dust.position += dust.velocity;
		dust.velocity += new Vector2(0, 0.015f).RotatedByRandom(MathHelper.Pi * 2d);
		dust.velocity *= 0.95f;
		dust.scale = (float)Math.Sin(dust.alpha / 255d * Math.PI) * dust.dustIndex / 300f;
		Lighting.AddLight(dust.position, 0.0096f * dust.scale / 1.8f, 0.0955f * dust.scale / 1.8f, 0.4758f * dust.scale / 1.8f);
		if (dust.alpha > 254)
			dust.active = false;

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float timeValue = dust.alpha / 255f;
		timeValue -= 0.5f;
		timeValue *= 2f;
		float r = 1 - MathF.Pow(MathF.Abs(MathF.Sin(timeValue * MathHelper.Pi / 2f)), 0.3f);
		r *= 0.4f;
		float g = 1 - MathF.Pow(MathF.Abs(MathF.Sin(timeValue * MathHelper.Pi / 2f)), 0.5f);
		g *= 0.2f;
		float b = 1 - MathF.Pow(MathF.Max(MathF.Abs(timeValue) * 2f - 1, 0), 3.5f);
		b *= 0.15f;
		return new Color?(new Color(r, g, b, 0f));
	}
}
public class IceScale2 : ModDust
{
	public override string Texture => "Everglow/Myth/Misc/Dusts/IceScale";
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 15, 15);
		dust.alpha = 0;
		dust.dustIndex = (int)(dust.scale * 300); //用旋转角度存尺寸极值
	}

	public override bool Update(Dust dust)
	{
		dust.alpha += 12;
		dust.position += dust.velocity;
		dust.velocity += new Vector2(0, 0.015f).RotatedByRandom(MathHelper.Pi * 2d);
		dust.velocity *= 0.95f;
		dust.scale = (float)Math.Sin(dust.alpha / 255d * Math.PI) * dust.dustIndex / 300f;
		Lighting.AddLight(dust.position, 0.0096f * dust.scale / 1.8f, 0.0955f * dust.scale / 1.8f, 0.4758f * dust.scale / 1.8f);
		if (dust.alpha > 254)
			dust.active = false;

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float timeValue = dust.alpha / 255f;
		timeValue -= 0.5f;
		timeValue *= 2f;
		float r = 1 - MathF.Pow(MathF.Abs(MathF.Sin(timeValue * MathHelper.Pi / 2f)), 0.3f);
		r *= 0.4f;
		float g = 1 - MathF.Pow(MathF.Abs(MathF.Sin(timeValue * MathHelper.Pi / 2f)), 0.5f);
		g *= 0.4f;
		float b = 1 - MathF.Pow(MathF.Max(MathF.Abs(timeValue) * 2f - 1, 0), 3.5f);
		b *= 0.25f;
		return new Color?(new Color(r, g, b, 0f));
	}
}
public class IceScale3 : ModDust
{
	public override string Texture => "Everglow/Myth/Misc/Dusts/IceScale";
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 15, 15);
		dust.alpha = 0;
		dust.dustIndex = (int)(dust.scale * 300); //用旋转角度存尺寸极值
	}

	public override bool Update(Dust dust)
	{
		dust.alpha += 18;
		dust.position += dust.velocity;
		dust.velocity += new Vector2(0, 0.015f).RotatedByRandom(MathHelper.Pi * 2d);
		dust.velocity *= 0.95f;
		dust.scale = (float)Math.Sin(dust.alpha / 255d * Math.PI) * dust.dustIndex / 300f;
		Lighting.AddLight(dust.position, 0.0096f * dust.scale / 1.8f, 0.0955f * dust.scale / 1.8f, 0.4758f * dust.scale / 1.8f);
		if (dust.alpha > 254)
			dust.active = false;

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float timeValue = dust.alpha / 255f;
		timeValue -= 0.5f;
		timeValue *= 2f;
		float r = 1 - MathF.Pow(MathF.Abs(MathF.Sin(timeValue * MathHelper.Pi / 2f)), 0.3f);
		r *= 0.4f;
		float g = 1 - MathF.Pow(MathF.Abs(MathF.Sin(timeValue * MathHelper.Pi / 2f)), 0.5f);
		g *= 0.4f;
		float b = 1 - MathF.Pow(MathF.Max(MathF.Abs(timeValue) * 2f - 1, 0), 3.5f);
		b *= 0.2f;
		return new Color?(new Color(r, g, b, 0f));
	}
}
