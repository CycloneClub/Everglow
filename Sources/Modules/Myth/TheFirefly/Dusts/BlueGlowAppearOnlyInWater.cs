namespace Everglow.Myth.TheFirefly.Dusts;

public class BlueGlowAppearOnlyInWater : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 8, 8);
		dust.alpha = 0;
		dust.rotation = dust.scale * 0.3f; // 用旋转角度存尺寸极值
	}

	public override bool Update(Dust dust)
	{
		Point worldCoordAddY = new Point((int)((dust.position.X + 4) / 16f), (int)((dust.position.Y - 12 + dust.velocity.Y) / 16f));
		if (Main.tile[worldCoordAddY.X, worldCoordAddY.Y].liquid <= 1)
		{
			dust.velocity.Y *= -1;
		}
		Point worldCoordAddX = new Point((int)((dust.position.X + 4 + dust.velocity.X) / 16f), (int)((dust.position.Y - 12) / 16f));
		if (Main.tile[worldCoordAddX.X, worldCoordAddX.Y].liquid <= 1)
		{
			dust.velocity.X *= -1;
		}
		Point worldCoordNow = new Point((int)((dust.position.X + 4) / 16f), (int)((dust.position.Y - 12) / 16f));
		if (Main.tile[worldCoordNow.X, worldCoordNow.Y].liquid <= 1)
		{
			dust.alpha += 150;
		}
		dust.alpha += 3;
		dust.position += dust.velocity;
		dust.velocity += new Vector2(0, 0.015f).RotatedByRandom(MathHelper.Pi * 2d);
		dust.velocity *= 0.95f;
		dust.scale = (float)Math.Sin(dust.alpha / 255d * Math.PI) * dust.rotation;
		Lighting.AddLight(dust.position, 0.0096f * dust.scale / 1.8f, 0.0955f * dust.scale / 1.8f, 0.4758f * dust.scale / 1.8f);
		if (dust.alpha > 254)
			dust.active = false;

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0f));
	}
}
