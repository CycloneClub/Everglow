namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic;

public class EndlessCurseFlame : ModProjectile
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("EndlessCurseFlame");
	}

	public override void SetDefaults()
	{
		Projectile.width = 210;
		Projectile.height = 210;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 300;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.CursedInferno, 60, false);
	}

	public override void AI()
	{
		if (Projectile.timeLeft > 100)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(0f, 10f)).RotatedByRandom(Math.PI * 2f);
			int num3 = Dust.NewDust(Projectile.Center, 0, 0, DustID.CursedTorch, v.X, v.Y, 0, default, 3f);
			Main.dust[num3].noGravity = true;
			Main.dust[num3].velocity = v;
		}
		else
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(0f, 10f) * Projectile.timeLeft / 100f).RotatedByRandom(Math.PI * 2f);
			int num3 = Dust.NewDust(Projectile.Center, 0, 0, DustID.CursedTorch, v.X, v.Y, 0, default, 3f * Projectile.timeLeft / 100f);
			Main.dust[num3].noGravity = true;
			Main.dust[num3].velocity = v;
		}
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color?(new Color(0, 0, 0, 0));
	}
}
