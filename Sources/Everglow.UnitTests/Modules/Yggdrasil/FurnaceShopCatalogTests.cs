using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.YggdrasilTown.UI;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class FurnaceShopCatalogTests
{
	private static readonly Func<IReadOnlyList<(int ItemType, int Price)>> GetShopEntries = typeof(FurnaceScoreShopUI)
		.GetMethod("GetShopEntries", BindingFlags.Static | BindingFlags.NonPublic)!
		.CreateDelegate<Func<IReadOnlyList<(int ItemType, int Price)>>>();

	private readonly List<Action> restoreContent = [];
	private readonly Dictionary<string, int> types = [];

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		// The required 09-sheet inventory, independent of the shop's production catalog.
		string[] furniture = ["Bathtub", "Bed", "Bookcase", "Candelabra", "Candle", "Chair", "Chandelier", "Chest", "Clock", "Door", "Dresser", "Lamp", "Lantern", "Piano", "Sink", "Sofa", "Table", "Toilet", "Workbench"];
		foreach (var name in furniture)
		{
			Bind("Placeables.Furniture.Furnace.Heatproof" + name + "_item");
		}
		foreach (var name in new[] { "MagicalBoomerang", "PearShapedNeedle", "ChainGrenade", "ArmorPiercingBlaster", "Caltrop", "IntroductiontoThermalElectricity", "MeltingSideGyroscope", "QuenchingBlade", "ResistanceWireBayonet" })
		{
			Bind("Weapons." + name);
		}
		foreach (var name in new[] { "AnemoShell", "TelescopicPlatformAlpha", "TelescopicPlatformBeta" })
		{
			Bind("Accessories." + name);
		}
		Bind("Miscs.ElectroProbe");
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var restore in restoreContent.AsEnumerable().Reverse())
		{
			restore();
		}
	}

	[TestMethod]
	public void EveryPlannedPurchaseHasOnePositivePriceAndNoMilestoneRewardLeaksIntoStock()
	{
		var entries = GetShopEntries();
		CollectionAssert.AreEquivalent(types.Values.ToArray(), entries.Select(x => x.ItemType).ToArray());
		Assert.IsTrue(entries.All(x => x.Price > 0));
		Assert.AreEqual(entries.Count, entries.Select(x => x.ItemType).Distinct().Count());
	}

	[TestMethod]
	public void PreviouslySpecifiedPricesAndProbePriceArePreserved()
	{
		var entries = GetShopEntries().ToDictionary(x => x.ItemType, x => x.Price);
		var expected = new Dictionary<string, int>
		{
			["MagicalBoomerang"] = 14500, ["PearShapedNeedle"] = 15500,
			["ChainGrenade"] = 17000, ["ArmorPiercingBlaster"] = 14000,
			["Caltrop"] = 12800, ["AnemoShell"] = 26200,
			["TelescopicPlatformAlpha"] = 75000, ["TelescopicPlatformBeta"] = 900000,
			["ElectroProbe"] = 5000,
		};
		foreach (var (name, price) in expected)
		{
			Assert.IsTrue(entries.TryGetValue(types[name], out int actual), name);
			Assert.AreEqual(price, actual, name);
		}
	}

	private void Bind(string relativeName)
	{
		var type = typeof(FurnaceScoreShopUI).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Items." + relativeName, true)!;
		var content = RuntimeHelpers.GetUninitializedObject(type);
		var item = (Item)RuntimeHelpers.GetUninitializedObject(typeof(Item));
		item.type = 41000 + types.Count;
		types.Add(type.Name, item.type);
		typeof(ModType<Item>).GetProperty("Entity")!.SetValue(content, item);
		var instance = typeof(ContentInstance<>).MakeGenericType(type).GetProperty("Instance")!;
		var previous = instance.GetValue(null);
		instance.SetValue(null, content);
		restoreContent.Add(() => instance.SetValue(null, previous));
	}
}
