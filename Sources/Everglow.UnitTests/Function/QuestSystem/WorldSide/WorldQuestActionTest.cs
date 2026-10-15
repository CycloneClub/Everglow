using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldQuestActionTest
{
	private sealed class StubQuest : WorldQuestBase
	{
		public string HintValue { get; set; } = string.Empty;

		public StubQuest()
		{
			Objectives.Add(new StubObjective());
		}

		public void SetState(WorldQuestState state) => State = state;

		public void SetTime(int time) => Time = time;

		public override string Hint => HintValue;
	}

	private sealed class StubObjective : WorldObjectiveBase
	{
		public override bool CheckCompletion() => false;

		public override string GetObjectiveText() => string.Empty;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}
	}

	private sealed class StubGameStateProvider : IGameStateProvider
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
		Main.netMode = NetmodeID.SinglePlayer;
		Main.myPlayer = 0;
		Main.player[Main.myPlayer] = new Player { name = "ActionTester" };
	}

	[TestMethod]
	public void FailedSinglePlayerQuest_ExportsOnlyRetryAction()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Failed);

		IReadOnlyList<QuestAction> actions = WorldQuestActionAdapter.GetActions(quest);

		Assert.HasCount(1, actions);
		Assert.AreEqual(QuestActionType.Retry, actions[0].Type);
		Assert.AreEqual(quest.Name, actions[0].Quest.DefinitionId);
		Assert.AreEqual(quest.Name, actions[0].Quest.InstanceId);
	}

	[TestMethod]
	public void CompletedUnclaimedSinglePlayerQuest_ExportsOnlyClaimRewardAction()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Completed);

		IReadOnlyList<QuestAction> actions = WorldQuestActionAdapter.GetActions(quest);

		Assert.HasCount(1, actions);
		Assert.AreEqual(QuestActionType.ClaimReward, actions[0].Type);
	}

	[TestMethod]
	public void RetryAction_ChangesStateOnceAndKeepsWorldIdentity()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Failed);
		quest.SetTime(120);
		var manager = new WorldQuestManager(new StubGameStateProvider());
		manager.AddQuest(quest);
		var actions = new WorldQuestActions(manager);
		QuestAction action = WorldQuestActionAdapter.GetActions(quest).Single();

		bool applied = actions.TryExecute(action);
		bool repeated = actions.TryExecute(action);

		Assert.IsTrue(applied);
		Assert.IsFalse(repeated);
		Assert.AreEqual(WorldQuestState.Active, quest.State);
		Assert.AreEqual(0, quest.Time);
		Assert.AreEqual(quest.Name, action.Quest.InstanceId);
	}

	[TestMethod]
	public void RetryAction_WithUnexpectedArgs_IsRejected()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Failed);
		var manager = new WorldQuestManager(new StubGameStateProvider());
		manager.AddQuest(quest);
		var actions = new WorldQuestActions(manager);
		QuestAction action = WorldQuestActionAdapter.GetActions(quest).Single() with { Args = "unexpected" };

		bool applied = actions.TryExecute(action);

		Assert.IsFalse(applied);
		Assert.AreEqual(WorldQuestState.Failed, quest.State);
	}

	[TestMethod]
	public void RetryAction_PublishesStatusAndObjectiveUpdates()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Failed);
		var manager = new WorldQuestManager(new StubGameStateProvider());
		manager.AddQuest(quest);
		var actions = new WorldQuestActions(manager);
		int statusUpdateCount = 0;
		int objectiveUpdateCount = 0;
		manager.QuestStatusUpdated += _ => statusUpdateCount++;
		manager.QuestObjectiveUpdated += _ => objectiveUpdateCount++;
		QuestAction action = WorldQuestActionAdapter.GetActions(quest).Single();

		bool applied = actions.TryExecute(action);

		Assert.IsTrue(applied);
		Assert.AreEqual(1, statusUpdateCount);
		Assert.AreEqual(1, objectiveUpdateCount);
	}

	[TestMethod]
	public void RetryAction_PublishesRestartedNotification()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Failed);
		var manager = new WorldQuestManager(new StubGameStateProvider());
		manager.AddQuest(quest);
		var actions = new WorldQuestActions(manager);
		QuestAction action = WorldQuestActionAdapter.GetActions(quest).Single();
		QuestNotification? notification = null;
		WorldQuestManager.NotificationRequested += CaptureNotification;

		try
		{
			bool applied = actions.TryExecute(action);

			Assert.IsTrue(applied);
			Assert.AreEqual(
				new QuestNotification(action.Quest, QuestNotificationType.Restarted),
				notification);
		}
		finally
		{
			WorldQuestManager.NotificationRequested -= CaptureNotification;
		}

		void CaptureNotification(QuestNotification value) => notification = value;
	}

	[TestMethod]
	public void ClaimRewardAction_ClaimsOnceForLocalPlayer()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Completed);
		var manager = new WorldQuestManager(new StubGameStateProvider());
		manager.AddQuest(quest);
		var actions = new WorldQuestActions(manager);
		QuestAction action = WorldQuestActionAdapter.GetActions(quest).Single();

		bool applied = actions.TryExecute(action);
		bool repeated = actions.TryExecute(action);

		Assert.IsTrue(applied);
		Assert.IsFalse(repeated);
		Assert.IsTrue(quest.RewardClaimedPlayers.Contains("ActionTester"));
	}

	[TestMethod]
	public void CompletedQuest_ExportsClaimForOtherUnclaimedName()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Completed);
		Assert.IsTrue(quest.TryRecordRewardClaim("ActionTester"));

		IReadOnlyList<QuestAction> claimedActions = WorldQuestActionAdapter.GetActions(quest);
		Main.LocalPlayer.name = "OtherPlayer";
		IReadOnlyList<QuestAction> otherActions = WorldQuestActionAdapter.GetActions(quest);

		Assert.IsEmpty(claimedActions);
		Assert.HasCount(1, otherActions);
		Assert.AreEqual(QuestActionType.ClaimReward, otherActions[0].Type);
	}

	[TestMethod]
	[DataRow("Follow the trail")]
	[DataRow(QuestHintText.Masked)]
	public void HintedFailedSinglePlayerQuest_ExportsNoActions(string hint)
	{
		var quest = new StubQuest { HintValue = hint };
		quest.SetState(WorldQuestState.Failed);

		IReadOnlyList<QuestAction> actions = WorldQuestActionAdapter.GetActions(quest);

		Assert.IsEmpty(actions);
	}

	[TestMethod]
	[DataRow(" ")]
	[DataRow("\t")]
	public void WhitespaceHint_DoesNotHideOrRejectRetryAction(string hint)
	{
		var quest = new StubQuest { HintValue = hint };
		quest.SetState(WorldQuestState.Failed);
		var manager = new WorldQuestManager(new StubGameStateProvider());
		manager.AddQuest(quest);
		var worldActions = new WorldQuestActions(manager);

		QuestAction action = WorldQuestActionAdapter.GetActions(quest).Single();
		bool applied = worldActions.TryExecute(action);

		Assert.IsTrue(applied);
		Assert.AreEqual(WorldQuestState.Active, quest.State);
	}

	[TestMethod]
	public void MultiplayerQuest_ExportsNoActionsAndRejectsStaleSinglePlayerAction()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Failed);
		QuestAction singlePlayerAction = WorldQuestActionAdapter.GetActions(quest).Single();
		var manager = new WorldQuestManager(new StubGameStateProvider());
		manager.AddQuest(quest);
		var worldActions = new WorldQuestActions(manager);
		Main.netMode = NetmodeID.MultiplayerClient;

		IReadOnlyList<QuestAction> actions = WorldQuestActionAdapter.GetActions(quest);
		bool applied = worldActions.TryExecute(singlePlayerAction);

		Assert.IsEmpty(actions);
		Assert.IsFalse(applied);
		Assert.AreEqual(WorldQuestState.Failed, quest.State);
	}

	[TestMethod]
	public void WorldActionWithMismatchedIdentity_DoesNotChangeQuest()
	{
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Failed);
		var manager = new WorldQuestManager(new StubGameStateProvider());
		manager.AddQuest(quest);
		var actions = new WorldQuestActions(manager);
		var action = new QuestAction(
			new(QuestSide.World, quest.Name, "runtime-index"),
			QuestActionType.Retry);

		bool applied = actions.TryExecute(action);

		Assert.IsFalse(applied);
		Assert.AreEqual(WorldQuestState.Failed, quest.State);
	}
}
