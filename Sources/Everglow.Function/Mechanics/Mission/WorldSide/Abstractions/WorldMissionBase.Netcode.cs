namespace Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

public abstract partial class WorldMissionBase : IMissionNetcode
{
	public virtual void NetSend(BinaryWriter writer)
	{
		writer.Write((int)State);
		writer.Write(Time);
		writer.Write(RewardClaimed);
		writer.Write(RewardClaimedPlayers.Count);
		foreach (var player in RewardClaimedPlayers)
		{
			writer.Write(player);
		}
		Objectives.NetSend(writer);
	}

	public virtual void NetReceive(BinaryReader reader)
	{
		var oldState = State;
		State = (WorldMissionState)reader.ReadInt32();
		Time = reader.ReadInt32();
		RewardClaimed = reader.ReadBoolean();
		int rewardPlayerCount = reader.ReadInt32();
		for (int i = 0; i < rewardPlayerCount; i++)
		{
			RewardClaimedPlayers.Add(reader.ReadString());
		}
		Objectives.NetReceive(reader);
		if (!RecoverInvalidObjectiveState())
		{
			ApplyObjectiveSnapshot(oldState, State);
		}

		if (oldState != State)
		{
			if (State == WorldMissionState.Active)
			{
				if (oldState == WorldMissionState.Locked)
				{
					var unlockText = $"[{DisplayName}]任务已解锁";
					var unlockTextColor = new Color(150, 150, 250);
					WorldMissionManager.NewText(unlockText, unlockTextColor);
				}
				else
				{
					var unlockText = $"[{DisplayName}]任务已恢复";
					var unlockTextColor = new Color(150, 150, 250);
					WorldMissionManager.NewText(unlockText, unlockTextColor);
				}
			}
			else if (State == WorldMissionState.Completed)
			{
				var completeText = $"[{DisplayName}]任务已完成";
				var completeTextColor = new Color(150, 250, 150);
				WorldMissionManager.NewText(completeText, completeTextColor);
			}
			else if (State == WorldMissionState.Failed)
			{
				var failText = $"[{DisplayName}]任务已失败";
				var failTextColor = new Color(250, 150, 150);
				WorldMissionManager.NewText(failText, failTextColor);
			}
		}
	}

	public void OnMPSync()
	{
		Objectives.OnMPSync();
	}
}
