using Everglow.Commons.Templates.Weapons.StabbingSwords;

namespace Everglow.EternalResolve.Projectiles
{
	public class IronStabbingSword_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			Projectile.localNPCHitCooldown = 8;
			AttackColor = new Color(120, 108, 96);
			MaxDarkAttackUnitCount = 4;
			OldColorFactor = 0.4f;
			CurrentColorFactor = 0.2f;
			ShadeMultiplicative_Modifier = 0.44f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 0.6f;
			LightColorValueMultiplicative_Modifier = 0.4f;
			AttackLength = 0.70f;
			AttackEffectWidth = 0.4f;
		}
	}
}
