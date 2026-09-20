using Terraria.Audio;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class BoulderCatapult_Proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override string Texture => Commons.ModAsset.White_Mod;

	private bool AppliedDirectBonus { get; set; }

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;

		Projectile.DamageType = DamageClass.Ranged;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
	}

	public override void AI()
	{
		Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
		if (Projectile.velocity.Y > 16f)
		{
			Projectile.velocity.Y = 16f;
		}
		Projectile.rotation += 0.2f * Projectile.direction;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Projectile.owner == Main.myPlayer && !AppliedDirectBonus)
		{
			AppliedDirectBonus = true;
			Main.player[Projectile.owner].ApplyDamageToNPC(target, (int)(Projectile.damage * 0.5f), Projectile.knockBack, 0);
		}
		Projectile.Kill();
	}

	public override void OnKill(int timeLeft)
	{
		if (!Main.dedServ)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
			for (int i = 0; i < 20; i++)
			{
				var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
				dust.noGravity = false;
				dust.velocity *= 0.5f;
				dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
			}
		}

		if (Main.myPlayer == Projectile.owner)
		{
			var projNum = Main.rand.Next(3, 7);
			float knockback = 1f;
			for (int i = 0; i < projNum; i++)
			{
				var projVelo = new Vector2(0, 10f).RotatedByRandom(MathHelper.TwoPi);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, projVelo, ModContent.ProjectileType<BoulderCatapult_SubProj>(), (int)(Projectile.damage * 0.15f), knockback, Projectile.owner);
			}
		}
	}
}
