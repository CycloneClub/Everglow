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
			Assert.IsTrue(supplies.All(x => x.stack > 0));
		}
	}

	[TestMethod]
	public void SupplementsNeverDuplicate()
	{
		WorldGen._genRand = new UnifiedRandom(123);
		for (int i = 0; i < 10000; i++)
		{
			var loot = YggdrasilTownGeneration.BridgeChestContents();
			Assert.AreEqual(loot.Count, loot.Select(x => x.type).Distinct().Count());
			Assert.AreEqual(1, loot.Count(x => x.type == Rope));
			Assert.IsTrue(loot.Count(x => Potions.Contains(x.type)) <= 1);
			Assert.IsTrue(loot.All(item => item.stack > 0));
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
		double Share(int type) => counts.GetValueOrDefault(type) / (double)samples;
		void EqualGroup(IEnumerable<int> types)
		{
			double rate = Share(types.First());
			foreach (int type in types)
			{
				Assert.AreEqual(rate, Share(type), 0.015, $"Pool members diverged at {type}.");
			}

			Assert.IsTrue(rate > 0);
		}

		EqualGroup(Primary);
		EqualGroup(Secondary);
		EqualGroup(Potions);
		Assert.AreEqual(Share(ItemID.SilverCoin), Share(ItemID.LesserHealingPotion), 0.015);
		Assert.AreEqual(Share(ItemID.FlamingArrow), Share(ItemID.SilverBullet), 0.015);
		Assert.AreEqual(1, Share(Rope), 0.015);
		Assert.IsTrue(twoSecondary > 0 && twoSecondary < samples);
	}
}
