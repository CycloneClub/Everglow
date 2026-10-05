namespace Everglow.Yggdrasil.KelpCurtain.NPCs;

/// <summary>
/// The Kelp Curtain's shared, server-safe spawn predicates together with the conservative spawn-weight
/// bands every Phase 4 creature reuses.
/// <para>
/// Every method here is a pure function of <see cref="NPCSpawnInfo"/> and world tile state: none of
/// them reads the local player, the screen position or any other client-only value, because
/// <c>ModNPC.SpawnChance</c> runs in single player or on the server only, where the client camera is
/// zero (D-52/D-55, QUAL-03).
/// </para>
/// <para>
/// The design keeps three distinct water conditions apart - 水面上 (the surface), 水底 (the bottom) and
/// land - so they are three distinct predicates here instead of one "in water" flag (Pitfall 6). All
/// three are deliberately derived from <see cref="NPCSpawnInfo"/> and the spawn tile's own liquid
/// state, so no predicate depends on terrain that Phases 5-6 have not built yet (D-53).
/// </para>
/// </summary>
public static class KelpCurtainSpawnConditions
{
	/// <summary>
	/// The conservative land-creature spawn weight (band 0.75f-1.5f), recorded as a D-54 default in
	/// 04-DEVIATIONS.md section 10.
	/// </summary>
	public const float LandWeight = 1f;

	/// <summary>
	/// The conservative water-creature spawn weight (band 0.5f-1f), recorded as a D-54 default in
	/// 04-DEVIATIONS.md section 10.
	/// </summary>
	public const float WaterWeight = 0.75f;

	/// <summary>
	/// The conservative rare / water-bottom spawn weight (band 0.25f-0.5f), recorded as a D-54 default
	/// in 04-DEVIATIONS.md section 10. The design makes the lake-bottom predators the rarest aquatic
	/// creatures (权重是全部水生生物最低).
	/// </summary>
	public const float RareWaterBottomWeight = 0.35f;

	/// <summary>
	/// The conservative mini-boss spawn weight (band at or below 0.1f), recorded as a D-54 default in
	/// 04-DEVIATIONS.md section 10.
	/// </summary>
	public const float MiniBossWeight = 0.1f;

	/// <summary>
	/// The bounded window, in tiles, of the downward floor probe used by <see cref="IsWaterBottom"/>. The
	/// design's 水底 creatures sit on a floor a handful of tiles beneath the spawn tile, so a small
	/// window keeps the probe cheap and cannot mistake an open water column for a lake bed.
	/// </summary>
	private const int MaxBottomScanTiles = 24;

	/// <summary>
	/// The land condition (D-53): true when the spawn is not a water spawn and the spawn tile itself
	/// holds no liquid. This is the 格普螺 (<c>GuppyConch</c>) land rejection generalised, so a land
	/// creature cannot appear in a submerged tile even when the engine reports
	/// <see cref="NPCSpawnInfo.Water"/> as false.
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context; only its player-independent fields are read.</param>
	/// <returns>True when the tile is dry land.</returns>
	public static bool IsDryLand(NPCSpawnInfo spawnInfo)
	{
		if (spawnInfo.Water)
		{
			return false;
		}

		int tileX = spawnInfo.SpawnTileX;
		int tileY = spawnInfo.SpawnTileY;
		if (!WorldGen.InWorld(tileX, tileY, 1))
		{
			// Outside the world: the engine's own boundary handles the spawn, so do not block it.
			return true;
		}

		return Main.tile[tileX, tileY].LiquidAmount <= 0;
	}

	/// <summary>
	/// The 水面上 condition (Pitfall 6): <see cref="NPCSpawnInfo"/> exposes no surface flag, so the
	/// surface is derived - the spawn must be a water spawn and the tile directly above it must be dry.
	/// A secondary cross-check against <c>DeathJadeLakeBiome.LiquidSurfaceY</c> is kept as a tuning note
	/// in 04-DEVIATIONS.md section 5 in case the dry-tile-above test proves too strict in the client run.
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>True when the spawn is in the water's top tile.</returns>
	public static bool IsWaterSurface(NPCSpawnInfo spawnInfo)
	{
		if (!spawnInfo.Water)
		{
			return false;
		}

		int tileX = spawnInfo.SpawnTileX;
		int tileY = spawnInfo.SpawnTileY - 1;
		if (!WorldGen.InWorld(tileX, tileY, 1))
		{
			return false;
		}

		return Main.tile[tileX, tileY].LiquidAmount <= 0;
	}

	/// <summary>
	/// The 水底 condition: the spawn must be a water spawn and the liquid column below the spawn tile
	/// must reach a solid floor inside the bounded probe window. This is what separates the design's
	/// bottom-dwellers (碧灵鮟鱇, 放射虫, 大型覆藻章鱼) from an ordinary "in water" spawn.
	/// </summary>
	/// <param name="spawnInfo">The engine's spawn context.</param>
	/// <returns>True when the spawn tile is a liquid tile whose column meets a solid floor below.</returns>
	public static bool IsWaterBottom(NPCSpawnInfo spawnInfo)
	{
		if (!spawnInfo.Water)
		{
			return false;
		}

		int tileX = spawnInfo.SpawnTileX;
		int tileY = spawnInfo.SpawnTileY;
		if (!WorldGen.InWorld(tileX, tileY, 1) || Main.tile[tileX, tileY].LiquidAmount <= 0)
		{
			return false;
		}

		for (int offset = 1; offset <= MaxBottomScanTiles; offset++)
		{
			int checkY = tileY + offset;
			if (!WorldGen.InWorld(tileX, checkY, 1))
			{
				return false;
			}

			Tile tile = Main.tile[tileX, checkY];
			if (tile.HasTile && tile.LiquidAmount <= 0)
			{
				// The liquid column reaches its solid floor: this is a lake-bed spawn.
				return true;
			}

			if (tile.LiquidAmount <= 0)
			{
				// A dry gap under the spawn tile: an ordinary water spawn, not a bottom one.
				return false;
			}
		}

		return false;
	}
}
