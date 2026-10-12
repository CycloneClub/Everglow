using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class LanternGhostKingExplosion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => "Everglow/" + ModAsset.DarkLanternBombExplosion_Path;

	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.width = 400;
		Projectile.height = 400;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 240;
		Projectile.penetrate = -1;
		Timer = 0;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		ExplodeEffect(Projectile.Center);
		for (int i = 0; i < 14; i++)
		{
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, Main.rand.NextFloat(10, 80)).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<BurningLanternWreck>(), 30, 1, Projectile.owner);
			p0.timeLeft = Main.rand.Next(100, 240);
		}
	}

	public override void AI()
	{
		Timer++;
		int threthod = 20;
		if (Timer < threthod)
		{
			int count = threthod - Timer;

			// count *= 3;
			for (int x = 0; x < count; x++)
			{
				SmallFlame(Projectile.Center);
			}
			for (int x = 0; x < count; x++)
			{
				LargeFlame(Projectile.Center);
			}
		}
	}

	public void ExplodeEffect(Vector2 pos)
	{
		for (int x = 0; x < 100; x++)
		{
			SmallFlame(pos);
		}
		for (int x = 0; x < 100; x++)
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
			Speed = 55f,
			Range = 0,
			Timer = 0,
			MaxTime = 90,
			SpeedDecay = 0.95f,
			Active = true,
			Visible = true,
		};
		Ins.VFXManager.Add(wave);

		var redWave = new RedLanternWave
		{
			Position = pos,
			Speed = 50f,
			Range = 0,
			Timer = 0,
			MaxTime = 60,
			SpeedDecay = 0.95f,
			Active = true,
			Visible = true,
		};
		Ins.VFXManager.Add(redWave);
	}

	public void Spark(Vector2 pos)
	{
		for (int g = 0; g < 30; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(20f, 32f)).RotatedByRandom(MathHelper.TwoPi);
			var sparkFlame = new LanternRedPaperFlame
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				RotateSpeed = Main.rand.NextFloat(-1.8f, 1.8f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (g % 2 - 0.5f) * 0.2f,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(45, 60),
				Scale = Main.rand.NextFloat(3f, 12f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkFlame);
		}

		for (int g = 0; g < 90; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(34f, 60f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new LanternExplosionSpark
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				RotateSpeed = Main.rand.NextFloat(-0.7f, 0.7f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.05f, 0.025f) * (g % 2 - 0.5f) * 2,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(60, 120),
				Scale = Main.rand.NextFloat(2f, 3f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(spark);
		}
		for (int g = 0; g < 90; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(34f, 120f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new LanternExplosionSpark
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				RotateSpeed = Main.rand.NextFloat(-0.7f, 0.7f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.015f, 0.05f) * (g % 2 - 0.5f) * 2,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(60, 120),
				Scale = Main.rand.NextFloat(2f, 3f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(spark);
		}

		for (int g = 0; g < 24; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(15f, 30f)).RotatedByRandom(MathHelper.TwoPi);
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

		for (int g = 0; g < 60; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(20f, 30f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new LanternGhostKing_SmokeSpike
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 6,
				MaxTime = Main.rand.Next(20, 60),
				Scale = Main.rand.NextFloat(2f, Main.rand.NextFloat(3f, 30.0f)),
			};
			Ins.VFXManager.Add(spark);
		}
		for (int g = 0; g < 2; g++)
		{
			float scale = 200f;
			int maxTime = 80;
			float rot = Main.rand.NextFloat(MathHelper.TwoPi);
			if (g == 1)
			{
				maxTime = 60;
				scale = 200f;
				rot += MathHelper.Pi;
			}
			if (g == 0)
			{
				maxTime = 50;
				scale = 300f;
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
		Vector2 newVelocity = new Vector2(0, sqrtSpeed * 24).RotatedByRandom(MathHelper.TwoPi);
		var somg = new LanternFlameDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = pos + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * Timer * 0.5f,
			MaxTime = Main.rand.Next(120, 185),
			Scale = Main.rand.NextFloat(240f, 320f),
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
		};
		Ins.VFXManager.Add(somg);
	}

	public void SmallFlame(Vector2 pos)
	{
		float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
		Vector2 newVelocity = new Vector2(0, sqrtSpeed * 32f).RotatedByRandom(MathHelper.TwoPi);
		var somg = new LanternFlameDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = pos + new Vector2(Main.rand.NextFloat(30), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * Timer * 0.1f,
			MaxTime = Main.rand.Next(90, 135),
			Scale = Main.rand.NextFloat(50f, 120f),
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
		};
		Ins.VFXManager.Add(somg);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float timeDecay = Projectile.timeLeft / 240f;
		Texture2D spot = Commons.ModAsset.Point.Value;
		Color drawColor = Color.Lerp(new Color(0.3f, 0f, 0f, 0f), new Color(1f, 0.3f, 0f, 0f), timeDecay) * timeDecay;
		if (Timer < 20f)
		{
			float extraLight = Timer / 20f;
			drawColor = Color.Lerp(new Color(1f, 1f, 1f, 0f), drawColor, extraLight);
		}
		var drawPos = Projectile.Center - Main.screenPosition;
		Main.EntitySpriteDraw(spot, drawPos, null, drawColor, 0, spot.Size() * 0.5f, 10f, SpriteEffects.None, 0);
		return false;
	}
}
