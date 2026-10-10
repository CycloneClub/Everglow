using Everglow.Myth.TheFirefly.Dusts;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class CorruptDust : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 60;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 6;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 90;
	}

	public override void AI()
	{
		if (Collision.SolidCollision(Projectile.Center + Projectile.velocity, 0, 0))
		{
			Projectile.velocity *= 0.93f;
			Projectile.friendly = false;
		}
		else
		{
			for (int t = 0; t < 3; t++)
			{
				float timeValue = Projectile.timeLeft / 60f;
				int index = Dust.NewDust(Projectile.Center - new Vector2(5), 2, 2, ModContent.DustType<BlueGlowAppear>(), 0f, 0f, 0, default, Main.rand.NextFloat(0.2f, 0.9f) * timeValue);
				Main.dust[index].velocity = Projectile.velocity * 0.7f + new Vector2(0, 1f).RotatedByRandom(6.283f);
				Main.dust[index].alpha = Main.rand.Next(240);
			}
			if (Main.rand.NextBool(2))
			{
				float timeValue = Projectile.timeLeft / 60f;
				int index = Dust.NewDust(Projectile.Center - new Vector2(5), 2, 2, ModContent.DustType<BlueGlowAppear>(), 0f, 0f, 0, default, Main.rand.NextFloat(1.2f, 1.9f) * timeValue);
				Main.dust[index].velocity = Projectile.velocity * 0.5f + new Vector2(0, 1f).RotatedByRandom(6.283f);
				Main.dust[index].alpha = Main.rand.Next(240);
			}
		}
		for (int i = 1; i < Projectile.oldPos.Length - 5; i += 5)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}
			float k0 = Projectile.timeLeft / 120f;
			int length = 60 - Projectile.timeLeft;
			var factor = i / (float)length;
			Lighting.AddLight(Projectile.Center, factor * 0.2f * k0, (1 - factor) * 0.3f * k0, k0);
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.velocity *= Main.rand.NextFloat(0.3f, 0.8f);
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		float dark = 0.7f;
		DrawTrail(new Color(dark, dark, dark, dark), ModAsset.CorruptDustDark.Value);
		DrawTrail(new Color(0, 0.6f, 1f, 0), ModAsset.CorruptDustLine.Value);
	}

	private void DrawTrail(Color c0, Texture2D tex, Texture fadeTexture = null)
	{
		fadeTexture ??= ModAsset.CorruptDustFadePowder.Value;
		float k0 = Projectile.timeLeft / 60f;
		var bars = new List<Vertex2D>();

		int length = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			length++;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			float width = 108 - k0 * 36;
			width *= Projectile.ai[0];
			if (i < 10)
			{
				width *= i / 10f;
			}

			if (Projectile.ai[0] == 3)
			{
				width *= 0.5f;
			}

			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)length;
			float x0 = factor * 1.6f - (float)(Main.timeForVisualEffects / 315d) + 10000 + MathF.Sin(Projectile.whoAmI);
			x0 %= 1f;
			c0.R = (byte)(factor * 120f);
			c0.G = (byte)((1 - factor) * 140f);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(Projectile.width / 2f, Projectile.height / 2f), c0, new Vector3(x0, 1, k0 - factor * 0.5f)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(Projectile.width / 2f, Projectile.height / 2f), c0, new Vector3(x0, 0, k0 - factor * 0.5f)));
			if (i < Projectile.oldPos.Length - 1)
			{
				if (Projectile.oldPos[i + 1] == Vector2.Zero)
				{
					break;
				}

				var factorII = (i + 1) / (float)length;
				var x1 = factorII * 1.6f - (float)(Main.timeForVisualEffects / 315d) + 10000 + MathF.Sin(Projectile.whoAmI);
				x1 %= 1f;
				if (x0 > x1)
				{
					float MidValue = (1 - x0) / (1 - x0 + x1);
					var factorIII = factorII * (1 - MidValue) + factor * MidValue;
					Vector2 MidPoint = Projectile.oldPos[i] * (1 - MidValue) + Projectile.oldPos[i + 1] * MidValue;
					c0.G = (byte)((1 - factor) * 140f);
					bars.Add(new Vertex2D(MidPoint + normalDir * -width * (1 - factorIII) + new Vector2(Projectile.width / 2f, Projectile.height / 2f), c0, new Vector3(1, 1, k0 - factorIII * 0.5f)));
					bars.Add(new Vertex2D(MidPoint + normalDir * width * (1 - factorIII) + new Vector2(Projectile.width / 2f, Projectile.height / 2f), c0, new Vector3(1, 0, k0 - factorIII * 0.5f)));
					bars.Add(new Vertex2D(MidPoint + normalDir * -width * (1 - factorIII) + new Vector2(Projectile.width / 2f, Projectile.height / 2f), c0, new Vector3(0, 1, k0 - factorIII * 0.5f)));
					bars.Add(new Vertex2D(MidPoint + normalDir * width * (1 - factorIII) + new Vector2(Projectile.width / 2f, Projectile.height / 2f), c0, new Vector3(0, 0, k0 - factorIII * 0.5f)));
				}
			}
		}

		if (bars.Count > 3)
		{
			Effect e = ModAsset.Powderlization.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
			e.Parameters["uTransform"].SetValue(model * projection);
			e.Parameters["tex0"].SetValue(fadeTexture);
			e.CurrentTechnique.Passes["Test"].Apply();
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
