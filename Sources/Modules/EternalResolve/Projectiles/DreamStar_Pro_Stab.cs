using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;

namespace Everglow.EternalResolve.Projectiles
{
	public class DreamStar_Pro_Stab : StabbingProjectile_Stab
	{
		internal int ContinuousHit = 0;

		public override void SetCustomDefaults()
		{
			StabColor = Color.Gold;
			StabShade = 0.2f;
			StabDistance = 1.05f;
			StabEffectWidth = 0.4f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 1000;
		}

		public override void HitTile()
		{
			for (int x = 0; x < 24; x++)
			{
				var dust = Dust.NewDustDirect(StabEndPoint_WorldPos, 0, 0, ModContent.DustType<StarShine_purple_withoutPlayer>());
				dust.scale = Main.rand.NextFloat(0.15f, 0.75f);
				dust.color.R = (byte)(dust.scale * 100f);
				if (dust.color.R < 25)
				{
					dust.color.R = 25;
				}
				dust.velocity = new Vector2(0, Main.rand.NextFloat(Main.rand.NextFloat(0f, 4f), 4f)).RotatedByRandom(6.283);
				dust.color.A = (byte)Main.rand.Next(68, 240);
			}
			for (int x = 0; x < 12; x++)
			{
				var dust = Dust.NewDustDirect(StabEndPoint_WorldPos, 0, 0, ModContent.DustType<StarShine_yellow_withoutPlayer>());
				dust.scale = Main.rand.NextFloat(0.15f, 0.75f);
				dust.color.R = (byte)(dust.scale * 100f);
				if (dust.color.R < 25)
				{
					dust.color.R = 25;
				}
				dust.velocity = new Vector2(0, Main.rand.NextFloat(Main.rand.NextFloat(0f, 7f), 7f)).RotatedByRandom(6.283);
				dust.color.A = (byte)Main.rand.Next(68, 240);
			}
			base.HitTile();
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			ContinuousHit++;
			Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center - new Vector2(Main.rand.NextFloat(-200, 200), 800), new Vector2(Main.rand.NextFloat(-5, 5), 20), ModContent.ProjectileType<DreamStar_FallenStar>(), Projectile.damage * 2, Projectile.knockBack * 2.20f, Projectile.owner, target.whoAmI);

			// Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<DreamStar_Explosion>(), (int)(Projectile.damage * 2.20 * (ContinuousHit / 5f + 0.10f)), Projectile.knockBack * 2.20f * (ContinuousHit / 5f + 0.05f), Projectile.owner, ContinuousHit / 5f + 0.05f);
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
			float dark = MathF.Sin(value * MathF.PI) * 16;
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
				effect.Parameters["uProcession"].SetValue(0.5f);
				effect.CurrentTechnique.Passes[0].Apply();
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
			var alphaColor = Color.Lerp(new Color(190, 2, 205), new Color(255, 255, 0), (value * 0.8f + time) % 1);
			alphaColor.A = 0;

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
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_6.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}

			alphaColor = new Color(255, 255, 0);
			alphaColor.A = 0;
			alphaColor.B = 0;
			normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 46 * StabTimer / 120f * StabEffectWidth;
			bars = new List<Vertex2D>
			{
				new Vertex2D(Vector2.Lerp(start, middle, 0.4f) + normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 0, 0)),
				new Vertex2D(Vector2.Lerp(start, middle, 0.4f) - normalized, new Color(0, 0, 0, 0), new Vector3(1 + time, 1, 0)),
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

		public override void AI()
		{
			if (Main.rand.NextBool(6))
			{
				Vector2 end = Projectile.Center + Projectile.velocity * 80 * StabDistance;
				if (StabEndPoint_WorldPos != Vector2.zeroVector)
				{
					end = StabEndPoint_WorldPos;
				}
				int type = ModContent.DustType<StarShine_yellow_withoutPlayer>();
				if (Main.rand.NextBool(3))
				{
					type = ModContent.DustType<StarShine_purple_withoutPlayer>();
				}
				var dust = Dust.NewDustDirect(Vector2.Lerp(StabStartPoint_WorldPos, end, Main.rand.NextFloat(0.3f, 1f)) - new Vector2(4), 0, 0, type, 0, 0, 0, default, Main.rand.NextFloat(0.24f, 0.85f));
				dust.color.R = (byte)(dust.scale * 100f);
				dust.color.A = (byte)Main.rand.Next(8, 240);
				dust.velocity = new Vector2(0, Main.rand.NextFloat(3f)).RotateRandom(6.283);
			}
			base.AI();
		}
	}
}
