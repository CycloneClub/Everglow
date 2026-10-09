namespace Everglow.Minortopography.GiantPinetree.Dusts;

public class IceParticle : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 9);
		dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
	}

	public override bool Update(Dust dust)
	{
		dust.rotation += dust.velocity.X * 0.3f;
		dust.velocity.X += Main.rand.NextFloat(-0.07f, 0.07f) + Main.windSpeedCurrent / 30f;
		dust.velocity.Y += 0.14f;
		if (Math.Abs(dust.velocity.X) > 1.7f)
		{
			dust.velocity.X *= 0.98f;
		}

		dust.position += dust.velocity;

		if (dust.alpha > 245)
		{
			dust.active = false;
		}

		if (Collision.SolidCollision(dust.position - Vector2.One * 5f + new Vector2(dust.velocity.X, 0), 10, 10))
		{
			dust.alpha += 5;
			dust.velocity.X *= -0.6f;
			dust.velocity *= Main.rand.NextFloat(0.4f, 0.9f);
		}
		if (Collision.SolidCollision(dust.position - Vector2.One * 5f + new Vector2(0, dust.velocity.Y), 10, 10))
		{
			dust.alpha += 5;
			dust.velocity.Y *= -0.6f;
			dust.velocity *= Main.rand.NextFloat(0.4f, 0.9f);
		}
		if (Collision.SolidCollision(dust.position - Vector2.One * 5f, 10, 10))
		{
			dust.alpha += 5;
			dust.velocity *= 0.95f;
		}
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return lightColor * ((255 - dust.alpha) / 255f);
	}
}
