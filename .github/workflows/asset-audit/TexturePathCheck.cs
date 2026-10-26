using System;
using System.Collections.Generic;

namespace CiAssetAudit;

public enum TexturePathResult
{
	Present,
	Vanilla,
	Missing,
	Invalid,
}

public static class TexturePathCheck
{
	public static TexturePathResult Check(string path, IReadOnlyDictionary<string, HashSet<string>> inventories)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return TexturePathResult.Invalid;
		}
		// Match tML's resource separators while retaining case-sensitive package lookup.
		int separator = path.IndexOfAny(new[] { '/', ' ', ':' });
		if (separator < 1 || separator == path.Length - 1)
		{
			return TexturePathResult.Invalid;
		}
		string modName = path.Substring(0, separator);
		string assetName = path.Substring(separator + 1).Replace('\\', '/');
		// Vanilla client assets are not shipped in the dedicated-server distribution.
		if (modName == "Terraria")
		{
			return TexturePathResult.Vanilla;
		}
		return inventories.TryGetValue(modName, out var files) &&
			(files.Contains(assetName + ".png") || files.Contains(assetName + ".rawimg") || files.Contains(assetName + ".xnb"))
			? TexturePathResult.Present
			: TexturePathResult.Missing;
	}
}
