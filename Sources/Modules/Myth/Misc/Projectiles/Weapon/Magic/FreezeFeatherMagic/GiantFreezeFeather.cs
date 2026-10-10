using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic.FreezeFeatherMagic;

public class GiantFreezeFeather : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 360;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.extraUpdates = 3;
		Projectile.localNPCHitCooldown = 2;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 80;
	}

	public int TimeToKill = -1;

	public override void AI()
	{
		if (TimeToKill >= 0 && TimeToKill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeToKill <= 80 && TimeToKill > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
		}

		TimeToKill--;
		if (TimeToKill >= 0)
		{
			if (TimeToKill < 10)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
		}
		else
		{
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
			Projectile.tileCollide = Projectile.position.Y >= Main.MouseWorld.Y;

			if (Projectile.Center.X > Main.screenPosition.X - 100 && Projectile.Center.X < Main.screenPosition.X + Main.screenWidth + 100 && Projectile.Center.Y > Main.screenPosition.Y - 100 && Projectile.Center.Y < Main.screenPosition.Y + Main.screenWidth + 100)
			{
				for (int x = 0; x < 4; x++)
				{
					Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 0.6f)).RotatedByRandom(MathHelper.TwoPi);
					var smog = new IceSmogDust
					{
						Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0f, 0.03f),
						Active = true,
						Visible = true,
						Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
						MaxTime = Main.rand.Next(437, 545),
						Scale = Main.rand.NextFloat(18f, 45f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.005f, 0.005f) },
					};
					Ins.VFXManager.Add(smog);
				}
				for (int x = 0; x < 4; x++)
				{
					Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 0.6f)).RotatedByRandom(MathHelper.TwoPi);
					var smog = new IceSmogDust2
					{
						Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0f, 0.03f),
						Active = true,
						Visible = true,
						Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
						MaxTime = Main.rand.Next(437, 545),
						Scale = Main.rand.NextFloat(18f, 45f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.005f, 0.005f) },
					};
					Ins.VFXManager.Add(smog);
				}
				for (int x = 0; x < 4; x++)
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
				if (Projectile.position.X <= 320 || Projectile.position.X >= Main.maxTilesX * 16 - 320)
				{
					Projectile.Kill();
				}
			}
		}
		if (Main.rand.NextBool(6))
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(0, 2)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.FreezeFeather>(), v.X, v.Y, 150, default, Main.rand.NextFloat(0.8f, 1.7f));
		}
		for (int g = 0; g < 1; g++)
		{
			Vector2 iceV = new Vector2(0, Main.rand.NextFloat(0, 0.9f)).RotatedByRandom(MathHelper.TwoPi);
			Dust ice = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.IceCrystal>(), iceV.X, iceV.Y, 150, default, Main.rand.NextFloat(0.2f, 0.6f));
			ice.velocity = iceV;
			ice.color.G = (byte)Main.rand.Next(240);
		}

		if (Projectile.position.Y <= 320 || Projectile.position.Y >= Main.maxTilesY * 16 - 320)
		{
			Projectile.Kill();
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		if (TimeToKill >= 0)
		{
			float timeValue = (80 - TimeToKill) / 80f;
			DrawTrail(Commons.ModAsset.Trail_2_black_thick.Value, Color.White * 0.3f, 36);
			DrawTrail(Commons.ModAsset.Trail_6.Value, new Color(0.55f * (1 - timeValue) * (1 - timeValue), 0.9f * (1 - timeValue), 1f, 0f), Math.Max(TimeToKill - 44, 0));
			return;
		}
		else
		{
			DrawTrail(Commons.ModAsset.Trail_2_black_thick.Value, Color.White * 0.3f);
			DrawTrail(Commons.ModAsset.Trail_6.Value, new Color(0.55f, 0.9f, 1f, 0f));
		}
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spriteEffects = SpriteEffects.FlipHorizontally;
		}

		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		int frameHeight = texture.Height / Main.projFrames[Projectile.type];
		int startY = frameHeight * Projectile.frame;
		var sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
		Vector2 origin = sourceRectangle.Size() / 2f;
		float offsetX = 20f;
		origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
		float amount = 1f;
		if (Projectile.timeLeft >= 1040)
		{
			amount = (1080 - Projectile.timeLeft) / 40f;
		}
		Color aimColor = new Color(1f, 1f, 1f, 1f);
		Color drawColor = Color.Lerp(lightColor, aimColor, amount);
		if (Projectile.wet)
		{
			float value = 0.6f;
			if (Projectile.timeLeft < 700)
			{
				value = (Projectile.timeLeft - 100) / 1000f;
			}
			aimColor = new Color(value, value / 12f, 0f, 1f);
			drawColor = aimColor;
		}
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
	}

	public void DrawTrail(Texture2D tex, Color color, float width = 36)
	{
		var c0 = color;
		var bars = new List<Vertex2D>();

		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueL++;
		}
		if (TimeToKill > 0)
		{
			trueL = Math.Max(trueL, TimeToKill);
			if (trueL == 0)
			{
				return;
			}
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			float width2 = width;
			if (Projectile.timeLeft <= 40)
			{
				width2 = Projectile.timeLeft * 0.9f;
			}

			if (i < 10)
			{
				width2 *= i / 10f;
			}

			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			if (normalDir == Vector2.zeroVector)
			{
				continue;
			}
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = factor * 0.6f - (float)(Main.timeForVisualEffects / 35d) + 10000;
			if (i > 2)
			{
				if ((Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1]) == Vector2.zeroVector)
				{
					bars.Add(new Vertex2D(Projectile.oldPos[i - 1] + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 0.5f, w)));
					bars.Add(new Vertex2D(Projectile.oldPos[i - 1] + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 0.5f, w)));
				}
			}
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width2 * (1 - factor) + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width2 * (1 - factor) + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 0, w)));
		}

		Texture2D t = tex;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		AmmoHit();
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GiantFreezeFeatherExplosion>(), Projectile.damage, 10, Projectile.owner);
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		AmmoHit();
		target.AddBuff(BuffID.Frostburn2, 1200);
		target.AddBuff(ModContent.BuffType<Buffs.Freeze>(), 60);
	}

	public void AmmoHit()
	{
		SoundEngine.PlaySound(SoundID.Shatter.WithVolumeScale(0.8f), Projectile.Center);
		if (TimeToKill > 0)
		{
			return;
		}
		Player player = Main.player[Projectile.owner];
		TimeToKill = 80;
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.ignoreWater = true;
		Projectile.velocity = Projectile.oldVelocity;
		SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact.WithVolume(0.3f).WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), Projectile.Center);
		for (int j = 0; j < 40; j++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(7, 120)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.FreezeFeather>(), v.X, v.Y, 150, default, Main.rand.NextFloat(1.8f, 3.7f));
		}
		for (int j = 0; j < 20; j++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(2f, 14.6f)).RotatedByRandom(MathHelper.TwoPi);
			Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.IceCrystal2>(), v.X, v.Y, 150, default, Main.rand.NextFloat(0.6f, 1.4f));
			d.velocity = v;
		}
		for (int j = 0; j < 60; j++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(1f, 12.6f)).RotatedByRandom(MathHelper.TwoPi);
			Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.IceCrystal>(), v.X, v.Y, 150, default, Main.rand.NextFloat(0.4f, 0.6f));
			d.velocity = v;
			d.color.G = 120;
		}
		GenerateSmog(20);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GiantFreezeFeatherExplosion>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 8);
		Projectile.position -= Projectile.velocity;
	}

	public void GenerateSmog(int frequency)
	{
		float mulVelocity = 3f;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new IceSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 16 - Projectile.velocity,
				MaxTime = Main.rand.Next(637, 745),
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
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 16 - Projectile.velocity,
				MaxTime = Main.rand.Next(637, 745),
				Scale = Main.rand.NextFloat(220f, 235f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(smog);
		}
		for (int g = 0; g < frequency * 20; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(6f, 17.6f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new SnowPieceDust
			{
				Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0f, 0.03f),
				Active = true,
				Visible = true,
				Coord0 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
				Coord1 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(47, 185),
				Scale = Main.rand.NextFloat(2f, 14f),
				Rotation = Main.rand.NextFloat(6.283f),
				Rotation2 = Main.rand.NextFloat(6.283f),
				Omega = Main.rand.NextFloat(-10f, 10f),
				Phi = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
		for (int g = 0; g < frequency * 12; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new IceParticleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(37, 145),
				Scale = Main.rand.NextFloat(1.20f, 12.35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(smog);
		}
	}
}
