namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 大型荆棘苔龟's state-2 and state-3 floor slam: 震击地板，产生范围的震荡波 - an area shockwave that expands
/// out of the impact point along the floor.
/// Design row: 大型荆棘苔龟 (Large Mossy Thorn Turtle), behavior block TyxWdwlaMoE4mhxYWfEcM9gtnUg,
/// region 森雨幽谷 / Valley of Lush and Moist.
/// <para>
/// The wave is spawned by <see cref="NPCs.ValleyOfLushAndMoist.LargeMossyThornTurtle"/> from
/// <c>NPC.GetSource_FromAI()</c> inside a <c>Main.netMode != NetmodeID.MultiplayerClient</c> guard, so
/// exactly the authoritative side creates it (D-55). The design's 震荡波 damage (70 at the normal
/// difficulty scale) is passed in by the spawn call rather than being re-declared here.
/// </para>
/// <para>
/// This is a hostile enemy projectile and deliberately does <b>not</b> reuse a friendly player-owned
/// projectile: the phase builds exactly eleven projectiles (04-DEVIATIONS.md section 7) and this is item 10.
/// </para>
/// <para>
/// Approved artwork (贴图) is missing from the repository, so the projectile requests the shared
/// Commons.ModAsset.White_Mod fallback. When the art arrives the D-49 migration is adding
/// LargeMossyThornTurtle_Shockwave.png beside this .cs and deleting the Texture override (D-48/D-49).
/// </para>
/// </summary>
public class LargeMossyThornTurtle_Shockwave : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	// Approved artwork is missing from the repository; reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	/// <summary>The pulse's lifetime, in ticks.</summary>
	private const int Lifetime = 40;

	/// <summary>The pulse's hit box at the moment of impact, in pixels.</summary>
	private const int InitialWidth = 40;

	/// <summary>The pulse's initial hit-box height, in pixels (roughly 1.5 tiles of floor-hugging wave).</summary>
	private const int InitialHeight = 24;

	/// <summary>
	/// 范围的震荡波: the pulse's fully expanded width, in pixels (20 tiles). The design gives no radius, so a
	/// conservative arena-wide value is used and recorded as a D-54 default.
	/// </summary>
	private const int MaxWidth = 320;

	/// <summary>The pulse's fully expanded height, in pixels (3 tiles), so the wave can catch a jumping player.</summary>
	private const int MaxHeight = 48;

	/// <summary>
	/// The bounded downward window, in tiles, of the floor probe. A slam always happens on a floor, so a
	/// small window keeps the probe cheap while still tolerating a spawn a few pixels above the surface.
	/// </summary>
	private const int MaxFloorProbeTiles = 24;

	/// <summary>The sink speed, in pixels per tick, used only while no floor tile is under the pulse yet.</summary>
	private const float FloorSinkSpeed = 8f;

	public override void SetDefaults()
	{
		Projectile.width = InitialWidth;
		Projectile.height = InitialHeight;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = Lifetime;

		// The pulse is anchored on its first update, when Projectile.Center is already the spawn position;
		// the sentinel keeps it inert until then.
		Projectile.ai[0] = 0f;
	}

	/// <summary>
	/// The expanding floor pulse. It has no horizontal velocity, so the first tick's centre is the impact
	/// point; from there the hit box grows symmetrically and the pulse stays anchored to the floor tile under
	/// that point. Reads only world tile state - never <c>Main.screenPosition</c>, never the local player
	/// (D-52/D-55).
	/// </summary>
	public override void AI()
	{
		if (Projectile.ai[0] <= 0f)
		{
			// The impact point, captured before the pulse has moved.
			Projectile.ai[0] = Projectile.Center.X;
		}

		float anchorX = Projectile.ai[0];

		// 范围的震荡波: the wave expands as it travels outward from the impact point.
		float progress = MathHelper.Clamp((Lifetime - Projectile.timeLeft) / (float)Lifetime, 0f, 1f);
		Projectile.width = (int)MathHelper.Lerp(InitialWidth, MaxWidth, progress);
		Projectile.height = (int)MathHelper.Lerp(InitialHeight, MaxHeight, progress);

		int floorTileY = FindFloorTileY(anchorX);
		if (floorTileY >= 0)
		{
			// A floor tile sits below the pulse: anchor it to that tile's top edge so the wave reads as a
			// ground shockwave rather than a floating bar.
			Projectile.position.Y = floorTileY * 16f - Projectile.height;
		}
		else
		{
			// Nothing under the pulse yet: sink until it finds the ground.
			Projectile.position.Y += FloorSinkSpeed;
		}

		// Keep the growing hit box centred on the impact point rather than letting it drift right.
		Projectile.position.X = anchorX - Projectile.width / 2f;

		if (!Main.dedServ)
		{
			for (int i = 0; i < 2; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, 0f, -1f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].scale = Main.rand.NextFloat(0.9f, 1.4f);
			}
		}
	}

	/// <summary>
	/// The solid floor tile just below the pulse's anchor column inside the bounded probe window, or
	/// <c>-1</c> when no floor is found there yet. While the pulse is still sinking, no tile sits at its
	/// current height and the probe returns <c>-1</c>, so it keeps sinking until it lands.
	/// </summary>
	/// <param name="anchorX">The impact point's world X.</param>
	/// <returns>The floor tile row, or <c>-1</c>.</returns>
	private int FindFloorTileY(float anchorX)
	{
		int probeTileX = (int)(anchorX / 16f);
		int startTileY = (int)((Projectile.Bottom.Y + 8f) / 16f);

		for (int offset = 0; offset <= MaxFloorProbeTiles; offset++)
		{
			int tileY = startTileY + offset;
			if (!WorldGen.InWorld(probeTileX, tileY, 1))
			{
				return -1;
			}

			if (Main.tile[probeTileX, tileY].HasTile)
			{
				return tileY;
			}
		}

		return -1;
	}
}
