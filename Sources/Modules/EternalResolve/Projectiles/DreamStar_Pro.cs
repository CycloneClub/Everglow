using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.EternalResolve.Items.Miscs;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.EternalResolve.Projectiles
{
	public class DreamStar_Pro : StabbingProjectile
	{
		private NPC lastHitTarget = null;
		private NPC starNPC = null;
		internal int ContinuousHit = 0;

		public override int SoundTimer => 10;

		public override void SetCustomDefaults()
		{
			AttackColor = Color.Gold;
			MaxDarkAttackUnitCount = 6;
			OldColorFactor = 0.4f;
			CurrentColorFactor = 0.2f;
			ShadeMultiplicative_Modifier = 0.74f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 1f;
			LightColorValueMultiplicative_Modifier = 0.9f;
			AttackLength = 1.05f;
			AttackEffectWidth = 0.4f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
		}

		public override void HitTileEffect(Vector2 hitPosition, float rotation, float power)
		{
			HitTileSparkColor = new Color(230, 120, 195, 120);
			if (Main.rand.NextBool(5))
			{
				HitTileSparkColor = new Color(255, 231, 0, 0);
				power *= 1.5f;
			}
			var hitSparkFixed = new StabbingProjectile_HitEffect()
			{
				Active = true,
				Visible = true,
				Position = hitPosition,
				MaxTime = 8,
				Scale = Math.Min(0.12f * power, 0.5f),
				Rotation = rotation,
				Color = HitTileSparkColor,
			};
			Ins.VFXManager.Add(hitSparkFixed);
			Vector2 tilePos = hitPosition + new Vector2(1, 0).RotatedBy(rotation);
			Point tileCoord = tilePos.ToTileCoordinates();
			Tile tile = WorldGenMisc.SafeGetTile(tileCoord);
			if (TileUtils.Sets.TileFragile[tile.TileType])
			{
				WorldGenMisc.DamageTile(tileCoord, (int)(power * 10), Owner);
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (lastHitTarget != null)
			{
				if (lastHitTarget == target)
				{
					if (starNPC != target)
					{
						ContinuousHit++;
						if (ContinuousHit >= 5)
						{
							if (starNPC != null)
							{
								Projectile.NewProjectile(Projectile.GetSource_FromAI(), starNPC.Center, Vector2.zeroVector, ModContent.ProjectileType<DreamStar_Explosion>(), (int)(Projectile.damage * 4.26), Projectile.knockBack * 4.26f, Projectile.owner, 1.2f);
								Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<DreamStar_Explosion>(), (int)(Projectile.damage * 4.26), Projectile.knockBack * 4.26f, Projectile.owner, 1.2f);
							}
							starNPC = target;
							ContinuousHit = 0;
						}
					}
				}
				else
				{
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), lastHitTarget.Center, Vector2.zeroVector, ModContent.ProjectileType<DreamStar_Explosion>(), (int)(Projectile.damage * 2.20 * (ContinuousHit / 5f + 0.00f)), Projectile.knockBack * 2.20f * (ContinuousHit / 5f + 0.05f), Projectile.owner, ContinuousHit / 5f + 0.05f);
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<DreamStar_Explosion>(), (int)(Projectile.damage * 2.20 * (ContinuousHit / 5f + 0.00f)), Projectile.knockBack * 2.20f * (ContinuousHit / 5f + 0.05f), Projectile.owner, ContinuousHit / 5f + 0.05f);
					lastHitTarget = target;
					ContinuousHit = 0;
				}
			}
			else
			{
				lastHitTarget = target;
			}
		}

		public override void VisualParticle()
		{
			Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
			Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.1f, 0.2f);
			if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
			{
				int type = ModContent.DustType<StarShine_yellow>();
				if (Main.rand.NextBool(3))
				{
					type = ModContent.DustType<StarShine_purple>();
				}
				var dust = Dust.NewDustDirect(pos, Projectile.width, Projectile.height, type, 0, 0, 0, default, Main.rand.NextFloat(0.75f, 1.25f));
				dust.velocity = vel;
			}
			if (starNPC != null)
			{
				if (!starNPC.active)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), starNPC.Center, Vector2.zeroVector, ModContent.ProjectileType<DreamStar_Explosion>(), (int)(Projectile.damage * 4.26), Projectile.knockBack * 4.26f);
					Item.NewItem(starNPC.GetSource_Death(), starNPC.Hitbox, ModContent.ItemType<StarSeed>());
					starNPC = null;
				}
			}
			if (lastHitTarget != null)
			{
				if (!lastHitTarget.active)
				{
					lastHitTarget = null;
				}
			}
		}

		private float bottomPos1 = 0f;
		private float bottomPos2 = 0f;

		public override void DrawItem(Color lightColor)
		{
			if (!Main.gamePaused)
			{
				bottomPos1 = bottomPos1 * 0.8f + Main.rand.NextFloat(0.45f, 1.75f) * 0.2f;
				bottomPos2 = bottomPos2 * 0.8f + Main.rand.NextFloat(0.45f, 1.75f) * 0.2f;
			}
			else
			{
				bottomPos1 = bottomPos1 * 0.9f;
				bottomPos2 = bottomPos2 * 0.9f;
			}
			float scale = MathF.Sin((float)Main.timeForVisualEffects);
			DrawFlags(lightColor, -8, 10, ModAsset.DreamStar_flag.Value, bottomPos1 * scale, bottomPos2 * scale);

			Texture2D itemTexture = ModAsset.DreamStar_withouFlag.Value;
			Main.spriteBatch.Draw(itemTexture, ItemDraw.Postion - Main.screenPosition, null, lightColor, ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);
			Main.spriteBatch.Draw(ModAsset.DreamStar_glow.Value, ItemDraw.Postion - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);
		}

		public override void PostDraw(Player player, Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
		{
			Texture2D shadow = Commons.ModAsset.Star2_black.Value;
			Texture2D light = Commons.ModAsset.StabbingProjectile.Value;
			Vector2 drawOrigin = light.Size() / 2f;
			Vector2 drawShadowOrigin = shadow.Size() / 2f;
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			DrawItem(lightColor);
			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					Main.spriteBatch.Draw(shadow, DarkAttackEffect[f].Postion - Main.screenPosition, null, Color.White * DarkAttackEffect[f].DarkShadow, DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					Color fadeLight = new Color(230, 120, 195) * DarkAttackEffect[f].DarkShadow;
					fadeLight.A = 0;
					fadeLight = fadeLight * OldLightColorValue * MathF.Pow(LightColorValueMultiplicative_Modifier, f);
					if (fadeLight.G > 20)
					{
						fadeLight.G -= 20;
					}
					else
					{
						fadeLight.G = 0;
					}
					fadeLight = new Color(lightColor.R / 255f * fadeLight.R / 255f, lightColor.G / 255f * fadeLight.G / 255f, lightColor.B / 255f * fadeLight.B / 255f, 0);
					Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, fadeLight, DarkAttackEffect[f].Rotation, drawOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
				}
			}
			if (CurrentColorFactor > 0)
			{
				Main.spriteBatch.Draw(shadow, LightAttackEffect.Postion - Main.screenPosition, null, Color.White * CurrentColorFactor, LightAttackEffect.Rotation, drawShadowOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
			}
			Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, new Color(AttackColor.R / 255f, AttackColor.G / 255f, lightColor.B / 255f * AttackColor.B / 255f, 0), LightAttackEffect.Rotation, drawOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			float drawTimer = (float)Main.time * 0.04f;
			if (starNPC != null)
			{
				float starSize = MathF.Sqrt(starNPC.width * starNPC.height) * 0.8f;
				Vector2 drawStarCenter = starNPC.Center - Main.screenPosition;
				Color drawColor = Color.Yellow;
				drawColor.A = 0;
				var bars = new List<Vertex2D>();
				for (int x = 0; x < 5; x++)
				{
					Vector2 bump = new Vector2(0, starSize).RotatedBy(x / 5.0 * Math.PI * 2 + drawTimer * -0.4);
					bars.Add(new Vertex2D(drawStarCenter + bump, drawColor, new Vector3(x / 5.0f + drawTimer, 0, 0)));
					bars.Add(new Vertex2D(drawStarCenter + bump * 0.1f, drawColor, new Vector3(x / 5.0f + drawTimer, 1, 0)));
					Vector2 dent = new Vector2(0, starSize * 0.5f).RotatedBy((x + 0.5) / 5.0 * Math.PI * 2 + drawTimer * -0.4);
					bars.Add(new Vertex2D(drawStarCenter + dent, drawColor, new Vector3((x + 0.5f) / 5.0f + drawTimer, 0, 0)));
					bars.Add(new Vertex2D(drawStarCenter + dent * 0.1f, drawColor, new Vector3((x + 0.5f) / 5.0f + drawTimer, 1, 0)));
					if (x == 4)
					{
						bump = new Vector2(0, starSize).RotatedBy((x + 1) / 5.0 * Math.PI * 2 + drawTimer * -0.4);
						bars.Add(new Vertex2D(drawStarCenter + bump, drawColor, new Vector3((x + 1) / 5.0f + drawTimer, 0, 0)));
						bars.Add(new Vertex2D(drawStarCenter + bump * 0.1f, drawColor, new Vector3((x + 1) / 5.0f + drawTimer, 1, 0)));
					}
				}
				Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StabbingProjectile.Value;
				if (bars.Count > 3)
				{
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				}
			}
			if (lastHitTarget != null)
			{
				float lastHitSize = MathF.Sqrt(lastHitTarget.width * lastHitTarget.height) * 0.8f;
				Vector2 drawTargetCenter = lastHitTarget.Center - Main.screenPosition;
				Color drawColor = Color.Yellow;
				drawColor.A = 0;
				var bars = new List<Vertex2D>();
				for (int x = 0; x < ContinuousHit; x++)
				{
					Vector2 bump = new Vector2(0, lastHitSize).RotatedBy(x / 5.0 * Math.PI * 2 + drawTimer * -0.4);
					bars.Add(new Vertex2D(drawTargetCenter + bump, drawColor, new Vector3(x / 5.0f + drawTimer, 0, 0)));
					bars.Add(new Vertex2D(drawTargetCenter + bump * 0.1f, drawColor, new Vector3(x / 5.0f + drawTimer, 1, 0)));
					Vector2 dent = new Vector2(0, lastHitSize * 0.5f).RotatedBy((x + 0.5) / 5.0 * Math.PI * 2 + drawTimer * -0.4);
					bars.Add(new Vertex2D(drawTargetCenter + dent, drawColor, new Vector3((x + 0.5f) / 5.0f + drawTimer, 0, 0)));
					bars.Add(new Vertex2D(drawTargetCenter + dent * 0.1f, drawColor, new Vector3((x + 0.5f) / 5.0f + drawTimer, 1, 0)));
				}
				Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StabbingProjectile.Value;
				if (bars.Count > 3)
				{
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				}
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
	}
}
