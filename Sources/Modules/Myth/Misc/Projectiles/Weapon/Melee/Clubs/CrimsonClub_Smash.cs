namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CrimsonClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.CrimsonClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}

	public override string TrailColorTex() => ModAsset.CrimsonClub_Light_Mod;
}
