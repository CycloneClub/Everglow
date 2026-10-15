using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Structure;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class PlayerObjectiveStructureTest
{
	private sealed class TestQuest : PlayerQuestBase
	{
		public override string DisplayName => nameof(TestQuest);
	}

	private sealed class TestObjective : PlayerObjectiveBase
	{
		public bool Ready { get; set; }

		public bool CompleteDuringUpdate { get; set; }

		public int Activations { get; private set; }

		public int Deactivations { get; private set; }

		public int UpdateCalls { get; private set; }

		public override float Progress => Ready ? 1f : 0f;

		public override bool CheckCompletion() => Ready;

		public override void Update()
		{
			UpdateCalls++;
			Ready |= CompleteDuringUpdate;
		}

		public override void Activate(PlayerQuestBase sourceQuest) => Activations++;

		public override void Deactivate() => Deactivations++;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}

		public override string GetObjectiveText() => "Test";

		public override void SaveData(TagCompound tag)
		{
			tag.Add(nameof(Ready), Ready ? 1 : 0);
		}

		public override void LoadData(TagCompound tag)
		{
			Ready = tag.TryGet<int>(nameof(Ready), out var ready) && ready != 0;
		}

	}

	[TestMethod]
	public void LinearNode_AdvancesAndActivatesNextLeaf()
	{
		var first = new TestObjective { Ready = true };
		var second = new TestObjective();
		var quest = new TestQuest();
		quest.Objectives.Add(first).Add(second);

		quest.Objectives.Activate(quest);
		Assert.AreSame(first, quest.CurrentObjective);
		Assert.AreEqual(1, first.Activations);

		Assert.IsTrue(quest.Objectives.Update(quest));
		Assert.IsTrue(first.Completed);
		Assert.AreEqual(1, first.Deactivations);
		Assert.AreSame(second, quest.CurrentObjective);
		Assert.AreEqual(1, second.Activations);
	}

	[TestMethod]
	public void ParallelNode_CompletesLeavesIndividuallyAndKeepsRemainingLeafActive()
	{
		var complete = new TestObjective { Ready = true };
		var pending = new TestObjective();
		var quest = new TestQuest();
		quest.Objectives.AddParallel(complete, pending);

		quest.Objectives.Activate(quest);
		CollectionAssert.AreEquivalent(new[] { complete, pending }, quest.Objectives.ActiveObjectives.ToArray());

		quest.Objectives.Update(quest);
		Assert.IsTrue(complete.Completed);
		Assert.IsFalse(pending.Completed);
		CollectionAssert.AreEqual(new[] { pending }, quest.Objectives.ActiveObjectives.ToArray());
		Assert.AreEqual(1, complete.Deactivations);
		Assert.AreEqual(2, pending.Activations);
	}

	[TestMethod]
	public void OptionalNode_CompletesWhenAnyLeafCompletes()
	{
		var selected = new TestObjective { Ready = true };
		var skipped = new TestObjective();
		var next = new TestObjective();
		var quest = new TestQuest();
		quest.Objectives.AddOptional(selected, skipped).Add(next);

		quest.Objectives.Activate(quest);
		quest.Objectives.Update(quest);

		Assert.IsTrue(selected.Completed);
		Assert.IsFalse(skipped.Completed);
		Assert.AreSame(next, quest.CurrentObjective);
	}

	[TestMethod]
	public void BranchNode_SelectsCompletedHeadAndAdvancesAlongSelectedBranch()
	{
		var firstBranch = new TestObjective();
		var secondBranch = new TestObjective { Ready = true };
		var branchContinuation = new TestObjective();
		var next = new TestObjective();
		var quest = new TestQuest();
		quest.Objectives.AddBranch([firstBranch], [secondBranch, branchContinuation]).Add(next);

		quest.Objectives.Activate(quest);
		quest.Objectives.Update(quest);

		Assert.IsTrue(secondBranch.Completed);
		Assert.IsFalse(firstBranch.Completed);
		Assert.AreSame(branchContinuation, quest.CurrentObjective);

		branchContinuation.Ready = true;
		quest.Objectives.Update(quest);
		Assert.AreSame(next, quest.CurrentObjective);
	}

	[TestMethod]
	public void Reset_ResetsLeavesAndDeactivatesActiveLeaves()
	{
		var first = new TestObjective { Ready = true };
		var second = new TestObjective();
		var quest = new TestQuest();
		quest.Objectives.Add(first).Add(second);
		quest.Objectives.Activate(quest);
		quest.Objectives.Update(quest);

		quest.Reset();

		Assert.IsFalse(first.Completed);
		Assert.AreEqual(1, second.Deactivations);
		Assert.AreSame(first, quest.Objectives.FindCurrentObjectives().Single());
		Assert.IsEmpty(quest.Objectives.ActiveObjectives);
	}

	[TestMethod]
	public void ResetProgress_PreservesRewardClaimState()
	{
		var objective = new TestObjective { Ready = true };
		objective.Complete();

		objective.ResetProgress();

		Assert.IsFalse(objective.Completed);
		Assert.IsTrue(objective.HasGivenRewardItems);
		Assert.IsTrue(objective.Ready);
	}

	[TestMethod]
	public void TimedObjective_AdvancesOnlyWhileActiveAndExpiresInPlace()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(PlayerQuestManager.UpdateInterval * 2);
		var pending = new TestObjective();
		pending.WithTimeLimit(PlayerQuestManager.UpdateInterval);
		var quest = new TestQuest();
		quest.Objectives.Add(timed).Add(pending);
		quest.Objectives.Activate(quest);

		quest.Objectives.Update(quest);
		Assert.AreEqual(PlayerQuestManager.UpdateInterval, timed.Timer.ElapsedTime);
		Assert.AreEqual(0, pending.Timer.ElapsedTime);

		quest.Objectives.Update(quest);
		Assert.IsTrue(timed.IsTimedOut);
		Assert.IsFalse(timed.Completed);
		Assert.AreSame(timed, quest.Objectives.FindCurrentObjectives().Single());
		Assert.IsEmpty(quest.Objectives.ActiveObjectives);
		Assert.AreEqual(1, timed.Deactivations);

		quest.Objectives.Update(quest);
		Assert.AreEqual(2, timed.UpdateCalls);
		Assert.AreEqual(PlayerQuestManager.UpdateInterval * 2, timed.Timer.ElapsedTime);
		Assert.AreEqual(1, timed.Deactivations);
		Assert.AreEqual(0, pending.Timer.ElapsedTime);
	}

	[TestMethod]
	public void TimedObjective_CompletionWinsAtExpiryBoundary()
	{
		var timed = new TestObjective { CompleteDuringUpdate = true };
		timed.WithTimeLimit(PlayerQuestManager.UpdateInterval);
		var next = new TestObjective();
		var quest = new TestQuest();
		quest.Objectives.Add(timed).Add(next);
		quest.Objectives.Activate(quest);

		quest.Objectives.Update(quest);

		Assert.IsTrue(timed.Completed);
		Assert.IsFalse(timed.IsTimedOut);
		Assert.AreEqual(0, timed.Timer.ElapsedTime);
		Assert.AreSame(next, quest.CurrentObjective);
	}

	[TestMethod]
	public void TimedObjective_ResetAllowsReactivation()
	{
		var timed = new TestObjective();
		timed.WithTimeLimit(PlayerQuestManager.UpdateInterval);
		var quest = new TestQuest();
		quest.Objectives.Add(timed);
		quest.Objectives.Activate(quest);
		quest.Objectives.Update(quest);

		quest.Reset();
		quest.Activate();

		Assert.IsFalse(timed.IsTimedOut);
		Assert.AreEqual(0, timed.Timer.ElapsedTime);
		Assert.AreEqual(2, timed.Activations);
		CollectionAssert.AreEqual(new[] { timed }, quest.Objectives.ActiveObjectives.ToArray());
	}

	[TestMethod]
	public void NewlyActivatedTimedObjective_DoesNotConsumePreviousInterval()
	{
		var first = new TestObjective { Ready = true };
		var second = new TestObjective();
		second.WithTimeLimit(PlayerQuestManager.UpdateInterval);
		var quest = new TestQuest();
		quest.Objectives.Add(first).Add(second);
		quest.Objectives.Activate(quest);

		quest.Objectives.Update(quest);

		Assert.AreSame(second, quest.CurrentObjective);
		Assert.AreEqual(0, second.Timer.ElapsedTime);
		Assert.IsFalse(second.IsTimedOut);
	}

	[TestMethod]
	public void SaveData_LoadData_RoundTripsStructuralProgress()
	{
		var savedFirst = new TestObjective { Ready = true };
		var savedSecond = new TestObjective();
		var saved = new PlayerObjectiveContainer().Add(new TestObjective { Ready = true }).AddParallel(savedFirst, savedSecond);
		var quest = new TestQuest();
		saved.Activate(quest);
		saved.Update(quest);
		saved.Update(quest);
		var tag = new TagCompound();
		saved.SaveData(tag);

		var loadedFirst = new TestObjective();
		var loadedSecond = new TestObjective();
		var loaded = new PlayerObjectiveContainer().Add(new TestObjective()).AddParallel(loadedFirst, loadedSecond);
		loaded.LoadData(tag);

		Assert.IsTrue(loadedFirst.Completed);
		Assert.IsFalse(loadedSecond.Completed);
		Assert.AreSame(loadedSecond, loaded.FindCurrentObjectives().Single());
	}

	[TestMethod]
	public void LoadData_MigratesLegacyFlatLeafSave()
	{
		var legacyTag = new TagCompound();
		legacyTag.Add(nameof(PlayerQuestBase.Objectives), new List<TagCompound>
		{
			new() { { nameof(TestObjective.Ready), 1 } },
			new() { { nameof(TestObjective.Ready), 0 } },
		});
		var first = new TestObjective();
		var second = new TestObjective();
		var objectives = new PlayerObjectiveContainer().Add(first).Add(second);

		objectives.LoadData(legacyTag);

		Assert.IsTrue(first.Ready);
		Assert.IsFalse(second.Ready);
		Assert.AreSame(first, objectives.FindCurrentObjectives().Single());
	}

	[TestMethod]
	public void LoadData_InvalidBranchCursor_ResetsWholeObjectiveStructure()
	{
		var savedFirst = new TestObjective { Ready = true };
		var savedBranchA = new TestObjective { Ready = true };
		var savedBranchB = new TestObjective { Ready = true };
		var saved = new PlayerObjectiveContainer()
			.Add(savedFirst)
			.AddBranch([savedBranchA], [savedBranchB]);
		savedFirst.Complete();
		savedBranchA.Complete();
		var tag = new TagCompound();
		saved.SaveData(tag);
		var nodeTags = tag.GetList<TagCompound>("StructuralObjectives");
		nodeTags[1]["Selected"] = 99;
		nodeTags[1]["Index"] = 1;

		var loadedFirst = new TestObjective();
		var loadedBranchA = new TestObjective();
		var loadedBranchB = new TestObjective();
		var loaded = new PlayerObjectiveContainer()
			.Add(loadedFirst)
			.AddBranch([loadedBranchA], [loadedBranchB]);
		loaded.LoadData(tag);

		Assert.IsTrue(loaded.RecoveredInvalidState);
		Assert.IsTrue(loaded.AllObjectives.All(objective => !objective.Completed));
		Assert.IsTrue(loaded.AllObjectives.All(objective => !objective.HasGivenRewardItems));
		Assert.IsTrue(loaded.AllObjectives.Cast<TestObjective>().All(objective => !objective.Ready));
		Assert.AreSame(loadedFirst, loaded.FindCurrentObjectives().Single());
	}

	[TestMethod]
	public void QuestLoad_InvalidBranchCursor_ReopensCompletedQuest()
	{
		var saved = new PlayerObjectiveContainer()
			.Add(new TestObjective { Ready = true })
			.AddBranch([new TestObjective { Ready = true }], [new TestObjective { Ready = true }]);
		var tag = new TagCompound
		{
			[nameof(PlayerQuestBase.State)] = (int)PlayerQuestState.Completed,
			[PlayerQuestBase.TimeSaveKey] = 120L,
		};
		saved.SaveData(tag);
		var nodeTags = tag.GetList<TagCompound>("StructuralObjectives");
		nodeTags[1]["Selected"] = -1;
		nodeTags[1]["Index"] = 2;

		var quest = new TestQuest();
		quest.Objectives
			.Add(new TestObjective())
			.AddBranch([new TestObjective()], [new TestObjective()]);

		quest.LoadData(tag);

		Assert.IsTrue(quest.Objectives.RecoveredInvalidState);
		Assert.AreEqual(PlayerQuestState.Accepted, quest.State);
		Assert.AreEqual(0, quest.Time);
		Assert.IsFalse(quest.OldCheckComplete);
	}
}
