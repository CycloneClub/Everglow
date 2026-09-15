using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class YggdrasilMoonBlade : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 300;
		Projectile.aiStyle = -1;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	public Vector2 startVelocity;

	public override void OnSpawn(IEntitySource source)
	{
		startVelocity = Vector2.Normalize(Projectile.velocity);
		base.OnSpawn(source);
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(6.6f, 18f)).RotateRandom(MathHelper.TwoPi);
			float mulWidth = Main.rand.NextFloat(6.6f, 18f);
			var darknessNight = new Smog_MoonBladeDust
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
				maxTime = Main.rand.Next(27, 72),
				scale = mulWidth,
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(darknessNight);
		}
	}

	public override void AI()
	{
		if (Projectile.timeLeft == 295)
		{
			Projectile.extraUpdates = 4;
		}
		if (Projectile.timeLeft == 260)
		{
			Projectile.extraUpdates = 0;
		}
		if (Projectile.timeLeft < 260)
		{
			Projectile.velocity = Vector2.Normalize(Projectile.velocity) * Projectile.ai[2];
		}
		Lighting.AddLight(Projectile.Center, 0.14f, 0.47f, 0.97f);
		Vector2 newVelocity = Projectile.velocity + new Vector2(0, Main.rand.NextFloat(1.0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
		var spark = new Spark_MoonBladeDust
		{
			velocity = newVelocity,
			Active = true,
			Visible = true,
			position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
			maxTime = Main.rand.Next(7, 45),
			scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 7.0f)),
			rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
		};
		Ins.VFXManager.Add(spark);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float colorValue = 0.1f;
		int maxLength = 280 - Projectile.timeLeft;
		if (Projectile.timeLeft < 260)
		{
			maxLength = 20;
			colorValue = 1f;
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float timeValue = (float)Main.timeForVisualEffects * 0.002f + Projectile.whoAmI;
		var bars = new List<Vertex2D>();
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[1]) * 90f;
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(0, 24, 255, 0), new Vector3(0.2f + timeValue, x / 12f + Projectile.whoAmI * 0.5f, 0));
			bars.Add(pos - startVelocity * 80f * (2 - Math.Abs(x) / 20f) * colorValue, Color.Transparent, new Vector3(0 + timeValue, x / 12f + Projectile.whoAmI * 0.5f, 0));
		}
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_longitudinalFold.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		bars = new List<Vertex2D>();
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[1]) * 90f;
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(0, 255, 60, 0) * colorValue, new Vector3(0, 0.5f, 0));
			bars.Add(pos - startVelocity * 80f * (1 - Math.Abs(x) / 20f), new Color(100, 30, 255, 0) * 0.5f * colorValue, new Vector3(0, 0, 0));
		}
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		int maxLength = 280 - Projectile.timeLeft;
		if (Projectile.timeLeft < 260)
		{
			maxLength = 20;
		}
		float timeValue = (float)Main.timeForVisualEffects * 0.003f + Projectile.whoAmI * 0.3f;
		float redValue = startVelocity.ToRotation() / MathHelper.TwoPi;
		var bars = new List<Vertex2D>();
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[1]) * 90f;
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(redValue, 0.002f * Math.Abs(x), 0, 0), new Vector3(0.2f + timeValue, x / 35f, 0));
			bars.Add(pos - startVelocity * 50f, new Color(redValue, 0, 0, 0), new Vector3(0 + timeValue, x / 35f, 0));
		}
		Texture2D t = Commons.ModAsset.Noise_melting.Value;

		if (bars.Count > 3)
		{
			spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
		}
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		if (Main.expertMode)
		{
			target.AddBuff(BuffID.BrokenArmor, 180);
		}
		if (Main.masterMode)
		{
			target.AddBuff(BuffID.BrokenArmor, 240);
		}
		base.ModifyHitPlayer(target, ref modifiers);
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
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<YggdrasilMoonBladeHit>(), 0, 0, -1, 20, startVelocity.ToRotation());
	}
}
