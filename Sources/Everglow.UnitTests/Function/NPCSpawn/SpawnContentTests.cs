using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.Common.NPCSpawn;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Everglow.Yggdrasil.KelpCurtain.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.NPCSpawn;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function.NPCSpawn;

[TestClass]
[DoNotParallelize]
public class SpawnContentTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	public void RegionalTables_ResolveAll45ContentIdsAndGroupSizes()
	{
		string[] expected =
		[
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.AlgaeOctopus",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.ArmoredShrimp",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.BombJellyfish",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.CannonBarnacle",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.FluorescentHydra",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.GiantTigerShrimp",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.GlowSalamander",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.JadeSpiritAnglerfish",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.LargeAlgaeOctopus",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.LargeBombJellyfish",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.Radiolarian",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.SailfinSnakehead",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.ToxicToad",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.WaterStrider",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.GiantDandelion",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.GuppyConch",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.MossyThornTurtle",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.AnimatedWitherbarkHound",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.AnimatedWitherbarkSoldier",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.AnimatedWitherbarkSoldierRanged",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.AnimatedWitherbarkSoldierSpell",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.BrodieFlydragon",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.CourtCommander",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.SmallBrodieFlydragon",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.AssassinRaspberry",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.LargeMossyThornTurtle",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.RedNeedleCaterpillar",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.SerpentMoss",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.SmallGuppyConch",
			"Everglow.Yggdrasil.KelpCurtain.NPCs.VerdantRods",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.BarkSpicyCaterpillar",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.CarapaceCaterpillar",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.CrimsonSpell",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.CyanOreBeetle",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.DarkGlimmeringRods",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.GiantJellyBall",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.JellyBall",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.LampFruitBorer",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.LeafcutterAnt",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.LightBulb",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.RockElemental",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.ShadowstrikeCaterpillar",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.WhisperingGhost",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.UmbrellaTailThief",
			"Everglow.Yggdrasil.YggdrasilTown.NPCs.PurpleBombCaterpillar",
		];
		var restore = new List<(PropertyInfo Property, object? Value)>();
		try
		{
			// 仅为 ModContent.NPCType<T>() 提供已分配的类型号，不运行 NPC 构造、SetDefaults 或内容加载。
			for (int i = 0; i < expected.Length; i++)
			{
				Type type = typeof(YggdrasilWorld).Assembly.GetType(expected[i], true)!;
				var content = (ModNPC)RuntimeHelpers.GetUninitializedObject(type);
				var npc = (NPC)RuntimeHelpers.GetUninitializedObject(typeof(NPC));
				npc.type = 1000 + i;
				type.GetProperty("Entity")!.SetValue(content, npc);
				var property = typeof(ContentInstance<>).MakeGenericType(type).GetProperty("Instance")!;
				restore.Add((property, property.GetValue(null)));
				property.SetValue(null, content);
			}
			var kelp = new List<SubworldSpawnRule>();
			KelpCurtainSpawnTable.AddRules(kelp);
			var town = new List<SubworldSpawnRule>();
			YggdrasilTownSpawnTable.AddRules(town);
			Assert.AreEqual(30, kelp.Count);
			Assert.AreEqual(22, town.Count);
			var actual = kelp.Concat(town).DistinctBy(rule => rule.Type).OrderBy(rule => rule.Type).ToArray();
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(1000 + i, actual[i].Type, expected[i]);
				Assert.IsNotNull(actual[i].SpawnChance);
				bool shrimp = expected[i].EndsWith(".ArmoredShrimp", StringComparison.Ordinal);
				Assert.AreEqual(shrimp ? 2 : 1, actual[i].MinGroupSize);
				Assert.AreEqual(shrimp ? 5 : 1, actual[i].MaxGroupSize);
			}

			VerifyTownRegionalRules(expected.Select((name, i) => (Name: name.Split('.').Last(), Type: 1000 + i)).ToDictionary(entry => entry.Name, entry => entry.Type));
			VerifyTownSafeArea();
		}
		finally
		{
			foreach (var (property, value) in restore)
			{
				property.SetValue(null, value);
			}
		}
	}

	private static void VerifyTownRegionalRules(Dictionary<string, int> npcTypes)
	{
		var worldField = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
		object? previousWorld = worldField.GetValue(null);
		var previousCenter = YggdrasilTownBiome.BiomeCenter;
		int previousWidth = Main.maxTilesX;
		int previousHeight = Main.maxTilesY;
		int previousNetMode = Main.netMode;
		bool previousDay = Main.dayTime;
		var previousNpcs = Main.npc;
		var instanceProperty = typeof(ContentInstance<SubworldSpawnSystem>).GetProperty("Instance")!;
		object? previousSystem = instanceProperty.GetValue(null);
		var system = new SubworldSpawnSystem();
		var restore = new List<Action>();
		void RegisterTile(Type type, ushort id)
		{
			object content = RuntimeHelpers.GetUninitializedObject(type);
			type.GetProperty("Type")!.SetValue(content, id);
			var property = typeof(ContentInstance<>).MakeGenericType(type).GetProperty("Instance")!;
			object? previous = property.GetValue(null);
			property.SetValue(null, content);
			restore.Add(() => property.SetValue(null, previous));
		}
		try
		{
			RegisterTile(typeof(Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood.DarkForestGrass), 200);
			RegisterTile(typeof(Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.TwilightGrassBlock), 201);
			RegisterTile(typeof(Everglow.Yggdrasil.YggdrasilTown.Tiles.JellyBallSecretion), 202);
			RegisterTile(typeof(Everglow.Yggdrasil.YggdrasilTown.Walls.JellyBallSecretionWall), 200);
			foreach (int type in new[] { 1, 200, 201, 202 })
			{
				bool previous = Main.tileSolid[type];
				Main.tileSolid[type] = true;
				restore.Add(() => Main.tileSolid[type] = previous);
			}
			foreach (int wall in new[] { 0, 200 })
			{
				bool previous = Main.wallHouse[wall];
				Main.wallHouse[wall] = false;
				restore.Add(() => Main.wallHouse[wall] = previous);
			}
			worldField.SetValue(null, new YggdrasilWorld());
			Main.maxTilesX = 2000;
			Main.maxTilesY = 1600;
			Main.netMode = Terraria.ID.NetmodeID.Server;
			YggdrasilTownBiome.BiomeCenter = new Vector2(487, 1300) * 16;
			Main.npc = Enumerable.Range(0, Main.maxNPCs + 1).Select(_ => (NPC)RuntimeHelpers.GetUninitializedObject(typeof(NPC))).ToArray();
			instanceProperty.SetValue(null, system);
			new YggdrasilSpawnRegistration().PostSetupContent();
			var hook = new ManagedSpawnGlobalNPC();
			var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
			void Check(int x, int y, int floorType, bool day, (string Name, float Weight)[] expected, ushort wall = 0, byte liquid = 0)
			{
				// 只借用两个已有图块槽，结束后逐项恢复，不创建世界或重分配 Tilemap。
				Tile floor = Main.tile[x, y];
				Tile above = Main.tile[x, y - 1];
				var oldFloor = (floor.TileType, floor.HasTile, floor.IsActuated);
				var oldAbove = (above.WallType, above.LiquidAmount);
				try
				{
					floor.TileType = (ushort)floorType;
					floor.HasTile = true;
					floor.IsActuated = false;
					above.WallType = wall;
					above.LiquidAmount = liquid;
					Main.dayTime = day;
					player.Center = new Vector2(x * 16 + 8, y * 16 - 32);
					var pool = new Dictionary<int, float> { [0] = 1 };
					hook.EditSpawnPool(pool, new NPCSpawnInfo { Player = player, SpawnTileX = x, SpawnTileY = y, SpawnTileType = floorType });
					Assert.AreEqual(expected.Length, pool.Count, $"At ({x}, {y}), tile {floorType}, day={day}.");
					foreach (var entry in expected)
					{
						Assert.IsTrue(pool.TryGetValue(npcTypes[entry.Name], out float weight), entry.Name);
						Assert.AreEqual(entry.Weight, weight, entry.Name);
					}
				}
				finally
				{
					floor.TileType = oldFloor.TileType;
					floor.HasTile = oldFloor.HasTile;
					floor.IsActuated = oldFloor.IsActuated;
					above.WallType = oldAbove.WallType;
					above.LiquidAmount = oldAbove.LiquidAmount;
				}
			}
			(string, float)[] forestDay = [("BarkSpicyCaterpillar", 2), ("LeafcutterAnt", 1), ("PurpleBombCaterpillar", 6), ("UmbrellaTailThief", 6)];
			(string, float)[] lampDay = [.. forestDay, ("LightBulb", 3), ("LampFruitBorer", 2)];
			(string, float)[] forestNight = [("ShadowstrikeCaterpillar", 2), ("DarkGlimmeringRods", 0.75f), ("WhisperingGhost", 1.5f), ("CrimsonSpell", 0.05f)];
			(string, float)[] shallow = [("CyanOreBeetle", 3), ("JellyBall", 8), ("RockElemental", 0.05f)];
			(string, float)[] deep = [("CyanOreBeetle", 3), ("JellyBall", 3), ("GiantJellyBall", 8), ("CarapaceCaterpillar", 2), ("RockElemental", 0.05f)];
			(string, float)[] hotbed = [("JellyBall", 12), ("GiantJellyBall", 6), ("CyanOreBeetle", 1), ("CarapaceCaterpillar", 1)];
			Check(1500, 800, 200, true, lampDay);
			Check(1500, 800, 201, true, forestDay);
			Check(1500, 800, 200, false, forestNight);
			Check(1500, 800, 201, false, forestNight);
			foreach (bool day in new[] { true, false })
			{
				Check(100, 780, 1, day, shallow);
				Check(100, 1079, 1, day, shallow);
				Check(100, 1080, 1, day, deep);
				Check(100, 1549, 1, day, deep);
				Check(1800, 1300, 202, day, hotbed);
				Check(1800, 1300, 1, day, hotbed, wall: 200);
			}
			Check(100, 779, 1, true, []);
			Check(100, 1550, 1, true, []);
			Check(39, 800, 1, true, []);
			Check(1050, 800, 1, true, []);
			Check(100, 800, 200, true, lampDay);
			Check(100, 800, 202, true, hotbed);
			Check(1500, 800, 200, true, [], liquid: 255);
			Main.wallHouse[200] = true;
			Check(1500, 800, 200, true, [], wall: 200);
			Main.wallHouse[200] = false;
			Main.npc[0].active = true;
			Main.npc[0].type = npcTypes["RockElemental"];
			Check(100, 800, 1, true, shallow.Where(entry => entry.Item1 != "RockElemental").ToArray());
			Main.npc[0].type = npcTypes["CrimsonSpell"];
			Check(1500, 800, 200, false, forestNight.Where(entry => entry.Item1 != "CrimsonSpell").ToArray());
		}
		finally
		{
			system.Unload();
			instanceProperty.SetValue(null, previousSystem);
			foreach (var action in restore)
			{
				action();
			}
			worldField.SetValue(null, previousWorld);
			YggdrasilTownBiome.BiomeCenter = previousCenter;
			Main.maxTilesX = previousWidth;
			Main.maxTilesY = previousHeight;
			Main.netMode = previousNetMode;
			Main.dayTime = previousDay;
			Main.npc = previousNpcs;
		}
	}

	private static void VerifyTownSafeArea()
	{
		var worldField = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
		object? oldWorld = worldField.GetValue(null);
		Vector2 oldCenter = YggdrasilTownBiome.BiomeCenter;
		bool oldCheckedCenter = YggdrasilTownBiome.CheckedBiomeCenter;
		int oldTilesX = Main.maxTilesX;
		int oldNetMode = Main.netMode;
		double oldSurface = Main.worldSurface;
		double oldRockLayer = Main.rockLayer;
		var center = new Vector2(487 * 16 + 8, 20711 * 16 + 8);
		var system = new SubworldSpawnSystem();
		var instanceProperty = typeof(ContentInstance<SubworldSpawnSystem>).GetProperty(nameof(ContentInstance<SubworldSpawnSystem>.Instance))!;
		object? previousInstance = instanceProperty.GetValue(null);
		instanceProperty.SetValue(null, system);
		try
		{
			worldField.SetValue(null, new YggdrasilWorld());
			YggdrasilTownBiome.BiomeCenter = center;
			YggdrasilTownBiome.CheckedBiomeCenter = true;
			// 不加载地形；下方将玩家放在其他地层，普通规则在读取图块前即被地层条件排除。
			Main.maxTilesX = 0;
			new YggdrasilSpawnRegistration().PostSetupContent();
			SubworldSpawnSystem.RegisterRules(YggdrasilSpawnRegistration.WorldId, "safe-area-check",
				[new() { Type = 999 }], YggdrasilSpawnRegistration.InYggdrasilTown);
			var hook = new ManagedSpawnGlobalNPC();
			var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
			var info = new NPCSpawnInfo { Player = player };
			// 现有 TownArea 为相对锚点 (-257, -191)、尺寸 (706, 275)，包含四侧边界格。
			(int X, int Y, bool Inside)[] samples =
			[
				(487, 20711, true),
				(230, 20520, true),
				(936, 20520, true),
				(230, 20795, true),
				(936, 20795, true),
				(229, 20711, false),
				(937, 20730, false),
				(487, 20519, false),
				(487, 20796, false),
				// 镇外轨道走廊，边缘外一格仍保留普通候选。
				(937, 20711, true),
				(1000, 20709, true),
				(1295, 20724, true),
				(1000, 20708, false),
				(1000, 20725, false),
				// 晶塔车站，包括站台；不覆盖下方沼泽。
				(1400, 20700, true),
				(1296, 20666, true),
				(1525, 20723, true),
				(1295, 20666, false),
				(1526, 20700, false),
				(1400, 20665, false),
				(1400, 20724, false),
				(1400, 20750, false),
			];
			foreach (var (x, y, inside) in samples)
			{
				// 玩家在其他地层，真实名单全部退出；用固定候选验证注册区域的落点过滤。
				player.Center = Vector2.Zero;
				info.SpawnTileX = x;
				info.SpawnTileY = y;
				var pool = new Dictionary<int, float> { [0] = 1, [999] = 5 };
				hook.EditSpawnPool(pool, info);
				Assert.AreEqual(inside ? 0 : 1, pool.Count, $"Safe area at ({x}, {y}).");
				if (!inside)
				{
					Assert.AreEqual(1f, pool[999]);
				}

				player.Center = new Vector2(x * 16 + 8, y * 16 + 8);
				int rate = 600;
				int capacity = 8;
				hook.EditSpawnRate(player, ref rate, ref capacity);
				Assert.AreEqual(inside ? 0 : 9, capacity, $"Town player at ({x}, {y}).");
				Assert.AreEqual(inside ? int.MaxValue : 120, rate);
			}

			// 旧椭圆区域以及不同深度的野外都采用相同节奏，不读取原版地表判定。
			foreach (var tilePosition in new[] { new Vector2(600, 11630), new Vector2(750, 11630), new Vector2(900, 11630), new Vector2(1500, 17000) })
			{
				player.Center = tilePosition * 16f;
				int rate = 1800;
				int capacity = 3;
				hook.EditSpawnRate(player, ref rate, ref capacity);
				Assert.AreEqual(120, rate, $"Wilderness at {tilePosition}.");
				Assert.AreEqual(9, capacity);
			}

			// 更换唯一中心后，边界和刷怪规则都应立即跟随，不读取另一份私有缓存。
			var relocatedCenter = new Vector2(700 * 16 + 8, 20400 * 16 + 8);
			YggdrasilTownBiome.BiomeCenter = relocatedCenter;
			Assert.AreEqual(relocatedCenter, YggdrasilTownBiome.GetBiomeCenter(), "Repeated reads must preserve the discovered anchor.");
			Assert.AreEqual(new Rectangle(443, 20209, 706, 275), YggdrasilTownCentralSystem.TownArea);
			player.Center = relocatedCenter;
			int relocatedRate = 600;
			int relocatedCapacity = 8;
			hook.EditSpawnRate(player, ref relocatedRate, ref relocatedCapacity);
			Assert.AreEqual(0, relocatedCapacity);
			player.Center = center;
			relocatedRate = 600;
			relocatedCapacity = 8;
			hook.EditSpawnRate(player, ref relocatedRate, ref relocatedCapacity);
			Assert.AreEqual(9, relocatedCapacity, "The old center must no longer remain a safe area.");

			// 专服不经过客户端 OnEnter/群落绘制；OnLoad 自身必须刷新旧的城镇中心缓存。
			Main.netMode = Terraria.ID.NetmodeID.Server;
			var world = (YggdrasilWorld)SubworldSystem.Current;
			world.OnLoad();
			Assert.IsTrue(YggdrasilTownBiome.CheckedBiomeCenter);
			Assert.AreEqual(center, YggdrasilTownBiome.BiomeCenter, "Without a marker, preserve the existing default center and replace the prior world's cache.");
			new YggdrasilTownBiome().Unload();
			Assert.AreEqual(Vector2.Zero, YggdrasilTownBiome.BiomeCenter);
			Assert.IsFalse(YggdrasilTownBiome.CheckedBiomeCenter);

			worldField.SetValue(null, null);
			var mainWorldPool = new Dictionary<int, float> { [0] = 1, [999] = 5 };
			hook.EditSpawnPool(mainWorldPool, info);
			Assert.AreEqual(2, mainWorldPool.Count, "Town suppression must not affect the main world.");
		}
		finally
		{
			system.Unload();
			instanceProperty.SetValue(null, previousInstance);
			Main.maxTilesX = oldTilesX;
			Main.netMode = oldNetMode;
			Main.worldSurface = oldSurface;
			Main.rockLayer = oldRockLayer;
			YggdrasilTownBiome.BiomeCenter = oldCenter;
			YggdrasilTownBiome.CheckedBiomeCenter = oldCheckedCenter;
			worldField.SetValue(null, oldWorld);
		}
	}

	[TestMethod]
	public void PublicRuleNeedsNeitherNpcInterfaceNorHabitatClassification()
	{
		var assembly = typeof(SubworldSpawnRule).Assembly;
		Assert.IsNull(assembly.GetType("Everglow.Commons.Mechanics.NPCSpawn.IManagedSpawnProvider"));
		Assert.IsNull(assembly.GetType("Everglow.Commons.Mechanics.NPCSpawn.SpawnHabitat"));
		Assert.IsNull(typeof(SubworldSpawnRule).GetProperty("Id"));
		Assert.IsNull(typeof(SubworldSpawnRule).GetProperty("FollowerAi1"));
	}
}
