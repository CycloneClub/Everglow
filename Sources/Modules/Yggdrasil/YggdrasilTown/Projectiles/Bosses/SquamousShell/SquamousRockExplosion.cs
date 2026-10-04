using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class SquamousRockExplosion : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 120;
		Projectile.height = 120;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 6;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.timeLeft <= 198)
		{
			Projectile.friendly = false;
		}
		else
		{
			GenerateSmog(20);
			for (int x = 0; x < 2 * Projectile.ai[0]; x++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<SquamousShellStone>(), 0f, 0f, 0, default, Main.rand.NextFloat(0.7f, 1.7f));
				dust.velocity = new Vector2(0, Main.rand.NextFloat(0f, 2f * Projectile.ai[0])).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int x = 0; x < 2 * Projectile.ai[0]; x++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<SquamousShellStone_dark>(), 0f, 0f, 0, default, Main.rand.NextFloat(0.7f, 1.7f));
				dust.velocity = new Vector2(0, Main.rand.NextFloat(0f, 2f * Projectile.ai[0])).RotatedByRandom(MathHelper.TwoPi);
			}
		}
		Lighting.AddLight(Projectile.Center, new Vector3(0.05f, 0.07f, 1f) * Projectile.timeLeft / 60f);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float range = Projectile.ai[0] * 6;
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < range;
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < range;
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < range;
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < range;
		return bool0 || bool1 || bool2 || bool3;
	}

	private static void DrawTexCircle(float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radious / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radious % 1, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radious % 1, 0.5f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(1, 0.5f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(0, 0.5f, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	public override void PostDraw(Color lightColor)
	{
		var light = lightColor.ToVector4();
		float timeValue = (200 - Projectile.timeLeft) / 200f;

		// SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		// Main.spriteBatch.End();
		// Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		// DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 8 * (1 - timeValue) * Projectile.ai[0], lightColor * (1 - timeValue) * 0.75f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_black.Value);
		// DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 8 * (1 - timeValue) * Projectile.ai[0], new Color(0.32f * light.X, 0.18f * light.Y, 0.24f * light.Z, 0f) * (1 - timeValue), Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_6.Value);
		// Main.spriteBatch.End();
		// Main.spriteBatch.Begin(sBS);
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(0f, 1f)) * Projectile.ai[0]).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(57, 125),
				Scale = Main.rand.NextFloat(10f, 15f) * Projectile.ai[0],
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		// SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		// Main.spriteBatch.End();
		// Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		// Effect explosion = ModAsset.SquamousRollingStoneExplosionShader.Value;
		// var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		// var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		// explosion.Parameters["uTransform"].SetValue(model * projection);
		// explosion.Parameters["uTime"].SetValue((200 - Projectile.timeLeft) * 0.01f);
		// explosion.CurrentTechnique.Passes[0].Apply();

		// var texMain = Commons.ModAsset.Noise_forceField_sparse.Value;
		// Color drawColor = new Color(1f, 1f, 1f, 1f);
		// Vector2 drawCenter = Projectile.Center;
		// List<Vertex2D> bars = new List<Vertex2D>();
		// bars.Add(drawCenter + new Vector2(0, -200), drawColor, new Vector3(0, 0, 0));
		// bars.Add(drawCenter + new Vector2(200, 0), drawColor, new Vector3(1, 0, 0));

		// bars.Add(drawCenter + new Vector2(-200, 0), drawColor, new Vector3(0, 1, 0));
		// bars.Add(drawCenter + new Vector2(0, 200), drawColor, new Vector3(1, 1, 0));
		// Main.graphics.graphicsDevice.Textures[0] = texMain;
		// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		// Main.spriteBatch.End();
		// Main.spriteBatch.Begin(sBS);
		return false;
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		Color c0 = color;
		c0.R = 0;
		for (int h = 0; h < radious / 2; h += 1)
		{
			c0.R = (byte)(h / radious * 2 * 255);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radious, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radious, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), c0, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), c0, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), c0, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), c0, new Vector3(0, 0, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Trail.Value;
		float width = 60;
		if (Projectile.timeLeft < 60)
		{
			width = Projectile.timeLeft;
		}
		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 34 * Projectile.ai[0], width * 2, new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}
}
