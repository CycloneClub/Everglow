using Everglow.Commons.Vertex;
using Everglow.Ocean.VFXs;

namespace Everglow.Ocean.Projectiles.Weapons;

public class TsunamiShark_bullet : ModProjectile
{
	public override string Texture => "Everglow/Ocean/Projectiles/Weapons/TsunamiShark/TsunamiShark_proj";

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 3600;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.extraUpdates = 3;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
	}

	/// <summary>
	/// 内部变量,别动
	/// </summary>
	internal int TimeTokill = -1;

	public override void AI()
	{
		if (TimeTokill >= 0 && TimeTokill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeTokill <= 15 && TimeTokill > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
		}

		TimeTokill--;
		float valueLight = 0.4f;
		if (TimeTokill >= 0)
		{
			if (TimeTokill < 10)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
			valueLight = TimeTokill / 20f;
		}
		Lighting.AddLight(Projectile.Center, 0, valueLight * valueLight * 0.1f, valueLight);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		AmmoHit();
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		AmmoHit();
	}

	public virtual void AmmoHit()
	{
		TimeTokill = 30;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.velocity = Projectile.oldVelocity;
		GenerateVFXKill(6);
		if (!Projectile.wet)
		{
			for (int x = 0; x < 3; x++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.SnowSpray, 0, 0, 0, default, Main.rand.NextFloat(0.4f, 0.8f));
				dust.velocity = new Vector2(0, Main.rand.NextFloat(4f)).RotatedByRandom(6.283);
			}
			for (int x = 0; x < 9; x++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Water, 0, 0, 0, default, Main.rand.NextFloat(0.8f, 1.5f));
				dust.velocity = new Vector2(0, Main.rand.NextFloat(4f)).RotatedByRandom(6.283);
			}
		}
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<TsunamiShark_bullet_hit>(), Projectile.damage, Projectile.knockBack);
	}

	public void GenerateVFXKill(int Frequency)
	{
		float mulVelocity = 1.5f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, 2f).RotatedByRandom(6.283);
			var wave = new WaveSprayDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(12, 24),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.23f), 0, Main.rand.NextFloat(2f, 4f) },
			};
			Ins.VFXManager.Add(wave);
		}
		mulVelocity = 0.6f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, 2f).RotatedByRandom(6.283);
			var wave = new WaveSprayDust
			{
				velocity = afterVelocity * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(12, 24),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.23f), 0, Main.rand.NextFloat(6f, 12f) },
			};
			Ins.VFXManager.Add(wave);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		DrawWaterDarkTrail();
		DrawWaterTrail();
	}

	public void DrawWaterDarkTrail()
	{
		var bars = new List<Vertex2D>();
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
				{
					return;
				}

				break;
			}

			TrueL = i;
		}
		for (int i = 1; i < TrueL; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 6;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			var color = Color.Lerp(new Color(1f, 1f, 1f, 1f) * 0.4f, new Color(0, 0, 0, 0), factor);

			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(5) - Main.screenPosition, color, new Vector3(factor, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(5) - Main.screenPosition, color, new Vector3(factor, 1, 0)));
		}

		if (bars.Count > 2)
		{
			Texture2D t = ModAsset.WaterLineBlackShade.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawWaterTrail()
	{
		var bars = new List<Vertex2D>();
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
				{
					return;
				}

				break;
			}

			TrueL = i;
		}
		for (int i = 1; i < TrueL; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 6;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			var color = Color.Lerp(new Color(0, (1 - factor) * 0.4f, (1 - factor * factor) * 2f, 0), new Color(0, 0, 0, 0), factor);

			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(5) - Main.screenPosition, color, new Vector3(factor, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(5) - Main.screenPosition, color, new Vector3(factor, 1, 0)));
		}

		if (bars.Count > 2)
		{
			Texture2D t = ModAsset.WaterLine.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
