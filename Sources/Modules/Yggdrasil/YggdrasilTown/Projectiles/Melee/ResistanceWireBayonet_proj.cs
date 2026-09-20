using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee
{
	public class ResistanceWireBayonet_proj : StabbingProjectile
	{
		public float Power = 0;

		public override void SetDefaults()
		{
			AttackColor = new Color(155, 162, 164);
			base.SetDefaults();
			MaxDarkAttackUnitCount = 4;
			OldColorFactor = 0.3f;
			CurrentColorFactor = 0.2f;
			ShadeMultiplicative_Modifier = 0.64f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 1f;
			LightColorValueMultiplicative_Modifier = 0.4f;
			AttackLength = 1.05f;
			AttackEffectWidth = 0.4f;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (Power > 10)
			{
				target.AddBuff(BuffID.OnFire, (int)Power * 10);
			}
			bool hasHit = target.HasBuff(ModContent.BuffType<HasHitByResistenceWireBayonet>());
			float mulDamage = 1f;
			mulDamage += Power * 0.8f / 100f;
			if (!hasHit)
			{
				target.AddBuff(ModContent.BuffType<HasHitByResistenceWireBayonet>(), 360000000);
				mulDamage *= 2.6f;
			}
			modifiers.FinalDamage *= mulDamage;
			Player player = Main.player[Projectile.owner];
			ResistanceWireBayonet rWB = null;
			if (player.HeldItem.ModItem is not null)
			{
				rWB = player.HeldItem.ModItem as ResistanceWireBayonet;
			}
			if (rWB == null)
			{
				return;
			}
			if (rWB.Power > 10)
			{
				rWB.Power -= 10;
			}
			else
			{
				rWB.Power = 0;
			}
		}

		public override void DrawEffect(Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			ResistanceWireBayonet rWB = null;
			if (player.HeldItem.ModItem is not null)
			{
				rWB = player.HeldItem.ModItem as ResistanceWireBayonet;
			}
			if (rWB == null)
			{
				return;
			}
			Power = rWB.Power;
			GlowColor = Color.Lerp(Color.Transparent, new Color(255, 106, 0, 0), Power / 10f);
			Texture2D Shadow = Commons.ModAsset.Star2_black.Value;
			Texture2D light = Commons.ModAsset.StabbingProjectile.Value;
			Vector2 drawOrigin = light.Size() / 2f;
			Vector2 drawShadowOrigin = Shadow.Size() / 2f;
			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					Main.spriteBatch.Draw(Shadow, DarkAttackEffect[f].Postion - Main.screenPosition, null, Color.White * (DarkAttackEffect[f].Color.A / 255f), DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					Color fadeLight = AttackColor * (DarkAttackEffect[f].Color.A / 255f);
					fadeLight.A = 0;
					fadeLight = fadeLight * OldLightColorValue * MathF.Pow(LightColorValueMultiplicative_Modifier, f);
					fadeLight = new Color(lightColor.R / 255f * fadeLight.R / 255f, lightColor.G / 255f * fadeLight.G / 255f, lightColor.B / 255f * fadeLight.B / 255f, 0);
					Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, fadeLight, DarkAttackEffect[f].Rotation, drawOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					if (GlowColor != Color.Transparent)
					{
						Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, GlowColor * MathF.Pow(FadeGlowColorValue, f), DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					}
				}
			}
			if (CurrentColorFactor > 0)
			{
				Main.spriteBatch.Draw(Shadow, LightAttackEffect.Postion - Main.screenPosition, null, Color.White * CurrentColorFactor, LightAttackEffect.Rotation, drawShadowOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
			}
			Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, new Color(lightColor.R / 255f * AttackColor.R / 255f, lightColor.G / 255f * AttackColor.G / 255f, lightColor.B / 255f * AttackColor.B / 255f, 0), LightAttackEffect.Rotation, drawOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
			if (GlowColor != Color.Transparent)
			{
				Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, GlowColor, LightAttackEffect.Rotation, drawShadowOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
			}
		}

		public override void DrawItem(Color lightColor)
		{
			base.DrawItem(lightColor);
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			// ResistanceWireBayonet rWB = null;
			// if (player.HeldItem.ModItem is not null)
			// {
			// rWB = player.HeldItem.ModItem as ResistanceWireBayonet;
			// }
			// if (rWB != null)
			// {
			// Power = rWB.Power;
			// }
			base.AI();
		}
	}
}
