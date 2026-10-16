namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class ChlorophyteClub_smash : ClubProjSmash
{
	public override string Texture => ModAsset.ChlorophyteClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}

	public override void Smash(int level)
	{
		if (level == 0)
		{
			for (int i = 0; i < 3; i++)
			{
				Vector2 v0 = new Vector2(0, -1 * Player.gravDir).RotatedBy((i - 1) / 6f * MathHelper.TwoPi);
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, v0, ModContent.ProjectileType<ChlorophyteClub_fly_2>(), (int)(Projectile.damage * 0.16f), Projectile.knockBack * 0.4f, Projectile.owner, 0.5f);
				p0.rotation = Main.rand.NextFloat(6.283f);
			}
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChlorophyteClub_VFX_2>(), Projectile.damage, Projectile.knockBack * 0.4f, Projectile.owner, 0.4f, Player.gravDir);
		}
		if (level == 1)
		{
			for (int i = 0; i < 5; i++)
			{
				Vector2 v0 = new Vector2(0, -1 * Player.gravDir).RotatedBy((i - 2) / 10f * MathHelper.TwoPi);
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, v0, ModContent.ProjectileType<ChlorophyteClub_fly_2>(), (int)(Projectile.damage * 0.12f), Projectile.knockBack * 0.4f, Projectile.owner, 1.1f);
				p0.rotation = Main.rand.NextFloat(6.283f);
			}
			for (int i = 0; i < 6; i++)
			{
				Vector2 v0 = new Vector2(0, -1 * Player.gravDir).RotatedBy((i - 2.5f) / 10f * MathHelper.TwoPi);
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, v0, ModContent.ProjectileType<ChlorophyteClub_fly_2>(), (int)(Projectile.damage * 0.12f), Projectile.knockBack * 0.4f, Projectile.owner, 0.8f);
				p0.rotation = Main.rand.NextFloat(6.283f);
			}
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChlorophyteClub_VFX_2>(), Projectile.damage, Projectile.knockBack * 0.4f, Projectile.owner, 0.7f, Player.gravDir);
		}
		base.Smash(level);
	}
}
