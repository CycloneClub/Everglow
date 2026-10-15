using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class WorldObjectiveStructureRecoveryTest
{
	private sealed class TestQuest : WorldQuestBase
	{
		public TestQuest()
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
			State = WorldQuestState.Completed;
			TryRecordRewardClaim("Player");
			State = WorldQuestState.Active;
			Activate();
		}
	}

	private sealed class TestObjective : WorldObjectiveBase
	{
		public int Value { get; set; }

		public override float Progress => Value;

		public override bool CheckCompletion() => false;

		public override string GetObjectiveText() => string.Empty;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
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
	public void LoadData_InvalidBranchCursor_ReopensCompletedQuestAndRewards()
	{
		var quest = new TestQuest();
		quest.First.Value = 5;
		quest.BranchA.Value = 6;
		quest.BranchB.Value = 7;
		quest.First.Complete();
		quest.BranchA.Complete();
		quest.BranchB.Complete();
		quest.SeedActiveClaimedState();
		var objectivesTag = new TagCompound();
		quest.Objectives.SaveData(objectivesTag);
		var nodeTags = objectivesTag.GetList<TagCompound>("Objectives");
		nodeTags[1]["Selected"] = 99;
		nodeTags[1]["Index"] = 1;
		var tag = new TagCompound
		{
			[nameof(WorldQuestBase.State)] = (int)WorldQuestState.Completed,
			[nameof(WorldQuestBase.Time)] = 120,
			[nameof(WorldQuestBase.Objectives)] = objectivesTag,
		};

		quest.LoadData(tag);

		Assert.IsTrue(quest.Objectives.RecoveredInvalidState);
		Assert.AreEqual(WorldQuestState.Active, quest.State);
		Assert.AreEqual(0, quest.Time);
		Assert.IsEmpty(quest.RewardClaimedPlayers);
		Assert.IsTrue(quest.Objectives.AllObjectives.All(objective => !objective.Completed));
		Assert.IsTrue(quest.Objectives.AllObjectives.All(objective => !objective.RewardClaimed));
		Assert.IsTrue(quest.Objectives.AllObjectives.Cast<TestObjective>().All(objective => objective.Value == 0));
		Assert.AreSame(quest.First, quest.ActiveObjectives.Single());
	}

	[TestMethod]
	public void NetReceive_InvalidBranchCursor_ConsumesSnapshotAndResetsWholeQuest()
	{
		var quest = new TestQuest();
		quest.SeedActiveClaimedState();
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write((int)WorldQuestState.Completed);
		writer.Write(120);
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

		quest.NetReceive(reader);

		Assert.IsTrue(quest.Objectives.RecoveredInvalidState);
		Assert.AreEqual(12345, reader.ReadInt32());
		Assert.AreEqual(WorldQuestState.Active, quest.State);
		Assert.AreEqual(0, quest.Time);
		Assert.IsEmpty(quest.RewardClaimedPlayers);
		Assert.IsTrue(quest.Objectives.AllObjectives.All(objective => !objective.Completed));
		Assert.IsTrue(quest.Objectives.AllObjectives.All(objective => !objective.RewardClaimed));
		Assert.IsTrue(quest.Objectives.AllObjectives.Cast<TestObjective>().All(objective => objective.Value == 0));
		Assert.AreSame(quest.First, quest.ActiveObjectives.Single());
	}

	private static void WriteObjective(BinaryWriter writer, int value)
	{
		writer.Write(true);
		writer.Write(true);
		writer.Write(value);
	}
}
