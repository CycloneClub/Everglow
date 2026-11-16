namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class TinClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.TinClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}
}
