using System.Reflection;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TwilightRelicChestLootTests
{
	private static readonly string[] Themes = ["Wood", "Water", "Shadow", "Blood", "Rock", "Sand"];
	private static readonly int[] Primary = [1, 2, 3, 4, 5, 6];
	private static readonly int[] Secondary = [40, 41, 42, 43, 44, 45];
	private readonly List<Action> restore = [];
	private readonly Dictionary<Point, string> rooms = [];

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		var chests = Main.chest;
		var random = WorldGen._genRand;
		bool basicChest = TileID.Sets.BasicChest[TileID.Containers];
		restore.Add(() => { Main.chest = chests; WorldGen._genRand = random; TileID.Sets.BasicChest[TileID.Containers] = basicChest; });
		TileID.Sets.BasicChest[TileID.Containers] = true;
		Main.chest = new Chest[7];
		SetItem<WiltedForestLamp>(Primary[0]);
		SetItem<DivineAscend>(Primary[1]);
		SetItem<MagicOfLightAndShadow>(Primary[2]);
		SetItem<BloodySwamp>(Primary[3]);
		SetItem<RockQuake>(Primary[4]);
		SetItem<SpringOfQuicksand>(Primary[5]);
		SetItem<MechanismHalberd>(Secondary[0]);
		SetItem<BloodTearCrystalCrown>(Secondary[1]);
		SetItem<CelesteStoneWaistPendant>(Secondary[2]);
		SetItem<SpikedParma>(Secondary[3]);
		SetItem<DarkMassacreDagger>(Secondary[4]);
		SetItem<CyanVineRing>(Secondary[5]);
		SetItem<NylonRope>(ItemID.Rope);
		SetItem<YggdrasilAmber>(ItemID.Amber);
		SetItem<CyanVineOre>(ItemID.CopperOre);
		for (int i = 0; i < 6; i++)
		{
			int side = i % 2 == 0 ? 1 : 0;
			rooms.Add(new Point(side, i), Themes[i]);
			int x = 150 + side * 100, y = 112 + i * 25;
			var tile = Main.tile[x, y];
			ushort type = tile.TileType;
			var state = tile.Get<TileWallWireStateData>();
			restore.Add(() => { var saved = Main.tile[x, y]; saved.TileType = type; saved.Get<TileWallWireStateData>() = state; });
			tile.HasTile = true;
			tile.TileType = TileID.Containers;
			Main.chest[i] = new Chest { x = x, y = y };
		}
		Main.chest[6] = new Chest { x = 400, y = 400 };
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var action in restore.AsEnumerable().Reverse())
		{
			action();
		}
	}

	private void SetItem<T>(int type) where T : ModItem, new()
	{
		var property = typeof(ContentInstance<T>).GetProperty(nameof(ContentInstance<T>.Instance))!;
		object? previous = property.GetValue(null);
		restore.Add(() => property.SetValue(null, previous));
		property.SetValue(null, (T)new T().NewInstance(new Item { type = type }));
	}

	private void Fill()
	{
		typeof(YggdrasilTownGeneration).GetMethod("FillTwilightRelicRewards", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [200, 200, rooms]);
	}

	[TestMethod]
	public void EveryWorldHasAllSixSecondaryRewardsAndPreservesThemedPrimaries()
	{
		var assignments = new HashSet<string>();
		for (int seed = 0; seed < 256; seed++)
		{
			WorldGen._genRand = new UnifiedRandom(seed);
			foreach (var chest in Main.chest)
			{
				chest.item = Enumerable.Range(0, 40).Select(_ => new Item()).ToArray();
			}
			Fill();
			var found = new List<int>();
			for (int i = 0; i < 6; i++)
			{
				var loot = Main.chest[i].item.Where(item => !item.IsAir).ToArray();
				Assert.AreEqual(1, loot.Count(item => Primary.Contains(item.type)));
				Assert.AreEqual(Primary[i], loot[0].type);
				Assert.AreEqual(1, loot[0].stack);
				var secondary = loot.Single(item => Secondary.Contains(item.type));
				Assert.AreEqual(1, secondary.stack);
				found.Add(secondary.type);
				Assert.IsTrue(loot.Any(item => item.type == ItemID.Rope));
			}
			CollectionAssert.AreEquivalent(Secondary, found.ToArray());
			assignments.Add(string.Join(",", found));
			Assert.IsTrue(Main.chest[6].item.All(item => item.IsAir));
		}
		Assert.IsTrue(assignments.Count > 1, "Secondary rewards must not be tied to fixed themes.");
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void MissingOrOccupiedThemeChestCannotSilentlyOmitRewards(bool occupied)
	{
		WorldGen._genRand = new UnifiedRandom(1);
		if (occupied)
		{
			Main.chest[0].item[0] = new Item(ItemID.DirtBlock);
		}
		else
		{
			Main.chest[0] = null!;
		}
		var error = Assert.ThrowsException<TargetInvocationException>(Fill);
		Assert.IsInstanceOfType<InvalidOperationException>(error.InnerException);
	}

	[TestMethod]
	public void MissingThemeCannotSilentlyOmitASecondaryReward()
	{
		WorldGen._genRand = new UnifiedRandom(1);
		rooms.Remove(new Point(1, 0));
		var error = Assert.ThrowsException<TargetInvocationException>(Fill);
		Assert.IsInstanceOfType<InvalidOperationException>(error.InnerException);
	}
}
