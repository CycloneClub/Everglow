using Everglow.Commons.Mechanics.Mission.WorldSide;
using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Packets;

public class ObjectiveDeltaSyncPacket_MainProgress : IPacket
{
	private string _missionName;

	private IDeltaSyncObjective syncObjective;

	public ObjectiveDeltaSyncPacket_MainProgress()
	{
	}

	public ObjectiveDeltaSyncPacket_MainProgress(string missionName, IDeltaSyncObjective objective)
	{
		_missionName = missionName;
		syncObjective = objective;
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		var missionName = reader.ReadString();
		var objectiveId = reader.ReadInt32();
		var mission = WorldMissionManager.Instance.GetMission(missionName);
		var objective = mission.Objectives.AllObjectives[objectiveId];
		if (objective is IDeltaSyncObjective deltaSyncObjective)
		{
			deltaSyncObjective.ReceiveMain(reader);
		}
		else
		{
			Ins.Logger.Error($"{missionName} {objectiveId} {objective.GetType().Name} is not {nameof(IDeltaSyncObjective)}.");
		}
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(_missionName);
		writer.Write((syncObjective as WorldObjectiveBase).ObjectiveID);
		syncObjective.SendMain(writer);
	}

	[HandlePacket(typeof(ObjectiveDeltaSyncPacket_MainProgress))]
	public class MissionDeltaSyncPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
		}
	}
}
