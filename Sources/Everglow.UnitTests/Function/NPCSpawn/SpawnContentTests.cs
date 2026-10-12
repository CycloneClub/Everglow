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
	public void RegionalTables_ResolveAll45ContentIdsAndPreserveWeights()
	{
		(string Name, float Weight)[] expected =
		[
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.AlgaeOctopus", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.ArmoredShrimp", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.BombJellyfish", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.CannonBarnacle", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.FluorescentHydra", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.GiantTigerShrimp", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.GlowSalamander", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.JadeSpiritAnglerfish", 0.35f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.LargeAlgaeOctopus", 0.35f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.LargeBombJellyfish", 0.5f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.Radiolarian", 0.35f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.SailfinSnakehead", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.ToxicToad", 1f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake.WaterStrider", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.GiantDandelion", 0.25f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.GuppyConch", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.MossyThornTurtle", 1.5f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.AnimatedWitherbarkHound", 1f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.AnimatedWitherbarkSoldier", 1.5f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.AnimatedWitherbarkSoldierRanged", 1f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.AnimatedWitherbarkSoldierSpell", 0.75f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.BrodieFlydragon", 1f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.CourtCommander", 0.5f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt.SmallBrodieFlydragon", 0.5f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.AssassinRaspberry", 1f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.LargeMossyThornTurtle", 0.1f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.RedNeedleCaterpillar", 1f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.SerpentMoss", 1f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist.SmallGuppyConch", 1f),
			("Everglow.Yggdrasil.KelpCurtain.NPCs.VerdantRods", 1f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.BarkSpicyCaterpillar", 8f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.CarapaceCaterpillar", 8f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.CrimsonSpell", 0.05f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.CyanOreBeetle", 3f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.DarkGlimmeringRods", 3f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.GiantJellyBall", 3f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.JellyBall", 3f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.LampFruitBorer", 8f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.LeafcutterAnt", 3f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.LightBulb", 3f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.RockElemental", 0.05f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.ShadowstrikeCaterpillar", 8f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.WhisperingGhost", 3f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.UmbrellaTailThief", 3f),
			("Everglow.Yggdrasil.YggdrasilTown.NPCs.PurpleBombCaterpillar", 3f),
		];
		var restore = new List<(PropertyInfo Property, object? Value)>();
		try
		{
			// 仅为 ModContent.NPCType<T>() 提供已分配的类型号，不运行 NPC 构造、SetDefaults 或内容加载。
			for (int i = 0; i < expected.Length; i++)
			{
				Type type = typeof(YggdrasilWorld).Assembly.GetType(expected[i].Name, true)!;
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
			Assert.AreEqual(15, town.Count);
			var actual = kelp.Concat(town).OrderBy(rule => rule.Type).ToArray();
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(1000 + i, actual[i].Type, expected[i].Name);
				Assert.AreEqual(expected[i].Weight, actual[i].Weight, expected[i].Name);
				Assert.IsNotNull(actual[i].Condition);
				bool shrimp = expected[i].Name.EndsWith(".ArmoredShrimp", StringComparison.Ordinal);
				Assert.AreEqual(shrimp ? 2 : 1, actual[i].MinGroupSize);
				Assert.AreEqual(shrimp ? 5 : 1, actual[i].MaxGroupSize);
			}

			// 旧灯木森林条件使用已有的场景图块计数；迁移后仍保留 >150 的阈值。
			var counter = (Everglow.Yggdrasil.Common.YggdrasilBiomeTileCounter)RuntimeHelpers.GetUninitializedObject(
				typeof(Everglow.Yggdrasil.Common.YggdrasilBiomeTileCounter));
			var biome = RuntimeHelpers.GetUninitializedObject(typeof(Everglow.Yggdrasil.YggdrasilTown.Biomes.LampWoodForest));
			foreach (object content in new object[] { counter, biome })
			{
				var property = typeof(ContentInstance<>).MakeGenericType(content.GetType()).GetProperty("Instance")!;
				restore.Add((property, property.GetValue(null)));
				property.SetValue(null, content);
			}
			foreach (var rule in town.Where(rule => rule.Type == ModContent.NPCType<Everglow.Yggdrasil.YggdrasilTown.NPCs.UmbrellaTailThief>() ||
				rule.Type == ModContent.NPCType<Everglow.Yggdrasil.YggdrasilTown.NPCs.PurpleBombCaterpillar>()))
			{
				var info = new NPCSpawnInfo { Player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player)) };
				counter.DarkForestGrassCount = 150;
				Assert.IsFalse(rule.Condition(info));
				counter.DarkForestGrassCount = 151;
				Assert.IsTrue(rule.Condition(info));
				counter.DarkForestGrassCount = 0;
				Assert.IsFalse(rule.Condition(info));
			}
			counter.DarkForestGrassCount = 151;
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
				(937, 20711, false),
				(487, 20519, false),
				(487, 20796, false),
			];
			foreach (var (x, y, inside) in samples)
			{
				// 玩家位于其他地层，保留不依赖地形的灯木森林候选；镇内空池必须来自候选位置过滤。
				player.Center = Vector2.Zero;
				info.SpawnTileX = x;
				info.SpawnTileY = y;
				var pool = new Dictionary<int, float> { [0] = 1, [999] = 5 };
				hook.EditSpawnPool(pool, info);
				Assert.AreEqual(inside ? 0 : 2, pool.Count, $"Town candidate at ({x}, {y}), inside={inside}.");
				if (!inside)
				{
					Assert.IsTrue(pool.Values.All(weight => weight == 3f));
				}

				player.Center = new Vector2(x * 16 + 8, y * 16 + 8);
				int rate = 600;
				int capacity = 8;
				hook.EditSpawnRate(player, ref rate, ref capacity);
				Assert.AreEqual(inside ? 0 : 8, capacity, $"Town player at ({x}, {y}).");
				Assert.AreEqual(inside ? int.MaxValue : 600, rate);
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
			Assert.AreEqual(8, relocatedCapacity, "The old center must no longer remain a safe area.");

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
