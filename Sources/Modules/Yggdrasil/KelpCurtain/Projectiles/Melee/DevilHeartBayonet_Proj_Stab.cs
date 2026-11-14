using Everglow.Commons.Templates.Weapons.StabbingSwords;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;

public class DevilHeartBayonet_Proj_Stab : StabbingProjectile_Stab
{
	public float Power = 0;

	public override void SetCustomDefaults()
	{
		StabColor = new Color(255, 107, 171);
		StabShade = 0.2f;
		StabDistance = 0.70f;
		StabEffectWidth = 0.4f;
		HitTileSparkColor = new Color(0.8f, 0.32f, 0.65f, 0);
	}

	public override void OnStaminaDepleted(Player player)
	{
		player.Heal(5);
	}
}
