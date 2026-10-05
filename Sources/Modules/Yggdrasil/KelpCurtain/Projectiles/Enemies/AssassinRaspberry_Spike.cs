using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 阿萨辛覆盘子's 地刺 (ground spike): the buried ambusher's only attack, scattered 4~6 at a time toward the
/// prey from the exact 4~8 tile window (behavior block PqdMdnpwQomTTIxDt3PcdyGDnNd).
/// <para>
/// It is spawned by <c>AssassinRaspberry.FireSpikeScatter</c> from <c>NPC.GetSource_FromAI()</c> inside
/// <c>Main.netMode != NetmodeID.MultiplayerClient</c>, so the damage credit belongs to the creature and
/// the volley is never duplicated per client (D-55). A <c>ModProjectile</c> owns no <c>NPC</c> member, so
/// the source call lives at that spawn site rather than in this file (the plan 04-02/04-06 precedent,
/// recorded rather than faked with a comment-only token).
/// </para>
/// <para>
/// No approved artwork exists for this attack, so the class requests only the existing shared fallback
/// texture instead of creating placeholder art (D-48/D-51).
/// </para>
/// </summary>
public class AssassinRaspberry_Spike : ModProjectile
{
	/// <summary>Gravity per tick, in pixels per tick squared: the spike arcs back to the ground (D-54 default).</summary>
	private const float Gravity = 0.25f;

	/// <summary>Terminal fall speed, in pixels per tick (D-54 default).</summary>
	private const float MaxFallSpeed = 10f;

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	// Approved artwork is missing from the repository (D-48); reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = 1;

		// A ground spike: it lands and buries itself in the first tile it meets, and its lifetime is short
		// so a stray scatter cannot linger (D-54 default).
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 240;
	}

	public override void AI()
	{
		Projectile.velocity.Y += Gravity;
		if (Projectile.velocity.Y > MaxFallSpeed)
		{
			Projectile.velocity.Y = MaxFallSpeed;
		}

		Projectile.rotation = Projectile.velocity.ToRotation();

		if (!Main.dedServ && Main.rand.NextBool(3))
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SucculentHerbDust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
		}
	}

	/// <summary>
	/// The spike breaking on terrain. Client-only work, so it sits behind the dedicated-server guard
	/// (D-35/D-55) and reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	/// <param name="timeLeft">The projectile's remaining lifetime.</param>
	public override void OnKill(int timeLeft)
	{
		if (Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 4; i++)
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SucculentHerbDust>(), 0f, 0f);
			Main.dust[dust].velocity *= 0.5f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.1f);
		}
	}
}
