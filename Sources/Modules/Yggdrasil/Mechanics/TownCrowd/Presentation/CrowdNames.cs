namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>One stable localized identity per randomly seeded journey.</summary>
public static class CrowdNames
{
	private static readonly string[] Pool =
	[
		"Alden", "Mira", "Rowan", "Lyra", "Orin", "Elara", "Finn", "Nia",
		"Silas", "Iris", "Theo", "Luna", "Arlo", "Cora", "Jasper", "Vera",
		"Emil", "Faye", "Noel", "Sora", "Ren", "Yuna", "Lian", "Mei",
		"Kai", "Asha", "Tarin", "Selene", "Bram", "Tessa", "Eira", "Ravi",
	];

	public static string KeyFor(int seed) => $"Mods.Everglow.Travelers.Names.{Pool[(uint)seed % Pool.Length]}";
}
