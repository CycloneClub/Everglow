using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Netcode;
using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Packets;

public class ObjectiveDeltaSyncPacket_SubProgress : IPacket
{
	private string _missionName;

	private IDeltaSyncObjective syncObjective;

	public ObjectiveDeltaSyncPacket_SubProgress()
	{
	}

	public ObjectiveDeltaSyncPacket_SubProgress(string missionName, IDeltaSyncObjective objective)
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
			deltaSyncObjective.ReceiveDelta(reader);
			ModIns.PacketResolver.Route(new ObjectiveDeltaSyncPacket_MainProgress(missionName, deltaSyncObjective), RouteDestination.AllDownstream);
		}
		else
		{
			Ins.Logger.Error($"{missionName} {objectiveId} {objective.GetType().Name} is not {nameof(IDeltaSyncObjective)}.");
		}
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(_missionName); // Mission id
		writer.Write((syncObjective as WorldObjectiveBase).ObjectiveID); // Objective id
		syncObjective.SendDelta(writer);
	}

	[HandlePacket(typeof(ObjectiveDeltaSyncPacket_SubProgress))]
	public class MissionDeltaSyncPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
		}
	}
}
