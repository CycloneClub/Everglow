namespace Everglow.Commons.Utilities;

public static class AssetUtils
{
	/// <summary>
	/// Load a vanilla item texture when graphics are available.
	/// </summary>
	/// <param name="type">The item type.</param>
	public static void LoadVanillaItemTexture(int type)
	{
		if (Main.dedServ || Main.instance is null || type < 0 || type >= ItemID.Count)
		{
			return;
		}

		Main.instance.LoadItem(type);
	}

	/// <summary>
	/// Load not-loaded textures for vanilla items
	/// </summary>
	/// <param name="types"></param>
	public static void LoadVanillaItemTextures(IEnumerable<int> types)
	{
		ArgumentNullException.ThrowIfNull(types);

		foreach (var type in types)
		{
			LoadVanillaItemTexture(type);
		}
	}

	/// <summary>
	/// Load a vanilla NPC texture when graphics are available.
	/// </summary>
	/// <param name="type">The NPC type.</param>
	public static void LoadVanillaNPCTexture(int type)
	{
		if (Main.dedServ || Main.instance is null || type < 0 || type >= NPCID.Count)
		{
			return;
		}

		Main.instance.LoadNPC(type);
	}

	/// <summary>
	/// Load not-loaded textures for vanilla NPCs
	/// </summary>
	/// <param name="types"></param>
	public static void LoadVanillaNPCTextures(IEnumerable<int> types)
	{
		ArgumentNullException.ThrowIfNull(types);

		foreach (var type in types)
		{
			LoadVanillaNPCTexture(type);
		}
	}
}
