using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 放射虫's 水弹（water bolt）- the ranged half of the creature's attack
/// (behavior block TdwNdpn2XoFla9x8OUKc8AQanrd: 在水中时连续发射水弹进行攻击).
/// <para>
/// Spawned by <c>Radiolarian</c> from <c>NPC.GetSource_FromAI()</c> inside
/// <c>Main.netMode != NetmodeID.MultiplayerClient</c>, so the damage credit belongs to the creature and
/// the bolt is never duplicated per client (D-55). The 35 damage the design gives the ranged attack is
/// passed at the spawn site (the creature's own <c>NPC.damage</c> carries the melee 45).
/// </para>
/// <para>
/// No approved artwork exists for this attack, so it requests only the existing shared fallback texture
/// instead of creating placeholder art (D-48/D-51).
/// </para>
/// </summary>
public class Radiolarian_WaterBolt : ModProjectile
{
	/// <summary>
	/// 水弹: a bolt travelling through the lake, so its drag is the water's own and it never bounces off
	/// terrain. The design names no terrain interaction (D-54 default).
	/// </summary>
	private const float WaterDrag = 0.996f;

	/// <summary>The bolt's lifetime, in ticks, so a missed shot cannot accumulate forever (D-54 default).</summary>
	private const int Lifetime = 180;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	// Approved artwork is missing from the repository (D-48); reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 14;
		Projectile.height = 14;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = 1;

		// 水弹 travels through the water it is fired into: it is never stopped by a tile or slowed by the
		// liquid collision path, and its own drag models the water instead.
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = Lifetime;
	}

	public override void AI()
	{
		// 水弹: the water's own drag, applied on every side from the synced velocity.
		Projectile.velocity *= WaterDrag;
		Projectile.rotation = Projectile.velocity.ToRotation();

		// The bolt carries no debuff the design does not name, so its on-hit path is empty.

		if (!Main.dedServ && Main.rand.NextBool(3))
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<KelpWaterDrop>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.5f, 0.9f);
		}
	}

	/// <summary>
	/// The bolt bursting. Client-only work, so it sits behind the dedicated-server guard (D-35/D-55) and
	/// reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	/// <param name="timeLeft">The projectile's remaining lifetime.</param>
	public override void OnKill(int timeLeft)
	{
		if (Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 5; i++)
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<KelpWaterDrop>(), 0f, 0f);
			Main.dust[dust].velocity *= 0.5f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
		}
	}
}
