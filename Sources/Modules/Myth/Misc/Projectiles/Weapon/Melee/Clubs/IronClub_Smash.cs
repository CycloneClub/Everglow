namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class IronClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.IronClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}
}
