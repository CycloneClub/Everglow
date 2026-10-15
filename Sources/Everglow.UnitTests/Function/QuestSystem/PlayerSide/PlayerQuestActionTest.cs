using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class PlayerQuestActionTest
{
	private PlayerQuestManager _manager;
	private PlayerQuestActions _actions;

	private sealed class StubQuest : PlayerQuestBase
	{
		public bool CancellableValue { get; set; }

		public bool CompleteValue { get; set; }

		public string HintValue { get; set; } = string.Empty;

		public override string DisplayName => nameof(StubQuest);

		public override bool Cancellable => CancellableValue;

		public override string Hint => HintValue;

		public override bool CheckComplete() => CompleteValue;
	}

	[TestInitialize]
	public void Initialize()
	{
		_manager = new PlayerQuestManager();
		_actions = new PlayerQuestActions(_manager);
	}

	[TestCleanup]
	public void Cleanup() => _manager.Clear();

	[TestMethod]
	public void AvailableQuest_ExportsOnlyAcceptAction()
	{
		var quest = new StubQuest { State = PlayerQuestState.Available };

		IReadOnlyList<QuestAction> actions = PlayerQuestActionAdapter.GetActions(quest);

		Assert.HasCount(1, actions);
		Assert.AreEqual(QuestActionType.Accept, actions[0].Type);
		Assert.AreEqual(quest.Name, actions[0].Quest.DefinitionId);
		Assert.AreEqual(quest.InstanceId, actions[0].Quest.InstanceId);
	}

	[TestMethod]
	public void CancellableIncompleteActiveQuest_ExportsOnlyCancelAction()
	{
		var quest = new StubQuest
		{
			State = PlayerQuestState.Accepted,
			CancellableValue = true,
		};

		IReadOnlyList<QuestAction> actions = PlayerQuestActionAdapter.GetActions(quest);

		Assert.HasCount(1, actions);
		Assert.AreEqual(QuestActionType.Cancel, actions[0].Type);
	}

	[TestMethod]
	public void CompleteAcceptedQuest_ExportsOnlySubmitAction()
	{
		var quest = new StubQuest
		{
			State = PlayerQuestState.Accepted,
			CancellableValue = true,
			CompleteValue = true,
		};

		IReadOnlyList<QuestAction> actions = PlayerQuestActionAdapter.GetActions(quest);

		Assert.HasCount(1, actions);
		Assert.AreEqual(QuestActionType.Submit, actions[0].Type);
	}

	[TestMethod]
	public void HintedCompleteAcceptedQuest_ExportsNoActions()
	{
		var quest = new StubQuest
		{
			State = PlayerQuestState.Accepted,
			CompleteValue = true,
			HintValue = QuestHintText.Masked,
		};

		IReadOnlyList<QuestAction> actions = PlayerQuestActionAdapter.GetActions(quest);

		Assert.IsEmpty(actions);
	}

	[TestMethod]
	public void CompletionLostAfterExport_PreventsSubmitExecution()
	{
		var quest = new StubQuest
		{
			State = PlayerQuestState.Accepted,
			CompleteValue = true,
		};
		_manager.ApplyData(new PlayerQuestManagerData([], [quest]));
		QuestAction action = PlayerQuestActionAdapter.GetActions(quest).Single();
		quest.CompleteValue = false;

		bool applied = _actions.TryExecute(action);

		Assert.IsFalse(applied);
		Assert.AreEqual(PlayerQuestState.Accepted, quest.State);
	}

	[TestMethod]
	public void AcceptAction_ChangesStateOnceAndPreservesInstanceIdentity()
	{
		var quest = new StubQuest { State = PlayerQuestState.Available };
		_manager.ApplyData(new PlayerQuestManagerData([], [quest]));
		QuestAction action = PlayerQuestActionAdapter.GetActions(quest).Single();
		string instanceId = quest.InstanceId;
		int statusUpdateCount = 0;
		_manager.QuestStatusUpdated += _ => statusUpdateCount++;

		bool applied = _actions.TryExecute(action);
		bool repeated = _actions.TryExecute(action);

		Assert.IsTrue(applied);
		Assert.IsFalse(repeated);
		Assert.AreEqual(PlayerQuestState.Accepted, quest.State);
		Assert.AreEqual(instanceId, quest.InstanceId);
		Assert.AreEqual(1, statusUpdateCount);
	}

	[TestMethod]
	public void AcceptAction_WithUnexpectedArgs_IsRejected()
	{
		var quest = new StubQuest { State = PlayerQuestState.Available };
		_manager.ApplyData(new PlayerQuestManagerData([], [quest]));
		QuestAction action = PlayerQuestActionAdapter.GetActions(quest).Single() with { Args = 0 };

		bool applied = _actions.TryExecute(action);

		Assert.IsFalse(applied);
		Assert.AreEqual(PlayerQuestState.Available, quest.State);
	}

	[TestMethod]
	[DataRow("Follow the trail")]
	[DataRow(QuestHintText.Masked)]
	public void HintedAvailableQuest_ExportsNoActions(string hint)
	{
		var quest = new StubQuest
		{
			State = PlayerQuestState.Available,
			HintValue = hint,
		};

		IReadOnlyList<QuestAction> actions = PlayerQuestActionAdapter.GetActions(quest);

		Assert.IsEmpty(actions);
	}

	[TestMethod]
	[DataRow(" ")]
	[DataRow("\t")]
	public void WhitespaceHint_DoesNotHideOrRejectAcceptAction(string hint)
	{
		var quest = new StubQuest
		{
			State = PlayerQuestState.Available,
			HintValue = hint,
		};
		_manager.ApplyData(new PlayerQuestManagerData([], [quest]));

		QuestAction action = PlayerQuestActionAdapter.GetActions(quest).Single();
		bool applied = _actions.TryExecute(action);

		Assert.IsTrue(applied);
		Assert.AreEqual(PlayerQuestState.Accepted, quest.State);
	}

	[TestMethod]
	public void CancelAction_ChangesStateOnce()
	{
		var quest = new StubQuest
		{
			State = PlayerQuestState.Accepted,
			CancellableValue = true,
		};
		_manager.ApplyData(new PlayerQuestManagerData([], [quest]));
		QuestAction action = PlayerQuestActionAdapter.GetActions(quest).Single();

		bool applied = _actions.TryExecute(action);
		bool repeated = _actions.TryExecute(action);

		Assert.IsTrue(applied);
		Assert.IsFalse(repeated);
		Assert.AreEqual(PlayerQuestState.Failed, quest.State);
	}

	[TestMethod]
	public void ActionForReplacedPlayerInstance_DoesNotAffectCurrentInstance()
	{
		var replaced = new StubQuest { State = PlayerQuestState.Available };
		QuestAction staleAction = PlayerQuestActionAdapter.GetActions(replaced).Single();
		var current = new StubQuest { State = PlayerQuestState.Available };
		_manager.ApplyData(new PlayerQuestManagerData([], [current]));

		bool applied = _actions.TryExecute(staleAction);

		Assert.IsFalse(applied);
		Assert.AreEqual(PlayerQuestState.Available, current.State);
		Assert.AreNotEqual(staleAction.Quest.InstanceId, current.InstanceId);
	}

	[TestMethod]
	public void HintAddedAfterExport_PreventsExecution()
	{
		var quest = new StubQuest { State = PlayerQuestState.Available };
		_manager.ApplyData(new PlayerQuestManagerData([], [quest]));
		QuestAction action = PlayerQuestActionAdapter.GetActions(quest).Single();
		quest.HintValue = QuestHintText.Masked;

		bool applied = _actions.TryExecute(action);

		Assert.IsFalse(applied);
		Assert.AreEqual(PlayerQuestState.Available, quest.State);
	}

	[TestMethod]
	public void InvisibleAvailableQuest_StillExportsAcceptAction()
	{
		var quest = new StubQuest
		{
			State = PlayerQuestState.Available,
			IsVisible = false,
		};

		IReadOnlyList<QuestAction> actions = PlayerQuestActionAdapter.GetActions(quest);

		Assert.HasCount(1, actions);
		Assert.AreEqual(QuestActionType.Accept, actions[0].Type);
	}
}
