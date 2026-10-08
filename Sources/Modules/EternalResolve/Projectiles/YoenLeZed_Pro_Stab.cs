using Everglow.Commons.Coroutines;
using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.EternalResolve.Buffs;
using Everglow.EternalResolve.VFXs;

namespace Everglow.EternalResolve.Projectiles
{
	public class YoenLeZed_Pro_Stab : StabbingProjectile_Stab
	{
		public int Timer = 0;

		public override void SetCustomDefaults()
		{
			StabColor = new Color(100, 180, 255);
			StabShade = 0.2f;
			StabDistance = 1.15f;
			StabEffectWidth = 0.4f;
			HitTileSparkColor = new Color(0.4f, 0.8f, 1f, 0);
		}

		public override IEnumerator<ICoroutineInstruction> Generate3DRingVFX(Vector2 velocity)
		{
			yield return new WaitForFrames(40);
			StabVFX v = new SelfLightingStabVFX()
			{
				Position = Projectile.Center + Projectile.velocity * StabDistance * 80 * (1 - StabTimer / 135f),
				Velocity = velocity,
				StabEffectColor = Color.Lerp(StabColor, Color.White, 0.2f),
				Scale = 30,
				MaxTime = 10,
				TimeLeft = 10,
			};
			if (StabEndPoint_WorldPos == Vector2.Zero)
			{
				Ins.VFXManager.Add(v);
			}
			yield return new WaitForFrames(40);
			v = new SelfLightingStabVFX()
			{
				Position = Projectile.Center + Projectile.velocity * StabDistance * 80 * (1 - StabTimer / 135f),
				Velocity = velocity,
				StabEffectColor = Color.Lerp(StabColor, Color.White, 0.4f),
				Scale = 15,
				MaxTime = 10,
				TimeLeft = 10,
			};
			if (StabEndPoint_WorldPos == Vector2.Zero)
			{
				Ins.VFXManager.Add(v);
			}
		}

		public override void DrawEffect(Color lightColor)
		{
			Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 50 * StabTimer / 120f * StabEffectWidth;
			Vector2 start = StabStartPoint_WorldPos;
			Vector2 end = Projectile.Center + Projectile.velocity * 100 * StabDistance;
			if (StabEndPoint_WorldPos != Vector2.Zero)
			{
				end = StabEndPoint_WorldPos;
			}
			float value = (Projectile.timeLeft + StabTimer) / 135f;
			var middle = Vector2.Lerp(end, start, MathF.Sqrt(value) * 0.5f);
			float time = (float)(Main.time * 0.03);
			float dark = MathF.Sin(value * MathF.PI) * 4;
			var bars = new List<Vertex2D>
			{
				new Vertex2D(start + normalized, new Color(0, 0, 0, 120) * 0.4f * StabShade, new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized, new Color(0, 0, 0, 120) * 0.4f * StabShade, new Vector3(1 + time, 1, 0)),
				new Vertex2D(middle + normalized, Color.White * 0.4f * dark * StabShade, new Vector3(0.5f + time, 0, 0.5f)),
				new Vertex2D(middle - normalized, Color.White * 0.4f * dark * StabShade, new Vector3(0.5f + time, 1, 0.5f)),
				new Vertex2D(end + normalized, Color.White * 0.9f * dark * StabShade, new Vector3(0f + time, 0, 1)),
				new Vertex2D(end - normalized, Color.White * 0.9f * dark * StabShade, new Vector3(0f + time, 1, 1)),
			};
			if (bars.Count >= 3)
			{
				SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
				Effect effect = Commons.ModAsset.StabSwordEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uProcession"].SetValue(0.21f);
				effect.CurrentTechnique.Passes[0].Apply();
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_black.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
			Color alphaColor = StabColor;
			alphaColor.A = 0;
			alphaColor.R = alphaColor.R;
			alphaColor.G = alphaColor.G;
			alphaColor.B = alphaColor.B;

			normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 72 * StabTimer / 120f * StabEffectWidth;
			bars = new List<Vertex2D>
			{
				new Vertex2D(start + normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 1, 0)),
				new Vertex2D(middle + normalized, alphaColor, new Vector3(0.5f + time, 0, 0.5f)),
				new Vertex2D(middle - normalized, alphaColor, new Vector3(0.5f + time, 1, 0.5f)),
				new Vertex2D(end + normalized, alphaColor * 1.2f, new Vector3(0f + time, 0, 1)),
				new Vertex2D(end - normalized, alphaColor * 1.2f, new Vector3(0f + time, 1, 1)),
			};
			if (bars.Count >= 3)
			{
				SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
				Effect effect = Commons.ModAsset.StabSwordEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uProcession"].SetValue(value * 1.1f);
				effect.CurrentTechnique.Passes[0].Apply();
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}

			alphaColor.A = 0;
			alphaColor.R = 255;
			alphaColor.G = 255;
			alphaColor.B = 255;
			normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 24 * StabTimer / 120f * StabEffectWidth;
			bars = new List<Vertex2D>
			{
				new Vertex2D(start + normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 1, 0)),
				new Vertex2D(middle + normalized, alphaColor, new Vector3(0.5f + time, 0, 0.5f)),
				new Vertex2D(middle - normalized, alphaColor, new Vector3(0.5f + time, 1, 0.5f)),
				new Vertex2D(end + normalized, alphaColor, new Vector3(0f + time, 0, 1)),
				new Vertex2D(end - normalized, alphaColor, new Vector3(0f + time, 1, 1)),
			};
			if (bars.Count >= 3)
			{
				SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
				Effect effect = Commons.ModAsset.StabSwordEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uProcession"].SetValue(value * 0.6f + 0.4f);
				effect.CurrentTechnique.Passes[0].Apply();
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
		}

