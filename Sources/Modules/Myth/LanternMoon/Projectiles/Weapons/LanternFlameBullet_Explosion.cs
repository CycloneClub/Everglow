using Terraria.Localization;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class LanternFlameBullet_Explosion : Lantern_ExplosionEffect
{
	public override LocalizedText DisplayName => ModContent.GetInstance<Lantern_ExplosionEffect>().DisplayName;

	protected override DamageClass ExplosionDamageType => DamageClass.Ranged;
}
