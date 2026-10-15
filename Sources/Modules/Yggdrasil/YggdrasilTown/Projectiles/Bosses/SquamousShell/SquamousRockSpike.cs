using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class SquamousRockSpike : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 3600;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	internal int Target = -1;
	internal int TimeTokill = -1;

	public override void OnSpawn(IEntitySource source)
	{
		Target = (int)Projectile.ai[0];
		if (Main.expertMode)
		{
			Projectile.extraUpdates = 1;
		}
		if (Main.masterMode)
		{
			Projectile.extraUpdates = 3;
		}
	}

	public override void AI()
	{
		if (Target == -1)
		{
			Projectile.Kill();
			return;
		}
		Player player = Main.player[Target];
		if (TimeTokill >= 0 && TimeTokill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeTokill <= 15 && TimeTokill > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
		}

		TimeTokill--;
		if (TimeTokill < 0)
		{
			if (Projectile.timeLeft >= 3520)
			{
				Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
				Projectile.velocity = Projectile.velocity * 0.3f + (player.Center - Projectile.Center).SafeNormalize(Vector2.zeroVector) * 0.4f;
			}
			else
			{
				if (Projectile.timeLeft < 3500)
				{
					Projectile.extraUpdates = 0;
					float maxSpeed = 10f;
					if (Main.expertMode)
					{
						maxSpeed = 12f;
					}
					if (Main.masterMode)
					{
						maxSpeed = 20f;
					}
					if (Projectile.velocity.Length() < maxSpeed)
					{
						Projectile.velocity *= maxSpeed / Projectile.velocity.Length();
					}
				}
			}
		}
		else
		{
			if (TimeTokill < 10)
			{
				Projectile.damage = 0;
				Projectile.hostile = false;
			}
			Projectile.velocity *= 0f;
		}
		if (Projectile.timeLeft > 3550)
		{
			ConcentratingDust(1);
		}
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(40f, 55f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < Frequency * 10; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(15f, 26f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmog_ConeDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(20, 42),
				Scale = Main.rand.NextFloat(0.6f, 5f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void ConcentratingDust(int Frequency)
	{
		for (int g = 0; g < Frequency * 3; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 2f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new Rock_Concentrating_dust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(0, 160f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(120, 142),
				Scale = Main.rand.NextFloat(0.6f, 5f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(3f, 15f), Projectile.whoAmI, Projectile.type, Main.rand.NextFloat(10f) },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		AmmoHit();
		Projectile.timeLeft = 10;
		return false;
	}

	public void AmmoHit()
	{
		TimeTokill = 240;
		Projectile.velocity = Projectile.oldVelocity;
		for (int x = 0; x < 8; x++)
		{
			Dust.NewDust(Projectile.Center - Projectile.velocity * 2 - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<SquamousShellStone>(), 0f, 0f, 0, default, 0.7f);
		}
		for (int x = 0; x < 8; x++)
		{
			Dust.NewDust(Projectile.Center - Projectile.velocity * 2 - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<SquamousShellStone_dark>(), 0f, 0f, 0, default, 0.7f);
		}
		for (int x = 0; x < 8; x++)
		{
			var d0 = Dust.NewDustDirect(Projectile.Bottom - new Vector2(4, -4), 0, 0, ModContent.DustType<SquamousShellWingDust>());
			d0.velocity = new Vector2(0, 12).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, -8);
			d0.noGravity = false;
			d0.scale *= Main.rand.NextFloat(1.3f);
		}
		GenerateSmog(4);
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolume(0.4f), Projectile.Center);
		Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<SquamousRockExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 10);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (TimeTokill > 0)
		{
			return false;
		}
		var textureShade = ModAsset.SquamousRockSpike_Shade.Value;
		var textureBloom = ModAsset.SquamousRockSpike_Glow.Value;
		Main.spriteBatch.Draw(textureShade, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, textureShade.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(textureBloom, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, textureBloom.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		float timerProj = 3600 - Projectile.timeLeft;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		if (timerProj <= 80)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			// draw concentrated energy flow
			Effect effect = ModAsset.TeleportToYggdrasilFlowEffect.Value;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.CurrentTechnique.Passes[0].Apply();
			Vector2 drawCenter = Projectile.Center;

			// inner timer.
			float timeValue = (float)Main.time * 0.04f;
			float decrease = 1 - timerProj / 80f;
			float addTimeValue = MathF.Pow(0.75f - decrease, 3) * 10f;
			decrease = MathF.Pow(decrease, 3);
			decrease = MathF.Sin(decrease * MathHelper.Pi) * 14f;

			for (int r = 0; r < 6; r++)
			{
				var flows = new List<Vertex2D>();
				for (int i = 0; i <= 20; i++)
				{
					Vector2 thisJoint = new Vector2(0, i * 10).RotatedBy(GetFlowEffectRotation(i, r));
					Vector2 nextJoint = new Vector2(0, (i + 1) * 10).RotatedBy(GetFlowEffectRotation(i + 1, r));
					Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
					float vertexWidth = 40 + MathF.Sin(r) * 10;
					normalWidth *= vertexWidth;
					float drawWidth = MathF.Sin(Math.Min(i / 10f, 0.5f) * MathF.PI);
					float fade = 1;
					if (i > decrease)
					{
						fade *= Math.Clamp((decrease + 6 - i) / 6f, 0, 1);
					}
					if (timerProj > 50)
					{
						fade *= Math.Clamp((70 - timerProj) / 20f, 0, 1);
					}

					var drawColor = new Color(1f, 1f, 1f, 1);
					flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.07f + timeValue + r * 0.2f + addTimeValue, 0, drawWidth));
					flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.07f + timeValue + r * 0.2f + addTimeValue, 1, drawWidth));
				}
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
			}
			for (int r = 0; r < 6; r++)
			{
				var flows = new List<Vertex2D>();
				for (int i = 0; i <= 20; i++)
				{
					Vector2 thisJoint = new Vector2(0, i * 6).RotatedBy(GetFlowEffectRotation(i * 0.5f, r) + MathHelper.Pi / 6f);
					Vector2 nextJoint = new Vector2(0, (i + 1) * 6).RotatedBy(GetFlowEffectRotation(i * 0.5f + 0.5f, r) + MathHelper.Pi / 6f);
					Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
					float vertexWidth = 34 + MathF.Sin(r) * 5;
					normalWidth *= vertexWidth;
					float drawWidth = MathF.Sin(Math.Min(i / 4f, 0.5f) * MathF.PI);
					float fade = 1;
					if (i > decrease)
					{
						fade *= Math.Clamp((decrease + 6 - i) / 6f, 0, 1);
					}
					if (timerProj > 50)
					{
						fade *= Math.Clamp((70 - timerProj) / 20f, 0, 1);
					}

					var drawColor = new Color(1f, 1f, 1f, 1);
					flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.035f + timeValue + r * 0.2f + addTimeValue, 0, drawWidth));
					flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.035f + timeValue + r * 0.2f + addTimeValue, 1, drawWidth));
				}
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_3_black.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
			}

			for (int r = 0; r < 6; r++)
			{
				var flows = new List<Vertex2D>();
				for (int i = 0; i <= 20; i++)
				{
					Vector2 thisJoint = new Vector2(0, i * 10).RotatedBy(GetFlowEffectRotation(i, r));
					Vector2 nextJoint = new Vector2(0, (i + 1) * 10).RotatedBy(GetFlowEffectRotation(i + 1, r));
					Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
					float vertexWidth = 40 + MathF.Sin(r) * 10;
					normalWidth *= vertexWidth;
					float drawWidth = MathF.Sin(Math.Min(i / 10f, 0.5f) * MathF.PI);
					float fade = 1;
					if (i > decrease)
					{
						fade *= Math.Clamp((decrease + 6 - i) / 6f, 0, 1);
					}
					if (timerProj > 50)
					{
						fade *= Math.Clamp((70 - timerProj) / 20f, 0, 1);
					}

					var drawColor = new Color(0.4f, 0.3f, 0.3f, 0);
					flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.07f + timeValue + r * 0.2f + addTimeValue, 0, drawWidth));
					flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.07f + timeValue + r * 0.2f + addTimeValue, 1, drawWidth));
				}
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
			}
			for (int r = 0; r < 6; r++)
			{
				var flows = new List<Vertex2D>();
				for (int i = 0; i <= 20; i++)
				{
					Vector2 thisJoint = new Vector2(0, i * 6).RotatedBy(GetFlowEffectRotation(i * 0.5f, r) + MathHelper.Pi / 6f);
					Vector2 nextJoint = new Vector2(0, (i + 1) * 6).RotatedBy(GetFlowEffectRotation(i * 0.5f + 0.5f, r) + MathHelper.Pi / 6f);
					Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
					float vertexWidth = 34 + MathF.Sin(r) * 5;
					normalWidth *= vertexWidth;
					float drawWidth = MathF.Sin(Math.Min(i / 4f, 0.5f) * MathF.PI);
					float fade = 1;
					if (i > decrease)
					{
						fade *= Math.Clamp((decrease + 6 - i) / 6f, 0, 1);
					}
					if (timerProj > 50)
					{
						fade *= Math.Clamp((70 - timerProj) / 20f, 0, 1);
					}

					var drawColor = new Color(0.4f, 0.3f, 0.3f, 0);
					flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.035f + timeValue + r * 0.2f + addTimeValue, 0, drawWidth));
					flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.035f + timeValue + r * 0.2f + addTimeValue, 1, drawWidth));
				}
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_3.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.DissolveWithLight.Value;
		float dissolveDuration = timerProj / 80f * 1f - 0.5f;
		if (Projectile.timeLeft < 3520)
		{
			dissolveDuration = 1.2f;
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Sand.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uLightColor"].SetValue(lightColor.ToVector4());
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.4f, 0.3f, 0.3f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(2f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center, null, lightColor, Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		if (Projectile.timeLeft < 3520)
		{
			float fade = 1f;
			if (Projectile.timeLeft > 3500)
			{
				fade = (3520 - Projectile.timeLeft) / 20f;
			}
			var texGlow = ModAsset.SquamousRockSpike_glow_crack.Value;
			float breathValue = 0.8f + 0.2f * MathF.Sin((float)Main.timeForVisualEffects * 0.24f + Projectile.whoAmI);
			Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, breathValue) * breathValue * fade, Projectile.rotation, texGlow.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}

	private float GetFlowEffectRotation(float i, float r)
	{
		float timeValue = (float)Main.time * 0.04f;
		timeValue *= 0.5f;

		return r / 6f * MathHelper.TwoPi + MathF.Sin(i * 0.07f + r * 0.03f + timeValue + Projectile.whoAmI) * 1.4f + MathF.Sin(i * 0.12f + r + timeValue) * 0.2f;
	}

	public override void PostDraw(Color lightColor)
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawTrail_dark(lightColor);
		DrawTrail(lightColor);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public void DrawTrail(Color light)
	{
		float dissolveDuration = (3600 - Projectile.timeLeft) / 80f - 0.7f;
		if (Projectile.timeLeft < 3450)
		{
			dissolveDuration = 1;
		}
		float drawC = 0.4f * dissolveDuration;

		var bars = new List<Vertex2D>();
		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
				{
					return;
				}

				break;
			}

			trueL = i;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 27;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)trueL;
			var color = Color.Lerp(new Color(drawC * light.R / 255f * 0.3f, drawC * light.G / 255f * 0.2f, drawC * light.B / 255f * 0.1f, 0), new Color(0, 0, 0, 0), factor);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(1, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(1, 1, 0)));
		}
		if (bars.Count > 2)
		{
			Texture2D t = Commons.ModAsset.Trail_4.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawTrail_dark(Color light)
	{
		float dissolveDuration = (3600 - Projectile.timeLeft) / 80f - 0.7f;
		if (Projectile.timeLeft < 3450)
		{
			dissolveDuration = 1;
		}
		float drawC = 0.4f * dissolveDuration;
		var bars = new List<Vertex2D>();
		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
				{
					return;
				}

				break;
			}
			trueL = i;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 27;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)trueL;
			var color = Color.Lerp(new Color(drawC * light.R / 255f, drawC * light.G / 255f, drawC * light.B / 255f, drawC), new Color(0, 0, 0, 0), factor);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(1, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(1, 1, 0)));
		}
		if (bars.Count > 2)
		{
			Texture2D t = Commons.ModAsset.Trail_4_black.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
