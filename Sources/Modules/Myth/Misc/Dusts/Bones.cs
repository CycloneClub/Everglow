namespace Everglow.Myth.Misc.Dusts;

public class Bones : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, Main.rand.Next(3) * 15, 15, 15);
		dust.alpha = 0;
		dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.velocity += new Vector2(0, 0.15f).RotatedByRandom(MathHelper.Pi * 2d);
		dust.velocity *= 0.98f;
		dust.scale *= 0.9f;

		if (dust.scale < 0.3f)
		{
			dust.active = false;
		}

		return false;
	}
}
