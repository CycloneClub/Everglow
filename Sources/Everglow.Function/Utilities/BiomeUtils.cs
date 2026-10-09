namespace Everglow.Commons.Utilities;

public static class BiomeUtils
{
	/// <summary>
	/// Checks if the player's position is under <see cref="Main.worldSurface"/>.
	/// <para/>
	/// </summary>
	/// <param name="player"></param>
	/// <param name="baseBiomeCheck">Additional biome condition that must be true for the depth check to succeed.</param>
	/// <returns></returns>
	public static bool InCavernBiome(this Player player, bool baseBiomeCheck = true) =>
		player.Center.ToTileCoordinates().Y > Main.rockLayer && baseBiomeCheck;

	/// <summary>
	/// Checks if the player's position is above <see cref="Main.worldSurface"/>.
	/// </summary>
	/// <param name="player"></param>
	/// <param name="baseBiomeCheck">Additional biome condition that must be true for the depth check to succeed.</param>
	/// <returns></returns>
	public static bool InSurfaceAndUndergroundBiome(this Player player, bool baseBiomeCheck = true) =>
		!player.InCavernBiome() && baseBiomeCheck;
}
