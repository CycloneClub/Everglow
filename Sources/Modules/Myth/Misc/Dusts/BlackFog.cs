namespace Everglow.Myth.Misc.Dusts;

public class BlackFog : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.noLight = true;
		dust.scale *= 1f;
		dust.rotation = Main.rand.NextFloat((float)Math.PI);
	}
	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += 0.1f;
		dust.scale *= 0.995f;
		dust.velocity.X = (float)(Math.Sin(Main.time / 30d + dust.frame.Y) * 0.3f * dust.scale);
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}

		return false;
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color?(new Color(0f, 0f, 0f, 0.5f));
	}
}
