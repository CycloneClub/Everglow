using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Example.Projectiles;

public class ExampleClubProj : ClubProj
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override string GlowTexture => base.GlowTexture;

	public override void SetCustomDefaults()
	{
		EnableReflection = true;
		ReflectionStrength = 5f;
		ReflectionTexture = Commons.ModAsset.White_Mod;
	}

	public override void PostPreDraw() => base.PostPreDraw();
}
