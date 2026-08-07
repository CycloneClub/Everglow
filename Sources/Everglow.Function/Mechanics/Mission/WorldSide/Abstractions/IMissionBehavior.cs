using Everglow.Commons.Mechanics.Mission.WorldSide.MissionStructure;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

public interface IMissionBehavior
{
	public int WhoAmI { get; }

	public WorldMissionState State { get; }

	public float Progress { get; }

	public WorldStructuralObjectiveContainer Objectives { get; }

	public int Time { get; }

	public bool Retriable { get; }

	/// <summary>
	/// A flag indicating whether the mission reward has been claimed.
	/// <br/>This is used to prevent players from claiming rewards multiple times by resetting the mission.
	/// <para/> TODO: Use player id list instead of a single flag to support multiplayer.
	/// </summary>
	public bool RewardClaimed { get; }

	public void Unlock();

	public void CompleteMission();

	public void Update();

	public void Retry();

	public void GiveRewards();

	// public void Activate();

	// public void Deactivate();

	public void Reset();

	public void ResetProgress();
}
