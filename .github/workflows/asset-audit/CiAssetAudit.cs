using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace CiAssetAudit;

// Built only into the isolated CI save directory, never into Everglow.tmod.
public sealed class CiAssetAudit : Mod
{
	public override void PostSetupContent()
	{
		var target = ModLoader.GetMod("Everglow");
		var inventories = ModLoader.Mods.ToDictionary(mod => mod.Name,
			mod => new HashSet<string>(mod.GetFileNames() ?? new List<string>(), StringComparer.Ordinal),
			StringComparer.Ordinal);
		var failures = new List<string>();
		int contentCount = 0;
		int equipmentCount = 0;
		int vanillaCount = 0;

		void Check(string owner, string path)
		{
			switch (TexturePathCheck.Check(path, inventories))
			{
				case TexturePathResult.Vanilla:
					vanillaCount++;
					break;
				case TexturePathResult.Invalid:
					failures.Add($"{owner}: invalid texture path '{path}'");
					break;
				case TexturePathResult.Missing:
					failures.Add($"{owner}: missing texture '{path}' (.png/.rawimg/.xnb) in the loaded mod package");
					break;
			}
		}

		// Use loaded instances: respects IsLoadingEnabled, Texture overrides and module discovery.
		foreach (var content in target.GetContent())
		{
			if (content is not (ModItem or ModNPC or ModProjectile or ModTexturedType))
			{
				continue;
			}
			string texture = content switch
			{
				ModItem item => item.Texture,
				ModNPC npc => npc.Texture,
				ModProjectile projectile => projectile.Texture,
				ModTexturedType textured => textured.Texture,
				_ => null,
			};
			contentCount++;
			Check(content.GetType().FullName, texture);
		}

		// Includes both AutoloadEquip and explicitly registered equipment, with their actual paths.
		// Fail closed if tML changes this internal registry instead of silently skipping armor.
		var field = typeof(Mod).GetField("equipTextures", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field?.GetValue(target) is not IDictionary<Tuple<string, EquipType>, EquipTexture> equipment)
		{
			throw new InvalidOperationException("tML equipment registry changed; update the CI asset audit.");
		}
		foreach (var entry in equipment.Values)
		{
			equipmentCount++;
			Check($"{target.Name}/{entry.Name} ({entry.Type})", entry.Texture);
		}
		Logger.Info($"Asset audit checked {contentCount} content textures and {equipmentCount} equipment textures; skipped {vanillaCount} vanilla references.");
		foreach (string failure in failures)
		{
			Logger.Error(failure);
		}
		if (failures.Count != 0 || contentCount == 0 || equipmentCount == 0)
		{
			throw new InvalidOperationException($"Asset audit failed: {failures.Count} missing/invalid textures; content={contentCount}; equipment={equipmentCount}.");
		}
		Logger.Info($"Asset audit passed: Everglow; content={contentCount}; equipment={equipmentCount}.");
	}
}
