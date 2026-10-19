using Everglow.Commons.Templates.Weapons.StabbingSwords;

namespace Everglow.EternalResolve.Projectiles
{
	public class TinStabbingSword_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			Projectile.localNPCHitCooldown = 8;
			AttackColor = new Color(196, 176, 117);
			MaxDarkAttackUnitCount = 4;
			OldColorFactor = 0.7f;
			CurrentColorFactor = 0.2f;
			ShadeMultiplicative_Modifier = 0.64f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 1f;
			LightColorValueMultiplicative_Modifier = 0.4f;
			AttackLength = 0.70f;
			AttackEffectWidth = 0.4f;
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
			DrawFlags(lightColor, -9, 10, ModAsset.LeadStabbingSword_flag.Value, bottomPos1, bottomPos2);
			Texture2D itemTexture = ModAsset.LeadStabbingSword_withouFlag.Value;
			Main.spriteBatch.Draw(itemTexture, ItemDraw.Postion - Main.screenPosition, null, lightColor, ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);
		}
	}
}
