using Everglow.Commons.Mechanics.Mission.WorldSide;
using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.MissionSystem;

[TestClass]
public class WorldObjectiveStructureRecoveryTest
{
	private sealed class TestMission : WorldMissionBase
	{
		public TestMission()
		{
			First = new TestObjective();
			BranchA = new TestObjective();
			BranchB = new TestObjective();
			Objectives.Add(First).AddBranch([BranchA], [BranchB]);
		}

		public TestObjective First { get; }

		public TestObjective BranchA { get; }

		public TestObjective BranchB { get; }

		public void SeedActiveClaimedState()
		{
			State = WorldMissionState.Active;
			RewardClaimed = true;
			RewardClaimedPlayers.Add("Player");
			Activate();
		}
	}

	private sealed class TestObjective : WorldObjectiveBase
	{
		public int Value { get; set; }

		public override float Progress => Value;

		public override bool CheckCompletion() => false;

		public override void GetObjectivesText()
		{
		}

		public override void ResetProgress()
		{
			base.ResetProgress();
			Value = 0;
		}

		public override void LoadData(TagCompound tag)
		{
			Value = tag.GetInt(nameof(Value));
		}

		public override void SaveData(TagCompound tag)
		{
			tag[nameof(Value)] = Value;
		}

		public override void NetSend(BinaryWriter writer)
		{
			base.NetSend(writer);
			writer.Write(Value);
		}

		public override void NetReceive(BinaryReader reader)
		{
			base.NetReceive(reader);
			Value = reader.ReadInt32();
		}
	}

	[TestMethod]
	public void LoadData_InvalidBranchCursor_ReopensCompletedMissionAndRewards()
	{
		var mission = new TestMission();
		mission.First.Value = 5;
		mission.BranchA.Value = 6;
		mission.BranchB.Value = 7;
		mission.First.Complete();
		mission.BranchA.Complete();
		mission.BranchB.Complete();
		mission.SeedActiveClaimedState();
		var objectivesTag = new TagCompound();
		mission.Objectives.SaveData(objectivesTag);
		var nodeTags = objectivesTag.GetList<TagCompound>("Objectives");
		nodeTags[1]["Selected"] = 99;
		nodeTags[1]["Index"] = 1;
		var tag = new TagCompound
		{
			[nameof(WorldMissionBase.State)] = (int)WorldMissionState.Completed,
			[nameof(WorldMissionBase.Time)] = 120,
			[nameof(WorldMissionBase.Objectives)] = objectivesTag,
		};

		mission.LoadData(tag);

		Assert.IsTrue(mission.Objectives.RecoveredInvalidState);
		Assert.AreEqual(WorldMissionState.Active, mission.State);
		Assert.AreEqual(0, mission.Time);
		Assert.IsFalse(mission.RewardClaimed);
		Assert.IsEmpty(mission.RewardClaimedPlayers);
		Assert.IsTrue(mission.Objectives.AllObjectives.All(objective => !objective.Completed));
		Assert.IsTrue(mission.Objectives.AllObjectives.All(objective => !objective.RewardClaimed));
		Assert.IsTrue(mission.Objectives.AllObjectives.Cast<TestObjective>().All(objective => objective.Value == 0));
		Assert.AreSame(mission.First, mission.ActiveObjectives.Single());
	}

	[TestMethod]
	public void NetReceive_InvalidBranchCursor_ConsumesSnapshotAndResetsWholeMission()
	{
		var mission = new TestMission();
		mission.SeedActiveClaimedState();
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((int)WorldMissionState.Completed);
		writer.Write(120);
		writer.Write(true);
		writer.Write(1);
		writer.Write("ServerPlayer");
		WriteObjective(writer, value: 5);
		writer.Write(99);
		writer.Write(1);
		WriteObjective(writer, value: 6);
		WriteObjective(writer, value: 7);
		writer.Write(12345);
		writer.Flush();
		stream.Position = 0;
		using var reader = new BinaryReader(stream);

		mission.NetReceive(reader);

		Assert.IsTrue(mission.Objectives.RecoveredInvalidState);
		Assert.AreEqual(12345, reader.ReadInt32());
		Assert.AreEqual(WorldMissionState.Active, mission.State);
		Assert.AreEqual(0, mission.Time);
		Assert.IsFalse(mission.RewardClaimed);
		Assert.IsEmpty(mission.RewardClaimedPlayers);
		Assert.IsTrue(mission.Objectives.AllObjectives.All(objective => !objective.Completed));
		Assert.IsTrue(mission.Objectives.AllObjectives.All(objective => !objective.RewardClaimed));
		Assert.IsTrue(mission.Objectives.AllObjectives.Cast<TestObjective>().All(objective => objective.Value == 0));
		Assert.AreSame(mission.First, mission.ActiveObjectives.Single());
	}

	private static void WriteObjective(BinaryWriter writer, int value)
	{
		writer.Write(true);
		writer.Write(true);
		writer.Write(value);
	}
}
