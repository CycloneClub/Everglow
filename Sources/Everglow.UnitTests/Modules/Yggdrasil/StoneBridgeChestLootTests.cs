using Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;
using Everglow.Yggdrasil.YggdrasilTown.Items.Potions;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria;
using Terraria.ModLoader;
using Everglow.Yggdrasil.WorldGeneration;
using Terraria.ID;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class StoneBridgeChestLootTests
{
	// Vanilla IDs stand in for mod content while exercising the real Item creation path.
	private static readonly int[] Primary = [ItemID.CopperShortsword, ItemID.CopperBroadsword, ItemID.CopperAxe, ItemID.CopperHammer];
	private static readonly int[] Secondary = [ItemID.Torch, ItemID.ThrowingKnife];
	private const int Rope = ItemID.Rope;
	private readonly List<Action> restoreContent = [];
	private UnifiedRandom? originalRandom;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalRandom = WorldGen._genRand;
		SetItem<ForsakenBoots>(Primary[0]);
		SetItem<FlappedHangGlider>(Primary[1]);
		SetItem<OldPortableLamp>(Primary[2]);
		SetItem<RustSlingshot>(Primary[3]);
		SetItem<RuinTorch_Item>(Secondary[0]);
		SetItem<Caltrop>(ItemID.Shuriken);
		SetItem<StoneJavelin>(Secondary[1]);
		SetItem<LightArrow>(ItemID.FlamingArrow);
		SetItem<LightBullet>(ItemID.SilverBullet);
		SetItem<GuidePotion>(ItemID.ShinePotion);
		SetItem<RayPotion>(ItemID.NightOwlPotion);
		SetItem<JumpPotion>(ItemID.SwiftnessPotion);
		SetItem<ScalePotion>(ItemID.IronskinPotion);
		SetItem<NylonRope>(Rope);
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var restore in restoreContent)
		{
			restore();
		}
		restoreContent.Clear();
		WorldGen._genRand = originalRandom;
	}

	private void SetItem<T>(int type)
		where T : ModItem, new()
	{
		var property = typeof(ContentInstance<T>).GetProperty(nameof(ContentInstance<T>.Instance))!;
		object? original = property.GetValue(null);
		restoreContent.Add(() => property.SetValue(null, original));
		property.SetValue(null, (T)new T().NewInstance(new Item { type = type }));
	}

	private static readonly int[] Potions = [ItemID.IronskinPotion, ItemID.ShinePotion, ItemID.NightOwlPotion, ItemID.SwiftnessPotion, ItemID.MiningPotion, ItemID.BuilderPotion];

	[TestMethod]
	public void EveryChestHasOneEquipmentAndOneOrTwoDistinctSecondaryRewards()
	{
		WorldGen._genRand = new UnifiedRandom(42);
		for (int i = 0; i < 10000; i++)
		{
			var loot = YggdrasilTownGeneration.BridgeChestContents();
			Assert.IsFalse(loot.Any(item => item.type == ItemID.Shuriken), "Caltrop is sold only for furnace contribution.");
			var equipment = loot.Where(x => Primary.Contains(x.type)).ToArray();
			Assert.AreEqual(1, equipment.Length);
			Assert.AreEqual(1, equipment[0].stack);
			var supplies = loot.Where(x => Secondary.Contains(x.type)).ToArray();
			Assert.IsTrue(supplies.Length is 1 or 2);
			Assert.AreEqual(supplies.Length, supplies.Select(x => x.type).Distinct().Count());
			Assert.IsTrue(supplies.All(x => x.stack >= 16 && x.stack <= 32));
		}
	}

	[TestMethod]
	public void SupplementsNeverDuplicateAndIncludeInclusiveQuantityEndpoints()
	{
		WorldGen._genRand = new UnifiedRandom(123);
		var ranges = new Dictionary<int, (int Min, int Max)> { [Rope] = (70, 150), [ItemID.SilverCoin] = (10, 29), [ItemID.LesserHealingPotion] = (3, 5), [ItemID.RecallPotion] = (3, 5), [ItemID.FlamingArrow] = (25, 50), [ItemID.SilverBullet] = (25, 50) };
		foreach (int type in Primary)
		{
			ranges[type] = (1, 1);
		}
		foreach (int type in Secondary)
		{
			ranges[type] = (16, 32);
		}
		foreach (int type in Potions)
		{
			ranges[type] = (1, 2);
		}
		var seen = new HashSet<(int Type, int Stack)>();
		for (int i = 0; i < 10000; i++)
		{
			var loot = YggdrasilTownGeneration.BridgeChestContents();
			Assert.AreEqual(loot.Count, loot.Select(x => x.type).Distinct().Count());
			Assert.AreEqual(1, loot.Count(x => x.type == Rope));
			Assert.IsTrue(loot.Count(x => Potions.Contains(x.type)) <= 1);
			foreach (var item in loot)
			{
				Assert.IsTrue(ranges.TryGetValue(item.type, out var range), "Unexpected item in the bridge-specific pool.");
				Assert.IsTrue(item.stack >= range.Min && item.stack <= range.Max);
				seen.Add((item.type, item.stack));
			}
		}
		foreach (var (type, range) in ranges)
		{
			Assert.IsTrue(seen.Contains((type, range.Min)), $"Missing minimum for {type}.");
			Assert.IsTrue(seen.Contains((type, range.Max)), $"Missing inclusive maximum for {type}.");
		}
	}

	[TestMethod]
	public void FixedSeedSampleMatchesIndependentPoolProbabilities()
	{
		const int samples = 60000;
		WorldGen._genRand = new UnifiedRandom(20261021);
		var counts = new Dictionary<int, int>();
		int twoSecondary = 0;
		for (int i = 0; i < samples; i++)
		{
			var loot = YggdrasilTownGeneration.BridgeChestContents();
			if (loot.Count(x => Secondary.Contains(x.type)) == 2)
			{
				twoSecondary++;
			}
			foreach (var item in loot)
			{
				counts[item.type] = counts.GetValueOrDefault(item.type) + 1;
			}
		}
		void Rate(int type, double expected) => Assert.AreEqual(expected, counts.GetValueOrDefault(type) / (double)samples, 0.015, $"Wrong rate for {type}.");
		foreach (int type in Primary)
		{
			Rate(type, 0.25);
		}
		foreach (int type in Secondary)
		{
			Rate(type, 0.75);
		}
		foreach (int type in Potions)
		{
			Rate(type, 1.0 / 9);
		}
		Rate(ItemID.SilverCoin, 0.5);
		Rate(ItemID.LesserHealingPotion, 0.5);
		Rate(ItemID.RecallPotion, 2.0 / 3);
		Rate(ItemID.FlamingArrow, 0.25);
		Rate(ItemID.SilverBullet, 0.25);
		Rate(Rope, 1);
		Assert.AreEqual(0.5, twoSecondary / (double)samples, 0.015);
	}
}
