using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CobaltClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.CobaltClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}

	public override string TrailColorTex() => ModAsset.CobaltClub_Light_Mod;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[1] = -60;
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Projectile.ai[1]++;
		if (Projectile.ai[1] % 6 == 5 && Projectile.ai[1] > 0)
		{
			Projectile.ai[1] = 0;
			Vector2 addPos = new Vector2(0, -54).RotatedByRandom(6.283) + new Vector2(0, -60 * Player.gravDir);
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + addPos + new Vector2(0, -60 * Player.gravDir), -addPos * 0.1f + new Vector2(0, 12 * Player.gravDir), ModContent.ProjectileType<CobaltClub_Falling_Shoot>(), Projectile.damage / 4, 0, Player.whoAmI, 1.85f);
		}
		base.AI();
	}
}
