using System.Collections;
using System.Reflection;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.YggdrasilTown;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing.Trash;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class YggdrasilTownFishingTests
{
	private static readonly FieldInfo CurrentSubworld = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
	private readonly List<Action> restoreContent = [];
	private Subworld? originalWorld;
	private UnifiedRandom originalRandom = null!;
	private BitArray biomeFlags = null!;
	private YggdrasilTownFishingPlayer fishingPlayer = null!;
	private int nextItemType;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalWorld = (Subworld?)CurrentSubworld.GetValue(null);
		originalRandom = Main.rand;
		Main.rand = new UnifiedRandom(11);
		CurrentSubworld.SetValue(null, new YggdrasilWorld());
		var player = new Player();
		biomeFlags = new BitArray(3);
		typeof(Player).GetField("modBiomeFlags", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(player, biomeFlags);
		fishingPlayer = (YggdrasilTownFishingPlayer)new YggdrasilTownFishingPlayer().NewInstance(player);
		SetBiome<YggdrasilTownBiome>(0);
		SetBiome<LampWoodForest>(1);
		SetBiome<TwilightForsetAndRelic>(2);
		nextItemType = ItemID.Count;
		SetItem<GrassCarp>();
		SetItem<Filefish>();
		SetItem<Duskfin>();
		SetItem<GuidyFish>();
		SetItem<DimmyFish>();
		SetItem<CloggedPressureGauge>();
		SetItem<SoggyMenu>();
		SetItem<WormEatenLampfruitHusk>();
		SetItem<TangledOldRope>();
		SetItem<FadedRitualCloth>();
		SetItem<LeakyCrystalHusk>();
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var restore in restoreContent)
		{
			restore();
		}
		restoreContent.Clear();
		CurrentSubworld.SetValue(null, originalWorld);
		Main.rand = originalRandom;
	}

	[TestMethod]
	public void VanillaTiers_SelectFishWithoutRequiringBass()
	{
		biomeFlags[2] = true;
		Assert.AreEqual(ModContent.ItemType<GrassCarp>(), Catch(new FishingAttempt { common = true }, ItemID.Trout));
		Assert.AreEqual(ModContent.ItemType<Filefish>(), Catch(new FishingAttempt { common = true, uncommon = true }, ItemID.Trout));
		Assert.AreEqual(ModContent.ItemType<Duskfin>(), Catch(new FishingAttempt { common = true, uncommon = true, rare = true }, ItemID.Trout));
	}

	[TestMethod]
	public void ForestFish_UseCachedBiomeFlagsAndTakePriorityOverTownBackground()
	{
		biomeFlags[0] = true;
		biomeFlags[1] = true;
		int fish = Catch(new FishingAttempt { rare = true });
		Assert.IsTrue(fish == ModContent.ItemType<GuidyFish>() || fish == ModContent.ItemType<DimmyFish>());
		biomeFlags[1] = false;
		biomeFlags[2] = true;
		Assert.AreEqual(ModContent.ItemType<Duskfin>(), Catch(new FishingAttempt { rare = true }));
		biomeFlags[2] = false;
		Assert.AreEqual(ItemID.Bass, Catch(new FishingAttempt { rare = true }));
	}

	[TestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	public void Junk_RemainsJunkAndComesFromTheActiveRegion(int biome)
	{
		biomeFlags[biome] = true;
		int[] allowed = biome switch
		{
			0 => [ModContent.ItemType<CloggedPressureGauge>(), ModContent.ItemType<SoggyMenu>()],
			1 => [ModContent.ItemType<WormEatenLampfruitHusk>(), ModContent.ItemType<TangledOldRope>()],
			_ => [ModContent.ItemType<FadedRitualCloth>(), ModContent.ItemType<LeakyCrystalHusk>()],
		};
		foreach (int junk in new[] { ItemID.OldShoe, ItemID.TinCan, ItemID.Seaweed })
		{
			CollectionAssert.Contains(allowed, Catch(new FishingAttempt { common = true }, junk));
		}
	}

	[TestMethod]
	public void ProtectedCatches_AreNotReplaced()
	{
		biomeFlags[2] = true;
		Assert.AreEqual(ItemID.Bass, Catch(new FishingAttempt { common = true, inLava = true }));
		Assert.AreEqual(ItemID.Bass, Catch(new FishingAttempt { common = true, inHoney = true }));
		Assert.AreEqual(ItemID.WoodenCrate, Catch(new FishingAttempt { crate = true, rare = true }, ItemID.WoodenCrate));
		Assert.AreEqual(ItemID.Bass, Catch(new FishingAttempt { rare = true, questFish = ItemID.Bass }));
		Assert.AreEqual(ItemID.ZephyrFish, Catch(new FishingAttempt { uncommon = true, veryrare = true }, ItemID.ZephyrFish));
		Assert.AreEqual(ItemID.GoldenCarp, Catch(new FishingAttempt { rare = true, legendary = true }, ItemID.GoldenCarp));
		Assert.AreEqual(ItemID.Bass, Catch(new FishingAttempt { common = true }, npc: NPCID.ZombieMerman));
		Assert.AreEqual(ItemID.None, Catch(new FishingAttempt { common = true }, ItemID.None));
		Assert.AreEqual(ItemID.Count + 100, Catch(new FishingAttempt { rare = true }, ItemID.Count + 100));
	}

	[TestMethod]
	public void OutsideTargetBiomesOrYggdrasil_LeavesCatchUnchanged()
	{
		Assert.AreEqual(ItemID.Bass, Catch(new FishingAttempt { common = true }));
		biomeFlags[2] = true;
		CurrentSubworld.SetValue(null, null);
		Assert.AreEqual(ItemID.Bass, Catch(new FishingAttempt { rare = true }));
	}

	private int Catch(FishingAttempt attempt, int item = ItemID.Bass, int npc = -1)
	{
		var sonar = new AdvancedPopupRequest();
		var sonarPosition = Vector2.Zero;
		int originalNpc = npc;
		fishingPlayer.CatchFish(attempt, ref item, ref npc, ref sonar, ref sonarPosition);
		Assert.AreEqual(originalNpc, npc);
		return item;
	}

	private void SetItem<T>() where T : ModItem, new()
	{
		SetInstance((T)new T().NewInstance(new Item { type = nextItemType++ }));
	}

	private void SetBiome<T>(int type) where T : ModBiome, new()
	{
		var biome = new T();
		typeof(ModSceneEffect).GetProperty(nameof(ModSceneEffect.Type))!.SetValue(biome, type);
		SetInstance(biome);
	}

	private void SetInstance<T>(T instance) where T : class
	{
		var property = typeof(ContentInstance<T>).GetProperty(nameof(ContentInstance<T>.Instance))!;
		object? original = property.GetValue(null);
		restoreContent.Add(() => property.SetValue(null, original));
		property.SetValue(null, instance);
	}
}
