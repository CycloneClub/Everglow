using Everglow.Myth.Misc.Projectiles.Weapon.Magic.FireFeatherMagic;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class MythrilClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.MythrilClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}

	public override string TrailColorTex() => ModAsset.MythrilClub_Light_Mod;

	public override void Smash(int level)
	{
		if (level == 0)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Player.gravDir == 1 ? Player.Bottom : Player.Top, Vector2.zeroVector, ModContent.ProjectileType<MythrilClub_Smash_Explosion>(), 0, 0, Player.whoAmI, 1 * Projectile.scale, Main.rand.NextFloat(MathHelper.TwoPi));
			for (int i = 0; i <= 5; i++)
			{
				Projectile p1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Player.gravDir == 1 ? Player.Bottom : Player.Top, Vector2.zeroVector, ModContent.ProjectileType<MythrilClub_Smash_Explosion2>(), 0, 0, Player.whoAmI, 1 * Projectile.scale, i / 5f);
				p1.timeLeft = 65 + i * 3;
			}
			for (int i = -3; i <= 3; i++)
			{
				if (i == 0)
				{
					continue;
				}
				for (int j = -3; j <= 3; j++)
				{
					Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(i * 90 + Main.rand.NextFloat(-45, 45), 60), new Vector2(0, -8 + Math.Abs(i * 1.5f) + Main.rand.NextFloat(-3, 3)), ModContent.ProjectileType<MythrilClub_SmashMagicBall>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack * 0.4f, Projectile.owner, 0.24f);
					p0.rotation = Main.rand.NextFloat(6.283f);
					p0.timeLeft = Math.Abs(i * 8) + 120 + j * 2;
					int count = 0;
					Vector2 checkPoint = Projectile.Center + new Vector2(i * 90 + Main.rand.NextFloat(-45, 45), 60);
					while (TileUtils.PlatformCollision(checkPoint))
					{
						count++;
						checkPoint += new Vector2(0, -8);
						if (count > 200)
						{
							break;
						}
					}
					count = 0;
					while (!TileUtils.PlatformCollision(checkPoint) && !TileUtils.PlatformCollision(checkPoint + new Vector2(0, 8)))
					{
						count++;
						checkPoint += new Vector2(0, 8);
						if (count > 200)
						{
							break;
						}
					}
					p0.Center = checkPoint;
				}
			}
		}
		if (level == 1)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Player.gravDir == 1 ? Player.Bottom : Player.Top, Vector2.zeroVector, ModContent.ProjectileType<MythrilClub_Smash_Explosion>(), 0, 0, Player.whoAmI, 1.4f * Projectile.scale, Main.rand.NextFloat(MathHelper.TwoPi));
			for (int i = 0; i <= 5; i++)
			{
				Projectile p1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Player.gravDir == 1 ? Player.Bottom : Player.Top, Vector2.zeroVector, ModContent.ProjectileType<MythrilClub_Smash_Explosion2>(), 0, 0, Player.whoAmI, 1.4f * Projectile.scale, i / 5f);
				p1.timeLeft = 65 + i * 3;
			}
			for (int i = -6; i <= 6; i++)
			{
				for (int j = -3; j <= 3; j++)
				{
					if (i == 0)
					{
						continue;
					}
					Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(i * 90 + Main.rand.NextFloat(-45, 45), 60), new Vector2(0, -9 + Math.Abs(i * 0.7f) + Main.rand.NextFloat(-3, 3)), ModContent.ProjectileType<MythrilClub_SmashMagicBall>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack * 0.4f, Projectile.owner, 0.24f);
					p0.rotation = Main.rand.NextFloat(6.283f);
					p0.timeLeft = Math.Abs(i * 8) + 120 + j * 2;
					int count = 0;
					Vector2 checkPoint = Projectile.Center + new Vector2(i * 90 + Main.rand.NextFloat(-45, 45), 60);
					while (TileUtils.PlatformCollision(checkPoint))
					{
						count++;
						checkPoint += new Vector2(0, -8);
						if (count > 200)
						{
							break;
						}
					}
					count = 0;
					while (!TileUtils.PlatformCollision(checkPoint) && !TileUtils.PlatformCollision(checkPoint + new Vector2(0, 8)))
					{
						count++;
						checkPoint += new Vector2(0, 8);
						if (count > 200)
						{
							break;
						}
					}
					p0.Center = checkPoint;
				}
			}
		}
		base.Smash(level);
	}
}
