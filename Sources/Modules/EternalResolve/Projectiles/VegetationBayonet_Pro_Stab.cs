using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Terraria.Audio;

namespace Everglow.EternalResolve.Projectiles
{
	public class VegetationBayonet_Pro_Stab : StabbingProjectile_Stab
	{
		public override void SetCustomDefaults()
		{
			StabColor = new Color(66, 137, 58);
			StabShade = 0.3f;
			StabEffectWidth = 0.4f;
			HitTileSparkColor = new Color(66, 137, 58, 60);
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
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_Black.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
			Color alphaColor = StabColor;
			alphaColor.A = 0;
			alphaColor.R = (byte)(alphaColor.R * lightColor.R / 255f);
			alphaColor.G = (byte)(alphaColor.G * lightColor.G / 255f);
			alphaColor.B = (byte)(alphaColor.B * lightColor.B / 255f);

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
			alphaColor.R = (byte)(0 * lightColor.R / 255f);
			alphaColor.G = (byte)(565 * lightColor.G / 255f);
			alphaColor.B = (byte)(0 * lightColor.B / 255f);
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

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.buffImmune[BuffID.Poisoned] = false;
			target.AddBuff(BuffID.Poisoned, 360);
		}

		public override void AI()
		{
			if (Main.rand.NextBool(7))
			{
				Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
				Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.1f, 0.2f);
				if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
				{
					var dust = Dust.NewDustDirect(pos, Projectile.width, Projectile.height, ModContent.DustType<LeafShine>(), 0, 0, 0, default, Main.rand.NextFloat(0.95f, 1.1f));
					dust.velocity = vel;
				}
			}
			base.AI();
		}

		public override void HitTile()
		{
			SoundEngine.PlaySound(SoundID.Dig.WithPitchOffset(Main.rand.NextFloat(0.6f, 1f)), Projectile.Center);
			var hitSparkFixed = new StabbingProjectile_HitEffect()
			{
				Active = true,
				Visible = true,
				Position = StabEndPoint_WorldPos,
				MaxTime = 16,
				Scale = 0.24f,
				Rotation = Projectile.velocity.ToRotation(),
				Color = HitTileSparkColor,
			};
			Ins.VFXManager.Add(hitSparkFixed);

			Vector2 tilePos = StabEndPoint_WorldPos + new Vector2(1, 0).RotatedBy(Projectile.velocity.ToRotation());
			Point tileCoord = tilePos.ToTileCoordinates();
			Tile tile = WorldGenMisc.SafeGetTile(tileCoord);
			if (TileUtils.Sets.TileFragile[tile.TileType])
			{
				WorldGenMisc.DamageTile(tileCoord, 100);
			}
		}
	}
}
