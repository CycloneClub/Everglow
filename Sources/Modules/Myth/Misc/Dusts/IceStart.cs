namespace Everglow.Myth.Misc.Dusts;

public class IceStart : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, Main.rand.Next(3) * 15, 15, 15);
		// If our texture had 3 different dust on top of each other (a 30x90 pixel image), we might do this:
		// dust.frame = new Rectangle(0, 0, 15, 15);
		// dust.alpha = 60;
	}

	public override bool Update(Dust dust)
	{
		// Move the dust based on its velocity and reduce its size to then remove it, as the 'return false;' at the end will prevent vanilla logic.
		dust.alpha += 10;

		if (dust.alpha > 245)
			// dust.color = new Color(1f,1f,1f, dust.alpha / 255f);
			dust.active = false;
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
}
