namespace Everglow.Food.Dusts;

public class PanHitSpark : ModDust
{
	public override bool MidUpdate(Dust dust)
	{
		return true;
	}
	public override bool Update(Dust dust)
	{
		dust.scale *= 0.96f;
		dust.color.R += (byte)Main.rand.Next(-2, 3);
		dust.velocity = dust.velocity.RotatedBy((dust.color.R - 100f) / 100f);
		dust.velocity *= 0.96f;
		dust.position += dust.velocity;
		dust.rotation = dust.velocity.ToRotation();
		if (dust.velocity.Length() > 8)
		{
			dust.frame.X = 18;
		}
		else if (dust.velocity.Length() > 4)
		{
			dust.scale *= 0.94f;
			dust.frame.X = 9;
		}
		else
		{
			dust.scale *= 0.92f;
			dust.frame.X = 0;
		}
		if (Collision.SolidCollision(dust.position, 8, 8))
		{
			dust.scale *= 0.72f;
			dust.velocity *= 0;
		}
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}

		return false;
	}
	public override void OnSpawn(Dust dust)
	{
		dust.color.R = (byte)Main.rand.Next(90, 111);
		base.OnSpawn(dust);
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(dust.scale, dust.scale * dust.scale * 0.5f, dust.scale - 2f, 0);
	}
}
