using Everglow.Commons.Templates.Weapons;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class HuskburstBullet_SubProj : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 20;
		TrailTexture = Commons.ModAsset.Trail.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_black.Value;
		TrailBackgroundDarkness = 0.5f;
		TrailLength = 7;
		TrailColor = new Color(0.88f, 0.02f, 0f, 0f);
		TrailWidth = 6f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.damage += 2;
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		if (Projectile.timeLeft <= 19)
		{
			Projectile.friendly = true;
		}
		if (Projectile.timeLeft == 10)
		{
			DestroyEntity();
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
}
