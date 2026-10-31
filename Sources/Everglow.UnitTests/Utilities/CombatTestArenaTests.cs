using System.Reflection;
using Everglow.Commons.FeatureFlags;
using Everglow.Commons.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Utilities;

[TestClass]
[DoNotParallelize]
public class CombatTestArenaTests
{
	private readonly List<Action> restore = new();
	private EverglowConfig config = null!;
	private static readonly Rectangle Area = new(40, 30, 160, 60);

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		int width = Main.maxTilesX;
		int height = Main.maxTilesY;
		var chests = Main.chest;
		bool generating = WorldGen.gen;
		bool server = Main.dedServ;
		var instance = typeof(ContentInstance<EverglowConfig>).GetProperty("Instance")!;
		var oldConfig = instance.GetValue(null);
		restore.Add(() =>
		{
			Main.maxTilesX = width;
			Main.maxTilesY = height;
			Main.chest = chests;
			WorldGen.gen = generating;
			Main.dedServ = server;
			instance.SetValue(null, oldConfig);
		});
		WorldGen.gen = true;
		Main.dedServ = true;
		Main.maxTilesX = 256;
		Main.maxTilesY = 128;
		Main.chest = new Chest[Main.maxChests];
		config = new EverglowConfig();
		instance.SetValue(null, config);
		Preserve<TileTypeData>();
		Preserve<WallTypeData>();
		Preserve<LiquidData>();
		Preserve<TileWallWireStateData>();
		Preserve<TileWallBrightnessInvisibilityData>();
		for (int x = 20; x < 235; x++)
		{
			for (int y = 20; y < 108; y++)
			{
				var tile = Main.tile[x, y];
				tile.HasTile = true;
				tile.TileType = TileID.Stone;
				tile.WallType = WallID.Stone;
				tile.LiquidAmount = 100;
			}
		}
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var action in Enumerable.Reverse(restore))
		{
			action();
		}
	}

	private void Preserve<T>()
		where T : unmanaged, ITileData
	{
		var data = Main.tile.GetData<T>();
		int stride = Main.tile.Height;
		var snapshot = new T[256 * 128];
		for (int x = 0; x < 256; x++)
		{
			Array.Copy(data, x * stride, snapshot, x * 128, 128);
			Array.Clear(data, x * stride, 128);
		}
		restore.Add(() =>
		{
			for (int x = 0; x < 256; x++)
			{
				Array.Copy(snapshot, x * 128, data, x * stride, 128);
			}
		});
	}

	private bool Build(bool enabled, Rectangle? area = null, Rectangle protectedArea = default)
	{
		var field = typeof(EverglowConfig).GetField("combatTestArenas");
		Assert.IsNotNull(field, "The opt-in arena setting must exist.");
		field.SetValue(config, enabled);
		var method = typeof(TileUtils).GetMethod("PrepareCombatTestArena");
		Assert.IsNotNull(method);
		return (bool)method.Invoke(null, new object[] { area ?? Area, protectedArea })!;
	}

	[TestMethod]
	public void DisabledSettingLeavesEveryTileUnchanged()
	{
		Assert.IsFalse(Build(false));
		for (int x = Area.Left; x < Area.Right; x++)
		{
			for (int y = Area.Top; y < Area.Bottom; y++)
			{
				var tile = Main.tile[x, y];
				Assert.IsTrue(tile.HasTile);
				Assert.AreEqual((ushort)TileID.Stone, tile.TileType);
				Assert.AreEqual((byte)100, tile.LiquidAmount);
			}
		}
	}

	[TestMethod]
	public void EnabledSettingFillsTheArenaWidthWithPlatformsEveryTwentyTiles()
	{
		Assert.IsTrue(Build(true));
		int platforms = 0;
		for (int x = Area.Left; x < Area.Right; x++)
		{
			for (int y = Area.Top; y < Area.Bottom; y++)
			{
				var tile = Main.tile[x, y];
				bool platform = y == 49 || y == 69 || y == 89;
				Assert.AreEqual(platform, tile.HasTile, $"{x}, {y}");
				Assert.AreEqual((ushort)WallID.Stone, tile.WallType);
				Assert.AreEqual((byte)0, tile.LiquidAmount);
				if (platform)
				{
					Assert.AreEqual((ushort)TileID.Platforms, tile.TileType);
					platforms++;
				}
			}
		}
		Assert.AreEqual(480, platforms);
		Assert.IsTrue(Main.tile[39, 40].HasTile);
		Assert.AreEqual((byte)100, Main.tile[39, 40].LiquidAmount);
	}

	[TestMethod]
	public void PreservesSealAndChestSupport()
	{
		Main.chest[0] = new Chest { x = 80, y = 65 };
		Assert.IsTrue(Build(true, protectedArea: new Rectangle(110, 80, 20, 10)));
		Assert.AreEqual((ushort)TileID.Stone, Main.tile[115, 81].TileType);
		Assert.IsTrue(Main.tile[115, 81].HasTile);
		Assert.AreEqual((byte)100, Main.tile[80, 65].LiquidAmount);
		Assert.IsTrue(Main.tile[81, 67].HasTile);
		Assert.IsFalse(Main.tile[85, 66].HasTile);
	}

	[TestMethod]
	public void PreservesMultiTileObjectCrossingTheArenaEdge()
	{
		var dataField = typeof(Terraria.ObjectData.TileObjectData).GetField("_data", BindingFlags.Static | BindingFlags.NonPublic)!;
		var oldData = dataField.GetValue(null);
		bool important = Main.tileFrameImportant[TileID.WorkBenches];
		restore.Add(() =>
		{
			dataField.SetValue(null, oldData);
			Main.tileFrameImportant[TileID.WorkBenches] = important;
		});
		var data = new Terraria.ObjectData.TileObjectData();
		typeof(Terraria.ObjectData.TileObjectData).GetMethod("SetupBaseObject", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(data, null);
		data.Width = 2;
		data.Height = 1;
		data.CoordinateHeights = [16];
		data.CoordinateWidth = 16;
		data.CoordinatePadding = 2;
		var entries = Enumerable.Repeat<Terraria.ObjectData.TileObjectData>(null!, TileID.Count).ToList();
		entries[TileID.WorkBenches] = data;
		dataField.SetValue(null, entries);
		Main.tileFrameImportant[TileID.WorkBenches] = true;
		for (int x = 39; x <= 40; x++)
		{
			var tile = Main.tile[x, 60];
			tile.TileType = TileID.WorkBenches;
			tile.TileFrameX = (short)((x - 39) * 18);
		}
		Assert.IsTrue(Build(true));
		for (int x = 39; x <= 40; x++)
		{
			Assert.IsTrue(Main.tile[x, 60].HasTile);
			Assert.AreEqual((ushort)TileID.WorkBenches, Main.tile[x, 60].TileType);
			Assert.IsTrue(Main.tile[x, 61].HasTile);
		}
		Assert.IsFalse(Main.tile[45, 60].HasTile);
	}

	[TestMethod]
	public void PreservesBuildingThatExtendsBeyondClearingBounds()
	{
		var building = new Rectangle(180, 50, 40, 50);
		Assert.IsTrue(Build(true, protectedArea: building));
		for (int x = building.Left; x < building.Right; x++)
		{
			for (int y = building.Top; y < building.Bottom; y++)
			{
				Assert.IsTrue(Main.tile[x, y].HasTile);
				Assert.AreEqual((ushort)TileID.Stone, Main.tile[x, y].TileType);
				Assert.AreEqual((byte)100, Main.tile[x, y].LiquidAmount);
			}
		}
	}

	[TestMethod]
	public void OutOfBoundsAreaIsRejectedWithoutPartialClearing()
	{
		Assert.IsFalse(Build(true, new Rectangle(-10, 30, 160, 60)));
		Assert.IsTrue(Main.tile[50, 50].HasTile);
		Assert.AreEqual((byte)100, Main.tile[50, 50].LiquidAmount);
	}
}
