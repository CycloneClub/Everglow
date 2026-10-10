namespace Everglow.Myth.OmniElementItems.Projectiles;

internal class LilyHarpWave : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = player.Center + new Vector2(player.direction * 12, -12 * player.gravDir);
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.itemTime <= 0)
		{
			Projectile.Kill();
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return true;
	}
}
