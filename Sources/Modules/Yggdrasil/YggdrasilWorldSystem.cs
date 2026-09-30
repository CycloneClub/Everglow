using System.IO;
using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil;

public class YggdrasilWorldSystem : ModSystem
{
	// 世界进度独立于任务；任务、商店和其他内容都可以读取这些状态。
	public static bool DownedSquamousShell;

	public override void ClearWorld()
	{
		DownedSquamousShell = false;
	}

	public override void SaveWorldData(TagCompound tag)
	{
		if (DownedSquamousShell)
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
		DownedSquamousShell = reader.ReadBoolean();
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
