using Everglow.Commons.Coroutines;
using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Everglow.EternalResolve.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Projectiles
{
	public class EternalNight_Pro_Stab : StabbingProjectile_Stab
	{
		public override void SetCustomDefaults()
		{
			StabColor = new Color(119, 34, 255);
			StabShade = 1f;
			StabDistance = 1.40f;
			StabEffectWidth = 0.4f;
			HitTileSparkColor = new Color(0.05f, 0f, 0.15f, 0.75f);
		}

		public override IEnumerator<ICoroutineInstruction> Generate3DRingVFX(Vector2 velocity)
		{
			yield return new WaitForFrames(45);
			StabVFX v = new NightStabVFX()
			{
				Position = Projectile.Center + Projectile.velocity * StabDistance * 80 * (1 - StabTimer / 135f),
				Velocity = velocity,
				StabEffectColor = StabColor * 0.4f,
				Scale = 25,
				MaxTime = 10,
				TimeLeft = 10,
			};
			if (StabEndPoint_WorldPos == Vector2.Zero)
			{
				Ins.VFXManager.Add(v);
			}
			yield return new WaitForFrames(40);
			v = new NightStabVFX()
			{
				Position = Projectile.Center + Projectile.velocity * StabDistance * 80 * (1 - StabTimer / 135f),
				Velocity = velocity,
				StabEffectColor = StabColor * 0.4f,
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
			Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 60 * StabTimer / 120f * StabEffectWidth;
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
				new Vertex2D(start + normalized, new Color(120, 120, 120, 120) * 0.9f * StabShade, new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized, new Color(120, 120, 120, 120) * 0.9f * StabShade, new Vector3(1 + time, 1, 0)),
				new Vertex2D(middle + normalized, Color.White * 0.8f * dark * StabShade, new Vector3(0.5f + time, 0, 0.5f)),
				new Vertex2D(middle - normalized, Color.White * 0.8f * dark * StabShade, new Vector3(0.5f + time, 1, 0.5f)),
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
				effect.Parameters["uProcession"].SetValue(0.5f);
				effect.CurrentTechnique.Passes[0].Apply();
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
			Color alphaColor = StabColor;
			alphaColor.A = 0;
			alphaColor.R = (byte)(alphaColor.R * lightColor.R / 255f);
			alphaColor.G = (byte)(alphaColor.G * lightColor.G / 255f);
			alphaColor.B = (byte)(alphaColor.B * lightColor.B / 255f);

			normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 96 * StabTimer / 120f * StabEffectWidth;
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
			alphaColor.R = (byte)(184 * lightColor.R / 255f);
			alphaColor.G = (byte)(0 * lightColor.G / 255f);
			alphaColor.B = (byte)(378 * lightColor.B / 255f);
			normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 96 * StabTimer / 120f * StabEffectWidth;
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

			if (StabEndPoint_WorldPos != Vector2.Zero)
			{
				end = StabEndPoint_WorldPos;
			}
			value = (Projectile.timeLeft + StabTimer) / 135f;
			middle = Vector2.Lerp(end, start, MathF.Sqrt(value) * 0.5f);
			time = (float)(Main.time * 0.03);
			normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 20 * StabTimer / 120f * StabEffectWidth;
			bars = new List<Vertex2D>
			{
				new Vertex2D(start + normalized, Color.White, new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized, Color.White, new Vector3(1 + time, 1, 0)),
				new Vertex2D(middle + normalized, Color.White, new Vector3(0.5f + time, 0, 0.5f)),
				new Vertex2D(middle - normalized, Color.White, new Vector3(0.5f + time, 1, 0.5f)),
				new Vertex2D(end + normalized, Color.White, new Vector3(0f + time, 0, 1)),
				new Vertex2D(end - normalized, Color.White, new Vector3(0f + time, 1, 1)),
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
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_7_black.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
		}

		public override void AI()
		{
			if (Main.rand.NextBool(7))
			{
				Vector2 end = Projectile.Center + Projectile.velocity * 80 * StabDistance;
				if (StabEndPoint_WorldPos != Vector2.zeroVector)
				{
					end = StabEndPoint_WorldPos;
				}
				var dust = Dust.NewDustDirect(Vector2.Lerp(StabStartPoint_WorldPos, end, Main.rand.NextFloat(0.3f, 1f)) - new Vector2(4), 0, 0, ModContent.DustType<NightDust>(), 0, 0, 0, default, Main.rand.NextFloat(0.85f, 1.2f));
				dust.velocity = new Vector2(0, Main.rand.NextFloat(2f)).RotateRandom(6.283);
				dust.noGravity = true;
			}
			base.AI();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			for (int k = 0; k < 3; k++)
			{
				var p0 = Projectile.NewProjectileDirect(
					Projectile.GetSource_FromAI(),
					target.Center - new Vector2(0, Main.rand.NextFloat(115, 180) * Main.player[Projectile.owner].gravDir).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)),
					Vector2.zeroVector, ModContent.ProjectileType<EternalNight_shadow>(),
					Projectile.damage / 7, Projectile.knockBack * 0.6f, Projectile.owner, target.whoAmI);
				p0.timeLeft = 240 + k * 5;
			}
			base.OnHitNPC(target, hit, damageDone);
		}

		public override void OnSpawn(IEntitySource source)
		{
			base.OnSpawn(source);
			for (int k = 0; k < 7; k++)
			{
				var p0 = Projectile.NewProjectileDirect(
					Projectile.GetSource_FromAI(),
					Projectile.Center + Projectile.velocity * k * 25f - new Vector2(0, Main.rand.NextFloat(80, 180) * Main.player[Projectile.owner].gravDir).RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)),
					Vector2.zeroVector, ModContent.ProjectileType<EternalNight_shadow>(),
					Projectile.damage / 7, Projectile.knockBack * 0.6f, Projectile.owner, -1);
				p0.timeLeft = 240 + k * 4;
				p0.rotation = MathF.PI * 0.75f + Main.rand.NextFloat(-1f, 1f);
			}
		}
	}
}
