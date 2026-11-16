namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CorruptClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.CorruptClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}
}
