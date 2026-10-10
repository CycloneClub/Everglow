namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class LanternPhantomBullet : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.timeLeft = 3600;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.penetrate = 1;
		Projectile.aiStyle = -1;
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0f, 0.1f));
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D black = ModAsset.LanternBullet_black.Value;
		Texture2D bullet = ModContent.Request<Texture2D>(Texture).Value;
		Texture2D bloom = ModAsset.LanternPhantomBullet_bloom.Value;

		Main.EntitySpriteDraw(black, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, Projectile.rotation, black.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(bullet, Projectile.Center - Main.screenPosition, null, Color.White * 0.75f, Projectile.rotation, bullet.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, bloom.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		return false;
	}
}
