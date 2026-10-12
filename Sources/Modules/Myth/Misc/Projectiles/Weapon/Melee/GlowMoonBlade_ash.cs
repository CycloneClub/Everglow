namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee
{
	public class GlowMoonBlade_ash : ModProjectile
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

		public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.melee = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 255;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false; // 能穿墙，反义为false
			Projectile.timeLeft = 255; // 存在时间，60是1秒
			Projectile.extraUpdates = 12;
		}

		public override void AI()
		{
			Projectile.velocity *= 0;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			float energyValue = Projectile.timeLeft / 300f;

			Color c0 = new Color(energyValue, energyValue * energyValue, energyValue * energyValue * 0.5f, 0);
			energyValue *= energyValue;
			Vector2 size = new Vector2(5f * energyValue, 3) * 0.7f * Projectile.scale;
			Texture2D dark = Commons.ModAsset.StarSlash_black.Value;
			Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, Projectile.rotation, dark.Size() / 2f, size, SpriteEffects.None, 0);

			Texture2D light = Commons.ModAsset.StarSlash.Value;
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, light.Size() / 2f, size, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, light.Size() / 2f, size, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, light.Size() / 2f, size, SpriteEffects.None, 0);
			return false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			bool b0 = Collision.CheckAABBvLineCollision2(targetHitbox.TopLeft(), new Vector2(targetHitbox.Width, targetHitbox.Height), Projectile.Center + new Vector2(0, -100).RotatedBy(Projectile.rotation) * Projectile.scale, Projectile.Center + new Vector2(0, 100).RotatedBy(Projectile.rotation) * Projectile.scale);
			return b0;
		}
	}
}
