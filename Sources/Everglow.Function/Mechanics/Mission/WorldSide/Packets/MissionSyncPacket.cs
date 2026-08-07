using Everglow.Commons.Mechanics.Mission.WorldSide;
using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Packets;

public class MissionSyncPacket : IPacket
{
	private WorldMissionBase _mission;

	public MissionSyncPacket()
	{
	}

	public MissionSyncPacket(WorldMissionBase mission)
	{
		_mission = mission;
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		var name = reader.ReadString();
		_mission = WorldMissionManager.Instance.GetMission(name);
		_mission.NetReceive(reader);
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(_mission.Name);
		_mission.NetSend(writer);
	}

	[HandlePacket(typeof(MissionSyncPacket))]
	public class MissionSyncPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
		}
	}
}
