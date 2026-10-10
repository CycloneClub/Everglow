using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class WindBladeBallHit : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => ModAsset.YggdrasilMoonBlade_friendly_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 120;
		Projectile.height = 120;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 6;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public void GenerateSpark(int Frequency)
	{
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(10f, 14f) * Projectile.ai[0] / 16f).RotatedByRandom(MathHelper.TwoPi);
			var spark = new Spark_MoonBladeDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(37, 145) * Projectile.ai[0] / 10f,
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 17.0f)) * Projectile.ai[0] / 10f,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.timeLeft <= 199)
		{
			Projectile.friendly = false;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 9 * Projectile.ai[0];
		return bool0 || bool1 || bool2 || bool3;
	}

	private static void DrawTexCircle(float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radious / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radious % 1, 1f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radious % 1, 0f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(1, 0f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(0, 0f, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		Texture2D shadow = Commons.ModAsset.Point.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		var c = new Color(81, 81, 255, 0);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, c * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f * dark, SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = ModAsset.SandDissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoiseSize"].SetValue(4f);
		dissolve.Parameters["uNoiseXY"].SetValue(Vector2.zeroVector);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Sand.Value);
		dissolve.Parameters["duration"].SetValue(MathF.Pow(1f - timeValue, 5));
		dissolve.CurrentTechnique.Passes[0].Apply();
		DrawTexCircle(MathF.Sqrt(timeValue) * 12 * Projectile.ai[0], 24 * Projectile.ai[0], c, Projectile.Center, Commons.ModAsset.Trail_0.Value);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		var c = new Color(0f, 1f, 0.7f, 0f);

		Texture2D light = Commons.ModAsset.StabbingProjectile.Value;

		if (Projectile.hostile)
		{
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 0, light.Size() / 2f, new Vector2(1f, dark * dark * 0.4f) * Projectile.ai[0] * 0.04f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 1.57f, light.Size() / 2f, new Vector2(0.5f, dark * 0.4f) * Projectile.ai[0] * 0.04f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, Projectile.ai[1] + 1.57f, light.Size() / 2f, new Vector2(1f, dark * dark * 0.2f) * Projectile.ai[0] * 0.2f, SpriteEffects.None, 0);
		}
		else
		{
			dark = Math.Max(Projectile.timeLeft / 200f, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 0, light.Size() / 2f, new Vector2(1f, dark * dark * 0.4f) * Projectile.ai[0] * 0.08f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, 1.57f, light.Size() / 2f, new Vector2(0.5f, dark * 0.4f) * Projectile.ai[0] * 0.08f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, Projectile.ai[1] + 1.57f, light.Size() / 2f, new Vector2(1f, dark * dark * 0.2f) * Projectile.ai[0] * 0.4f, SpriteEffects.None, 0);
		}
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

		Texture2D t = Commons.ModAsset.Trail_6.Value;

		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 12f * Projectile.ai[0], 12 * (1 - value) * Projectile.ai[0], new Color(colorV, colorV * 0.1f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(BuffID.Bleeding, 360);
		target.AddBuff(BuffID.BrokenArmor, 360);
	}
}
