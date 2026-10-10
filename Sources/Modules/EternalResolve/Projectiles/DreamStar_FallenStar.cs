using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;

namespace Everglow.EternalResolve.Projectiles
{
	public class DreamStar_FallenStar : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_12";

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 100;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;

			// Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
			oldPos = new Vector2[15];
			Projectile.extraUpdates = 1;
		}

		private Vector2[] oldPos;

		public static void TrackOldValue<T>(T[] array, T curValue)
		{
			for (int i = array.Length - 1; i > 0; i--)
			{
				array[i] = array[i - 1];
			}
			array[0] = curValue;
		}

		public override void OnSpawn(IEntitySource source)
		{
			SoundEngine.PlaySound(SoundID.Item9);
			base.OnSpawn(source);
		}

		public override void AI()
		{
			TrackOldValue(oldPos, Projectile.Center);
			Projectile.rotation += 0.2f;

			NPC target = Main.npc[(int)Projectile.ai[0]];
			if (target.active)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 15, 0.1f);
			}
			else
			{
				float maxdis = 500;
				foreach (NPC npc in Main.npc)
				{
					if (npc.CanBeChasedBy() && Vector2.Distance(npc.Center, Projectile.Center) < maxdis)
					{
						Projectile.ai[0] = npc.whoAmI;
						maxdis = Vector2.Distance(npc.Center, Projectile.Center);
					}
				}
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			for (int i = 0; i < 10f; i++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.YellowStarDust, 0, 0, 0, default, 1f);
				dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4f);
			}
			ParticleOrchestrator.BroadcastParticleSpawn(ParticleOrchestraType.TrueExcalibur, new ParticleOrchestraSettings()
			{
				PositionInWorld = Projectile.Center,
			});
			ParticleOrchestrator.BroadcastParticleSpawn(ParticleOrchestraType.StellarTune, new ParticleOrchestraSettings()
			{
				PositionInWorld = Projectile.Center,
			});
		}

		public override bool PreDraw(Player player, ref Color lightColor)
		{
			Texture2D tex0 = Terraria.GameContent.TextureAssets.MagicPixel.Value;

			var color = new Color(1f, 0.25f, 0.8f, 0.0f);
			var bars = new List<Vertex2D>
			{
				new Vertex2D(Projectile.Center - Main.screenPosition, color, new Vector3(0, 0.5f, 0)),
			};
			float counts = 0;
			for (int i = 0; i < oldPos.Length; i++)
			{
				if (oldPos[i] != Vector2.Zero)
				{
					counts++;
				}
			}
			for (int i = 0; i < oldPos.Length - 1; ++i)
			{
				if (oldPos[i + 1] == Vector2.Zero)
				{
					break;
				}

				var normalDir = oldPos[i] - oldPos[i + 1];
				normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
				var factor = i / (float)counts;
				var w = MathHelper.Lerp(1f, 0.1f, factor);
				float width = MathHelper.Lerp(10, 0, factor);
				bars.Add(new Vertex2D((oldPos[i] + oldPos[i + 1]) / 2 - Main.screenPosition + normalDir * width, color * w, new Vector3((float)Math.Sqrt(factor), 1, w)));
				bars.Add(new Vertex2D((oldPos[i] + oldPos[i + 1]) / 2 - Main.screenPosition + normalDir * -width, color * w, new Vector3((float)Math.Sqrt(factor), 0, w)));
			}
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;

			if (bars.Count >= 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);

			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
			Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f), Projectile.rotation, tex.Size() / 2, Projectile.scale, 0, 0);
			Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(0.7f, 0f, 0.7f, 0f), -Projectile.rotation, tex.Size() / 2, Projectile.scale * 1.2f, 0, 0);

			return false;
		}
	}
}
