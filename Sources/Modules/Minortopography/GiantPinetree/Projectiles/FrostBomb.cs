using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Minortopography.GiantPinetree.Dusts;
using Terraria.Audio;

namespace Everglow.Minortopography.GiantPinetree.Projectiles;

public class FrostBomb : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.friendly = true;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 360;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 2;
	}

	public override void AI()
	{
		Projectile.rotation += Projectile.velocity.X * 0.03f;
		if (Projectile.velocity.Length() >= 1)
		{
			GenerateDust();
		}

		if (Projectile.position.X <= 320 || Projectile.position.X >= Main.maxTilesX * 16 - 320)
		{
			Projectile.Kill();
		}
		if (Projectile.position.Y <= 320 || Projectile.position.Y >= Main.maxTilesY * 16 - 320)
		{
			Projectile.Kill();
		}

		// 超过一定时间开始下坠
		if (Projectile.timeLeft < 350)
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

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Projectile.timeLeft < 5)
		{
			bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 80;
			bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 80;
			bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 80;
			bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 80;
			return bool0 || bool1 || bool2 || bool3;
		}
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
	{
		fallThrough = false;
		return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
	}

	public void GenerateDust()
	{
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
					Scale = Main.rand.NextFloat(18f, 45f),
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
					Scale = Main.rand.NextFloat(18f, 45f),
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
					Scale = Main.rand.NextFloat(2f, 8f),
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
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return true;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.velocity *= 0f;
		Projectile.position += Projectile.velocity;
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Projectile.timeLeft > 5)
		{
			Projectile.timeLeft = 5;
		}
		target.AddBuff(BuffID.Frostburn, 270);
	}

	public override void OnKill(int timeLeft)
	{
		GenerateSmog(6);
		for (int g = 0; g < 18; g++)
		{
			Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<IceParticle>(), 0, 0, 0, default, Main.rand.NextFloat(0.75f, 1.75f));
			dust.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 8f).RotatedByRandom(MathHelper.TwoPi);
		}
		for (int g = 0; g < 9; g++)
		{
			Vector2 velo = new Vector2(Main.rand.NextFloat(7f, 12f), 0).RotatedByRandom(6.283);
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + velo, velo, ModContent.ProjectileType<FrostSpice>(), (int)(Projectile.damage * 0.375f), Projectile.knockBack * 0.2f, Projectile.owner);
		}
		GenerateFrostFlame(25);
		GenerateIceDust(30);
		GenerateSnowPiece(15);
		SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
	}

	public void GenerateFrostFlame(int frequency = 1)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 7f)).RotatedByRandom(MathHelper.TwoPi);
			var fire = new FrostFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(16, 45),
				Scale = Main.rand.NextFloat(40f, 90f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, 1f },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void GenerateIceDust(int frequency)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 17f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new IceParticleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(237, 345),
				Scale = Main.rand.NextFloat(14.20f, 37.35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public void GenerateSnowPiece(int frequency)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 9.6f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new SnowPieceDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Coord0 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
				Coord1 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(147, 285),
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

	public void GenerateSmog(int frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new IceSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 16,
				MaxTime = Main.rand.Next(237, 345),
				Scale = Main.rand.NextFloat(320f, 435f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(smog);
		}
		for (int g = 0; g < frequency / 2 - 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new IceSmogDust2
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
			Ins.VFXManager.Add(smog);
		}
		for (int g = 0; g < frequency * 6; g++)
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
