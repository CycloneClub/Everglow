using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.EternalResolve.Buffs;
using Everglow.EternalResolve.VFXs;
using Terraria.Audio;
using Terraria.GameContent;

namespace Everglow.EternalResolve.Projectiles
{
	public class YoenLeZed_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			AttackColor = new Color(100, 180, 255);
			MaxDarkAttackUnitCount = 6;
			OldColorFactor = 0.2f;
			CurrentColorFactor = 0.2f;
			ShadeMultiplicative_Modifier = 0.74f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 1f;
			LightColorValueMultiplicative_Modifier = 0.8f;
			AttackLength = 1.15f;
			AttackEffectWidth = 0.4f;
			HitTileSparkColor = new Color(0.4f, 0.8f, 1f, 0);
		}

		public override void VisualParticle()
		{
			if (Main.rand.NextBool(3))
			{
				GenerateVFX(1);
			}
			if (Main.rand.NextBool(2))
			{
				SplitVFX(1);
			}
		}

		public override void AI()
		{
			base.AI();
			if (UpdateTimer % Projectile.extraUpdates == 0)
			{
				if (Main.timeForVisualEffects % 10 == 0)
				{
					SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/ElectricCurrency").WithVolume(0.6f), Projectile.Center);
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
			ActiveSound sound = SoundEngine.FindActiveSound(new SoundStyle("Everglow/EternalResolve/Sounds/ElectricCurrency").WithVolume(0.6f));
			if (sound != null)
			{
				sound.Stop();
			}
		}

		public void GenerateVFX(int Frequency)
		{
			float mulVelocity = Main.rand.NextFloat(0.25f, 0.5f);
			for (int g = 0; g < Frequency; g++)
			{
				float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(8f, 16f));
				Vector2 afterVelocity = Projectile.velocity.RotateRandom(0.7f);
				var electric = new YoenLeZedElecticFlow
				{
					Velocity = afterVelocity * mulVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = size * size / 12f,
					Scale = size,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), 1, Main.rand.NextFloat(-0.2f, 0.2f) },
				};
				Ins.VFXManager.Add(electric);
			}
		}

		public void SplitVFX(int Frequency)
		{
			float mulVelocity = 0.5f;
			for (int g = 0; g < Frequency; g++)
			{
				float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(4f, 10f));
				Vector2 afterVelocity = Projectile.velocity.RotateRandom(0.3f);
				var electric = new YoenLeZedElecticFlow
				{
					Velocity = afterVelocity * mulVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * MathF.Sqrt(Main.rand.NextFloat(1f)) * 6f - afterVelocity,
					MaxTime = size * size / 18f,
					Scale = size,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), 2, Main.rand.NextFloat(-0.2f, 0.2f) },
				};
				Ins.VFXManager.Add(electric);
			}
		}

		public override void PostDraw(Color lightColor)
		{
			Lighting.AddLight(Projectile.Center + Projectile.velocity, new Vector3(0.2f, 0.24f, 0.3f));
			Player player = Main.player[Projectile.owner];
			Texture2D itemTexture = TextureAssets.Item[Main.player[Projectile.owner].HeldItem.type].Value;
			Texture2D Shadow = Commons.ModAsset.Star2_black.Value;
			Texture2D light = Commons.ModAsset.StabbingProjectile.Value;
			Vector2 drawOrigin = light.Size() / 2f;
			Vector2 drawShadowOrigin = Shadow.Size() / 2f;
			Main.spriteBatch.Draw(itemTexture, ItemDraw.Postion - Main.screenPosition + Projectile.velocity, null, lightColor, ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);
			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					Main.spriteBatch.Draw(Shadow, DarkAttackEffect[f].Postion - Main.screenPosition, null, Color.White * DarkAttackEffect[f].DarkShadow, DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					Color fadeLight = AttackColor * DarkAttackEffect[f].DarkShadow;
					fadeLight.A = 0;
					fadeLight = fadeLight * OldLightColorValue * MathF.Pow(LightColorValueMultiplicative_Modifier, f);
					fadeLight = new Color(fadeLight.R / 255f, fadeLight.G / 255f, fadeLight.B / 255f, 0);
					Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, fadeLight, DarkAttackEffect[f].Rotation, drawOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					if (GlowColor != Color.Transparent)
					{
						Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, light.Frame(1, 6, 1, 1), GlowColor * MathF.Pow(FadeGlowColorValue, f), DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					}
				}
			}
			if (Main.myPlayer == Projectile.owner)
			{
				if (player.channel && !player.noItems && !player.CCed)
				{
					if (CurrentColorFactor > 0)
					{
						Main.spriteBatch.Draw(Shadow, LightAttackEffect.Postion - Main.screenPosition, null, Color.White * CurrentColorFactor, LightAttackEffect.Rotation, drawShadowOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
					}
					Color glowColor = AttackColor;
					glowColor.A = 0;
					Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, glowColor, LightAttackEffect.Rotation, drawOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
					if (GlowColor != Color.Transparent)
					{
						Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, GlowColor, LightAttackEffect.Rotation, drawShadowOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
					}
				}
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			var d = Dust.NewDustDirect(target.Center, 0, 0, ModContent.DustType<TriggerElectricCurrentDust>(), 0, 0);
			d.scale = Main.rand.NextFloat(0.85f, 1.15f) * 0.1f;
			target.AddBuff(ModContent.BuffType<OnElectric>(), 180);
			base.OnHitNPC(target, hit, damageDone);
		}
	}
}
