namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.IstafelsEffects;

public class IstafelsSunfireDrop_glimmer : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.scale = 0.05f;
	}

	public override bool Update(Dust dust)
	{
		if (dust.customData is Projectile)
		{
			Projectile projectile = dust.customData as Projectile;
			if (projectile is not null)
			{
				dust.position = projectile.Center + (dust.velocity - projectile.velocity).NormalizeSafe() * projectile.width * 0.3f;
				dust.scale += 0.005f;
				Lighting.AddLight(dust.position, new Vector3(1f, 1f, 0.4f) * dust.scale);
				if (dust.scale > 0.6f)
				{
					Vector2 afterVelocity = projectile.velocity;
					float mulScale = Main.rand.NextFloat(4f, 8f);
					var drop = new IstafelsSunfireDrop
					{
						Velocity = afterVelocity,
						Active = true,
						Visible = true,
						Position = dust.position,
						MaxTime = Main.rand.Next(222, 264),
						Scale = mulScale,
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
					};
					Ins.VFXManager.Add(drop);
					dust.active = false;
				}
			}
			else
			{
				dust.active = false;
			}
		}
		else
		{
			dust.active = false;
		}

		return false;
	}

	public float Noise(Dust dust)
	{
		float value = 0;
		for (int i = 0; i < 8; i++)
		{
			value += MathF.Sin(dust.position.X * 0.05f * MathF.Pow(2, i) + (float)Main.time * 0.02f * i % 3) / MathF.Pow(2, i);
			value += MathF.Sin(dust.position.Y * 0.05f * MathF.Pow(2, i) - (float)Main.time * 0.03f * i % 5) / MathF.Pow(2, i);
		}
		return value;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(255, 255, 125, 0);
	}
}
