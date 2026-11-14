using ReLogic.Content;
using Terraria.GameContent;

namespace Everglow.Commons;

/// <summary>
/// 通过GlobalItem实现自动加载Glow
/// </summary>
public class ItemGlowManager : GlobalItem
{
	private static short begin = 0;

	private static Dictionary<int, short> glowMapping = new();

	private static List<Asset<Texture2D>> glowMasks = new();

	public override void Load()
	{
		ModIns.OnPostSetupContent += LoadGlowMasks;
	}

	public static void LoadGlowMasks()
	{
		begin = (short)TextureAssets.GlowMask.Length;

		// 只读，线程安全
		var asset = ModIns.Mod.Assets;
		var source = ModIns.Mod.RootContentSource;
		short index = 0;
		foreach (var item in ModIns.Mod.GetContent<ModItem>())
		{
			var itemLocation = item.Texture.AsSpan(9);
			var path = string.Concat(itemLocation, "_Glow");
			if (source.HasAsset(path))
			{
				glowMasks.Add(asset.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad));
				glowMapping.Add(item.Type, (short)(begin + index));
				index++;
				continue;
			}

			var moduleName = itemLocation[..(itemLocation.IndexOf('/') + 1)];
			path = string.Concat(moduleName, "Glows/", item.Name, "_Glow");
			if (source.HasAsset(path))
			{
				glowMasks.Add(asset.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad));
				glowMapping.Add(item.Type, (short)(begin + index));
				index++;
				continue;
			}
		}
		TextureAssets.GlowMask = TextureAssets.GlowMask.Concat(glowMasks).ToArray();
	}

	public override void SetDefaults(Item item)
	{
		if (glowMapping.TryGetValue(item.type, out var index))
		{
			item.glowMask = index;
		}
	}

	public override void Unload()
	{
		ModIns.OnPostSetupContent -= LoadGlowMasks;
		TextureAssets.GlowMask = TextureAssets.GlowMask[..begin].Concat(TextureAssets.GlowMask[(begin + glowMasks.Count)..]).ToArray();
		glowMapping.Clear();
		glowMasks.Clear();
	}
}
