using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 剧毒蟾蜍's 剧毒云（poison cloud）- the small cloud its death explosion leaves behind
/// (behavior block GjwedzoyPouXssx66zpcBbo5nWd: 死亡后爆炸，产生一小团剧毒云，持续3秒，接触后造成10伤害且造成
/// 5秒酸性毒液).
/// <para>
/// Spawned by <c>ToxicToad.HitEffect</c> from <c>NPC.GetSource_FromAI()</c> inside
/// <c>Main.netMode != NetmodeID.MultiplayerClient</c>, so one cloud appears per death on the authoritative
/// side and is never duplicated per client (D-55). No approved artwork exists for this attack, so it
/// requests only the existing shared fallback texture instead of creating placeholder art (D-48/D-51).
/// </para>
/// </summary>
public class ToxicToad_PoisonCloud : ModProjectile
{
	/// <summary>持续3秒: the cloud's lifetime, in frames (3 s * 60).</summary>
	private const int Lifetime = 180;

	/// <summary>接触后造成10伤害: the contact damage, matching the value the toad passes when it spawns the cloud.</summary>
	private const int ContactDamage = 10;

	/// <summary>造成5秒酸性毒液: 5 s = 300 ticks of the mapped <c>BuffID.Venom</c>.</summary>
	private const int VenomTicks = 300;

	/// <summary>
	/// The cloud's lifetime counter, in frames of age. It mirrors the engine's own 180-frame
	/// <c>Projectile.timeLeft</c> budget but is carried in the synced <c>ai[]</c> array through this
	/// named wrapper, so the cloud's own fade reads a named value instead of the engine field.
	/// </summary>
	private int LifetimeCounter
	{
		get => (int)Projectile.ai[0];
		set => Projectile.ai[0] = value;
	}

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	// Approved artwork is missing from the repository (D-48); reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		// 一小团: a small cloud, close to two tiles across.
		Projectile.width = 32;
		Projectile.height = 32;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;

		// A lingering cloud that keeps damaging anything standing in it, and never moves.
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;

		Projectile.damage = ContactDamage;
		Projectile.knockBack = 0f;
		Projectile.timeLeft = Lifetime;
		LifetimeCounter = 0;
	}

	public override void AI()
	{
		// Stationary: 产生一小团剧毒云, it hangs where the creature died.
		Projectile.velocity = Vector2.Zero;
		LifetimeCounter++;

		if (!Main.dedServ)
		{
			UpdateVisual();
		}
	}

	/// <summary>
	/// The cloud's own puffing. Client-only work, so it sits behind the dedicated-server guard
	/// (D-35/D-55) and reuses an existing Kelp Curtain dust rather than creating a new dust class (D-51).
	/// </summary>
	private void UpdateVisual()
	{
		// 一小团剧毒云 slowly thins out as its 3 seconds run down, so the cloud reads its own fade from
		// the named lifetime counter.
		float remaining = MathHelper.Clamp(1f - LifetimeCounter / (float)Lifetime, 0f, 1f);
		Projectile.scale = MathHelper.Lerp(0.6f, 1f, remaining);

		int puffs = 1 + (int)(remaining * 2f);
		for (int i = 0; i < puffs; i++)
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<JadeLakeGreenAlgaeDust>(), 0f, -0.3f);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.4f) * Projectile.scale;
		}
	}

	/// <summary>
	/// 接触后造成10伤害且造成5秒酸性毒液: the contact damage is the projectile's own 10, and the debuff is 5 s
	/// (300 ticks). 酸性毒液 maps to the vanilla <c>BuffID.Venom</c> because <c>BuffID.AcidVenom</c> does not
	/// exist in this tML build (04-DEVIATIONS.md section 10). tML documents this hook as running on the
	/// local client, so it is deliberately not wrapped in a multiplayer-client guard.
	/// </summary>
	/// <param name="target">The player standing in the cloud.</param>
	/// <param name="info">The resolved hit.</param>
	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(BuffID.Venom, VenomTicks);
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

		for (int i = 0; i < 10; i++)
		{
			int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<JadeLakeGreenAlgaeDust>(), 0f, -0.3f);
			Main.dust[dust].velocity *= 0.4f;
			Main.dust[dust].noGravity = true;
			Main.dust[dust].scale = Main.rand.NextFloat(0.8f, 1.5f);
		}
	}
}
