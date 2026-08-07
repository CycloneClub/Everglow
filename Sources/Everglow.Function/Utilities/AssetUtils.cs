namespace Everglow.Commons.Utilities;

public static class AssetUtils
{
	/// <summary>
	/// Load not-loaded textures for vanilla items
	/// </summary>
	/// <param name="types"></param>
	public static void LoadVanillaItemTextures(IEnumerable<int> types)
	{
		foreach (var type in types.Distinct().Where(t => t <= ItemID.Count))
		{
			// The Main.LoadItem function will skip the loaded items
			Main.instance.LoadItem(type);
		}
	}

	/// <summary>
	/// Load not-loaded textures for vanilla NPCs
	/// </summary>
	/// <param name="types"></param>
	public static void LoadVanillaNPCTextures(IEnumerable<int> types)
	{
		foreach (var type in types.Distinct().Where(t => t <= NPCID.Count))
		{
			// The Main.LoadItem function will skip the loaded items
			Main.instance.LoadNPC(type);
		}
	}
}
