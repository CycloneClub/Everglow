using Everglow.Myth.LanternMoon.VFX;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class DarkLanternBombExplosion : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.aiStyle = -1;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 30;
		Projectile.penetrate = -1;
	}

	public override void AI()
	{
		float value = Projectile.timeLeft / 20f;
		Lighting.AddLight(Projectile.Center, new Vector3(value * 3.5f, value * value * 1.5f, value * value * value));
		base.AI();
	}

	public override void OnSpawn(IEntitySource source)
	{
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolumeScale(Projectile.ai[0] / 2.4f), Projectile.Center);
		ExplodeEffect(Projectile.Center);
	}

	public void ExplodeEffect(Vector2 pos)
	{
		for (int x = 0; x < 15 * Projectile.ai[0]; x++)
		{
			SmallFlame(pos);
		}
		for (int x = 0; x < 15 * Projectile.ai[0]; x++)
		{
			LargeFlame(pos);
		}

		Spark(pos);
		Wave(pos);
	}

	public void Wave(Vector2 pos)
	{
		var wave = new WarpLanternWave
		{
			Position = pos,
			Speed = 25f * Projectile.ai[0],
			Range = 0,
			Timer = 0,
			MaxTime = 30,
			SpeedDecay = 0.9f,
			Active = true,
			Visible = true,
		};
		Ins.VFXManager.Add(wave);

		var redWave = new RedLanternWave
		{
			Position = pos,
			Speed = 25f * Projectile.ai[0],
			Range = 0,
			Timer = 0,
			MaxTime = 30,
			SpeedDecay = 0.9f,
			Active = true,
			Visible = true,
		};
		Ins.VFXManager.Add(redWave);
	}

	public void Spark(Vector2 pos)
	{
		for (int g = 0; g < 20 * Projectile.ai[0]; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(9f, 16f) * Projectile.ai[0]).RotatedByRandom(MathHelper.TwoPi);
			var sparkFlame = new LanternRedPaperFlame
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				RotateSpeed = Main.rand.NextFloat(-1.2f, 1.2f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (g % 2 - 0.5f) * 2,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(45, 60),
				Scale = Main.rand.NextFloat(3f, 12f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkFlame);
		}

		for (int g = 0; g < 32 * Projectile.ai[0]; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(17f, 30f) * Projectile.ai[0]).RotatedByRandom(MathHelper.TwoPi);
			var spark = new LanternExplosionSpark
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				RotateSpeed = Main.rand.NextFloat(-0.7f, 0.7f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.05f, 0.25f) * (g % 2 - 0.5f) * 2,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(30, 60),
				Scale = Main.rand.NextFloat(2f, 3f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(spark);
		}

		for (int g = 0; g < 6; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(10f, 15f)).RotatedByRandom(MathHelper.TwoPi);
			var sparkStar = new LanternExplosionSparkStar
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(20, 40),
				Scale = Main.rand.NextFloat(0.5f, 2.6f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkStar);
		}

		for (int g = 0; g < 30; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(10f, 18f) * Projectile.ai[0]).RotatedByRandom(MathHelper.TwoPi);
			var spark = new LanternGhostKing_SmokeSpike
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				MaxTime = Main.rand.Next(10, 30),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(1f, 25.0f)) * Projectile.ai[0],
			};
			Ins.VFXManager.Add(spark);
		}
		for (int g = 0; g < 2; g++)
		{
			float scale = 100f;
			int maxTime = 50;
			float rot = Main.rand.NextFloat(MathHelper.TwoPi);
			if (g == 1)
			{
				maxTime = 50;
				scale = 100f * Projectile.ai[0];
				rot += MathHelper.Pi;
			}
			if (g == 0)
			{
				maxTime = 30;
				scale = 150f * Projectile.ai[0];
			}
			var blackLine = new LanternExplosionDecayBlackLines
			{
				Position = pos,
				Timer = 0,
				MaxTime = maxTime,
				Scale = scale,
				Rotation = rot,
				ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), scale / 6f },
				Active = true,
				Visible = true,
			};
			Ins.VFXManager.Add(blackLine);
		}
	}

	public void LargeFlame(Vector2 pos)
	{
		float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
		Vector2 newVelocity = new Vector2(0, sqrtSpeed * 8).RotatedByRandom(MathHelper.TwoPi) * Projectile.ai[0];
		var somg = new LanternFlameDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi),
			MaxTime = Main.rand.Next(60, 75) * MathF.Pow(Projectile.ai[0], 0.5f),
			Scale = Main.rand.NextFloat(160f, 240f) * Projectile.ai[0],
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
		};
		Ins.VFXManager.Add(somg);
	}

	public void SmallFlame(Vector2 pos)
	{
		float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
		Vector2 newVelocity = new Vector2(0, sqrtSpeed * 16f).RotatedByRandom(MathHelper.TwoPi) * Projectile.ai[0];
		var somg = new LanternFlameDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = pos + new Vector2(Main.rand.NextFloat(30), 0).RotatedByRandom(MathHelper.TwoPi),
			MaxTime = Main.rand.Next(30, 45) * MathF.Pow(Projectile.ai[0], 0.5f),
			Scale = Main.rand.NextFloat(50f, 120f) * Projectile.ai[0],
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
		};
		Ins.VFXManager.Add(somg);
	}

	public Vector2 RandomVector2(float maxLength, float minLength = 0)
	{
		if (maxLength <= minLength)
		{
			maxLength = minLength + 0.001f;
		}
		return new Vector2(Main.rand.NextFloat(minLength, maxLength), 0).RotatedByRandom(6.283);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	private void DrawWarpTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Vector2 center, Texture2D tex, float warpStrength, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		Color color = new Color(0f, 0f, 0f, 0f);
		for (int h = 0; h < radius / 2; h += 1)
		{
			float colorR = (h / radius * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);
			float color2R = ((h + 1) / radius * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);
			color = new Color(colorR, warpStrength, 0f, 0f);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
			if (Math.Abs(color2R - colorR) > 0.8f)
			{
				float midValue = (1f - colorR) / (float)(color2R + (1f - colorR));
				color.R = 255;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 1, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 0, 0)));
				color.R = 0;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 1, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 0, 0)));
			}
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch sb)
	{
		float value = (30 - Projectile.timeLeft) / 15f;
		value = MathF.Sqrt(value);

		Texture2D t = Commons.ModAsset.NoiseWave.Value;
		float width = 15;
		if (Projectile.timeLeft < 15)
		{
			width = Projectile.timeLeft;
		}

		DrawWarpTexCircle_VFXBatch(sb, value * value * 80 + 90, width * 8, Projectile.Center - Main.screenPosition, t, Projectile.timeLeft / 4000f);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float maxDistance = 120;
		maxDistance *= Projectile.ai[0];
		bool CheckCenter(Vector2 pos)
		{
			return (pos - projHitbox.Center()).Length() < maxDistance / 0.9f;
		}
		return CheckCenter(targetHitbox.TopLeft()) || CheckCenter(targetHitbox.TopRight()) || CheckCenter(targetHitbox.BottomLeft()) || CheckCenter(targetHitbox.BottomRight());
	}
}
