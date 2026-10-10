using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.CorruptWormHive.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.CorruptWormHive.Projectiles.Melee.TrueDeathSickle;

public class TrueDeathSickle_MoonBlade : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 300;
		Projectile.penetrate = 25;
		Projectile.aiStyle = -1;
	}

	public Vector2 startVelocity;

	public override void OnSpawn(IEntitySource source)
	{
		startVelocity = Vector2.Normalize(Projectile.velocity);
		base.OnSpawn(source);
	}

	public void GenerateSmog(int frequency)
	{
		for (int g = -frequency; g < frequency; g++)
		{
			if (Main.rand.NextBool(10))
			{
				Vector2 v0 = startVelocity.RotatedBy(g / 20f * Projectile.ai[1]) * Projectile.ai[0];
				Vector2 pos = Projectile.Center + v0 - startVelocity * 40f;
				var df = new DevilFlame3DSickle_worldCoordDust
				{
					Velocity3D = new Vector3(startVelocity * 15, 0),
					Active = true,
					Visible = true,
					Position3D = new Vector3(pos, 0),
					rotateAxis = new Vector3(0, 0, 1),
					Scale = Main.rand.NextFloat(6, 12),
					MaxTime = Main.rand.Next(16, 20),
					OwnerWhoAmI = Projectile.owner,
					ai = new float[] { Main.rand.NextFloat(0, 1f), Main.rand.NextFloat(-0.1f, 0.1f), 0f },
				};
				Ins.VFXManager.Add(df);
			}
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
			Projectile.tileCollide = true;
			Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 35f;
		}
		int maxLength = (280 - Projectile.timeLeft) * 4;
		if (Projectile.timeLeft < 275)
		{
			maxLength = 20;
		}
		GenerateSmog(maxLength);
		Lighting.AddLight(Projectile.Center, 0.14f, 0.47f, 0.97f);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float colorValue = 0.1f;
		int maxLength = (280 - Projectile.timeLeft) * 4;
		if (Projectile.timeLeft < 275)
		{
			maxLength = 20;
			colorValue = 1f;
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float timeValue = (float)Main.timeForVisualEffects * 0.002f + Projectile.whoAmI;
		var bars = new List<Vertex2D>();
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[1]) * Projectile.ai[0];
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(135, 0, 135, 150), new Vector3(0.2f + timeValue, x / 12f + Projectile.whoAmI * 0.5f, 0));
			bars.Add(pos - startVelocity * 80f * (2 - Math.Abs(x) / 20f) * colorValue, Color.Transparent, new Vector3(0 + timeValue, x / 12f + Projectile.whoAmI * 0.5f, 0));
		}
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_longitudinalFold.Value;
		if (bars.Count > 2)
		{
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		bars = new List<Vertex2D>();
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[1]) * Projectile.ai[0];
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(33, 232, 255, 0) * colorValue, new Vector3(0, 0.5f, 0));
			bars.Add(pos - startVelocity * 80f * (1 - Math.Abs(x) / 20f), new Color(100, 30, 255, 0) * 0.5f * colorValue, new Vector3(0, 0, 0));
		}
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
		if (bars.Count > 2)
		{
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
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
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[1]) * Projectile.ai[0];
			Vector2 pos = Projectile.Center + v0 - Main.screenPosition - startVelocity * 40f;
			bars.Add(pos, new Color(redValue, 0.002f * (Math.Abs(x) + 12), 0, 0), new Vector3(0.2f + timeValue, x / 35f, 0));
			bars.Add(pos - startVelocity * 50f, new Color(redValue, 0, 0, 0), new Vector3(0 + timeValue, x / 35f, 0));
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

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<TrueDeathSickleHit>(), 0, 0, -1, 10, startVelocity.ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f));
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		int maxLength = (280 - Projectile.timeLeft) * 4;
		if (Projectile.timeLeft < 275)
		{
			maxLength = 20;
		}
		for (int x = -20; x <= maxLength; x++)
		{
			Vector2 v0 = startVelocity.RotatedBy(x / 20f * Projectile.ai[1]) * Projectile.ai[0];
			Vector2 pos = Projectile.Center + v0 - startVelocity * 40f;
			var proj = new Rectangle((int)pos.X - 30, (int)pos.Y - 30, 60, 60);
			if (Rectangle.Intersect(proj, targetHitbox) != Rectangle.emptyRectangle)
			{
				return true;
			}
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<TrueDeathSickleHit>(), 0, 0, -1, 20, startVelocity.ToRotation());
	}
}
