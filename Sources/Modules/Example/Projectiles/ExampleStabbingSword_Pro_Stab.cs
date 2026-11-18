using Everglow.Commons.Templates.Weapons.StabbingSwords;

namespace Everglow.Example.Projectiles;

public class ExampleStabbingSword_Pro_Stab : StabbingProjectile_Stab
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override void SetCustomDefaults()
	{
		StabColor = new Color(80, 34, 5);
		StabShade = 2f;
		StabDistance = 0.70f;
		StabEffectWidth = 0.4f;
	}
}
