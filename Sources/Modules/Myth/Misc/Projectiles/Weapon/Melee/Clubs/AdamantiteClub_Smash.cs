namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class AdamantiteClub_Smash : ClubProjSmash
{
	public override string Texture => ModAsset.AdamantiteClub_Mod;

	public override void SetDef()
	{
		EnableReflection = true;
	}

	public override void Smash(int level)
	{
		if (level == 0)
		{
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(20, 0), ModContent.ProjectileType<AdamantiteClub_Round>(), (int)(Projectile.damage * 0.2f), Projectile.knockBack * 0.4f, Projectile.owner, 0.5f);
			p0.rotation = Main.rand.NextFloat(6.283f);
			p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(-20, 0), ModContent.ProjectileType<AdamantiteClub_Round>(), (int)(Projectile.damage * 0.2f), Projectile.knockBack * 0.4f, Projectile.owner, 0.5f);
			p0.rotation = Main.rand.NextFloat(6.283f);

			for (int x = 0; x < 8; x++)
			{
				Vector2 v = new Vector2(0, Main.rand.NextFloat(4f, 8f)).RotatedByRandom(Math.PI * 2) * 8f;
				Vector2 v2 = new Vector2(0, 124).RotatedBy(Main.rand.NextFloat(-1f, 1f));
				Projectile p1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - v * 3 + v2 + new Vector2(0, -120), v, ModContent.ProjectileType<AdamantiteClub_Slash>(), Projectile.damage / 2, 0, Projectile.owner, Main.rand.NextFloat(-0.05f, 0.05f));
				p1.timeLeft = Main.rand.Next(120, 136) + x * 5;
			}
		}
		if (level == 1)
		{
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(20, 0), ModContent.ProjectileType<AdamantiteClub_Round>(), (int)(Projectile.damage * 0.32f), Projectile.knockBack * 0.4f, Projectile.owner, 1f);
			p0.rotation = Main.rand.NextFloat(6.283f);
			p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(-20, 0), ModContent.ProjectileType<AdamantiteClub_Round>(), (int)(Projectile.damage * 0.32f), Projectile.knockBack * 0.4f, Projectile.owner, 1f);
			p0.rotation = Main.rand.NextFloat(6.283f);

			for (int x = 0; x < 18; x++)
			{
				Vector2 v = new Vector2(0, Main.rand.NextFloat(4f, 8f)).RotatedByRandom(Math.PI * 2) * 11f;
				Vector2 v2 = new Vector2(0, 204).RotatedBy(Main.rand.NextFloat(-1f, 1f));
				Projectile p1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - v * 3 + v2 + new Vector2(0, -214), v, ModContent.ProjectileType<AdamantiteClub_Slash>(), Projectile.damage / 2, 0, Projectile.owner, Main.rand.NextFloat(-0.05f, 0.05f));
				p1.timeLeft = Main.rand.Next(120, 136) + x * 5;
			}
		}
		base.Smash(level);
	}
}
