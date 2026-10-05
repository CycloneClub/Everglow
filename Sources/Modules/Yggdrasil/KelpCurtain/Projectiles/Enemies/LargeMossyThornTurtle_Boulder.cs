namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 大型荆棘苔龟's state-3 boulder rain: 从上方屏幕外均匀落下3颗巨石 - one gravity-affected, tile-colliding
/// hostile boulder, dropped from above as one of three evenly spaced columns.
/// Design row: 大型荆棘苔龟 (Large Mossy Thorn Turtle), behavior block TyxWdwlaMoE4mhxYWfEcM9gtnUg,
/// region 森雨幽谷 / Valley of Lush and Moist.
/// <para>
/// The boulders are spawned by <see cref="NPCs.ValleyOfLushAndMoist.LargeMossyThornTurtle"/> from
/// <c>NPC.GetSource_FromAI()</c> inside a <c>Main.netMode != NetmodeID.MultiplayerClient</c> guard, so
/// exactly the authoritative side creates them (D-55). The design's 巨石 damage (55 at the normal difficulty
/// scale) is passed in by the spawn call rather than being re-declared here.
/// </para>
/// <para>
/// This is a hostile enemy projectile and deliberately reuses no friendly, player-owned projectile class,
/// and it references no item type at all. The phase builds exactly eleven projectiles
/// (04-DEVIATIONS.md section 7) and this is item 11.
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the projectile requests the shared
/// Commons.ModAsset.White_Mod fallback. When the art arrives the D-49 migration is adding
/// LargeMossyThornTurtle_Boulder.png beside this .cs and deleting the Texture override (D-48/D-49).
/// </para>
/// </summary>
public class LargeMossyThornTurtle_Boulder : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	// Approved artwork is missing from the repository; reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	/// <summary>Gravity applied per tick, in pixels per tick squared.</summary>
	private const float Gravity = 0.35f;

	/// <summary>Terminal fall speed, in pixels per tick.</summary>
	private const float MaxFallSpeed = 16f;

	/// <summary>Sprite spin per tick, in radians.</summary>
	private const float SpinSpeed = 0.2f;

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 600;
	}

	/// <summary>
	/// The gravity arc of a spinning boulder, plus its client-only stone dust. The dust sits inside
	/// <c>!Main.dedServ</c> so no graphics call can run on a dedicated server (D-35/D-55).
	/// </summary>
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

	/// <summary>
	/// The impact burst. Client-only work, so the whole body sits behind the dedicated-server guard
	/// (D-35/D-55).
	/// </summary>
	/// <param name="timeLeft">The remaining lifetime at despawn.</param>
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
