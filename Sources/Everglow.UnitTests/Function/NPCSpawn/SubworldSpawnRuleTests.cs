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
	public void Pool_ReplacesVanillaAndModCandidatesAndAddsMatchingWeights()
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		bool enabled = true;
		SubworldSpawnSystem.RegisterRules("tree", "kelp",
		[
			new() { Type = 10, Weight = 2, Condition = info => info.Water && enabled },
			new() { Type = 10, Weight = 3, Condition = info => info.Water },
			new() { Type = 11, Weight = 7, Condition = info => !info.Water },
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
	public void Registration_RejectsInvalidWeightsAndDuplicateTables()
	{
		SubworldSpawnSystem.RegisterWorld(new("tree", () => true));
		Assert.ThrowsExactly<ArgumentException>(() =>
			SubworldSpawnSystem.RegisterRules("tree", "bad", [new() { Type = 10, Weight = float.NaN }], _ => true));
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
		SubworldSpawnSystem.RegisterRules("tree", "table", [new() { Type = 1000, Weight = 7 }], _ => true);
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
