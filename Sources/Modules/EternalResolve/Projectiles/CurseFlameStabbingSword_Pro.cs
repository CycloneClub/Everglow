using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.VFX.CommonVFXDusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;

namespace Everglow.EternalResolve.Projectiles
{
	public class CurseFlameStabbingSword_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			AttackColor = new Color(87, 224, 0);
			MaxDarkAttackUnitCount = 9;
			OldColorFactor = 0.3f;
			CurrentColorFactor = 0.6f;
			ShadeMultiplicative_Modifier = 0.9f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 1f;
			LightColorValueMultiplicative_Modifier = 0.8f;
			AttackLength = 1.15f;
			AttackEffectWidth = 0.4f;
			HitTileSparkColor = new Color(0.2f, 1f, 0f, 0);
		}

		public override void VisualParticle()
		{
			GenerateSpark();
			if (!Main.rand.NextBool(3))
			{
				GenerateVFX();
			}
		}

		private void GenerateVFX()
		{
			Vector2 playerVel = Main.player[Projectile.owner].velocity;
			Vector2 projVel = Projectile.velocity;
			float rot = Main.rand.NextFloat(-0.4f, 0.4f);
			Vector2 velocity = playerVel + projVel.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.75f, 3.25f);
			Vector2 position = Projectile.Center + projVel.RotatedBy(rot) * Main.rand.NextFloat(1f, 2.5f);
			var cf = new CursedFlame_flowDust
			{
				Velocity = velocity * 0.15f,
				Active = true,
				Visible = true,
				Position = position,
				MaxTime = Main.rand.Next(12, 22),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), -rot * 0.02f, Main.rand.NextFloat(9.6f, 20f), Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(cf);
		}

		private void GenerateSpark()
		{
			Vector2 playerVel = Main.player[Projectile.owner].velocity;
			Vector2 projVel = Projectile.velocity;
			float rot = Main.rand.NextFloat(-0.3f, 0.3f);
			Vector2 velocity = playerVel + projVel.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.75f, 2.25f);
			Vector2 position = Projectile.Center + projVel.RotatedBy(rot) * Main.rand.NextFloat(0.1f, 5f);
			var spark = new CurseFlameSparkDust
			{
				Velocity = velocity * 0.15f,
				Active = true,
				Visible = true,
				Position = position,
				MaxTime = Main.rand.Next(137, 245),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 17.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.02f, 0.02f) },
			};
			Ins.VFXManager.Add(spark);
		}

		public override void PostDraw(Player player, Color lightColor)
		{
			float value = Projectile.timeLeft / MaxDarkAttackUnitCount / (Projectile.extraUpdates + 1);
			Lighting.AddLight(Projectile.Center + Projectile.velocity, 0.2f * value, 0.24f * value, 0.3f * value);
			Texture2D itemTexture = TextureAssets.Item[player.HeldItem.type].Value;
			Texture2D shadow = Commons.ModAsset.Star2_black.Value;
			Texture2D light = Commons.ModAsset.StabbingProjectile.Value;
			Vector2 drawOrigin = light.Size() / 2f;
			Vector2 drawShadowOrigin = shadow.Size() / 2f;
			Main.spriteBatch.Draw(itemTexture, ItemDraw.Postion - Main.screenPosition + Projectile.velocity, null, lightColor, ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);
			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					Main.spriteBatch.Draw(shadow, DarkAttackEffect[f].Postion - Main.screenPosition, null, Color.White * DarkAttackEffect[f].DarkShadow, DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
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
						Main.spriteBatch.Draw(shadow, LightAttackEffect.Postion - Main.screenPosition, null, Color.White * CurrentColorFactor, LightAttackEffect.Rotation, drawShadowOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
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
			target.AddBuff(BuffID.CursedInferno, 600);
			base.OnHitNPC(target, hit, damageDone);
		}
	}
}
