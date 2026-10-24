using Everglow.Commons.Mechanics.MirrorReflection;
using Everglow.Commons.Mechanics.MirrorReflection.Core;
using Everglow.Commons.Netcode;
using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.YggdrasilTown.Walls;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SubworldLibrary;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
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
		ClearVanillaEvents();
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
		On_Main.ShouldNormalEventsBeAbleToStart += StopNaturalEvents;
		On_Main.CanStartInvasion += CanStartInvasion;
		IL_Main.StartInvasion += SkipEventInYggdrasil;
		IL_Main.StartRain += SkipEventInYggdrasil;
		IL_Main.StartSlimeRain += SkipEventInYggdrasil;
		IL_Main.startPumpkinMoon += SkipEventInYggdrasil;
		IL_Main.startSnowMoon += SkipEventInYggdrasil;
		IL_Main.checkHalloween += SkipEventInYggdrasil;
		IL_Main.checkXMas += SkipEventInYggdrasil;
		IL_BirthdayParty.NaturalAttempt += SkipEventInYggdrasil;
		IL_BirthdayParty.ToggleManualParty += SkipEventInYggdrasil;
		IL_LanternNight.NaturalAttempt += SkipEventInYggdrasil;
		IL_LanternNight.ToggleManualLanterns += SkipEventInYggdrasil;
		IL_Sandstorm.StartSandstorm += SkipEventInYggdrasil;
		IL_DD2Event.StartInvasion += SkipEventInYggdrasil;
		IL_DD2Event.UpdateTime += SkipEventInYggdrasil;
		IL_DD2Event.SummonCrystal += SkipEventInYggdrasil;
		IL_DD2Event.SummonCrystalDirect += SkipEventInYggdrasil;
		IL_CultistRitual.TrySpawning += SkipEventInYggdrasil;
		IL_WorldGen.TriggerLunarApocalypse += SkipEventInYggdrasil;
		IL_WorldGen.StartImpendingDoom += SkipEventInYggdrasil;
		IL_WorldGen.dropMeteor += SkipEventInYggdrasil;
		IL_MysticLogFairiesEvent.StartNight += SkipEventInYggdrasil;
		IL_MysticLogFairiesEvent.UpdateTime += SkipEventInYggdrasil;
		IL_Main.UpdateTime_SpawnTownNPCs += PreventOldManSpawningInYggdrasil;
		base.Load();
	}

	public override void Unload()
	{
		On_Main.ShouldNormalEventsBeAbleToStart -= StopNaturalEvents;
		On_Main.CanStartInvasion -= CanStartInvasion;
		IL_Main.StartInvasion -= SkipEventInYggdrasil;
		IL_Main.StartRain -= SkipEventInYggdrasil;
		IL_Main.StartSlimeRain -= SkipEventInYggdrasil;
		IL_Main.startPumpkinMoon -= SkipEventInYggdrasil;
		IL_Main.startSnowMoon -= SkipEventInYggdrasil;
		IL_Main.checkHalloween -= SkipEventInYggdrasil;
		IL_Main.checkXMas -= SkipEventInYggdrasil;
		IL_BirthdayParty.NaturalAttempt -= SkipEventInYggdrasil;
		IL_BirthdayParty.ToggleManualParty -= SkipEventInYggdrasil;
		IL_LanternNight.NaturalAttempt -= SkipEventInYggdrasil;
		IL_LanternNight.ToggleManualLanterns -= SkipEventInYggdrasil;
		IL_Sandstorm.StartSandstorm -= SkipEventInYggdrasil;
		IL_DD2Event.StartInvasion -= SkipEventInYggdrasil;
		IL_DD2Event.UpdateTime -= SkipEventInYggdrasil;
		IL_DD2Event.SummonCrystal -= SkipEventInYggdrasil;
		IL_DD2Event.SummonCrystalDirect -= SkipEventInYggdrasil;
		IL_CultistRitual.TrySpawning -= SkipEventInYggdrasil;
		IL_WorldGen.TriggerLunarApocalypse -= SkipEventInYggdrasil;
		IL_WorldGen.StartImpendingDoom -= SkipEventInYggdrasil;
		IL_WorldGen.dropMeteor -= SkipEventInYggdrasil;
		IL_MysticLogFairiesEvent.StartNight -= SkipEventInYggdrasil;
		IL_MysticLogFairiesEvent.UpdateTime -= SkipEventInYggdrasil;
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
		}
		base.PostUpdateEverything();
	}

	public override bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber)
	{
		if (!YggdrasilWorld.InYggdrasil || Main.netMode != NetmodeID.Server
			|| messageType != MessageID.SpawnBossUseLicenseStartEvent)
		{
			return false;
		}

		long position = reader.BaseStream.Position;
		try
		{
			if (reader.BaseStream.Length - position < sizeof(short) * 2)
			{
				return false;
			}
			reader.ReadInt16(); // Player index; vanilla validates it after this hook.
			int summon = reader.ReadInt16();
			// -11..-18 are books, licenses and the mechanical boss combination. Other negative
			// values start events; even unknown negatives fall back to a goblin invasion.
			return summon < 0 && summon is not (>= -18 and <= -11);
		}
		finally
		{
			reader.BaseStream.Position = position;
		}
	}

	public override void PreUpdateEntities() => ClearVanillaEvents();

	public override void PreUpdateTime() => ClearVanillaEvents();

	public override void PostUpdateTime() => ClearVanillaEvents();

	private static bool StopNaturalEvents(On_Main.orig_ShouldNormalEventsBeAbleToStart orig)
	{
		// Despite its name, true is the stopEvents flag consumed by vanilla time updates.
		return YggdrasilWorld.InYggdrasil || orig();
	}

	private static bool CanStartInvasion(On_Main.orig_CanStartInvasion orig, int type, bool ignoreDelay)
	{
		return !YggdrasilWorld.InYggdrasil && orig(type, ignoreDelay);
	}

	private static void SkipEventInYggdrasil(ILContext il)
	{
		// An entry guard does not depend on vanilla local-variable layouts or instruction patterns.
		ILCursor cursor = new(il);
		var originalEntry = cursor.DefineLabel();
		cursor.EmitDelegate<Func<bool>>(() => YggdrasilWorld.InYggdrasil);
		cursor.Emit(OpCodes.Brfalse, originalEntry);
		cursor.Emit(OpCodes.Ret);
		cursor.MarkLabel(originalEntry);
	}

	private static void ClearVanillaEvents()
	{
		if (!YggdrasilWorld.InYggdrasil)
		{
			return;
		}

		Main.bloodMoon = false;
		Main.eclipse = false;
		Main.pumpkinMoon = false;
		Main.snowMoon = false;
		Main.slimeRain = false;
		Main.slimeRainTime = 0;
		Main.slimeRainKillCount = 0;
		Main.slimeWarningTime = 0;
		Main.raining = false;
		Main.rainTime = 0;
		Main.maxRaining = 0;
		Main.windSpeedCurrent = 0;
		Main.windSpeedTarget = 0;
		Main.windCounter = 0;
		Star.starfallBoost = 1f;
		Main.invasionType = InvasionID.None;
		Main.invasionSize = 0;
		Main.invasionSizeStart = 0;
		Main.invasionDelay = 0;
		Main.invasionWarn = 0;
		Main.invasionProgress = 0;
		Main.invasionProgressMax = 0;
		Main.invasionProgressWave = 0;
		Main.invasionProgressDisplayLeft = 0;
		Main.invasionProgressAlpha = 0;
		Main.invasionProgressIcon = 0;
		Main.halloween = false;
		Main.xMas = false;
		Main.forceHalloweenForToday = false;
		Main.forceXMasForToday = false;
		BirthdayParty.ManualParty = false;
		BirthdayParty.GenuineParty = false;
		BirthdayParty.CelebratingNPCs.Clear();
		LanternNight.ManualLanterns = false;
		LanternNight.GenuineLanterns = false;
		LanternNight.NextNightIsLanternNight = false;
		Sandstorm.Happening = false;
		Sandstorm.TimeLeft = 0;
		Sandstorm.Severity = 0;
		Sandstorm.IntendedSeverity = 0;
		DD2Event.Ongoing = false;
		NPC.LunarApocalypseIsUp = false;
		NPC.TowerActiveSolar = false;
		NPC.TowerActiveVortex = false;
		NPC.TowerActiveNebula = false;
		NPC.TowerActiveStardust = false;
		NPC.MoonLordCountdown = 0;
		WorldGen.spawnEye = false;
		WorldGen.spawnHardBoss = 0;
		// Vanilla queues a meteor before checking stopEvents at dusk.
		WorldGen.spawnMeteor = false;

		if (!Main.dedServ)
		{
			// Subworld transitions can leave a sky active after its event flag has been cleared.
			SkyManager.Instance["Party"]?.Reset();
			SkyManager.Instance["Lantern"]?.Reset();
			SkyManager.Instance["Sandstorm"]?.Reset();
			SkyManager.Instance["Slime"]?.Reset();
			Filters.Scene["Sandstorm"]?.Deactivate();
			Overlays.Scene["Sandstorm"]?.Deactivate();
			Filters.Scene["MoonLordShake"]?.Deactivate();
		}
	}
}
