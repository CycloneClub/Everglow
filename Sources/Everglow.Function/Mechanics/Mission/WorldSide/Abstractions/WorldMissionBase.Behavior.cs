using Everglow.Commons.Mechanics.Mission.WorldSide.MissionStructure;
using Everglow.Commons.Mechanics.Mission.WorldSide.MissionStructure.Nodes;
using Everglow.Commons.Mechanics.Mission.WorldSide.Packets;
using Everglow.Commons.Netcode;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

public abstract partial class WorldMissionBase : IMissionBehavior
{
	protected WorldMissionBase()
	{
		Objectives.OnNodeCompleted += Objectives_OnNodeCompleted;
		Objectives.OnObjectiveActivated += Objectives_OnObjectiveActivated;
		Objectives.OnObjectiveDeactivated += Objectives_OnObjectiveDeactivated;
		Objectives.OnMPSyncTriggered += Objectives_OnMPSyncTriggered;
		Objectives.OnObjectiveSynced += Objectives_OnObjectiveSynced;
	}

	public const string RewardItemsSourceContext = "Everglow.MissionSystem";

	private readonly List<WorldObjectiveBase> _activatedObjectives = [];

	public int WhoAmI { get; internal set; }

	public WorldMissionState State { get; protected set; } = WorldMissionState.Locked;

	public virtual float Progress => Objectives.Progress;

	public WorldStructuralObjectiveContainer Objectives { get; } = new();

	public IReadOnlyList<WorldObjectiveBase> ActiveObjectives => _activatedObjectives;

	public int Time { get; protected set; }

	public bool Retriable => true;

	public bool RewardClaimed { get; protected set; }

	public HashSet<string> RewardClaimedPlayers { get; protected set; } = [];

	public void Activate()
	{
		Objectives.Activate();
	}

	public void Deactivate()
	{
		Objectives.Deactivate();
	}

	public void Unlock()
	{
		if (UnlockCore())
		{
			var unlockText = $"[{DisplayName}]任务已解锁";
			var unlockTextColor = new Color(150, 150, 250);
			WorldMissionManager.NewText(unlockText, unlockTextColor);

			if (NetUtils.IsMainServer)
			{
				Ins.Logger.Info(unlockText);
				ModIns.PacketResolver.Route(new MissionSyncPacket(this), RouteDestination.AllDownstream);
			}
		}
	}

	private bool UnlockCore()
	{
		if (State != WorldMissionState.Locked)
		{
			return false;
		}

		State = WorldMissionState.Active;
		Activate();
		OnUnlock();

		return true;
	}

	public void Update()
	{
		if (!UpdateTime())
		{
			return;
		}

		if (Objectives.Completed)
		{
			CompleteMission();
			return;
		}

		Objectives.UpdateNode();
	}

	public bool UpdateTime()
	{
		if (TimeLimit <= 0)
		{
			return true;
		}

		Time += WorldMissionManager.UpdateInterval;
		if (Time >= TimeLimit)
		{
			Time = TimeLimit;

			if (ExpireCore())
			{
				var failText = $"[{DisplayName}]任务已失败";
				var failTextColor = new Color(250, 150, 150);
				WorldMissionManager.NewText(failText, failTextColor);

				if (NetUtils.IsMainServer)
				{
					Ins.Logger.Info(failText);
					ModIns.PacketResolver.Route(new MissionSyncPacket(this), RouteDestination.AllDownstream);
				}
			}

			return false;
		}

		return true;
	}

	private bool ExpireCore()
	{
		if (State != WorldMissionState.Active)
		{
			return false;
		}

		State = WorldMissionState.Failed;
		OnExpire();
		Deactivate();

		return true;
	}

	public void CompleteMission()
	{
		if (CompleteMissionCore())
		{
			var completeText = $"[{DisplayName}]任务已完成";
			var completeTextColor = new Color(150, 250, 150);
			WorldMissionManager.NewText(completeText, completeTextColor);

			if (NetUtils.IsMainServer)
			{
				Ins.Logger.Info(completeText);
				ModIns.PacketResolver.Route(new MissionSyncPacket(this), RouteDestination.AllDownstream);
			}
		}
	}

	private bool CompleteMissionCore()
	{
		if (State != WorldMissionState.Active)
		{
			return false;
		}

		State = WorldMissionState.Completed;
		OnComplete();
		Deactivate(); // Maybe not necessary, because the current objective is null here.

		return true;
	}

	/// <summary>
	/// Requests the mission retry. Called by clicking the 'retry' button in the mission panel.
	/// </summary>
	public void Retry()
	{
		if (NetUtils.IsSingle)
		{
			if (!RetryCore())
			{
				return;
			}

			WorldMissionManager.NewText($"[{DisplayName}]任务已重启", 150, 250, 150);
		}
		else if (NetUtils.IsMainServer)
		{
			// TODO: Waiting for retry packet
			return;
		}
		else if (NetUtils.IsClient)
		{
			// TODO: Send retry request packet to server
		}
	}

