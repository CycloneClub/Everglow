using Everglow.Myth.LanternMoon.VFX;
using Terraria.Audio;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class CylindricalLantern_explosion : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 720;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 120;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 2;
	}

	public float Timer = 0;

	public override void AI()
	{
		Timer++;
		Projectile.velocity *= 0;
		if (Timer == 60)
		{
			SmallFlame(Projectile.Center, 120);
			LargeFlame(Projectile.Center, 10);
			Spark(Projectile.Center);
			Projectile.hostile = true;
			SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolumeScale(0.6f), Projectile.Center);
		}
	}

	public void Spark(Vector2 pos)
	{
		for (int g = 0; g < 20; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(9f, 16f) * (Main.rand.Next(2) - 0.5f) * 2f);
			var sparkFlame = new LanternRedPaperFlame
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(-40, 40), 0),
				RotateSpeed = Main.rand.NextFloat(-1.2f, 1.2f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (Main.rand.Next(2) - 0.5f) * 2,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(45, 60),
				Scale = Main.rand.NextFloat(3f, 12f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkFlame);
		}

		for (int g = 0; g < 32; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(17f, 30f) * (Main.rand.Next(2) - 0.5f) * 2f);
			var spark = new LanternExplosionSpark
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(-40, 40), 0),
				RotateSpeed = Main.rand.NextFloat(-0.7f, 0.7f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.05f, 0.25f) * (Main.rand.Next(2) - 0.5f) * 2,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(30, 60),
				Scale = Main.rand.NextFloat(2f, 3f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(spark);
		}

		for (int g = 0; g < 6; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(10f, 15f) * (Main.rand.Next(2) - 0.5f) * 2f);
			var sparkStar = new LanternExplosionSparkStar
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(-40, 40), 0),
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(20, 40),
				Scale = Main.rand.NextFloat(0.5f, 2.6f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkStar);
		}
	}

	public void SmallFlame(Vector2 pos, int count)
	{
		for (int c = 0; c < count; c++)
		{
			float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
			Vector2 newVelocity = new Vector2(0, sqrtSpeed * 40f * (Main.rand.Next(2) - 0.5f) * 2f);
			var somg = new LanternFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(-40, 40), 0),
				MaxTime = Main.rand.Next(30, 45),
				Scale = Main.rand.NextFloat(50f, 120f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void LargeFlame(Vector2 pos, int count)
	{
		for (int c = 0; c < count; c++)
		{
			float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f) + 0.5f);
			Vector2 newVelocity = new Vector2(0, sqrtSpeed * 40f * (Main.rand.Next(2) - 0.5f) * 2f);
			var somg = new LanternFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(-10, 10), 0),
				MaxTime = Main.rand.Next(60, 75),
				Scale = Main.rand.NextFloat(160f, 240f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Texture2D trail = Commons.ModAsset.TrailV.Value;
		var drawPos = Projectile.Center - Main.screenPosition;
		var drawColor = new Color(1f, 0.2f, 0f, 0f);
		if (Timer < 60)
		{
			float value = MathF.Pow(Timer / 60f, 0.5f);
			Main.EntitySpriteDraw(star, drawPos + new Vector2(40 * value, 0), null, drawColor, 0, star.Size() * 0.5f, new Vector2(1f, 4f), SpriteEffects.None, 0);
			Main.EntitySpriteDraw(star, drawPos + new Vector2(-40 * value, 0), null, drawColor, 0, star.Size() * 0.5f, new Vector2(1f, 4f), SpriteEffects.None, 0);

			float value2 = MathF.Pow(Timer / 60f, 1f);
			Main.EntitySpriteDraw(trail, drawPos + new Vector2(0, 360 * value2), null, drawColor * (1 - value), MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(0.25f, 0.32f * value), SpriteEffects.None, 0);
			Main.EntitySpriteDraw(trail, drawPos + new Vector2(0, -360 * value2), null, drawColor * (1 - value), MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(0.25f, 0.32f * value), SpriteEffects.None, 0);
		}
		if (Timer >= 60 && Timer < 90)
		{
			float value = 1f;
			Main.EntitySpriteDraw(star, drawPos + new Vector2(40 * value, 0), null, drawColor, 0, star.Size() * 0.5f, new Vector2(1f, 4f), SpriteEffects.None, 0);
			Main.EntitySpriteDraw(star, drawPos + new Vector2(-40 * value, 0), null, drawColor, 0, star.Size() * 0.5f, new Vector2(1f, 4f), SpriteEffects.None, 0);
		}
		if (Timer >= 90)
		{
			float value = MathF.Pow(Projectile.timeLeft / 30f, 1f);
			Main.EntitySpriteDraw(star, drawPos + new Vector2(40 * value, 0), null, drawColor, 0, star.Size() * 0.5f, new Vector2(1f, 4f), SpriteEffects.None, 0);
			Main.EntitySpriteDraw(star, drawPos + new Vector2(-40 * value, 0), null, drawColor, 0, star.Size() * 0.5f, new Vector2(1f, 4f), SpriteEffects.None, 0);
		}
		return false;
	}
}
