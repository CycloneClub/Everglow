using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Minortopography.GiantPinetree.Dusts;
using Terraria.Audio;

namespace Everglow.Minortopography.GiantPinetree.Projectiles;

public class FrostBall : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 360;
		Projectile.penetrate = 1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 2;
	}

	public override void AI()
	{
		Projectile.rotation += 0.3f;

		if (Projectile.Center.X > Main.screenPosition.X - 100 && Projectile.Center.X < Main.screenPosition.X + Main.screenWidth + 100 && Projectile.Center.Y > Main.screenPosition.Y - 100 && Projectile.Center.Y < Main.screenPosition.Y + Main.screenWidth + 100)
		{
			if (Main.rand.NextBool(2))
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 0.6f)).RotatedByRandom(MathHelper.TwoPi);
				var smog = new IceSmogDust
				{
					Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0f, 0.03f),
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
					MaxTime = Main.rand.Next(137, 245),
					Scale = Main.rand.NextFloat(6f, 9f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.005f, 0.005f) },
				};
				Ins.VFXManager.Add(smog);
			}
			else
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 0.6f)).RotatedByRandom(MathHelper.TwoPi);
				var smog = new IceSmogDust2
				{
					Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0f, 0.03f),
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
					MaxTime = Main.rand.Next(137, 245),
					Scale = Main.rand.NextFloat(3f, 15f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.005f, 0.005f) },
				};
				Ins.VFXManager.Add(smog);
			}
			if (Main.rand.NextBool(3))
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 0.6f)).RotatedByRandom(MathHelper.TwoPi);
				var smog = new SnowPieceDust
				{
					Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0f, 0.03f),
					Active = true,
					Visible = true,
					Coord0 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
					Coord1 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
					MaxTime = Main.rand.Next(37, 125),
					Scale = Main.rand.NextFloat(2f, 4f),
					Rotation = Main.rand.NextFloat(6.283f),
					Rotation2 = Main.rand.NextFloat(6.283f),
					Omega = Main.rand.NextFloat(-10f, 10f),
					Phi = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
				};
				Ins.VFXManager.Add(smog);
			}
		}

		Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, DustID.Ice, 0, 0, 0, default, Main.rand.NextFloat(0.75f, 1.25f));
		dust.noGravity = true;
		if (Projectile.position.X <= 320 || Projectile.position.X >= Main.maxTilesX * 16 - 320)
		{
			Projectile.Kill();
		}
		if (Projectile.position.Y <= 320 || Projectile.position.Y >= Main.maxTilesY * 16 - 320)
		{
			Projectile.Kill();
		}

		// 超过一定时间开始下坠
		if (Projectile.timeLeft < 320)
		{
			Projectile.velocity.Y += 0.25f;
			Projectile.velocity *= 0.98f;
		}
		if (Projectile.lavaWet)
		{
			if (Projectile.timeLeft > 2)
			{
				Projectile.timeLeft = 2;
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return true;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		return true;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Frostburn, 180);
	}

	public override void OnKill(int timeLeft)
	{
		GenerateSmog(6);
		for (int g = 0; g < 18; g++)
		{
			Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<IceParticle>(), 0, 0, 0, default, Main.rand.NextFloat(0.75f, 1.75f));
			dust.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 8f).RotatedByRandom(MathHelper.TwoPi);
		}
		GenerateFrostFlame(20);
		GenerateIceDust(20);
		GenerateSnowPiece(10);
		SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.type == ModContent.ProjectileType<FrostBomb>())
			{
				if ((proj.Center - Projectile.Center).Length() < 150f)
				{
					if (proj.active && proj.timeLeft > 5)
					{
						proj.timeLeft = 5;
					}
				}
			}
		}
	}

	public void GenerateIceDust(int frequency)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new IceParticleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(237, 345),
				Scale = Main.rand.NextFloat(8.20f, 27.10f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void GenerateSnowPiece(int frequency)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 5.6f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new SnowPieceDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Coord0 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
				Coord1 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(147, 285),
				Scale = Main.rand.NextFloat(2f, 7f),
				Rotation = Main.rand.NextFloat(6.283f),
				Rotation2 = Main.rand.NextFloat(6.283f),
				Omega = Main.rand.NextFloat(-10f, 10f),
				Phi = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public void GenerateFrostFlame(int frequency = 1)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var fire = new FrostFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(16, 45),
				Scale = Main.rand.NextFloat(20f, 60f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, 1f },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void GenerateSmog(int Frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < Frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new IceSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 16,
				MaxTime = Main.rand.Next(237, 345),
				Scale = Main.rand.NextFloat(100f, 135f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < Frequency / 2 - 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new IceSmogDust2
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 16,
				MaxTime = Main.rand.Next(237, 345),
				Scale = Main.rand.NextFloat(220f, 235f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < Frequency * 6; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 4.6f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new SnowPieceDust
			{
				Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0f, 0.03f),
				Active = true,
				Visible = true,
				Coord0 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
				Coord1 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
				MaxTime = Main.rand.Next(47, 85),
				Scale = Main.rand.NextFloat(2f, 12f),
				Rotation = Main.rand.NextFloat(6.283f),
				Rotation2 = Main.rand.NextFloat(6.283f),
				Omega = Main.rand.NextFloat(-10f, 10f),
				Phi = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
	}
}
