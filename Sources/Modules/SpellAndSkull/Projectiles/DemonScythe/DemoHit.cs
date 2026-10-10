using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using static Everglow.SpellAndSkull.Common.SpellAndSkullUtils;

namespace Everglow.SpellAndSkull.Projectiles.DemonScythe;

public class DemoHit : ModProjectile, IWarpProjectile, IBloomProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override bool CloneNewInstances => false;

	public override bool IsCloneable => false;

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 4;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void AI()
	{
		Projectile.velocity *= 0.95f;

		if (Projectile.timeLeft <= 198)
		{
			Projectile.friendly = false;
		}

		int maxC = (int)(Projectile.ai[0] / 6 + 5);
		maxC = Math.Min(26, maxC);
		if (Projectile.timeLeft >= 200)
		{
			for (int x = 0; x < maxC; x++)
			{
				sparkVelocity[x] = new Vector2(0, Projectile.ai[0] * 1.6f).RotatedByRandom(6.283) * Main.rand.NextFloat(0.05f, 1.2f);
				sparkOldPos[x, 0] = Projectile.Center;
			}
		}

		for (int x = 0; x < maxC; x++)
		{
			for (int y = 39; y > 0; y--)
			{
				sparkOldPos[x, y] = sparkOldPos[x, y - 1];
			}
			if (Collision.SolidCollision(sparkOldPos[x, 0] + new Vector2(sparkVelocity[x].X, 0), 0, 0))
			{
				sparkVelocity[x].X *= -0.95f;
			}

			if (Collision.SolidCollision(sparkOldPos[x, 0] + new Vector2(0, sparkVelocity[x].Y), 0, 0))
			{
				sparkVelocity[x].Y *= -0.95f;
			}

			sparkOldPos[x, 0] += sparkVelocity[x];

			if (sparkVelocity[x].Length() > 0.3f)
			{
				sparkVelocity[x] *= 0.95f;
			}

			sparkVelocity[x].Y += 0.04f;
		}
		Projectile.velocity *= 0;
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHitLight.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, new Color(55, 0, 145, 0) * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f * dark, SpriteEffects.None, 0);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHit.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f, SpriteEffects.None, 0);
		Texture2D light = ModAsset.CursedHitStar.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(55, 0, 145, 0), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] / 20f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(55, 0, 145, 0), 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] / 20f, SpriteEffects.None, 0);
		float size = Math.Clamp(Projectile.timeLeft / 8f - 10, 0f, 20f) * 1.6f;
		if (size > 0)
		{
			DrawSpark(Color.White * 0.5f, size, ModAsset.SparkDark.Value);
			DrawSpark(new Color(55, 0, 145, 0), size, ModAsset.SparkLight.Value);
		}
		return false;
	}

	private Vector2[,] sparkOldPos = new Vector2[27, 40];
	private Vector2[] sparkVelocity = new Vector2[27];

	internal void DrawSpark(Color c0, float width, Texture2D tex)
	{
		int maxC = (int)(Projectile.ai[0] / 6 + 5);
		maxC = Math.Min(26, maxC);
		var bars = new List<Vertex2D>();
		for (int x = 0; x < maxC; x++)
		{
			int TrueL = 0;
			for (int i = 1; i < 40; ++i)
			{
				if (sparkOldPos[x, i] == Vector2.Zero)
				{
					break;
				}

				TrueL++;
			}
			for (int i = 1; i < 40; ++i)
			{
				if (sparkOldPos[x, i] == Vector2.Zero)
				{
					break;
				}

				var normalDir = sparkOldPos[x, i - 1] - sparkOldPos[x, i];
				normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
				var factor = i / (float)TrueL;
				var w = MathHelper.Lerp(1f, 0.05f, factor);
				float x0 = 1 - factor;
				if (i == 1)
				{
					bars.Add(new Vertex2D(sparkOldPos[x, i] + normalDir * -width + new Vector2(5f, 5f) - Main.screenPosition, Color.Transparent, new Vector3(x0, 1, w)));
					bars.Add(new Vertex2D(sparkOldPos[x, i] + normalDir * width + new Vector2(5f, 5f) - Main.screenPosition, Color.Transparent, new Vector3(x0, 0, w)));
				}
				bars.Add(new Vertex2D(sparkOldPos[x, i] + normalDir * -width + new Vector2(5f, 5f) - Main.screenPosition, c0, new Vector3(x0, 1, w)));
				bars.Add(new Vertex2D(sparkOldPos[x, i] + normalDir * width + new Vector2(5f, 5f) - Main.screenPosition, c0, new Vector3(x0, 0, w)));
				if (i == 39)
				{
					bars.Add(new Vertex2D(sparkOldPos[x, i] + normalDir * -width + new Vector2(5f, 5f) - Main.screenPosition, Color.Transparent, new Vector3(x0, 1, w)));
					bars.Add(new Vertex2D(sparkOldPos[x, i] + normalDir * width + new Vector2(5f, 5f) - Main.screenPosition, Color.Transparent, new Vector3(x0, 0, w)));
				}
			}
			Texture2D t = tex;
			Main.graphics.GraphicsDevice.Textures[0] = t;
		}
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / (float)Projectile.timeLeft * 1.4f;
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Wave.Value;
		DrawTexCircle(value * 16 * Projectile.ai[0], 100, new Color(colorV, colorV * 0.02f, colorV, 0f), Projectile.Center - Main.screenPosition, t);
	}

	public void DrawBloom()
	{
		/*
            float size = Math.Clamp(Projectile.timeLeft / 8f - 60, 0f, 20f);
            if (size > 0)
            {
                DrawSpark(new Color(255, 255, 255, 0), size, ModAsset.SparkLight.Value);
            }*/
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}
}
