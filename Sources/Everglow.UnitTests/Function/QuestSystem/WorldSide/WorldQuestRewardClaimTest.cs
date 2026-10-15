using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class WorldQuestRewardClaimTest
{
	private class TestQuest : WorldQuestBase
	{
		public override string Name => nameof(TestQuest);

		public void SetState(WorldQuestState state) => State = state;
	}

	private sealed class ResettableTestQuest : TestQuest
	{
		public ResettableTestQuest()
		{
			Objectives.Add(new TestObjective());
		}
	}

	private sealed class TestObjective : WorldObjectiveBase
	{
		public override bool CheckCompletion() => false;

		public override string GetObjectiveText() => string.Empty;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}
	}

	[TestMethod]
	public void CompletedQuest_AllowsEachOrdinalPlayerNameOnce()
	{
		var quest = new TestQuest();
		quest.SetState(WorldQuestState.Completed);

		Assert.IsTrue(quest.TryRecordRewardClaim("Alice"));
		Assert.IsFalse(quest.TryRecordRewardClaim("Alice"));
		Assert.IsTrue(quest.TryRecordRewardClaim("alice"));
		CollectionAssert.AreEquivalent(
			new[] { "Alice", "alice" },
			quest.RewardClaimedPlayers.ToArray());
	}

	[TestMethod]
	[DataRow(WorldQuestState.Locked)]
	[DataRow(WorldQuestState.Active)]
	[DataRow(WorldQuestState.Failed)]
	public void NonCompletedQuest_RejectsRewardClaim(WorldQuestState state)
	{
		var quest = new TestQuest();
		quest.SetState(state);

		Assert.IsFalse(quest.CanClaimReward("Alice"));
		Assert.IsFalse(quest.TryRecordRewardClaim("Alice"));
		Assert.IsEmpty(quest.RewardClaimedPlayers);
	}

	[TestMethod]
	public void Reset_ClearsAllClaimedPlayerNames()
	{
		var quest = new ResettableTestQuest();
		quest.SetState(WorldQuestState.Completed);
		Assert.IsTrue(quest.TryRecordRewardClaim("Alice"));

		quest.Reset();

		Assert.IsEmpty(quest.RewardClaimedPlayers);
	}

	[TestMethod]
	public void LoadData_LegacyTrueBoolWithoutNames_ReopensClaims()
	{
		var quest = new TestQuest();
		var tag = new TagCompound
		{
			[nameof(WorldQuestBase.State)] = (int)WorldQuestState.Completed,
		};
		var values = (Dictionary<string, object>)typeof(TagCompound)
			.GetField("dict", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.GetValue(tag)!;
		values["RewardClaimed"] = true;

		quest.LoadData(tag);

		Assert.IsTrue(quest.CanClaimReward("Alice"));
		Assert.IsEmpty(quest.RewardClaimedPlayers);
	}

	[TestMethod]
	public void LoadData_ExistingNameListRemainsAuthoritative()
	{
		var quest = new TestQuest();
		var tag = new TagCompound
		{
			[nameof(WorldQuestBase.State)] = (int)WorldQuestState.Completed,
			[nameof(WorldQuestBase.RewardClaimedPlayers)] = new List<string> { "Alice" },
		};

		quest.LoadData(tag);

		Assert.IsFalse(quest.CanClaimReward("Alice"));
		Assert.IsTrue(quest.CanClaimReward("Bob"));
		CollectionAssert.AreEqual(new[] { "Alice" }, quest.RewardClaimedPlayers.ToArray());
	}

	[TestMethod]
	public void SaveData_OmitsLegacyBoolAndPreservesAllNames()
	{
		var quest = new TestQuest();
		quest.SetState(WorldQuestState.Completed);
		Assert.IsTrue(quest.TryRecordRewardClaim("bob"));
		Assert.IsTrue(quest.TryRecordRewardClaim("Alice"));
		var tag = new TagCompound();

		quest.SaveData(tag);

		Assert.IsFalse(tag.ContainsKey("RewardClaimed"));
		CollectionAssert.AreEquivalent(
			new[] { "Alice", "bob" },
			tag.GetList<string>(nameof(WorldQuestBase.RewardClaimedPlayers)).ToArray());
	}

	[TestMethod]
	public void NetSend_OmitsLegacyBoolAndPreservesAllNames()
	{
		var quest = new TestQuest();
		quest.SetState(WorldQuestState.Completed);
		Assert.IsTrue(quest.TryRecordRewardClaim("bob"));
		Assert.IsTrue(quest.TryRecordRewardClaim("Alice"));
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);

		quest.NetSend(writer);
		writer.Flush();
		stream.Position = 0;
		using var reader = new BinaryReader(stream);

		Assert.AreEqual((int)WorldQuestState.Completed, reader.ReadInt32());
		Assert.AreEqual(0, reader.ReadInt32());
		Assert.AreEqual(2, reader.ReadInt32());
		CollectionAssert.AreEquivalent(
			new[] { "Alice", "bob" },
			new[] { reader.ReadString(), reader.ReadString() });
	}

	[TestMethod]
	public void NetReceive_ReplacesNamesInsteadOfMergingSnapshots()
	{
		var source = new TestQuest();
		source.SetState(WorldQuestState.Completed);
		Assert.IsTrue(source.TryRecordRewardClaim("ServerPlayer"));
		var target = new TestQuest();
		target.SetState(WorldQuestState.Completed);
		Assert.IsTrue(target.TryRecordRewardClaim("StaleClientPlayer"));
		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			source.NetSend(writer);
		}
		stream.Position = 0;

		using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			target.NetReceive(reader);
		}

		CollectionAssert.AreEqual(
			new[] { "ServerPlayer" },
			target.RewardClaimedPlayers.ToArray());
	}
}
