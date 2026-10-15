using Everglow.Commons.Mechanics.Quest.Core;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

public abstract partial class WorldQuestBase
{
	public virtual void NetSend(BinaryWriter writer)
	{
		writer.Write((int)State);
		writer.Write(Time);
		writer.Write(_rewardClaimedPlayers.Count);
		foreach (string player in _rewardClaimedPlayers)
		{
			writer.Write(player);
		}
		Objectives.NetSend(writer);
	}

	public virtual void NetReceive(BinaryReader reader)
	{
		var oldState = State;
		State = (WorldQuestState)reader.ReadInt32();
		Time = reader.ReadInt32();
		int rewardPlayerCount = reader.ReadInt32();
		_rewardClaimedPlayers.Clear();
		for (int i = 0; i < rewardPlayerCount; i++)
		{
			_rewardClaimedPlayers.Add(reader.ReadString());
		}
		Objectives.NetReceive(reader);
		if (!RecoverInvalidObjectiveState())
		{
			ApplyObjectiveSnapshot(oldState, State);
		}

		if (oldState != State)
		{
			if (State == WorldQuestState.Active)
			{
				if (oldState == WorldQuestState.Locked)
				{
					WorldQuestManager.Notify(this, QuestNotificationType.Unlocked);
				}
				else
				{
					WorldQuestManager.Notify(this, QuestNotificationType.Restored);
				}
			}
			else if (State == WorldQuestState.Completed)
			{
				WorldQuestManager.Notify(this, QuestNotificationType.Completed);
			}
			else if (State == WorldQuestState.Failed)
			{
				WorldQuestManager.Notify(this, QuestNotificationType.Failed);
			}
		}
	}

	public void OnMPSync()
	{
		Objectives.OnMPSync();
	}
}
