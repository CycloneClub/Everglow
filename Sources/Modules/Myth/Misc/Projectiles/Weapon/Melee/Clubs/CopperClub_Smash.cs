namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CopperClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.CopperClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}
}
