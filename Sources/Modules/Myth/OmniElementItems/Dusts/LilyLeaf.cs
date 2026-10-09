namespace Everglow.Myth.OmniElementItems.Dusts;

public class LilyLeaf : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.noLight = false;
		dust.scale *= 1f;
		dust.alpha = 0;
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += 0.1f;
		dust.scale *= 0.98f;
		dust.velocity *= 0.95f;
		dust.velocity = dust.velocity.RotatedBy(0.015f / dust.scale + Math.Sin(Main.time / 100f) * 0.003f);
		if (dust.scale < 0.15f)
		{
			dust.active = false;
		}

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(155, 155, 155, 120);
	}
}
