using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.VFX;
using Newtonsoft.Json.Linq;
using Spine;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class GreenFlameSharpCrystal : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		TrailColor = new Color(1, 0.07f, 0.1f, 0f);
		TrailBackgroundDarkness = 0.1f;
		TrailWidth = 16f;
		SelfLuminous = false;
		TrailLength = 24;
		TrailTexture = Commons.ModAsset.Trail_10.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_10_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
		Projectile.timeLeft = 300;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = false;
		Projectile.hostile = true;
	}

	public override void Behaviors()
	{
		Lighting.AddLight(Projectile.Center, new Vector3(0.6f, 0, 0));
		if (Timer < 60)
		{
			Projectile.velocity *= 0.96f;
			float timeValue = Timer / 60f;
			Lighting.AddLight(Projectile.Center, new Vector3(1.2f, 0, 0) * (1 - timeValue));
		}
		if (Timer == 60)
		{
			Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 30f;
		}
		Projectile.rotation = Projectile.velocity.ToRotation();
	}

	public override void DestroyEntityEffect()
	{
		for (int x = 0; x < 10; x++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 5).RotatedByRandom(MathHelper.TwoPi);
			Vector2 newVelocity = offsetPos / 2f;
			var spark = new GreenLanternRedStar
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + offsetPos,
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(30, 60),
				Scale = Main.rand.NextFloat(0.5f, 1f),
			};
			Ins.VFXManager.Add(spark);
		}
		for (int g = 0; g < 8; g++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 5).RotatedByRandom(MathHelper.TwoPi);
			Vector2 newVelocity = offsetPos / 2.4f;
			offsetPos *= 6;
			var sparkFlame = new GreenLanternFragment
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + offsetPos,
				RotateSpeed = Main.rand.NextFloat(-0.3f, 0.3f),
				Rotate2Speed = Main.rand.NextFloat(-0.5f, 0.5f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (g % 2 - 0.5f) * 0.2f,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Rotation2 = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(35, 60),
				Scale = Main.rand.NextFloat(0.2f, 0.5f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkFlame);
		}
	}

	public override void DrawSelf()
	{
		float fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade *= Projectile.timeLeft / 60f;
		}
		float fade2 = 1f;
		if (Timer < 60f)
		{
			fade2 = Timer / 60f;
		}
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Color color = TrailColor;
		if (!SelfLuminous)
		{
			color = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		}
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texMain.Size() / 2f, fade, SpriteEffects.None, 0);
		var texGlow = ModAsset.GreenFlameSharpCrystal_reflect.Value;
		float rotValue = ((Projectile.rotation + MathHelper.TwoPi) % MathHelper.TwoPi) / MathHelper.TwoPi / 4f;
		int glowY = (int)rotValue % 4;
		Rectangle glowFrame = new Rectangle(0, 14 * glowY, 26, 14);
		Color glowColor = new Color(1f, 1f, 1f, 0f) * MathF.Pow(MathF.Sin(rotValue * MathF.PI), 6) * 6;
		Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, glowFrame, glowColor, Projectile.rotation, glowFrame.Size() / 2f, fade, SpriteEffects.None, 0);

		var posCheck = Projectile.Center;
		var dir = Projectile.velocity.NormalizeSafe();
		float widthRay = 2;
		Color drawC = Color.Lerp(new Color(0.2f, 0.7f, 0.8f, 0f), new Color(0f, 0.2f, 0.1f, 0f), fade2) * (1.2f - fade2);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < 600; i++)
		{
			var drawPos = posCheck - Main.screenPosition;
			bars.Add(drawPos + dir.RotatedBy(MathHelper.PiOver2) * widthRay, drawC, new Vector3(i / 4f, 0, 0));
			bars.Add(drawPos - dir.RotatedBy(MathHelper.PiOver2) * widthRay, drawC, new Vector3(i / 4f, 1, 0));
			if (!Collision.SolidCollision(posCheck - new Vector2(4), 8, 8))
			{
				posCheck += dir * 8;
			}
			else
			{
				break;
			}
		}
		if (bars.Count > 2)
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue) => base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style is 0 or 1)
		{
			float fade = 1f;
			if (Projectile.timeLeft < 60f)
			{
				fade *= Projectile.timeLeft / 60f;
			}
			if (style == 1)
			{
				Color c0 = base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
				Color c1 = new Color(0.1f, 0, 0.6f, 0);
				Color lightC = Lighting.GetColor(worldPos.ToTileCoordinates());
				c1.R = (byte)(lightC.R * c1.R / 255f);
				c1.G = (byte)(lightC.G * c1.G / 255f);
				c1.B = (byte)(lightC.B * c1.B / 255f);
				Color c2 = Color.Lerp(c0, c1, factor) * fade;
				if (Timer < 60)
				{
					Color c3 = new Color(0.2f, 1f, 1f, 0);
					float timeValue = Timer / 60f;
					Color c4 = Color.Lerp(c2, c3, 1 - timeValue);
					return c4;
				}
				return c2;
			}
			return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1) * fade;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}
}
