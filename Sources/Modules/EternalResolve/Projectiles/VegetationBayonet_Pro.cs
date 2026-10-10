using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Projectiles
{
	public class VegetationBayonet_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			AttackColor = new Color(66, 137, 58);
			MaxDarkAttackUnitCount = 4;
			OldColorFactor = 0.7f;
			CurrentColorFactor = 0.3f;
			ShadeMultiplicative_Modifier = 0.3f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 0.6f;
			LightColorValueMultiplicative_Modifier = 0.1f;
			AttackEffectWidth = 0.4f;
			HitTileSparkColor = new Color(66, 137, 58, 60);
		}

		private VegetationBayonet sourceItem = null;

		public override void OnSpawn(IEntitySource source)
		{
			if (source is EntitySource_ItemUse_WithAmmo eiw)
			{
				sourceItem = eiw.Item.ModItem as VegetationBayonet;
				if (sourceItem is null)
				{
					Projectile.Kill();
				}
				else
				{
					sourceItem.specialDelay = 0;
				}
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.buffImmune[BuffID.Poisoned] = false;
			target.AddBuff(BuffID.Poisoned, 240);
		}

		public override void VisualParticle()
		{
			Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
			Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.1f, 0.2f);
			if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
			{
				var dust = Dust.NewDustDirect(pos, Projectile.width, Projectile.height, ModContent.DustType<LeafShine>(), 0, 0, 0, default, Main.rand.NextFloat(0.95f, 1.1f));
				dust.velocity = vel;
			}
			sourceItem.specialDelay++;
		}

		private float bottomPos1 = 0f;
		private float bottomPos2 = 0f;

		public override void DrawItem(Color lightColor)
		{
			Texture2D itemTexture = ModAsset.VegetationBayonet_withoutFlag.Value;
			Main.spriteBatch.Draw(itemTexture, ItemDraw.Postion - Main.screenPosition, null, lightColor, ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);
			Texture2D itemGlowTexture = ModAsset.VegetationBayonet_glow_item.Value;
			Main.spriteBatch.Draw(itemGlowTexture, ItemDraw.Postion - Main.screenPosition, null, new Color(255, 255, 255, 0), ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);

			if (!Main.gamePaused)
			{
				bottomPos1 = bottomPos1 * 0.8f + Main.rand.NextFloat(0.45f, 1.75f) * 0.2f;
				bottomPos2 = bottomPos2 * 0.8f + Main.rand.NextFloat(0.45f, 1.75f) * 0.2f;
			}
			else
			{
				// 暂停的时候可以有一个渐停效果，看起来很好
				bottomPos1 = bottomPos1 * 0.9f;
				bottomPos2 = bottomPos2 * 0.9f;
			}
			float scale = MathF.Sin((float)Main.time * 0.8f);
			DrawGlowBerry(lightColor, -5, 11, ModAsset.VegetationBayonet_flag.Value, scale);
			DrawGlowBerry(new Color(255, 255, 255, 0), -5, 11, ModAsset.VegetationBayonet_flag.Value, scale);
		}

		public void DrawGlowBerry(Color lightColor, float flagLeftX, float flagTopY, Texture2D flagTexture, float addRotation = 0f)
		{
			float flagTopCenter = flagLeftX + flagTexture.Width / 2f;
			Main.spriteBatch.Draw(flagTexture, ItemDraw.Postion + new Vector2(flagTopCenter, flagTopY).RotatedBy(ItemDraw.Rotation) - Main.screenPosition, null, lightColor, addRotation, new Vector2(0, flagTexture.Width / 2f), Projectile.scale, SpriteEffects.None, 0);
		}

		public override void PostDraw(Player player, Color lightColor)
		{
			Texture2D Shadow = Commons.ModAsset.Star2_black.Value;
			Texture2D light = Commons.ModAsset.StabbingProjectile.Value;
			Vector2 drawOrigin = light.Size() / 2f;
			Vector2 drawShadowOrigin = Shadow.Size() / 2f;
			DrawItem(lightColor);
			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					Main.spriteBatch.Draw(Shadow, DarkAttackEffect[f].Postion - Main.screenPosition, null, Color.White * DarkAttackEffect[f].DarkShadow, DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					Color fadeLight = new Color(147, 125, 119) * DarkAttackEffect[f].DarkShadow;
					fadeLight.A = 0;
					fadeLight = fadeLight * OldLightColorValue * MathF.Pow(LightColorValueMultiplicative_Modifier, f);
					fadeLight = new Color(lightColor.R / 255f * fadeLight.R / 255f, lightColor.G / 255f * fadeLight.G / 255f, lightColor.B / 255f * fadeLight.B / 255f, 0);
					Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, fadeLight, DarkAttackEffect[f].Rotation, drawOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
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
					Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, new Color(lightColor.R / 255f * AttackColor.R / 255f, lightColor.G / 255f * AttackColor.G / 255f, lightColor.B / 255f * AttackColor.B / 255f, 0), LightAttackEffect.Rotation, drawOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
				}
			}
		}

		public override void HitTileSound(float scale)
		{
			SoundEngine.PlaySound(SoundID.Dig.WithVolume(1 - scale / 2.42f).WithPitchOffset(Main.rand.NextFloat(0.6f, 1f)), Projectile.Center);
			Projectile.soundDelay = SoundTimer;
		}
	}
}
