namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class GoldClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.GoldClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}
}
