using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Everglow.EternalResolve.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Projectiles
{
	public class PrisonFireBayonet_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			AttackColor = Color.Orange;
			MaxDarkAttackUnitCount = 6;
			OldColorFactor = 1f;
			CurrentColorFactor = 0.65f;
			ShadeMultiplicative_Modifier = 0.84f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 1f;
			LightColorValueMultiplicative_Modifier = 0.4f;
			AttackLength = 0.90f;
			AttackEffectWidth = 0.4f;
		}

		public int SuddenCooling = 0;

		public override void OnSpawn(IEntitySource source)
		{
			Player player = Main.player[Projectile.owner];
			if (!player.wet || player.lavaWet)
			{
				SuddenCooling = 0;
			}
			else
			{
				SuddenCooling = 60;
			}
		}

		public void GenerateSmoke(int frequency)
		{
			for (int g = 0; g < frequency; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, -4f);
				var smog = new VaporDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3 + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.24f, 0.24f)) * MathF.Sqrt(Main.rand.NextFloat(1f)) * 9f,
					MaxTime = Main.rand.Next(10, 90),
					Scale = Main.rand.NextFloat(20f, 135f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(-0.05f, -0.01f), 0 },
				};
				Ins.VFXManager.Add(smog);
			}
		}

		public override void AI()
		{
			base.AI();
			if (UpdateTimer % Projectile.extraUpdates == 0)
			{
				Player player = Main.player[Projectile.owner];
				if (!player.wet || player.lavaWet)
				{
					for (int x = 0; x < 4; x++)
					{
						Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 6f);
						Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.2f, 0.4f);
						if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
						{
							var dust = Dust.NewDustDirect(pos, Projectile.width, Projectile.height, ModContent.DustType<FlameShine>(), 0, 0, 0, default, Main.rand.NextFloat(0.35f, 0.6f));
							dust.velocity = vel;
						}
					}
					if (SuddenCooling > 0)
					{
						for (int x = 0; x < 4; x++)
						{
							Vector2 posII = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
							Vector2 velII = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.04f, 0.08f) + new Vector2(Main.rand.NextFloat(-1f, 1f), -SuddenCooling / 12f);
							if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, posII + velII, 0, 0))
							{
								GenerateSmoke(1);
							}
						}
						SuddenCooling -= 1;
					}
					else
					{
						SuddenCooling = 0;
					}
					Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 2f)).RotatedByRandom(MathHelper.TwoPi) + Projectile.velocity * 0.2f + player.velocity * 0.5f;
					var fire = new BayonetFlameDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = player.Center + new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3 + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.24f, 0.24f)) * MathF.Sqrt(Main.rand.NextFloat(1f)) * 6f,
						MaxTime = Main.rand.Next(6, 25),
						Scale = Main.rand.NextFloat(10f, 50f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
					};
					Ins.VFXManager.Add(fire);
				}
				else
				{
					Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
					Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.04f, 0.08f);
					if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
					{
						GenerateSmoke(1);
					}
					if (SuddenCooling < 60)
					{
						SuddenCooling += 1;
						for (int x = 0; x < 4; x++)
						{
							Vector2 posII = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
							Vector2 velII = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.04f, 0.08f) + new Vector2(Main.rand.NextFloat(-1f, 1f), (SuddenCooling - 60f) / 12f);
							if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, posII + velII, 0, 0))
							{
								GenerateSmoke(1);
							}
						}
						if (SuddenCooling == 1)
						{
							SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/CoolingSword"), Projectile.Center);
						}
					}
					else
					{
						SuddenCooling = 60;
					}
				}
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			if (!player.wet || player.lavaWet)
			{
				target.AddBuff(BuffID.OnFire3, 150);
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			Player player = Main.player[Projectile.owner];
			if (player.lavaWet)
			{
				modifiers.FinalDamage *= 2f;
			}
		}

		public override void PostDraw(Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			base.PostDraw(lightColor);
			float valueLight = Projectile.timeLeft / MaxDarkAttackUnitCount / (Projectile.extraUpdates + 1);
			Lighting.AddLight(Projectile.Center + Projectile.velocity, 1f * valueLight, 0.4f * valueLight, 0f);
			Texture2D light = Commons.ModAsset.StabbingProjectile.Value;
			Vector2 drawOrigin = light.Size() / 2f;
			if (Main.myPlayer == Projectile.owner)
			{
				if (player.channel && !player.noItems && !player.CCed)
				{
					if (!player.wet || player.lavaWet)
					{
						AttackColor = Color.Orange;
						MaxDarkAttackUnitCount = 6;
						OldColorFactor = 1f;
						CurrentColorFactor = 0.65f;
						ShadeMultiplicative_Modifier = 0.84f;
						ScaleMultiplicative_Modifier = 1;
						OldLightColorValue = 1f;
						LightColorValueMultiplicative_Modifier = 0.4f;
						AttackLength = 0.90f;
						AttackEffectWidth = 0.6f;

						for (int f = Projectile.timeLeft - 1; f > -1; f--)
						{
							float value = (MaxDarkAttackUnitCount - f) / (float)MaxDarkAttackUnitCount;
							Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, new Color(value, value * value * value * 0.5f, 0, 0), DarkAttackEffect[f].Rotation, drawOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
						}
						Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, new Color(255, 155, 0, 0), LightAttackEffect.Rotation, drawOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
					}
					else
					{
						AttackColor = new Color(80, 0, 0, 0);
						MaxDarkAttackUnitCount = 6;
						OldColorFactor = 0.4f;
						CurrentColorFactor = 0.65f;
						ShadeMultiplicative_Modifier = 0.74f;
						ScaleMultiplicative_Modifier = 1;
						OldLightColorValue = 1f;
						LightColorValueMultiplicative_Modifier = 0.4f;
						AttackLength = 0.90f;
						AttackEffectWidth = 0.6f;
						for (int f = Projectile.timeLeft - 1; f > -1; f--)
						{
							float value = (MaxDarkAttackUnitCount - f) / (float)MaxDarkAttackUnitCount;
							value *= value;
							Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, new Color(value * 0.1f, 0, 0, 0), DarkAttackEffect[f].Rotation, drawOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
						}
						Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, new Color(20, 0, 0, 0), LightAttackEffect.Rotation, drawOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
					}
				}
			}
		}
	}
}
