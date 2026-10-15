using System.Runtime.CompilerServices;
using Everglow.Commons.Mechanics.Events;
using Everglow.Commons.Mechanics.NPCSpawn;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function.NPCSpawn;

[TestClass]
[DoNotParallelize]
public class SubworldSpawnRuleTests
{
	private readonly ManagedSpawnGlobalNPC hook = new();
	private SubworldSpawnSystem system = null!;
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
	}

	[TestCleanup]
	public void Cleanup()
	{
		system.Unload();
		InstanceProperty.SetValue(null, previousInstance);
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
	public void OverlappingTables_UseWinningRuleForWeightGroupAndRegion(float firstWeight, float secondWeight,
		int firstCount, int secondCount, int expectedCount)
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
			Assert.AreEqual(firstWins ? expectedCount - 1 : 0, firstRegionChecks);
			Assert.AreEqual(firstWins ? 0 : expectedCount - 1, secondRegionChecks);
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
	public void Rate_UsesNativeInputsAndOnlyAppliesWorldDensityOnce()
	{
		float density = 0.25f;
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true, _ => density));
		var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		int rate = 600;
		int capacity = 8;
		hook.EditSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(2400, rate);
		Assert.AreEqual(2, capacity);

		density = 0;
		hook.EditSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(0, capacity);
		Assert.IsTrue(rate > 0);

		density = 1;
		rate = 120;
		capacity = 12;
		hook.EditSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(120, rate);
		Assert.AreEqual(12, capacity);
	}

	[TestMethod]
	public void Rate_LeavesUnregisteredWorldAloneAndUsesDefaultDensityForActiveWorld()
	{
		bool active = false;
		SubworldSpawnSystem.RegisterWorld(new("tree", () => active));
		var player = (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
		int rate = 77;
		int capacity = 9;
		hook.EditSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(77, rate);
		Assert.AreEqual(9, capacity);
		active = true;
		hook.EditSpawnRate(player, ref rate, ref capacity);
		Assert.AreEqual(77, rate);
		Assert.AreEqual(9, capacity);
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
	public void Ellipse_CenterAxesAndBoundaryRemainFinite()
	{
		Assert.AreEqual(0f, Everglow.Yggdrasil.Common.NPCSpawn.YggdrasilSpawnRegistration.EllipseDensity(new Vector2(600, 11630)));
		Assert.AreEqual(0.25f, Everglow.Yggdrasil.Common.NPCSpawn.YggdrasilSpawnRegistration.EllipseDensity(new Vector2(750, 11630)));
		Assert.AreEqual(1f, Everglow.Yggdrasil.Common.NPCSpawn.YggdrasilSpawnRegistration.EllipseDensity(new Vector2(900, 11630)));
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void CustomEvent_OwnsPoolAndRateRegardlessOfHookOrder(bool eventHookFirst)
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true, _ => 0f));
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
			hook.EditSpawnRate(player, ref rate, ref capacity);
			if (!eventHookFirst)
			{
				Assert.AreEqual(1f, pool[0], "Ordinary spawning must leave the incoming pool untouched while yielding.");
				EditEventSpawns();
			}
			Assert.AreEqual(1, pool.Count);
			Assert.AreEqual(7f, pool[999]);
			Assert.AreEqual(40, rate);
			Assert.AreEqual(60, capacity, "The ordinary zero-density area must not suppress event spawns.");

			Assert.IsTrue(EventSystem.Deactivate(customEvent));
			hook.EditSpawnPool(pool, new NPCSpawnInfo { Player = player });
			Assert.AreEqual(1, pool.Count);
			Assert.AreEqual(1f, pool[10]);
			hook.EditSpawnRate(player, ref rate, ref capacity);
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
			SubworldSpawnSystem.RegisterWorld(new("tree", () => true, _ => 0.5f));
			SubworldSpawnSystem.RegisterRules("tree", "table", [new() { Type = 10 }], _ => true);
			var pool = new Dictionary<int, float> { [0] = 1 };
			hook.EditSpawnPool(pool, default);
			Assert.AreEqual(1, pool.Count);
			Assert.AreEqual(1f, pool[10]);
			int rate = 600;
			int capacity = 8;
			hook.EditSpawnRate((Player)RuntimeHelpers.GetUninitializedObject(typeof(Player)), ref rate, ref capacity);
			Assert.AreEqual(1200, rate);
			Assert.AreEqual(4, capacity);
		}
		finally
		{
			Main.bloodMoon = oldBloodMoon;
		}
	}

	[TestMethod]
	public void LocalEvent_LeavesOrdinarySpawningEnabled()
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true, _ => 0.5f));
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
			hook.EditSpawnRate(player, ref rate, ref capacity);
			Assert.AreEqual(1200, rate);
			Assert.AreEqual(4, capacity);

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
