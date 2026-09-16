using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 剧毒蟾蜍's 毒泡泡（poison bubble）- the amphibious ambusher's only attack
/// (behavior block GjwedzoyPouXssx66zpcBbo5nWd: 通过发射毒泡泡攻击).
/// <para>
/// Spawned by <c>ToxicToad</c> from <c>NPC.GetSource_FromAI()</c> inside
/// <c>Main.netMode != NetmodeID.MultiplayerClient</c>, so the damage credit belongs to the creature and
/// the bubble is never duplicated per client (D-55). No approved artwork exists for this attack, so it
/// requests only the existing shared fallback texture instead of creating placeholder art (D-48/D-51).
/// </para>
/// </summary>
public class ToxicToad_PoisonBubble : ModProjectile
{
	/// <summary>毒泡泡: a light bubble rather than a bullet, so its drop is gentle (D-54 default).</summary>
	private const float Gravity = 0.06f;

	/// <summary>The bubble's terminal fall speed, in pixels per tick (D-54 default).</summary>
	private const float MaxFallSpeed = 5f;

	/// <summary>
	/// 不论何种方式都会有75%概率造成10秒中毒: 10 s = 600 ticks. The design's 75% branch is
	/// <c>Main.rand.NextBool(3)</c> (3 in 4).
	/// </summary>
	private const int PoisonTicks = 600;

	/// <summary>剩下25%概率造成7秒酸性毒液: 7 s = 420 ticks of the mapped <c>BuffID.Venom</c>.</summary>
	private const int VenomTicks = 420;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	// Approved artwork is missing from the repository (D-48); reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 18;
		Projectile.height = 18;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = 1;

		// 毒泡泡 does not bounce off terrain: it drifts along the attack line and bursts on the first
		// player it reaches (the design names no terrain interaction).
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.timeLeft = 300;
	}

	public override void AI()
	{
		// A light bubble: a gentle drop with a capped fall speed.
		Projectile.velocity.Y += Gravity;
		if (Projectile.velocity.Y > MaxFallSpeed)
		{
			Projectile.velocity.Y = MaxFallSpeed;
		}

		Projectile.rotation = Projectile.velocity.ToRotation();

		if (!Main.dedServ && Main.rand.NextBool(4))
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<JadeLakeGreenAlgaeDust>(), 0f, 0f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.6f, 1f);
		}
	}

	/// <summary>
	/// 不论何种方式都会有75%概率造成10秒中毒，剩下25%概率造成7秒酸性毒液: the bubble carries the same split as
	/// the creature's own contact damage. 酸性毒液 maps to the vanilla <c>BuffID.Venom</c>
	/// (04-DEVIATIONS.md section 10). tML documents this hook as running on the local client, so it is
	/// deliberately not wrapped in a multiplayer-client guard.
	/// </summary>
	/// <param name="target">The player the bubble burst on.</param>
	/// <param name="info">The resolved hit.</param>
	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if (Main.rand.NextBool(3))
		{
			// 75% -> 10 s (600 ticks) of 中毒.
			target.AddBuff(BuffID.Poisoned, PoisonTicks);
		}
		else
		{
			// the remaining 25% -> 7 s (420 ticks) of 酸性毒液.
			target.AddBuff(BuffID.Venom, VenomTicks);
		}
	}

	/// <summary>
	/// The bubble bursting. Client-only work, so it sits behind the dedicated-server guard (D-35/D-55)
	/// and reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
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
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<JadeLakeGreenAlgaeDust>(), 0f, 0f);
			Main.dust[dust].velocity *= 0.5f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.1f);
		}
	}
}
