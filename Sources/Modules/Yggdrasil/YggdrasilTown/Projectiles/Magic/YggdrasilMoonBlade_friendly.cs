using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class YggdrasilMoonBlade_friendly : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.magic = true;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 300;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 1;
	}

	public Vector2 startVelocity;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[0] = Main.rand.NextFloat(0.7f, 1.4f);
		startVelocity = Vector2.Normalize(Projectile.velocity);
		base.OnSpawn(source);
	}

	public void GenerateSmog(int Frequency)
	{
		if (ProjectileUtils.IsSafeInTheWorld(Projectile))
		{
			for (int g = 0; g < Frequency; g++)
			{
				Vector2 vel = new Vector2(0, Main.rand.NextFloat(6.6f, 9f)).RotateRandom(MathHelper.TwoPi);
				float mulWidth = Main.rand.NextFloat(6.6f, 18f);
				var darknessNight = new Smog_MoonBladeDust
				{
					Velocity = vel,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
					MaxTime = Main.rand.Next(27, 72),
					Scale = mulWidth,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.3f, 0.3f) },
				};
				Ins.VFXManager.Add(darknessNight);
			}
		}
	}

	public override void AI()
	{
		if (Projectile.timeLeft == 295)
		{
			Projectile.extraUpdates = 10;
		}
		if (Projectile.timeLeft == 270)
		{
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 0;
		}
		if (Projectile.timeLeft < 260)
		{
			Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 30f;
		}
		else
		{
			Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 1f;
		}
		Lighting.AddLight(Projectile.Center, 0.14f, 0.47f, 0.97f);
		GenerateSpark();
	}

	public void GenerateSpark()
	{
		if (ProjectileUtils.IsSafeInTheWorld(Projectile))
		{
			Vector2 newVelocity = Vector2.Normalize(Projectile.velocity) * 5f + new Vector2(0, Main.rand.NextFloat(1.0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new Spark_MoonBladeDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + Vector2.Normalize(Projectile.velocity).RotatedBy(Main.rand.NextFloat(-1f, 1f)) * 35,
				MaxTime = Main.rand.Next(7, 25),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(2f, 5.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.3f, 0.3f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float colorValue = (300 - Projectile.timeLeft) / 40f;
		int maxLength = 20;
		if (Projectile.timeLeft < 260)
		{
			colorValue = 1f;
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float timeValue = (float)Main.timeForVisualEffects * 0.002f + Projectile.whoAmI;
		List<Vertex2D> bars;

		bars = new List<Vertex2D>();
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[0]) * 60f;
			v0.X *= Projectile.ai[1];
			v0.Y /= Projectile.ai[1];
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(255, 255, 255, 255) * colorValue, new Vector3(0, 0.5f, 0));
			bars.Add(pos - startVelocity * 80f * (1 - Math.Abs(x) / 20f), new Color(255, 255, 255, 0) * 0.5f * colorValue, new Vector3(0, 0, 0));
		}
		Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_black.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		bars = new List<Vertex2D>();
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[0]) * 60f;
			v0.X *= Projectile.ai[1];
			v0.Y /= Projectile.ai[1];
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(81, 81, 255, 0), new Vector3(0.2f + timeValue, x / 12f + Projectile.whoAmI * 0.5f, 0));
			bars.Add(pos - startVelocity * 40f * (2 - Math.Abs(x) / 20f) * colorValue, Color.Transparent, new Vector3(0 + timeValue, x / 12f + Projectile.whoAmI * 0.5f, 0));
		}
		Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_longitudinalFold.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		bars = new List<Vertex2D>();
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[0]) * 60f;
			v0.X *= Projectile.ai[1];
			v0.Y /= Projectile.ai[1];
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(33, 232, 255, 0) * colorValue, new Vector3(0, 0.5f, 0));
			bars.Add(pos - startVelocity * 80f * (1 - Math.Abs(x) / 20f), new Color(100, 30, 255, 0) * 0.5f * colorValue, new Vector3(0, 0, 0));
		}
		Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		int maxLength = 20;
		float timeValue = (float)Main.timeForVisualEffects * 0.003f + Projectile.whoAmI * 0.3f;
		float redValue = startVelocity.ToRotation() / MathHelper.TwoPi;
		var bars = new List<Vertex2D>();
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[0]) * 60f;
			v0.X *= Projectile.ai[1];
			v0.Y /= Projectile.ai[1];
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(redValue, 0.01f * (Math.Abs(x) + 12), 0, 0), new Vector3(0.2f + timeValue, x / 35f, 0));
			bars.Add(pos - startVelocity * Math.Min(250f, (300f - Projectile.timeLeft) * 14), new Color(redValue, 0, 0, 0), new Vector3(0 + timeValue, x / 35f, 0));
		}
		Texture2D t = Commons.ModAsset.Noise_melting.Value;

		if (bars.Count > 3)
		{
			spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		return true;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override void OnKill(int timeLeft)
	{
		var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity, Vector2.zeroVector, ModContent.ProjectileType<YggdrasilMoonBladeHit>(), Projectile.damage / 2, 0, Projectile.owner, 9, startVelocity.ToRotation());
		p.friendly = true;
		p.hostile = false;
	}
}
