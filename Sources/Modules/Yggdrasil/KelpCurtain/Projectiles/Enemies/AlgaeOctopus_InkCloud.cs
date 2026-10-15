using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 覆藻章鱼 and 大型覆藻章鱼's 墨水云（ink cloud）- the shared ink projectile both octopuses attack with
/// (behavior blocks WwyUddu8SooiTDxy6tPcrgGEn3b and ZyROdHeOZoAGLsxHnTUcIK7unSb: 墨水云会造成黑暗效果2秒).
/// <para>
/// Spawned by both creatures from <c>NPC.GetSource_FromAI()</c> inside
/// <c>Main.netMode != NetmodeID.MultiplayerClient</c>, so the damage credit belongs to the creature and
/// the cloud is never duplicated per client (D-55). One class covers both owners because the damage is
/// carried through the synced <c>Projectile.ai[0]</c>: 覆藻章鱼 passes its 25 and 大型覆藻章鱼 its 60
/// (04-DEVIATIONS.md section 7 item 8 - the design's 4-cloud wave is four spawns of this one projectile,
/// a simplification of a pure-VFX multiplicity, never of the damage).
/// </para>
/// <para>
/// No approved artwork exists for this attack, so it requests only the existing shared fallback texture
/// instead of creating placeholder art (D-48/D-51).
/// </para>
/// </summary>
public class AlgaeOctopus_InkCloud : ModProjectile
{
	/// <summary>
	/// 墨水云会造成黑暗效果2秒: the design's own duration, 2 s = 120 ticks.
	/// </summary>
	public const int DarknessTicks = 120;

	/// <summary>
	/// The damage used when <c>Projectile.ai[0]</c> carries nothing: 覆藻章鱼's 25（墨水）, which is also
	/// the design's lower of the two ink values.
	/// </summary>
	public const int DefaultDamage = 25;

	/// <summary>
	/// 墨水云: a lingering cloud rather than a travelling shot, so the cloud's own lifetime is bounded
	/// and it drifts to a halt inside that window (D-54 default).
	/// </summary>
	private const int Lifetime = 240;

	/// <summary>墨水云: the per-tick bleed of the launched cloud's velocity (D-54 default).</summary>
	private const float Drag = 0.9f;

	/// <summary>
	/// 墨水云会造成黑暗效果2秒: the cloud's damage, read from the synced <c>Projectile.ai[0]</c> so one
	/// class carries both owners' values (25 / 60) instead of one class per owner.
	/// </summary>
	private int AssignedDamage => Projectile.ai[0] > 0f ? (int)Projectile.ai[0] : DefaultDamage;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	// Approved artwork is missing from the repository (D-48); reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		// A cloud of ink is wider than it is tall but roughly round in the water.
		Projectile.width = 44;
		Projectile.height = 44;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;

		// 墨水云 lingers: it is not consumed by the first player it touches, so a dash trail keeps
		// threatening the ground it was left on.
		Projectile.penetrate = -1;

		// The cloud does not bounce off terrain and is never stopped by the water it is fired into.
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = Lifetime;
	}

	public override void AI()
	{
		// 墨水云会造成黑暗效果: the damage the owner passed is kept on the synced ai[0], and damage is not
		// part of the projectile net message, so every side re-asserts it here.
		Projectile.damage = AssignedDamage;

		// 墨水云: the cloud spreads out of the dash and then hangs in the water.
		Projectile.velocity *= Drag;
		Projectile.rotation += 0.03f;

		if (!Main.dedServ && Main.rand.NextBool(3))
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DarkLakeBottomMudDust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.3f);
		}
	}

	/// <summary>
	/// 墨水云会造成黑暗效果2秒: every player the cloud touches gets the design's 2 s (120 ticks) of 黑暗
	/// (BuffID.Darkness, verified present in this tML build). tML documents this hook as running on the
	/// local client, so it is deliberately not wrapped in a multiplayer-client guard (the 剧毒蟾蜍
	/// precedent): such a guard would make the design's Darkness dead code in multiplayer.
	/// </summary>
	/// <param name="target">The player the cloud touched.</param>
	/// <param name="info">The resolved hit.</param>
	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(BuffID.Darkness, DarknessTicks);
	}

	/// <summary>
	/// The cloud dissipating. Client-only work, so it sits behind the dedicated-server guard (D-35/D-55)
	/// and reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	/// <param name="timeLeft">The projectile's remaining lifetime.</param>
	public override void OnKill(int timeLeft)
	{
		if (Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 8; i++)
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DarkLakeBottomMudDust>(), 0f, 0f);
			Main.dust[dust].velocity *= 0.6f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.9f, 1.5f);
		}
	}
}
