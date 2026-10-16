namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class TitaniumClub_smash : ClubProjSmash
{
	public override string Texture => ModAsset.TitaniumClub_Mod;

	public override string TrailColorTex() => ModAsset.TitaniumClub_light_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
		ReflectionStrength = 5f;
	}

	public override void Smash(int level)
	{
		int fragmentCount = 0;
		foreach (Projectile fragment in Main.projectile)
		{
			if (fragment != null && fragment.active && fragment.owner == Projectile.owner && fragment.type == ModContent.ProjectileType<Titanium_Fragment>())
			{
				fragmentCount++;
			}
		}

		if (level == 0)
		{
			for (int x = 0; x < 6 && fragmentCount < 24; x++)
			{
				Vector2 v0 = new Vector2(0, -Main.rand.NextFloat(7, 14f) * Player.gravDir).RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f));
				Projectile p0 = Projectile.NewProjectileDirect(null, Player.Center, v0, ModContent.ProjectileType<Titanium_Fragment>(), (int)(Projectile.damage * 0.5f), 0, Player.whoAmI, 72);
				fragmentCount++;
			}
			foreach (Projectile projectile in Main.projectile)
			{
				if (projectile != null && projectile.active)
				{
					if (projectile.type == ModContent.ProjectileType<Titanium_Fragment>())
					{
						if (projectile.owner == Projectile.owner)
						{
							Vector2 v0 = new Vector2(0, -Main.rand.NextFloat(7, 12f) * Player.gravDir).RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f));
							projectile.velocity = v0;
							Titanium_Fragment tF = projectile.ModProjectile as Titanium_Fragment;
							if (tF != null)
							{
								tF.AITimer = Main.rand.Next(20, 50);
							}
						}
					}
				}
			}

			Projectile.NewProjectileDirect(null, Player.gravDir == 1 ? Player.Bottom : Player.Top, Vector2.zeroVector, ModContent.ProjectileType<TitaniumClub_smash_explosion>(), 0, 0, Player.whoAmI, 20, 0, Player.gravDir);
		}
		if (level == 1)
		{
			for (int x = 0; x < 12 && fragmentCount < 24; x++)
			{
				Vector2 v0 = new Vector2(0, -Main.rand.NextFloat(7, 24f) * Player.gravDir).RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f));
				Projectile p0 = Projectile.NewProjectileDirect(null, Player.Center, v0, ModContent.ProjectileType<Titanium_Fragment>(), (int)(Projectile.damage * 0.5f), 0, Player.whoAmI, 144);
				fragmentCount++;
			}
			for (int x = 0; x < 12 && fragmentCount < 24; x++)
			{
				Vector2 v0 = new Vector2(0, -Main.rand.NextFloat(7, 24f) * Player.gravDir).RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f));
				Projectile p0 = Projectile.NewProjectileDirect(null, Player.Center, v0, ModContent.ProjectileType<Titanium_Fragment>(), (int)(Projectile.damage * 0.5f), 0, Player.whoAmI, 72);
				fragmentCount++;
			}
			foreach (Projectile projectile in Main.projectile)
			{
				if (projectile != null && projectile.active)
				{
					if (projectile.type == ModContent.ProjectileType<Titanium_Fragment>())
					{
						if (projectile.owner == Projectile.owner)
						{
							Vector2 v0 = new Vector2(0, -Main.rand.NextFloat(7, 12f) * Player.gravDir).RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f)) * projectile.ai[0] / 90f;
							projectile.velocity = v0;
							Titanium_Fragment tF = projectile.ModProjectile as Titanium_Fragment;
							if (tF != null)
							{
								tF.AITimer = Main.rand.Next(20, 50);
							}
						}
					}
				}
			}

			Projectile.NewProjectileDirect(null, Player.gravDir == 1 ? Player.Bottom : Player.Top, Vector2.zeroVector, ModContent.ProjectileType<TitaniumClub_smash_explosion>(), 0, 0, Player.whoAmI, 30, 0, Player.gravDir);
		}
		base.Smash(level);
	}
}
