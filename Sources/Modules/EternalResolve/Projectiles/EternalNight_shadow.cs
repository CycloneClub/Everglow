using Everglow.Commons.DataStructures;
using Everglow.Commons.MEAC;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Projectiles
{
	public class EternalNight_shadow : ModProjectile, IWarpProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.netImportant = true;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 240;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.alpha = 255;
			Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
			Projectile.usesOwnerLight = true;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 4;
			Projectile.aiStyle = -1;
			ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 12800;
		}

		public int timeToKill = 0;
		public int targetNPC = -1;
		public int stickNPC = -1;
		public float relativeAngle = 0;
		public Vector2 relativePos = Vector2.zeroVector;
		public Vector2 startCenter = Vector2.zeroVector;
		public Vector2 endCenter = Vector2.zeroVector;
		public Vector2 flashVelocity = Vector2.zeroVector;
		public float hitTargetAngle = 0;

		public override void OnSpawn(IEntitySource source)
		{
			targetNPC = (int)Projectile.ai[0];
			startCenter = Projectile.Center;
			Projectile.ai[1] = Main.rand.NextFloat(1f);
			Projectile.ai[2] = Main.rand.NextFloat(1f);
		}

		public void UpdateTarget()
		{
			if (Projectile.ai[0] == -1)
			{
				return;
			}
			if (targetNPC >= 0 && targetNPC < 200)
			{
				if (Main.npc[targetNPC].active && !Main.npc[targetNPC].dontTakeDamage && (Main.npc[targetNPC].Center - Projectile.Center).Length() < 900f)
				{
					return;
				}
			}
			float minDis = 600;
			int whoAmI = 0;
			foreach (NPC npc in Main.npc)
			{
				if (npc.active && !npc.dontTakeDamage)
				{
					float length = (npc.Center - Projectile.Center).Length();
					if (!npc.CanBeChasedBy())
					{
						length = 580 + Main.rand.NextFloat(10f);
					}
					if (npc.type == NPCID.TargetDummy)
					{
						length = 590 + Main.rand.NextFloat(10f);
					}
					if (length < minDis)
					{
						minDis = npc.Center.Length();
						whoAmI = npc.whoAmI;
					}
				}
			}
			if (minDis != 600)
			{
				targetNPC = whoAmI;
			}
			else
			{
				targetNPC = -1;
			}
		}

		public void AdjustRotation()
		{
			if (Projectile.ai[0] == -1)
			{
				return;
			}
			var toTarget = new Vector2(0, 10);
			if (targetNPC != -1)
			{
				NPC target = Main.npc[targetNPC];
				toTarget = target.Center - Projectile.Center;
			}
			float aimRot = MathF.Atan2(toTarget.Y, toTarget.X) + MathF.PI * 0.25f;
			Projectile.rotation = Projectile.rotation * 0.8f + aimRot * 0.2f;
		}

		public override void AI()
		{
			timeToKill--;
			if (timeToKill < 0)
			{
				if (Projectile.timeLeft > 180)
				{
					UpdateTarget();
					if (Projectile.timeLeft < 240)
					{
						Projectile.alpha -= 6;
						if (Projectile.alpha < 0)
						{
							Projectile.alpha = 0;
						}
					}
					AdjustRotation();
				}
				if (Projectile.timeLeft == 180)
				{
					Projectile.friendly = true;
					Vector2 toTarget = new Vector2(10, -10).RotatedBy(Projectile.rotation);
					if (targetNPC != -1)
					{
						NPC target = Main.npc[targetNPC];
						toTarget = target.Center - Projectile.Center;
					}
					flashVelocity = toTarget;
					startCenter -= toTarget.SafeNormalize(new Vector2(0, 1)) * 120f;
					while (!Collide(Projectile.Center))
					{
						Projectile.Center += toTarget.SafeNormalize(new Vector2(0, 1));
						endCenter = Projectile.Center;
						if (Main.rand.NextBool(7))
						{
							var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<NightDust>(), 0, 0, 0, default, Main.rand.NextFloat(0.85f, 1.6f));
							dust.velocity = new Vector2(0, Main.rand.NextFloat(2f)).RotateRandom(6.283);
							dust.noGravity = true;
						}
						if (Projectile.Center.X <= 320 || Projectile.Center.X >= Main.maxTilesX * 16 - 320 || Projectile.Center.Y <= 320 || Projectile.Center.Y >= Main.maxTilesY * 16 - 320)
						{
							break;
						}
					}
					for (int x = 0; x < 15; x++)
					{
						var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<NightDust>(), 0, 0, 0, default, Main.rand.NextFloat(0.85f, 1.6f));
						dust.velocity = new Vector2(0, Main.rand.NextFloat(2f, 3f)).RotateRandom(6.283);
						dust.noGravity = true;
					}
					SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/Slash").WithVolumeScale(0.33f).WithPitchOffset(1f), Projectile.Center);
				}
				if (Projectile.timeLeft < 180)
				{
					if (Main.rand.NextBool(7))
					{
						var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<NightDust>(), 0, 0, 0, default, Main.rand.NextFloat(0.85f, 1.6f));
						dust.velocity = new Vector2(0, Main.rand.NextFloat(2f)).RotateRandom(6.283);
						dust.noGravity = true;
					}
					Projectile.velocity *= 0;
					Projectile.friendly = false;
					if (stickNPC != -1)
					{
						NPC stick = Main.npc[stickNPC];
						if (stick != null && stick.active)
						{
							Projectile.rotation = stick.rotation + relativeAngle;
							Projectile.Center = stick.Center + relativePos.RotatedBy(stick.rotation + relativeAngle - hitTargetAngle);
						}
						else
						{
							stickNPC = -1;
						}
					}
					else
					{
						if (!Collision.SolidCollision(Projectile.Center, 0, 0))
						{
							Projectile.velocity.Y += 0.2f;
						}
					}
				}
			}
		}

		public bool Collide(Vector2 positon)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc.active && !npc.dontTakeDamage)
				{
					if (new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1).Intersects(npc.Hitbox))
					{
						relativeAngle = Projectile.rotation - npc.rotation;
						hitTargetAngle = Projectile.rotation;
						relativePos = Projectile.Center - npc.Center;
						stickNPC = npc.whoAmI;
						return true;
					}
				}
			}
			return Collision.SolidCollision(positon, 0, 0);
		}

		public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			float coordY = 0.6f;
			if ((endCenter - Projectile.Center).Length() > 10)
			{
				coordY = 0.9f;
			}
			if (Projectile.timeLeft < 180 && endCenter != Vector2.zeroVector)
			{
				Vector2 hitCenter = startCenter + Vector2.Normalize(flashVelocity) * 120f;
				lightColor = Lighting.GetColor((int)(hitCenter.X / 16f), (int)(hitCenter.Y / 16f));
				float newTime = Math.Clamp(6 - Projectile.timeLeft / 30f, 0, 1);
				float value1 = MathF.Pow(newTime, 0.5f);
				float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 10f;
				Vector2 normalizedVelocity = flashVelocity.SafeNormalize(Vector2.zeroVector);
				Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width * 1.5f;

				var light = new Color(0.85f * lightColor.R / 255f, 0, 2f * lightColor.B / 255f, 0);
				light *= width / 10f;
				var bars = new List<Vertex2D>
				{
					new Vertex2D(startCenter + normalize - Main.screenPosition, light, new Vector3(0, 0, 0)),
					new Vertex2D(startCenter - normalize - Main.screenPosition, light, new Vector3(0, 1, 0)),
					new Vertex2D(endCenter + normalize - Main.screenPosition, light, new Vector3(coordY, 0, 0)),
					new Vertex2D(endCenter - normalize - Main.screenPosition, light, new Vector3(coordY, 1, 0)),
				};
				Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StabbingProjectile.Value;
				if (bars.Count > 3)
				{
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				}
				normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
				Color shadow = Color.White;
				bars = new List<Vertex2D>
				{
					new Vertex2D(startCenter + normalize - Main.screenPosition, shadow, new Vector3(0, 0, 0)),
					new Vertex2D(startCenter - normalize - Main.screenPosition, shadow, new Vector3(0, 1, 0)),
					new Vertex2D(endCenter + normalize - Main.screenPosition, shadow, new Vector3(coordY, 0, 0)),
					new Vertex2D(endCenter - normalize - Main.screenPosition, shadow, new Vector3(coordY, 1, 0)),
				};
				Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Star2_black.Value;
				if (bars.Count > 3)
				{
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				}
			}

			if (Projectile.timeLeft < 200 && Projectile.timeLeft > 130)
			{
				Texture2D texGlow = ModAsset.EternalNight_shadow_glow.Value;
				float glowValue = (Projectile.timeLeft - 130f) / 70f;
				glowValue = MathF.Sin(glowValue * MathF.PI) * 1.5f;
				Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(glowValue, glowValue, glowValue, 0), Projectile.rotation, texGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect dissolve = Commons.ModAsset.Dissolve.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
			float dissolveDuration = Projectile.timeLeft / 60f - 0.2f;
			if (Projectile.alpha > 0)
			{
				dissolveDuration = (255 - Projectile.alpha) / 255f * 1.2f - 0.2f;
			}
			dissolve.Parameters["uTransform"].SetValue(model * projection);
			dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
			dissolve.Parameters["duration"].SetValue(dissolveDuration);
			dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.3f, 0, 0.5f, 1f));
			dissolve.Parameters["uNoiseSize"].SetValue(4f);
			dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
			dissolve.CurrentTechnique.Passes[0].Apply();

			Texture2D tex = ModAsset.EternalNight_shadow.Value;
			float colorValue = (255 - Projectile.alpha) / 255f;
			Main.spriteBatch.Draw(tex, Projectile.Center, null, new Color(colorValue, colorValue, colorValue, colorValue), Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			// timeToKill = 90;
			Projectile.velocity *= 0;
			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			base.OnHitNPC(target, hit, damageDone);
		}

		public void DrawWarp(VFXBatch sb)
		{
			if (Projectile.timeLeft < 180)
			{
				float time = (float)(Main.time * 0.03);
				float newTime = Math.Clamp(6 - Projectile.timeLeft / 30f, 0, 1);
				float value1 = MathF.Pow(newTime, 0.5f);
				float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 5f;
				Vector2 normalizedVelocity = flashVelocity.SafeNormalize(Vector2.zeroVector);
				Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
				Vector2 start = startCenter - Main.screenPosition;
				Vector2 end = endCenter - Main.screenPosition;
				var middle = Vector2.Lerp(start, end, 0.5f);
				float rotation = MathF.Atan2(flashVelocity.Y, flashVelocity.X);
				Color alphaColor = Color.White;
				alphaColor.A = 0;
				alphaColor.R = (byte)((rotation + Math.PI) % 6.283 / 6.283 * 255);
				alphaColor.G = 15;
				var bars = new List<Vertex2D>
				{
					new Vertex2D(start - normalize, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0.3f, 0)),
					new Vertex2D(start + normalize, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0.7f, 0)),
					new Vertex2D(middle - normalize, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0.3f, 0.5f)),
					new Vertex2D(middle + normalize, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0.7f, 0.5f)),
					new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
					new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
				};
				sb.Draw(Commons.ModAsset.Trail_1.Value, bars, PrimitiveType.TriangleStrip);
			}
		}
	}
}