	public bool RetryCore()
	{
		if (!Retriable || State != WorldMissionState.Failed)
		{
			return false;
		}

		State = WorldMissionState.Active;
		Time = 0;
		ResetProgress();
		Activate();

		return true;
	}

	/// <summary>
	/// Requests the mission reward. Called by clicking the 'reward' button in the mission panel.
	/// </summary>
	public void GiveRewards()
	{
		if (RewardClaimed || State != WorldMissionState.Completed)
		{
			return;
		}

		if (NetUtils.IsSingle)
		{
			if (GiveRewardsCore(Main.LocalPlayer.name))
			{
				foreach (var item in RewardItems)
				{
					Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc(RewardItemsSourceContext), item, item.stack);
				}
			}
		}
		else if (NetUtils.IsClient)
		{
			// TODO: Send reward claim request packet to server
		}
	}

	private bool GiveRewardsCore(string player)
	{
		if (State != WorldMissionState.Completed)
		{
			return false;
		}

		if (!RewardClaimed)
		{
			RewardClaimed = true;
		}

		// TODO: We haven't decided how to handle rewards for multiplayers yet.
		RewardClaimedPlayers.Add(player);

		return true;
	}

	/// <summary>
	/// Resets the mission to its initial state, clearing progress, time, and objectives.
	/// <br/> This is called when loading a world.
	/// <para/> Override <see cref="OnReset"/> to add custom reset behavior.
	/// </summary>
	public void Reset()
	{
		State = WorldMissionState.Locked;
		Time = 0;
		RewardClaimed = false;
		RewardClaimedPlayers = [];
		ResetProgress();
		OnReset();
	}

	public void ResetProgress()
	{
		foreach (var objective in _activatedObjectives)
		{
			objective.Deactivate();
		}

		_activatedObjectives.Clear();
		Objectives.ResetProgress();
	}

	/// <summary>
	/// Use this method to adapt to a fully new snapshot.
	/// <br/> This is called when loading a world, or syncing from server.
	/// </summary>
	/// <param name="oldState"></param>
	/// <param name="newState"></param>
	private void ApplyObjectiveSnapshot(WorldMissionState oldState, WorldMissionState newState)
	{
		var missionActiveBefore = oldState == WorldMissionState.Active;
		var missionActiveAfter = newState == WorldMissionState.Active;
		Objectives.ApplyObjectiveSnapshot(missionActiveBefore, missionActiveAfter);
	}

	public virtual void OnUnlock()
	{
	}

	public virtual void OnComplete()
	{
	}

	public virtual void OnExpire()
	{
	}

	public virtual void OnReset()
	{
	}

	private void Objectives_OnNodeCompleted(WorldObjectiveNodeBase current)
	{
		var objectiveCompleteText = $"[{DisplayName}]任务当前节点[{current?.GetType().Name}]中目标已完成";
		var objectiveCompleteTextColor = new Color(250, 250, 150);
		WorldMissionManager.NewText(objectiveCompleteText, objectiveCompleteTextColor);

		if (NetUtils.IsMainServer)
		{
			Ins.Logger.Info(objectiveCompleteText);
			if (current != null) // Skip the last objective sync because a packet for completion will be sent.
			{
				ModIns.PacketResolver.Route(new MissionSyncPacket(this), RouteDestination.AllDownstream);
			}
		}
	}

	private void Objectives_OnObjectiveActivated(WorldObjectiveNodeBase node)
	{
		if (node == null)
		{
			return;
		}

		if (_activatedObjectives.Count != 0)
		{
			throw new InvalidOperationException("Objectives must be deactivated before activation.");
		}

		foreach (var objective in node.FindAllEntrances())
		{
			objective.Activate(this);
			_activatedObjectives.Add(objective);
		}
	}

	private void Objectives_OnObjectiveDeactivated()
	{
		foreach (var objective in _activatedObjectives)
		{
			objective.Deactivate();
		}

		_activatedObjectives.Clear();
	}

	private void Objectives_OnMPSyncTriggered(IDeltaSyncObjective deltaSync)
	{
		if (NetUtils.IsClient || NetUtils.IsSubServer)
		{
			ModIns.PacketResolver.Route(new ObjectiveDeltaSyncPacket_SubProgress(Name, deltaSync), RouteDestination.MainServer);
		}
		else
		{
			ModIns.PacketResolver.Route(new ObjectiveDeltaSyncPacket_MainProgress(Name, deltaSync), RouteDestination.AllDownstream);
		}
	}

	private void Objectives_OnObjectiveSynced(WorldObjectiveNodeBase node)
	{
		var newObjectives = node is not null
			? string.Join(' ', node.FindAllEntrances().Select(x => x.ObjectiveID))
			: "-1";
		WorldMissionManager.NewText($"节点已同步为: {newObjectives}", 150, 250, 150);
		WorldMissionManager.NewText($"目标进度已同步为: {node?.Progress ?? -1}", 150, 250, 150);
	}
}
