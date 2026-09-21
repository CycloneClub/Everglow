using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 红针洋辣子's 红针 (needle): the segmented worm's only ranged attack, fired 4~6 at a time from its head
/// in the design's spiky-slime pattern (behavior block G1ModIH6no6tFXxeuLCcZ8HYnGc).
/// <para>
/// It is spawned by <c>RedNeedleCaterpillar</c> from <c>NPC.GetSource_FromAI()</c> inside
/// <c>Main.netMode != NetmodeID.MultiplayerClient</c>, so the damage credit belongs to the creature and
/// the volley is never duplicated per client (D-55). A <c>ModProjectile</c> owns no <c>NPC</c> member,
/// so the source call lives at that spawn site rather than in this file (the plan 04-02/04-06 precedent,
/// recorded rather than faked with a comment-only token).
/// </para>
/// <para>
/// No approved artwork exists for this attack, so the class requests only the existing shared fallback
/// texture instead of creating placeholder art (D-48/D-51).
/// </para>
/// </summary>
public class RedNeedleCaterpillar_Spike : ModProjectile
{
	/// <summary>Gravity per tick, in pixels per tick squared: the needle arcs like a thrown spike (D-54 default).</summary>
	private const float Gravity = 0.18f;

	/// <summary>Terminal fall speed, in pixels per tick (D-54 default).</summary>
	private const float MaxFallSpeed = 12f;

	/// <summary>命中后有25%概率会造成中毒20秒: 20 s = 1200 ticks, rolled 1 in 4.</summary>
	private const int PoisonLongTicks = 1200;

	/// <summary>The 25% long-poison denominator.</summary>
	private const int PoisonLongChanceDenominator = 4;

	/// <summary>37.5%概率造成中毒10秒: 10 s = 600 ticks, rolled 3 in 8.</summary>
	private const int PoisonShortTicks = 600;

	/// <summary>The 37.5% short-poison numerator (3 of 8).</summary>
	private const int PoisonShortChanceNumerator = 3;

	/// <summary>The 37.5% short-poison denominator (3 of 8).</summary>
	private const int PoisonShortChanceDenominator = 8;

	// No HJSON key is created for this class; localization stays deferred (D-20).
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	// Approved artwork is missing from the repository (D-48); reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 12;
		Projectile.height = 12;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = 1;

		// A needle: it buries itself in the first tile or player it meets, and a short lifetime keeps a
		// stray volley from lingering (D-54 default).
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 300;
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
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<KelpMoss_dust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
		}
	}

	/// <summary>
	/// 命中后有25%概率会造成中毒20秒，37.5%概率造成中毒10秒: the needle carries the same split as the
	/// creature's own contact damage. tML documents this hook as running on the local client, so it is
	/// deliberately not wrapped in a multiplayer-client guard.
	/// </summary>
	/// <param name="target">The player the needle struck.</param>
	/// <param name="info">The resolved hit.</param>
	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if (Main.rand.NextBool(PoisonLongChanceDenominator))
		{
			target.AddBuff(BuffID.Poisoned, PoisonLongTicks);
			return;
		}

		if (Main.rand.Next(PoisonShortChanceDenominator) < PoisonShortChanceNumerator)
		{
			target.AddBuff(BuffID.Poisoned, PoisonShortTicks);
		}
	}

	/// <summary>
	/// The needle breaking on terrain. Client-only work, so it sits behind the dedicated-server guard
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
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<KelpMoss_dust>(), 0f, 0f);
			Main.dust[dust].velocity *= 0.5f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.1f);
		}
	}
}
