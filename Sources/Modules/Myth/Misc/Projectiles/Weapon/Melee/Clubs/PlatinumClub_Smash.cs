namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class PlatinumClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.PlatinumClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}
}
