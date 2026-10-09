namespace Everglow.Myth.Misc.Dusts;

public class XiaoDustCyan : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.alpha = 0;
		dust.rotation = 0;
	}
	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-0.0075f, 0.0075f));
		dust.velocity *= 0.9f;
		dust.scale *= 0.9f;
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}

		return false;
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(255, 255, 255, 150);
	}
}
