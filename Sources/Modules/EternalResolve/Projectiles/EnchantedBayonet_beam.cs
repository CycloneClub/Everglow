using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.EternalResolve.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace Everglow.EternalResolve.Projectiles
{
	public class EnchantedBayonet_beam : ModProjectile
	{
		public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.netImportant = true;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 3600;
			Projectile.alpha = 255;
		}

		public override void AI()
		{
			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 25;
			}
			else
			{
				Projectile.alpha = 0;
			}

			float alphaValue = (255 - Projectile.alpha) / 255f;
			Vector2 newVelocity = Projectile.velocity * 0.6f;
			var spark = new Spark_EnchantedStabDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + Projectile.velocity,
				maxTime = Main.rand.Next(14, 25),
				scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(1f, 7.0f)) * alphaValue,
				rotation = Main.rand.NextFloat(6.283f),
				noGravity = true,
				ai = new float[] { 2, Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
		}

		public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
		{
			float alphaValue = (255 - Projectile.alpha) / 255f;
			Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 50 * alphaValue;
			Vector2 start = Projectile.Center;
			Vector2 end = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 90f;
			float time = (float)(Main.time * 0.03);
			var bars = new List<Vertex2D>
			{
				new Vertex2D(start + normalized, new Color(40, 50, 255, 0), new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized, new Color(40, 50, 255, 0), new Vector3(1 + time, 1, 0)),
				new Vertex2D(end + normalized, Color.Transparent, new Vector3(0f + time, 0, 1)),
				new Vertex2D(end - normalized, Color.Transparent, new Vector3(0f + time, 1, 1)),
			};
			normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 25 * alphaValue;
			var bars2 = new List<Vertex2D>
			{
				new Vertex2D(start + normalized, new Color(0, 210, 255, 0), new Vector3(1 + time, 0, 0)),
				new Vertex2D(start - normalized, new Color(0, 210, 255, 0), new Vector3(1 + time, 1, 0)),
				new Vertex2D(end + normalized, Color.Transparent, new Vector3(0f + time, 0, 1)),
				new Vertex2D(end - normalized, Color.Transparent, new Vector3(0f + time, 1, 1)),
			};

			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			Effect effect = ModAsset.BeamFlow.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_4.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);

			float timeValue = 0;
			if (Projectile.timeLeft <= 200)
			{
				timeValue = (200 - Projectile.timeLeft) / 200f;
			}
			float dark = 0.75f;
			Texture2D light = Commons.ModAsset.Entities_Star.Value;
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0f * (1 - timeValue) * (1 - timeValue), 1f * (1 - timeValue), 1f, 0f), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * 0.2f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0f * (1 - timeValue) * (1 - timeValue), 1f * (1 - timeValue), 1f, 0f), 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * 0.2f, SpriteEffects.None, 0);
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return base.OnTileCollide(oldVelocity);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			base.OnHitNPC(target, hit, damageDone);
		}

		public override void OnKill(int timeLeft)
		{
			if (Projectile.alpha < 180)
			{
				SoundStyle ss = SoundID.NPCHit4;
				SoundEngine.PlaySound(ss.WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), Projectile.Center);
				for (int g = 0; g < 30; g++)
				{
					Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
					var spark = new Spark_EnchantedStabDust
					{
						velocity = newVelocity,
						Active = true,
						Visible = true,
						position = Projectile.Center,
						maxTime = Main.rand.Next(34, 75),
						scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(5f, 27.0f)),
						rotation = Main.rand.NextFloat(6.283f),
						noGravity = true,
						ai = new float[] { 2, Main.rand.NextFloat(-0.13f, 0.13f) },
					};
					Ins.VFXManager.Add(spark);
				}
			}
		}
	}
}
