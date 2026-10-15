using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Packets;

public sealed class QuestGiveRewardPacket : IPacket
{
	public QuestGiveRewardPacket()
	{
	}

	public QuestGiveRewardPacket(string questName, int whoAmI, string expectedPlayerName)
	{
		QuestName = questName;
		PlayerWhoAmI = whoAmI;
		ExpectedPlayerName = expectedPlayerName;
	}

	public string QuestName { get; private set; } = string.Empty;

	public int PlayerWhoAmI { get; private set; }

	public string ExpectedPlayerName { get; private set; } = string.Empty;

	public void Send(BinaryWriter writer)
	{
		writer.Write(QuestName);
		writer.Write(PlayerWhoAmI);
		writer.Write(ExpectedPlayerName);
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		QuestName = reader.ReadString();
		PlayerWhoAmI = reader.ReadInt32();
		ExpectedPlayerName = reader.ReadString();
	}

	[HandlePacket(typeof(QuestGiveRewardPacket))]
	public sealed class QuestRewardGrantPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			if (packet is QuestGiveRewardPacket grant)
			{
				WorldQuestManager.Instance.TryGiveRewards(
					grant.QuestName,
					grant.PlayerWhoAmI,
					grant.ExpectedPlayerName,
					whoAmI);
			}
		}
	}
}
