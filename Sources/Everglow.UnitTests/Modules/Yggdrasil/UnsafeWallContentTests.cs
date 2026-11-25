using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.Common.Items;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class UnsafeWallContentTests
{
	private static readonly (Type SafeWall, Type UnsafeWall, Type Item)[] Variants =
	[
		(typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.DarkForestSoilWall), typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.DarkForestSoilWall_Unsafe), typeof(Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls.DarkForestSoilWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.FemaleLampWoodWall), typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.FemaleLampWoodWall_Unsafe), typeof(Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls.FemaleLampWoodWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.StoneDragonScaleWoodWall), typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.StoneDragonScaleWoodWall_Unsafe), typeof(Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls.StoneDragonScaleWoodWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.LampWood_Wood_Wall), typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.LampWood_Wood_Wall_Unsafe), typeof(Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls.LampWood_Wood_Wall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.TwilightForest.GreenRelicWall), typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.TwilightForest.GreenRelicWall_Unsafe), typeof(Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls.GreenRelicWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.KelpCurtain.Walls.DragonScaleWoodWall), typeof(Everglow.Yggdrasil.KelpCurtain.Walls.DragonScaleWoodWall_Unsafe), typeof(Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls.DragonScaleWoodWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.KelpCurtain.Walls.OldMossWall), typeof(Everglow.Yggdrasil.KelpCurtain.Walls.OldMossWall_Unsafe), typeof(Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls.OldMossWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.KelpCurtain.Walls.MossProneSandSoilWall), typeof(Everglow.Yggdrasil.KelpCurtain.Walls.MossProneSandSoilWall_Unsafe), typeof(Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls.MossProneSandSoilWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.KelpCurtain.Walls.DarkLakeBottomMudWall), typeof(Everglow.Yggdrasil.KelpCurtain.Walls.DarkLakeBottomMudWall_Unsafe), typeof(Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls.DarkLakeBottomMudWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.KelpCurtain.Walls.DecaySandSoilWall), typeof(Everglow.Yggdrasil.KelpCurtain.Walls.DecaySandSoilWall_Unsafe), typeof(Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls.DecaySandSoilWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.KelpCurtain.Walls.GreenCourtWall), typeof(Everglow.Yggdrasil.KelpCurtain.Walls.GreenCourtWall_Unsafe), typeof(Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls.GreenCourtWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.KelpCurtain.Walls.SharpBarnacleWall), typeof(Everglow.Yggdrasil.KelpCurtain.Walls.SharpBarnacleWall_Unsafe), typeof(Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls.SharpBarnacleWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.KelpCurtain.Walls.WaterErodedGreenBrickWall_Fixed), typeof(Everglow.Yggdrasil.KelpCurtain.Walls.WaterErodedGreenBrickWall_Fixed_Unsafe), typeof(Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls.WaterErodedGreenBrickWall_Fixed_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.Common.Walls.YggdrasilBlackRockWall), typeof(Everglow.Yggdrasil.Common.Walls.YggdrasilBlackRockWall_Unsafe), typeof(Everglow.Yggdrasil.Common.Items.YggdrasilBlackRockWall_Unsafe_Item)),
		(typeof(Everglow.Yggdrasil.Common.Walls.YggdrasilGrayRockWall), typeof(Everglow.Yggdrasil.Common.Walls.YggdrasilGrayRockWall_Unsafe), typeof(Everglow.Yggdrasil.Common.Items.YggdrasilGrayRockWall_Unsafe_Item)),
	];

	private static readonly Type[] DustTypes =
	[
		typeof(Everglow.Yggdrasil.Common.Dusts.YggdrasilBlackRock_Dust),
		typeof(Everglow.Yggdrasil.Common.Dusts.YggdrasilGrayRock_Dust),
		typeof(Everglow.Yggdrasil.KelpCurtain.Dusts.DarkLakeBottomMudDust),
		typeof(Everglow.Yggdrasil.KelpCurtain.Dusts.DecaySandSoil_Dust),
		typeof(Everglow.Yggdrasil.KelpCurtain.Dusts.DragonScaleWoodDust),
		typeof(Everglow.Yggdrasil.KelpCurtain.Dusts.GreenCourtBrickDust),
		typeof(Everglow.Yggdrasil.KelpCurtain.Dusts.MossProneSandSoilDust),
		typeof(Everglow.Yggdrasil.KelpCurtain.Dusts.OldMossDust),
		typeof(Everglow.Yggdrasil.KelpCurtain.Dusts.SharpBarnacle_Dust),
		typeof(Everglow.Yggdrasil.KelpCurtain.Dusts.WaterErodedGreenBrickDust),
		typeof(Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest.GreenRelicBrick_Dust),
	];

	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	public void UnsafeVariantsPreserveSafeHousingAndPlaceTheirOwnWallWithSkullIndicator()
	{
		const ushort safeId = 200;
		const ushort unsafeId = 201;
		const int itemId = 1000;
		bool previousSafe = Main.wallHouse[safeId];
		bool previousUnsafe = Main.wallHouse[unsafeId];
		bool previousIndicator = ItemID.Sets.DrawUnsafeIndicator[itemId];
		bool previousServer = Main.dedServ;
		var mapInitialized = typeof(MapLoader).GetField("initialized", BindingFlags.Static | BindingFlags.NonPublic)!;
		object? previousMapInitialized = mapInitialized.GetValue(null);
		var restore = new List<Action>();
		try
		{
			Main.dedServ = true;
			// 此测试只验证住房/放置契约，不创建地图条目，不加载贴图或启动内容加载。
			mapInitialized.SetValue(null, true);
			foreach (Type type in DustTypes)
			{
				var dust = (ModDust)RuntimeHelpers.GetUninitializedObject(type);
				type.GetProperty(nameof(ModDust.Type))!.SetValue(dust, 1000);
				RegisterInstance(type, dust, restore);
			}
			foreach (var variant in Variants)
			{
				var safe = (ModWall)RuntimeHelpers.GetUninitializedObject(variant.SafeWall);
				var dangerous = (ModWall)RuntimeHelpers.GetUninitializedObject(variant.UnsafeWall);
				typeof(ModWall).GetProperty(nameof(ModWall.Type))!.SetValue(safe, safeId);
				typeof(ModWall).GetProperty(nameof(ModWall.Type))!.SetValue(dangerous, unsafeId);
				RegisterInstance(variant.UnsafeWall, dangerous, restore);
				Main.wallHouse[safeId] = false;
				Main.wallHouse[unsafeId] = true;
				safe.SetStaticDefaults();
				dangerous.SetStaticDefaults();
				Assert.IsTrue(Main.wallHouse[safeId], variant.SafeWall.Name);
				Assert.IsFalse(Main.wallHouse[unsafeId], variant.UnsafeWall.Name);
				Assert.AreEqual(safe.DustType, dangerous.DustType, variant.UnsafeWall.Name);
				Assert.IsTrue(variant.UnsafeWall.IsSubclassOf(variant.SafeWall), "Special framing and wall behavior must be retained.");

				AssertAssetExists(dangerous.Texture);
				bool missingItemTexture = variant.Item == typeof(Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls.DarkForestSoilWall_Unsafe_Item)
					|| variant.Item == typeof(Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls.FemaleLampWoodWall_Unsafe_Item);
				Assert.AreEqual(!missingItemTexture, AutoloadAttribute.GetValue(variant.Item).Value, variant.Item.Name);
				if (missingItemTexture)
				{
					continue;
				}

				var item = (ModItem)RuntimeHelpers.GetUninitializedObject(variant.Item);
				typeof(ModItem).GetProperty("Entity")!.SetValue(item, new Item { type = itemId });
				ItemID.Sets.DrawUnsafeIndicator[itemId] = false;
				item.SetStaticDefaults();
				item.SetDefaults();
				Assert.IsTrue(ItemID.Sets.DrawUnsafeIndicator[itemId], variant.Item.Name);
				Assert.AreEqual((int)unsafeId, item.Item.createWall, variant.Item.Name);
				Assert.IsTrue(item.Item.consumable, variant.Item.Name);

				AssertAssetExists(item.Texture);
			}
		}
		finally
		{
			for (int i = restore.Count - 1; i >= 0; i--)
			{
				restore[i]();
			}
			Main.wallHouse[safeId] = previousSafe;
			Main.wallHouse[unsafeId] = previousUnsafe;
			ItemID.Sets.DrawUnsafeIndicator[itemId] = previousIndicator;
			Main.dedServ = previousServer;
			mapInitialized.SetValue(null, previousMapInitialized);
		}
	}

	[TestMethod]
	[DataRow(typeof(YggdrasilBlackRockWall_Item), typeof(YggdrasilBlackRockWall_Unsafe_Item))]
	[DataRow(typeof(YggdrasilGrayRockWall_Item), typeof(YggdrasilGrayRockWall_Unsafe_Item))]
	[DataRow(typeof(DarkLakeBottomMudWall_Item), typeof(DarkLakeBottomMudWall_Unsafe_Item))]
	[DataRow(typeof(DecaySandSoilWall_Item), typeof(DecaySandSoilWall_Unsafe_Item))]
	[DataRow(typeof(DragonScaleWoodWall_Item), typeof(DragonScaleWoodWall_Unsafe_Item))]
	[DataRow(typeof(GreenCourtWall_Item), typeof(GreenCourtWall_Unsafe_Item))]
	[DataRow(typeof(MossProneSandSoilWall_Item), typeof(MossProneSandSoilWall_Unsafe_Item))]
	[DataRow(typeof(OldMossWall_Item), typeof(OldMossWall_Unsafe_Item))]
	[DataRow(typeof(SharpBarnacleWall_Item), typeof(SharpBarnacleWall_Unsafe_Item))]
	[DataRow(typeof(WaterErodedGreenBrickWall_Item), typeof(WaterErodedGreenBrickWall_Fixed_Unsafe_Item))]
	[DataRow(typeof(GreenRelicWall_Item), typeof(GreenRelicWall_Unsafe_Item))]
	[DataRow(typeof(LampWoodWall_Item), typeof(LampWood_Wood_Wall_Unsafe_Item))]
	[DataRow(typeof(StoneDragonScaleWoodWall_Item), typeof(StoneDragonScaleWoodWall_Unsafe_Item))]
	public void SafeWallItemShimmersIntoItsUnsafeVariant(Type safeType, Type unsafeType)
	{
		const int safeId = 1000;
		const int unsafeId = 1001;
		var safeItem = (ModItem)RuntimeHelpers.GetUninitializedObject(safeType);
		var unsafeItem = (ModItem)RuntimeHelpers.GetUninitializedObject(unsafeType);
		var entity = typeof(ModItem).GetProperty("Entity")!;
		entity.SetValue(safeItem, new Item { type = safeId });
		entity.SetValue(unsafeItem, new Item { type = unsafeId });
		int previousTarget = ItemID.Sets.ShimmerTransformToItem[safeId];
		var restore = new List<Action>();
		try
		{
			RegisterInstance(unsafeType, unsafeItem, restore);
			ItemID.Sets.ShimmerTransformToItem[safeId] = -1;
			safeItem.SetStaticDefaults();
			Assert.AreEqual(unsafeId, ItemID.Sets.ShimmerTransformToItem[safeId]);
		}
		finally
		{
			ItemID.Sets.ShimmerTransformToItem[safeId] = previousTarget;
			foreach (var action in restore)
			{
				action();
			}
		}
	}

	private static void RegisterInstance(Type type, object instance, List<Action> restore)
	{
		var property = typeof(ContentInstance<>).MakeGenericType(type).GetProperty("Instance")!;
		object? previous = property.GetValue(null);
		property.SetValue(null, instance);
		restore.Add(() => property.SetValue(null, previous));
	}

	private static void AssertAssetExists(string texture)
	{
		const string prefix = "Everglow/Yggdrasil/";
		Assert.IsTrue(texture.StartsWith(prefix, StringComparison.Ordinal), texture);
		string path = Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "Yggdrasil", texture[prefix.Length..] + ".png"));
		Assert.IsTrue(File.Exists(path), $"Missing reused texture: {texture}");
	}
}
