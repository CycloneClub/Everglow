namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class RestrictionDroneRE01Laser : ModProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonProjectiles;

	public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.PurpleLaser}";

	public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.PurpleLaser);
		AIType = ProjectileID.PurpleLaser;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
	}
}
