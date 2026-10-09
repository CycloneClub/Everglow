namespace Everglow.Myth.TheTusk.Dusts;

public class TuskBreak : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, Main.rand.Next(8) * 22, 20, 22);
		dust.alpha = 0;
		dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
	}
	public override bool Update(Dust dust)
	{
		if (dust.fadeIn == 0)
		{
			dust.fadeIn = Main.rand.NextFloat(-0.019f, 0.019f);
		}

		dust.position += dust.velocity;

		if (dust.alpha > 245)
		{
			dust.active = false;
		}

		if (Collision.SolidCollision(dust.position + new Vector2(10) + new Vector2(dust.velocity.X, 0), 0, 0))
		{
			if (dust.velocity.Length() >= 1)
			{
				dust.velocity.X *= Main.rand.NextFloat(-0.7f, -0f);
				dust.position += dust.velocity * 2f;
			}
			else
			{
				dust.velocity *= 0;
			}
		}
		if (Collision.SolidCollision(dust.position + new Vector2(10) + new Vector2(0, dust.velocity.Y), 0, 0))
		{
			if (dust.velocity.Length() >= 1)
			{
				dust.velocity.Y *= Main.rand.NextFloat(-0.7f, -0f);
				dust.position += dust.velocity * 2f;
			}
			else
			{
				dust.velocity *= 0;
			}
		}
		else
		{
			dust.rotation += dust.fadeIn;
			dust.velocity.Y += 0.15f;
		}
		if (dust.velocity.Length() < 0.03f)
		{
			dust.alpha += 2;
		}

		return false;
	}
}
