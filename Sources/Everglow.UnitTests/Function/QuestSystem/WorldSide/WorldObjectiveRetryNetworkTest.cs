using System.Reflection;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Utilities;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldObjectiveRetryNetworkTest
{
	private int _originalNetMode;

	private sealed class TestQuest : WorldQuestBase
	{
		public TestQuest()
		{
			Objective = new TestObjective { ProgressValue = 7 };
			Objective.WithTimeLimit(WorldQuestManager.UpdateInterval);
			Objectives.Add(Objective);
		}

		public override string Name => nameof(WorldObjectiveRetryNetworkTest);

		public TestObjective Objective { get; }

		public void ActivateAndExpire()
		{
			State = WorldQuestState.Active;
			Activate();
			Objectives.UpdateNode();
		}
	}

	private sealed class TestObjective : WorldObjectiveBase
	{
		public int ProgressValue { get; set; }

		public int Activations { get; private set; }

		public int ResetCalls { get; private set; }

		public override float Progress => ProgressValue / 10f;

		public override bool CheckCompletion() => false;

		public override void Activate(WorldQuestBase sourceQuest) => Activations++;

		public override void ResetProgress()
		{
			base.ResetProgress();
			ProgressValue = 0;
			ResetCalls++;
		}

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}

		public override string GetObjectiveText() => string.Empty;

		public override void NetSend(BinaryWriter writer)
		{
			base.NetSend(writer);
			writer.Write(ProgressValue);
		}

		public override void NetReceive(BinaryReader reader)
		{
			base.NetReceive(reader);
			ProgressValue = reader.ReadInt32();
		}
	}

	private sealed class TestGameStateProvider : IGameStateProvider
	{
		public double TimeForVisualEffects => 0;

		public bool GameMenu => false;

		public bool GameInactive => false;

		public bool GamePaused => false;
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
	public void SinglePlayerAction_AppliesImmediatelyAndPublishesOnce()
	{
		var manager = CreateManager();
		var quest = AddExpiredQuest(manager);
		var actions = new WorldQuestActions(manager);
		int objectiveUpdates = 0;
		manager.QuestObjectiveUpdated += _ => objectiveUpdates++;
		var action = new QuestAction(GetIdentity(quest), QuestActionType.Retry, quest.Objective.ObjectiveID);

		bool applied = actions.TryExecute(action);
		bool repeated = actions.TryExecute(action);

		Assert.IsTrue(applied);
		Assert.IsFalse(repeated);
		Assert.AreEqual(0, quest.Objective.ProgressValue);
		Assert.AreEqual(0, quest.Objective.Timer.ElapsedTime);
		Assert.AreEqual(1, quest.Objective.ResetCalls);
		Assert.AreEqual(1, objectiveUpdates);
	}

	[TestMethod]
	public void NonAuthoritativeManager_RejectsRetryWithoutMutation()
	{
		var manager = CreateManager();
		var quest = AddExpiredQuest(manager);
		Main.netMode = NetmodeID.MultiplayerClient;

		bool applied = InvokeManagerRetry(manager, quest.Name, quest.Objective.ObjectiveID);

		Assert.IsFalse(applied);
		Assert.IsTrue(quest.Objective.IsTimedOut);
		Assert.AreEqual(7, quest.Objective.ProgressValue);
		Assert.AreEqual(0, quest.Objective.ResetCalls);
	}

	[TestMethod]
	public void ClientAction_RejectsMismatchedIdentityAndInvalidObjective()
	{
		var manager = CreateManager();
		var quest = AddExpiredQuest(manager);
		var actions = new WorldQuestActions(manager);
		QuestIdentity identity = GetIdentity(quest);
		Main.netMode = NetmodeID.MultiplayerClient;

		Assert.IsFalse(actions.TryExecute(new QuestAction(identity with { InstanceId = "stale" }, QuestActionType.Retry, quest.Objective.ObjectiveID)));
		Assert.IsFalse(actions.TryExecute(new QuestAction(identity with { Side = QuestSide.Player }, QuestActionType.Retry, quest.Objective.ObjectiveID)));
		Assert.IsFalse(actions.TryExecute(new QuestAction(identity, QuestActionType.Retry, -1)));
		Assert.IsFalse(actions.TryExecute(new QuestAction(identity, QuestActionType.Retry, quest.Objectives.AllObjectives.Count)));
		Assert.IsFalse(actions.TryExecute(new QuestAction(identity, QuestActionType.Retry, quest.Objective.ObjectiveID.ToString())));
		Assert.IsTrue(quest.Objective.IsTimedOut);
	}

	[TestMethod]
	public void MainServerManager_RejectsInvalidRequestWithoutMutation()
	{
		var manager = CreateManager();
		var quest = AddExpiredQuest(manager);
		Main.netMode = NetmodeID.Server;
		Assert.IsTrue(NetUtils.IsMainServer);

		Assert.IsFalse(InvokeManagerRetry(manager, "MissingQuest", quest.Objective.ObjectiveID));
		Assert.IsFalse(InvokeManagerRetry(manager, quest.Name, -1));
		Assert.IsFalse(InvokeManagerRetry(manager, quest.Name, quest.Objectives.AllObjectives.Count));
		Assert.IsTrue(quest.Objective.IsTimedOut);
		Assert.AreEqual(7, quest.Objective.ProgressValue);
		Assert.AreEqual(0, quest.Objective.ResetCalls);
	}

	[TestMethod]
	public void AuthoritativeSnapshot_RestoresProgressTimerAndActiveLifecycleDownstream()
	{
		var manager = CreateManager();
		var authoritative = AddExpiredQuest(manager);
		Assert.IsTrue(InvokeManagerRetry(manager, authoritative.Name, authoritative.Objective.ObjectiveID));
		var downstream = new TestQuest();
		downstream.ActivateAndExpire();
		downstream.Objective.ProgressValue = 9;
		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			authoritative.NetSend(writer);
		}
		stream.Position = 0;

		using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			downstream.NetReceive(reader);
		}

		Assert.AreEqual(0, downstream.Objective.ProgressValue);
		Assert.AreEqual(0, downstream.Objective.Timer.ElapsedTime);
		Assert.IsFalse(downstream.Objective.IsTimedOut);
		Assert.AreEqual(2, downstream.Objective.Activations);
		CollectionAssert.AreEqual(new[] { downstream.Objective }, downstream.ActiveObjectives.ToArray());
	}

	private static WorldQuestManager CreateManager() => new(new TestGameStateProvider());

	private static bool InvokeManagerRetry(WorldQuestManager manager, string questName, int objectiveId) =>
		(bool)typeof(WorldQuestManager)
			.GetMethod("TryRetryObjective", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(manager, [questName, objectiveId])!;

	private static TestQuest AddExpiredQuest(WorldQuestManager manager)
	{
		var quest = new TestQuest();
		manager.AddQuest(quest);
		quest.ActivateAndExpire();
		return quest;
	}

	private static QuestIdentity GetIdentity(WorldQuestBase quest) =>
		new(QuestSide.World, quest.Name, quest.Name);
}
