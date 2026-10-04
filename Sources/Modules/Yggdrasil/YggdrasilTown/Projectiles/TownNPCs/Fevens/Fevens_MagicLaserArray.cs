using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_MagicLaserArray : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 1200;
		Projectile.tileCollide = false;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 180000;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	public int MaxTime = 300;
	public float Radius = 0;
	public float Omega = 0;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.timeLeft = MaxTime;
		Radius = 0;
		Projectile.rotation = Projectile.ai[1];
		Omega = Projectile.ai[0];
	}

	public override void AI()
	{
		if (Projectile.timeLeft > MaxTime - 60)
		{
			Radius = 120 * 0.1f + Radius * 0.9f;
			Projectile.rotation += Omega;
			Omega *= 0.95f;
		}
		else
		{
			Radius = 120;
		}
		var arrow0 = new Vector2(-1, 1).RotatedBy(Projectile.rotation) * Radius + Projectile.Center;
		var arrow1 = new Vector2(1, 1).RotatedBy(Projectile.rotation) * Radius + Projectile.Center;
		if (Projectile.timeLeft < MaxTime - 90 && Projectile.timeLeft > 60)
		{
			Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
			if (Projectile.timeLeft % 30 < 20 && Projectile.timeLeft % 3 == 1)
			{
				Vector2 toPlayer0 = Vector2.Normalize(player.Center - arrow0) * 12;
				var arrowP0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), arrow0, toPlayer0, ModContent.ProjectileType<Fevens_Arrow>(), 30, 20, default);

				Vector2 toPlayer1 = Vector2.Normalize(player.Center - arrow1) * 12;
				var arrowP1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), arrow1, toPlayer1, ModContent.ProjectileType<Fevens_Arrow>(), 30, 20, default);
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float value = 1f;
		if (Projectile.timeLeft > MaxTime - 90)
		{
			value *= (MaxTime - Projectile.timeLeft) / 90f;
		}
		if (Projectile.timeLeft < 60)
		{
			value *= Projectile.timeLeft / 60f;
		}

		// Shader of magic array
		Effect dissolve = Commons.ModAsset.DissolveWithLight.Value;
		float dissolveDuration = Projectile.timeLeft / 60f - 0.2f;
		if (Projectile.timeLeft > 60)
		{
			dissolveDuration = 1.2f;
		}
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Sand.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uLightColor"].SetValue(new Vector4(0.4f, 0f, 0.7f, 1f) * value);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.2f, 0f, 0.9f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(3f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();
		float drawSize = 1f;
		if (Projectile.timeLeft > MaxTime - 90)
		{
			drawSize = MathF.Pow(value, 0.4f);
		}
		Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
		var toTarget = Vector2.Normalize(player.Center - Projectile.Center);
		Texture2D magicArray = ModAsset.MagicArrayEye.Value;
		Main.spriteBatch.Draw(magicArray, Projectile.Center, null, new Color(0.4f, 0f, 0.7f, 1f) * value, toTarget.ToRotation() - MathHelper.PiOver2, magicArray.Size() * 0.5f, drawSize, SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Texture2D magicArrayGlow = ModAsset.MagicArrayEyeGlow.Value;
		Main.spriteBatch.Draw(magicArrayGlow, Projectile.Center - Main.screenPosition, null, new Color(0.4f, 0f, 0.7f, 0f) * value, toTarget.ToRotation() - MathHelper.PiOver2, magicArrayGlow.Size() * 0.5f, drawSize, SpriteEffects.None, 0);

		DrawLaserArray(new Vector2(-1, -1).RotatedBy(Projectile.rotation) * Radius);
		DrawLaserArray(new Vector2(1, -1).RotatedBy(Projectile.rotation) * Radius);

		DrawArrowArray(new Vector2(-1, 1).RotatedBy(Projectile.rotation) * Radius);
		DrawArrowArray(new Vector2(1, 1).RotatedBy(Projectile.rotation) * Radius);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawLaserArray(Vector2 relativePos)
	{
		float value = 1f;
		if (Projectile.timeLeft > MaxTime - 90)
		{
			value *= (MaxTime - Projectile.timeLeft) / 90f;
		}
		if (Projectile.timeLeft < 60)
		{
			value *= Projectile.timeLeft / 60f;
		}

		Vector2 drawPos = Projectile.Center - Main.screenPosition + relativePos;
		var drawColor = new Color(1f, 0f, 0f, 0f);
		var darkDraw = Color.White * value;
		var timeValue = (float)Main.time * 0.07f;

		Texture2D swirlPointDark = Commons.ModAsset.Point_black.Value;
		Main.spriteBatch.Draw(swirlPointDark, drawPos, null, Color.White * value, timeValue, swirlPointDark.Size() * 0.5f, value * 0.5f, SpriteEffects.None, 0);
		var circleDark = new List<Vertex2D>();
		var circle = new List<Vertex2D>();
		for (int c = 0; c <= 30; c++)
		{
			Vector2 radius = new Vector2(0, 60).RotatedBy(c / 30f * MathHelper.TwoPi) * (MathF.Sin(timeValue) + 5) / 6f;
			float width = (float)Utils.Lerp(1f, 0.2f, value);
			circleDark.Add(drawPos + radius * 0.8f, darkDraw, new Vector3(c / 30f + timeValue * 1.3f, 0, 0));
			circleDark.Add(drawPos + radius * width * 0.8f, darkDraw, new Vector3(c / 30f + timeValue * 1.3f, 1, 0));
			circle.Add(drawPos + radius, drawColor, new Vector3(c / 30f + timeValue, 0, 0));
			circle.Add(drawPos + radius * width, drawColor, new Vector3(c / 30f + timeValue, 1, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0_black.Value;
		if (circleDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circleDark.ToArray(), 0, circleDark.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
		if (circle.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}

		Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
		var toTarget = Vector2.Normalize(player.Center - Projectile.Center);
		var normal = toTarget.RotatedBy(MathHelper.PiOver2) * value * 16;
		var checkPos = Projectile.Center + relativePos;
		var laserColor = drawColor;
		if (Projectile.timeLeft > MaxTime - 60)
		{
			laserColor *= 0.2f;
		}
		if (Projectile.timeLeft > MaxTime - 90 && Projectile.timeLeft < MaxTime - 60)
		{
			var timeValue2 = (MaxTime - 60 - Projectile.timeLeft) / 30f;
			laserColor = Color.Lerp(new Color(1f, 1f, 1f, 0), new Color(1f, 0, 0, 0), timeValue2);
			normal *= 1 + (Projectile.timeLeft - MaxTime + 90) / 30f * 2;

			float dark = Math.Max((Projectile.timeLeft - MaxTime + 90) / 30f, 0);
			Texture2D light = Commons.ModAsset.StarSlash.Value;
			Main.spriteBatch.Draw(light, drawPos, new Rectangle(0, 0, light.Width / 2, light.Height), laserColor, MathF.Sin(Projectile.whoAmI - Projectile.position.X) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * 1.62f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, drawPos, new Rectangle(0, 0, light.Width / 2, light.Height), laserColor, MathF.Sin(Projectile.whoAmI) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * 1.62f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, drawPos, null, laserColor, MathF.Sin(Projectile.whoAmI + Projectile.position.Y) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * 1.62f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, drawPos, new Rectangle(0, 0, light.Width / 2, light.Height), laserColor, MathF.Sin(Projectile.type * 0.4f + Projectile.whoAmI) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * 1.62f, SpriteEffects.None, 0);
		}
		else
		{
			laserColor *= (MathF.Sin(timeValue * 8) + 4) / 5f;
		}
		var laserDark = new List<Vertex2D>();
		var laser = new List<Vertex2D>();
		for (int c = 0; c <= 1000; c++)
		{
			bool fade = false;
			if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16))
			{
				fade = true;

				Vector2 checkPos1 = checkPos - toTarget * 45f;
				Vector2 checkPos2 = checkPos + toTarget * 5f;
				Vector2 checkPos3 = checkPos + toTarget * 30f;
				float headWidth = 2.4f;
				var laserHead = new List<Vertex2D>
				{
					new Vertex2D(checkPos1 + normal * headWidth, new Color(0, 0, 0, 0), new Vector3(1 + timeValue, 0, 0)),
					new Vertex2D(checkPos1 - normal * headWidth, new Color(0, 0, 0, 0), new Vector3(1 + timeValue, 1, 0)),
					new Vertex2D(checkPos2 + normal * headWidth, laserColor * 0.75f, new Vector3(0.5f + timeValue, 0, 0.5f)),
					new Vertex2D(checkPos2 - normal * headWidth, laserColor * 0.75f, new Vector3(0.5f + timeValue, 1, 0.5f)),
					new Vertex2D(checkPos3 + normal * headWidth, laserColor * 1.2f, new Vector3(0f + timeValue, 0, 1)),
					new Vertex2D(checkPos3 - normal * headWidth, laserColor * 1.2f, new Vector3(0f + timeValue, 1, 1)),
				};
				var laserHeadDark = new List<Vertex2D>
				{
					new Vertex2D(checkPos1 + normal * headWidth, new Color(0, 0, 0, 0), new Vector3(1 + timeValue, 0, 0)),
					new Vertex2D(checkPos1 - normal * headWidth, new Color(0, 0, 0, 0), new Vector3(1 + timeValue, 1, 0)),
					new Vertex2D(checkPos2 + normal * headWidth, darkDraw * 0.75f, new Vector3(0.5f + timeValue, 0, 0.5f)),
					new Vertex2D(checkPos2 - normal * headWidth, darkDraw * 0.75f, new Vector3(0.5f + timeValue, 1, 0.5f)),
					new Vertex2D(checkPos3 + normal * headWidth, darkDraw * 1.2f, new Vector3(0f + timeValue, 0, 1)),
					new Vertex2D(checkPos3 - normal * headWidth, darkDraw * 1.2f, new Vector3(0f + timeValue, 1, 1)),
				};
				if (laserHead.Count >= 3)
				{
					SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
					Main.spriteBatch.End();
					Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
					Effect effect = Commons.ModAsset.StabSwordEffect.Value;
					var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
					var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
					effect.Parameters["uTransform"].SetValue(model * projection);
					effect.Parameters["uProcession"].SetValue(0.5f);
					effect.CurrentTechnique.Passes[0].Apply();
					Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_1_black.Value;
					Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, laserHeadDark.ToArray(), 0, laserHeadDark.Count - 2);

					Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
					Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, laserHead.ToArray(), 0, laserHead.Count - 2);
					Main.spriteBatch.End();
					Main.spriteBatch.Begin(sBS);
				}
				if (!Main.gamePaused)
				{
					for (int t = 0; t < 3; t++)
					{
						Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(4f, 6f)).RotatedByRandom(MathHelper.TwoPi) * value;
						var smog = new Fevens_LaserSpark
						{
							Velocity = newVelocity,
							Active = true,
							Visible = true,

							Position = checkPos + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
							MaxTime = Main.rand.Next(27, 35),
							Scale = Main.rand.NextFloat(1f, 2f) * value,
							Rotation = newVelocity.ToRotation(),

							ai = new float[] { Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(-0.005f, 0.005f) },
						};
						Ins.VFXManager.Add(smog);
					}
				}
			}
			var color2 = laserColor;
			var color2Dark = darkDraw;
			if (fade)
			{
				color2 *= 0;
				color2Dark *= 0;
			}
			laser.Add(checkPos + normal - Main.screenPosition, color2, new Vector3(c / 30f - timeValue, 0, 0));
			laser.Add(checkPos - normal - Main.screenPosition, color2, new Vector3(c / 30f - timeValue, 1, 0));

			laserDark.Add(checkPos + normal - Main.screenPosition, color2Dark, new Vector3(c / 30f - timeValue, 0, 0));
			laserDark.Add(checkPos - normal - Main.screenPosition, color2Dark, new Vector3(c / 30f - timeValue, 1, 0));
			checkPos += toTarget * 30f;
			if (fade)
			{
				break;
			}
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0_black.Value;
		if (laserDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, laserDark.ToArray(), 0, laserDark.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
		if (laser.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, laser.ToArray(), 0, laser.Count - 2);
		}

		Texture2D swirlPoint = Commons.ModAsset.SwirlPoint.Value;
		Main.spriteBatch.Draw(swirlPoint, drawPos, null, drawColor, timeValue, swirlPoint.Size() * 0.5f, value * 0.5f, SpriteEffects.None, 0);
	}

	public void DrawArrowArray(Vector2 relativePos)
	{
		float value = 1f;
		if (Projectile.timeLeft > MaxTime - 90)
		{
			value *= (MaxTime - Projectile.timeLeft) / 90f;
		}
		if (Projectile.timeLeft < 60)
		{
			value *= Projectile.timeLeft / 60f;
		}
		Vector2 drawPos = Projectile.Center - Main.screenPosition + relativePos;
		var drawColor = new Color(0f, 0.4f, 0.8f, 0f);
		var timeValue = (float)Main.time * 0.07f;
		var darkDraw = Color.White * value;
		var circleDark = new List<Vertex2D>();
		var circle = new List<Vertex2D>();
		for (int c = 0; c <= 30; c++)
		{
			Vector2 radius = new Vector2(0, 60).RotatedBy(c / 30f * MathHelper.TwoPi) * (MathF.Sin(timeValue) + 5) / 6f;
			float width = (float)Utils.Lerp(1f, 0.2f, value);
			circleDark.Add(drawPos + radius * 0.8f, darkDraw, new Vector3(c / 30f + timeValue * 1.3f, 0, 0));
			circleDark.Add(drawPos + radius * width * 0.8f, darkDraw, new Vector3(c / 30f + timeValue * 1.3f, 1, 0));
			circle.Add(drawPos + radius, drawColor, new Vector3(c / 30f + timeValue, 0, 0));
			circle.Add(drawPos + radius * width, drawColor, new Vector3(c / 30f + timeValue, 1, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0_black.Value;
		if (circleDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circleDark.ToArray(), 0, circleDark.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
		if (circle.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}

		Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
		var toTarget = Vector2.Normalize(player.Center - Projectile.Center - relativePos);
		var normal = toTarget.RotatedBy(MathHelper.PiOver2) * value * 16;
		var checkPos = Projectile.Center + relativePos;
		var laser = new List<Vertex2D>();
		for (int c = 0; c <= 12; c++)
		{
			float fade = 1 - c / 12f;
			laser.Add(checkPos + normal * fade - Main.screenPosition, drawColor, new Vector3(c / 30f - timeValue, 0, 0));
			laser.Add(checkPos - normal * fade - Main.screenPosition, drawColor, new Vector3(c / 30f - timeValue, 1, 0));
			checkPos += toTarget * 10f;
			if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16))
			{
				break;
			}
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
		if (laser.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, laser.ToArray(), 0, laser.Count - 2);
		}
		Texture2D swirlPoint = Commons.ModAsset.SwirlPoint.Value;
		Main.spriteBatch.Draw(swirlPoint, drawPos, null, drawColor, timeValue, swirlPoint.Size() * 0.5f, value * 0.5f, SpriteEffects.None, 0);
	}

	public bool CheckLaser(Vector2 relativePos)
	{
		if (Projectile.timeLeft > MaxTime - 75)
		{
			return false;
		}
		if (Projectile.timeLeft < 50)
		{
			return false;
		}
		Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
		var toTarget = Vector2.Normalize(player.Center - Projectile.Center);
		var checkPos = Projectile.Center + relativePos;
		for (int c = 0; c <= 1000; c++)
		{
			checkPos += toTarget * 30f;
			if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16))
			{
				break;
			}
		}
		if (CollisionUtils.Intersect(player.Hitbox.Left(), player.Hitbox.Right(), player.Hitbox.Height, Projectile.Center + relativePos, checkPos, 2))
		{
			return true;
		}
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		var laser0 = new Vector2(-1, -1).RotatedBy(Projectile.rotation) * Radius;
		var laser1 = new Vector2(1, -1).RotatedBy(Projectile.rotation) * Radius;
		var arrow0 = new Vector2(-1, 1).RotatedBy(Projectile.rotation) * Radius;
		var arrow1 = new Vector2(1, 1).RotatedBy(Projectile.rotation) * Radius;
		if (CheckLaser(laser0) || CheckLaser(laser1))
		{
			return true;
		}
		if (targetHitbox.Contains(arrow0.ToPoint()) || targetHitbox.Contains(arrow1.ToPoint()) || targetHitbox.Contains(laser0.ToPoint()) || targetHitbox.Contains(laser1.ToPoint()))
		{
			return true;
		}
		return false;
	}
}
