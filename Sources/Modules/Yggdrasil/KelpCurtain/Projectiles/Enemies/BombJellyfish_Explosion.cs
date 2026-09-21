using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 爆弹水母's 死亡后自爆 (the death blast) - the expanding detonation the creature leaves behind when it dies.
/// Design row: 爆弹水母 (Bomb Jellyfish), behavior blocks OwnZdVZaJogXNSxk6M7cSF2enHW /
/// WxyJdoRyXo0cFNxkaGac67tNnde, region 亡碧湖 (Death Jade Lake).
/// <para>
/// Spawned by both variants of the row (<see cref="NPCs.DeathJadeLake.BombJellyfish"/> for 30, and its 大
/// sibling <see cref="NPCs.DeathJadeLake.LargeBombJellyfish"/> for 50) from <c>NPC.GetSource_FromAI()</c>
/// inside a <c>Main.netMode != NetmodeID.MultiplayerClient</c> guard, so exactly the authoritative side
/// creates it and one blast appears per death (D-55).
/// </para>
/// <para>
/// <b>One projectile, two damages (OQ2).</b> The design's 30（小） / 50（大） pair is <b>not</b> hard-coded here:
/// the spawning creature passes its own damage as the projectile's damage parameter <i>and</i> as
/// <c>ai0</c>, and this class reads that named <c>ai[]</c> value to scale the blast's radius. Both values are
/// therefore reachable from the damage parameter alone.
/// </para>
/// <para>
/// This is a hostile enemy projectile and deliberately does <b>not</b> reuse a friendly player-owned
/// projectile: the phase builds exactly eleven projectiles (04-DEVIATIONS.md section 7) and this is item 9.
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the projectile requests the shared
/// Commons.ModAsset.White_Mod fallback. When the art arrives the D-49 migration is adding
/// BombJellyfish_Explosion.png beside this .cs and deleting the Texture override (D-48/D-49); the runtime
/// asset path is Everglow/Yggdrasil/KelpCurtain/Projectiles/Enemies/BombJellyfish_Explosion.png.
/// </para>
/// </summary>
public class BombJellyfish_Explosion : ModProjectile
{
	/// <summary>The detonation's lifetime, in ticks (D-54 default: short enough to read as a blast).</summary>
	private const int Lifetime = 18;

	/// <summary>The detonation's hit box at the moment of death, in pixels.</summary>
	private const int InitialSize = 16;

	/// <summary>
	/// The blast radius, in pixels, per point of the design's blast damage. The design gives no radius, so
	/// the 小 variant's 30 resolves to 3 tiles and the 大 variant's 50 to 5 tiles, which is recorded as a
	/// D-54 conservative default (04-DEVIATIONS.md section 10).
	/// </summary>
	private const float RadiusPerDamage = 1.6f;

	/// <summary>
	/// The floor a blast always expands to even when no damage value reached it, so a projectile spawned
	/// without its <c>ai0</c> parameter still detonates visibly instead of collapsing to a point.
	/// </summary>
	private const float MinRadius = 24f;

	/// <summary>
	/// Named wrapper over <c>Projectile.ai[0]</c>: the design's blast damage as handed over by the dying
	/// creature. It is the single input the radius is derived from, so neither 30 nor 50 is hard-coded in
	/// this class.
	/// </summary>
	private int BlastDamage
	{
		get => (int)Projectile.ai[0];
		set => Projectile.ai[0] = value;
	}

	/// <summary>The blast's final radius, in pixels, derived from <see cref="BlastDamage"/>.</summary>
	private float BlastRadius => MathF.Max(MinRadius, BlastDamage * RadiusPerDamage);

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	// Approved artwork is missing from the repository (D-48); reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Projectile.width = InitialSize;
		Projectile.height = InitialSize;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;

		// 死亡后自爆: the blast stays where the creature died and is not stopped by terrain, so it reads as an
		// explosion rather than a projectile that can be blocked by the lake bed.
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;

		Projectile.knockBack = 0f;
		Projectile.timeLeft = Lifetime;

		// The damage parameter the dying creature passed is what actually hurts the player, and its copy in
		// ai0 is what BlastRadius reads. Both arrive from the spawn call, so no hard-coded 30 or 50 lives in
		// this class and nothing here needs to seed them.
	}

	/// <summary>
	/// The expanding detonation. It never travels - 死亡后自爆 means it detonates where the creature died - so
	/// the whole update grows its hit box symmetrically around its own centre and emits only client-side dust.
	/// </summary>
	public override void AI()
	{
		Projectile.velocity = Vector2.Zero;

		Vector2 center = Projectile.Center;
		float progress = MathHelper.Clamp((Lifetime - Projectile.timeLeft) / (float)Lifetime, 0f, 1f);
		int size = (int)MathHelper.Lerp(MinRadius, BlastRadius, progress);

		// Grow the box around the death point rather than letting it drift as it expands.
		Projectile.width = size;
		Projectile.height = size;
		Projectile.position = center - new Vector2(size / 2f, size / 2f);

		UpdateVisual();
	}

	/// <summary>
	/// The detonation's residue. Client-only work, so it sits behind the dedicated-server guard (D-35/D-55)
	/// and reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	public override void OnKill(int timeLeft)
	{
		if (Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 12; i++)
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<KelpWaterDrop>(), 0f, -0.4f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.5f);
		}
	}

	/// <summary>
	/// The blast's own puffing. Client-only work, so it sits behind the dedicated-server guard (D-35/D-55)
	/// and reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	private void UpdateVisual()
	{
		if (Main.dedServ)
		{
			return;
		}

		if (!Main.rand.NextBool(2))
		{
			return;
		}

		int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<KelpWaterDrop>(), 0f, -0.6f);
		Main.dust[dust].noGravity = true;
		Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.4f);
	}
}
