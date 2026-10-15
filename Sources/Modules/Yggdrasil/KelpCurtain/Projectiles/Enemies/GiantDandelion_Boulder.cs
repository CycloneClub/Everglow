namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 巨树人's thrown boulder: a hostile, gravity-affected projectile that arcs toward the player,
/// rotates with its velocity and despawns on the first tile it strikes (behaviour block
/// Zm3ZdguPboviV1xCmcmcSIEznZd, mid-range swing and post-recovery yank).
/// No approved artwork exists for this attack, so it requests only the existing shared fallback
/// texture instead of creating placeholder art (03-DEVIATIONS.md section 9).
/// </summary>
public class GiantDandelion_Boulder : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	// Approved artwork is missing from the repository; reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	/// <summary>Gravity applied per tick, in pixels per tick squared (conservative default, D-34).</summary>
	private const float Gravity = 0.35f;

	/// <summary>Terminal fall speed, in pixels per tick.</summary>
	private const float MaxFallSpeed = 16f;

	/// <summary>Sprite spin per tick, in radians.</summary>
	private const float SpinSpeed = 0.2f;

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 600;
	}

	public override void AI()
	{
		// A rotating boulder on a gravity arc.
		Projectile.velocity.Y += Gravity;
		if (Projectile.velocity.Y > MaxFallSpeed)
		{
			Projectile.velocity.Y = MaxFallSpeed;
		}

		int spinDirection = Projectile.velocity.X >= 0f ? 1 : -1;
		Projectile.rotation += SpinSpeed * spinDirection;

		if (!Main.dedServ && Main.rand.NextBool(3))
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.2f);
		}
	}

	public override void OnKill(int timeLeft)
	{
		if (!Main.dedServ)
		{
			for (int i = 0; i < 10; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
				Main.dust[dust].velocity *= 0.5f;
				Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
			}
		}
	}
}
