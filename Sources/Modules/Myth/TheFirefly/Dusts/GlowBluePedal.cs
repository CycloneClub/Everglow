namespace Everglow.Myth.TheFirefly.Dusts;

public class GlowBluePedal : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, Main.rand.Next(3) * 8, 8, 8);
		dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
		dust.scale = Main.rand.NextFloat(0.65f, 1.0f);
	}

	public override bool Update(Dust dust)
	{
		if (dust.velocity.Y > -0.12f)
		{
			dust.velocity.Y += 0.01f;
		}

		if (dust.velocity.Y < -0.3f)
		{
			dust.velocity.Y *= 0.98f;
		}

		dust.rotation += Main.rand.NextFloat(-0.17f, 0.17f);
		dust.velocity.X += Main.rand.NextFloat(-0.07f, 0.07f) + Main.windSpeedCurrent / 30f;
		dust.velocity.Y += Main.rand.NextFloat(-0.01f, 0.03f);
		if (Math.Abs(dust.velocity.X) > 1.7f)
		{
			dust.velocity.X *= 0.98f;
		}

		dust.position += dust.velocity;

		if (dust.alpha > 245)
		{
			dust.active = false;
		}

		if (dust.scale < 0.1f)
		{
			dust.active = false;
		}

		if (Collision.SolidCollision(dust.position - Vector2.One * 5f, 10, 10) && dust.fadeIn == 0f)
		{
			dust.scale *= 0.93f;
			dust.alpha += 5;
			dust.velocity *= 0.25f;
		}
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(150, 150, 255, 0);
	}
}
