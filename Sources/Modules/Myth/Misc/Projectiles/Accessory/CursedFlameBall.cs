using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;

namespace Everglow.Myth.Misc.Projectiles.Accessory;

public class CursedFlameBall : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 1000;
		Projectile.alpha = 0;
		Projectile.penetrate = 3;
		Projectile.scale = 1f;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 70;
	}

	public override void AI()
	{
		Projectile.velocity.Y += 0.010f;
		if (Ins.VisualQuality.High)
		{
			if (Main.rand.NextBool(4))
			{
				GenerateVFX(1);
			}
			if (Main.rand.NextBool(4))
			{
				var spark = new CurseFlameSparkDust
				{
					Velocity = Projectile.velocity + new Vector2(0, Main.rand.NextFloat(0.07f, 1f)).RotatedByRandom(6.283),
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity,
					MaxTime = Main.rand.Next(7, 95),
					Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 47.0f)),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
				};
				Ins.VFXManager.Add(spark);
			}
		}
	}

	public void GenerateVFX(int frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < frequency; g++)
		{
			var cf = new CurseFlameDust
			{
				Velocity = Projectile.velocity * Main.rand.NextFloat(0.65f, 2.5f) * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * 1,
				MaxTime = Main.rand.Next(27, 32),
				Scale = 4f,
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.01f, 0.01f), Main.rand.NextFloat(0.2f, 0.8f) },
			};
			Ins.VFXManager.Add(cf);
		}

		if (Main.rand.NextBool(3))
		{
			var cf = new CurseFlame_HighQualityDust
			{
				Velocity = Projectile.velocity * 2.7f,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(27, 122),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), 0, Main.rand.NextFloat(3.6f, 10f) },
			};
			Ins.VFXManager.Add(cf);
		}
	}

	public void GenerateVFXExpolode(int frequency, float mulVelocity = 1f)
	{
		if (Ins.VisualQuality.Low)
		{
			frequency /= 3;
		}
		for (int g = 0; g < frequency * 3; g++)
		{
			var cf = new CurseFlameDust
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(1.65f, 2.5f)).RotatedByRandom(6.283) * mulVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-26f, 26f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(12, 66),
				Scale = 12f * mulVelocity,
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.18f, 0.18f), Main.rand.NextFloat(1f, 2.2f) * mulVelocity },
			};
			Ins.VFXManager.Add(cf);
		}
		for (int g = 0; g < frequency; g++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(1.65f, 3.5f)).RotatedByRandom(6.283) * mulVelocity;
			var cf = new CurseFlameDust
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center + vel * 3,
				MaxTime = Main.rand.Next(12, 70),
				Scale = 12f * mulVelocity,
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(2f, 3.2f) * mulVelocity },
			};
			Ins.VFXManager.Add(cf);
		}
		for (int g = 0; g < frequency * 3; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new CurseFlameSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity + newVelocity * 3,
				MaxTime = Main.rand.Next(37, 145),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 47.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D light = ModAsset.GlowStar.Value;
		Texture2D shade = Commons.ModAsset.Point_black.Value;
		var c0 = new Color(0.4f, 0.3f + 0.6f, 0, 0);

		var bars0 = new List<Vertex2D>();
		float width = 24;

		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueL++;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var c1 = Color.White;
			if (i < 4)
			{
				c1 = new Color(i / 5f, i / 5f, i / 5f, i / 5f);
			}
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = (float)(factor * 0.6f + Main.time * 0.04);
			x0 %= 1f;
			bars0.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c1, new Vector3(x0, 1, w)));
			bars0.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c1, new Vector3(x0, 0, w)));
		}
		Texture2D t = Commons.ModAsset.Trail_2_black_thick.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars0.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars0.ToArray(), 0, bars0.Count - 2);
		}

		Main.spriteBatch.Draw(shade, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, Color.Transparent, Projectile.rotation, light.Size() / 2f, (1 / 1.8f + 0.2f) / (Projectile.ai[0] + 3) * 2.5f, SpriteEffects.None, 0);

		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var c1 = c0;
			if (i < 4)
			{
				c1 = new Color(0.4f * i / 5f, 0.9f * i / 5f, 0, 0);
			}
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = (float)(factor * 1.6 + Main.time * 0.04);
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c1, new Vector3(x0, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c1, new Vector3(x0, 0, w)));
		}
		t = Commons.ModAsset.Trail_2_thick.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, light.Size() / 2f, 0.6f, SpriteEffects.None, 0);
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, light.Size() / 2f, 0.6f, SpriteEffects.None, 0);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float width = 24;
		float mulByTimeLeft = 1f;
		if (Projectile.timeLeft < 500)
		{
			mulByTimeLeft = Projectile.timeLeft / 500f;
		}

		width *= mulByTimeLeft;
		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueL++;
		}
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float mulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i <= 8)
			{
				mulColor = i / 9f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}

			float k0 = (float)Math.Atan2(normalDir.Y, normalDir.X);
			k0 += 3.14f + 1.57f;
			if (k0 > 6.28f)
			{
				k0 -= 6.28f;
			}

			Color c0 = new Color(k0, 0.02f * mulColor, 0, 0) * mulByTimeLeft;

			var factor = i / (float)trueL;
			float x0 = factor * 1.3f - (float)(Main.time / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)trueL;
			var x1 = factor * 1.3f - (float)(Main.time / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float deltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * deltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = Commons.ModAsset.Trail_2_thick.Value;

		if (bars.Count > 3)
		{
			spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
		}
	}

	public override void OnKill(int timeLeft)
	{
		ScreenShaker gsPlayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		gsPlayer.FlyCamPosition = new Vector2(0, 33).RotatedByRandom(6.283);

		GenerateVFXExpolode(8, 2.2f);

		for (int d = 0; d < 70; d++)
		{
			Vector2 basePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
			var d0 = Dust.NewDustDirect(basePos, 0, 0, DustID.CursedTorch, 0, 0, 0, default, 0.6f);
			d0.velocity = new Vector2(0, Main.rand.NextFloat(3.65f, 7.5f)).RotatedByRandom(6.283);
		}
		int hitType = ModContent.ProjectileType<CursedFlameBrust>();
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One, hitType, Projectile.damage, Projectile.knockBack * 6, Projectile.owner, 18, Projectile.rotation + Main.rand.NextFloat(6.283f));

		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolumeScale(0.4f), Projectile.Center);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		ScreenShaker gsPlayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		gsPlayer.FlyCamPosition = new Vector2(0, 11).RotatedByRandom(6.283);
		GenerateVFXExpolode(5, 0.6f);
		for (int d = 0; d < 28; d++)
		{
			Vector2 basePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
			var d0 = Dust.NewDustDirect(basePos, 0, 0, DustID.CursedTorch, 0, 0, 0, default, 0.6f);
			d0.velocity = new Vector2(0, Main.rand.NextFloat(1.65f, 5.5f)).RotatedByRandom(6.283);
		}

		int hitType = ModContent.ProjectileType<CursedFlameBrust>();
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One, hitType, Projectile.damage, Projectile.knockBack * 2, Projectile.owner, 10, Projectile.rotation + Main.rand.NextFloat(6.283f));
		target.AddBuff(BuffID.CursedInferno, 900);
		Projectile.damage = (int)(Projectile.damage * 1.2);

		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot.WithVolumeScale(0.4f), Projectile.Center);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		ScreenShaker gsPlayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		gsPlayer.FlyCamPosition = new Vector2(0, 11).RotatedByRandom(6.283);
		GenerateVFXExpolode(5, 0.6f);
		for (int d = 0; d < 28; d++)
		{
			Vector2 basePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
			var d0 = Dust.NewDustDirect(basePos, 0, 0, DustID.CursedTorch, 0, 0, 0, default, 0.6f);
			d0.velocity = new Vector2(0, Main.rand.NextFloat(1.65f, 5.5f)).RotatedByRandom(6.283);
		}
		int hitType = ModContent.ProjectileType<CursedFlameBrust>();
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One, hitType, Projectile.damage, Projectile.knockBack * 2, Projectile.owner, 10, Projectile.rotation + Main.rand.NextFloat(6.283f));
		target.AddBuff(BuffID.CursedInferno, 900);
		Projectile.damage = (int)(Projectile.damage * 1.2);

		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot.WithVolumeScale(0.4f), Projectile.Center);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		ScreenShaker gsPlayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		gsPlayer.FlyCamPosition = new Vector2(0, 11).RotatedByRandom(6.283);
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot.WithVolumeScale(0.4f), Projectile.Center);
		GenerateVFXExpolode(5, 0.6f);
		for (int d = 0; d < 28; d++)
		{
			Vector2 basePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
			var d0 = Dust.NewDustDirect(basePos, 0, 0, DustID.CursedTorch, 0, 0, 0, default, 0.6f);
			d0.velocity = new Vector2(0, Main.rand.NextFloat(1.65f, 5.5f)).RotatedByRandom(6.283);
		}
		int hitType = ModContent.ProjectileType<CursedFlameBrust>();
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One, hitType, Projectile.damage, Projectile.knockBack * 2, Projectile.owner, 10, Projectile.rotation + Main.rand.NextFloat(6.283f));
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		Projectile.velocity *= 0.98f;
		Projectile.penetrate--;
		Projectile.damage = (int)(Projectile.damage * 1.2);
		return false;
	}
}
