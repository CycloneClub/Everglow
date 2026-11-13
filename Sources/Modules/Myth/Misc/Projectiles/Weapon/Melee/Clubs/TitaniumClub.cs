namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class TitaniumClub : ClubProj
{
	public override void SetCustomDefaults()
	{
		EnableReflection = true;
		ReflectionStrength = 5f;
		ReflectionTexture = ModAsset.TitaniumClub_Light_Mod;
		Beta = 0.0062f;
		MaxOmega = 0.468f;
	}
}
