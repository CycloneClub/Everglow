namespace Everglow.Myth.TheFirefly.Dusts;

public class BlueGlowAppear_dark : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 8, 8);
		dust.alpha = 0;
		dust.rotation = dust.scale * 0.4f; // 用旋转角度存尺寸极值
	}

	public override bool Update(Dust dust)
	{
		dust.alpha += 6;
		dust.position += dust.velocity;
		dust.velocity += new Vector2(0, 0.015f).RotatedByRandom(MathHelper.Pi * 2d);
		dust.velocity *= 0.95f;
		dust.scale = (float)Math.Sin(dust.alpha / 255d * Math.PI) * dust.rotation;
		Lighting.AddLight(dust.position, 0.0096f * dust.scale / 1.8f, 0.0955f * dust.scale / 1.8f, 0.4758f * dust.scale / 1.8f);
		if (dust.alpha > 254)
		{
			dust.active = false;
		}

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color?(new Color(15, 15, 255, 150));
	}
}
