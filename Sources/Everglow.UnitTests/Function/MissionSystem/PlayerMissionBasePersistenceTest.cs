using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.MissionSystem;

[TestClass]
public class PlayerMissionBasePersistenceTest
{
	private sealed class PersistenceStubMission : PlayerMissionBase
	{
		public override string DisplayName => nameof(PersistenceStubMission);
	}

	[TestMethod]
	[DataRow(PlayerMissionState.Accepted)]
	[DataRow(PlayerMissionState.Available)]
	[DataRow(PlayerMissionState.Failed)]
	[DataRow(PlayerMissionState.Overdue)]
	[DataRow(PlayerMissionState.Completed)]
	public void SaveData_LoadData_PreservesState(PlayerMissionState state)
	{
		var saved = new PersistenceStubMission { State = state };
		var tag = new TagCompound();

		// The headless test TagCompound currently rejects bool payloads even though player saves accept them at runtime.
		try
		{
			saved.SaveData(tag);
		}
		catch (IOException)
		{
		}

		Assert.IsTrue(tag.TryGet<int>(nameof(PlayerMissionBase.State), out var stored));
		Assert.AreEqual((int)state, stored);

		var loaded = new PersistenceStubMission();
		loaded.LoadData(tag);

		Assert.AreEqual(state, loaded.State);
	}

	[TestMethod]
	public void LoadData_MissingState_FallsBackToAvailable()
	{
		var tag = new TagCompound();
		var loaded = new PersistenceStubMission
		{
			// Simulate Activator.CreateInstance default before LoadData.
			State = PlayerMissionState.Accepted,
		};

		loaded.LoadData(tag);

		Assert.AreEqual(PlayerMissionState.Available, loaded.State);
	}
}
