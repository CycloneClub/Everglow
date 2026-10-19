using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;

namespace Everglow.EternalResolve.Projectiles
{
	public class EnchantedBayonet_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			Projectile.localNPCHitCooldown = 8;
			AttackColor = new Color(76, 126, 255);
			MaxDarkAttackUnitCount = 4;
			OldColorFactor = 0.8f;
			CurrentColorFactor = 0.2f;
			ShadeMultiplicative_Modifier = 0.82f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 1f;
			LightColorValueMultiplicative_Modifier = 0.5f;
			AttackLength = 0.88f;
			AttackEffectWidth = 0.4f;
		}

		public override void VisualParticle()
		{
			Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
			Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.04f, 0.08f);
			if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
			{
				var dust = Dust.NewDustDirect(pos, Projectile.width, Projectile.height, ModContent.DustType<EnchantedDustMoveWithPlayer>(), 0, 0, 0, default, Main.rand.NextFloat(0.45f, 0.7f));
				dust.velocity = vel;
				dust.noGravity = true;
			}
		}

		public override void DrawEffect(Color lightColor)
		{
			Texture2D Shadow = Commons.ModAsset.Star2_black.Value;
			Texture2D light = Commons.ModAsset.StabbingProjectile.Value;
			Vector2 drawOrigin = light.Size() / 2f;
			Vector2 drawShadowOrigin = Shadow.Size() / 2f;
			if (!Main.gamePaused)
			{
				switch (Main.rand.Next(4))
				{
					case 0:
						DarkAttackEffect[0].Color = new Color(76, 126, 255, (byte)(ShadeMultiplicative_Modifier * 25));
						break;
					case 1:
						DarkAttackEffect[0].Color = new Color(229, 111, 216, (byte)(ShadeMultiplicative_Modifier * 25));
						break;
					case 2:
						DarkAttackEffect[0].Color = new Color(247, 233, 141, (byte)(ShadeMultiplicative_Modifier * 25));
						break;
					case 3:
						DarkAttackEffect[0].Color = new Color(114, 177, 204, (byte)(ShadeMultiplicative_Modifier * 25));
						break;
				}
				DarkAttackEffect[0].DarkShadow = 0.4f;
				HitTileSparkColor = DarkAttackEffect[0].Color;
				HitTileSparkColor.A = 0;
			}
			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					Main.spriteBatch.Draw(Shadow, DarkAttackEffect[f].Postion - Main.screenPosition, null, Color.White * DarkAttackEffect[f].DarkShadow, DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					Color fadeLight = DarkAttackEffect[f].Color * DarkAttackEffect[f].DarkShadow;
					fadeLight.A = 0;
					fadeLight = fadeLight * OldLightColorValue * MathF.Pow(LightColorValueMultiplicative_Modifier, f);
					Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, fadeLight, DarkAttackEffect[f].Rotation, drawOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
				}
			}
			if (CurrentColorFactor > 0)
			{
				Main.spriteBatch.Draw(Shadow, LightAttackEffect.Postion - Main.screenPosition, null, Color.White * CurrentColorFactor, LightAttackEffect.Rotation, drawShadowOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
			}
			Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, new Color(lightColor.R / 255f * AttackColor.R / 255f, lightColor.G / 255f * AttackColor.G / 255f, lightColor.B / 255f * AttackColor.B / 255f, 0), LightAttackEffect.Rotation, drawOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
		}
	}
}
