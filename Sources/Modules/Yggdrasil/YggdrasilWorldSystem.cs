using System.IO;
using Everglow.Commons.Netcode;
using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.KelpCurtain;
using SubworldLibrary;
using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil;

public class YggdrasilWorldSystem : ModSystem, ICopyWorldData
{
	private const string ProgressKey = "EverglowYggdrasilProgress";

	// 同一存档内共享进度；主服持有权威状态，任务、商店和其他内容只读取。
	public static bool DownedSquamousShell;
	public static bool DownedKingJellyBall;
	public static bool EnteredKelpCurtain;

	public override void OnWorldLoad()
	{
		if (NetUtils.IsSubServer)
		{
			// 新启动的子服向主服获取最新进度，同时兼容旧子世界存档中的记录。
			ModIns.PacketResolver.Route(new YggdrasilProgressSyncPacket(DownedSquamousShell, DownedKingJellyBall, EnteredKelpCurtain), RouteDestination.MainServer);
		}
	}

	void ICopyWorldData.CopyMainWorldData()
	{
		int progress = (DownedSquamousShell ? 1 : 0) | (DownedKingJellyBall ? 2 : 0) | (EnteredKelpCurtain ? 4 : 0);
		SubworldSystem.CopyWorldData(ProgressKey, progress);
	}

	void ICopyWorldData.ReadCopiedMainWorldData()
	{
		// SubworldLibrary 在单机往返世界时也会调用这组接口。
		int progress = SubworldSystem.ReadCopiedWorldData<int>(ProgressKey);
		DownedSquamousShell |= (progress & 1) != 0;
		DownedKingJellyBall |= (progress & 2) != 0;
		EnteredKelpCurtain |= (progress & 4) != 0;
	}

	public override void ClearWorld()
	{
		DownedSquamousShell = false;
		DownedKingJellyBall = false;
		EnteredKelpCurtain = false;
	}

	public override void SaveWorldData(TagCompound tag)
	{
		if (!NetUtils.IsSubServer)
		{
			tag[nameof(DownedSquamousShell)] = DownedSquamousShell;
			tag[nameof(DownedKingJellyBall)] = DownedKingJellyBall;
			tag[nameof(EnteredKelpCurtain)] = EnteredKelpCurtain;
		}
	}

	public override void LoadWorldData(TagCompound tag)
	{
		DownedSquamousShell = tag.GetBool(nameof(DownedSquamousShell));
		DownedKingJellyBall = tag.GetBool(nameof(DownedKingJellyBall));
		EnteredKelpCurtain = tag.GetBool(nameof(EnteredKelpCurtain));
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(DownedSquamousShell);
		writer.Write(DownedKingJellyBall);
		writer.Write(EnteredKelpCurtain);
	}

	public override void NetReceive(BinaryReader reader)
	{
		DownedSquamousShell |= reader.ReadBoolean();
		DownedKingJellyBall |= reader.ReadBoolean();
		EnteredKelpCurtain |= reader.ReadBoolean();
	}

	public override void Load()
	{
		On_NPC.AI_007_FindGoodRestingSpot += On_hook_AI_007_FindGoodRestingSpot;
		On_NPC.AI_007_TryForcingSitting += On_hook_AI_007_TryForcingSitting;
		base.Load();
	}

	public void On_hook_AI_007_FindGoodRestingSpot(On_NPC.orig_AI_007_FindGoodRestingSpot orig, NPC self, int myTileX, int myTileY, out int floorX, out int floorY)
	{
		if (SubworldSystem.Current is YggdrasilWorld)
		{
			floorX = -1;
			floorY = -1;
			orig(self, myTileX, myTileY, out floorX, out floorY);
			return;
		}
		orig(self, myTileX, myTileY, out floorX, out floorY);
	}

	public void On_hook_AI_007_TryForcingSitting(On_NPC.orig_AI_007_TryForcingSitting orig, NPC self, int floorX, int floorY)
	{
		if (SubworldSystem.Current is YggdrasilWorld)
		{
			if (floorX > 0 && floorX < Main.tile.Width && floorY > 0 && floorY < Main.tile.Height)
			{
				orig(self, floorX, floorY);
			}
			return;
		}
		orig(self, floorX, floorY);
	}

	public override void PostUpdateEverything()
	{
		if (YggdrasilWorld.InYggdrasil)
		{
			YggdrasilWorld.YggdrasilTimer++;

			if (!NetUtils.IsClient && !EnteredKelpCurtain
				&& (NetUtils.IsSingle || Main.GameUpdateCount % 60 == 0))
			{
				foreach (var player in Main.ActivePlayers)
				{
					if (!KelpCurtainBiome.IsKelpCurtainLayer(player))
					{
						continue;
					}
					if (NetUtils.IsSingle)
					{
						EnteredKelpCurtain = true;
					}
					else
					{
						ModIns.PacketResolver.Route(new YggdrasilProgressSyncPacket(enteredKelpCurtain: true), RouteDestination.MainServer);
					}
					break;
				}
			}

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
