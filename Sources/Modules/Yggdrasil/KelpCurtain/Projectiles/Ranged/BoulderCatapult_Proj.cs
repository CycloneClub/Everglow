using Terraria.Audio;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class BoulderCatapult_Proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	/// <summary>
	/// Guards the 150% direct-hit bonus so it is applied at most once per projectile.
	/// </summary>
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
		// 直击造成150%伤害: the direct hit already dealt Projectile.damage, so add the
		// remaining 50% as a second owner-guarded hit exactly once.
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
			// 碎裂爆开3~6块小石块，造成15%伤害
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
