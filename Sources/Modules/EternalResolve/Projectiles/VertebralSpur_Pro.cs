using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Terraria.Audio;

namespace Everglow.EternalResolve.Projectiles
{
	public class VertebralSpur_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			Projectile.localNPCHitCooldown = 8;
			AttackColor = new Color(105, 92, 76);
			MaxDarkAttackUnitCount = 4;
			OldColorFactor = 0.7f;
			CurrentColorFactor = 0.8f;
			ShadeMultiplicative_Modifier = 0.34f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 0.6f;
			LightColorValueMultiplicative_Modifier = 0.4f;
			AttackLength = 0.70f;
			AttackEffectWidth = 0.4f;
			HitTileSparkColor = new Color(105, 92, 76, 50);
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
			DrawFlags(lightColor, -13, 9, ModAsset.VertebralSpur_flag.Value, bottomPos1, bottomPos2);
			Texture2D itemTexture = ModAsset.VertebralSpur_withouFlag.Value;
			Main.spriteBatch.Draw(itemTexture, ItemDraw.Postion - Main.screenPosition, null, lightColor, ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);
		}

		public override void VisualParticle()
		{
			Vector2 pos = Projectile.position + Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.4f, 8f);
			Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.04f, 0.08f);
			if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, pos + vel, 0, 0))
			{
				var dust = Dust.NewDustDirect(pos, Projectile.width, Projectile.height, ModContent.DustType<Vertebral>(), 0, 0, 0, default, Main.rand.NextFloat(0.65f, 0.9f));
				dust.velocity = vel;
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			modifiers.FinalDamage.Multiplicative *= MathF.Min(2f, MathF.Log2(2 + (target.velocity - Main.player[Projectile.owner].velocity).Length()));
		}

		public override void HitTileSound(float scale)
		{
			SoundEngine.PlaySound(SoundID.Dig.WithVolume(1 - scale / 2.42f).WithPitchOffset(Main.rand.NextFloat(0.6f, 1f)), Projectile.Center);
			Projectile.soundDelay = SoundTimer;
		}
	}
}
