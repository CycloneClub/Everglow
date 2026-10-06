using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.SpellAndSkull.Common;
using Terraria.DataStructures;

namespace Everglow.SpellAndSkull.Projectiles.GoldenShower;

public class GoldenShowerBomb : ModProjectile, IWarpProjectile
{
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
		Projectile.extraUpdates = 1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 200;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float maxDis = MathF.Sqrt(timeValue) * 24 * Projectile.ai[0];
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < maxDis;
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < maxDis;
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < maxDis;
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < maxDis;
		return bool0 || bool1 || bool2 || bool3;
	}

	public override void OnSpawn(IEntitySource source)
	{
		float times = Projectile.ai[0] / 2.5f;
		if (Ins.VisualQuality.Low)
		{
			times *= 0.5f;
		}

		for (int g = 0; g < 12 * times; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(1.1f)).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(6f, 14f);
			var blood = new IchorDrop
			{
				Velocity = afterVelocity * MathF.Sqrt(Projectile.ai[0]),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(82, 164),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		for (int g = 0; g < 6 * times; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(0.8f)).RotatedByRandom(MathHelper.TwoPi);
			var blood = new IchorSplash
			{
				Velocity = afterVelocity * MathF.Sqrt(Projectile.ai[0]),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(42, 164),
				Scale = Main.rand.NextFloat(6f, 12f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.4f), 0 },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHitLight.Value;
		float dark = Math.Max((Projectile.timeLeft - 100) / 50f, 0);
		float dark2 = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, new Color(255, 205, 0, 0) * dark2, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f * dark2, SpriteEffects.None, 0);

		float timeValue = (200 - Projectile.timeLeft) / 200f;
		Color cDark = new Color(0, 0, 0, 1f - timeValue) * 0.5f;
		DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 20 * (1 - timeValue) * Projectile.ai[0], cDark * dark, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_2_black_thick.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 4 * (1 - timeValue) * Projectile.ai[0], new Color(255, 145, 0, 0) * dark, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_10_black.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 4 * (1 - timeValue) * Projectile.ai[0], new Color(255, 145, 0, 0) * dark, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_10.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 48 * Projectile.ai[0], 40 * Projectile.ai[0], new Color(255, 145, 0, 0) * dark2, Projectile.Center - Main.screenPosition, Commons.ModAsset.Noise_hiveCyber.Value);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHit.Value;
		float dark = Math.Max((Projectile.timeLeft - 120) / 80f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f, SpriteEffects.None, 0);
		Texture2D light = ModAsset.LineLight_2.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(255, 205, 0, 0), 1.57f, light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 0.2f, SpriteEffects.None, 0);
		return false;
	}

	private void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
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

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Trail.Value;
		float width = 60;
		if (Projectile.timeLeft < 60)
		{
			width = Projectile.timeLeft;
		}

		SpellAndSkullUtils.DrawTexCircle_Warp(spriteBatch, MathF.Sqrt(value) * 64 * Projectile.ai[0], width * 2, new Color(colorV, colorV * 0.11f * value, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
		SpellAndSkullUtils.DrawTexCircle_Warp(spriteBatch, MathF.Sqrt(value) * 32 * Projectile.ai[0], width * 2, new Color(colorV, colorV * 0.2f * value, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Ichor, 900);
	}
}
