namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class MythrilClub : ClubProj
{
	public override void SetCustomDefaults()
	{
		EnableReflection = true;
		Beta = 0.005f;
		MaxOmega = 0.43f;
		ReflectionTexture = ModAsset.MythrilClub_Light_Mod;
		ReflectionStrength = 2f;
	}
}
