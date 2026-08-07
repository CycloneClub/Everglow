using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.MissionStructure;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.MissionSystem;

[TestClass]
public class PlayerObjectiveStructureTest
{
	private sealed class TestMission : PlayerMissionBase
	{
		public override string DisplayName => nameof(TestMission);
	}

	private sealed class TestObjective : PlayerObjectiveBase
	{
		public bool Ready { get; set; }

		public int Activations { get; private set; }

		public int Deactivations { get; private set; }

		public override float Progress => Ready ? 1f : 0f;

		public override bool CheckCompletion() => Ready;

		public override void Activate(PlayerMissionBase sourceMission) => Activations++;

		public override void Deactivate() => Deactivations++;

		public override void GetObjectivesIcon(MissionIconGroup iconGroup)
		{
		}

		public override void GetObjectivesText(List<string> lines) => lines.Add("Test\n");

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
		var mission = new TestMission();
		mission.Objectives.Add(first).Add(second);

		mission.Objectives.Activate(mission);
		Assert.AreSame(first, mission.CurrentObjective);
		Assert.AreEqual(1, first.Activations);

		Assert.IsTrue(mission.Objectives.Update(mission));
		Assert.IsTrue(first.Completed);
		Assert.AreEqual(1, first.Deactivations);
		Assert.AreSame(second, mission.CurrentObjective);
		Assert.AreEqual(1, second.Activations);
	}

	[TestMethod]
	public void ParallelNode_CompletesLeavesIndividuallyAndKeepsRemainingLeafActive()
	{
		var complete = new TestObjective { Ready = true };
		var pending = new TestObjective();
		var mission = new TestMission();
		mission.Objectives.AddParallel(complete, pending);

		mission.Objectives.Activate(mission);
		CollectionAssert.AreEquivalent(new[] { complete, pending }, mission.Objectives.ActiveObjectives.ToArray());

		mission.Objectives.Update(mission);
		Assert.IsTrue(complete.Completed);
		Assert.IsFalse(pending.Completed);
		CollectionAssert.AreEqual(new[] { pending }, mission.Objectives.ActiveObjectives.ToArray());
		Assert.AreEqual(1, complete.Deactivations);
		Assert.AreEqual(2, pending.Activations);
	}

	[TestMethod]
	public void OptionalNode_CompletesWhenAnyLeafCompletes()
	{
		var selected = new TestObjective { Ready = true };
		var skipped = new TestObjective();
		var next = new TestObjective();
		var mission = new TestMission();
		mission.Objectives.AddOptional(selected, skipped).Add(next);

		mission.Objectives.Activate(mission);
		mission.Objectives.Update(mission);

		Assert.IsTrue(selected.Completed);
		Assert.IsFalse(skipped.Completed);
		Assert.AreSame(next, mission.CurrentObjective);
	}

	[TestMethod]
	public void BranchNode_SelectsCompletedHeadAndAdvancesAlongSelectedBranch()
	{
		var firstBranch = new TestObjective();
		var secondBranch = new TestObjective { Ready = true };
		var branchContinuation = new TestObjective();
		var next = new TestObjective();
		var mission = new TestMission();
		mission.Objectives.AddBranch([firstBranch], [secondBranch, branchContinuation]).Add(next);

		mission.Objectives.Activate(mission);
		mission.Objectives.Update(mission);

		Assert.IsTrue(secondBranch.Completed);
		Assert.IsFalse(firstBranch.Completed);
		Assert.AreSame(branchContinuation, mission.CurrentObjective);

		branchContinuation.Ready = true;
		mission.Objectives.Update(mission);
		Assert.AreSame(next, mission.CurrentObjective);
	}

	[TestMethod]
	public void Reset_ResetsLeavesAndDeactivatesActiveLeaves()
	{
		var first = new TestObjective { Ready = true };
		var second = new TestObjective();
		var mission = new TestMission();
		mission.Objectives.Add(first).Add(second);
		mission.Objectives.Activate(mission);
		mission.Objectives.Update(mission);

		mission.Reset();

		Assert.IsFalse(first.Completed);
		Assert.AreEqual(1, second.Deactivations);
		Assert.AreSame(first, mission.Objectives.FindCurrentObjectives().Single());
		Assert.IsEmpty(mission.Objectives.ActiveObjectives);
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
	public void SaveData_LoadData_RoundTripsStructuralProgress()
	{
		var savedFirst = new TestObjective { Ready = true };
		var savedSecond = new TestObjective();
		var saved = new PlayerStructuralObjectiveContainer().Add(new TestObjective { Ready = true }).AddParallel(savedFirst, savedSecond);
		var mission = new TestMission();
		saved.Activate(mission);
		saved.Update(mission);
		saved.Update(mission);
		var tag = new TagCompound();
		saved.SaveData(tag);

		var loadedFirst = new TestObjective();
		var loadedSecond = new TestObjective();
		var loaded = new PlayerStructuralObjectiveContainer().Add(new TestObjective()).AddParallel(loadedFirst, loadedSecond);
		loaded.LoadData(tag);

		Assert.IsTrue(loadedFirst.Completed);
		Assert.IsFalse(loadedSecond.Completed);
		Assert.AreSame(loadedSecond, loaded.FindCurrentObjectives().Single());
	}

	[TestMethod]
	public void LoadData_MigratesLegacyFlatLeafSave()
	{
		var legacyTag = new TagCompound();
		legacyTag.Add(nameof(PlayerMissionBase.Objectives), new List<TagCompound>
		{
			new() { { nameof(TestObjective.Ready), 1 } },
			new() { { nameof(TestObjective.Ready), 0 } },
		});
		var first = new TestObjective();
		var second = new TestObjective();
		var objectives = new PlayerStructuralObjectiveContainer().Add(first).Add(second);

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
		var saved = new PlayerStructuralObjectiveContainer()
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
		var loaded = new PlayerStructuralObjectiveContainer()
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
	public void MissionLoad_InvalidBranchCursor_ReopensCompletedMission()
	{
		var saved = new PlayerStructuralObjectiveContainer()
			.Add(new TestObjective { Ready = true })
			.AddBranch([new TestObjective { Ready = true }], [new TestObjective { Ready = true }]);
		var tag = new TagCompound
		{
			[nameof(PlayerMissionBase.State)] = (int)PlayerMissionState.Completed,
			[PlayerMissionBase.TimeSaveKey] = 120L,
		};
		saved.SaveData(tag);
		var nodeTags = tag.GetList<TagCompound>("StructuralObjectives");
		nodeTags[1]["Selected"] = -1;
		nodeTags[1]["Index"] = 2;

		var mission = new TestMission();
		mission.Objectives
			.Add(new TestObjective())
			.AddBranch([new TestObjective()], [new TestObjective()]);

		mission.LoadData(tag);

		Assert.IsTrue(mission.Objectives.RecoveredInvalidState);
		Assert.AreEqual(PlayerMissionState.Accepted, mission.State);
		Assert.AreEqual(0, mission.Time);
		Assert.IsFalse(mission.OldCheckComplete);
	}
}
