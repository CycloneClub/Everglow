using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Hjson;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Valiant;
using Terraria;
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
		var root = new DirectoryInfo(AppContext.BaseDirectory);
		while (root is not null && !File.Exists(Path.Combine(root.FullName, "Everglow.sln")))
		{
			root = root.Parent;
		}
		Assert.IsNotNull(root);
		var texts = (Dictionary<string, LocalizedText>)typeof(LanguageManager)
			.GetField("_localizedTexts", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(LanguageManager.Instance)!;
		foreach (var category in new[] { "Armor", "Accessories", "Accessories.Wings" })
		{
			var path = Path.Combine(root.FullName, "Sources", "Everglow", "Localization", culture, $"Mods.Everglow.Items.{category}.hjson");
			using var document = JsonDocument.Parse(HjsonValue.Parse(File.ReadAllText(path)).ToString(Stringify.Plain));
			foreach (var item in document.RootElement.EnumerateObject())
			{
				foreach (var field in item.Value.EnumerateObject())
				{
					string key = $"Mods.Everglow.Items.{category}.{item.Name}.{field.Name}";
					texts[key] = (LocalizedText)Activator.CreateInstance(
						typeof(LocalizedText),
						BindingFlags.Instance | BindingFlags.NonPublic, null, [key, field.Value.GetString()!], null)!;
				}
			}
		}

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
				StringAssert.Contains(player.setBonus, culture == "zh-Hans" ? "4个百分点" : "4 percentage points");
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

	private static void AssertFormatted(LocalizedText text, string culture)
	{
		Assert.IsFalse(string.IsNullOrWhiteSpace(text.Value), $"Empty {culture}: {text.Key}");
		Assert.AreNotEqual(text.Key, text.Value, $"Unresolved {culture}: {text.Key}");
		Assert.IsFalse(Regex.IsMatch(text.Value, @"\{(?:\d|\$)"), $"Unformatted {culture}: {text.Key}: {text.Value}");
	}
}
