using Everglow.Commons.Templates.Weapons;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.TheFirefly.Buffs;
using Everglow.Myth.TheFirefly.Projectiles.PylonPostEffect;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class PylonStonePostProj_corrupt_explosion : NoTextureProjectile, IWarpProjectile
{
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
		Projectile.extraUpdates = 6;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void OnSpawn(IEntitySource source)
	{
		SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/Crystal_Burst_Normal").WithVolumeScale(Projectile.ai[0] / 20f + 0.2f), Projectile.Center);

		// GenerateVFXExpolode(10);
		for (int g = 0; g < 2; g++)
		{
			var darknessWave = new WaveOfEffectPylonHit_CorruptWave(Projectile.Center, Vector2.Zero, [Main.rand.NextFloat(0.1f, 1f), 4f, 80f + g * 15])
			{
				Active = true,
				Visible = true,
				MaxTime = 60,
				radius = 0,
			};
			Ins.VFXManager.Add(darknessWave);
		}
	}

	public void GenerateVFXExpolode(int Frequency, float mulVelocity = 1f)
	{
		if (Ins.VisualQuality.Low)
		{
			Frequency /= 3;
		}
		for (int g = 0; g < Frequency * 3; g++)
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
		for (int g = 0; g < Frequency; g++)
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
		for (int g = 0; g < Frequency * 3; g++)
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

	public override void AI()
	{
		Projectile.velocity *= 0;
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
		for (int h = 0; h <= 60; h++)
		{
			float factor = h / 60f;
			Vector2 v0 = new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(factor * MathHelper.TwoPi);
			Vector2 v1 = new Vector2(0, radius).RotatedBy(factor * MathHelper.TwoPi);
			v0 = v0.RotatedBy(addRot);
			v1 = v1.RotatedBy(addRot);
			circle.Add(new Vertex2D(center + v0, color, new Vector3(factor * 3f, 0.8f, 0)));
			circle.Add(new Vertex2D(center + v1, color, new Vector3(factor * 3f, 0.5f, 0)));
		}
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D shadow = ModAsset.CursedHitLight.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Color c = new Color(0.6f * MathF.Sqrt(1 - timeValue), 0.2f * (1 - timeValue) * (1 - timeValue), 1f * (1 - timeValue), 0f);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, c * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f * dark, SpriteEffects.None, 0);
		Color cDark = new Color(0, 0, 0, 1f - timeValue);
		DrawTexCircle(MathF.Sqrt(timeValue) * 12 * Projectile.ai[0], 20 * (1 - timeValue) * Projectile.ai[0], cDark, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_2_black_thick.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 12 * Projectile.ai[0], 4 * (1 - timeValue) * Projectile.ai[0], c * 0.4f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_6.Value);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D shadow = ModAsset.CursedHit.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Color c = new Color(1.2f * MathF.Sqrt(1 - timeValue), 0.6f * (1 - timeValue) * (1 - timeValue), 2f * (1 - timeValue), 0f);
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
			Vector2 v0 = new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4);
			Vector2 v1 = new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4);
			v0 = v0.RotatedBy(addRot);
			v1 = v1.RotatedBy(addRot);
			circle.Add(new Vertex2D(center + v0, c0, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + v1, c0, new Vector3(h * 2 / radius, 0.5f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(1, 0.5f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(0, 0.5f, 0)));
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

		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 11f * Projectile.ai[0], 12 * (1 - value) * Projectile.ai[0], new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<FireflyInferno>(), (int)(Projectile.ai[0] * 10f));
	}
}
