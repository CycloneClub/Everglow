using Everglow.Commons.Templates.Weapons.Clubs;

namespace Everglow.Example.Projectiles;

public class ExampleClubSmash : ClubProjSmash
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override string Texture => ModAsset.ExampleClubProj_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
		ReflectionStrength = 5f;
	}
}
