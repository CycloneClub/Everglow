using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Commons.CustomTiles;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.CustomTiles;
using Everglow.Yggdrasil.YggdrasilTown.Travelers;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.OriginPylon;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.RandomNPC;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TravelerTownPlanningTests
{
	private readonly List<Action> restore = [];
	private static Type PlacesType => typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerPlaces")!;
	private static Type PlaceType => typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerPlace")!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		var oldTiles = Main.tile;
		int oldWidth = Main.maxTilesX;
		int oldHeight = Main.maxTilesY;
		var oldCenter = YggdrasilTownBiome.BiomeCenter;
		var oldNpcs = Main.npc;
		var oldRandom = Main.rand;
		var oldScreen = Main.screenPosition;
		int oldScreenWidth = Main.screenWidth;
		int oldScreenHeight = Main.screenHeight;
		bool solid = Main.tileSolid[TileID.Stone];
		restore.Add(() =>
		{
			Main.tile = oldTiles;
			Main.maxTilesX = oldWidth;
			Main.maxTilesY = oldHeight;
			Main.npc = oldNpcs;
			Main.rand = oldRandom;
			Main.screenPosition = oldScreen;
			Main.screenWidth = oldScreenWidth;
			Main.screenHeight = oldScreenHeight;
			Main.tileSolid[TileID.Stone] = solid;
			YggdrasilTownBiome.BiomeCenter = oldCenter;
		});
		Main.maxTilesX = 256;
		Main.maxTilesY = 128;
		Main.tile = (Tilemap)typeof(Tilemap).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
			null, [typeof(ushort), typeof(ushort)], null)!.Invoke([(ushort)256, (ushort)128]);
		Main.npc = Enumerable.Range(0, Main.maxNPCs).Select(_ => new NPC()).ToArray();
		Main.rand = new UnifiedRandom(42);
		Main.tileSolid[TileID.Stone] = true;
		YggdrasilTownBiome.BiomeCenter = new Vector2(257, 191) * 16;
		for (int x = 5; x < 251; x++)
		{
			var tile = Main.tile[x, 64];
			tile.HasTile = true;
			tile.TileType = TileID.Stone;
		}
		var manager = new ColliderManager();
		typeof(ColliderManager).GetField("rigidbodies", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(manager, new List<RigidEntity>());
		var instance = typeof(ContentInstance<ColliderManager>).GetProperty(nameof(ContentInstance<ColliderManager>.Instance))!;
		var oldManager = instance.GetValue(null);
		restore.Add(() => instance.SetValue(null, oldManager));
		instance.SetValue(null, manager);
		// Use a fresh catalog without registering content or loading graphics.
		var catalog = typeof(TownTravelerSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		foreach (string name in new[] { "doors", "pylons", "stations" })
		{
			var list = (System.Collections.IList)PlacesType.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(catalog)!;
			var snapshot = list.Cast<object>().ToArray();
			list.Clear();
			restore.Add(() => { list.Clear(); foreach (var entry in snapshot) { list.Add(entry); } });
		}
		foreach (string name in new[] { "clock", "planningBudget", "rejectedRoutes" })
		{
			var field = typeof(TownTravelerSystem).GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!;
			var previous = field.GetValue(null);
			restore.Add(() => field.SetValue(null, previous));
		}
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var action in restore.AsEnumerable().Reverse())
		{
			action();
		}
		restore.Clear();
	}

	private static bool Plan(out List<Point>? path, Point? source = null, string? exclude = null, bool departing = false)
	{
		new TownTravelerSystem().PreUpdateNPCs();
		object?[] args = [source ?? new Point(100, 63), null, departing, null, null, exclude];
		bool success = (bool)typeof(TownTravelerSystem).GetMethod("TryPlan", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, args)!;
		path = args[4] as List<Point>;
		return success;
	}

	[TestMethod]
	[DataRow(SlopeType.SlopeDownLeft)]
	[DataRow(SlopeType.SlopeDownRight)]
	public void NativeSlopePlatformStallCanRecoverOntoNearbySolidFloor(SlopeType slope)
	{
		bool oldSolid = Main.tileSolid[TileID.Platforms];
		bool oldTop = Main.tileSolidTop[TileID.Platforms];
		restore.Add(() => { Main.tileSolid[TileID.Platforms] = oldSolid; Main.tileSolidTop[TileID.Platforms] = oldTop; });
		Main.tileSolid[TileID.Platforms] = Main.tileSolidTop[TileID.Platforms] = true;
		var tile = Main.tile[100, 62];
		tile.HasTile = true;
		tile.TileType = TileID.Platforms;
		tile.Slope = slope;
		var npc = new NPC { width = 16, height = 48, noGravity = true, aiStyle = -1 };
		npc.Bottom = new Vector2(1608, 992);
		void Step(bool fall)
		{
			npc.velocity.Y += TravelerNavigation.Gravity;
			npc.oldVelocity = npc.velocity;
			npc.velocity = Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height, fall, fall);
			typeof(NPC).GetMethod("Collision_MoveWhileDry", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(npc, null);
			typeof(NPC).GetMethod("Collision_MoveSlopesAndStairFall", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(npc, [fall]);
		}
		for (int i = 0; i < 30; i++)
		{
			Step(false);
		}
		Assert.IsTrue(npc.Bottom.Y < 1020, "Native slope collision must reproduce the elevated resting position.");
		Assert.AreEqual(0f, npc.velocity.Y);
		var terrain = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerTerrain")!;
		Assert.IsFalse((bool)terrain.GetMethod("Grounded")!.Invoke(null, [npc])!, "The ordinary floor probe does not recognize this slope rest.");
		var recovery = terrain.GetMethod("TryPlatformRecovery")!;
		Assert.IsNotNull(recovery);
		object?[] args = [npc, 0f];
		Assert.IsTrue((bool)recovery.Invoke(null, args)!);
		Assert.AreEqual(1024f, args[1]);
		for (int i = 0; i < 40; i++)
		{
			Step(true);
		}
		Assert.AreEqual(1024f, npc.Bottom.Y, 0.1f);
		Assert.IsTrue((bool)terrain.GetMethod("Grounded")!.Invoke(null, [npc])!);
	}

	[TestMethod]
	[DataRow(SlopeType.SlopeDownLeft)]
	[DataRow(SlopeType.SlopeDownRight)]
	public void SlopeRecoveryLandsOnFirstFlatPlatformWithoutFallingThrough(SlopeType slope)
	{
		bool oldSolid = Main.tileSolid[TileID.Platforms];
		bool oldTop = Main.tileSolidTop[TileID.Platforms];
		restore.Add(() => { Main.tileSolid[TileID.Platforms] = oldSolid; Main.tileSolidTop[TileID.Platforms] = oldTop; });
		Main.tileSolid[TileID.Platforms] = Main.tileSolidTop[TileID.Platforms] = true;
		for (int y = 62; y <= 63; y++)
		{
			var tile = Main.tile[100, y];
			tile.HasTile = true;
			tile.TileType = TileID.Platforms;
			tile.Slope = y == 62 ? slope : SlopeType.Solid;
		}
		var npc = new NPC { width = 16, height = 48 };
		npc.Bottom = new Vector2(1608, 992);
		var terrain = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerTerrain")!;
		object?[] args = [npc, 0f];
		Assert.IsTrue((bool)terrain.GetMethod("TryPlatformRecovery")!.Invoke(null, args)!);
		Assert.AreEqual(1008f, args[1], "Recovery must select the first safe flat platform, not the deeper stone floor.");
		var type = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerJourney")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		type.GetField("<Body>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, npc);
		type.GetField("recoveringPlatform", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, true);
		type.GetField("dropping", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, true);
		type.GetField("dropSourceY", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, args[1]);
		for (int i = 0; i < 40; i++)
		{
			npc.velocity.Y += TravelerNavigation.Gravity;
			bool fall = (bool)type.GetProperty("FallThroughPlatforms")!.GetValue(journey)!;
			npc.oldVelocity = npc.velocity;
			npc.velocity = Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height, fall, fall);
			typeof(NPC).GetMethod("Collision_MoveWhileDry", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(npc, null);
			typeof(NPC).GetMethod("Collision_MoveSlopesAndStairFall", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(npc, [fall]);
			Assert.IsTrue(npc.Bottom.Y <= 1008.1f, "Recovery must never pass its chosen platform.");
		}
		Assert.AreEqual(1008f, npc.Bottom.Y, 0.1f);
	}

	[TestMethod]
	[DataRow("deep")]
	[DataRow("lava")]
	[DataRow("partial")]
	[DataRow("ascending")]
	[DataRow("air")]
	public void PlatformRecoveryRejectsUnsafeOrUnrelatedDrops(string condition)
	{
		bool oldSolid = Main.tileSolid[TileID.Platforms];
		bool oldTop = Main.tileSolidTop[TileID.Platforms];
		restore.Add(() => { Main.tileSolid[TileID.Platforms] = oldSolid; Main.tileSolidTop[TileID.Platforms] = oldTop; });
		Main.tileSolid[TileID.Platforms] = Main.tileSolidTop[TileID.Platforms] = true;
		var slope = Main.tile[100, 62];
		slope.HasTile = condition != "air";
		slope.TileType = TileID.Platforms;
		slope.Slope = SlopeType.SlopeDownLeft;
		var npc = new NPC { width = 16, height = 48 };
		npc.Bottom = new Vector2(condition == "partial" ? 1610 : 1608, 992);
		if (condition == "deep")
		{
			var floor = Main.tile[100, 64];
			floor.HasTile = false;
		}
		if (condition == "partial")
		{
			var floor = Main.tile[101, 64];
			floor.HasTile = false;
		}
		if (condition == "lava")
		{
			var lava = Main.tile[100, 63];
			lava.LiquidType = LiquidID.Lava;
			lava.LiquidAmount = 255;
		}
		if (condition == "ascending")
		{
			npc.velocity.Y = -1;
		}
		var terrain = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerTerrain")!;
		object?[] args = [npc, 0f];
		Assert.IsFalse((bool)terrain.GetMethod("TryPlatformRecovery")!.Invoke(null, args)!);
	}

	[TestMethod]
	[DataRow(1040f, 1024f)]
	[DataRow(1008f, 1024f)]
	[DataRow(1008f, 1012f)]
	public void RecoveryEndsWhenLandingChangesOrBodyPassesTarget(float targetY, float feetY)
	{
		var current = typeof(SubworldLibrary.SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
		var oldWorld = current.GetValue(null);
		var oldMode = TownTravelerSystem.Mode;
		int oldNetMode = Main.netMode;
		bool oldServer = Main.dedServ;
		restore.Add(() =>
		{
			current.SetValue(null, oldWorld);
			typeof(TownTravelerSystem).GetProperty("Mode")!.SetValue(null, oldMode);
			Main.netMode = oldNetMode;
			Main.dedServ = oldServer;
		});
		current.SetValue(null, new Everglow.Yggdrasil.YggdrasilWorld());
		typeof(TownTravelerSystem).GetProperty("Mode")!.SetValue(null, TravelerMode.NPC);
		Main.netMode = NetmodeID.SinglePlayer;
		Main.dedServ = false;
		var type = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerJourney")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string name, object value) => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		var npc = new NPC { width = 16, height = 48 };
		npc.Bottom = new Vector2(1608, feetY);
		Set("<Body>k__BackingField", npc);
		Set("<Active>k__BackingField", true);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Planning"));
		Set("recoveringPlatform", true);
		Set("dropping", true);
		Set("dropSourceY", targetY);
		Set("recentPlaces", new Queue<string>());
		// Force a deferred plan so this tests the recovery transition, not destination selection.
		typeof(TownTravelerSystem).GetField("planningBudget", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, 0);
		type.GetMethod("Update")!.Invoke(journey, null);
		Assert.IsFalse((bool)type.GetField("recoveringPlatform", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(journey)!);
		Assert.IsFalse((bool)type.GetProperty("FallThroughPlatforms")!.GetValue(journey)!);
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(1)]
	public void GroundedLedgeRecoveryReturnsToAdjacentSupportedCell(int direction)
	{
		var floor = Main.tile[100, 64];
		floor.HasTile = false;
		var npc = new NPC { width = 16, height = 48 };
		npc.Bottom = new Vector2(1608 + direction * 0.003f, 1024);
		var terrain = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerTerrain")!;
		Assert.IsTrue((bool)terrain.GetMethod("Grounded")!.Invoke(null, [npc])!);
		var recover = terrain.GetMethod("TryStandingRecovery");
		Assert.IsNotNull(recover);
		object?[] args = [npc, 0f];
		Assert.IsTrue((bool)recover.Invoke(null, args)!);
		Assert.AreEqual(1608f + direction * 16, args[1]);
		for (int i = 0; i < 16; i++)
		{
			npc.velocity = new Vector2(Math.Clamp((float)args[1]! - npc.Bottom.X, -1.5f, 1.5f), TravelerNavigation.Gravity);
			npc.velocity = Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height);
			npc.position += npc.velocity;
		}
		Assert.AreEqual(1024f, npc.Bottom.Y);
		Assert.AreEqual((float)args[1]!, npc.Bottom.X);
	}

	[TestMethod]
	[DataRow("air")]
	[DataRow("deep")]
	[DataRow("hazard")]
	public void StandingRecoveryRejectsUnsupportedOrUnsafeNudges(string condition)
	{
		for (int x = 99; x <= 101; x++)
		{
			var floor = Main.tile[x, 64];
			floor.HasTile = x == 99 && condition != "deep";
		}
		var npc = new NPC { width = 16, height = 48 };
		npc.Bottom = new Vector2(1607.997f, condition == "air" ? 1020 : 1024);
		if (condition == "hazard")
		{
			var lava = Main.tile[99, 63];
			lava.LiquidType = LiquidID.Lava;
			lava.LiquidAmount = 255;
		}
		var terrain = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerTerrain")!;
		object?[] args = [npc, 0f];
		Assert.IsFalse((bool)terrain.GetMethod("TryStandingRecovery")!.Invoke(null, args)!);
	}

	[TestMethod]
	public void PlannerAvoidsAStandingPositionReservedByAnotherActivity()
	{
		Main.rand = new UnifiedRandom(42);
		Assert.IsTrue(Plan(out var firstPath));
		var cell = firstPath![^1];
		var type = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerJourney")!;
		var resident = RuntimeHelpers.GetUninitializedObject(type);
		type.GetProperty("Active")!.SetValue(resident, true);
		type.GetProperty("Stage")!.SetValue(resident, Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Activity"));
		type.GetField("destination", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(resident,
			Activator.CreateInstance(PlaceType, TravelerActivity.Scenery, cell, "different-activity", cell.ToVector2() * 16, null, 0, -1, 0));
		var travelers = (System.Collections.IList)typeof(TownTravelerSystem).GetField("Travelers", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		travelers.Add(resident);
		restore.Add(() => travelers.Remove(resident));
		Main.rand = new UnifiedRandom(42);
		Assert.IsTrue(Plan(out var nextPath));
		Assert.IsTrue(Math.Abs(nextPath![^1].X - cell.X) >= 2 || Math.Abs(nextPath[^1].Y - cell.Y) >= 3,
			"Distinct activity keys must not allow two people to stand on the same ground coordinate.");
	}

	[TestMethod]
	public void SharedActivityUsesDistinctSupportedStandingPositions()
	{
		var catalog = typeof(TownTravelerSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var method = PlacesType.GetMethod("TryStandingPlace")!;
		Assert.IsNotNull(method, "The activity catalog needs safe standing-space selection.");
		var place = Activator.CreateInstance(PlaceType, TravelerActivity.Scenery, new Point(100, 63), "shared", new Vector2(1608, 1024), null, 0, -1, 0)!;
		var reserved = new List<Point> { new(100, 63) };
		bool Occupied(Point cell) => reserved.Any(other => Math.Abs(other.X - cell.X) < 2 && Math.Abs(other.Y - cell.Y) < 3);
		for (int i = 0; i < 2; i++)
		{
			object?[] args = [place, (Func<Point, bool>)Occupied, null];
			Assert.IsTrue((bool)method.Invoke(catalog, args)!);
			var cell = (Point)PlaceType.GetProperty("Cell")!.GetValue(args[2])!;
			Assert.IsFalse(Occupied(cell));
			Assert.AreEqual(63, cell.Y);
			Assert.AreEqual("shared", PlaceType.GetProperty("Key")!.GetValue(args[2]));
			reserved.Add(cell);
		}
	}

	[TestMethod]
	public void StandingAlternativesStayOnTheSameSideOfAnInterveningWall()
	{
		for (int y = 56; y < 64; y++)
		{
			var tile = Main.tile[99, y];
			tile.HasTile = true;
			tile.TileType = TileID.Stone;
		}
		var catalog = typeof(TownTravelerSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var place = Activator.CreateInstance(PlaceType, TravelerActivity.Scenery, new Point(100, 63), "wall-side", new Vector2(1608, 1024), null, 0, -1, 0)!;
		object?[] args = [place, (Func<Point, bool>)(cell => cell == new Point(100, 63)), null];
		Assert.IsTrue((bool)PlacesType.GetMethod("TryStandingPlace")!.Invoke(catalog, args)!);
		Assert.AreEqual(new Point(102, 63), PlaceType.GetProperty("Cell")!.GetValue(args[2]),
			"The free point behind a wall must not hide a reachable point beside the original activity.");
	}

	[TestMethod]
	public void StandingAlternativesCannotUseMissingFloorOrMoveAnExit()
	{
		var catalog = typeof(TownTravelerSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var method = PlacesType.GetMethod("TryStandingPlace")!;
		Assert.IsNotNull(method);
		foreach (var activity in new[] { TravelerActivity.EnterBuilding, TravelerActivity.LeavePylon })
		{
			var exit = Activator.CreateInstance(PlaceType, activity, new Point(100, 63), "fixed", new Vector2(1608, 1024), null, 0, -1, 0)!;
			object?[] exitArgs = [exit, (Func<Point, bool>)(cell => cell == new Point(100, 63)), null];
			Assert.IsFalse((bool)method.Invoke(catalog, exitArgs)!, "A free neighboring floor cannot move an entrance animation.");
		}
		for (int x = 94; x <= 106; x++)
		{
			if (x != 100)
			{
				var tile = Main.tile[x, 64];
				tile.HasTile = false;
			}
		}
		var place = Activator.CreateInstance(PlaceType, TravelerActivity.Scenery, new Point(100, 63), "fixed", new Vector2(1608, 1024), null, 0, -1, 0)!;
		object?[] args = [place, (Func<Point, bool>)(cell => cell == new Point(100, 63)), null];
		Assert.IsFalse((bool)method.Invoke(catalog, args)!);
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(1)]
	public void WaitingPositionsExpandAwayFromTheTrain(int side)
	{
		var catalog = typeof(TownTravelerSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var method = PlacesType.GetMethod("TryStandingPlace")!;
		Assert.IsNotNull(method);
		var place = Activator.CreateInstance(PlaceType, TravelerActivity.BoardTrain, new Point(100, 63), "station", new Vector2(1608 - side * 912, 1040), null, 0, -1, 0)!;
		object?[] args = [place, (Func<Point, bool>)(cell => cell == new Point(100, 63)), null];
		Assert.IsTrue((bool)method.Invoke(catalog, args)!);
		var cell = (Point)PlaceType.GetProperty("Cell")!.GetValue(args[2])!;
		Assert.IsTrue((cell.X - 100) * side >= 2, "Waiting space must not expand inward onto the train deck.");
	}

	[TestMethod]
	public void ActualTownPlannerWorksWithoutStationAndChoosesBothDirections()
	{
		bool left = false;
		bool right = false;
		for (int seed = 0; seed < 24; seed++)
		{
			Main.rand = new UnifiedRandom(seed);
			if (Plan(out var path))
			{
				Assert.IsNotNull(path);
				Assert.IsTrue(path.Count > 1);
				Assert.IsTrue(path.All(point => point.Y == 63));
				left |= path[^1].X < 100;
				right |= path[^1].X > 100;
			}
		}
		Assert.IsTrue(left && right, "Town activities must be reachable in both directions without any station or pylon.");
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void OrdinaryActivityCannotOccupyTrainDeck(bool moving)
	{
		var train = new YggdrasilTown_NormalMetro
		{
			Position = new Vector2(1200, 1008), Size = new Vector2(600, 16), Active = true,
			State = (int)(moving ? YggdrasilTown_NormalMetro.BehaviorType.Move : YggdrasilTown_NormalMetro.BehaviorType.Stop),
			Velocity = moving ? Vector2.UnitX : Vector2.Zero,
		};
		((List<RigidEntity>)typeof(ColliderManager).GetField("rigidbodies", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(ColliderManager.Instance)!).Add(train);
		var terrain = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerTerrain")!;
		var stand = terrain.GetMethod("CanStand", [typeof(Point)])!;
		Assert.IsFalse((bool)stand.Invoke(null, [new Point(100, 63)])!, "Normal activities must not overlap the solid deck even when it is not a walkable support.");
		if (!moving)
		{
			var boardingStand = terrain.GetMethod("CanStand", [typeof(Point), typeof(YggdrasilTown_NormalMetro)])!;
			Assert.IsTrue((bool)boardingStand.Invoke(null, [new Point(100, 62), train])!);
		}
	}

	[TestMethod]
	[DataRow(1440)]
	[DataRow(1342)]
	public void TravelerOnTrainRemainsVisibleIncludingAtDoor(int x)
	{
		Main.screenPosition = new Vector2(400, 0);
		Main.screenWidth = 2400;
		Main.screenHeight = 1400;
		var type = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerJourney")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var body = new NPC { width = 16, height = 48, position = new Vector2(x, 960) };
		var train = new YggdrasilTown_NormalMetro { Position = new Vector2(1200, 1008), Size = new Vector2(600, 16), Active = true };
		var view = new VFXPerson();
		type.GetField("<Body>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, body);
		type.GetField("train", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, train);
		type.GetField("view", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, view);
		type.GetField("aboardOffset", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, body.position - train.Position);
		type.GetField("stageTime", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, 60);
		type.GetMethod("Ride", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, null);
		Assert.AreEqual(1f, view.Opacity, "Passengers must remain visible throughout their ride, including at a door.");
	}

	[TestMethod]
	[DataRow("Riding", 1400, 960, true)]
	[DataRow("Boarding", 1400, 960, true)]
	[DataRow("Alighting", 1400, 960, true)]
	[DataRow("Boarding", 1170, 960, false)]
	[DataRow("Alighting", 1810, 960, false)]
	[DataRow("Boarding", 1400, 800, false)]
	[DataRow("Alighting", 1400, 1040, false)]
	[DataRow("Walking", 1400, 960, false)]
	public void PassengerUsesTrainInteriorLayerOnlyWhileInsideItsCarriage(string stage, int x, int y, bool inside)
	{
		var type = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerJourney")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var body = new NPC { width = 16, height = 48, position = new Vector2(x, y) };
		var train = new YggdrasilTown_NormalMetro { Position = new Vector2(1200, 1008), Size = new Vector2(600, 16), Active = true };
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		Set("<Body>k__BackingField", body);
		Set("<Active>k__BackingField", true);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, stage));
		Set("train", train);
		var interior = type.GetProperty("InteriorTrain", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(interior, "Passengers need a carriage-owned draw layer instead of the late foreground VFX layer.");
		foreach (float shellAlpha in new[] { 0f, 0.5f, 1f })
		{
			train.ShellAlpha = shellAlpha;
			Assert.AreEqual(inside ? train : null, interior.GetValue(journey), "The shell must occlude passengers through draw order, not passenger opacity.");
		}
		train.Active = false;
		Assert.IsNull(interior.GetValue(journey));
		train.Active = true;
		Set("<Active>k__BackingField", false);
		Assert.IsNull(interior.GetValue(journey));
	}

	[TestMethod]
	public void ExitRequiresTravelerToActuallyReachEndpoint()
	{
		var tile = Main.tile[40, 60];
		tile.HasTile = true;
		tile.TileType = TileID.Rope;
		var place = Activator.CreateInstance(PlaceType, TravelerActivity.LeavePylon, new Point(42, 63), "pylon", new Vector2(680, 1024), new Point(40, 60), (int)TileID.Rope, -1, 0)!;
		var type = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerJourney")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var body = new NPC { width = 16, height = 48, position = new Vector2(1000, 976) };
		type.GetField("<Body>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, body);
		type.GetField("destination", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, place);
		type.GetMethod("ReachDestination", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, null);
		Assert.AreEqual("Planning", type.GetProperty("Stage")!.GetValue(journey)!.ToString(), "A displaced traveler cannot fade out at an endpoint it has not reached.");
	}

	[TestMethod]
	public void IncomingPassengerSurvivesOffscreenAndTracksFractionalTrainMovement()
	{
		Main.screenPosition = Vector2.Zero;
		Main.screenWidth = 800;
		Main.screenHeight = 600;
		var type = typeof(TownTravelerSystem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerJourney")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var body = new NPC { active = true, width = 16, height = 48 };
		var train = new YggdrasilTown_NormalMetro { Active = true, Position = new Vector2(2000.5f, 1008), Size = new Vector2(1696, 16) };
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		Set("<Body>k__BackingField", body);
		Set("<Active>k__BackingField", true);
		Set("train", train);
		Set("view", new VFXPerson());
		Set("aboardOffset", new Vector2(100, -48));
		var ride = type.GetMethod("Ride", BindingFlags.Instance | BindingFlags.NonPublic)!;
		for (int i = 0; i < 200; i++)
		{
			train.Position += new Vector2(0.25f, 0);
			ride.Invoke(journey, null);
		}
		Assert.IsTrue((bool)type.GetProperty("Active")!.GetValue(journey)!);
		Assert.AreEqual(train.Position + new Vector2(100, -48), body.position);
		train.State = (int)YggdrasilTown_NormalMetro.BehaviorType.Stop;
		ride.Invoke(journey, null);
		train.State = (int)YggdrasilTown_NormalMetro.BehaviorType.Move;
		for (int i = 0; i < 180; i++)
		{
			ride.Invoke(journey, null);
		}
		Assert.IsFalse((bool)type.GetProperty("Active")!.GetValue(journey)!);
	}

	[TestMethod]
	public void LightweightPassengerDoesNotApplyTrainDisplacementTwiceAfterRiding()
	{
		var assembly = typeof(TownTravelerSystem).Assembly;
		var type = assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerJourney")!;
		var actorType = assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Travelers.TravelerActor")!;
		var actor = Activator.CreateInstance(actorType, true)!;
		var body = (Entity)actorType.GetProperty("Body")!.GetValue(actor)!;
		body.position = new Vector2(1342, 960);
		var train = new YggdrasilTown_NormalMetro { Active = true, Position = new Vector2(1200, 1008), Size = new Vector2(1696, 16) };
		actorType.GetField("ground", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, train);
		actorType.GetField("groundPosition", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, train.Position);
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		Set("<Body>k__BackingField", body);
		Set("<Actor>k__BackingField", actor);
		Set("train", train);
		Set("view", new VFXPerson());
		type.GetMethod("AttachToTrain", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, [false]);
		train.Position += new Vector2(400, 0);
		type.GetMethod("Ride", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, null);
		Vector2 aboard = body.position;
		actorType.GetMethod("Move")!.Invoke(actor, [false]);
		Assert.AreEqual(aboard, body.position, "The first detached movement must not replay the train's entire journey.");
	}

	[TestMethod]
	public void DepartureCanReachBeyondWideViewportInsteadOfWaitingForever()
	{
		Main.screenPosition = new Vector2(408, 0);
		Main.screenWidth = 2400;
		Main.screenHeight = 1400;
		bool found = false;
		for (int i = 0; i < 8; i++)
		{
			if (Plan(out var path, departing: true))
			{
				found = true;
				Assert.IsNotNull(path);
				int x = path[^1].X * 16 + 8;
				Assert.IsTrue(x < 248 || x > 2968);
			}
		}
		Assert.IsTrue(found);
	}

	[TestMethod]
	public void EnclosedTravelerDoesNotPlanThroughWallsToReachActivity()
	{
		foreach (int x in new[] { 97, 103 })
		{
			for (int y = 5; y < 64; y++)
			{
				var tile = Main.tile[x, y];
				tile.HasTile = true;
				tile.TileType = TileID.Stone;
			}
		}
		Assert.IsFalse(Plan(out _));
	}

	[TestMethod]
	public void EntranceCannotRemainValidAfterItsApproachFloorIsRemoved()
	{
		var catalog = Activator.CreateInstance(PlacesType, true)!;
		var origin = new Point(40, 60);
		((Dictionary<int, Point>)PlacesType.GetField("doorSizes", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(catalog)!).Add(TileID.Rope, new Point(5, 4));
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 4; y++)
			{
				var tile = Main.tile[40 + x, 60 + y];
				tile.HasTile = true;
				tile.TileType = TileID.Rope;
				tile.TileFrameX = (short)(x * 18);
				tile.TileFrameY = (short)(y * 18);
			}
		}
		var place = Activator.CreateInstance(PlaceType, TravelerActivity.EnterBuilding, new Point(42, 63), "door", new Vector2(680, 1024), origin, (int)TileID.Rope, -1, 0)!;
		var valid = PlacesType.GetMethod("Valid", BindingFlags.Instance | BindingFlags.Public)!;
		Assert.IsTrue((bool)valid.Invoke(catalog, [place])!);
		var floor = Main.tile[42, 64];
		floor.HasTile = false;
		Assert.IsFalse((bool)valid.Invoke(catalog, [place])!, "A surviving door tile cannot authorize fading out above a removed floor.");
		floor.HasTile = true;
		var doorPart = Main.tile[44, 63];
		doorPart.HasTile = false;
		Assert.IsFalse((bool)valid.Invoke(catalog, [place])!, "An incomplete multi-tile door must be rejected.");
	}

	[TestMethod]
	[DataRow("clear", true)]
	[DataRow("wall", false)]
	[DataRow("talking", false)]
	[DataRow("fighting", false)]
	[DataRow("moving", false)]
	public void SocialActivityRequiresAnAvailableVisibleResident(string condition, bool expected)
	{
		var resident = (Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs.InnKeeper)RuntimeHelpers.GetUninitializedObject(
			typeof(Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs.InnKeeper));
		resident.Talking = condition == "talking";
		resident.ArenaFighting = condition == "fighting";
		var npc = Main.npc[0];
		npc.active = npc.friendly = true;
		npc.type = 500;
		npc.width = 18;
		npc.height = 40;
		npc.Bottom = new Vector2(1672, 1024);
		npc.velocity.X = condition == "moving" ? 2 : 0;
		typeof(NPC).GetProperty(nameof(NPC.ModNPC))!.SetValue(npc, resident);
		if (condition == "wall")
		{
			for (int y = 58; y < 64; y++)
			{
				var tile = Main.tile[102, y];
				tile.HasTile = true;
				tile.TileType = TileID.Stone;
			}
		}
		var catalog = Activator.CreateInstance(PlacesType, true)!;
		var place = Activator.CreateInstance(PlaceType, TravelerActivity.Socialize, new Point(100, 63), "npc:0:500",
			npc.Bottom, null, 0, 0, npc.type)!;
		var valid = PlacesType.GetMethod("Valid", BindingFlags.Instance | BindingFlags.Public)!;
		Assert.AreEqual(expected, (bool)valid.Invoke(catalog, [place])!);
	}

	[TestMethod]
	public void PylonPlazaOffersLocalActivityBeforeDepartingForTown()
	{
		YggdrasilTownBiome.BiomeCenter += new Vector2(10000, 0);
		var pylonInstance = typeof(ContentInstance<OriginPylon>).GetProperty(nameof(ContentInstance<OriginPylon>.Instance))!;
		var previous = pylonInstance.GetValue(null);
		restore.Add(() => pylonInstance.SetValue(null, previous));
		var pylon = new OriginPylon();
		typeof(ModTile).GetProperty(nameof(ModTile.Type))!.SetValue(pylon, (ushort)TileID.Rope);
		pylonInstance.SetValue(null, pylon);
		var origin = new Point(100, 50);
		var tile = Main.tile[100, 50];
		tile.HasTile = true;
		tile.TileType = TileID.Rope;
		var catalog = typeof(TownTravelerSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var list = (System.Collections.IList)PlacesType.GetField("pylons", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(catalog)!;
		var markerType = PlacesType.GetNestedType("Landmark", BindingFlags.NonPublic)!;
		list.Add(Activator.CreateInstance(markerType, origin, (int)TileID.Rope, 1, 1)!);
		bool found = false;
		for (int i = 0; i < 8; i++)
		{
			found |= Plan(out _, new Point(104, 63), $"tile:{origin}");
		}
		Assert.IsTrue(found, "A pylon arrival must have a safe plaza activity even when the town is elsewhere.");
	}
}
