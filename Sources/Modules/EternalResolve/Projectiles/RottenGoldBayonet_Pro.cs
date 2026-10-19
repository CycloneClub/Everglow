using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Projectiles
{
	public class RottenGoldBayonet_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			Projectile.localNPCHitCooldown = 8;
			AttackColor = new Color(105, 105, 255);
			MaxDarkAttackUnitCount = 8;
			OldColorFactor = 0.7f;
			CurrentColorFactor = 0.5f;
			ShadeMultiplicative_Modifier = 0.6f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 0.6f;
			LightColorValueMultiplicative_Modifier = 0.1f;
			AttackEffectWidth = 0.4f;
			HitTileSparkColor = new Color(105, 105, 255, 150);
		}

		private RottenGoldBayonet sourceItem = null;

		public override void OnSpawn(IEntitySource source)
		{
			if (source is EntitySource_ItemUse_WithAmmo eiw)
			{
				sourceItem = eiw.Item.ModItem as RottenGoldBayonet;
				if (sourceItem is null)
				{
					Projectile.Kill();
				}
			}
		}

		public override void VisualParticle()
		{
			Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
			Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.04f, 0.08f);
			if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
			{
				var dust = Dust.NewDustDirect(pos, Projectile.width, Projectile.height, ModContent.DustType<CorruptShine>(), 0, 0, 0, default, Main.rand.NextFloat(0.95f, 1.7f));
				dust.velocity = vel;
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			modifiers.ArmorPenetration += 5;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (sourceItem.specialDelay == 0)
			{
				sourceItem.specialDelay = 60;
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<RottenGoldBayonet_Mark>(), (int)(Projectile.damage * 1f), Projectile.knockBack * 2.97f, Projectile.owner, 0, target.whoAmI);
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
				// 暂停的时候可以有一个渐停效果，看起来很好
				bottomPos1 = bottomPos1 * 0.9f;
				bottomPos2 = bottomPos2 * 0.9f;
			}
			float scale = MathF.Sin((float)Main.timeForVisualEffects);
			DrawFlags(lightColor, -13, 14, ModAsset.RottenGoldBayonet_flag.Value, bottomPos1 * scale, bottomPos2 * scale);

			Texture2D itemTexture = ModAsset.RottenGoldBayonet_withouFlag.Value;
			Main.spriteBatch.Draw(itemTexture, ItemDraw.Postion - Main.screenPosition, null, lightColor, ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);
		}
	}
}
