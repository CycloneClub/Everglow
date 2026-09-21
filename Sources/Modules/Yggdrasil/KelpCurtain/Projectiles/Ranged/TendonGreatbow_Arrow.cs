namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class TendonGreatbow_Arrow : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BoneArrow}";

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;

		Projectile.DamageType = DamageClass.Ranged;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
		Projectile.arrow = true;
	}

	public override void AI()
	{
		Projectile.velocity.Y += 0.1f;
		if (Projectile.velocity.Y > 16f)
		{
			Projectile.velocity.Y = 16f;
		}
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
	}
}
