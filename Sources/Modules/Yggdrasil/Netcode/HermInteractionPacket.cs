using Everglow.Commons.Netcode.Abstracts;
using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.YggdrasilTown.Quests;

namespace Everglow.Yggdrasil.Netcode;

/// <summary>主服只接受当前世界服务端验证后的对话事件。</summary>
public sealed class HermValidatedInteractionPacket : IPacket
{
	public void Send(BinaryWriter writer)
	{
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
	}

	[HandlePacket(typeof(HermValidatedInteractionPacket))]
	public sealed class Handler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			if (whoAmI == -1 && NetUtils.IsMainServer && packet is HermValidatedInteractionPacket)
			{
				LostMinerQuest.ApplyValidatedMeeting();
			}
		}
	}
}
