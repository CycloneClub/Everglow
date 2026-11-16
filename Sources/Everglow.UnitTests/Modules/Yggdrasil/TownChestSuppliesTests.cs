using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;
using Everglow.Yggdrasil.YggdrasilTown.Items.Potions;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TownChestSuppliesTests
{
	private static readonly int[] Ammo = [ItemID.WoodenArrow, ItemID.MusketBall, ItemID.ThrowingKnife];
	private static readonly int[] Potions = [ItemID.ShinePotion, ItemID.NightOwlPotion, ItemID.FeatherfallPotion, ItemID.IronskinPotion, ItemID.MiningPotion, ItemID.SpelunkerPotion, ItemID.HunterPotion, ItemID.WaterWalkingPotion];
	private readonly List<Action> restore = [];

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		var random = WorldGen._genRand;
		restore.Add(() => WorldGen._genRand = random);
		SetItem<AmberMagicOrb>(600);
		SetItem<YggdrasilAmberLaser>(601);
		SetItem<SevenShotGun>(602);
		SetItem<YggdrasilStoneGyroscope>(603);
		SetItem<LampWoodPollenBottle>(604);
		SetItem<CelticSeal>(605);
		SetItem<ConcealSpell>(606);
		SetItem<OldPickaxe>(610);
		SetItem<ShacklesBall>(611);
		SetItem<SkullCollection>(612);
		SetItem<FragmentsOfWreckage>(613);
		SetItem<FuneraryGoods>(614);
		SetItem<NylonRope>(ItemID.Rope);
		SetItem<RuinTorch_Item>(ItemID.Torch);
		SetItem<LightArrow>(Ammo[0]);
		SetItem<LightBullet>(Ammo[1]);
		SetItem<StoneJavelin>(Ammo[2]);
		SetItem<GuidePotion>(Potions[0]);
		SetItem<RayPotion>(Potions[1]);
		SetItem<JumpPotion>(Potions[2]);
		SetItem<ScalePotion>(Potions[3]);
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
	public void CommonSupplyPoolsHaveIndependentRatesAndInclusiveStackLimits()
	{
		const int Samples = 60000;
		WorldGen._genRand = new UnifiedRandom(20261023);
		int ammoCount = 0;
		int torchCount = 0;
		int both = 0;
		for (int i = 0; i < Samples; i++)
		{
			var loot = YggdrasilTownGeneration.NormalChestContents();
			Assert.IsTrue(loot.Count < 38, "Leave room for both guaranteed themed rewards.");
			Assert.AreEqual(loot.Count, loot.Select(item => item.type).Distinct().Count());
			Assert.AreEqual(1, loot.Count(item => item.type == ItemID.Rope));
			Assert.IsTrue(loot.Count(item => Ammo.Contains(item.type)) <= 1);
			Assert.IsTrue(loot.Count(item => Potions.Contains(item.type)) <= 1);
			bool hasAmmo = loot.Any(item => Ammo.Contains(item.type));
			bool hasTorch = loot.Any(item => item.type == ItemID.Torch);
			if (hasAmmo)
			{
				ammoCount++;
			}

			if (hasTorch)
			{
				torchCount++;
			}

			if (hasAmmo && hasTorch)
			{
				both++;
			}

			Assert.IsTrue(loot.All(item => item.stack > 0));
		}

		double ammoRate = ammoCount / (double)Samples;
		double torchRate = torchCount / (double)Samples;
		Assert.AreEqual(ammoRate * torchRate, both / (double)Samples, 0.012, "Ammo and lighting rolls are independent.");
	}

	[TestMethod]
	public void LampWoodAndSubminePreserveTheirPrimaryPoolsAndIncludeSupplies()
	{
		WorldGen._genRand = new UnifiedRandom(831);
		var lampPrimaries = new HashSet<int>();
		var minePrimaries = new HashSet<int>();
		bool lampAmmo = false;
		bool mineAmmo = false;
		for (int i = 0; i < 5000; i++)
		{
			var lamp = YggdrasilTownGeneration.LampWoodChestContents();
			var mine = YggdrasilTownGeneration.SubmineChestContents();
			Assert.IsTrue(lamp[0].type is >= 600 and <= 606);
			Assert.IsTrue(mine[0].type is >= 610 and <= 614);
			Assert.AreEqual(1, lamp[0].stack);
			Assert.AreEqual(1, mine[0].stack);
			lampPrimaries.Add(lamp[0].type);
			minePrimaries.Add(mine[0].type);
			Assert.IsFalse(lamp.Any(item => item.type is ItemID.Amber or ItemID.CopperOre));
			Assert.AreEqual(1, lamp.Count(item => item.type == ItemID.Rope));
			Assert.AreEqual(1, mine.Count(item => item.type == ItemID.Rope));
			lampAmmo |= lamp.Any(item => Ammo.Contains(item.type));
			mineAmmo |= mine.Any(item => Ammo.Contains(item.type));
		}
		Assert.AreEqual(7, lampPrimaries.Count);
		Assert.AreEqual(5, minePrimaries.Count);
		Assert.IsTrue(lampAmmo && mineAmmo);
	}
}
