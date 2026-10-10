namespace Everglow.Myth.Misc.Dusts;

public class Bones2 : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = false;
		dust.frame = new Rectangle(0, Main.rand.Next(3) * 15, 15, 15);
		dust.alpha = 0;
		dust.dustIndex = Main.rand.Next(100);
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.velocity.Y += 0.2f;
		dust.velocity *= 0.98f;
		dust.rotation += (dust.dustIndex - 50f) / 600f / dust.scale;
		if (TileUtils.PlatformCollision(dust.position + new Vector2(dust.velocity.X, 0) + new Vector2(4)))
		{
			dust.velocity.X *= -0.5f;
			dust.position += dust.velocity;
			dust.dustIndex = Main.rand.Next(100);
		}
		if (TileUtils.PlatformCollision(dust.position + new Vector2(0, dust.velocity.Y) + new Vector2(4)))
		{
			dust.velocity.Y *= -0.5f;
			dust.position += dust.velocity;
			dust.dustIndex = Main.rand.Next(100);
		}
		if (dust.velocity.Length() < 1)
		{
			dust.alpha += 15;
			if (dust.alpha > 245)
			{
				dust.active = false;
			}
		}

		return false;
	}
}
