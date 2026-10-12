using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Structure.Nodes;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldObjectiveRetryTest
{
	private int _originalNetMode;

	private sealed class TestQuest : WorldQuestBase
	{
		public void SetState(WorldQuestState state) => State = state;

		public void SetActive()
		{
			State = WorldQuestState.Active;
			Activate();
		}
	}

	private sealed class TestObjective : WorldObjectiveBase
	{
		public bool Ready { get; set; }

		public int ProgressValue { get; set; }

		public int Activations { get; private set; }

		public int Deactivations { get; private set; }

		public int ResetCalls { get; private set; }

		public override float Progress => ProgressValue / 10f;

		public override bool CheckCompletion() => Ready;

		public override void Activate(WorldQuestBase sourceQuest) => Activations++;

		public override void Deactivate() => Deactivations++;

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
	public void TryRetryObjectiveCore_ResetsOnlyTimedOutCurrentObjectiveAndReactivatesIt()
	{
		var timed = new TestObjective { ProgressValue = 7 };
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval);
		var sibling = new TestObjective { ProgressValue = 4 };
		sibling.WithTimeLimit(WorldQuestManager.UpdateInterval * 3);
		var quest = new TestQuest();
		quest.Objectives.AddParallel(timed, sibling);
		quest.SetActive();
		WorldObjectiveNodeBase currentNode = quest.Objectives.Current;
		quest.Objectives.UpdateNode();
		int siblingElapsedTime = sibling.Timer.ElapsedTime;

		bool retried = quest.TryRetryObjectiveCore(timed.ObjectiveID);

		Assert.IsTrue(retried);
		Assert.AreEqual(0, timed.ProgressValue);
		Assert.AreEqual(0, timed.Timer.ElapsedTime);
		Assert.IsFalse(timed.IsTimedOut);
		Assert.AreEqual(2, timed.Activations);
		Assert.AreEqual(1, timed.ResetCalls);
		CollectionAssert.Contains(quest.ActiveObjectives.ToArray(), timed);
		Assert.AreEqual(4, sibling.ProgressValue);
		Assert.AreEqual(siblingElapsedTime, sibling.Timer.ElapsedTime);
		Assert.AreSame(currentNode, quest.Objectives.Current);
		Assert.AreEqual(WorldQuestState.Active, quest.State);
	}

	[TestMethod]
	[DataRow(WorldQuestState.Locked)]
	[DataRow(WorldQuestState.Completed)]
	[DataRow(WorldQuestState.Failed)]
	public void CanRetryObjective_RejectsNonActiveQuest(WorldQuestState state)
	{
		var timed = new TestObjective { ProgressValue = 7 };
		timed.WithTimeLimit(10);
		timed.Timer.Update(10);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		quest.SetState(state);
		quest.Activate();

		Assert.IsFalse(quest.CanRetryObjective(timed.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(timed.ObjectiveID));
		Assert.AreEqual(7, timed.ProgressValue);
		Assert.IsTrue(timed.IsTimedOut);
		Assert.AreEqual(0, timed.ResetCalls);
	}

	[TestMethod]
	public void CanRetryObjective_RejectsActiveQuestWhoseNodeWasNotActivated()
	{
		var timed = new TestObjective { ProgressValue = 7 };
		timed.WithTimeLimit(10);
		timed.Timer.Update(10);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		quest.SetState(WorldQuestState.Active);

		Assert.IsFalse(quest.CanRetryObjective(timed.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(timed.ObjectiveID));
		Assert.AreEqual(7, timed.ProgressValue);
		Assert.IsTrue(timed.IsTimedOut);
		Assert.AreEqual(0, timed.ResetCalls);
	}

	[TestMethod]
	public void CanRetryObjective_RejectsInvalidObjectiveIdsWithoutThrowing()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		quest.SetActive();
		quest.Objectives.UpdateNode();

		Assert.IsFalse(quest.CanRetryObjective(-1));
		Assert.IsFalse(quest.TryRetryObjectiveCore(-1));
		Assert.IsFalse(quest.CanRetryObjective(quest.Objectives.AllObjectives.Count));
		Assert.IsFalse(quest.TryRetryObjectiveCore(quest.Objectives.AllObjectives.Count));
		Assert.IsTrue(timed.IsTimedOut);
		Assert.AreEqual(0, timed.ResetCalls);
	}

	[TestMethod]
	public void CanRetryObjective_RejectsNonRetriableTimedOutObjective()
	{
		var timed = new TestObjective { ProgressValue = 7 };
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval, retriable: false);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		quest.SetActive();
		quest.Objectives.UpdateNode();

		Assert.IsFalse(timed.IsRetriable);
		Assert.IsTrue(timed.IsTimedOut);
		Assert.IsFalse(quest.CanRetryObjective(timed.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(timed.ObjectiveID));
		Assert.AreEqual(7, timed.ProgressValue);
		Assert.IsTrue(timed.IsTimedOut);
		Assert.AreEqual(0, timed.ResetCalls);
	}

	[TestMethod]
	public void CanRetryObjective_RejectsUntimedUnexpiredAndCompletedObjectives()
	{
		var untimed = new TestObjective { ProgressValue = 1 };
		var unexpired = new TestObjective { ProgressValue = 2 };
		unexpired.WithTimeLimit(WorldQuestManager.UpdateInterval * 3);
		var completed = new TestObjective { ProgressValue = 10 };
		completed.WithTimeLimit(1);
		completed.Timer.Update(1);
		completed.Complete();
		var quest = new TestQuest();
		quest.Objectives.AddParallel(untimed, unexpired, completed);
		quest.SetActive();

		Assert.IsFalse(quest.CanRetryObjective(untimed.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(untimed.ObjectiveID));
		Assert.IsFalse(quest.CanRetryObjective(unexpired.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(unexpired.ObjectiveID));
		Assert.IsFalse(quest.CanRetryObjective(completed.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(completed.ObjectiveID));
		Assert.AreEqual(1, untimed.ProgressValue);
		Assert.AreEqual(2, unexpired.ProgressValue);
		Assert.AreEqual(10, completed.ProgressValue);
		Assert.AreEqual(0, untimed.ResetCalls);
		Assert.AreEqual(0, unexpired.ResetCalls);
		Assert.AreEqual(0, completed.ResetCalls);
	}

	[TestMethod]
	public void CanRetryObjective_RejectsTimedOutObjectiveInFutureNode()
	{
		var current = new TestObjective();
		var future = new TestObjective { ProgressValue = 6 };
		future.WithTimeLimit(10);
		future.Timer.Update(10);
		var quest = new TestQuest();
		quest.Objectives.Add(current).Add(future);
		quest.SetActive();

		Assert.IsFalse(quest.CanRetryObjective(future.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(future.ObjectiveID));
		Assert.AreEqual(6, future.ProgressValue);
		Assert.IsTrue(future.IsTimedOut);
		Assert.AreEqual(0, future.ResetCalls);
		Assert.AreSame(current, quest.ActiveObjectives.Single());
	}

	[TestMethod]
	public void CanRetryObjective_RejectsTimedOutObjectiveInSkippedBranch()
	{
		var skipped = new TestObjective { ProgressValue = 6 };
		skipped.WithTimeLimit(10);
		skipped.Timer.Update(10);
		var selectedHead = new TestObjective { Ready = true };
		var selectedCurrent = new TestObjective();
		var quest = new TestQuest();
		quest.Objectives.AddBranch([skipped], [selectedHead, selectedCurrent]);
		quest.SetActive();
		((WorldBranchNode)quest.Objectives.Current).Complete();

		Assert.IsFalse(quest.CanRetryObjective(skipped.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(skipped.ObjectiveID));
		Assert.AreEqual(6, skipped.ProgressValue);
		Assert.IsTrue(skipped.IsTimedOut);
		Assert.AreEqual(0, skipped.ResetCalls);
		Assert.AreSame(selectedCurrent, quest.Objectives.Current.FindAllEntrances().Single());
	}

	[TestMethod]
	[DataRow("Leaf")]
	[DataRow("Parallel")]
	[DataRow("Optional")]
	[DataRow("Branch")]
	public void TryRetryObjectiveCore_AcceptsTimedOutEntranceForEveryCurrentNodeType(string nodeType)
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval);
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
			default:
				Assert.Fail($"Unknown node type {nodeType}.");
				break;
		}
		quest.SetActive();
		quest.Objectives.UpdateNode();

		Assert.IsTrue(quest.CanRetryObjective(timed.ObjectiveID));
		Assert.IsTrue(quest.TryRetryObjectiveCore(timed.ObjectiveID));
		Assert.IsFalse(timed.IsTimedOut);
		CollectionAssert.Contains(quest.ActiveObjectives.ToArray(), timed);
	}

	[TestMethod]
	public void TryRetryObjectiveCore_RejectsRepeatedRequestWithoutResettingAgain()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(WorldQuestManager.UpdateInterval);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		quest.SetActive();
		quest.Objectives.UpdateNode();

		Assert.IsTrue(quest.TryRetryObjectiveCore(timed.ObjectiveID));
		Assert.IsFalse(quest.TryRetryObjectiveCore(timed.ObjectiveID));
		Assert.AreEqual(1, timed.ResetCalls);
		Assert.AreEqual(2, timed.Activations);
	}
}
