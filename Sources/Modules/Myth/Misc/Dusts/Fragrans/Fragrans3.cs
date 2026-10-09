namespace Everglow.Myth.Misc.Dusts.Fragrans;

public class Fragrans3 : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 16, 16);
		// If our texture had 3 different dust on top of each other (a 30x90 pixel image), we might do this:
		// dust.frame = new Rectangle(0, Main.rand.Next(3) * 15, 15, 15);
		dust.alpha = 0;
		dust.rotation = 0;
	}

	public override bool Update(Dust dust)
	{
		// Move the dust based on its velocity and reduce its size to then remove it, as the 'return false;' at the end will prevent vanilla logic.
		dust.position += dust.velocity;
		dust.velocity += new Vector2(0, 0.015f).RotatedByRandom(MathHelper.Pi * 2d);
		dust.velocity *= 0.95f;
		dust.scale *= 0.96f;
		Lighting.AddLight(dust.position, 0.6196f * dust.scale / 1.8f, 0.4855f * dust.scale / 1.8f, 0.0758f * dust.scale / 1.8f);
		if (dust.scale < 0.07f)
		{
			dust.active = false;
		}
		/*if(Main.tile[(int)((dust.position.X + dust.velocity.X * 6) / 16f), (int)((dust.position.Y + dust.velocity.Y * 6) / 16f)].CollisionType == 0)
	{
		dust.velocity *= -1;
	}
	if (Main.tile[(int)((dust.position.X + dust.velocity.X * 6) / 16f), (int)((dust.position.Y + dust.velocity.Y * 6) / 16f)].CollisionType == 1)
	{
		dust.velocity.X *= -1;
	}
	if (Main.tile[(int)((dust.position.X + dust.velocity.X * 6) / 16f), (int)((dust.position.Y + dust.velocity.Y * 6) / 16f)].CollisionType == 2)
	{
		dust.velocity.Y *= -1;
	}*/
		return false;
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0f));
	}
}
