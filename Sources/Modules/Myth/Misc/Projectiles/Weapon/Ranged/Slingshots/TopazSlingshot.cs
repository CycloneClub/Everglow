namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

internal class TopazSlingshot : GemSlingshotProjectile
{
	public override void SetDef()
	{
		ShootProjType = ModContent.ProjectileType<TopazBead>();
		TexPath = "Misc/Projectiles/Weapon/Ranged/Slingshots/Topaz";
		base.SetDef();
	}
}
