using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.UnitTests.Function.QuestSystem;

public partial class PlayerQuestViewAdapterTest
{
	[TestMethod]
	public void Create_MapsObjectiveTimerSnapshotAndTimedOutState()
	{
		var objective = new StubObjective("timed");
		objective.WithTimeLimit(100);
		objective.Timer.Update(40);
		var quest = new StubQuest { State = PlayerQuestState.Accepted };
		quest.Objectives.Add(objective);
		quest.Activate();

		var activeView = ((LeafObjectiveNodeView)PlayerQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;

		Assert.AreEqual(ObjectiveViewState.Active, activeView.State);
		Assert.IsFalse(activeView.CanRetry);
		Assert.IsNotNull(activeView.Timer);
		Assert.AreEqual(100, activeView.Timer.TimeLimit);
		Assert.AreEqual(40, activeView.Timer.ElapsedTime);
		Assert.AreEqual(60, activeView.Timer.RemainingTime);

		objective.Timer.Update(60);
		var timedOutView = ((LeafObjectiveNodeView)PlayerQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;

		Assert.AreEqual(ObjectiveViewState.TimedOut, timedOutView.State);
		Assert.IsTrue(timedOutView.CanRetry);
		Assert.IsNotNull(timedOutView.Timer);
		Assert.AreEqual(0, timedOutView.Timer.RemainingTime);
		Assert.AreEqual(40, activeView.Timer.ElapsedTime);

		quest.State = PlayerQuestState.Failed;
		var failedView = ((LeafObjectiveNodeView)PlayerQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;
		Assert.IsFalse(failedView.CanRetry);
	}

	[TestMethod]
	public void Create_UntimedObjectiveHasNoTimer()
	{
		var quest = new StubQuest();
		quest.Objectives.Add(new StubObjective("untimed"));

		var view = ((LeafObjectiveNodeView)PlayerQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;

		Assert.IsNull(view.Timer);
	}

	[TestMethod]
	public void Create_ObjectiveRetryCapabilityTracksHintAndRetryWithoutMutatingOldSnapshot()
	{
		var objective = new StubObjective("timed");
		objective.WithTimeLimit(20);
		var quest = new StubQuest { State = PlayerQuestState.Accepted };
		quest.Objectives.Add(objective);
		quest.Activate();
		quest.Objectives.Update(quest);
		var expiredView = ((LeafObjectiveNodeView)PlayerQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;
		Assert.IsTrue(expiredView.CanRetry);
		Assert.AreEqual(20, objective.Timer.ElapsedTime);

		quest.HintValue = "Hidden details";
		var hintedView = ((LeafObjectiveNodeView)PlayerQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;
		Assert.IsFalse(hintedView.CanRetry);
		quest.HintValue = " ";
		Assert.IsTrue(quest.TryRetryObjectiveCore(objective.ObjectiveID));
		var retriedView = ((LeafObjectiveNodeView)PlayerQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;

		Assert.AreEqual(ObjectiveViewState.Active, retriedView.State);
		Assert.IsFalse(retriedView.CanRetry);
		Assert.AreEqual(0, retriedView.Timer.ElapsedTime);
		Assert.AreEqual(ObjectiveViewState.TimedOut, expiredView.State);
		Assert.IsTrue(expiredView.CanRetry);
		Assert.AreEqual(20, expiredView.Timer.ElapsedTime);
	}

	[TestMethod]
	public void Create_CompletedAndSkippedStatesTakePriorityOverTimedOut()
	{
		var completed = new StubObjective("completed");
		completed.WithTimeLimit(1);
		completed.Timer.Update(1);
		completed.Complete();

		var skipped = new StubObjective("skipped");
		skipped.WithTimeLimit(1);
		skipped.Timer.Update(1);
		var selected = new StubObjective("selected") { Ready = true };
		var quest = new StubQuest { State = PlayerQuestState.Accepted };
		quest.Objectives.Add(completed).AddBranch([skipped], [selected]);
		quest.Objectives.Activate(quest);
		quest.Objectives.Update(quest);

		QuestView view = PlayerQuestViewAdapter.Create(quest);
		var completedView = ((LeafObjectiveNodeView)view.ObjectiveNodes[0]).Objective;
		var branchView = (BranchObjectiveNodeView)view.ObjectiveNodes[1];

		Assert.AreEqual(ObjectiveViewState.Completed, completedView.State);
		Assert.AreEqual(ObjectiveViewState.Skipped, branchView.Branches[0].Objectives[0].State);
	}
}
