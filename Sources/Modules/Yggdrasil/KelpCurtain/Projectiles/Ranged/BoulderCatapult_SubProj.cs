using Terraria.Audio;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class BoulderCatapult_SubProj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MiniBoulder}";

	public override void SetDefaults()
	{
		Projectile.width = 14;
		Projectile.height = 22;

		Projectile.DamageType = DamageClass.Ranged;
		Projectile.friendly = true;
		Projectile.penetrate = 2;
		Projectile.timeLeft = 300;
		Projectile.tileCollide = true;
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

		if (!Main.dedServ && Main.rand.NextBool(3))
		{
			var dust = Dust.NewDustPerfect(Projectile.Center, DustID.Stone);
			dust.velocity = -Projectile.velocity * 0.2f;
			dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
			dust.noGravity = true;
		}
	}

	public override void OnKill(int timeLeft)
	{
		if (!Main.dedServ)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			for (int i = 0; i < 10; i++)
			{
				var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
				dust.noGravity = false;
				dust.velocity *= 0.5f;
				dust.scale = Main.rand.NextFloat(0.7f, 1.2f);
			}
		}
	}
}
