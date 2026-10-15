using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Structure.Nodes;

namespace Everglow.UnitTests.Function.QuestSystem;

public partial class WorldQuestViewAdapterTest
{
	[TestMethod]
	public void Create_MapsWorldObjectiveTimerSnapshotAndTimedOutState()
	{
		var objective = new StubObjective { ObjectiveTextValue = "timed" };
		objective.WithTimeLimit(100);
		objective.Timer.Update(40);
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Active);
		quest.Objectives.Add(objective);
		quest.Activate();

		var activeView = ((LeafObjectiveNodeView)WorldQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;

		Assert.AreEqual(ObjectiveViewState.Active, activeView.State);
		Assert.IsFalse(activeView.CanRetry);
		Assert.IsNotNull(activeView.Timer);
		Assert.AreEqual(100, activeView.Timer.TimeLimit);
		Assert.AreEqual(40, activeView.Timer.ElapsedTime);
		Assert.AreEqual(60, activeView.Timer.RemainingTime);

		objective.Timer.Update(60);
		var timedOutView = ((LeafObjectiveNodeView)WorldQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;

		Assert.AreEqual(ObjectiveViewState.TimedOut, timedOutView.State);
		Assert.IsTrue(timedOutView.CanRetry);
		Assert.IsNotNull(timedOutView.Timer);
		Assert.AreEqual(0, timedOutView.Timer.RemainingTime);

		quest.SetState(WorldQuestState.Failed);
		var failedQuestView = ((LeafObjectiveNodeView)WorldQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;

		Assert.IsFalse(failedQuestView.CanRetry);
	}

	[TestMethod]
	public void Create_UntimedWorldObjectiveHasNoTimer()
	{
		var quest = new StubQuest();
		quest.Objectives.Add(new StubObjective());

		var view = ((LeafObjectiveNodeView)WorldQuestViewAdapter.Create(quest).ObjectiveNodes.Single()).Objective;

		Assert.IsNull(view.Timer);
	}

	[TestMethod]
	public void Create_CompletedAndSkippedStatesTakePriorityOverWorldObjectiveTimeout()
	{
		var completed = new StubObjective();
		completed.WithTimeLimit(1);
		completed.Timer.Update(1);
		completed.Complete();

		var skipped = new StubObjective();
		skipped.WithTimeLimit(1);
		skipped.Timer.Update(1);
		var selected = new StubObjective { Ready = true };
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Active);
		quest.Objectives.Add(completed).AddBranch([skipped], [selected]);
		var branchNode = (WorldBranchNode)quest.Objectives.AllNodes[1];
		branchNode.Complete();

		QuestView view = WorldQuestViewAdapter.Create(quest);
		var completedView = ((LeafObjectiveNodeView)view.ObjectiveNodes[0]).Objective;
		var branchView = (BranchObjectiveNodeView)view.ObjectiveNodes[1];

		Assert.AreEqual(ObjectiveViewState.Completed, completedView.State);
		Assert.AreEqual(ObjectiveViewState.Skipped, branchView.Branches[0].Objectives[0].State);
	}
}
