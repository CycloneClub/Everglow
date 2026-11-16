namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class LampWood_Dust_Fluorescent_Appear : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 8, 8);
		dust.alpha = 0;
		dust.scale = 0;
		dust.rotation = dust.scale * 0.3f; // 用旋转角度存尺寸极值
	}

	public override bool Update(Dust dust)
	{
		dust.alpha += 3;
		dust.position += dust.velocity;
		dust.velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
		dust.velocity *= 0.96f;
		dust.velocity.Y -= 0.02f;
		dust.scale = (float)Math.Sin(dust.alpha / 255d * Math.PI) * dust.rotation;
		Lighting.AddLight(dust.position, 0.46f * dust.scale, 0.2955f * dust.scale, 0);
		if (dust.alpha > 254)
		{
			dust.active = false;
		}

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0f));
	}
}
