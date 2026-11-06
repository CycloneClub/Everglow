namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class OrichalcumClub_smash : ClubProjSmash
{
	public override string Texture => ModAsset.OrichalcumClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}

	public override void Smash(int level)
	{
		if (level == 0)
		{
			for (int x = 0; x < 7; x++)
			{
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Player.Center + new Vector2(Player.direction * 80 * x, -4), Vector2.zeroVector, ModContent.ProjectileType<OrichalcumPedal_slash>(), Projectile.damage / 2, 0, Player.whoAmI, 1.4f);
				p0.timeLeft = Main.rand.Next(120, 136) + x * 16;
			}
		}
		if (level == 1)
		{
			for (int x = 0; x < 12; x++)
			{
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Player.Center + new Vector2(Player.direction * 80 * x, -14), Vector2.zeroVector, ModContent.ProjectileType<OrichalcumPedal_slash>(), (int)(Projectile.damage * 0.85f), 0, Player.whoAmI, 1.85f);
				p0.timeLeft = Main.rand.Next(118, 126) + x * 13;
			}
		}
		base.Smash(level);
	}
}
