using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Packets;

public sealed class QuestClaimRewardPacket : IPacket
{
	public QuestClaimRewardPacket()
	{
	}

	public QuestClaimRewardPacket(string questName)
	{
		QuestName = questName;
	}

	public string QuestName { get; private set; } = string.Empty;

	public void Send(BinaryWriter writer) => writer.Write(QuestName);

	public void Receive(BinaryReader reader, int whoAmI) => QuestName = reader.ReadString();

	[HandlePacket(typeof(QuestClaimRewardPacket))]
	public sealed class QuestRewardClaimRequestPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			if (packet is QuestClaimRewardPacket request)
			{
				WorldQuestManager.Instance.TryClaimReward(request.QuestName, whoAmI);
			}
		}
	}
}
