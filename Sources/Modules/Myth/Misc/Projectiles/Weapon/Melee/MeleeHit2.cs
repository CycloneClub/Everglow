using Terraria.Localization;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class MeleeHit2 : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Null");
	}

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 1;
		Projectile.alpha = 0;
		Projectile.penetrate = 1;
		Projectile.scale = 1;
	}

	public override void AI()
	{
	}
}
