using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Packets;

public class QuestSyncPacket : IPacket
{
	private WorldQuestBase _quest;

	public QuestSyncPacket()
	{
	}

	public QuestSyncPacket(WorldQuestBase quest)
	{
		_quest = quest;
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		var name = reader.ReadString();
		_quest = WorldQuestManager.Instance.GetQuest(name);
		_quest.NetReceive(reader);
		WorldQuestManager.Instance.OnQuestStatusUpdated(_quest);
		WorldQuestManager.Instance.OnQuestObjectiveUpdated(_quest);
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(_quest.Name);
		_quest.NetSend(writer);
	}

	[HandlePacket(typeof(QuestSyncPacket))]
	public class QuestSyncPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
		}
	}
}
