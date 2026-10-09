namespace Everglow.Myth.Misc.Dusts.Slingshots;

public abstract class GemDust : ModDust
{
	internal Color dustColor = Color.White;
	public override void SetStaticDefaults()
	{
	}
	public override string Texture => "Everglow/Myth/Misc/Dusts/Slingshots/GemDust";
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.velocity *= 0;
		dust.color.A = (byte)Main.rand.Next(255); // 透明度存角速度
		dust.color.R = (byte)Main.rand.Next(120, 255); // 红度存黑化率
		SetDef();
	}
	public virtual void SetDef()
	{
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		if (Main.rand.NextBool(50))
		{
			dust.alpha = 1;
			return new Color?(new Color(1f, 1f, 1f, 0));
		}
		else
		{
			dust.alpha = 0;
			return new Color?(new Color(dust.scale / 0.7f * dustColor.R / 255f, dust.scale / 0.7f * dustColor.G / 255f, dust.scale / 0.7f * dustColor.B / 255f, 1 * dust.color.R / 155f - dust.scale));
		}
	}
	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += (dust.color.A - 127.5f) / 255f;
		dust.scale *= 0.96f;
		dust.velocity *= 0.95f;
		if (dust.alpha == 0)
			Lighting.AddLight(dust.position, dust.scale / 0.7f * dustColor.R / 255f, dust.scale / 0.7f * dustColor.G / 255f, dust.scale / 0.7f * dustColor.B / 255f);
		else
		{
			Lighting.AddLight(dust.position, dust.scale * 0.5f, dust.scale * 0.5f, dust.scale * 0.5f);
		}
		if (dust.scale < 0.15f)
		{
			dust.active = false;
		}

		return false;
	}
}
