namespace Everglow.Myth.Misc.Dusts;

public class LeafVFX : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.frame = new Rectangle(0, 14 * Main.rand.Next(8), 14, 14);
		dust.color.R = (byte)Main.rand.Next(188);
		dust.color.G = (byte)Main.rand.Next(100);
		dust.noGravity = true;
	}

	public override bool Update(Dust dust)
	{
		dust.scale -= 0.02f;
		int TimeLeft = (int)(dust.scale * 100f);
		if (TimeLeft % 6 == 0)
		{
			if (dust.frame.Y < 98)
				dust.frame.Y += 14;
			else
			{
				dust.frame.Y = 0;
			}
		}
		if (dust.scale < 0.01f)
			dust.active = false;
		dust.color.R += 5; //0.0~2.0
		if (dust.color.R > 255)
			dust.color.R = 0;
		if (dust.scale < 0.8f)
			dust.velocity *= 0.96f;
		dust.velocity = dust.velocity.RotatedBy(Math.PI / 60d * (float)Math.Sin(dust.color.R / 255f * MathHelper.TwoPi));
		dust.position += dust.velocity;
		dust.rotation += (dust.color.G - 50f) / 500f;
		return false;
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(255, 255, 255, 0);
	}
}
