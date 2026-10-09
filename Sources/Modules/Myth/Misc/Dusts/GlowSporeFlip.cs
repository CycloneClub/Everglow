namespace Everglow.Myth.Misc.Dusts;

public class GlowSporeFlip : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.alpha = 0;
		dust.rotation = 0;
	}
	public override bool Update(Dust dust)
	{
		// Move the dust based on its velocity and reduce its size to then remove it, as the 'return false;' at the end will prevent vanilla logic.
		dust.position += dust.velocity;
		dust.velocity = dust.velocity.RotatedBy(-0.03f * Main.rand.NextFloat(0.75f, 1.25f));
		dust.velocity *= 0.98f;
		dust.scale *= 0.98f;
		Lighting.AddLight(dust.position, 0.5296f * dust.scale / 1.8f, 0.5855f * dust.scale / 1.8f, 0.0758f * dust.scale / 1.8f);
		if (Main.rand.NextBool(3))
		{
			int type = dust.type;
			if (Main.rand.Next(100) > 50)
			{
				type = ModContent.DustType<GlowSpore>();
			}

			int r1 = Dust.NewDust(dust.position, 0, 0, type, 0, 0, 200, default, dust.scale * 0.75f);
			Main.dust[r1].velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(0.2f, 1.3f));
			Main.dust[r1].noGravity = true;
			int r2 = Dust.NewDust(dust.position, 0, 0, type, 0, 0, 200, default, dust.scale * 0.75f);
			Main.dust[r2].velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-1.3f, -0.2f));
			Main.dust[r2].noGravity = true;
			// dust.active = false;
		}
		if (dust.scale < 0.01f)
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
