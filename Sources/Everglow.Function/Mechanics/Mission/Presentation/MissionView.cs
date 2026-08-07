using Everglow.Commons.Mechanics.Mission.Core;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.WorldSide;
using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

namespace Everglow.Commons.Mechanics.Mission.Presentation;

public class MissionView
{
	public MissionView(PlayerMissionBase mission)
	{
		DisplayName = mission.DisplayName;
		State = MapState(mission.State);
		Type = mission.Type;
		TimeLimit = mission.TimeLimit;
		Source = mission.Source;
		Progress = mission.Progress;
		Visible = mission.IsVisible;
		Retriable = false;

		CompletedSteps = mission.Objectives.AllObjectives.Count(x => x.Completed);
		TotalSteps = mission.Objectives.AllObjectives.Count;

		Description = mission.Description;
		//CompletedObjectives = mission.Objectives.AllObjectives.Where(o => o.Completed).Select(o => new ObjectiveView(o));

		Rewards = mission.RewardItems;
		//ExtraRewards = mission.ExtraRewardsText;
	}

	public MissionView(WorldMissionBase mission)
	{
		DisplayName = mission.DisplayName;
		State = MapState(mission.State);
		Type = mission.Type;
		TimeLimit = mission.TimeLimit;
		Time = mission.Time;
		Source = mission.Source;
		Progress = mission.Progress;
		Visible = mission.Visible;
		Retriable = mission.Retriable;

		CompletedSteps = 0;
		TotalSteps = 0;

		//Description = mission.Description;
		//CompletedObjectives = mission.Objectives.AllObjectives.Where(o => o.Completed).Select(o => new ObjectiveView(o));

		//CurrentNode = new NodeView(mission.Objectives.FindCurrentNode());

		//CanClaimReward = !mission.RewardClaimed || !mission.RewardClaimedPlayers.Contains(Main.LocalPlayer.name);
		//Rewards = mission.RewardItems;
		//ExtraRewards = mission.ExtraRewardsText;
	}

	// Overview
	// ========
	public string DisplayName;
	public UIMissionState State;
	public MissionType Type;
	public long TimeLimit;
	public long Time;
	public MissionSourceBase Source;
	public float Progress;
	public bool Visible;
	public bool Retriable;

	// Detail
	// ======

	// Progress
	public int CompletedSteps;
	public int TotalSteps;

	public string Description;

	public IEnumerable<ObjectiveView> CompletedObjectives;

	// Current Objectives && Track Info
	public NodeView CurrentNode;

	// Reward
	public bool CanClaimReward;
	public IEnumerable<Item> Rewards;
	public IEnumerable<object> ExtraRewards;

	public static UIMissionState MapState(WorldMissionState state)
	{
		return state switch
		{
			WorldMissionState.Locked => UIMissionState.Locked,
			WorldMissionState.Active => UIMissionState.Accepted,
			WorldMissionState.Completed => UIMissionState.Completed,
			WorldMissionState.Failed => UIMissionState.Failed,
			_ => throw new InvalidDataException("Unknown world mission state."),
		};
	}

	public static UIMissionState MapState(PlayerMissionState state)
	{
		return state switch
		{
			PlayerMissionState.Available => UIMissionState.Available,
			PlayerMissionState.Accepted => UIMissionState.Accepted,
			PlayerMissionState.Failed => UIMissionState.Failed,
			PlayerMissionState.Overdue => UIMissionState.Overdue,
			PlayerMissionState.Completed => UIMissionState.Completed,
			_ => throw new InvalidDataException("Unknown player mission state."),
		};
	}
}
