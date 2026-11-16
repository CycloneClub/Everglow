namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class TinClub : ClubProjSmash
{
	public override string Texture => ModAsset.TinClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}
}
