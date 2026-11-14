namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CobaltClub : ClubProj
{
	public override void SetCustomDefaults()
	{
		EnableReflection = true;
		Beta = 0.005f;
		MaxOmega = 0.4f;
		ReflectionTexture = ModAsset.CobaltClub_Light_Mod;
		ReflectionStrength = 2f;
	}
}
