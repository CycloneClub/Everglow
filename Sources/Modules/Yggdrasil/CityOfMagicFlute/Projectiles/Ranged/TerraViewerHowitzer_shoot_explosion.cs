using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.CityOfMagicFlute.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.CityOfMagicFlute.Projectiles.Ranged;

public class TerraViewerHowitzer_shoot_explosion : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override string Texture => ModAsset.TerraViewerHowitzer_grenade_fall_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 160;
		Projectile.height = 160;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 2;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 30000;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 14400;
	}

	public void Spark()
	{
		for (int g = 0; g < 120; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 22f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireSparkDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(6.283) + newVelocity * 3,
				maxTime = Main.rand.Next(20, 40),
				scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(1f, 25.0f)),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public void FlameII()
	{
		var newVelocity = new Vector2(0, -10);
		Vector2 addPos = new Vector2(Main.rand.NextFloat(40), 0).RotatedByRandom(MathHelper.TwoPi);
		var fire = new FireDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + addPos - newVelocity,
			MaxTime = Main.rand.Next(30, 45),
			Scale = Main.rand.NextFloat(20f, 60f),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), addPos.X * 0.002f },
		};
		Ins.VFXManager.Add(fire);
	}

	public void LargeFlame()
	{
		Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 50f)).RotatedByRandom(MathHelper.TwoPi);
		var somg = new MissleFlameBlueDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + new Vector2(Main.rand.NextFloat(140), 0).RotatedByRandom(6.283),
			MaxTime = Main.rand.Next(60, 75),
			Scale = Main.rand.NextFloat(380f, 460f),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
		};
		Ins.VFXManager.Add(somg);
	}

	public void SmallFlame()
	{
		Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 50f)).RotatedByRandom(MathHelper.TwoPi);
		var somg = new MissleFlameBlueDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + new Vector2(Main.rand.NextFloat(60), 0).RotatedByRandom(6.283),
			MaxTime = Main.rand.Next(30, 85),
			Scale = Main.rand.NextFloat(50f, 90f),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
		};
		Ins.VFXManager.Add(somg);
	}

	public void NanoCloud()
	{
		Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 50f)).RotatedByRandom(MathHelper.TwoPi);
		var nano = new NanoCloudDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + new Vector2(MathF.Sqrt(Main.rand.NextFloat(1)) * 250, 0).RotatedByRandom(6.283),
			MaxTime = Main.rand.Next(30, 105),
			Scale = Main.rand.NextFloat(124f, 162f),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.1f, 0.1f), 0 },
		};
		Ins.VFXManager.Add(nano);
	}

	public void NanoCloudContinuely()
	{
		Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 5f)).RotatedByRandom(MathHelper.TwoPi);
		var nano = new NanoCloudDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + new Vector2(MathF.Sqrt(Main.rand.NextFloat(1)) * 420, 0).RotatedByRandom(6.283),
			MaxTime = Main.rand.Next(40, 80),
			Scale = 0.01f,
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f), Main.rand.NextFloat(24f, 82f) },
		};
		Ins.VFXManager.Add(nano);
	}

	public void NanoCloudRing()
	{
		Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0.2f, 0.5f)).RotatedByRandom(MathHelper.TwoPi);
		var nano = new NanoCloudDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + new Vector2(430, 0).RotatedByRandom(6.283),
			MaxTime = Main.rand.Next(40, 80),
			Scale = 0.01f,
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f), Main.rand.NextFloat(24f, 82f) },
		};
		Ins.VFXManager.Add(nano);
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[2] = 600;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.timeLeft == 190)
		{
			for (int x = 0; x < 150; x++)
			{
				LargeFlame();
				NanoCloud();
			}
			for (int x = 0; x < 600; x++)
			{
				SmallFlame();
			}
		}
		if (Projectile.timeLeft <= 190)
		{
			Projectile.friendly = false;
			for (int x = 0; x < 3; x++)
			{
				NanoCloudContinuely();
				NanoCloudRing();
			}
			if (Projectile.timeLeft < 3)
			{
				Projectile.timeLeft = 3;
				Projectile.ai[2]--;
				if (Projectile.ai[2] <= 0)
				{
					Projectile.active = false;
					Projectile.extraUpdates = 0;
				}
			}
		}
		Projectile.hide = true;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 420;
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 420;
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 420;
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 420;
		return bool0 || bool1 || bool2 || bool3;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		var c = new Color(0.3f * (1 - timeValue) * (1 - timeValue), 0.6f * (1 - timeValue) * (1 - timeValue), 3f * (1 - timeValue), 0f);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		for (int i = 0; i < 9; i++)
		{
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.whoAmI - Projectile.position.X) * 6 + Projectile.ai[1] + i * 0.6f, light.Size() / 2f, new Vector2(dark * dark, timeValue * 8) * Projectile.ai[0] * 1.62f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.whoAmI) * 6 + Projectile.ai[1] + i * 0.6f, light.Size() / 2f, new Vector2(MathF.Sqrt(dark), timeValue * 6) * Projectile.ai[0] * 1.62f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, MathF.Sin(Projectile.whoAmI + Projectile.position.Y) * 6 + Projectile.ai[1] + i * 0.6f, light.Size() / 2f, new Vector2(dark * dark, timeValue * 8) * Projectile.ai[0] * 1.62f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.type * 0.4f + Projectile.whoAmI) * 6 + Projectile.ai[1] + i * 0.6f, light.Size() / 2f, new Vector2(dark * dark, timeValue * 8) * Projectile.ai[0] * 1.62f, SpriteEffects.None, 0);
		}
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
		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 150f * Projectile.ai[0], 15 * (1 - value) * Projectile.ai[0], new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}
}
