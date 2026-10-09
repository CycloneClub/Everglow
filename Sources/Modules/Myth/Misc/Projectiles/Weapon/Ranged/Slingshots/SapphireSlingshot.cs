namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

internal class SapphireSlingshot : GemSlingshotProjectile
{
	public override void SetDef()
	{
		ShootProjType = ModContent.ProjectileType<SapphireBead>();
		TexPath = "Misc/Projectiles/Weapon/Ranged/Slingshots/Sapphire";
		base.SetDef();
	}
}
