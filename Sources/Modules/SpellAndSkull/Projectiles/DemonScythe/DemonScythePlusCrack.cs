using Everglow.Commons.Vertex;
using Everglow.SpellAndSkull.Common;
using Terraria;

namespace Everglow.SpellAndSkull.Projectiles.DemonScythe;

internal class DemonScythePlusCrack : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 16;
		Projectile.timeLeft = 1;
		Projectile.aiStyle = -1;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 3;
		Projectile.tileCollide = false;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
	}

	public override void AI()
	{
		float vL = Projectile.velocity.Length() * 0.1f;
		vL = Math.Min(vL, 4f);
		float kSize = Math.Min(vL, 1f);
		float kTime = Math.Min(Projectile.timeLeft / 30f, 1f);
		for (float x = -vL; x < vL + 1; x += 1)
		{
			if (x < vL * 0.6f)
			{
				continue;
			}

			float size = Main.rand.NextFloat(1.45f, 1.75f) * kSize * Projectile.ai[0] * 0.5f;
			var d0 = Dust.NewDustDirect(Projectile.Center - new Vector2(size * 4, size * 4.5f), 0, 0, ModContent.DustType<Dusts.DemoFlame>(), 0, 0, 0, default, size);
			d0.fadeIn = 12f;
			d0.scale *= kTime;
		}

		Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 0.11f * kTime, 0f, 0.45f * kTime);
		if (Collision.SolidCollision(Projectile.Center, 0, 0))
			Projectile.velocity *= 0.6f;
		else
		{
			Projectile.rotation += 0.3f;
			Projectile.velocity.Y += 0.35f;
			Projectile.velocity.X += Main.windSpeedCurrent * 0.1f;
			Projectile.velocity += new Vector2(0, Projectile.ai[0] * 4f).RotatedBy(Main.timeForVisualEffects * Projectile.ai[1] / 6d);
		}
	}

	public override void OnKill(int timeLeft)
	{

	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.Knockback *= Projectile.velocity.Length() * 0.12f;
		float k = Math.Clamp(Projectile.velocity.Length() / 48f, 1f, 5f);
		modifiers.FinalDamage *= k;
		target.AddBuff(BuffID.ShadowFlame, 30);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var c0 = new Color(0.4f, 0.0f, 0.8f, 0);
		float width = 16 * Projectile.ai[0];
		if (Projectile.timeLeft < 30)
		{
			width *= Projectile.timeLeft / 30f;
		}

		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}


		DrawFlameTrail(TrueL, width, true, Color.White);

		DrawFlameTrail(TrueL, width, false, c0);

		float Scl = 1f;
		if (Projectile.timeLeft < 30)
		{
			Scl *= Projectile.timeLeft / 30f;
		}

		Vector2 BasePos = Projectile.Center - Main.screenPosition;
		var circle = new List<Vertex2D>();
		circle.Add(new Vertex2D(BasePos + new Vector2(10, -10).RotatedBy(Projectile.rotation) * Scl, Color.Violet, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(BasePos + new Vector2(0, 15).RotatedBy(Projectile.rotation) * Scl, Color.Violet, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(BasePos + new Vector2(-10, -10).RotatedBy(Projectile.rotation) * Scl, Color.Violet, new Vector3(0.5f, 1, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_5.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, circle.ToArray(), 0, circle.Count / 3);
		}
		return false;
	}
	private void DrawFlameTrail(int TrueL, float width, bool Shade = false, Color c0 = new Color(), float Mulfactor = 1.6f)
	{
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float MulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				MulColor = 0f;
			}

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
			float x0 = factor * Mulfactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0 * MulColor, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0 * MulColor, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * Mulfactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * MulColor, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * MulColor, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * MulColor, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * MulColor, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = Commons.ModAsset.Trail_6.Value;
		if (Shade)
		{
			t = ModAsset.Darkline.Value;
		}

		Main.graphics.GraphicsDevice.Textures[0] = t;

		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
