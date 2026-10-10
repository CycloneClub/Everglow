using Everglow.Myth.TheFirefly.Dusts;
using Newtonsoft.Json.Linq;
using Terraria;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class MothArrow : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 600;
		Projectile.tileCollide = true;
		Projectile.DamageType = DamageClass.Ranged;
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0));
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
		if (Projectile.timeLeft % 3 == 0)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<BlueGlowAppear>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, Main.rand.NextFloat(0.6f, 1.8f));
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return true;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void OnKill(int timeLeft)
	{
		for (int j = 0; j < 16; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283);
			int num20 = Dust.NewDust(Projectile.Center + Vector2.Normalize(Projectile.velocity) * 16f, 0, 0, ModContent.DustType<BlueGlowAppear>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f));
			Main.dust[num20].noGravity = true;
		}
		for (int j = 0; j < 32; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283);
			int num21 = Dust.NewDust(Projectile.Center + Vector2.Normalize(Projectile.velocity) * 16f, 0, 0, ModContent.DustType<BlueParticleDark2>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(3.7f, 5.1f));
			Main.dust[num21].alpha = (int)(Main.dust[num21].scale * 50);
		}
	}
}
