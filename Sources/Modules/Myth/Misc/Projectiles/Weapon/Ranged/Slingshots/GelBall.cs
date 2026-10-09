using Everglow.Myth.Common;
using Terraria.Audio;
using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

public class GelBall : SlingshotAmmo
{
	public override void SetDef()
	{
		Projectile.penetrate = 22;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 12;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
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
			float MulColor = 1f;
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 4;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					MulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					MulColor = 0f;
				}
			}

			var factor = i / (float)TrueL;
			Color drawColor = Lighting.GetColor((int)(Projectile.oldPos[i].X / 16.0), (int)(Projectile.oldPos[i].Y / 16.0));
			drawColor.A = 0;
			var color = Color.Lerp(drawColor, new Color(0, 0, 0, 0), factor);

			float fac1 = factor * 3 + (float)(-Main.timeForVisualEffects * 0.03) + 100000;
			float fac2 = (i + 1) / (float)TrueL * 3 + (float)(-Main.timeForVisualEffects * 0.03) + 100000;
			// TODO:925分钟之后会炸

			fac1 %= 1f;
			fac2 %= 1f;
			if (fac1 > fac2)
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(fac1, 1, 0)));
				if (i < Projectile.oldPos.Length - 1)
				{
					float fac3 = 1 - fac1;
					fac3 /= 3 / (float)TrueL;
					normalDir = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
					normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(1, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(1, 1, 0)));

					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(0, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(0, 1, 0)));
				}
			}
			else
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(fac1, 1, 0)));
			}
		}

		if (bars.Count > 2)
		{
			Texture2D t = ModAsset.SlingshotTrailKS.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	private void DrawShade()
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
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			float MulColor = 1f;
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 4;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					MulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					MulColor = 0f;
				}
			}

			var factor = i / (float)TrueL;
			var color = Color.Lerp(Color.White * 0.6f, new Color(0, 0, 0, 0), factor);

			float fac1 = factor * 3 + (float)(-Main.timeForVisualEffects * 0.03) + 100000;
			float fac2 = (i + 1) / (float)TrueL * 3 + (float)(-Main.timeForVisualEffects * 0.03) + 100000;

			fac1 %= 1f;
			fac2 %= 1f;
			if (fac1 > fac2)
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(fac1, 1, 0)));
				if (i < Projectile.oldPos.Length - 1)
				{
					float fac3 = 1 - fac1;
					fac3 /= 3 / (float)TrueL;
					normalDir = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
					normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(1, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(1, 1, 0)));

					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(0, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(0, 1, 0)));
				}
			}
			else
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color * MulColor, new Vector3(fac1, 1, 0)));
			}
		}
		if (bars.Count > 2)
		{
			Texture2D t = Commons.ModAsset.Trail_black.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public override void AmmoHit()
	{
		SoundEngine.PlaySound(SoundID.Drip, Projectile.Center);
		Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NormalHit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.velocity.Length(), Main.rand.NextFloat(6.283f));
		float Power = Projectile.ai[0] + 0.5f;
		for (int x = 0; x < 14; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, 0, 0, 150, new Color(0, 0, 251, 0), Main.rand.NextFloat(0.75f, 1.15f));
			d.velocity = new Vector2(0, Main.rand.NextFloat(Main.rand.NextFloat(1f, 2f), 4f)).RotatedByRandom(6.283) * Power;
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.penetrate--;
		AmmoHit();
		if (Projectile.penetrate >= 2)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}

			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			Projectile.velocity *= 0.98f;
		}
		if (Projectile.penetrate < 2)
		{
			timeTokill = 30;
			Projectile.velocity *= 0;
			Projectile.tileCollide = false;
		}
		return false;
	}
}
