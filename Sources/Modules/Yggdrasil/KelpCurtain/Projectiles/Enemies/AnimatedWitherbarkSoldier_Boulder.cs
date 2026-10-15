namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 枯木活化士兵（远程）'s 巨石（thrown boulder）- the ranged variant's only attack
/// (behaviour block MGYedPrhioUFFdxP5XrcMpnxnMd: 远程敌怪会向玩家投掷巨石).
/// <para>
/// Spawned by <c>AnimatedWitherbarkSoldierRanged</c> from <c>NPC.GetSource_FromAI()</c> inside
/// <c>Main.netMode != NetmodeID.MultiplayerClient</c>, so the damage credit belongs to the creature and the
/// boulder is never duplicated per client (D-55). No approved artwork exists for this attack, so it requests
/// only the existing shared fallback texture instead of creating placeholder art (D-48/D-51).
/// </para>
/// </summary>
public class AnimatedWitherbarkSoldier_Boulder : ModProjectile
{
	/// <summary>Gravity applied per tick, in pixels per tick squared (D-54 default, 04-DEVIATIONS.md section 10).</summary>
	private const float Gravity = 0.35f;

	/// <summary>Terminal fall speed, in pixels per tick.</summary>
	private const float MaxFallSpeed = 16f;

	/// <summary>Sprite spin per tick, in radians.</summary>
	private const float SpinSpeed = 0.18f;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	// Approved artwork is missing from the repository (D-48); reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;

		// A short lifetime: a missed boulder despawns instead of rolling through the courtyard forever.
		Projectile.timeLeft = 300;
	}

	public override void AI()
	{
		// A thrown rock on a gravity arc.
		Projectile.velocity.Y += Gravity;
		if (Projectile.velocity.Y > MaxFallSpeed)
		{
			Projectile.velocity.Y = MaxFallSpeed;
		}

		int spinDirection = Projectile.velocity.X >= 0f ? 1 : -1;
		Projectile.rotation += SpinSpeed * spinDirection;

		// Dust is client-only work: the dedicated server has no graphics access (D-35/D-55).
		if (!Main.dedServ && Main.rand.NextBool(3))
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.2f);
		}
	}

	public override void OnKill(int timeLeft)
	{
		if (Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 8; i++)
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
			Main.dust[dust].velocity *= 0.5f;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
		}
	}
}
