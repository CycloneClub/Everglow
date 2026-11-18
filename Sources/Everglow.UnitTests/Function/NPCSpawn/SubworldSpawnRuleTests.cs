using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Commons.Mechanics.Events;
using Everglow.Commons.Mechanics.NPCSpawn;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function.NPCSpawn;

[TestClass]
[DoNotParallelize]
public class SubworldSpawnRuleTests
{
	private readonly ManagedSpawnGlobalNPC hook = new();
	private SubworldSpawnSystem system = null!;
	private ILHook rateHook = null!;
	private object? previousInstance;
	private static readonly System.Reflection.PropertyInfo InstanceProperty =
		typeof(ContentInstance<SubworldSpawnSystem>).GetProperty(nameof(ContentInstance<SubworldSpawnSystem>.Instance))!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		// 仅替换 tML 内容实例；被测行为全部经由公开注册方法和原生 hook 调用。
		previousInstance = InstanceProperty.GetValue(null);
		system = new SubworldSpawnSystem();
		InstanceProperty.SetValue(null, system);
		var patch = typeof(SubworldSpawnSystem).GetMethod("PatchRateDispatch", BindingFlags.Instance | BindingFlags.NonPublic)!
			.CreateDelegate<ILContext.Manipulator>(system);
		var dispatch = typeof(SubworldSpawnRuleTests).GetMethod(nameof(DispatchNaturalSpawnRate), BindingFlags.Static | BindingFlags.NonPublic)!;
		rateHook = new ILHook(dispatch, patch);
	}

	[TestCleanup]
	public void Cleanup()
	{
		rateHook.Dispose();
		system.Unload();
		InstanceProperty.SetValue(null, previousInstance);
	}

	// 保留与自然生成相同的调用点，验证 IL 注入后传给各模组的实际 ref 参数。
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void DispatchNaturalSpawnRate(Player player, ref int rate, ref int capacity) =>
		NPCLoader.EditSpawnRate(player, ref rate, ref capacity);

	[TestMethod]
	public void Rate_PatchMatchesInstalledNaturalSpawnMethod()
	{
		system.Load();
	}

	[TestMethod]
	public void Rate_AllHeightsUseNativeSurfacePacing()
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		var patch = typeof(SubworldSpawnSystem).GetMethod("PatchNaturalSpawnRate", BindingFlags.Instance | BindingFlags.NonPublic)!
			.CreateDelegate<ILContext.Manipulator>(system);
		// 只执行实际 SpawnNPC 从初始化到地表事件汇合处的 IL，不创建 NPC 或加载地图。
		using var slice = new ILHook(typeof(NPC).GetMethod(nameof(NPC.SpawnNPC))!, (ILContext il) =>
		{
			var cursor = new ILCursor(il);
			cursor.GotoNext(i => i.MatchLdsfld<NPC>("defaultSpawnRate"), i => i.MatchStsfld<NPC>("spawnRate"));
			var start = cursor.MarkLabel();
			cursor.GotoNext(i => i.MatchLdsfld<Main>(nameof(Main.eclipse)));
			ILLabel end = null!;
			cursor.GotoNext(i => i.MatchBrfalse(out end));
			cursor.Goto(end.Target, MoveType.AfterLabel);
			cursor.EmitRet();
			cursor.Index = 0;
			cursor.EmitBr(start);
			patch(il);
		});
		var rate = typeof(NPC).GetField("spawnRate", BindingFlags.NonPublic | BindingFlags.Static)!;
		var capacity = typeof(NPC).GetField("maxSpawns", BindingFlags.NonPublic | BindingFlags.Static)!;
		var previousRate = rate.GetValue(null);
		var previousCapacity = capacity.GetValue(null);
		var previousPlayer = Main.player[0];
		var previousFlags = (Main.dayTime, Main.bloodMoon, Main.hardMode, Main.remixWorld, Main.eclipse, Main.pumpkinMoon, Main.snowMoon);
		try
		{
			Main.player[0] = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
			Main.remixWorld = Main.pumpkinMoon = Main.snowMoon = false;
			foreach (float y in new[] { 0f, (float)(Main.rockLayer + 100) * 16, (Main.UnderworldLayer + 100) * 16f })
			{
				Main.player[0].position.Y = y;
				foreach (var (day, blood, hard, eclipse, expectedRate, expectedCapacity) in new[]
				{
					(true, false, false, false, 120, 9),
					(false, false, false, false, 72, 11),
					(false, true, false, false, 21, 19),
					(true, false, true, false, 108, 10),
					(true, false, false, true, 24, 17),
				})
				{
					(Main.dayTime, Main.bloodMoon, Main.hardMode, Main.eclipse) = (day, blood, hard, eclipse);
					NPC.SpawnNPC();
					Assert.AreEqual(expectedRate, rate.GetValue(null));
					Assert.AreEqual(expectedCapacity, capacity.GetValue(null));
				}
			}
		}
		finally
		{
			(Main.dayTime, Main.bloodMoon, Main.hardMode, Main.remixWorld, Main.eclipse, Main.pumpkinMoon, Main.snowMoon) = previousFlags;
			Main.player[0] = previousPlayer;
			rate.SetValue(null, previousRate);
			capacity.SetValue(null, previousCapacity);
		}
	}

	[TestMethod]
	public void Pool_ComputesWeightFromCurrentWorldAndSpawnContext()
	{
		bool previousHardMode = Main.hardMode;
		try
		{
			Main.hardMode = false;
			int evaluations = 0;
			SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
			SubworldSpawnSystem.RegisterRules("tree", "dynamic",
			[
				new()
				{
					Type = 10,
					SpawnChance = info =>
					{
						evaluations++;
						return info.Water ? Main.hardMode ? 1.5f : 0.75f : 0f;
					},
				},
			], position => position.X >= 160);
			Assert.AreEqual(0, evaluations, "Registration must not evaluate runtime state.");
			var pool = new Dictionary<int, float>();
			var info = new NPCSpawnInfo { SpawnTileX = 20, SpawnTileY = 30, Water = true };
			hook.EditSpawnPool(pool, info);
			Assert.AreEqual(0.75f, pool[10]);
			Main.hardMode = true;
			hook.EditSpawnPool(pool, info);
			Assert.AreEqual(1.5f, pool[10]);
			info.Water = false;
			hook.EditSpawnPool(pool, info);
			Assert.IsFalse(pool.ContainsKey(10));
			Assert.AreEqual(3, evaluations);
			info.SpawnTileX = 1;
			hook.EditSpawnPool(pool, info);
			Assert.AreEqual(3, evaluations, "An unmatched table must not evaluate its rules.");
		}
		finally
		{
			Main.hardMode = previousHardMode;
		}
	}

	[TestMethod]
	public void GroupSelection_DoesNotReevaluateTheSelectedPoolWeights()
	{
		int evaluations = 0;
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		SubworldSpawnSystem.RegisterRules("tree", "groups",
		[
			new() { Type = 10, SpawnChance = _ => ++evaluations, MinGroupSize = 2, MaxGroupSize = 2 },
			new() { Type = 10, SpawnChance = _ => ++evaluations },
		], _ => true);
		var info = new NPCSpawnInfo { SpawnTileX = 20, SpawnTileY = 30 };
		var pool = new Dictionary<int, float>();
		hook.EditSpawnPool(pool, info);
		Assert.AreEqual(2f, pool[10]);
		Assert.AreEqual(2, evaluations);

		NPC previousNpc = Main.npc[0];
		var previousRandom = Main.rand;
		try
		{
			// 首只在 OnSpawn 中失活：仍执行配置选择，但不创建实际群体或访问地形。
			var first = (NPC)RuntimeHelpers.GetUninitializedObject(typeof(NPC));
			first.netID = 10;
			Main.npc[0] = first;
			Main.rand = new Terraria.Utilities.UnifiedRandom(1);
			hook.SpawnNPC(0, 20, 30);
			Assert.AreEqual(2, evaluations, "Group selection must reuse the weights used to build the pool.");
			hook.SpawnNPC(0, 20, 30);
			Assert.AreEqual(2, evaluations);
		}
		finally
		{
			Main.npc[0] = previousNpc;
			Main.rand = previousRandom;
		}
	}

	[TestMethod]
	[DataRow(5f, 2f, 2, 4, 2)]
	[DataRow(2f, 5f, 2, 4, 4)]
	[DataRow(5f, 5f, 2, 4, 2)]
	[DataRow(5f, 2f, 1, 4, 1)]
	[DataRow(2f, 5f, 4, 1, 1)]
	[DataRow(5f, 5f, 1, 4, 1)]
	[DataRow(5f, 2f, 2, 4, 2, 8f)]
	public void OverlappingTables_UseWinningRuleForWeightGroupAndRegion(float firstWeight, float secondWeight,
		int firstCount, int secondCount, int expectedCount, float nearbySlots = 0f)
	{
		bool expanding = false;
		int firstRegionChecks = 0;
		int secondRegionChecks = 0;
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		SubworldSpawnSystem.RegisterRules("tree", "first",
			[new() { Type = 10, SpawnChance = _ => firstWeight, MinGroupSize = firstCount, MaxGroupSize = firstCount }],
			_ =>
			{
				if (!expanding)
				{
					return true;
				}
				firstRegionChecks++;
				return false;
			});
		SubworldSpawnSystem.RegisterRules("tree", "second",
			[new() { Type = 10, SpawnChance = _ => secondWeight, MinGroupSize = secondCount, MaxGroupSize = secondCount }],
			_ =>
			{
				if (!expanding)
				{
					return true;
				}
				secondRegionChecks++;
				return false;
			});
		var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		player.active = true;
		player.nearbyActiveNPCs = nearbySlots;
		int rate = 600;
		int capacity = 5;
		DispatchNaturalSpawnRate(player, ref rate, ref capacity);
		var pool = new Dictionary<int, float>();
		hook.EditSpawnPool(pool, new NPCSpawnInfo { Player = player, SpawnTileX = 20, SpawnTileY = 30 });
		Assert.AreEqual(Math.Max(firstWeight, secondWeight), pool[10]);

		NPC previousNpc = Main.npc[0];
		var previousRandom = Main.rand;
		try
		{
			var first = (NPC)RuntimeHelpers.GetUninitializedObject(typeof(NPC));
			first.netID = 10;
			first.active = true;
			first.npcSlots = 1f;
			Main.npc[0] = first;
			var random = new GroupCountRandom();
			Main.rand = random;
			expanding = true;
			// 新位置由区域函数拒绝，不启动真实 NewNPC 或加载地形。
			hook.SpawnNPC(0, 20, 30);
			Assert.AreEqual(expectedCount > 1 ? 1 : 0, random.Calls);
			if (expectedCount > 1)
			{
				Assert.AreEqual(expectedCount, random.Minimum);
				Assert.AreEqual(expectedCount + 1, random.Maximum);
			}
			bool firstWins = firstWeight >= secondWeight;
			int attempts = nearbySlots >= capacity - first.npcSlots ? 0 : expectedCount - 1;
			Assert.AreEqual(firstWins ? attempts : 0, firstRegionChecks);
			Assert.AreEqual(firstWins ? 0 : attempts, secondRegionChecks);
			hook.SpawnNPC(0, 20, 30);
			Assert.AreEqual(expectedCount > 1 ? 1 : 0, random.Calls, "The winning rule can only be consumed once.");
		}
		finally
		{
			Main.npc[0] = previousNpc;
			Main.rand = previousRandom;
		}
	}

	private sealed class GroupCountRandom : Terraria.Utilities.UnifiedRandom
	{
		public int Calls { get; private set; }

		public int Minimum { get; private set; }

		public int Maximum { get; private set; }

		public override int Next(int minValue, int maxValue)
		{
			Calls++;
			Minimum = minValue;
			Maximum = maxValue;
			return minValue;
		}
	}

	[TestMethod]
	[DataRow(float.NaN)]
	[DataRow(float.PositiveInfinity)]
	[DataRow(float.NegativeInfinity)]
	[DataRow(-1f)]
	public void Pool_RejectsInvalidCalculatedWeights(float weight)
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		SubworldSpawnSystem.RegisterRules("tree", "invalid", [new() { Type = 10, SpawnChance = _ => weight }], _ => true);
		Assert.ThrowsExactly<InvalidOperationException>(() => hook.EditSpawnPool(new Dictionary<int, float>(), default));
	}

	[TestMethod]
	public void Pool_ReplacesCandidatesAndUsesHighestMatchingWeight()
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		bool enabled = true;
		SubworldSpawnSystem.RegisterRules("tree", "kelp",
		[
			new() { Type = 10, SpawnChance = info => info.Water && enabled ? 5f : 0f },
			new() { Type = 10, SpawnChance = info => info.Water ? 3f : 0f },
			new() { Type = 11, SpawnChance = info => !info.Water ? 7f : 0f },
		], position => position.X >= 160);
		var pool = new Dictionary<int, float> { [0] = 1, [999] = 5 };
		var info = new NPCSpawnInfo { SpawnTileX = 20, SpawnTileY = 30, Water = true };

		hook.EditSpawnPool(pool, info);
		Assert.AreEqual(1, pool.Count);
		Assert.AreEqual(5f, pool[10]);

		enabled = false;
		hook.EditSpawnPool(pool, info);
		Assert.AreEqual(3f, pool[10], "Conditions must be evaluated again on the next native spawn opportunity.");

		info.SpawnTileX = 1;
		hook.EditSpawnPool(pool, info);
		Assert.AreEqual(0, pool.Count, "An empty regional table must not restore vanilla key zero.");
	}

	[TestMethod]
	public void Pool_OnlyAppliesRegisteredActiveWorld()
	{
		bool active = false;
		SubworldSpawnSystem.RegisterWorld(new("tree", () => active));
		SubworldSpawnSystem.RegisterRules("tree", "test", [new() { Type = 10 }], _ => true);
		var pool = new Dictionary<int, float> { [0] = 1, [99] = 2 };

		hook.EditSpawnPool(pool, default);
		Assert.AreEqual(2, pool.Count);
		active = true;
		hook.EditSpawnPool(pool, default);
		Assert.AreEqual(1, pool.Count);
		Assert.AreEqual(1f, pool[10]);
		active = false;
		pool[99] = 2;
		hook.EditSpawnPool(pool, default);
		Assert.AreEqual(2, pool.Count);
	}

	[TestMethod]
	[DataRow(2, false, 60, 18)]
	[DataRow(0, false, int.MaxValue, 0)]
	[DataRow(2, true, int.MaxValue, 0)]
	public void Rate_ExternalGlobalReceivesWildernessBase(int multiplier, bool blocked, int expectedRate, int expectedCapacity)
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true, _ => !blocked));
		var list = typeof(NPCLoader).GetField("HookEditSpawnRate", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var globals = list.GetType().GetField("hookGlobals", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var previous = globals.GetValue(list);
		try
		{
			globals.SetValue(list, new GlobalNPC[] { new SpawnRateTool(multiplier) });
			int rate = 120;
			int capacity = 9;
			DispatchNaturalSpawnRate((Player)RuntimeHelpers.GetUninitializedObject(typeof(Player)), ref rate, ref capacity);
			Assert.AreEqual(expectedRate, rate);
			Assert.AreEqual(expectedCapacity, capacity);
		}
		finally
		{
			globals.SetValue(list, previous);
		}
	}

	private sealed class SpawnRateTool(int multiplier) : GlobalNPC
	{
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			spawnRate = multiplier == 0 ? 0 : spawnRate / multiplier;
			maxSpawns *= multiplier;
		}
	}

	[TestMethod]
	[DataRow(600, 5)]
	[DataRow(60, 18)]
	[DataRow(int.MaxValue, 0)]
	public void Rate_DispatchPreservesNativeModifiers(int rate, int capacity)
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		int expectedRate = rate;
		int expectedCapacity = capacity;
		DispatchNaturalSpawnRate((Player)RuntimeHelpers.GetUninitializedObject(typeof(Player)), ref rate, ref capacity);
		Assert.AreEqual(expectedRate, rate);
		Assert.AreEqual(expectedCapacity, capacity);
	}

	[TestMethod]
	public void Rate_OnlyDistinguishesBlockedAndEnabledAtPlayerTilePosition()
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true, position => position.X >= 100));
		var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		int rate = 600;
		int capacity = 8;
		player.Center = new Vector2(99 * 16, 200 * 16);
		DispatchNaturalSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(int.MaxValue, rate);
		Assert.AreEqual(0, capacity);

		rate = 120;
		capacity = 9;
		player.Center = new Vector2(100 * 16, 200 * 16);
		DispatchNaturalSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(120, rate, "Leaving a safe area must immediately restore full wilderness pacing.");
		Assert.AreEqual(9, capacity);
	}

	[TestMethod]
	public void Rate_LeavesUnregisteredWorldAloneAndRestoresItsInputsAfterExit()
	{
		bool active = false;
		SubworldSpawnSystem.RegisterWorld(new("tree", () => active));
		var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		int rate = 77;
		int capacity = 9;
		DispatchNaturalSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(77, rate);
		Assert.AreEqual(9, capacity);
		active = true;
		rate = 120;
		DispatchNaturalSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(120, rate);
		Assert.AreEqual(9, capacity);
		active = false;
		rate = 600;
		capacity = 5;
		DispatchNaturalSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(600, rate);
		Assert.AreEqual(5, capacity);
	}

	[TestMethod]
	public void Registration_RejectsMissingCallbacksInvalidTypesAndDuplicateTables()
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		Assert.ThrowsExactly<ArgumentException>(() =>
			SubworldSpawnSystem.RegisterRules("tree", "bad", [new() { Type = 10, SpawnChance = null! }], _ => true));
		Assert.ThrowsExactly<ArgumentException>(() =>
			SubworldSpawnSystem.RegisterRules("tree", "bad", [new() { Type = 0 }], _ => true));
		SubworldSpawnSystem.RegisterRules("tree", "test", [new() { Type = 10 }], _ => true);
		Assert.ThrowsExactly<InvalidOperationException>(() =>
			SubworldSpawnSystem.RegisterRules("tree", "test", [new() { Type = 20 }], _ => true));
	}

	[TestMethod]
	public void MultipleActiveWorldsAreRejectedInsteadOfMixingTheirPools()
	{
		SubworldSpawnSystem.RegisterWorld(new("first", () => true));
		SubworldSpawnSystem.RegisterWorld(new("second", () => true));
		Assert.ThrowsExactly<InvalidOperationException>(() =>
			hook.EditSpawnPool(new Dictionary<int, float>(), default));
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void CustomEvent_OwnsPoolAndRateRegardlessOfHookOrder(bool eventHookFirst)
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true, _ => false));
		SubworldSpawnSystem.RegisterRules("tree", "table", [new() { Type = 10 }], _ => true);
		var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		var pool = new Dictionary<int, float> { [0] = 1 };
		int rate = 600;
		int capacity = 8;
		var customEvent = new SpawnPriorityEvent();
		try
		{
			Assert.IsTrue(EventSystem.Activate(customEvent));
			void EditEventSpawns()
			{
				pool.Clear();
				pool.Add(999, 7f);
				rate /= 15;
				capacity = 60;
			}
			if (eventHookFirst)
			{
				EditEventSpawns();
			}
			hook.EditSpawnPool(pool, new NPCSpawnInfo { Player = player });
			DispatchNaturalSpawnRate(player, ref rate, ref capacity);
			if (!eventHookFirst)
			{
				Assert.AreEqual(1f, pool[0], "Ordinary spawning must leave the incoming pool untouched while yielding.");
				EditEventSpawns();
			}
			Assert.AreEqual(1, pool.Count);
			Assert.AreEqual(7f, pool[999]);
			Assert.AreEqual(40, rate);
			Assert.AreEqual(60, capacity, "The ordinary safe area must not suppress event spawns.");

			Assert.IsTrue(EventSystem.Deactivate(customEvent));
			hook.EditSpawnPool(pool, new NPCSpawnInfo { Player = player });
			Assert.AreEqual(1, pool.Count);
			Assert.AreEqual(1f, pool[10]);
			DispatchNaturalSpawnRate(player, ref rate, ref capacity);
			Assert.AreEqual(0, capacity, "Ordinary area policy resumes after the event ends.");
		}
		finally
		{
			EventSystem.Deactivate(customEvent);
		}
	}

	[TestMethod]
	public void VanillaBloodMoon_DoesNotMakeOrdinarySpawningYield()
	{
		bool oldBloodMoon = Main.bloodMoon;
		try
		{
			Main.bloodMoon = true;
			SubworldSpawnSystem.RegisterWorld(new("tree", () => true, _ => true));
			SubworldSpawnSystem.RegisterRules("tree", "table", [new() { Type = 10 }], _ => true);
			var pool = new Dictionary<int, float> { [0] = 1 };
			hook.EditSpawnPool(pool, default);
			Assert.AreEqual(1, pool.Count);
			Assert.AreEqual(1f, pool[10]);
			int rate = 600;
			int capacity = 8;
			DispatchNaturalSpawnRate((Player)RuntimeHelpers.GetUninitializedObject(typeof(Player)), ref rate, ref capacity);
			Assert.AreEqual(600, rate);
			Assert.AreEqual(8, capacity);
		}
		finally
		{
			Main.bloodMoon = oldBloodMoon;
		}
	}

	[TestMethod]
	public void LocalEvent_LeavesOrdinarySpawningEnabled()
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true, _ => true));
		SubworldSpawnSystem.RegisterRules("tree", "table", [new() { Type = 10, MinGroupSize = 2, MaxGroupSize = 2 }], _ => true);
		var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		var info = new NPCSpawnInfo { Player = player, SpawnTileX = 20, SpawnTileY = 30 };
		var localEvent = new LocalSpawnEvent();
		var overridingEvent = new SpawnPriorityEvent();
		try
		{
			Assert.IsTrue(EventSystem.Activate(localEvent));
			Assert.IsTrue(localEvent.Active);
			var pool = new Dictionary<int, float> { [0] = 1 };
			hook.EditSpawnPool(pool, info);
			Assert.AreEqual(1, pool.Count);
			Assert.IsTrue(pool.ContainsKey(10), "Default events must not make ordinary tables yield.");
			int rate = 600;
			int capacity = 8;
			DispatchNaturalSpawnRate(player, ref rate, ref capacity);
			Assert.AreEqual(600, rate);
			Assert.AreEqual(8, capacity);

			EventSystem.Activate(overridingEvent);
			pool.Clear();
			pool.Add(999, 7f);
			hook.EditSpawnPool(pool, info);
			Assert.AreEqual(7f, pool[999]);
			Assert.IsFalse(pool.ContainsKey(10));
			EventSystem.Deactivate(overridingEvent);
			Assert.IsTrue(localEvent.Active);
			hook.EditSpawnPool(pool, info);
			Assert.AreEqual(1, pool.Count);
			Assert.AreEqual(1f, pool[10], "Ordinary spawning resumes even while a local event remains active.");
		}
		finally
		{
			EventSystem.Deactivate(overridingEvent);
			EventSystem.Deactivate(localEvent);
		}
	}

	private sealed class LocalSpawnEvent : ModEvent
	{
	}

	private sealed class SpawnPriorityEvent : ModEvent
	{
		public override bool OverridesNaturalSpawning => true;
	}

	[TestMethod]
	public void UnifiedHook_UsesRegisteredTableWhenWorldIsActive()
	{
		bool active = false;
		SubworldSpawnSystem.RegisterWorld(new("tree", () => active));
		SubworldSpawnSystem.RegisterRules("tree", "table", [new() { Type = 1000, SpawnChance = _ => 7f }], _ => true);
		Assert.IsTrue(hook.IsLoadingEnabled(null!), "The shared hook must always load.");
		var pool = new Dictionary<int, float> { [0] = 1, [1] = 0.1f, [1000] = 2 };
		hook.EditSpawnPool(pool, default);
		Assert.AreEqual(3, pool.Count);
		active = true;
		hook.EditSpawnPool(pool, default);
		Assert.AreEqual(1, pool.Count);
		Assert.AreEqual(7f, pool[1000]);
		Assert.AreSame(system, ModContent.GetInstance<SubworldSpawnSystem>());
		rateHook.Dispose();
		system.Unload();
		pool[0] = 1;
		hook.EditSpawnPool(pool, default);
		Assert.AreEqual(2, pool.Count, "Unload must stop overriding the incoming pool.");
		Assert.AreEqual(1f, pool[0]);
		Assert.AreEqual(7f, pool[1000]);
	}

	[TestMethod]
	public void Registration_RejectsInvalidGroupSizes()
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		Assert.ThrowsExactly<ArgumentException>(() => SubworldSpawnSystem.RegisterRules("tree", "zero",
			[new() { Type = 10, MinGroupSize = 0 }], _ => true));
		Assert.ThrowsExactly<ArgumentException>(() => SubworldSpawnSystem.RegisterRules("tree", "reversed",
			[new() { Type = 10, MinGroupSize = 3, MaxGroupSize = 2 }], _ => true));
		Assert.ThrowsExactly<ArgumentException>(() => SubworldSpawnSystem.RegisterRules("tree", "unbounded",
			[new() { Type = 10, MaxGroupSize = int.MaxValue }], _ => true));
	}
}
