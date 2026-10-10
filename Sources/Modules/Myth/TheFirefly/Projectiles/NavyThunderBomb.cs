using Everglow.Myth.TheFirefly.Dusts;
using Terraria.Audio;
using static Everglow.Myth.Common.MythUtils;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class NavyThunderBomb : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	private float sparkleStrength = 20;

	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/MothBall";

	public override bool CloneNewInstances => false;

	public override bool IsCloneable => false;

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 300;
		Projectile.tileCollide = false;
	}

	public override void AI()
	{
		Projectile.velocity *= 0.95f;
		if (Projectile.timeLeft > 260)
		{
			sparkleStrength += 1f;
		}

		if (Projectile.timeLeft is <= 240 and >= 60)
		{
			sparkleStrength = 60 + (float)(10 * Math.Sin((Projectile.timeLeft - 60) / 60d * Math.PI));
		}

		if (Projectile.timeLeft < 60 && sparkleStrength > 0.5f)
		{
			sparkleStrength -= 1f;
		}

		if (Projectile.timeLeft < 10)
		{
			Projectile.friendly = true;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Projectile.timeLeft > 1)
		{
			return false;
		}
		float maxLength = 150f;
		float length0 = (targetHitbox.TopLeft() - Projectile.Center).Length();
		float length1 = (targetHitbox.TopRight() - Projectile.Center).Length();
		float length2 = (targetHitbox.BottomLeft() - Projectile.Center).Length();
		float length3 = (targetHitbox.BottomRight() - Projectile.Center).Length();
		float minLength = Math.Min(Math.Min(length0, length1), Math.Min(length2, length3));
		if (minLength < maxLength)
		{
			return true;
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
		Player player = Main.player[Projectile.owner];
		ScreenShaker Gsplayer = player.GetModPlayer<ScreenShaker>();
		Gsplayer.FlyCamPosition = new Vector2(0, Math.Max(0, 200 - (Projectile.Center - player.Center).Length() * 0.5f)).RotatedByRandom(6.283);
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BombShakeWave>(), 0, 0, Projectile.owner);
		float X = Main.rand.NextFloat(6.283f);
		for (int h = 0; h < 6; h++)
		{
			Vector2 v = new Vector2(0, 12f).RotatedBy(h * MathHelper.TwoPi / 6f + X);
			if (!Main.hardMode)
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + v, v, ModContent.ProjectileType<BlueMissilFriendly>(), (int)(Projectile.damage * 0.35f), 0f, Projectile.owner);
			}
			else
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + v, v, ModContent.ProjectileType<BlueMissilFriendly>(), (int)(Projectile.damage * 0.55f), 0f, Projectile.owner);
			}
		}

		for (int h = 0; h < 160; h++)
		{
			if (Main.rand.NextBool(4))
			{
				Vector2 v3 = new Vector2(0, Main.rand.NextFloat(1.0f, 14.5f)).RotatedByRandom(6.283);
				int SparkleStrength = Dust.NewDust(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<GreyBlue>(), 0, 0, 0, default, 15f * Main.rand.NextFloat(0.7f, 2.9f));
				Main.dust[SparkleStrength].noGravity = true;
				Main.dust[SparkleStrength].velocity = v3;
			}
			else
			{
				float scale = Main.rand.NextFloat(12.7f, 24.1f);
				Vector2 v0 = new Vector2(Main.rand.NextFloat(2, 114f) / scale, 0).RotatedByRandom(6.283);
				Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<MothSmog>(), v0.X, v0.Y, 100, default, scale);
				dust.alpha = (int)(scale * 10);
				dust.rotation = Main.rand.NextFloat(0, 6.283f);
			}
		}

		for (int y = 0; y < 6; y++)
		{
			Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(Math.PI * 2d), 0, 0, ModContent.DustType<BlueGlow>(), 0f, 0f, 100, default, Main.rand.NextFloat(10.8f, 38.4f));
			dust.noGravity = true;
			dust.velocity = new Vector2(0, Main.rand.NextFloat(0.0f, Main.rand.NextFloat(0.0f, 14.5f))).RotatedByRandom(Math.PI * 2d);
			dust.rotation = Main.rand.NextFloat(6.283f);
		}
		for (int y = 0; y < 26; y++)
		{
			Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(Math.PI * 2d), 0, 0, ModContent.DustType<BlueGlow>(), 0f, 0f, 100, default, Main.rand.NextFloat(2.4f, 18.4f));
			dust.noGravity = true;
			dust.velocity = new Vector2(0, Main.rand.NextFloat(1.0f, Main.rand.NextFloat(1.0f, 8.5f))).RotatedByRandom(Math.PI * 2d);
			dust.rotation = Main.rand.NextFloat(6.283f);
		}
		for (int y = 0; y < 60; y++)
		{
			Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(Math.PI * 2d), 0, 0, ModContent.DustType<BlueGlow>(), 0f, 0f, 100, default, Main.rand.NextFloat(1.2f, 3.2f));
			dust.noGravity = true;
			dust.velocity = new Vector2(0, Main.rand.NextFloat(1.0f, Main.rand.NextFloat(1.0f, 17.5f))).RotatedByRandom(Math.PI * 2d);
			dust.rotation = Main.rand.NextFloat(6.283f);
		}
		for (int y = 0; y < 12; y++)
		{
			Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.NextFloat(48f)).RotatedByRandom(Math.PI * 2d), 0, 0, ModContent.DustType<BlueGlow>(), 0f, 0f, 100, default, Main.rand.NextFloat(2.3f, 7.0f));
			dust.noGravity = true;
			dust.velocity = new Vector2(Main.rand.NextFloat(0.0f, 2.5f), Main.rand.NextFloat(1.8f, 5.5f)).RotatedByRandom(Math.PI * 2d);
			dust.rotation = Main.rand.NextFloat(6.283f);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.Kill();
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float lightValue = (Projectile.timeLeft - 100f) / 200f;
		Texture2D Water = ModAsset.ElecLine.Value;
		Texture2D WaterS = Commons.ModAsset.Trail_5_black.Value;
		float value0 = (float)(Math.Sin(800d / (Projectile.timeLeft + 35)) * 0.75f + 0.25f) * (300 - Projectile.timeLeft) / 300f;
		value0 = Math.Max(0, value0);
		DrawTexCircle(122, 42, new Color(0.33f * value0, 0.33f * value0, 0.33f * value0, 0.33f * value0), Projectile.Center - Main.screenPosition, WaterS, Main.time / 17);
		DrawTexCircle(132, 32, new Color(0, 0.45f * value0, 1f * value0, 0), Projectile.Center - Main.screenPosition, Water, Main.time / 17);
		DrawTexCircle(122, 42, new Color(0, 0.15f * value0, 0.33f * value0, 0), Projectile.Center - Main.screenPosition, Water, -Main.time / 17);

		Texture2D dark = ModAsset.BlueFlameDark.Value;
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), Projectile.rotation, dark.Size() / 2f, sparkleStrength / 60f, SpriteEffects.None, 0f);
		Texture2D Light = ModAsset.CorruptLight.Value;
		if (lightValue > 0)
		{
			Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition, null, new Color(lightValue, lightValue, lightValue, 0), Projectile.rotation, Light.Size() / 2f, Projectile.scale * sparkleStrength / 210f, SpriteEffects.None, 0);
		}
		if (Projectile.timeLeft <= 40 && Projectile.timeLeft > 5)
		{
			float k3 = (40 - Projectile.timeLeft) / 40f;
			k3 *= k3;
			if (Projectile.timeLeft % 8 > 3)
			{
				Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition, null, new Color(0f, 1f, 1f, 0), Projectile.rotation, Light.Size() / 2f, k3, SpriteEffects.None, 0);
			}
			else
			{
				Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), Projectile.rotation, dark.Size() / 2f, k3, SpriteEffects.None, 0);
			}
			Texture2D Star = Commons.ModAsset.Trail.Value;
			Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, new Color(0f, k3, 1f, 0), 0, Star.Size() / 2f, new Vector2(k3 * 1, 0.4f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, new Color(0f, k3, 1f, 0), MathF.PI / 2, Star.Size() / 2, new Vector2(k3 * 0.7f, 0.4f), SpriteEffects.None, 0);
		}
		else if (Projectile.timeLeft <= 5)
		{
			Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), Projectile.rotation, dark.Size() / 2f, Projectile.timeLeft / 5f, SpriteEffects.None, 0f);
			Texture2D Star = Commons.ModAsset.Trail.Value;
			Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, new Color(0f, 1f, 1f, 0), 0, Star.Size() / 2f, new Vector2(0.75f, 0.4f * Projectile.timeLeft / 10f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, new Color(0f, 1f, 1f, 0), MathF.PI / 2, Star.Size() / 2, new Vector2(0.52f, 0.4f * Projectile.timeLeft / 10f), SpriteEffects.None, 0);
		}

		Projectile.frameCounter = (600 - Projectile.timeLeft) / 3 % 30;
		int FraX = Projectile.frameCounter % 6 * 270;
		int FraY = Projectile.frameCounter / 6 * 290;
		Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - Main.screenPosition, new Rectangle(FraX, FraY + 10, 270, 270), new Color(1f, 1f, 1f, 0), Projectile.rotation, new Vector2(135f, 135f), sparkleStrength / 420f, SpriteEffects.None, 0f);
		return false;
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radius / 2; h += 1)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch sb)
	{
		float value = (300 - Projectile.timeLeft) / 300f;
		value = MathF.Sqrt(value);
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= 10;
		}

		Texture2D t = Commons.ModAsset.SparkLight.Value;
		float width = 60;
		if (Projectile.timeLeft < 60)
		{
			width = Projectile.timeLeft;
		}

		DrawTexCircle_VFXBatch(sb, value * 27 * Projectile.ai[0], width * 2, new Color(colorV, colorV * 0.07f, colorV, 0f), Projectile.Center - Main.screenPosition, t);
	}
}
