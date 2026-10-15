using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide.Structure;
using Everglow.Commons.Mechanics.Quest.WorldSide.Structure.Nodes;
using Everglow.Commons.Mechanics.Quest.WorldSide.Packets;
using Everglow.Commons.Netcode;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

public abstract partial class WorldQuestBase
{
	protected WorldQuestBase()
	{
		Objectives.OnNodeCompleted += Objectives_OnNodeCompleted;
		Objectives.OnObjectiveActivated += Objectives_OnObjectiveActivated;
		Objectives.OnObjectiveDeactivated += Objectives_OnObjectiveDeactivated;
		Objectives.OnObjectiveTimedOut += Objectives_OnObjectiveTimedOut;
		Objectives.OnMPSyncTriggered += Objectives_OnMPSyncTriggered;
	}

	public const string RewardItemsSourceContext = "Everglow.QuestSystem";

	private readonly List<WorldObjectiveBase> _activatedObjectives = [];
	private readonly HashSet<string> _rewardClaimedPlayers = new(StringComparer.Ordinal);

	public int WhoAmI { get; internal set; }

	public WorldQuestState State { get; protected set; } = WorldQuestState.Locked;

	public virtual float Progress => Objectives.Progress;

	public WorldObjectiveContainer Objectives { get; } = new();

	public IReadOnlyList<WorldObjectiveBase> ActiveObjectives => _activatedObjectives;

	public int Time { get; protected set; }

	public bool Retriable => true;

	public IReadOnlySet<string> RewardClaimedPlayers => _rewardClaimedPlayers;

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
			WorldQuestManager.Notify(this, QuestNotificationType.Unlocked);

