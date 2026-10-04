using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_AttackProj1 : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 1;
		Projectile.timeLeft = 2400;
		Projectile.alpha = 0;
		Projectile.penetrate = 30;
		Projectile.scale = 1f;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 90;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	private void AddLight()
	{
		if (TimeToKill < 0)
		{
			Lighting.AddLight(Projectile.Center, 1f, 0, 0);
		}
	}

	public override void AI()
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		if (TimeToKill >= 0 && TimeToKill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeToKill > 0)
		{
			Projectile.velocity *= 0.01f;
		}

		TimeToKill--;
		if (TimeToKill < 0)
		{
		}
		AddLight();
	}

	public int TimeToKill = -1;

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		HitToAnything();
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		HitToAnything();
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		HitToAnything();
		Projectile.tileCollide = false;
		return false;
	}

	private void HitToAnything()
	{
		Projectile.velocity = Projectile.oldVelocity;
		if (TimeToKill < 0)
		{
			for (int g = 0; g < 5; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 9.6f)).RotatedByRandom(MathHelper.TwoPi);
				var smog = new FevensCrystalPieceDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Coord0 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
					Coord1 = new Vector2(Main.rand.NextFloat(0.1f, 0.2f), 0).RotatedByRandom(6.283),
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(27, 35),
					Scale = Main.rand.NextFloat(2f, 12f),
					Rotation = Main.rand.NextFloat(6.283f),
					rotation2 = Main.rand.NextFloat(6.283f),
					Omega = Main.rand.NextFloat(-10f, 10f),
					Phi = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
				};
				Ins.VFXManager.Add(smog);
			}
			SoundEngine.PlaySound(SoundID.NPCHit4.WithVolumeScale(0.8f), Projectile.Center);
		}
		TimeToKill = 90;
	}

	private void DrawTrail()
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float k1 = 10f;
		float colorValue0 = (2400 - Projectile.timeLeft) / k1;

		if (Projectile.timeLeft <= 2400 - k1)
		{
			colorValue0 = 1;
		}

		int trueLength = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueLength++;
		}

		var c0 = new Color(colorValue0, 0, colorValue0 * colorValue0 * 0.3f, 0);

		var bars = new List<Vertex2D>();
		for (int i = 1; i < trueLength; ++i)
		{
			float width = 24;
			if (Projectile.timeLeft <= 40)
			{
				width = Projectile.timeLeft * 0.9f;
			}

			if (i < 10)
			{
				width *= i / 10f;
			}

			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueLength;
			var c1 = c0 * (1 - factor);
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = factor * 0.6f - (float)(Main.time / 35d) + 10000;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c1, new Vector3(x0, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c1, new Vector3(x0, 0, w)));
		}

		var barsDark = new List<Vertex2D>();
		for (int i = 1; i < trueLength; ++i)
		{
			float width = 20;
			if (Projectile.timeLeft <= 40)
			{
				width = Projectile.timeLeft * 0.9f;
			}

			if (i < 4)
			{
				width *= i / 4f;
			}

			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueLength;
			var c1 = new Color(255, 255, 255, 255) * (1 - factor);
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = factor * 1.6f - (float)(Main.time / 35d) + 10000;
			barsDark.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c1, new Vector3(x0, 1, w)));
			barsDark.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c1, new Vector3(x0, 0, w)));
		}

		Texture2D t = Commons.ModAsset.Trail_3.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		t = Commons.ModAsset.Trail_3_black.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (barsDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsDark.ToArray(), 0, barsDark.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawTrail();
		Texture2D star = ModAsset.FevensChasingProj.Value;
		if (TimeToKill < 0)
		{
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition - Projectile.velocity, null, new Color(250, 250, 250, 150), Projectile.rotation + MathHelper.PiOver4, star.Size() / 2f, 0.75f, SpriteEffects.None, 0);
		}
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float width = 16;
		float velocityValue = Projectile.velocity.Length() / 30f;
		float colorValueG = velocityValue;
		int trueLength = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueLength++;
		}
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float MulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				MulColor = 0f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					MulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					MulColor = 0f;
				}
			}

			float colorValue0 = (float)Math.Atan2(normalDir.Y, normalDir.X);
			colorValue0 += 3.14f + 1.57f;
			if (colorValue0 > 6.28f)
			{
				colorValue0 -= 6.28f;
			}

			var c0 = new Color(colorValue0, 0.04f * colorValueG * MulColor, 0, 0);

			var factor = i / (float)trueLength;
			float x0 = factor * 1.3f - (float)(Main.time / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factorII = (i + 1) / (float)trueLength;
			var x1 = factorII * 1.3f - (float)(Main.time / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(5f) - Main.screenPosition, c0, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = Commons.ModAsset.Trail_4.Value;

		if (bars.Count > 3)
		{
			spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
		}
	}
}
