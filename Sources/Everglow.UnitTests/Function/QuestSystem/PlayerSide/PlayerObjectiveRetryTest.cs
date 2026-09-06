using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Hooks;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class PlayerObjectiveRetryTest
{
	private PlayerQuestManager _manager = null!;
	private PlayerQuestActions _actions = null!;
	private int _originalNetMode;

	private sealed class TestQuest : PlayerQuestBase
	{
		public override string DisplayName => nameof(TestQuest);

		public string HintValue { get; set; } = string.Empty;

		public override string Hint => HintValue;
	}

	private sealed class TestObjective : PlayerObjectiveBase
	{
		public int ProgressValue { get; set; }

		public int Activations { get; private set; }

		public int Deactivations { get; private set; }

		public override float Progress => ProgressValue / 10f;

		public override bool CheckCompletion() => ProgressValue >= 10;

		public override void Activate(PlayerQuestBase sourceQuest) => Activations++;

		public override void Deactivate() => Deactivations++;

		public override void ResetProgress()
		{
			base.ResetProgress();
			ProgressValue = 0;
		}

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
		_manager = new PlayerQuestManager();
		_actions = new PlayerQuestActions(_manager);
	}

	[TestCleanup]
	public void Cleanup()
	{
		_manager.Clear();
		Main.netMode = _originalNetMode;
	}

	[TestMethod]
	public void RetryAction_ResetsOnlyExpiredObjectiveAndPublishesOneObjectiveUpdate()
	{
		var timed = new TestObjective { ProgressValue = 7 };
		timed.WithTimeLimit(20);
		var sibling = new TestObjective { ProgressValue = 4 };
		sibling.WithTimeLimit(100);
		var quest = new TestQuest { Time = 60 };
		quest.Objectives.AddParallel(timed, sibling);
		_manager.AddQuest(quest, PlayerQuestState.Accepted, showText: false);
		quest.Objectives.Update(quest);
		var current = quest.Objectives.Current;
		QuestAction action = RetryAction(quest, timed);
		int objectiveUpdates = 0;
		int statusUpdates = 0;
		_manager.QuestObjectiveUpdated += identity =>
		{
			Assert.AreEqual(action.Quest, identity);
			objectiveUpdates++;
		};
		_manager.QuestStatusUpdated += _ => statusUpdates++;

		Assert.IsTrue(_actions.TryExecute(action));
		Assert.IsFalse(_actions.TryExecute(action));

		Assert.AreEqual(0, timed.ProgressValue);
		Assert.AreEqual(0, timed.Timer.ElapsedTime);
		Assert.AreEqual(2, timed.Activations);
		Assert.AreEqual(1, timed.Deactivations);
		CollectionAssert.Contains(quest.Objectives.ActiveObjectives.ToArray(), timed);
		Assert.AreEqual(4, sibling.ProgressValue);
		Assert.AreEqual(20, sibling.Timer.ElapsedTime);
		Assert.AreEqual(1, sibling.Activations);
		Assert.AreEqual(0, sibling.Deactivations);
		Assert.AreSame(current, quest.Objectives.Current);
		Assert.AreEqual(60, quest.Time);
		Assert.AreEqual(PlayerQuestState.Accepted, quest.State);
		Assert.AreEqual(action.Quest.InstanceId, quest.InstanceId);
		Assert.AreEqual(1, objectiveUpdates);
		Assert.AreEqual(0, statusUpdates);
	}

	[TestMethod]
	[DataRow("Leaf")]
	[DataRow("Parallel")]
	[DataRow("Optional")]
	[DataRow("Branch")]
	public void RetryAction_ReopensExpiredEntranceInEveryNodeType(string nodeType)
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(20);
		var sibling = new TestObjective();
		var quest = new TestQuest();
		switch (nodeType)
		{
			case "Leaf":
				quest.Objectives.Add(timed);
				break;
			case "Parallel":
				quest.Objectives.AddParallel(timed, sibling);
				break;
			case "Optional":
				quest.Objectives.AddOptional(timed, sibling);
				break;
			case "Branch":
				quest.Objectives.AddBranch([timed], [sibling]);
				break;
		}
		_manager.AddQuest(quest, PlayerQuestState.Accepted, showText: false);
		quest.Objectives.Update(quest);

		Assert.IsTrue(_actions.TryExecute(RetryAction(quest, timed)));
		Assert.IsFalse(timed.IsTimedOut);
		quest.Objectives.Update(quest);
		Assert.IsTrue(timed.IsTimedOut);
		Assert.AreEqual(2, timed.Deactivations);
	}

	[TestMethod]
	[DataRow(NetmodeID.SinglePlayer)]
	[DataRow(NetmodeID.MultiplayerClient)]
	public void RetryAction_ResumesRealKillSubscriptionWithoutDuplicatingIt(int netMode)
	{
		Main.netMode = netMode;
		var timed = new KillNPCObjective([NPCID.BlueSlime], 3, true);
		timed.WithTimeLimit(20);
		var sibling = new KillNPCObjective([NPCID.BlueSlime], 10, true);
		var quest = new TestQuest();
		quest.Objectives.AddParallel(timed, sibling);
		_manager.AddQuest(quest, PlayerQuestState.Accepted, showText: false);
		var npc = new NPC { type = NPCID.BlueSlime };
		QuestGlobalNPC.TriggerOnKillNPCEvent(npc);
		quest.Objectives.Update(quest);
		QuestGlobalNPC.TriggerOnKillNPCEvent(npc);
		Assert.AreEqual(1, timed.KilledCount);

		Assert.IsTrue(_actions.TryExecute(RetryAction(quest, timed)));
		QuestGlobalNPC.TriggerOnKillNPCEvent(npc);

		Assert.AreEqual(1, timed.KilledCount);
		Assert.AreEqual(3, sibling.KilledCount);
	}

	[TestMethod]
	[DataRow(PlayerQuestState.Available)]
	[DataRow(PlayerQuestState.Completed)]
	[DataRow(PlayerQuestState.Failed)]
	public void RetryAction_RejectsNonAcceptedQuest(PlayerQuestState state)
	{
		var (quest, timed) = CreateExpiredQuest();
		_manager.ChangeQuestState(quest, PlayerQuestState.Accepted, state);

		Assert.IsFalse(_actions.TryExecute(RetryAction(quest, timed)));
		Assert.IsTrue(timed.IsTimedOut);
		Assert.AreEqual(state, quest.State);
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(1)]
	[DataRow("0")]
	[DataRow(null)]
	public void RetryAction_RejectsInvalidArguments(object? args)
	{
		var (quest, timed) = CreateExpiredQuest();

		Assert.IsFalse(_actions.TryExecute(RetryAction(quest, timed) with { Args = args }));
		Assert.IsTrue(timed.IsTimedOut);
	}

	[TestMethod]
	public void RetryAction_RejectsNonRetriableTimedOutObjective()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(20, retriable: false);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		_manager.AddQuest(quest, PlayerQuestState.Accepted, showText: false);
		quest.Objectives.Update(quest);

		Assert.IsFalse(timed.IsRetriable);
		Assert.IsTrue(timed.IsTimedOut);
		Assert.IsFalse(quest.CanRetryObjective(timed.ObjectiveID));
		Assert.IsFalse(_actions.TryExecute(RetryAction(quest, timed)));
		Assert.IsTrue(timed.IsTimedOut);
		Assert.AreEqual(20, timed.Timer.ElapsedTime);
	}

	[TestMethod]
	public void RetryAction_RejectsUntimedUnexpiredAndCompletedObjectives()
	{
		var untimed = new TestObjective();
		var unexpired = new TestObjective();
		unexpired.WithTimeLimit(100);
		var completed = new TestObjective();
		completed.WithTimeLimit(20);
		completed.Timer.Update(20);
		completed.Complete();
		var quest = new TestQuest();
		quest.Objectives.AddParallel(untimed, unexpired, completed);
		_manager.AddQuest(quest, PlayerQuestState.Accepted, showText: false);

		foreach (var objective in new[] { untimed, unexpired, completed })
		{
			Assert.IsFalse(_actions.TryExecute(RetryAction(quest, objective)));
		}
		Assert.IsTrue(completed.Completed);
		Assert.IsTrue(completed.HasGivenRewardItems);
	}

	[TestMethod]
	public void RetryAction_RejectsFutureAndSkippedObjectivesButPreservesSelectedBranch()
	{
		var skipped = new TestObjective();
		skipped.WithTimeLimit(20);
		var selectedHead = new TestObjective();
		var selectedCurrent = new TestObjective();
		selectedCurrent.WithTimeLimit(20);
		var future = new TestObjective();
		future.WithTimeLimit(20);
		future.Timer.Update(20);
		var quest = new TestQuest();
		quest.Objectives.AddBranch([skipped], [selectedHead, selectedCurrent]).Add(future);
		_manager.AddQuest(quest, PlayerQuestState.Accepted, showText: false);
		quest.Objectives.Update(quest);
		Assert.IsFalse(_actions.TryExecute(RetryAction(quest, future)));
		selectedHead.ProgressValue = 10;
		quest.Objectives.Update(quest);
		quest.Objectives.Update(quest);

		Assert.IsFalse(_actions.TryExecute(RetryAction(quest, skipped)));
		Assert.IsTrue(_actions.TryExecute(RetryAction(quest, selectedCurrent)));
		Assert.IsTrue(selectedHead.Completed);
		Assert.IsTrue(selectedHead.HasGivenRewardItems);
		Assert.IsTrue(skipped.IsTimedOut);
		Assert.IsTrue(future.IsTimedOut);
		Assert.AreSame(selectedCurrent, quest.Objectives.FindCurrentObjectives().Single());
	}

	[TestMethod]
	public void RetryAction_RejectsReplacedInstanceAndHintAddedAfterExport()
	{
		var (quest, timed) = CreateExpiredQuest();
		QuestAction staleAction = RetryAction(quest, timed);
		quest.HintValue = QuestHintText.Masked;
		Assert.IsFalse(_actions.TryExecute(staleAction));
		Assert.IsTrue(timed.IsTimedOut);
		_manager.RemoveQuest(quest.Name);
		var (replacement, replacementObjective) = CreateExpiredQuest();

		Assert.IsFalse(_actions.TryExecute(staleAction));
		Assert.IsTrue(replacementObjective.IsTimedOut);
		Assert.AreNotEqual(quest.InstanceId, replacement.InstanceId);
	}

	[TestMethod]
	public void CanRetryObjective_RejectsUnactivatedAndStaleCurrentNode()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(20);
		timed.Timer.Update(20);
		var quest = new TestQuest { State = PlayerQuestState.Accepted };
		quest.Objectives.Add(timed);
		Assert.IsFalse(quest.CanRetryObjective(timed.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(timed.ObjectiveID));

		var first = new TestObjective();
		var other = new TestQuest { State = PlayerQuestState.Accepted };
		other.Objectives.Add(first).Add(timed);
		other.Activate();
		first.Complete();
		Assert.IsFalse(other.CanRetryObjective(timed.ObjectiveID));
		Assert.IsFalse(other.TryRetryObjectiveCore(timed.ObjectiveID));
		Assert.IsTrue(timed.IsTimedOut);
		other.Deactivate();
	}

	[TestMethod]
	public void RetryAction_ReopensTimedOutObjectiveRestoredFromPlayerSave()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(20);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		string instanceId = Guid.NewGuid().ToString("N");
		quest.LoadData(new TagCompound
		{
			{ "State", (int)PlayerQuestState.Accepted },
			{ "QuestTime", 60 },
			{ "InstanceId", instanceId },
			{ "StructuralObjectives", new List<TagCompound>
				{
					new()
					{
						{ "TimerElapsedTime", 20 },
						{ "StructuralCompletionState", 0 },
					},
				}
			},
		});
		_manager.ApplyData(new PlayerQuestManagerData([], [quest]));
		Assert.IsTrue(timed.IsTimedOut);
		Assert.IsEmpty(quest.Objectives.ActiveObjectives);

		Assert.IsTrue(_actions.TryExecute(RetryAction(quest, timed)));
		Assert.AreEqual(0, timed.Timer.ElapsedTime);
		Assert.AreEqual(60, quest.Time);
		Assert.AreEqual(instanceId, quest.InstanceId);
		Assert.AreSame(timed, quest.Objectives.ActiveObjectives.Single());
	}

	private (TestQuest Quest, TestObjective Objective) CreateExpiredQuest()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(20);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		_manager.AddQuest(quest, PlayerQuestState.Accepted, showText: false);
		quest.Objectives.Update(quest);
		return (quest, timed);
	}

	private static QuestAction RetryAction(PlayerQuestBase quest, PlayerObjectiveBase objective) =>
		new(new(QuestSide.Player, quest.Name, quest.InstanceId), QuestActionType.Retry, objective.ObjectiveID);
}
