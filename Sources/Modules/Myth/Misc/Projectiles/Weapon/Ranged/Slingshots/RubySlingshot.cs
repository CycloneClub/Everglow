namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

internal class RubySlingshot : GemSlingshotProjectile
{
	public override void SetDef()
	{
		ShootProjType = ModContent.ProjectileType<RubyBead>();
		TexPath = "Misc/Projectiles/Weapon/Ranged/Slingshots/Ruby";
		base.SetDef();
	}
}
