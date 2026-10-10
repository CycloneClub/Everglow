using Everglow.Commons.Mechanics.Cooldown;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Localization;

namespace Everglow.Commons.Localization;

public class OutputLocalizationHjsonItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override string Texture => Commons.ModAsset.Point_Mod;

	private bool canUse = true;

	public override void SetDefaults()
	{
		Item.width = 90;
		Item.height = 20;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useTime = 10;
		Item.useAnimation = 10;

		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 20;
	}

	public override void HoldItem(Player player)
	{
		if (Main.myPlayer != player.whoAmI)
		{
			return;
		}

		if (canUse)
		{
			Main.instance.MouseText("可用");
		}

		if (PlayerInput.Triggers.JustPressed.MouseMiddle)
		{
			canUse = true;
		}
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (canUse)
		{
			canUse = false;

			string[] cultures = [
				GameCulture.FromCultureName(GameCulture.CultureName.Chinese).Name,
				GameCulture.FromCultureName(GameCulture.CultureName.English).Name
			];

			var items = GetItems();
			var npcs = GetNPCs();
			var biomes = GetModBiomes();
			var buffs = GetBuffs();
			var cooldowns = GetCooldowns();
			var projectiles = GetProjectiles();

			foreach (var cultureName in cultures)
			{
				ExportHjson.ExportCategoryFiles(items, cultureName);
				ExportHjson.ExportCategoryFiles(npcs, cultureName);
				ExportHjson.ExportCategoryFiles(biomes, cultureName);
				ExportHjson.ExportCategoryFiles(buffs, cultureName);
				ExportHjson.ExportCategoryFiles(cooldowns, cultureName);
				ExportHjson.ExportCategoryFiles(projectiles, cultureName);
			}
		}

		return false;
	}

	public Dictionary<string, List<string>> GetItems()
	{
		var itemNames = ItemID.Search.Names.Where(x => x.StartsWith(nameof(Everglow)));
		var number = itemNames.Count();
		int correct = 0;
		int wrong = 0;

		Dictionary<string, List<string>> keyToName = [];
		foreach (var itemName in itemNames)
		{
			var id = ItemID.Search.GetId(itemName);
			if (id == ItemID.None)
			{
				continue;
			}

			var item = new Item();
			item.SetDefaults(id);

			var category = item.ModItem.LocalizationCategory;
			if (category == "Items")
			{
				wrong++;
				Console.WriteLine("Item 未分类：" + itemName);
			}
			else
			{
				correct++;
				if (keyToName.TryGetValue(category, out var list))
				{
					list.Add(itemName);
				}
				else
				{
					keyToName[category] = [itemName];
				}
			}
		}

		Console.WriteLine($"ModItem: {correct}√ / {wrong}×");

		return keyToName;
	}

	public Dictionary<string, List<string>> GetNPCs()
	{
		var npcNames = NPCID.Search.Names.Where(x => x.StartsWith(nameof(Everglow)));
		var number = npcNames.Count();

		Dictionary<string, List<string>> keyToName = [];
		foreach (var npcName in npcNames)
		{
			var id = NPCID.Search.GetId(npcName);
			if (id == NPCID.None)
			{
				continue;
			}

			var npc = new NPC();
			npc.SetDefaults(id);

			var category = npc.ModNPC.LocalizationCategory;
			if (keyToName.TryGetValue(category, out var list))
			{
				list.Add(npcName);
			}
			else
			{
				keyToName.Add(category, [npcName]);
			}
		}

		return keyToName;
	}

	public Dictionary<string, List<string>> GetModBiomes()
	{
		var key = "Biomes";
		Dictionary<string, List<string>> keyToName = [];
		keyToName[key] = [];
		foreach (var biome in Ins.ModuleManager.Types.Where(x => !x.IsAbstract && x.IsAssignableTo(typeof(ModBiome))))
		{
			keyToName[key].Add(biome.Name);
		}

		return keyToName;
	}

	public Dictionary<string, List<string>> GetBuffs()
	{
		Dictionary<string, List<string>> keyToName = [];
		var buffNames = BuffID.Search.Names.Where(x => x.StartsWith(nameof(Everglow))).ToList();
		keyToName["Buffs"] = buffNames;
		return keyToName;
	}

	public Dictionary<string, List<string>> GetCooldowns()
	{
		var key = "Cooldowns";
		Dictionary<string, List<string>> keyToName = [];
		keyToName[key] = [];
		foreach (var cooldown in Ins.ModuleManager.Types.Where(x => !x.IsAbstract && x.IsAssignableTo(typeof(CooldownBase))))
		{
			keyToName[key].Add(cooldown.Name);
		}

		return keyToName;
	}

	public Dictionary<string, List<string>> GetProjectiles()
	{
		var itemNames = ProjectileID.Search.Names.Where(x => x.StartsWith(nameof(Everglow)));
		var number = itemNames.Count();
		int correct = 0;
		int wrong = 0;

		Dictionary<string, List<string>> keyToName = [];
		foreach (var itemName in itemNames)
		{
			var id = ProjectileID.Search.GetId(itemName);
			if (id == ProjectileID.None)
			{
				continue;
			}

			var item = new Projectile();
			item.SetDefaults(id);

			var category = item.ModProjectile.LocalizationCategory;
			if (category == "Projectiles")
			{
				wrong++;
				Console.WriteLine("Projectile 未分类：" + itemName);
			}
			else
			{
				correct++;
				if (keyToName.TryGetValue(category, out var list))
				{
					list.Add(itemName);
				}
				else
				{
					keyToName[category] = [itemName];
				}
			}
		}

		Console.WriteLine($"ModProjectile: {correct}√ / {wrong}×");

		return keyToName;
	}
}
