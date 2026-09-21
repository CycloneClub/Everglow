using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 枯木活化士兵（法术）'s 法术粒子束（spell beam）- one of the three rays the spell variant sprays forward
/// (behaviour block MGYedPrhioUFFdxP5XrcMpnxnMd: 法术为向前喷洒三束受重力影响的粒子).
/// <para>
/// Spawned three at a time by <c>AnimatedWitherbarkSoldierSpell</c> from <c>NPC.GetSource_FromAI()</c> inside
/// <c>Main.netMode != NetmodeID.MultiplayerClient</c>, so the damage credit belongs to the creature and a cast is
/// never duplicated per client (D-55). No approved artwork exists for this attack, so it requests only the existing
/// shared fallback texture instead of creating placeholder art (D-48/D-51), and it carries no debuff because the
/// design names none for the spell.
/// </para>
/// </summary>
public class AnimatedWitherbarkSoldier_SpellBeam : ModProjectile
{
	/// <summary>
	/// 受重力影响的粒子: the drop per tick, in pixels per tick squared. Deliberately gentler than a rock so the
	/// three rays bend into a spray instead of falling at once (D-54 default, 04-DEVIATIONS.md section 10).
	/// </summary>
	private const float Gravity = 0.14f;

	/// <summary>Terminal fall speed, in pixels per tick.</summary>
	private const float MaxFallSpeed = 9f;

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

		// A particle ray bursts on the first surface it reaches, so its gravity arc is what carries it onto the
		// player rather than through the courtyard's walls.
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;

		// A short lifetime, because the three rays are a spray and not a lingering field.
		Projectile.timeLeft = 180;
	}

	public override void AI()
	{
		// 受重力影响的粒子: the ray arcs downward as it travels.
		Projectile.velocity.Y += Gravity;
		if (Projectile.velocity.Y > MaxFallSpeed)
		{
			Projectile.velocity.Y = MaxFallSpeed;
		}

		Projectile.rotation = Projectile.velocity.ToRotation();

		// Dust is client-only work: the dedicated server has no graphics access (D-35/D-55).
		if (!Main.dedServ && Main.rand.NextBool(3))
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<WitherWoodDust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1.1f);
		}
	}

	public override void OnKill(int timeLeft)
	{
		if (Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 6; i++)
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<WitherWoodDust>(), 0f, 0f);
			Main.dust[dust].velocity *= 0.5f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
		}
	}
}
