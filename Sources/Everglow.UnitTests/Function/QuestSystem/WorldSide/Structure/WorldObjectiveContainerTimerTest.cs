using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Structure;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldObjectiveContainerTimerTest
{
	private int _originalNetMode;

	private sealed class TestQuest : WorldQuestBase
	{
		public void SetActive()
		{
			State = WorldQuestState.Active;
			Activate();
		}
	}

	private sealed class TestObjective : WorldObjectiveBase
	{
		public bool Ready { get; set; }

		public bool CompleteDuringUpdate { get; set; }

		public int Activations { get; private set; }

		public int Deactivations { get; private set; }

		public int UpdateCalls { get; private set; }

		public void MarkSyncNeeded() => NeedDeltaSync = true;

		public override bool CheckCompletion() => Ready;

		public override void Update()
		{
			UpdateCalls++;
			Ready |= CompleteDuringUpdate;
		}

		public override void Activate(WorldQuestBase sourceQuest) => Activations++;

		public override void Deactivate() => Deactivations++;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}

		public override string GetObjectiveText() => string.Empty;
	}

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalNetMode = Main.netMode;
		Main.netMode = NetmodeID.SinglePlayer;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.netMode = _originalNetMode;
	}

	[TestMethod]
	public void TimedObjective_AdvancesOnlyWhileActiveAndExpiresInPlace()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval * 2);
		var pending = new TestObjective();
		pending.WithTimeLimit(WorldQuestManager.UpdateInterval);
		var quest = new TestQuest();
		quest.Objectives.Add(timed).Add(pending);
		quest.SetActive();

		quest.Objectives.UpdateNode();

		Assert.AreEqual(WorldQuestManager.UpdateInterval, timed.Timer.ElapsedTime);
		Assert.AreEqual(0, pending.Timer.ElapsedTime);

		quest.Objectives.UpdateNode();

		Assert.IsTrue(timed.IsTimedOut);
		Assert.IsFalse(timed.Completed);
		Assert.AreSame(timed, quest.Objectives.FindCurrentObjectives().Single());
		Assert.IsEmpty(quest.ActiveObjectives);
		Assert.AreEqual(1, timed.Deactivations);

		quest.Objectives.UpdateNode();

		Assert.AreEqual(2, timed.UpdateCalls);
		Assert.AreEqual(WorldQuestManager.UpdateInterval * 2, timed.Timer.ElapsedTime);
		Assert.AreEqual(1, timed.Deactivations);
		Assert.AreEqual(0, pending.Timer.ElapsedTime);
	}

	[TestMethod]
	public void NetReceive_ExpiredTimerSnapshotDeactivatesLocallyActiveObjective()
	{
		var sentObjective = new TestObjective();
		sentObjective.WithTimeLimit(10);
		sentObjective.Timer.Update(10);
		var sentQuest = new TestQuest();
		sentQuest.Objectives.Add(sentObjective);
		sentQuest.SetActive();

		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			sentQuest.NetSend(writer);
		}

		var receivedObjective = new TestObjective();
		receivedObjective.WithTimeLimit(10);
		var receivedQuest = new TestQuest();
		receivedQuest.Objectives.Add(receivedObjective);
		receivedQuest.SetActive();
		stream.Position = 0;
		using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			receivedQuest.NetReceive(reader);
		}

		Assert.IsTrue(receivedObjective.IsTimedOut);
		Assert.IsEmpty(receivedQuest.ActiveObjectives);
		Assert.AreEqual(1, receivedObjective.Deactivations);
	}

	[TestMethod]
	public void NetReceive_RestoredTimerSnapshotReactivatesObjective()
	{
		var sentObjective = new TestObjective();
		sentObjective.WithTimeLimit(10);
		sentObjective.Timer.Update(5);
		var sentQuest = new TestQuest();
		sentQuest.Objectives.Add(sentObjective);
		sentQuest.SetActive();

		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			sentQuest.NetSend(writer);
		}

		var receivedObjective = new TestObjective();
		receivedObjective.WithTimeLimit(10);
		receivedObjective.Timer.Update(10);
		var receivedQuest = new TestQuest();
		receivedQuest.Objectives.Add(receivedObjective);
		receivedQuest.SetActive();
		stream.Position = 0;
		using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			receivedQuest.NetReceive(reader);
		}

		Assert.IsFalse(receivedObjective.IsTimedOut);
		Assert.AreEqual(5, receivedObjective.Timer.ElapsedTime);
		CollectionAssert.AreEqual(new[] { receivedObjective }, receivedQuest.ActiveObjectives.ToArray());
		Assert.AreEqual(1, receivedObjective.Activations);
	}

	[TestMethod]
	[DataRow("Parallel")]
	[DataRow("Optional")]
	[DataRow("Branch")]
	public void CompositeNode_DoesNotUpdateTimedOutLeaf(string nodeType)
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval);
		var sibling = new TestObjective();
		var quest = new TestQuest();
		switch (nodeType)
		{
			case "Parallel":
				quest.Objectives.AddParallel(timed, sibling);
				break;
			case "Optional":
				quest.Objectives.AddOptional(timed, sibling);
				break;
			case "Branch":
				quest.Objectives.AddBranch([timed], [sibling]);
				break;
			default:
				Assert.Fail($"Unknown node type {nodeType}.");
				break;
		}
		quest.SetActive();

		quest.Objectives.UpdateNode();
		quest.Objectives.UpdateNode();

		Assert.IsTrue(timed.IsTimedOut);
		Assert.AreEqual(1, timed.UpdateCalls);
		Assert.AreEqual(2, sibling.UpdateCalls);
	}

	[TestMethod]
	public void TimedObjective_CompletionWinsAtExpiryBoundary()
	{
		var timed = new TestObjective { CompleteDuringUpdate = true };
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval);
		var next = new TestObjective();
		WorldObjectiveContainer objectives = CreateStandaloneContainer();
		objectives.Add(timed).Add(next);
		objectives.Activate();

		objectives.UpdateNode();

		Assert.IsTrue(timed.Completed);
		Assert.IsFalse(timed.IsTimedOut);
		Assert.AreEqual(0, timed.Timer.ElapsedTime);
		Assert.AreSame(next, objectives.FindCurrentObjectives().Single());
	}

	[TestMethod]
	public void TimedObjective_ResetAllowsReactivation()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		quest.SetActive();
		quest.Objectives.UpdateNode();

		quest.Reset();
		quest.SetActive();

		Assert.IsFalse(timed.IsTimedOut);
		Assert.AreEqual(0, timed.Timer.ElapsedTime);
		Assert.AreEqual(2, timed.Activations);
		CollectionAssert.AreEqual(new[] { timed }, quest.ActiveObjectives.ToArray());
	}

	[TestMethod]
	public void NewlyActivatedTimedObjective_DoesNotConsumePreviousInterval()
	{
		var first = new TestObjective { Ready = true };
		var second = new TestObjective();
		second.WithTimeLimit(WorldQuestManager.UpdateInterval);
		WorldObjectiveContainer objectives = CreateStandaloneContainer();
		objectives.Add(first).Add(second);
		objectives.Activate();

		objectives.UpdateNode();

		Assert.AreSame(second, objectives.FindCurrentObjectives().Single());
		Assert.AreEqual(0, second.Timer.ElapsedTime);
		Assert.IsFalse(second.IsTimedOut);
	}

	[TestMethod]
	public void OnMPSync_DoesNotUploadTimedOutObjectiveProgress()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(1);
		timed.Timer.Update(1);
		timed.MarkSyncNeeded();
		var objectives = new WorldObjectiveContainer();
		objectives.Add(timed);
		int syncRequests = 0;
		objectives.OnMPSyncTriggered += _ => syncRequests++;

		objectives.OnMPSync();

		Assert.AreEqual(0, syncRequests);
	}

	[TestMethod]
	public void ExpiringObjective_FlushesPendingDeltaBeforeDeactivation()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval);
		timed.MarkSyncNeeded();
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		quest.SetActive();
		int syncRequests = 0;
		quest.Objectives.OnMPSyncTriggered += objective =>
		{
			Assert.AreSame(timed, objective);
			Assert.AreEqual(0, timed.Deactivations);
			syncRequests++;
		};

		quest.Objectives.UpdateNode();

		Assert.IsTrue(timed.IsTimedOut);
		Assert.AreEqual(1, syncRequests);
		Assert.AreEqual(1, timed.Deactivations);
	}

	private static WorldObjectiveContainer CreateStandaloneContainer()
	{
		var objectives = new WorldObjectiveContainer();
		objectives.OnObjectiveActivated += _ => { };
		objectives.OnObjectiveDeactivated += () => { };
		objectives.OnNodeCompleted += _ => { };
		return objectives;
	}
}
