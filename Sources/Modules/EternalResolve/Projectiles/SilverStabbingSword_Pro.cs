using Everglow.Commons.Templates.Weapons.StabbingSwords;

namespace Everglow.EternalResolve.Projectiles
{
	public class SilverStabbingSword_Pro : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			Projectile.localNPCHitCooldown = 8;
			AttackColor = new Color(180, 191, 193);
			MaxDarkAttackUnitCount = 4;
			OldColorFactor = 0.3f;
			CurrentColorFactor = 0.2f;
			ShadeMultiplicative_Modifier = 0.64f;
			ScaleMultiplicative_Modifier = 1;
			OldLightColorValue = 1f;
			LightColorValueMultiplicative_Modifier = 0.4f;
			AttackLength = 0.75f;
			AttackEffectWidth = 0.4f;
		}
	}
}
