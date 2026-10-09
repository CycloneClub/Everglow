namespace Everglow.Myth.Misc.Dusts.Slingshots;

public class LittleJungleSpore : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.alpha = 0;
		dust.rotation = 0;
		dust.color.R = (byte)Main.rand.Next(2);
	}
	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.velocity = dust.velocity.RotatedBy(0.06f * Main.rand.NextFloat(0.75f, 1.25f) * (dust.color.R - 0.5f));
		dust.velocity *= 0.98f;
		dust.scale *= 0.98f;
		Lighting.AddLight(dust.position, 0.5296f * dust.scale / 1.8f, 0.5855f * dust.scale / 1.8f, 0.0758f * dust.scale / 1.8f);
		if (Main.rand.NextBool(3))
		{
			int type = dust.type;
			int r1 = Dust.NewDust(dust.position, 0, 0, type, 0, 0, 200, default, dust.scale * 0.75f);
			Main.dust[r1].velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(0.2f, 1.3f));
			Main.dust[r1].noGravity = true;
			int r2 = Dust.NewDust(dust.position, 0, 0, type, 0, 0, 200, default, dust.scale * 0.75f);
			Main.dust[r2].velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-1.3f, -0.2f));
			Main.dust[r2].noGravity = true;
		}
		if (Collision.SolidCollision(dust.position, 8, 8))
		{
			Vector2 v0 = dust.velocity;
			int T = 0;
			while (Collision.SolidCollision(dust.position + v0, 8, 8))
			{
				T++;
				v0 = v0.RotatedByRandom(6.283);
				if (T > 10)
				{
					v0 *= -1;
					break;
				}
			}
			dust.velocity = v0;
		}
		if (dust.scale < 0.01f)
		{
			dust.active = false;
		}

		return false;
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0f));
	}
}
