using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Packets;

public sealed class ObjectiveRetryRequestPacket : IPacket
{
	public ObjectiveRetryRequestPacket()
	{
	}

	public ObjectiveRetryRequestPacket(string questName, int objectiveId)
	{
		QuestName = questName;
		ObjectiveId = objectiveId;
	}

	public string QuestName { get; private set; } = string.Empty;

	public int ObjectiveId { get; private set; }

	public void Send(BinaryWriter writer)
	{
		writer.Write(QuestName);
		writer.Write(ObjectiveId);
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		QuestName = reader.ReadString();
		ObjectiveId = reader.ReadInt32();
	}

	[HandlePacket(typeof(ObjectiveRetryRequestPacket))]
	public sealed class ObjectiveRetryRequestPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			if (packet is ObjectiveRetryRequestPacket request)
			{
				WorldQuestManager.Instance.TryRetryObjective(request.QuestName, request.ObjectiveId);
			}
		}
	}
}
