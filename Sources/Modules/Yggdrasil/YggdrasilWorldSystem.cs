using System.IO;
using Everglow.Commons.Netcode;
using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.Netcode;
using SubworldLibrary;
using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil;

public class YggdrasilWorldSystem : ModSystem, ICopyWorldData
{
	private const string ProgressKey = "EverglowYggdrasilProgress";

	// 同一存档内共享进度；主服持有权威状态，任务、商店和其他内容只读取。
	public static bool DownedSquamousShell;

	public override void OnWorldLoad()
	{
		if (NetUtils.IsSubServer)
		{
			// 新启动的子服向主服获取最新进度，同时兼容旧子世界存档中的记录。
			ModIns.PacketResolver.Route(new YggdrasilProgressSyncPacket(DownedSquamousShell), RouteDestination.MainServer);
		}
	}

	void ICopyWorldData.CopyMainWorldData()
	{
		SubworldSystem.CopyWorldData(ProgressKey, DownedSquamousShell ? 1 : 0);
	}

	void ICopyWorldData.ReadCopiedMainWorldData()
	{
		// SubworldLibrary 在单机往返世界时也会调用这组接口。
		DownedSquamousShell |= SubworldSystem.ReadCopiedWorldData<int>(ProgressKey) != 0;
	}

	public override void ClearWorld()
	{
		DownedSquamousShell = false;
	}

	public override void SaveWorldData(TagCompound tag)
	{
		if (!NetUtils.IsSubServer && DownedSquamousShell)
		{
			tag[nameof(DownedSquamousShell)] = true;
		}
	}

	public override void LoadWorldData(TagCompound tag)
	{
		DownedSquamousShell = tag.GetBool(nameof(DownedSquamousShell));
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(DownedSquamousShell);
	}

	public override void NetReceive(BinaryReader reader)
	{
		DownedSquamousShell |= reader.ReadBoolean();
	}

	public override void PostUpdateEverything()
	{
		if (YggdrasilWorld.InYggdrasil)
		{
			YggdrasilWorld.YggdrasilTimer++;

			if (Main.bloodMoon)
			{
				Main.bloodMoon = false;
			}
			if (Main.slimeRain)
			{
				Main.slimeRain = false;
			}
		}
		base.PostUpdateEverything();
	}
}
