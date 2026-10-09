using Everglow.Commons.Templates.Weapons.Slingshots;
using Everglow.Myth.Misc.Buffs;
using Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots.AmbiguousNightEffects;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

public class AmbiguousNightAmmo : SlingshotAmmo
{
	public float SpeedAtShoot = 0f;

	public override void SetDef()
	{
	}

	public override void OnSpawn(IEntitySource source)
	{
		SpeedAtShoot = Projectile.velocity.Length();
	}

	public override void AI()
	{
		if (Projectile.velocity.Length() > 1)
		{
			GenerateVFX(2);
		}
		int index = Dust.NewDust(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WaterCandle, 0f, 0f, 0, default, Main.rand.NextFloat(0.4f, 0.8f));
		Main.dust[index].velocity = Projectile.velocity * Main.rand.NextFloat(0.85f, 1.15f) * 0.55f;
		base.AI();
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public void GenerateVFX(int Frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = Projectile.velocity;
			if (afterVelocity.Length() > 25)
			{
				afterVelocity = afterVelocity * 25 / afterVelocity.Length();
			}
			float mulWidth = 1f;
			if (afterVelocity.Length() < 10)
			{
				mulWidth = afterVelocity.Length() / 10f;
			}
			if (Projectile.timeLeft > 3580)
			{
				mulWidth *= (3600 - Projectile.timeLeft) / 20f;
			}
			var darknessNight = new DarknessOfNightDust
			{
				velocity = afterVelocity * Main.rand.NextFloat(0.25f, 0.45f) * mulVelocity + Projectile.velocity.SafeNormalize(new Vector2(0, -1)),
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
				maxTime = Main.rand.Next(27, 72),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(6.6f, 18f) * mulWidth },
			};
			Ins.VFXManager.Add(darknessNight);
		}
	}

	public void GenerateVFXKill()
	{
		float size = 1f;
		if (Projectile.velocity.Length() > 12f)
		{
			size = Projectile.velocity.Length() / 12f;
		}
		var darknessWave = new DarknessOfNightWave
		{
			velocity = Vector2.Zero,
			Active = true,
			Visible = true,
			position = Projectile.Center,
			maxTime = 100,
			radius = 0,
			ai = new float[] { Main.rand.NextFloat(0.1f, 1f), 2f * size, 44f * size },
		};
		Ins.VFXManager.Add(darknessWave);
	}

	public override void DrawTrail()
	{
		DrawShade();
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
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 3;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			width *= 1 - factor;
			var color = new Color(255, 255, 255, 0);

			float fac1 = factor * 3 + Math.Abs((float)(-Main.timeForVisualEffects * 0.03) + 100000);
			float fac2 = (i + 1) / (float)TrueL * 3 + Math.Abs((float)(-Main.timeForVisualEffects * 0.03) + 100000);

			fac1 %= 1f;
			fac2 %= 1f;
			if (fac1 > fac2)
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 1, 0)));
				if (i < Projectile.oldPos.Length - 1)
				{
					float fac3 = 1 - fac1;
					fac3 /= 3 / (float)TrueL;
					normalDir = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
					normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(1, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(1, 1, 0)));

					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(0, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(0, 1, 0)));
				}
			}
			else
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 1, 0)));
			}
		}

		if (bars.Count > 2)
		{
			Texture2D t = ModAsset.ShadowTrail.Value;

			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		DrawShade(1);
	}

	private void DrawShade(int times = 0)
	{
		var bars = new List<Vertex2D>();
		float TrueL = 0;
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
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 12;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			width *= 1 - factor;
			var color = Color.White;

			float fac1 = factor * 3 + (float)(-Main.timeForVisualEffects * 0.09) + 100000;
			float fac2 = (i + 1) / (float)TrueL * 3 + (float)(-Main.timeForVisualEffects * 0.09) + 100000;

			fac1 %= 1f;
			fac2 %= 1f;
			if (fac1 > fac2)
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 1, 0)));
				if (i < Projectile.oldPos.Length - 1)
				{
					float fac3 = 1 - fac1;
					fac3 /= 3 / (float)TrueL;
					normalDir = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
					normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(1, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(1, 1, 0)));

					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(0, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(0, 1, 0)));
				}
			}
			else
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 1, 0)));
			}
		}
		if (bars.Count > 2)
		{
			Texture2D t = ModAsset.SlingshotTrailBlack.Value;
			if (times == 1)
			{
				t = ModAsset.ShadowTrailFlame.Value;
			}

			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public override void AmmoHit()
	{
		GenerateVFXKill();
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot.WithVolumeScale(Projectile.ai[0]), Projectile.Center);
		Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NormalHit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, SpeedAtShoot, Main.rand.NextFloat(6.283f));
		float blackHoleSize = 0.08f;
		if (SpeedAtShoot > 12f)
		{
			blackHoleSize = SpeedAtShoot / 12f * 0.08f;
		}
		Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AmbiguousNightHit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, blackHoleSize, Main.rand.NextFloat(6.283f));
		float Power = Projectile.ai[0] + 0.5f;

		for (int x = 0; x < 100 * Power; x++)
		{
			int index = Dust.NewDust(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WaterCandle, 0f, 0f, 0, default, Power * Main.rand.NextFloat(0.7f, 1.3f));
			Main.dust[index].noGravity = true;
			Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(3.5f, 4f)).RotatedByRandom(6.283) * Power;
		}
		Projectile.friendly = false;
		timeTokill = 30;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<ShadowSupervisor>(), (int)(600 * Projectile.ai[0]) + 120);
		AmmoHit();
	}
}
