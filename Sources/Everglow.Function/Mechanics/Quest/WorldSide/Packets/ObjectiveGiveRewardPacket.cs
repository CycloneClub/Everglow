using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Packets;

/// <summary>
/// 服务端通知当前世界的每个在线客户端发放一份目标奖励。
/// 不补领、不重发；完成状态快照不负责发奖。
/// </summary>
public sealed class ObjectiveGiveRewardPacket : IPacket
{
	public ObjectiveGiveRewardPacket()
	{
	}

	public ObjectiveGiveRewardPacket(string questName, int objectiveId)
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

	[HandlePacket(typeof(ObjectiveGiveRewardPacket))]
	public sealed class Handler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			// PacketResolver 将来自服务器的可信发送者表示为 -1。
			if (Main.netMode != NetmodeID.MultiplayerClient || whoAmI != -1
				|| packet is not ObjectiveGiveRewardPacket reward)
			{
				return;
			}

			var quest = WorldQuestManager.Instance.GetQuest(reward.QuestName);
			if (quest is null || reward.ObjectiveId < 0 || reward.ObjectiveId >= quest.Objectives.AllObjectives.Count)
			{
				return;
			}

			// 快照可能已经把 RewardClaimed 同步为 true，这不代表本地玩家已经收到了奖励。
			quest.Objectives.AllObjectives[reward.ObjectiveId].GiveRewards(Main.LocalPlayer);
		}
	}
}
