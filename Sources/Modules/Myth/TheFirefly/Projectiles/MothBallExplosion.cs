using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.TheFirefly.Buffs;
using Everglow.Myth.TheFirefly.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class MothBallExplosion : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 120;
		Projectile.height = 120;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 2;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void OnSpawn(IEntitySource source)
	{
		ScreenShaker mplayer = Main.player[Main.myPlayer].GetModPlayer<ScreenShaker>();
		mplayer.FlyCamPosition = new Vector2(0, 48).RotatedByRandom(6.283);
		SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/MothBallExplosion"), Projectile.Center);
		GenerateSmog((int)(1.3 * Projectile.ai[0]));
		GenerateFire((int)(2.3 * Projectile.ai[0]));
		GenerateSmog((int)(1.3 * Projectile.ai[0]));
		GenerateElectronic((int)(0.5f * Projectile.ai[0]));
		GenerateSpark((int)(20 * Projectile.ai[0]));
	}

	public void GenerateElectronic(int frequency)
	{
		float mulVelocity = 1;
		for (int g = 0; g < frequency; g++)
		{
			float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(20f, 40f));
			Vector2 afterVelocity = new Vector2(0, size * 1.3f).RotatedByRandom(MathHelper.TwoPi);
			var electric = new ElectricCurrent
			{
				Velocity = afterVelocity * mulVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(60, 120),
				Scale = size,
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), size / 2, Main.rand.NextFloat(0.8f, 1.2f) },
			};
			Ins.VFXManager.Add(electric);
		}

		for (int g = 0; g < frequency * 1.5f; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(20f, 30f)).RotatedByRandom(MathHelper.TwoPi);
			var electric = new MothBallCurrent
			{
				Velocity = afterVelocity * mulVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(20, 50),
				Scale = Main.rand.NextFloat(18, 19),
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(electric);
		}

		// 生成分叉闪电
		int totalLightnings = (int)(frequency * 0.35);
		float angleDivision = (float)(Math.PI * 2 / totalLightnings);
		for (int g = 0; g < totalLightnings; g++)
		{
			float randScale = Main.rand.NextFloat(0.75f, 1.0f);
			var lightning = new BranchedLightning(
				150f * randScale,
				8f * randScale,
				Projectile.position,
				g * angleDivision + Main.rand.NextFloat(angleDivision),
				45f * randScale);
			Ins.VFXManager.Add(lightning);
		}
	}

	public void GenerateSmog(int frequency)
	{
		float mulVelocity = Projectile.ai[0] / 5f;
		for (int g = 0; g < frequency; g++)
		{
			float sqrtRand = MathF.Pow(Main.rand.NextFloat(1), 0.4f);
			Vector2 newVelocity = new Vector2(0, mulVelocity * sqrtRand * 6).RotatedByRandom(MathHelper.TwoPi);
			var smog = new FireSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 1,
				MaxTime = Main.rand.Next(75, 125),
				Scale = Main.rand.NextFloat(2f, 7f) * Projectile.ai[0],
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public void GenerateFire(int frequency)
	{
		float mulVelocity = Projectile.ai[0] / 5f;
		for (int g = 0; g < frequency; g++)
		{
			float sqrtRand = MathF.Pow(Main.rand.NextFloat(1), 0.4f);
			Vector2 newVelocity = new Vector2(0, mulVelocity * sqrtRand * 6).RotatedByRandom(MathHelper.TwoPi);
			var fire = new MothBlueFireDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity,
				MaxTime = Main.rand.Next(49, 125),
				Scale = Main.rand.NextFloat(2f, 7f) * Projectile.ai[0],
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void GenerateSpark(int frequency)
	{
		float mulVelocity = Projectile.ai[0] / 5f;
		for (int g = 0; g < frequency; g++)
		{
			float sqrtRand = MathF.Pow(Main.rand.NextFloat(1), 0.4f);
			Vector2 newVelocity = new Vector2(0, mulVelocity * sqrtRand * 6).RotatedByRandom(MathHelper.TwoPi);
			var smog = new MothShimmerScaleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				Coord = new Vector2(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f)),
				MaxTime = Main.rand.Next(120, 185),
				Scale = Main.rand.NextFloat(3.4f, 18.4f),
				Rotation = Main.rand.NextFloat(6.283f),
				Rotation2 = Main.rand.NextFloat(6.283f),
				Omega = Main.rand.NextFloat(-30f, 30f),
				Phi = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.timeLeft <= 199)
		{
			Projectile.friendly = false;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		return bool0 || bool1 || bool2 || bool3;
	}

	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 0.5f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0.5f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0.5f, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D shadow = ModAsset.CursedHitLight.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Color c = new Color(0.2f * MathF.Sqrt(1 - timeValue), 0.6f * (1 - timeValue) * (1 - timeValue), 3f * (1 - timeValue), 0f);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, c * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f * dark, SpriteEffects.None, 0);
		Color cDark = new Color(0, 0, 0, 1f - timeValue);
		DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 20 * (1 - timeValue) * Projectile.ai[0], cDark, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_2_black_thick.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 4 * (1 - timeValue) * Projectile.ai[0], c * 0.4f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_6.Value);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D shadow = ModAsset.CursedHit.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Color c = new Color(0.2f * MathF.Sqrt(1 - timeValue), 0.6f * (1 - timeValue) * (1 - timeValue), 3f * (1 - timeValue), 0f);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] * 0.2f, SpriteEffects.None, 0);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] * 0.08f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 0.08f, SpriteEffects.None, 0);
		return false;
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		Color c0 = color;
		c0.R = 0;
		for (int h = 0; h < radius / 2; h += 1)
		{
			c0.R = (byte)(h / radius * 2 * 255);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(0, 0, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Trail.Value;

		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 30f * Projectile.ai[0], 40 * (1 - value) * Projectile.ai[0], new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<FireflyInferno>(), (int)(Projectile.ai[0] * 10f));
	}
}
