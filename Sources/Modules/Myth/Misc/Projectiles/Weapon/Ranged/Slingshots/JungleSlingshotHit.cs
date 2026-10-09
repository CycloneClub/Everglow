namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

class JungleSlingshotHit : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 68;
		Projectile.height = 68;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
	}
	public override void AI()
	{
		Projectile.velocity *= 0;
		for (int i = 0; i < Main.dust.Length; i++)
		{
			if (Main.dust[i].type == ModContent.DustType<Misc.Dusts.GlowSporeFlip>() || Main.dust[i].type == ModContent.DustType<Misc.Dusts.GlowSpore>())
			{
				if (Main.rand.NextBool(120))
				{
					int type = Main.dust[i].type;
					if (Main.rand.Next(100) > 50)
					{
						type = ModContent.DustType<Misc.Dusts.GlowSporeFlip>();
					}

					int r1 = Dust.NewDust(Main.dust[i].position, 0, 0, type, 0, 0, 200, default, Main.dust[i].scale * 0.75f);
					Main.dust[r1].velocity = Main.dust[i].velocity.RotatedBy(Main.rand.NextFloat(0.2f, 1.3f));
					Main.dust[r1].noGravity = true;
					int r2 = Dust.NewDust(Main.dust[i].position, 0, 0, type, 0, 0, 200, default, Main.dust[i].scale * 0.75f);
					Main.dust[r2].velocity = Main.dust[i].velocity.RotatedBy(Main.rand.NextFloat(-1.3f, -0.2f));
					Main.dust[r2].noGravity = true;
					Main.dust[i].active = false;
				}
			}
		}
	}
	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
