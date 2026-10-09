using Everglow.Myth.Common;
using Everglow.Myth.Misc.Dusts.Slingshots;
using Terraria;
using Terraria.Audio;
using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

public class GlowSporeBead : SlingshotAmmo
{
	public override void SetDef()
	{
	}

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
		if (TimeTokill < 0)
		{
			Projectile.velocity.Y += 0.17f;
			int index = Dust.NewDust(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<JungleSpore>(), 0f, 0f, 100, default, Main.rand.NextFloat(0.6f, 1.1f));
			Main.dust[index].velocity = Projectile.velocity * 0.5f;
			for (float v = 0; v < Projectile.velocity.Length(); v += 1f)
			{
				if (v % 8 == 0)
				{
					int index2 = Dust.NewDust(Projectile.position - Projectile.velocity.SafeNormalize(Vector2.Zero) * v - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<JungleSmogStoppedByTile>(), 0f, 0f, 0, default, Main.rand.NextFloat(3.7f, 5.1f));
					Main.dust[index2].velocity = Projectile.velocity * 0.5f;
					Main.dust[index2].alpha = (int)(Main.dust[index2].scale * 50);
				}

				int type = ModContent.DustType<LittleJungleSpore>();
				if (Main.rand.NextBool(8))
				{
					int r2 = Dust.NewDust(Projectile.Center + Projectile.velocity - Projectile.velocity.SafeNormalize(Vector2.Zero) * v - new Vector2(4), 0, 0, type, 0, 0, 200, default, Projectile.ai[0] * 2 + 0.8f);
					Main.dust[r2].velocity = Projectile.velocity * Main.rand.NextFloat(0.07f, 0.16f);
					Main.dust[r2].noGravity = true;
				}
			}
		}
		else
		{
			if (TimeTokill < 10)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
		}
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

			float width = 8;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			var color = Color.Lerp(new Color(255, 255, 255, 0) * 0.6f, new Color(0, 0, 0, 0), factor);

			float fac1 = factor * 3 + (float)(-Main.timeForVisualEffects * 0.09) + 100000;
			float fac2 = (i + 1) / (float)TrueL * 3 + (float)(-Main.timeForVisualEffects * 0.09) + 100000;
			// TODO:925分钟之后会炸

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
			Texture2D t = ModAsset.SporeTrace.Value;

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
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 8;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			var color = Color.Lerp(Color.White * 0.6f, new Color(0, 0, 0, 0), factor);

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
			Texture2D t = ModAsset.SporeTraceShade.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public override void AmmoHit()
	{
		SoundEngine.PlaySound(SoundID.Drip, Projectile.Center);
		Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NormalHit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.velocity.Length(), Main.rand.NextFloat(6.283f));
		float Power = Projectile.ai[0] + 0.5f;
		for (int x = 0; x < 100 * Power; x++)
		{
			int index = Dust.NewDust(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<JungleSpore>(), 0f, 0f, 100, default, Power);
			Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(3.5f, 4f)).RotatedByRandom(6.283);
			int index2 = Dust.NewDust(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<JungleSmogStoppedByTile>(), 0f, 0f, 0, default, Main.rand.NextFloat(3.7f, 5.1f));
			Main.dust[index2].velocity = new Vector2(0, Main.rand.NextFloat(3.5f, 4f)).RotatedByRandom(6.283);
			Main.dust[index2].alpha = (int)(Main.dust[index2].scale * 50);

			int type = ModContent.DustType<LittleJungleSpore>();
			int r2 = Dust.NewDust(Projectile.Center - new Vector2(4), 0, 0, type, 0, 0, 200, default, Power * 2);
			Main.dust[r2].velocity = new Vector2(0, Main.rand.NextFloat(3.5f, 4f)).RotatedByRandom(6.283);
			Main.dust[r2].noGravity = true;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Poisoned, 540);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(BuffID.Poisoned, 540);
	}
}
