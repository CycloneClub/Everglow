using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.SpellAndSkull.Projectiles.LunarFlare;

public class LunarFlareII : ModProjectile, IWarpProjectile// 将接口改为使用IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 26;
		Projectile.height = 26;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 120;
		Projectile.alpha = 0;
		Projectile.penetrate = 5;
		Projectile.scale = 1f;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[0] = Main.MouseWorld.X;
		Projectile.ai[1] = Main.MouseWorld.Y;
	}

	public override void AI()
	{
		if ((Projectile.Center - new Vector2(Projectile.ai[0], Projectile.ai[1])).Length() < 50f)
		{
			Projectile.tileCollide = true;
		}

		float MulScale = 1f;
		if (Projectile.timeLeft < 30)
		{
			MulScale = Projectile.timeLeft / 30f;
		}

		if (Main.rand.NextBool(5))
		{
			Vector2 BasePos = Projectile.Center - new Vector2(4) - Projectile.velocity + new Vector2(0, Main.rand.NextFloat(6f)).RotatedByRandom(6.283);
			var d0 = Dust.NewDustDirect(BasePos, 0, 0, DustID.UltraBrightTorch, 0, 0, 0, default, 0.8f * MulScale);
			d0.noGravity = true;
			d0.velocity = Projectile.velocity * 0.2f + new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(6.283);
		}
		if (Main.rand.NextBool(3))
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedByRandom(6.283) * Projectile.scale * MulScale * 0.2f;
			int dust0 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, DustID.UltraBrightTorch, v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.1f, 1f) * Projectile.scale * MulScale);
			Main.dust[dust0].noGravity = true;
			Main.dust[dust0].alpha = 175;
		}

		Projectile.velocity *= 0.99f;
		if (Projectile.penetrate != 1 && Projectile.friendly)
		{
			Lighting.AddLight(Projectile.Center, 0, 1f, 1f);

			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var c0 = new Color(0, 155, 155, 0);

		float width = 4;

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

		DrawFlameTrail(TrueL, width * 3, false, c0);
		return false;
	}

	private void DrawFlameTrail(int TrueL, float width, bool Shade = false, Color c0 = default(Color), float Mulfactor = 1.6f)
	{
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float mulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				mulColor = 0f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			var factor = i / (float)TrueL;
			float x0 = factor * Mulfactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * Mulfactor - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0 * mulColor, new Vector3(0, 0, 0)));
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

	public void DrawWarp(VFXBatch spriteBatch)
	{
		/*不再用开启shader
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Effect KEx = Everglow.Commons.ModAsset.DrawWarp.Value;
            KEx.CurrentTechnique.Passes[0].Apply();
            */
		float width = 16;

		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}
		var bars = new List<Vertex2D>();
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float mulColor = 1f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			if (i == 1)
			{
				mulColor = 0f;
			}

			if (i >= 2)
			{
				var normalDirII = Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}
			if (i < Projectile.oldPos.Length - 1)
			{
				var normalDirII = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
				normalDirII = Vector2.Normalize(new Vector2(-normalDirII.Y, normalDirII.X));
				if (Vector2.Dot(normalDirII, normalDir) <= 0.965f)
				{
					mulColor = 0f;
				}
			}

			float k0 = (float)Math.Atan2(normalDir.Y, normalDir.X);
			k0 += 3.14f + 1.57f;
			if (k0 > 6.28f)
			{
				k0 -= 6.28f;
			}

			var c0 = new Color(k0, 0.04f * mulColor, 0, 0);

			var factor = i / (float)TrueL;
			float x0 = factor * 1.3f - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x0 %= 1f;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(x0, 0, 0)));
			var factorII = factor;
			factor = (i + 1) / (float)TrueL;
			var x1 = factor * 1.3f - (float)(Main.timeForVisualEffects / 15d) + 100000;
			x1 %= 1f;
			if (x0 > x1)
			{
				float DeltaValue = 1 - x0;
				var factorIII = factorII * x0 + factor * DeltaValue;
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(1, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factorIII) + new Vector2(13f) - Main.screenPosition, c0, new Vector3(0, 0, 0)));
			}
		}
		Texture2D t = ModAsset.FogTraceLight.Value;

		// 贴图不用在这里传
		// Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
		{
			// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			// 里面的所有绘制（包括顶点绘制）改为用VFXBatch spriteBatch的Draw，如下
			spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
		}

		// 结尾的begin-end也删掉
		// Main.spriteBatch.End();
		// Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithVolumeScale(0.2f), Projectile.Center);
		Projectile.position += Projectile.velocity;
		if (Projectile.velocity.Length() > 3)
		{
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LunarFlareHit>(), Projectile.damage, Projectile.knockBack * 1.5f, Projectile.owner, 10f, Main.rand.NextFloat(6.283f));
		}

		Projectile.velocity *= 0;
		if (Projectile.timeLeft > 60)
		{
			Projectile.timeLeft = 60;
		}

		Projectile.friendly = false;

		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithVolumeScale(0.2f), Projectile.Center);
		Projectile.velocity *= 0;
		if (Projectile.timeLeft > 60)
		{
			Projectile.timeLeft = 60;
		}

		Projectile.friendly = false;
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LunarFlareHit>(), Projectile.damage, Projectile.knockBack * 1.5f, Projectile.owner, 10f, Main.rand.NextFloat(6.283f));
	}
}