			if (NetUtils.IsMainServer)
			{
				Ins.Logger.Info(unlockText);
				ModIns.PacketResolver.Route(new QuestSyncPacket(this), RouteDestination.AllDownstream);
			}
		}
	}

	private bool UnlockCore()
	{
		if (State != WorldQuestState.Locked)
		{
			return false;
		}

		State = WorldQuestState.Active;
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
			CompleteQuest();
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

		Time += WorldQuestManager.UpdateInterval;
		if (Time >= TimeLimit)
		{
			Time = TimeLimit;

			if (ExpireCore())
			{
				var failText = $"[{DisplayName}]任务已失败";
				WorldQuestManager.Notify(this, QuestNotificationType.Failed);

				if (NetUtils.IsMainServer)
				{
					Ins.Logger.Info(failText);
					ModIns.PacketResolver.Route(new QuestSyncPacket(this), RouteDestination.AllDownstream);
				}
			}

			return false;
		}

		return true;
	}

	private bool ExpireCore()
	{
		if (State != WorldQuestState.Active)
		{
			return false;
		}

		State = WorldQuestState.Failed;
		OnExpire();
		Deactivate();

		return true;
	}

	public void CompleteQuest()
	{
		if (CompleteQuestCore())
		{
			var completeText = $"[{DisplayName}]任务已完成";
			WorldQuestManager.Notify(this, QuestNotificationType.Completed);

			if (NetUtils.IsMainServer)
			{
				Ins.Logger.Info(completeText);
				ModIns.PacketResolver.Route(new QuestSyncPacket(this), RouteDestination.AllDownstream);
			}
		}
	}

	private bool CompleteQuestCore()
	{
		if (State != WorldQuestState.Active)
		{
			return false;
		}

		State = WorldQuestState.Completed;
		OnComplete();
		Deactivate(); // Maybe not necessary, because the current objective is null here.

		return true;
	}

	/// <summary>
	/// Requests the quest retry. Called by clicking the 'retry' button in the quest panel.
	/// </summary>
	public void Retry()
	{
		if (NetUtils.IsSingle)
		{
			if (!RetryCore())
			{
				return;
			}
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
		if (!Retriable || State != WorldQuestState.Failed)
		{
			return false;
		}

		State = WorldQuestState.Active;
		Time = 0;
		ResetProgress();
		Activate();
		WorldQuestManager.Notify(this, QuestNotificationType.Restarted);

		return true;
	}

	public bool CanRetryObjective(int objectiveId)
	{
		if (State != WorldQuestState.Active
			|| Objectives.Current is null
			|| !ReferenceEquals(Objectives.Current, Objectives.FindCurrentNode())
			|| (uint)objectiveId >= (uint)Objectives.AllObjectives.Count)
		{
			return false;
		}

		WorldObjectiveBase objective = Objectives.AllObjectives[objectiveId];
		return !objective.Completed
			&& objective.Timer?.IsExpired == true
			&& Objectives.Current.FindAllEntrances().Contains(objective);
	}

	public bool TryRetryObjectiveCore(int objectiveId)
	{
		if (!CanRetryObjective(objectiveId))
		{
			return false;
		}

		Objectives.AllObjectives[objectiveId].ResetProgress();
		RefreshActivatedObjectives(questActive: true);
		return true;
	}

	public bool CanClaimReward(string playerName) =>
		State == WorldQuestState.Completed
		&& !string.IsNullOrEmpty(playerName)
		&& !_rewardClaimedPlayers.Contains(playerName);

	public bool TryRecordRewardClaim(string playerName) =>
		CanClaimReward(playerName) && _rewardClaimedPlayers.Add(playerName);

	public void GiveRewards(Player player)
	{
		foreach (Item item in RewardItems)
		{
			player.QuickSpawnItem(player.GetSource_Misc(RewardItemsSourceContext), item, item.stack);
		}
	}

	/// <summary>
	/// Resets the quest to its initial state, clearing progress, time, and objectives.
	/// <br/> This is called when loading a world.
	/// <para/> Override <see cref="OnReset"/> to add custom reset behavior.
	/// </summary>
	public void Reset()
	{
		State = WorldQuestState.Locked;
		Time = 0;
		_rewardClaimedPlayers.Clear();
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
	private void ApplyObjectiveSnapshot(WorldQuestState oldState, WorldQuestState newState)
	{
		var questActiveBefore = oldState == WorldQuestState.Active;
		var questActiveAfter = newState == WorldQuestState.Active;
		Objectives.ApplyObjectiveSnapshot(questActiveBefore, questActiveAfter);
		RefreshActivatedObjectives(questActiveAfter);
	}

	private void RefreshActivatedObjectives(bool questActive)
	{
		List<WorldObjectiveBase> desiredObjectives = questActive
			? Objectives.FindCurrentObjectives().Where(objective => objective.CanProgress).ToList()
			: [];

		foreach (WorldObjectiveBase objective in _activatedObjectives.Except(desiredObjectives).ToList())
		{
			objective.Deactivate();
			_activatedObjectives.Remove(objective);
		}

		foreach (WorldObjectiveBase objective in desiredObjectives.Except(_activatedObjectives).ToList())
		{
			objective.Activate(this);
			_activatedObjectives.Add(objective);
		}
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
		WorldQuestManager.Notify(this, QuestNotificationType.ObjectiveCompleted, current?.GetType().Name);

		if (NetUtils.IsMainServer)
		{
			Ins.Logger.Info(objectiveCompleteText);
			if (current != null) // Skip the last objective sync because a packet for completion will be sent.
			{
				ModIns.PacketResolver.Route(new QuestSyncPacket(this), RouteDestination.AllDownstream);
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
			if (!objective.CanProgress)
			{
				continue;
			}

			objective.Activate(this);
			_activatedObjectives.Add(objective);
		}
	}

	private void Objectives_OnObjectiveTimedOut(WorldObjectiveBase objective)
	{
		if (!_activatedObjectives.Remove(objective))
		{
			return;
		}

		objective.Deactivate();

		if (NetUtils.IsMainServer)
		{
			ModIns.PacketResolver.Route(new QuestSyncPacket(this), RouteDestination.AllDownstream);
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
		else if (NetUtils.IsMainServer)
		{
			ModIns.PacketResolver.Route(new ObjectiveDeltaSyncPacket_MainProgress(Name, deltaSync), RouteDestination.AllDownstream);
		}
	}
}
