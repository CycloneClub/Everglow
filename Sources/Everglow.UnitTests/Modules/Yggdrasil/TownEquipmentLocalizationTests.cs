using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Hjson;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Valiant;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;
using Everglow.Yggdrasil.YggdrasilTown.Items.PermanentBoosters;
using Everglow.Yggdrasil.YggdrasilTown.Items.Potions;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TownEquipmentLocalizationTests
{
	private LanguageManager originalLanguage = null!;

	private sealed class TooltipMod : Mod
	{
		public override string Name => "Everglow";
	}

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalLanguage = LanguageManager.Instance;
		LanguageManager.Instance = (LanguageManager)Activator.CreateInstance(typeof(LanguageManager), true)!;
	}

	[TestCleanup]
	public void Cleanup() => LanguageManager.Instance = originalLanguage;

	[TestMethod]
	[DataRow("zh-Hans")]
	[DataRow("en-US")]
	public void EveryTownArmorAndAccessoryHasAFormattedTooltip(string culture)
	{
		var root = FindRepositoryRoot();
		var texts = LoadLocalization(culture, "Items.Armor", "Items.Accessories", "Items.Accessories.Wings");

		// Resolve only equipment types; scanning the whole assembly loads unrelated optional mod dependencies.
		var itemRoot = Path.Combine(root.FullName, "Sources", "Modules", "Yggdrasil", "YggdrasilTown", "Items");
		var types = new[] { "Armors", "Accessories" }
			.SelectMany(folder => Directory.EnumerateFiles(Path.Combine(itemRoot, folder), "*.cs", SearchOption.AllDirectories))
			.Select(path =>
			{
				string ns = Regex.Match(File.ReadAllText(path), @"namespace\s+([\w.]+)").Groups[1].Value;
				return typeof(RecoveryBand).Assembly.GetType(ns + "." + Path.GetFileNameWithoutExtension(path), true)!;
			})
			.Where(type => !type.IsAbstract && typeof(ModItem).IsAssignableFrom(type)).ToArray();
		Assert.IsTrue(types.Length > 0);
		var mod = (TooltipMod)RuntimeHelpers.GetUninitializedObject(typeof(TooltipMod));
		foreach (var type in types)
		{
			var item = (ModItem)RuntimeHelpers.GetUninitializedObject(type);
			typeof(ModType).GetProperty("Mod")!.SetValue(item, mod);
			typeof(ModType<Item>).GetProperty("Entity")!.SetValue(item, RuntimeHelpers.GetUninitializedObject(typeof(Item)));
			string key = item.GetLocalizationKey("Tooltip");
			Assert.IsTrue(texts.ContainsKey(key), $"Missing {culture}: {key}");
			AssertFormatted(item.Tooltip, culture);
			if (type.GetMethod(nameof(ModItem.UpdateArmorSet))!.DeclaringType == type)
			{
				var property = type.GetProperty("SetBonusText");
				Assert.IsNotNull(property, $"Missing set bonus localization: {type.Name}");
				AssertFormatted((LocalizedText)property.GetValue(item)!, culture);
			}
			if (item is ValiantHelmet)
			{
				var player = new Player { endurance = 0.2f, aggro = 500 };
				item.UpdateArmorSet(player);
				Assert.AreEqual(0.24f, player.endurance, 0.00001f);
				Assert.AreEqual(100, player.aggro);
				StringAssert.Contains(player.setBonus, "+4%");
			}
			if (item is EldamberHairpin)
			{
				var player = new Player { manaCost = 0.8f };
				item.UpdateArmorSet(player);
				Assert.AreEqual(0.68f, player.manaCost, 0.00001f);
				Assert.IsFalse(string.IsNullOrWhiteSpace(player.setBonus));
			}
			if (item is EldamberWarhelm)
			{
				var player = new Player { statManaMax2 = 80, manaCost = 0.8f };
				player.GetAttackSpeed(DamageClass.Melee) = 1.2f;
				item.UpdateEquip(player);
				new EldamberBreastplate().UpdateEquip(player);
				new EldamberBoots().UpdateEquip(player);
				item.UpdateArmorSet(player);
				Assert.AreEqual(108f, player.GetTotalDamage(DamageClass.Melee).ApplyTo(100f), 0.00001f);
				Assert.AreEqual(104f, player.GetTotalDamage(DamageClass.Magic).ApplyTo(100f), 0.00001f);
				Assert.AreEqual(1.35f, player.GetAttackSpeed(DamageClass.Melee), 0.00001f);
				Assert.AreEqual(80, player.statManaMax2);
				Assert.AreEqual(0.8f, player.manaCost);
				Assert.IsFalse(string.IsNullOrWhiteSpace(player.setBonus));
			}
		}
	}

	[TestMethod]
	[DataRow("zh-Hans")]
	[DataRow("en-US")]
	public void ManaBoostersAndScalePotionHaveRuntimeLocalization(string culture)
	{
		var texts = LoadLocalization(culture, "Items.PermanentBoosters", "Items.Potions", "Buffs");
		foreach (var type in new[] { typeof(ActiveMagicSpringPowder), typeof(MagicOrganExtract), typeof(ScalePotion) })
		{
			var item = CreateItem(type);
			foreach (string field in new[] { "DisplayName", "Tooltip" })
			{
				string key = item.GetLocalizationKey(field);
				Assert.IsTrue(texts.ContainsKey(key), $"Missing {culture}: {key}");
			}
			AssertFormatted(item.DisplayName, culture);
			AssertFormatted(item.Tooltip, culture);
			if (item is ActiveMagicSpringPowder)
			{
				StringAssert.Contains(item.Tooltip.Value, ActiveMagicSpringPowder.ManaPerUse.ToString());
				StringAssert.Contains(item.Tooltip.Value, ActiveMagicSpringPowder.MaxUse.ToString());
			}
			if (item is MagicOrganExtract)
			{
				StringAssert.Contains(item.Tooltip.Value, MagicOrganExtract.ManaPerUse.ToString());
				StringAssert.Contains(item.Tooltip.Value, MagicOrganExtract.MaxUse.ToString());
			}
		}
		var buff = (ScalePotionBuff)RuntimeHelpers.GetUninitializedObject(typeof(ScalePotionBuff));
		typeof(ModType).GetProperty("Mod")!.SetValue(buff, RuntimeHelpers.GetUninitializedObject(typeof(TooltipMod)));
		foreach (string field in new[] { "DisplayName", "Description" })
		{
			string key = buff.GetLocalizationKey(field);
			Assert.IsTrue(texts.ContainsKey(key), $"Missing {culture}: {key}");
			AssertFormatted(Language.GetText(key), culture);
		}
	}

	[TestMethod]
	[DataRow("zh-Hans")]
	[DataRow("en-US")]
	public void GunOfAvariceFormatsDynamicAmmoAndLevelLabels(string culture)
	{
		LoadLocalization(culture, "ExtraTooltip");
		var gun = (GunOfAvarice)CreateItem(typeof(GunOfAvarice));
		foreach (var (ammo, level) in new[] { (14, 1), (3, 7), (0, GunOfAvarice.MaxLevel) })
		{
			typeof(GunOfAvarice).GetProperty("AmmoAmount", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(gun, ammo);
			typeof(GunOfAvarice).GetProperty("Level", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(gun, level);
			var lines = new List<TooltipLine>();
			gun.ModifyTooltips(lines);
			Assert.AreEqual(culture == "zh-Hans" ? $"当前弹药：{ammo}/14" : $"Current ammo: {ammo}/14",
				lines.Single(line => line.Name == "GunOfAvariceAmmo").Text);
			Assert.AreEqual(culture == "zh-Hans" ? $"等级：{level}" : $"Level: {level}",
				lines.Single(line => line.Name == "GunOfAvariceLevel").Text);
		}
		Assert.AreEqual(culture == "zh-Hans" ? "最高等级" : "Max level",
			Language.GetTextValue("Mods.Everglow.ExtraTooltip.GunOfAvarice.MaxLevel"));
	}

	[TestMethod]
	[DataRow("zh-Hans")]
	[DataRow("en-US")]
	public void EquipmentNamesRespectLocalizationAfterSetDefaults(string culture)
	{
		LoadLocalization(culture, "Items.Accessories", "Items.PermanentBoosters", "Items.Weapons.Melee");
		var names = (LocalizedText[])typeof(Lang)
			.GetField("_itemNameCache", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(null)!;
		LocalizedText originalName = names[ItemID.Wood];
		bool originalSkipsSound = ItemID.Sets.SkipsInitialUseSound[ItemID.Wood];
		bool originalSpear = ItemID.Sets.Spears[ItemID.Wood];
		try
		{
			foreach (var type in new[]
			{
				typeof(HotAirBalloon), typeof(IstafelsSunfireGrasp), typeof(AntiHeavenSicknessPill),
				typeof(AmberHeadedSpear), typeof(AmethystHeadedSpear), typeof(TopaztHeadedSpear),
			})
			{
				var item = CreateItem(type);
				AssertFormatted(item.DisplayName, culture);
				// Supply the name cache normally populated by content loading without initializing the game.
				names[ItemID.Wood] = item.DisplayName;
				item.Item.type = item.Item.netID = ItemID.Wood;
				item.SetStaticDefaults();
				item.SetDefaults();
				Assert.AreEqual(item.DisplayName.Value, item.Item.Name, $"Name override bypasses {culture}: {type.Name}");
			}
		}
		finally
		{
			names[ItemID.Wood] = originalName;
			ItemID.Sets.SkipsInitialUseSound[ItemID.Wood] = originalSkipsSound;
			ItemID.Sets.Spears[ItemID.Wood] = originalSpear;
		}
	}

	private static ModItem CreateItem(Type type)
	{
		var item = (ModItem)RuntimeHelpers.GetUninitializedObject(type);
		typeof(ModType).GetProperty("Mod")!.SetValue(item, RuntimeHelpers.GetUninitializedObject(typeof(TooltipMod)));
		typeof(ModType<Item>).GetProperty("Entity")!.SetValue(item, RuntimeHelpers.GetUninitializedObject(typeof(Item)));
		return item;
	}

	private static DirectoryInfo FindRepositoryRoot()
	{
		var root = new DirectoryInfo(AppContext.BaseDirectory);
		while (root is not null && !File.Exists(Path.Combine(root.FullName, "Everglow.sln")))
		{
			root = root.Parent;
		}
		Assert.IsNotNull(root);
		return root;
	}

	private static Dictionary<string, LocalizedText> LoadLocalization(string culture, params string[] categories)
	{
		var texts = (Dictionary<string, LocalizedText>)typeof(LanguageManager)
			.GetField("_localizedTexts", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(LanguageManager.Instance)!;
		foreach (string category in categories)
		{
			string prefix = $"Mods.Everglow.{category}";
			string path = Path.Combine(FindRepositoryRoot().FullName, "Sources", "Everglow", "Localization", culture, prefix + ".hjson");
			using var document = JsonDocument.Parse(HjsonValue.Parse(File.ReadAllText(path)).ToString(Stringify.Plain));
			AddTexts(document.RootElement, prefix);
		}
		return texts;

		void AddTexts(JsonElement value, string prefix)
		{
			foreach (var field in value.EnumerateObject())
			{
				string key = prefix + "." + field.Name;
				if (field.Value.ValueKind == JsonValueKind.Object)
				{
					AddTexts(field.Value, key);
				}
				else
				{
					texts[key] = (LocalizedText)Activator.CreateInstance(
						typeof(LocalizedText), BindingFlags.Instance | BindingFlags.NonPublic,
						null, [key, field.Value.GetString()!], null)!;
				}
			}
		}
	}

	private static void AssertFormatted(LocalizedText text, string culture)
	{
		Assert.IsFalse(string.IsNullOrWhiteSpace(text.Value), $"Empty {culture}: {text.Key}");
		Assert.AreNotEqual(text.Key, text.Value, $"Unresolved {culture}: {text.Key}");
		Assert.IsFalse(Regex.IsMatch(text.Value, @"\{(?:\d|\$)"), $"Unformatted {culture}: {text.Key}: {text.Value}");
	}
}
