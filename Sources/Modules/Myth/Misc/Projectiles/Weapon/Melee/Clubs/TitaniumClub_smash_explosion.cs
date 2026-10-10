using Everglow.Commons.DataStructures;
using SteelSeries.GameSense;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class TitaniumClub_smash_explosion : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override string Texture => "Everglow/" + ModAsset.Empty_Path;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 4;
	}

	public override bool ShouldUpdatePosition()
	{
		return false;
	}

	public override void AI()
	{
		if (Projectile.timeLeft == 200)
		{
			if (Projectile.ai[0] == 20)
			{
				GenerateSpark(1000);
			}
			if (Projectile.ai[0] == 30)
			{
				GenerateSpark(3000);
			}
		}
	}

	public void GenerateSpark(int frequency)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, -7 * Main.player[Projectile.owner].gravDir * Main.rand.NextFloat(0.5f, 2.3f)).RotatedBy(Main.rand.NextFloat(-2f, 2f));
			if (Projectile.ai[0] == 30)
			{
				newVelocity *= 1.5f;
			}
			var spark = new FireSpark_TitaniumDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(27, 85),
				scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4.1f, 57.0f)),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override void PostDraw(Color lightColor)
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		if (Projectile.timeLeft > 200)
		{
			return;
		}
		Texture2D Shadow = ModAsset.CursedHitLight.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, new Color(0.5f, 0.5f, 0.5f, 0) * dark * (lightColor.B / 255f), 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 10f * dark, SpriteEffects.None, 0);
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		timeValue = MathF.Pow(timeValue, 0.5f);

		DrawTexCircle(timeValue * 23 * Projectile.ai[0], 17 * Projectile.ai[0], Color.White * (1 - timeValue), Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_10_black.Value);

		Color blue = new Color(0.1f * (1 - timeValue) * (1 - timeValue), 0.6f * (1 - timeValue) * (1 - timeValue), 1 - timeValue, 0f) * (1 - timeValue);
		var deltaBlue = new Vector2(MathF.Sin((float)Main.time * 0.2f) * 5, 0);
		DrawTexCircle(timeValue * 24 * Projectile.ai[0], 17 * Projectile.ai[0], blue, Projectile.Center - Main.screenPosition + deltaBlue, Commons.ModAsset.Trail_8.Value);

		Vector2 deltaPink = new Vector2(MathF.Sin((float)Main.time * 0.2f) * 5, 0).RotatedBy(MathHelper.PiOver2 / 3f);
		Color pink = new Color(1 - timeValue, 0.6f * (1 - timeValue) * (1 - timeValue), 0.1f * (1 - timeValue) * (1 - timeValue), 0f) * (1 - timeValue);
		DrawTexCircle(timeValue * 25 * Projectile.ai[0], 17 * Projectile.ai[0], pink, Projectile.Center - Main.screenPosition + deltaPink, Commons.ModAsset.Trail_10.Value);

		Vector2 deltaYellow = new Vector2(MathF.Sin((float)Main.time * 0.2f) * 15, 0).RotatedBy(MathHelper.PiOver2 / 3f * 2);
		var yellow = new Color(0.4f * (1 - timeValue) * (1 - timeValue), 0.4f * (1 - timeValue) * (1 - timeValue), 0.1f * (1 - timeValue) * (1 - timeValue), 0f);
		DrawTexCircle(timeValue * 23 * Projectile.ai[0], 17 * Projectile.ai[0], yellow, Projectile.Center - Main.screenPosition + deltaYellow, Commons.ModAsset.Trail_8.Value);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (Projectile.timeLeft > 200)
		{
			return false;
		}
		Texture2D shadow = ModAsset.CursedHit.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 10f, SpriteEffects.None, 0);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float drawValue = (1 - timeValue) * (1 - timeValue);
		Color drawC = new Color(drawValue * lightColor.R / 255f, drawValue * lightColor.G / 255f, drawValue * lightColor.B / 255f, 0) * 3f;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, drawC, 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] * 0.15f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, drawC, MathHelper.PiOver2 + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark * 2f) * Projectile.ai[0] * 0.15f, SpriteEffects.None, 0);

		return false;
	}

	private void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		int maxStep = (int)(radius / 2);
		for (int h = 0; h <= maxStep; h++)
		{
			float mulColor = 1;
			Vector2 outBound = new Vector2(0, radius).RotatedBy(h / (float)maxStep * Math.PI * 2 + addRot);
			if (outBound.Y > 50)
			{
				mulColor = MathF.Max(0, (70 - outBound.Y) / 20f);
			}
			if (Projectile.ai[2] == -1)
			{
				mulColor = 1;
				if (outBound.Y < -50)
				{
					mulColor = MathF.Max(0, (70 + outBound.Y) / 20f);
				}
			}
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / (float)maxStep * Math.PI * 2 + addRot), color * mulColor, new Vector3(h * 2f / maxStep, 1f, 0)));
			circle.Add(new Vertex2D(center + outBound, color * mulColor, new Vector3(h * 2f / maxStep, 0f, 0)));
		}

		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	private void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radius / 2; h += 1)
		{
			Color c0 = color;
			c0.R = (byte)((h / radius * 2 * 255) + 190 % 255);
			float mulColor = 1;
			Vector2 outBound = new Vector2(-radius, 0).RotatedBy(h / radius * Math.PI * 4 + addRot);
			if (outBound.Y > 50)
			{
				mulColor = MathF.Max(0, (70 - outBound.Y) / 20f);
			}
			c0.G = (byte)(c0.G * mulColor);
			circle.Add(new Vertex2D(center + new Vector2(-Math.Max(radius - width, 0), 0).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + outBound, c0, new Vector3(h * 2 / radius, 0, 0)));
			if (c0.R > (byte)(((h + 1) / radius * 2 * 255) + 190 % 255))
			{
				c0.R = 255;
				circle.Add(new Vertex2D(center + new Vector2(-Math.Max(radius - width, 0), 0).RotatedBy(Math.PI), c0, new Vector3(h * 2 / radius, 1, 0)));
				circle.Add(new Vertex2D(center + new Vector2(-radius, 0).RotatedBy(Math.PI), c0, new Vector3(h * 2 / radius, 0, 0)));
				c0.R = 0;
				circle.Add(new Vertex2D(center + new Vector2(-Math.Max(radius - width, 0), 0).RotatedBy(Math.PI), c0, new Vector3(h * 2 / radius, 1, 0)));
				circle.Add(new Vertex2D(center + new Vector2(-radius, 0).RotatedBy(Math.PI), c0, new Vector3(h * 2 / radius, 0, 0)));
			}
		}

		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		if (Projectile.timeLeft > 200)
		{
			return;
		}
		float value = (200 - Projectile.timeLeft) / 200f;
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Trail_10.Value;

		float timeValue = (200 - Projectile.timeLeft) / 200f;
		timeValue = MathF.Pow(timeValue, 0.5f);

		DrawTexCircle_VFXBatch(spriteBatch, timeValue * 24 * Projectile.ai[0], 17 * Projectile.ai[0], new Color(colorV, colorV * 0.04f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
	}
}
