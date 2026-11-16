using Everglow.Commons.Templates.Weapons.StabbingSwords;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;

public class DevilHeartBayonet_Proj : StabbingProjectile
{
	public const int HealAmountOnStaminaDepleted = 5;

	public override void SetCustomDefaults()
	{
		AttackColor = new Color(255, 107, 171);
		MaxDarkAttackUnitCount = 4;
		OldColorFactor = 0.3f;
		CurrentColorFactor = 0.2f;
		ShadeMultiplicative_Modifier = 0.64f;
		ScaleMultiplicative_Modifier = 1;
		OldLightColorValue = 1f;
		LightColorValueMultiplicative_Modifier = 0.4f;
		AttackLength = 1.05f;
		AttackEffectWidth = 0.4f;
		HitTileSparkColor = new Color(0.8f, 0.32f, 0.65f, 0);
	}

	public override void OnStaminaDepleted(Player player)
	{
		player.Heal(HealAmountOnStaminaDepleted);
	}
}
