using Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;
using Everglow.Yggdrasil.YggdrasilTown.Items.Potions;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing.FishingRods;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Pets;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TwilightChestLootTests
{
	private static readonly int[] Primary = [ItemID.CopperShortsword, ItemID.CopperBroadsword, ItemID.CopperAxe, ItemID.CopperHammer, ItemID.IronShortsword, ItemID.IronBroadsword, ItemID.IronPickaxe];
	private const int Mill = ItemID.IronAnvil;
	private readonly List<Action> restore = [];

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		var random = WorldGen._genRand;
		restore.Add(() => WorldGen._genRand = random);
		SetItem<LampWoodSeed>(Primary[0]);
		SetItem<RecoveryBand>(Primary[1]);
		SetItem<GlowstickLauncher>(Primary[2]);
		SetItem<NightfireStaff>(Primary[3]);
		SetItem<MidwinterNightmare>(Primary[4]);
		SetItem<HexaCrystalStaff>(Primary[5]);
		SetItem<TwilightRod>(Primary[6]);
		SetItem<TwilightCrystalMill_Item>(Mill);
		SetItem<LightArrow>(ItemID.FlamingArrow);
		SetItem<LightBullet>(ItemID.SilverBullet);
		SetItem<GuidePotion>(ItemID.ShinePotion);
		SetItem<RayPotion>(ItemID.NightOwlPotion);
		SetItem<JumpPotion>(ItemID.SwiftnessPotion);
		SetItem<ScalePotion>(ItemID.IronskinPotion);
		SetItem<RuinTorch_Item>(ItemID.Torch);
		SetItem<StoneJavelin>(ItemID.Javelin);
		SetItem<NylonRope>(ItemID.Rope);
		SetItem<YggdrasilAmber>(ItemID.Amber);
		SetItem<CyanVineOre>(ItemID.CopperOre);
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var action in restore.AsEnumerable().Reverse())
		{
			action();
		}
	}

	private void SetItem<T>(int type)
		where T : ModItem, new()
	{
		var property = typeof(ContentInstance<T>).GetProperty(nameof(ContentInstance<T>.Instance))!;
		object? previous = property.GetValue(null);
		restore.Add(() => property.SetValue(null, previous));
		property.SetValue(null, (T)new T().NewInstance(new Item { type = type }));
	}

	[TestMethod]
	public void ChestsKeepSevenWayPrimaryAndIndependentTwentyPercentMill()
	{
		WorldGen._genRand = new UnifiedRandom(20261023);
		const int Samples = 35000;
		var counts = Primary.ToDictionary(id => id, _ => 0);
		var mills = Primary.ToDictionary(id => id, _ => 0);
		for (int i = 0; i < Samples; i++)
		{
			var items = YggdrasilTownGeneration.TwilightNormalChestContents();
			var primary = items.Single(item => Primary.Contains(item.type));
			Assert.AreEqual(1, primary.stack);
			counts[primary.type]++;
			var mill = items.Where(item => item.type == Mill).ToArray();
			Assert.IsTrue(mill.Length <= 1);
			if (mill.Length == 1)
			{
				Assert.AreEqual(1, mill[0].stack);
				mills[primary.type]++;
			}
			Assert.IsTrue(items.Any(item => item.type == ItemID.Rope));
		}
		foreach (int id in Primary)
		{
			Assert.AreEqual(1.0 / 7, counts[id] / (double)Samples, 0.01);
			Assert.AreEqual(0.2, mills[id] / (double)counts[id], 0.025);
		}
	}

	[TestMethod]
	public void CrateHasSevenWayPrimaryAndSeparateFixedTwentyPercentMill()
	{
		var loot = new ItemLoot(ItemID.WoodenCrate, new ItemDropDatabase());
		new TwilightCrate_Item().ModifyItemLoot(loot);
		var rates = new List<DropRateInfo>();
		foreach (var rule in loot.Get())
		{
			rule.ReportDroprates(rates, new DropRateInfoChainFeed(1));
		}
		foreach (int id in Primary)
		{
			Assert.AreEqual(1f / 7, rates.Single(rate => rate.itemId == id).dropRate, 0.00001f);
		}
		var mill = loot.Get().OfType<CommonDrop>().Single(rule => rule.itemId == Mill);
		Assert.AreEqual(5, mill.chanceDenominator);
		Assert.AreEqual(1, mill.chanceNumerator);
		Assert.AreEqual(1, mill.amountDroppedMinimum);
		Assert.AreEqual(1, mill.amountDroppedMaximum);
		Assert.AreEqual(0, mill.ChainedRules.Count);
		Assert.AreNotEqual(typeof(CommonDrop), mill.GetType(), "The fixed 20% crate chance must not scale with luck.");
	}
}
