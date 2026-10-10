using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_LightingBolt : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 30;
		Projectile.tileCollide = false;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public List<Vector2> FlowPosList = new List<Vector2>();

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
		FailureVFX(8);
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 40, 20f, 120, 0.9f, 0.8f, 150);
		SoundEngine.PlaySound(SoundID.DD2_GoblinBomb, Projectile.Center);
	}

	public override void AI() => base.AI();

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		float timeValue = (float)Main.time * 0.03f;
		float mulColor = 1f;
		float width = 120f;

		if (Projectile.timeLeft > 30)
		{
			width *= Projectile.timeLeft / 120f;
		}
		else
		{
			width *= 0.5f + MathF.Sin(Projectile.timeLeft * 0.3f) * 0.2f;
			width *= 0.15f;
		}
		Vector2 hitCenter = Projectile.Center + new Vector2(0, -1000);
		var flowPosList = new List<Vector2>();
		Vector2 flowPos = Projectile.Center;
		Vector2 flowVelocity = Vector2.Normalize(hitCenter - Projectile.Center).RotatedBy(MathF.Sin(timeValue + Projectile.whoAmI) * 0.5f) * 4;
		if (Projectile.hostile)
		{
			if (Projectile.timeLeft <= 30)
			{
				flowVelocity = flowVelocity.RotatedBy(MathF.Sin(timeValue * 0.7f + Projectile.whoAmI) * 0.2f);
			}
		}
		for (int i = 0; i < 600; i++)
		{
			flowPosList.Add(flowPos);
			flowPos += flowVelocity;
			Vector2 toTarget = hitCenter - flowPos - flowVelocity;
			if (toTarget.Length() >= 20)
			{
				flowVelocity = Vector2.Normalize(toTarget) * 0.05f * 6f + flowVelocity * 0.95f;
				if (Projectile.hostile)
				{
					if (Projectile.timeLeft <= 30 && i % 7 == 3)
					{
						flowVelocity = flowVelocity.RotatedBy(MathF.Sin(timeValue * 2.2f + Projectile.whoAmI + i) * 0.4f);
						flowVelocity = flowVelocity.RotatedBy(MathF.Sin(timeValue * 1.07f + Projectile.whoAmI + i * 0.5f) * 0.3f);
						flowVelocity = flowVelocity.RotatedBy(MathF.Sin(timeValue * 0.48f + Projectile.whoAmI + i * 0.15f) * 0.2f);
						if (Projectile.timeLeft > 25)
						{
							Lighting.AddLight(flowPos, new Vector3(0.7f, 1f, 1f) * width / 4f);
						}
						else
						{
							Lighting.AddLight(flowPos, new Vector3(1f, 0f, 0.1f) * width / 12f);
						}
					}
					if (!Main.gamePaused)
					{
						if (Projectile.timeLeft == 1 && i % 8 == 0)
						{
							var dustVFX = new Fevens_LightingBoltDust
							{
								Velocity = flowVelocity * 0.5f,
								Active = true,
								Visible = true,
								Position = flowPos,
								MaxTime = Main.rand.Next(30, 50),
								Scale = Main.rand.Next(1, 2),
								ai = new float[] { Main.rand.NextFloat(1f, 8f), 0 },
							};
							Ins.VFXManager.Add(dustVFX);
						}
					}
				}
			}
			else
			{
				break;
			}
		}
		FlowPosList = flowPosList;
		Color drawColor = new Color(0.15f, 0f, 0.01f, 0f) * mulColor;
		Color drawColor2 = new Color(1f, 0f, 0.2f, 0f) * mulColor;
		drawColor = Color.Lerp(drawColor, drawColor2, 1 - MathF.Pow(Projectile.timeLeft / 120f, 1f / 3));
		Color drawColor_dark = new Color(1f, 1f, 1f, 1f) * mulColor;

		var powerFlow = new List<Vertex2D>();
		var powerFlow_dark = new List<Vertex2D>();
		if (flowPosList.Count > 2)
		{
			for (int i = 0; i < flowPosList.Count - 1; i++)
			{
				float mulColor2 = 1f;
				if (i < 3)
				{
					mulColor2 = i / 3f;
				}
				if (i >= flowPosList.Count - 4)
				{
					mulColor2 = (flowPosList.Count - 2 - i) / 3f;
				}
				if (Projectile.timeLeft > 25)
				{
					drawColor = new Color(1f, 1f, 1f, 0);
					mulColor2 = 3f;
				}
				Vector2 normal = (flowPosList[i] - flowPosList[i + 1]).SafeNormalize(Vector2.One);
				float coeWidth = 1;
				if (i > flowPosList.Count - 40)
				{
					coeWidth = (flowPosList.Count - i) / 40f;
				}
				normal = normal.RotatedBy(MathHelper.PiOver2) * width * coeWidth;
				powerFlow.Add(flowPosList[i] + normal - Main.screenPosition, drawColor * mulColor2, new Vector3(i * 0.05f - timeValue, 0, 0));
				powerFlow.Add(flowPosList[i] - normal - Main.screenPosition, drawColor * mulColor2, new Vector3(i * 0.05f - timeValue, 1, 0));
				powerFlow_dark.Add(flowPosList[i] + normal - Main.screenPosition, drawColor_dark * mulColor2, new Vector3(i * 0.05f - timeValue, 0, 0));
				powerFlow_dark.Add(flowPosList[i] - normal - Main.screenPosition, drawColor_dark * mulColor2, new Vector3(i * 0.05f - timeValue, 1, 0));
			}
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float lightValue = 0.5f + MathF.Sin(Projectile.timeLeft * 0.4f);
		lightValue *= 2;
		if (Projectile.timeLeft > 27)
		{
			lightValue = 5;
		}
		if (powerFlow.Count > 3)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerFlow_dark.ToArray(), 0, powerFlow_dark.Count - 2);

			for (int i = 0; i < lightValue; i++)
			{
				Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerFlow.ToArray(), 0, powerFlow.Count - 2);
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		Texture2D point = Commons.ModAsset.Point.Value;
		Main.spriteBatch.Draw(point, Projectile.Center - Main.screenPosition, null, new Color(Projectile.timeLeft / 30f, 0, 0, 0), 0, point.Size() * 0.5f, 2f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(point, Projectile.Center - Main.screenPosition, null, new Color(Projectile.timeLeft / 30f, Projectile.timeLeft / 120f, Projectile.timeLeft / 90f, 0), 0, point.Size() * 0.5f, 0.5f, SpriteEffects.None, 0);

		float dark = Projectile.timeLeft / 30f;
		Color drawStarColor = drawColor;
		if (Projectile.timeLeft > 25)
		{
			drawStarColor = new Color(1f, 1f, 1f, 0);
		}

		Color drawStarColor_Black = Color.White;
		if (Projectile.timeLeft >= 15 && Projectile.timeLeft % 10 < 3)
		{
			drawStarColor_Black *= 0;
		}
		if (Projectile.timeLeft < 15)
		{
			drawStarColor_Black *= Projectile.timeLeft / 15f;
		}
		Texture2D light_Black = Commons.ModAsset.StarSlash_black.Value;
		Main.spriteBatch.Draw(light_Black, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light_Black.Width / 2, light_Black.Height), drawStarColor_Black, MathF.Sin(Projectile.whoAmI - Projectile.position.X) * 6, light_Black.Size() / 2f, new Vector2(1f, dark * dark) * 4.05f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light_Black, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light_Black.Width / 2, light_Black.Height), drawStarColor_Black, MathF.Sin(Projectile.whoAmI) * 6, light_Black.Size() / 2f, new Vector2(0.5f, dark) * 4.05f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light_Black, Projectile.Center - Main.screenPosition, null, drawStarColor_Black, MathF.Sin(Projectile.whoAmI + Projectile.position.Y) * 6, light_Black.Size() / 2f, new Vector2(1f, dark * dark) * 4.05f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light_Black, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light_Black.Width / 2, light_Black.Height), drawStarColor_Black, MathF.Sin(Projectile.type * 0.4f + Projectile.whoAmI) * 6, light_Black.Size() / 2f, new Vector2(1f, dark * dark) * 4.05f, SpriteEffects.None, 0);

		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), drawStarColor, MathF.Sin(Projectile.whoAmI - Projectile.position.X) * 6, light.Size() / 2f, new Vector2(1f, dark * dark) * 4.05f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), drawStarColor, MathF.Sin(Projectile.whoAmI) * 6, light.Size() / 2f, new Vector2(0.5f, dark) * 4.05f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, drawStarColor, MathF.Sin(Projectile.whoAmI + Projectile.position.Y) * 6, light.Size() / 2f, new Vector2(1f, dark * dark) * 4.05f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), drawStarColor, MathF.Sin(Projectile.type * 0.4f + Projectile.whoAmI) * 6, light.Size() / 2f, new Vector2(1f, dark * dark) * 4.05f, SpriteEffects.None, 0);

		if (Projectile.timeLeft > 25)
		{
			Lighting.AddLight(Projectile.Center, new Vector3(0.7f, 1f, 1f) * 15);
		}
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (var vec in FlowPosList)
		{
			if (targetHitbox.Contains(vec.ToPoint()))
			{
				return true;
			}
		}
		return false;
	}

	public void FailureVFX(int level)
	{
		for (int i = 0; i < level * 4 + 12; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(5.6f, 18.4f)).RotatedByRandom(MathHelper.TwoPi);
			var dust = new Fevens_LightingBoltDust
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(60, 120),
				Scale = Main.rand.NextFloat(1f, 2f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
		for (int i = 0; i < level * 2; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(5.6f, 13.4f)).RotatedByRandom(MathHelper.TwoPi);
			var cube = new AvariceFailureCube
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(60, 120),
				Scale = Main.rand.NextFloat(10f, 50f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), 0.06f },
			};
			Ins.VFXManager.Add(cube);
		}
		float waveRot = Main.rand.NextFloat(6.283f);
		for (int i = 0; i < 2; i++)
		{
			var wave = new FevensLightingBoltWave
			{
				Velocity = Vector2.zeroVector,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(30, 40),
				Scale = 1 + i * 0.6f,
				Rotation = waveRot + i * 2f,
				ai = new float[] { 0.04f * MathF.Sqrt(level) },
			};
			Ins.VFXManager.Add(wave);
		}
	}
}
