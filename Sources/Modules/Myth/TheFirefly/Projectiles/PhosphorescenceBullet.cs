using Everglow.Myth.TheFirefly.Dusts;
using Terraria;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class PhosphorescenceBullet : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 15;
		Projectile.height = 15;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 120;
		Projectile.alpha = 0;
		Projectile.penetrate = 1;
		Projectile.scale = 1;
	}

	private bool aTm = false;

	public override void AI()
	{
		if (Projectile.timeLeft == 119 && !aTm)
		{
			Projectile.timeLeft = Main.rand.Next(80, 110);
			aTm = true;
		}
		Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		if (Projectile.timeLeft < 60)
		{
			Projectile.scale *= 0.96f;
			Projectile.velocity *= 0.99f;
			Projectile.velocity.Y += 0.05f;
		}
	}

	public override void OnKill(int timeLeft)
	{
		for (int j = 0; j < timeLeft / 8; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * Projectile.scale * 0.3f;
			int num20 = Dust.NewDust(Projectile.Center + Vector2.Normalize(Projectile.velocity) * 16f, 0, 0, ModContent.DustType<BlueGlowAppear>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * Projectile.scale * 0.4f);
			Main.dust[num20].noGravity = true;
		}
		for (int j = 0; j < timeLeft / 4; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * Projectile.scale * 0.3f;
			int num21 = Dust.NewDust(Projectile.Center + Vector2.Normalize(Projectile.velocity) * 16f, 0, 0, ModContent.DustType<BlueParticleDark2>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(3.7f, 5.1f));
			Main.dust[num21].alpha = (int)(Main.dust[num21].scale * 50);
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0));
	}
}
