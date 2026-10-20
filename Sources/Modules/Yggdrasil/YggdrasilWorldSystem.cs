using Everglow.Commons.Mechanics.MirrorReflection;
using Everglow.Commons.Mechanics.MirrorReflection.Core;
using Everglow.Commons.Netcode;
using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.YggdrasilTown.Walls;
using MonoMod.Cil;
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

	public override void PostSetupContent()
	{
		var reflections = ModContent.GetInstance<MirrorReflectionSystem>();
		reflections.RegisterWall(ModContent.WallType<JellyBallSecretionWall>(), new MirrorReflectionDefinition(0.22f, 0.9f) { Blur = 0 });
		reflections.RegisterWall(ModContent.WallType<HeatProofPlatingWall>(), new MirrorReflectionDefinition(0.26f) { Blur = 1 });
		reflections.RegisterWall(ModContent.WallType<FurnaceHeatProofPlatingWall>(), new MirrorReflectionDefinition(0.30f) { Blur = 1 });
		reflections.RegisterWall(ModContent.WallType<UnionMarbleWall_Dark>(), new MirrorReflectionDefinition(0.20f, 0.9f) { Blur = 0 });
	}

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
		IL_Main.UpdateTime_SpawnTownNPCs += PreventOldManSpawningInYggdrasil;
		base.Load();
	}

	public override void Unload()
	{
		IL_Main.UpdateTime_SpawnTownNPCs -= PreventOldManSpawningInYggdrasil;
	}

	private static void PreventOldManSpawningInYggdrasil(ILContext il)
	{
		ILCursor cursor = new(il);
		if (!cursor.TryGotoNext(
			MoveType.Before,
			i => i.MatchLdsfld<NPC>(nameof(NPC.downedBoss3)),
			i => i.MatchBrtrue(out _),
			i => i.MatchLdloc(out _),
			i => i.MatchBrtrue(out _),
			i => i.MatchCall<NPC>(nameof(NPC.GetSpawnSourceForTownSpawn)),
			i => i.MatchLdsfld<Main>(nameof(Main.dungeonX))))
		{
			throw new InvalidOperationException("Failed to locate the Old Man spawn condition in Main.UpdateTime_SpawnTownNPCs.");
		}

		// 天穹树没有原版地牢：仅让老人补生条件跳过，不修改骷髅王进度或后续城镇刷新。
		cursor.Index++;
		cursor.EmitDelegate<Func<bool, bool>>(downedSkeletron => downedSkeletron || YggdrasilWorld.InYggdrasil);
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
