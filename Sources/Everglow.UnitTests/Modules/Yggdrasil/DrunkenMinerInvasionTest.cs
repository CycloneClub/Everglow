using System.Reflection;
using Microsoft.Xna.Framework;
using Everglow.Commons.Mechanics.Events;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Mechanics.Quest.WorldSide.Structure;
using Everglow.Yggdrasil.YggdrasilTown.Quests;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.YggdrasilTown.Events;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class DrunkenMinerInvasionTest
{
	private static readonly FieldInfo CurrentWorld = typeof(SubworldSystem).GetField("current", BindingFlags.NonPublic | BindingFlags.Static)!;
	private Subworld? oldWorld;
	private int oldMode;
	private bool oldDay;
	private readonly EventSystem system = new();
	private readonly EventGlobalNPC hook = new();
	private readonly List<WorldObjectiveBase> objectives = [];
	private DrunkenMinerInvasion oldInvasion = null!;
	private IReadOnlyList<DrunkenMinerInvasion> oldInvasions = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		oldMode = Main.netMode;
		oldDay = Main.dayTime;
		oldInvasion = ContentInstance<DrunkenMinerInvasion>.Instance;
		oldInvasions = ContentInstance<DrunkenMinerInvasion>.Instances;
		Main.dayTime = true;
		oldWorld = (Subworld?)CurrentWorld.GetValue(null);
		Main.netMode = NetmodeID.SinglePlayer;
		CurrentWorld.SetValue(null, new YggdrasilWorld());
		system.Unload();
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var objective in objectives)
		{
			objective.Deactivate();
		}
		objectives.Clear();
		system.Unload();
		Main.netMode = oldMode;
		Main.dayTime = oldDay;
		SetInvasion(oldInvasion, oldInvasions);
		CurrentWorld.SetValue(null, oldWorld);
	}

	[TestMethod]
	public void DaytimeObjectiveStartsWhenItBecomesNight()
	{
		Main.dayTime = true;
		var invasion = CreateInvasion();
		var objective = CreateObjective(invasion);
		objective.Update();
		Assert.IsFalse(invasion.Active);
		Main.dayTime = false;
		objective.Update();
		Assert.IsTrue(invasion.Active);
	}

	[TestMethod]
	public void NighttimeObjectiveAttemptsImmediately()
	{
		Main.dayTime = false;
		var invasion = CreateInvasion();
		CreateObjective(invasion).Update();
		Assert.IsTrue(invasion.Active);
	}

	[TestMethod]
	public void ClientObjectiveCannotStartInvasion()
	{
		Main.dayTime = false;
		Main.netMode = NetmodeID.MultiplayerClient;
		var invasion = CreateInvasion();
		CreateObjective(invasion).Update();
		Assert.IsFalse(invasion.Active);
	}

	[TestMethod]
	public void PreviousVictorySatisfiesNewObjective()
	{
		Main.dayTime = false;
		var invasion = CreateInvasion();
		invasion.LoadWorldData(invasion.FullName, new TagCompound
		{
			[nameof(DrunkenMinerInvasion.TargetCount)] = 12,
			[nameof(DrunkenMinerInvasion.DefeatedEnemies)] = 12,
		});
		var objective = CreateObjective(invasion);
		objective.Update();
		Assert.IsTrue(objective.CheckCompletion());
		Assert.IsFalse(invasion.Active);
		Assert.IsTrue(invasion.Completed);
	}

	[TestMethod]
	public void MainWorldCannotStartTownInvasion()
	{
		Main.dayTime = false;
		CurrentWorld.SetValue(null, null);
		var invasion = CreateInvasion();
		CreateObjective(invasion).Update();
		Assert.IsFalse(invasion.Active);
		Assert.IsFalse(EventSystem.Activate(invasion));
	}

	[TestMethod]
	public void ObjectiveExitDoesNotCancelInvasion()
	{
		var invasion = CreateInvasion();
		var objective = CreateObjective(invasion);
		Main.dayTime = false;
		objective.Update();
		objective.Deactivate();
		Assert.IsTrue(invasion.Active);
	}

	[TestMethod]
	public void MultipleObjectivesReadTheSameVictoryRecord()
	{
		var invasion = CreateInvasion();
		var first = CreateObjective(invasion);
		var second = CreateObjective(invasion);
		invasion.Downed = true;
		first.Update();
		second.Update();
		Assert.IsTrue(first.CheckCompletion());
		Assert.IsTrue(second.CheckCompletion());
	}

	[TestMethod]
	public void OrdinaryStopDoesNotCompleteObjective()
	{
		var invasion = CreateInvasion();
		var objective = CreateObjective(invasion);
		Main.dayTime = false;
		objective.Update();
		EventSystem.Deactivate(invasion);
		Main.dayTime = true;
		objective.Update();
		Assert.IsFalse(invasion.Downed);
		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void ObjectiveProgressMapsEventProgressAndResetDoesNotChangeEvent()
	{
		var invasion = CreateInvasion();
		var objective = CreateObjective(invasion);
		EventSystem.Activate(invasion);
		invasion.LoadWorldData(invasion.FullName, new TagCompound
		{
			[nameof(DrunkenMinerInvasion.TargetCount)] = 12,
			[nameof(DrunkenMinerInvasion.DefeatedEnemies)] = 3,
		});
		Assert.AreEqual(0.25f, objective.Progress);
		StringAssert.Contains(objective.GetObjectiveText(), "3/12");
		objective.ResetProgress();
		Assert.IsTrue(invasion.Active);
		Assert.AreEqual(3, invasion.DefeatedEnemies);
		objective.Activate(null!);
		Assert.AreEqual(0.25f, objective.Progress);
	}

	[TestMethod]
	public void DownedIsReportedAgainAfterStaleSnapshotUntilMainServerAcknowledgesIt()
	{
		var invasion = CreateInvasion();
		var objective = CreateObjective(invasion);
		Main.netMode = NetmodeID.Server;
		invasion.Downed = true;
		objective.Update();
		Assert.AreEqual(1f, objective.Progress);
		Assert.IsFalse(objective.CheckCompletion());
		Assert.IsTrue(objective.NeedDeltaSync);
		var mainObjective = CreateObjective(invasion);
		Transfer(mainObjective.NetSend, objective.NetReceive);
		objective.Update();
		Assert.IsTrue(objective.NeedDeltaSync);
		Assert.AreEqual(1f, objective.Progress);
		Transfer(objective.SendDelta, mainObjective.ReceiveDelta);
		Assert.IsTrue(mainObjective.CheckCompletion());
		Main.dayTime = false;
		objective.Update();
		Assert.IsFalse(invasion.Active);
		Assert.IsTrue(objective.NeedDeltaSync);
		Transfer(mainObjective.SendMain, objective.ReceiveMain);
		Assert.IsTrue(objective.CheckCompletion());
	}

	[TestMethod]
	public void RetryUsesExistingDownedRecord()
	{
		var invasion = CreateInvasion();
		var objective = CreateObjective(invasion);
		objective.WithTimeLimit(10);
		objective.Timer.Update(10);
		invasion.Downed = true;
		objective.Update();
		Assert.IsFalse(objective.CheckCompletion());
		objective.ResetProgress();
		Assert.IsTrue(invasion.Downed);
		objective.Update();
		Assert.IsTrue(objective.CheckCompletion());
		Assert.IsFalse(invasion.Active);
	}

	[TestMethod]
	public void TimedOutObjectiveDoesNotStartEvent()
	{
		Main.dayTime = false;
		var invasion = CreateInvasion();
		var objective = CreateObjective(invasion);
		objective.WithTimeLimit(1);
		objective.Timer.Update(1);
		objective.Update();
		Assert.IsFalse(invasion.Active);
		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void ClientDownedRecordDoesNotCompleteOrUploadObjective()
	{
		var invasion = CreateInvasion();
		var objective = CreateObjective(invasion);
		Main.netMode = NetmodeID.MultiplayerClient;
		invasion.Downed = true;
		objective.Update();
		Assert.IsFalse(objective.CheckCompletion());
		Assert.IsFalse(objective.NeedDeltaSync);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void InMemoryInvasionRunAdvancesToReportAndSynchronizesVictory(bool subserver)
	{
		// Simulate NPC creation and transport boundaries; run the real event, kill hook and objective.
		var invasion = CreateInvasion();
		var objective = CreateObjective(invasion);
		var report = new WorldTalkObjective(NPCID.Guide);
		var container = new WorldObjectiveContainer();
		container.Add(objective).Add(report);
		container.OnObjectiveActivated += node =>
		{
			foreach (var current in node.FindAllEntrances())
			{
				current.Activate(null!);
			}
		};
		container.OnObjectiveDeactivated += objective.Deactivate;
		int completedNodes = 0;
		container.OnNodeCompleted += _ => completedNodes++;
		container.Activate();
		container.UpdateNode();
		Assert.IsFalse(invasion.Active, "Daytime must not start the invasion.");
		Main.dayTime = false;
		container.UpdateNode();
		Assert.IsTrue(invasion.Active);
		Assert.AreEqual(12, invasion.TargetCount);

		var hook = new EventGlobalNPC();
		hook.OnKill(new NPC { active = true, life = 0, type = NPCID.Zombie });
		Assert.AreEqual(0, invasion.DefeatedEnemies, "A wild zombie must not count.");
		var track = typeof(DrunkenMinerInvasion).GetMethod("TrackEnemy", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var vanished = new NPC { active = true, life = 10, type = NPCID.Zombie };
		track.Invoke(invasion, [vanished]);
		vanished.active = false;
		system.PostUpdateEverything();
		hook.OnKill(vanished);
		Assert.AreEqual(0, invasion.DefeatedEnemies, "Natural despawn must not count.");

		for (int killed = 1; killed <= invasion.TargetCount; killed++)
		{
			// Single-player authority drives kills without invoking the real socket/world serializer.
			Main.netMode = NetmodeID.SinglePlayer;
			var enemy = new NPC { active = true, life = 10, type = NPCID.Zombie };
			track.Invoke(invasion, [enemy]);
			enemy.life = 0;
			hook.OnKill(enemy);
			hook.OnKill(enemy);
			system.PostUpdateEverything();
			Assert.AreEqual(killed, invasion.DefeatedEnemies);
			Assert.AreEqual(killed == invasion.TargetCount, invasion.Downed);
			if (subserver)
			{
				Main.netMode = NetmodeID.Server;
			}
			container.UpdateNode();
			Assert.AreEqual((float)killed / invasion.TargetCount, objective.Progress);
			if (killed < invasion.TargetCount)
			{
				Assert.AreSame(objective, container.FindCurrentObjectives().Single());
				Assert.IsFalse(objective.Completed);
			}
		}
		Assert.IsFalse(invasion.Active);
		if (subserver)
		{
			Assert.IsFalse(objective.Completed, "Subserver must await main-server acknowledgement.");
			Assert.IsTrue(objective.NeedDeltaSync);
			var mainObjective = CreateObjective(invasion);
			Transfer(mainObjective.NetSend, objective.NetReceive);
			container.UpdateNode();
			Assert.IsTrue(objective.NeedDeltaSync, "A stale quest snapshot must not lose the victory.");
			Transfer(objective.SendDelta, reader =>
			{
				CurrentWorld.SetValue(null, null);
				mainObjective.ReceiveDelta(reader);
			});
			Assert.IsTrue(mainObjective.CheckCompletion());
			Transfer(mainObjective.SendMain, objective.ReceiveMain);
			CurrentWorld.SetValue(null, new YggdrasilWorld());
			container.UpdateNode();
		}
		Assert.IsTrue(objective.Completed);
		Assert.AreEqual(1, completedNodes);
		Assert.AreSame(report, container.FindCurrentObjectives().Single());

		using var snapshot = new MemoryStream();
		using var writer = new BinaryWriter(snapshot);
		system.NetSend(writer);
		for (int client = 0; client < 2; client++)
		{
			system.ClearWorld();
			Main.netMode = NetmodeID.MultiplayerClient;
			snapshot.Position = 0;
			system.NetReceive(new BinaryReader(snapshot));
			var clientObjective = CreateObjective(invasion);
			Transfer(objective.NetSend, clientObjective.NetReceive);
			Assert.IsTrue(invasion.Downed);
			Assert.IsFalse(invasion.Active);
			Assert.AreEqual(12, invasion.DefeatedEnemies);
			Assert.IsTrue(clientObjective.Completed);
			clientObjective.Update();
			Assert.IsFalse(clientObjective.NeedDeltaSync);
			Assert.IsFalse(EventSystem.Activate(invasion));
			snapshot.Position = 0;
			system.NetReceive(new BinaryReader(snapshot));
			Assert.IsTrue(invasion.Downed, "An unrelated WorldData refresh must preserve the result.");
		}
	}

	[TestMethod]
	public void OnlyOwnedKillsCountAndFinalKillCompletesExactlyOnce()
	{
		var invasion = CreateActiveInvasion();
		var first = CreateEnemy();
		TrackEnemy(invasion, first);
		hook.OnKill(CreateEnemy());
		Assert.AreEqual(0, invasion.DefeatedEnemies);
		hook.OnKill(first);
		hook.OnKill(first);
		Assert.AreEqual(1, invasion.DefeatedEnemies);
		Assert.IsTrue(invasion.Active);
		for (int i = 1; i < invasion.TargetCount; i++)
		{
			var enemy = CreateEnemy();
			TrackEnemy(invasion, enemy);
			hook.OnKill(enemy);
			hook.OnKill(enemy);
		}
		Assert.AreEqual(invasion.TargetCount, invasion.DefeatedEnemies);
		Assert.IsTrue(invasion.Completed);
		Assert.IsTrue(invasion.Downed);
		Assert.IsFalse(invasion.Active);
	}

	[TestMethod]
	public void DespawnAndNewNpcWithSameSlotDoNotCountAsKills()
	{
		var invasion = CreateActiveInvasion();
		var despawned = CreateEnemy();
		despawned.whoAmI = 7;
		TrackEnemy(invasion, despawned);
		despawned.active = false;
		invasion.PostUpdateEverything();
		var replacement = CreateEnemy();
		replacement.whoAmI = despawned.whoAmI;
		hook.OnKill(despawned);
		hook.OnKill(replacement);
		Assert.AreEqual(0, invasion.DefeatedEnemies);
		Assert.IsFalse(invasion.Completed);
		Assert.IsFalse(invasion.Downed);
	}

	[TestMethod]
	public void LeavingSpawnAreaKeepsEnemyAndItsKillStillCounts()
	{
		var invasion = CreateActiveInvasion();
		var enemy = CreateEnemy();
		TrackEnemy(invasion, enemy);
		enemy.position = new Vector2(2000, 2000);
		invasion.PostUpdateEverything();
		Assert.IsTrue(enemy.active);
		Assert.AreEqual(0, invasion.DefeatedEnemies);
		hook.OnKill(enemy);
		Assert.AreEqual(1, invasion.DefeatedEnemies);
	}
	[TestMethod]
	public void StoppingOrUnloadingDoesNotMeanSuccess()
	{
		var invasion = CreateActiveInvasion();
		var enemy = CreateEnemy();
		TrackEnemy(invasion, enemy);
		EventSystem.Deactivate(invasion);
		Assert.IsFalse(enemy.active);
		Assert.IsFalse(invasion.Completed);
		Assert.IsFalse(invasion.Downed);
		EventSystem.Activate(invasion);
		system.OnWorldUnload();
		Assert.IsFalse(invasion.Active);
		Assert.IsFalse(invasion.Completed);
		Assert.IsFalse(invasion.Downed);
	}

	[TestMethod]
	public void SaveRestoresRemainingKillsWithoutReplayingActivation()
	{
		var invasion = CreateActiveInvasion();
		var first = CreateEnemy();
		TrackEnemy(invasion, first);
		hook.OnKill(first);
		var tag = new TagCompound();
		system.SaveWorldData(tag);
		system.LoadWorldData(tag);
		Assert.IsTrue(invasion.Active);
		Assert.AreEqual(1, invasion.DefeatedEnemies);
		for (int i = 1; i < invasion.TargetCount; i++)
		{
			var replacement = CreateEnemy();
			TrackEnemy(invasion, replacement);
			hook.OnKill(replacement);
		}
		Assert.IsTrue(invasion.Completed);
		Assert.IsTrue(invasion.Downed);
	}

	[TestMethod]
	public void LateJoinReceivesCompletedResultAfterEventStops()
	{
		var invasion = CreateActiveInvasion();
		for (int i = 0; i < invasion.TargetCount; i++)
		{
			var npc = CreateEnemy();
			TrackEnemy(invasion, npc);
			hook.OnKill(npc);
		}
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		system.NetSend(writer);
		system.ClearWorld();
		Main.netMode = NetmodeID.MultiplayerClient;
		stream.Position = 0;
		system.NetReceive(new BinaryReader(stream));
		Assert.IsFalse(invasion.Active);
		Assert.IsTrue(invasion.Completed);
		Assert.IsTrue(invasion.Downed);
		Assert.AreEqual(invasion.TargetCount, invasion.DefeatedEnemies);
	}

	[TestMethod]
	public void DownedSurvivesRestartSaveAndNetworkSnapshotUntilWorldReset()
	{
		var invasion = CreateActiveInvasion();
		for (int i = 0; i < invasion.TargetCount; i++)
		{
			var npc = CreateEnemy();
			TrackEnemy(invasion, npc);
			hook.OnKill(npc);
		}
		Assert.IsTrue(EventSystem.Activate(invasion));
		Assert.IsTrue(invasion.Downed);
		Assert.IsFalse(invasion.Completed);
		Assert.AreEqual(0, invasion.DefeatedEnemies);

		var tag = new TagCompound();
		system.SaveWorldData(tag);
		system.ClearWorld();
		Assert.IsFalse(invasion.Downed);
		system.LoadWorldData(tag);
		Assert.IsTrue(invasion.Downed);
		Assert.IsTrue(invasion.Active);
		Assert.IsFalse(invasion.Completed);

		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		system.NetSend(writer);
		system.ClearWorld();
		Main.netMode = NetmodeID.MultiplayerClient;
		stream.Position = 0;
		system.NetReceive(new BinaryReader(stream));
		Assert.IsTrue(invasion.Downed);
		Assert.IsTrue(invasion.Active);
		Assert.IsFalse(invasion.Completed);
		system.ClearWorld();
		Assert.IsFalse(invasion.Downed);
	}

	private static NPC CreateEnemy() => new()
	{
		active = true, life = 10, type = NPCID.Zombie,
		position = new Vector2(100, 100), width = 32, height = 48,
	};

	private static void TrackEnemy(DrunkenMinerInvasion invasion, NPC npc) =>
		typeof(DrunkenMinerInvasion).GetMethod("TrackEnemy", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(invasion, [npc]);

	private static DrunkenMinerInvasion CreateActiveInvasion()
	{
		var invasion = CreateInvasion();
		Assert.IsTrue(EventSystem.Activate(invasion));
		return invasion;
	}

	private static DrunkenMinerInvasion CreateInvasion()
	{
		var invasion = new DrunkenMinerInvasion();
		typeof(ModType).GetProperty(nameof(ModType.Mod))!.SetValue(invasion, new TestMod());
		invasion.Register();
		SetInvasion(invasion, [invasion]);
		return invasion;
	}

	private WorldObjectiveBase CreateObjective(DrunkenMinerInvasion invasion)
	{
		SetInvasion(invasion, [invasion]);
		var objective = (WorldObjectiveBase)Activator.CreateInstance(
			typeof(DefendTownQuest).GetNestedType("DrunkenMinerInvasionObjective", BindingFlags.NonPublic)!, nonPublic: true)!;
		objective.Activate(null!);
		objectives.Add(objective);
		return objective;
	}

	private static void SetInvasion(DrunkenMinerInvasion invasion, IReadOnlyList<DrunkenMinerInvasion> invasions)
	{
		typeof(ContentInstance<DrunkenMinerInvasion>).GetProperty(nameof(ContentInstance<DrunkenMinerInvasion>.Instance))!.SetValue(null, invasion);
		typeof(ContentInstance<DrunkenMinerInvasion>).GetProperty(nameof(ContentInstance<DrunkenMinerInvasion>.Instances))!.SetValue(null, invasions);
	}

	private static void Transfer(Action<BinaryWriter> send, Action<BinaryReader> receive)
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		send(writer);
		stream.Position = 0;
		receive(new BinaryReader(stream));
	}
	private sealed class TestMod : Mod
	{
		private readonly string name = "MinerTests" + Guid.NewGuid().ToString("N");
		public override string Name => name;
	}
}
