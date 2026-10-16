using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;

namespace Everglow.EternalResolve.Projectiles
{
	public class EternalNight_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			Projectile.localNPCHitCooldown = 8;
			AttackColor = new Color(189, 14, 255);
			MaxDarkAttackUnitCount = 8;
			OldColorFactor = 0.8f;
			CurrentColorFactor = 1;
			ShadeMultiplicative_Modifier = 0.7f;
			ScaleMultiplicative_Modifier = 0.7f;
			OldLightColorValue = 0.6f;
			LightColorValueMultiplicative_Modifier = 0.1f;
			AttackLength = 1.20f;
			AttackEffectWidth = 0.8f;
			HitTileSparkColor = new Color(0.25f, 0f, 0.55f, 0.75f);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(3))
			{
				var p0 = Projectile.NewProjectileDirect(
					Projectile.GetSource_FromAI(),
					Projectile.Center - new Vector2(0, Main.rand.NextFloat(115, 180) * Main.player[Projectile.owner].gravDir).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)),
					Vector2.zeroVector, ModContent.ProjectileType<EternalNight_shadow>(),
					Projectile.damage, Projectile.knockBack * 0.6f, Projectile.owner, target.whoAmI);
				p0.timeLeft = 240;
			}
			base.OnHitNPC(target, hit, damageDone);
		}

		public override void VisualParticle()
		{
			Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
			Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.04f, 0.08f);
			if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
			{
				var dust = Dust.NewDustDirect(pos, Projectile.width, Projectile.height, ModContent.DustType<NightDustMoveWithPlayer>(), 0, 0, 0, default, Main.rand.NextFloat(0.95f, 1.2f));
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

			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					Color fadeLight = AttackColor * DarkAttackEffect[f].DarkShadow;
					fadeLight.A = 0;
					fadeLight = fadeLight * OldLightColorValue * MathF.Pow(LightColorValueMultiplicative_Modifier, f);
					fadeLight = new Color(lightColor.R / 255f * fadeLight.R / 255f, lightColor.G / 255f * fadeLight.G / 255f, lightColor.B / 255f * fadeLight.B / 255f, 0);
					Vector2 lightSize = DarkAttackEffect[f].Size;
					lightSize.Y *= 2;
					Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, fadeLight, DarkAttackEffect[f].Rotation, drawOrigin, lightSize, SpriteEffects.None, 0f);
					Vector2 darkSize = DarkAttackEffect[f].Size;
					darkSize.Y *= 0.4f;
					Main.spriteBatch.Draw(Shadow, DarkAttackEffect[f].Postion - Main.screenPosition, null, Color.White * DarkAttackEffect[f].DarkShadow, DarkAttackEffect[f].Rotation, drawShadowOrigin, darkSize, SpriteEffects.None, 0f);
				}
			}
			Vector2 size = LightAttackEffect.Size;
			size.Y *= 1.1f;
			Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, new Color(lightColor.R / 255f * AttackColor.R / 255f, lightColor.G / 255f * AttackColor.G / 255f, lightColor.B / 255f * AttackColor.B / 255f, 0), LightAttackEffect.Rotation, drawOrigin, size, SpriteEffects.None, 0f);
			size = LightAttackEffect.Size;
			size.Y *= 0.4f;
			Main.spriteBatch.Draw(Shadow, LightAttackEffect.Postion - Main.screenPosition, null, Color.White * CurrentColorFactor, LightAttackEffect.Rotation, drawShadowOrigin, size, SpriteEffects.None, 0f);
		}
	}
}
