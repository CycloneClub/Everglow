using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class Pycnidium_explosion : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => ModAsset.BacterialAgent_proj_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 120;
		Projectile.height = 120;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 12;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void OnSpawn(IEntitySource source)
	{
		SoundEngine.PlaySound(SoundID.DD2_GoblinBomb.WithVolume(0.5f), Projectile.Center);
	}

	public void GenerateLiquid(int frequency)
	{
		for (int x = 0; x < frequency; x++)
		{
			Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 6f), 0).RotatedByRandom(MathHelper.TwoPi);
			var splash = new LichenSlimeSplash
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(12, 40),
				Scale = Main.rand.NextFloat(6f, 18f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(splash);
		}
		for (int x = 0; x < frequency * 2; x++)
		{
			Vector2 velocity = new Vector2(Main.rand.NextFloat(3f, 6f), 0).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(6f, 9f);
			var blood = new LichenSlimeDrop
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(32, 64),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public void GenerateSpark(int frequency)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 velocity = new Vector2(Main.rand.NextFloat(3f, 6f), 0).RotatedByRandom(MathHelper.TwoPi);
			var smog = new LichenSlimeStar
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(20, 85),
				Scale = Main.rand.NextFloat(0.4f, 0.8f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.timeLeft == 199)
		{
			GenerateLiquid(3);
			GenerateSpark(6);
			Projectile.friendly = false;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 24;
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 24;
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 24;
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 24;
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

	public override void PostDraw(Player player, Color lightColor)
	{
		Texture2D shadow = Commons.ModAsset.Textures_Star.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		var c = new Color(0.7f * MathF.Sqrt(1 - timeValue) * (1 - timeValue), 1f * (1 - timeValue), 0.3f * (1 - timeValue), 0f);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, c * dark, 0, shadow.Size() / 2f, 0.4f * dark, SpriteEffects.None, 0);
		var cDark = new Color(0, 0, 0, 1f - timeValue);
		DrawTexCircle(MathF.Sqrt(timeValue) * 40, 70 * (1 - timeValue), cDark, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_2_black_thick.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 40, 20 * (1 - timeValue), c * 0.4f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_6.Value);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		var c = new Color(0.7f * MathF.Sqrt(1 - timeValue) * (1 - timeValue), 1f * (1 - timeValue), 0.3f * (1 - timeValue), 0f);

		Texture2D light = Commons.ModAsset.Textures_Star.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * 0.4f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * 0.4f, SpriteEffects.None, 0);
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
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 0.5f, 0)));
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
		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 40f, 22 * (1 - value), new Color(colorV, colorV * 0.02f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		hit.Damage = 15;

		// target.AddBuff(ModContent.BuffType<LichenInfected>(), 300);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		base.ModifyHitNPC(target, ref modifiers);
	}
}
