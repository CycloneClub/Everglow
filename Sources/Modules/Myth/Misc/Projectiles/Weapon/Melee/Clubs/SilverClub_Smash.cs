namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class SilverClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.SilverClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}
}
