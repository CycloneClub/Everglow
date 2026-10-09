using Terraria.Localization;

namespace Everglow.Myth.Misc.Projectiles.Weapon;

public class MagicHit : ModProjectile
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("");
	}

	public override void SetDefaults()
	{
		Projectile.width = 50;
		Projectile.height = 80;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 2;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1;
	}

	public override void AI()
	{
	}
}
