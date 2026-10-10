using Everglow.Commons.Netcode;
using Everglow.Commons.Netcode.Abstracts;
using Everglow.Commons.Utilities;

namespace Everglow.Yggdrasil.Netcode;

/// <summary>
/// 子服上报世界进度，主服合并后向所有世界广播；空进度也可用于请求当前状态。
/// </summary>
public class YggdrasilProgressSyncPacket : IPacket
{
	private bool downedSquamousShell;
	private bool downedKingJellyBall;
	private bool enteredKelpCurtain;

	public YggdrasilProgressSyncPacket()
	{
	}

	public YggdrasilProgressSyncPacket(bool downedSquamousShell = false, bool downedKingJellyBall = false, bool enteredKelpCurtain = false)
	{
		this.downedSquamousShell = downedSquamousShell;
		this.downedKingJellyBall = downedKingJellyBall;
		this.enteredKelpCurtain = enteredKelpCurtain;
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(downedSquamousShell);
		writer.Write(downedKingJellyBall);
		writer.Write(enteredKelpCurtain);
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		downedSquamousShell = reader.ReadBoolean();
		downedKingJellyBall = reader.ReadBoolean();
		enteredKelpCurtain = reader.ReadBoolean();
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
			// 世界进度只增加，延迟抵达的旧快照不能撤销已确认的进度。
			YggdrasilWorldSystem.DownedSquamousShell |= data.downedSquamousShell;
			YggdrasilWorldSystem.DownedKingJellyBall |= data.downedKingJellyBall;
			YggdrasilWorldSystem.EnteredKelpCurtain |= data.enteredKelpCurtain;
			if (NetUtils.IsMainServer)
			{
				ModIns.PacketResolver.Route(
					new YggdrasilProgressSyncPacket(
						YggdrasilWorldSystem.DownedSquamousShell,
						YggdrasilWorldSystem.DownedKingJellyBall, YggdrasilWorldSystem.EnteredKelpCurtain),
					RouteDestination.AllDownstream);
			}
		}
	}
}