		public void SplitVFX_Long(int frequency)
		{
			float mulVelocity = 1f;
			for (int g = 0; g < frequency; g++)
			{
				float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(4f, 10f));
				Vector2 afterVelocity = Projectile.velocity * 10;
				var electric = new YoenLeZedElecticFlow
				{
					Velocity = afterVelocity * mulVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * MathF.Sqrt(Main.rand.NextFloat(1f)) * 10,
					MaxTime = size * size / 8f,
					Scale = size,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), 2, Main.rand.NextFloat(0.2f, Main.rand.NextFloat(0.2f, 0.4f)) },
				};
				Ins.VFXManager.Add(electric);
			}
		}

		public void SplitVFX(int frequency)
		{
			float mulVelocity = 1f;
			for (int g = 0; g < frequency; g++)
			{
				float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(4f, 10f));
				Vector2 afterVelocity = Projectile.velocity * 10;
				var electric = new YoenLeZedElecticFlow
				{
					Velocity = afterVelocity * mulVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * MathF.Sqrt(Main.rand.NextFloat(1f)) * 10,
					MaxTime = size * size / 24f,
					Scale = size,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), 1, Main.rand.NextFloat(0.2f, Main.rand.NextFloat(0.2f, 0.4f)) },
				};
				Ins.VFXManager.Add(electric);
			}
		}

		public override void AI()
		{
			Timer++;
			if (Timer < 5)
			{
				SplitVFX_Long(1);
			}
			else
			{
				if (Main.rand.NextBool(30))
				{
					SplitVFX(1);
				}
			}

			Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.8f, 1f) * (StabTimer / 120f));
			base.AI();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			var d = Dust.NewDustDirect(target.Center, 0, 0, ModContent.DustType<TriggerElectricCurrentDust>(), 0, 0);
			d.scale = Main.rand.NextFloat(0.85f, 1.15f) * 0.1f;
			target.AddBuff(ModContent.BuffType<OnElectric>(), 270);
			base.OnHitNPC(target, hit, damageDone);
		}

		public override void HitTile()
		{
			base.HitTile();
			float scale = (260 - (StabEndPoint_WorldPos - StabStartPoint_WorldPos).Length()) / 9f;
			if (scale > 4.5f)
			{
				for (int g = 0; g < 10; g++)
				{
					float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(8f, 16f));
					Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(12, 15)).RotatedByRandom(6.283);
					var electric = new YoenLeZedElecticFlow
					{
						Velocity = afterVelocity * 0.6f,
						Active = true,
						Visible = true,
						Position = StabEndPoint_WorldPos + Vector2.Normalize(StabStartPoint_WorldPos - StabEndPoint_WorldPos) * 20,
						MaxTime = size * size / 16f,
						Scale = size,
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), 1, 0 },
					};
					Ins.VFXManager.Add(electric);
				}
			}
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), StabEndPoint_WorldPos, Vector2.zeroVector, ModContent.ProjectileType<YoenLeZed_Pro_Stab_HitTile>(), 1, 0, Projectile.owner, scale);
		}
	}
}
