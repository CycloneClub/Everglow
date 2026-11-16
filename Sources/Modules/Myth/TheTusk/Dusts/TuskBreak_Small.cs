namespace Everglow.Myth.TheTusk.Dusts;

public class TuskBreak_Small : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
		base.OnSpawn(dust);
	}

	public override bool Update(Dust dust)
	{
		if (Collision.SolidCollision(dust.position + new Vector2(4) + new Vector2(dust.velocity.X, 0), 0, 0))
		{
			if (dust.velocity.Length() >= 1)
			{
				dust.velocity.X *= -Main.rand.NextFloat(0f, 0.7f);
				dust.position += dust.velocity * 2f;
			}
			else
			{
				dust.velocity *= 0;
			}
		}
		if (Collision.SolidCollision(dust.position + new Vector2(4) + new Vector2(0, dust.velocity.Y), 0, 0))
		{
			if (dust.velocity.Length() >= 1)
			{
				dust.velocity.Y *= -Main.rand.NextFloat(0f, 0.7f);
				dust.position += dust.velocity * 2f;
			}
			else
			{
				dust.velocity *= 0;
			}
		}
		return base.Update(dust);
	}
}
