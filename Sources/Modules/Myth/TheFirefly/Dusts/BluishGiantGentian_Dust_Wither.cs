namespace Everglow.Myth.TheFirefly.Dusts;

public class BluishGiantGentian_Dust_Wither : ModDust
{
	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.velocity *= 0.99f;
		dust.velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
		dust.scale *= 0.995f;
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}
		if (Collision.SolidCollision(dust.position + new Vector2(dust.velocity.X, 0), 8, 8))
		{
			dust.velocity.X *= -1;
			dust.velocity *= 0.4f;
			dust.scale *= 0.9f;
		}
		if (Collision.SolidCollision(dust.position + new Vector2(0, dust.velocity.Y), 8, 8))
		{
			dust.velocity.Y *= -1;
			dust.velocity *= 0.4f;
			dust.scale *= 0.9f;
		}
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(0f, 0.9f, 1f, 0);
}
