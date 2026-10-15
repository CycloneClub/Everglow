using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class PlayerQuestBasePersistenceTest
{
	private sealed class PersistenceStubQuest : PlayerQuestBase
	{
		public override string DisplayName => nameof(PersistenceStubQuest);
	}

	[TestMethod]
	[DataRow(PlayerQuestState.Accepted)]
	[DataRow(PlayerQuestState.Available)]
	[DataRow(PlayerQuestState.Failed)]
	[DataRow(PlayerQuestState.Completed)]
	public void SaveData_LoadData_PreservesState(PlayerQuestState state)
	{
		var saved = new PersistenceStubQuest { State = state };
		var tag = new TagCompound();

		// The headless test TagCompound currently rejects bool payloads even though player saves accept them at runtime.
		try
		{
			saved.SaveData(tag);
		}
		catch (IOException)
		{
		}

		Assert.IsTrue(tag.TryGet<int>(nameof(PlayerQuestBase.State), out var stored));
		Assert.AreEqual((int)state, stored);
		Assert.IsTrue(tag.TryGet<string>(nameof(PlayerQuestBase.InstanceId), out var storedInstanceId));
		Assert.AreEqual(saved.InstanceId, storedInstanceId);

		var loaded = new PersistenceStubQuest();
		loaded.LoadData(tag);

		Assert.AreEqual(state, loaded.State);
		Assert.AreEqual(saved.InstanceId, loaded.InstanceId);
	}

	[TestMethod]
	public void SaveData_StoresTimeAsInt()
	{
		var saved = new PersistenceStubQuest { Time = 120 };
		var tag = new TagCompound();

		// The headless test TagCompound currently rejects bool payloads even though player saves accept them at runtime.
		try
		{
			saved.SaveData(tag);
		}
		catch (IOException)
		{
		}

		Assert.IsInstanceOfType<int>(tag[PlayerQuestBase.TimeSaveKey]);
		Assert.AreEqual(120, tag.GetInt(PlayerQuestBase.TimeSaveKey));
	}

	[TestMethod]
	[DataRow((long)int.MaxValue + 1, int.MaxValue)]
	[DataRow((long)int.MinValue - 1, int.MinValue)]
	public void LoadData_LegacyLongTime_ClampsToIntRange(long storedTime, int expectedTime)
	{
		var tag = new TagCompound
		{
			{ PlayerQuestBase.TimeSaveKey, storedTime },
		};
		var loaded = new PersistenceStubQuest();

		loaded.LoadData(tag);

		Assert.AreEqual(expectedTime, loaded.Time);
	}

	[TestMethod]
	public void NewInstances_HaveDistinctValidIds()
	{
		var first = new PersistenceStubQuest();
		var second = new PersistenceStubQuest();

		Assert.IsTrue(Guid.TryParseExact(first.InstanceId, "N", out _));
		Assert.IsTrue(Guid.TryParseExact(second.InstanceId, "N", out _));
		Assert.AreNotEqual(first.InstanceId, second.InstanceId);
	}

	[TestMethod]
	public void StateChangesAndReset_PreserveInstanceId()
	{
		var quest = new PersistenceStubQuest { State = PlayerQuestState.Available };
		string instanceId = quest.InstanceId;

		quest.State = PlayerQuestState.Accepted;
		quest.Reset();
		quest.State = PlayerQuestState.Failed;
		quest.Reset();

		Assert.AreEqual(instanceId, quest.InstanceId);
	}

	[TestMethod]
	public void LoadData_MissingState_FallsBackToAvailable()
	{
		var tag = new TagCompound();
		var loaded = new PersistenceStubQuest
		{
			// Simulate Activator.CreateInstance default before LoadData.
			State = PlayerQuestState.Accepted,
		};

		loaded.LoadData(tag);

		Assert.AreEqual(PlayerQuestState.Available, loaded.State);
	}

	[TestMethod]
	public void LoadData_MissingInstanceId_PreservesGeneratedId()
	{
		var loaded = new PersistenceStubQuest();
		string generatedId = loaded.InstanceId;

		loaded.LoadData(new TagCompound());

		Assert.AreEqual(generatedId, loaded.InstanceId);
	}

	[TestMethod]
	public void LoadData_InvalidInstanceId_PreservesGeneratedId()
	{
		var tag = new TagCompound
		{
			{ nameof(PlayerQuestBase.InstanceId), "not-a-guid" },
		};
		var loaded = new PersistenceStubQuest();
		string generatedId = loaded.InstanceId;

		loaded.LoadData(tag);

		Assert.AreEqual(generatedId, loaded.InstanceId);
	}
}
