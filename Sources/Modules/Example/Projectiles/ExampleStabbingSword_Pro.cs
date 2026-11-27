using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Example.Items;
using Terraria.DataStructures;

namespace Everglow.Example.Projectiles;

public class ExampleStabbingSword_Pro : StabbingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override void SetCustomDefaults()
	{
		MaxDarkAttackUnitCount = 4;
		OldColorFactor = 0.7f;
		CurrentColorFactor = 0.2f;
		ShadeMultiplicative_Modifier = 0.64f;
		ScaleMultiplicative_Modifier = 1;
		OldLightColorValue = 1f;
		LightColorValueMultiplicative_Modifier = 0.4f;
		AttackLength = 0.70f;
		AttackEffectWidth = 0.4f;
	}

	public override void CustomBehavior()
	{
		float timeValue = (float)(Main.timeForVisualEffects / 120f);
		AttackColor = Main.hslToRgb(timeValue % 1.0f, 1, 0.5f);
	}

	public override void OnSpawn(IEntitySource source)
	{
		GlowColor = Owner.GetModPlayer<ExampleStabbingSword_Config>().GlowColor;
		base.OnSpawn(source);
	}
}
