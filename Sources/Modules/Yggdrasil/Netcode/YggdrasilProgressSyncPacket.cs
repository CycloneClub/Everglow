using Everglow.Commons.Netcode;
using Everglow.Commons.Netcode.Abstracts;
using Everglow.Commons.Utilities;

namespace Everglow.Yggdrasil.Netcode;

/// <summary>
/// 子服上报击败记录，主服合并后向所有世界广播；空进度也可用于请求当前状态。
/// </summary>
public class YggdrasilProgressSyncPacket : IPacket
{
	private bool downedSquamousShell;

	public YggdrasilProgressSyncPacket()
	{
	}

	public YggdrasilProgressSyncPacket(bool downedSquamousShell)
	{
		this.downedSquamousShell = downedSquamousShell;
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(downedSquamousShell);
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		downedSquamousShell = reader.ReadBoolean();
	}

	[HandlePacket(typeof(YggdrasilProgressSyncPacket))]
	public class YggdrasilProgressSyncPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
			if (NetUtils.IsSingle || whoAmI != -1)
			{
				return;
			}
			var data = (YggdrasilProgressSyncPacket)packet;
			// 击败记录只增加，延迟抵达的旧快照不能撤销已确认的进度。
			YggdrasilWorldSystem.DownedSquamousShell |= data.downedSquamousShell;
			if (NetUtils.IsMainServer)
			{
				ModIns.PacketResolver.Route(
					new YggdrasilProgressSyncPacket(YggdrasilWorldSystem.DownedSquamousShell),
					RouteDestination.AllDownstream);
			}
		}
	}
}
