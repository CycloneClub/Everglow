using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class HuskburstBullet_Proj : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 3600;
		TrailTexture = Commons.ModAsset.Trail.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_black.Value;
		TrailBackgroundDarkness = 0.5f;
		TrailLength = 7;
		TrailColor = new Color(0.5f, 0.35f, 0.3f, 0f);
		TrailWidth = 6f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.damage += 2;
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		if (Projectile.timeLeft == 3540)
		{
			Projectile.damage -= 2;
		}
	}

	public override void DrawSelf()
	{
		Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Vector4 drawRed = TrailColor.ToVector4() * lightColor.ToVector4();
		var drawColor = new Color(drawRed.X, drawRed.Y, drawRed.Z, drawRed.W);
		Texture2D light_dark = Commons.ModAsset.StarSlash_black.Value;
		Main.spriteBatch.Draw(light_dark, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light_dark.Width, light_dark.Height / 2), new Color(0.6f, 0.6f, 0.6f, 0.6f), Projectile.velocity.ToRotationSafe() - MathHelper.PiOver2, light_dark.Size() * 0.5f, 0.7f, SpriteEffects.None, 0);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width, light.Height / 2), drawColor, Projectile.velocity.ToRotationSafe() - MathHelper.PiOver2, light.Size() * 0.5f, 0.7f, SpriteEffects.None, 0);
		var bullerColor = drawColor;
		bullerColor.A = 150;
		Texture2D ball = Commons.ModAsset.TileBlock.Value;
		Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, bullerColor, (float)Main.time * 0.03f + Projectile.whoAmI, ball.Size() * 0.5f, 0.4f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, bullerColor, (float)Main.time * 0.03f + Projectile.whoAmI + MathHelper.PiOver4, ball.Size() * 0.5f, 0.4f, SpriteEffects.None, 0);
		return;
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue) => base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void DestroyEntityEffect()
	{
		SoundEngine.PlaySound(SoundID.NPCHit4.WithVolumeScale(0.8f), Projectile.Center);
		var vel = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<HuskburstBullet_SubProj>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack * 0.3f, Projectile.owner);
		for (int d = 0; d < 3; d++)
		{
			var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<Husk>());
			dust.velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.3f;
			dust.scale = 1.5f;
		}
	}
}
