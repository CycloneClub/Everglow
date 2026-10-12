using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.DataStructures;
using static Everglow.SpellAndSkull.Common.SpellAndSkullUtils;

namespace Everglow.SpellAndSkull.Projectiles.MagnetSphere;

public class MagnetSphereLighting : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override bool CloneNewInstances => false;

	public override bool IsCloneable => false;

	public override void SetDefaults()
	{
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 4;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 6;
		Projectile.DamageType = DamageClass.Magic;
	}

	public void GenerateVFXExpolode(int Frequency, float mulVelocity = 1f)
	{
		for (int g = 0; g < Frequency * 3; g++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(4.65f, 5.5f)).RotatedByRandom(6.283) * mulVelocity;
			var me = new MagneticElectricity
			{
				velocity = vel,
				Active = true,
				Visible = true,
				maxTime = Main.rand.Next(24, 72),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.01f, 0.01f), Main.rand.NextFloat(1.6f, 2f) * mulVelocity },
				position = Projectile.Center - vel * 3,
			};
			Ins.VFXManager.Add(me);
		}
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(6.65f, 10.5f)).RotatedByRandom(6.283) * mulVelocity;
			var me = new MagneticElectricity
			{
				velocity = vel,
				Active = true,
				Visible = true,
				maxTime = Main.rand.Next(24, 72),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.01f, 0.01f), Main.rand.NextFloat(1.6f, 2f) * mulVelocity },
				position = Projectile.Center - vel * 3,
			};
			Ins.VFXManager.Add(me);
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		GenerateVFXExpolode(2, 0.6f);
	}

	public override void AI()
	{
		Projectile.velocity *= 0.95f;

		if (Projectile.timeLeft <= 198)
		{
			Projectile.friendly = false;
		}

		float LightS = Projectile.timeLeft / 2f - 95f;
		if (LightS > 0)
		{
			Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 0, LightS * 0.83f, LightS * 0.8f);
		}

		Projectile.velocity *= 0;
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHitLight.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, new Color(0, 199, 129, 0) * dark, 0, Shadow.Size() / 2f, 22 / 15f * dark, SpriteEffects.None, 0);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawLightingBolt(Color.White * 0.3f);
		DrawLightingBolt(new Color(0, 255, 199, 0));

		Texture2D Shadow = ModAsset.CursedHit.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, Shadow.Size() / 2f, 22f / 15f, SpriteEffects.None, 0);
		Texture2D light = ModAsset.CursedHitStar.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 199, 129, 0), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) / 2f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 199, 129, 0), 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.8f, dark / 2f), SpriteEffects.None, 0);

		float value = (480 - Projectile.timeLeft * 2.4f) / Projectile.timeLeft * 1.4f;
		if (value < 0)
		{
			value = 0;
		}

		float colorV = 0.9f * (1 - value);

		Texture2D t = Commons.ModAsset.Wave.Value;
		DrawTexCircle(value * 160, 10 * value * value, new Color(0, colorV, colorV * 0.7f, 0f), Projectile.Center - Main.screenPosition, t);

		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 199, 129, 0), (float)(Math.PI / 4d) + Projectile.ai[1], light.Size() / 2f, new Vector2(0.6f, dark / 2f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 199, 129, 0), (float)(Math.PI / 4d * 3) + Projectile.ai[1], light.Size() / 2f, new Vector2(0.6f, dark / 0f), SpriteEffects.None, 0);
		return false;
	}

	internal Vector2[] LightPos = new Vector2[30];

	private void DrawLightingBolt(Color c0)
	{
		var BasePos = new Vector2[30];
		float width = (Projectile.timeLeft - 170) / 1.8f;
		if (c0 == Color.White * 0.3f)
		{
			width *= 0.2f;
		}

		int LengthII = 0;
		if (width < 0)
		{
			return;
		}

		if (!Main.projectile[(int)Projectile.ai[0]].active)
		{
			return;
		}

		Vector2 AimC = Main.projectile[(int)Projectile.ai[0]].Center;
		if ((Projectile.Center - AimC).Length() > 900)
		{
			return;
		}

		if (LightPos[1] == Vector2.Zero)
		{
			BasePos[0] = Projectile.Center;
			float Length = AimC.Length() / 40f;
			if (Length > 30)
			{
				Length = 30;
			}

			Vector2 VLight = new Vector2(0, Main.rand.NextFloat(3f, 5f)).RotatedByRandom(6.283);
			for (int a = 1; a < Length - 1; a++)
			{
				if (a > 1)
				{
					LightPos[a] = LightPos[a - 1] + new Vector2(0, Main.rand.NextFloat(0f, 7f)).RotatedByRandom(6.283) + VLight;
					VLight = VLight.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
					if (a > Length - 10)
					{
						LightPos[a] -= LightPos[a - 1] * 0.2f;
						VLight -= Vector2.Normalize(LightPos[a - 1]) * 0.3f;
					}
				}
				else
				{
					LightPos[a] = new Vector2(0, Main.rand.NextFloat(0f, 5f)).RotatedByRandom(6.283);
				}
				if (a + 1 >= Length)
				{
					LightPos[a + 1] = Vector2.Zero;
				}
			}
		}
		for (int a = 1; a < 30; a++)
		{
			if (LightPos[a] == Vector2.Zero)
			{
				LengthII = a;
				break;
			}
		}
		for (int a = 0; a < LengthII; a++)
		{
			BasePos[a] = a / (float)LengthII * Projectile.Center + (LengthII - a) / (float)LengthII * AimC + LightPos[a];
		}

		for (int a = 0; a < LengthII; a++)
		{
			if (BasePos[a] != Vector2.Zero)
			{
				if (a % 4 == 0)
				{
					Lighting.AddLight((int)(BasePos[a].X / 16), (int)(BasePos[a].Y / 16), 0, width / 45f, width / 50f);
				}
			}
			else
			{
				break;
			}
		}
		var lighting = new List<Vertex2D>();
		for (int a = 0; a < LengthII; a++)
		{
			if (BasePos[a] != Vector2.Zero)
			{
				Vector2 NormalizedToTarget = (AimC - Projectile.Center).SafeNormalize(Vector2.One).RotatedBy(1.57) * width;
				if (a >= 1)
				{
					NormalizedToTarget = (BasePos[a] - BasePos[a - 1]).SafeNormalize(Vector2.One).RotatedBy(-1.57) * width;
				}

				lighting.Add(new Vertex2D(BasePos[a] - NormalizedToTarget - Main.screenPosition, c0, new Vector3(0, 0, 0)));
				lighting.Add(new Vertex2D(BasePos[a] + NormalizedToTarget - Main.screenPosition, c0, new Vector3(0, 1, 0)));
			}
		}
		if (lighting.Count > 0)
		{
			if (c0 == Color.White * 0.3f)
			{
				Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Darkline.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, lighting.ToArray(), 0, lighting.Count - 2);
				return;
			}
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.MagneticLight.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, lighting.ToArray(), 0, lighting.Count - 2);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / (float)Projectile.timeLeft * 1.4f;
		float colorV = 0.9f * (1 - value);
		Texture2D t = Commons.ModAsset.Wave.Value;
		DrawTexCircle(spriteBatch, value * 160, 100, new Color(colorV, colorV * 0.02f, colorV, 0f), Projectile.Center - Main.screenPosition, t);
	}
}
