namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

/// <summary>
/// 巨树人's ground smash: a hostile wave that travels horizontally along the floor away from the
/// creature's feet for a short lifetime, with the design's roughly 2-tile-high, large-radius hit
/// box (behaviour block Zm3ZdguPboviV1xCmcmcSIEznZd).
/// No approved artwork exists for this attack, so it requests only the existing shared fallback
/// texture instead of creating placeholder art (03-DEVIATIONS.md section 9).
/// </summary>
public class GiantDandelion_Shockwave : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	// Approved artwork is missing from the repository; reuse the existing shared fallback texture
	// rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	/// <summary>The wave's lifetime, in ticks (conservative default, D-34).</summary>
	public const int Lifetime = 40;

	public override void SetDefaults()
	{
		// ~2 tiles high and a large radius wide, matching the design's wording for the smash wave.
		Projectile.width = 200;
		Projectile.height = 32;

		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = Lifetime;
	}

	public override void AI()
	{
		// Ground-hugging: the wave keeps its horizontal speed while it is anchored to a floor.
		Projectile.velocity.Y = 0f;

		int floorTileY = FindFloorTileY();

		if (floorTileY >= 0)
		{
			// A floor tile sits just below the wave: anchor it to that tile's top edge so it reads
			// as a ground shockwave rather than a floating bar.
			Projectile.position.Y = floorTileY * 16f - Projectile.height;
		}
		else
		{
			// Nothing under the wave yet: sink until it finds the ground.
			Projectile.velocity.Y = 8f;
		}

		if (!Main.dedServ)
		{
			for (int i = 0; i < 2; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, -Projectile.velocity.X * 0.2f, -1f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].scale = Main.rand.NextFloat(0.9f, 1.4f);
			}
		}
	}

	/// <summary>
	/// The floor tile just below the 200 px-wide wave, or <c>-1</c> when no sampled column has a
	/// floor there. The leading edge, the centre and the trailing edge are all sampled and the
	/// deepest surface (largest tile Y) wins, so an isolated hole under one column does not drop
	/// the whole hit box and a higher terrace caught by the centre does not lift it off the ground
	/// its other half is over. While the wave is still falling, no column has a floor at its
	/// current height and the probe returns <c>-1</c>, so the wave keeps sinking until it lands.
	/// </summary>
	private int FindFloorTileY()
	{
		int probeTileY = (int)((Projectile.Bottom.Y + 8f) / 16f);
		int leadingTileX = (int)((Projectile.velocity.X >= 0f ? Projectile.Right.X : Projectile.Left.X) / 16f);
		int centerTileX = (int)(Projectile.Center.X / 16f);
		int trailingTileX = (int)((Projectile.velocity.X >= 0f ? Projectile.Left.X : Projectile.Right.X) / 16f);

		int deepestTileY = -1;
		deepestTileY = Math.Max(deepestTileY, ProbeFloorTileY(leadingTileX, probeTileY));
		deepestTileY = Math.Max(deepestTileY, ProbeFloorTileY(centerTileX, probeTileY));
		deepestTileY = Math.Max(deepestTileY, ProbeFloorTileY(trailingTileX, probeTileY));
		return deepestTileY;
	}

	/// <summary>Returns <paramref name="tileY"/> when that tile is in the world and solid, else <c>-1</c>.</summary>
	private static int ProbeFloorTileY(int tileX, int tileY)
	{
		if (WorldGen.InWorld(tileX, tileY, 1) && Main.tile[tileX, tileY].HasTile)
		{
			return tileY;
		}

		return -1;
	}
}
