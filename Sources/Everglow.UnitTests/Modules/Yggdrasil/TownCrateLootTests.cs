using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TownCrateLootTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	[DataRow(typeof(LampWoodCrate_Item))]
	[DataRow(typeof(TwilightCrate_Item))]
	[DataRow(typeof(UrbanCrate_Item))]
	public void EveryCrateRuleHasAtLeastOnePossibleOutcome(Type type)
	{
		var crate = (ModItem)RuntimeHelpers.GetUninitializedObject(type);
		var loot = new ItemLoot(1, new ItemDropDatabase());
		crate.ModifyItemLoot(loot);
		Assert.IsTrue(loot.Get().Count > 0);
		foreach (var rule in loot.Get())
		{
			List<DropRateInfo> rates = [];
			rule.ReportDroprates(rates, new DropRateInfoChainFeed(1));
			Assert.IsTrue(rates.Count > 0, $"{type.Name}: {rule.GetType().Name} contains an empty pool");
		}
	}

	[TestMethod]
	public void UrbanCrateAlwaysProvidesOneEqualChanceMineOrBridgeReward()
	{
		List<Action> restoreContent = [];
		int[] expected = Enumerable.Range(1001, 9).ToArray();
		try
		{
			SetItem<OldPickaxe>(expected[0]);
			SetItem<ShacklesBall>(expected[1]);
			SetItem<SkullCollection>(expected[2]);
			SetItem<FragmentsOfWreckage>(expected[3]);
			SetItem<FuneraryGoods>(expected[4]);
			SetItem<ForsakenBoots>(expected[5]);
			SetItem<FlappedHangGlider>(expected[6]);
			SetItem<OldPortableLamp>(expected[7]);
			SetItem<RustSlingshot>(expected[8]);
			var crate = (UrbanCrate_Item)RuntimeHelpers.GetUninitializedObject(typeof(UrbanCrate_Item));
			var loot = new ItemLoot(1, new ItemDropDatabase());
			crate.ModifyItemLoot(loot);
			var themedRule = loot.Get().OfType<OneFromOptionsNotScaledWithLuckDropRule>().Single();
			List<DropRateInfo> rates = [];
			themedRule.ReportDroprates(rates, new DropRateInfoChainFeed(1));
			CollectionAssert.AreEquivalent(expected, rates.Select(rate => rate.itemId).ToArray());
			float share = rates[0].dropRate;
			foreach (var rate in rates)
			{
				Assert.AreEqual(share, rate.dropRate, 0.000001f);
				Assert.AreEqual(1, rate.stackMin);
				Assert.AreEqual(1, rate.stackMax);
			}

			Assert.AreEqual(1f, rates.Sum(rate => rate.dropRate), 0.00001f);
		}
		finally
		{
			foreach (var restore in restoreContent)
			{
				restore();
			}
		}

		void SetItem<T>(int type)
			where T : ModItem, new()
		{
			var property = typeof(ContentInstance<T>).GetProperty(nameof(ContentInstance<T>.Instance))!;
			object? original = property.GetValue(null);
			restoreContent.Add(() => property.SetValue(null, original));
			property.SetValue(null, (T)new T().NewInstance(new Item { type = type }));
		}
	}
}
