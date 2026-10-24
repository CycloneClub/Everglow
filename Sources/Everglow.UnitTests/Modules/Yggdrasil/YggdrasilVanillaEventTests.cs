using System.IO;
using System.Reflection;
using Everglow.SubSpace;
using Everglow.Yggdrasil;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.Skies;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class YggdrasilVanillaEventTests
{
	private readonly Dictionary<FieldInfo, object?> savedFields = new();

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		Save(typeof(Main), "bloodMoon", "eclipse", "pumpkinMoon", "snowMoon", "slimeRain", "slimeRainTime", "slimeRainKillCount", "slimeWarningTime",
			"raining", "rainTime", "maxRaining", "windSpeedCurrent", "windSpeedTarget", "windCounter", "invasionType", "invasionSize", "invasionSizeStart", "invasionDelay", "invasionWarn", "invasionProgress",
			"invasionProgressMax", "invasionProgressWave", "invasionProgressDisplayLeft", "invasionProgressAlpha", "invasionProgressIcon",
			"halloween", "xMas", "forceHalloweenForToday", "forceXMasForToday");
		Save(typeof(BirthdayParty), "ManualParty", "GenuineParty");
		Set(typeof(BirthdayParty), nameof(BirthdayParty.CelebratingNPCs), new List<int>(BirthdayParty.CelebratingNPCs));
		Save(typeof(LanternNight), "ManualLanterns", "GenuineLanterns", "NextNightIsLanternNight");
		Save(typeof(Sandstorm), "Happening", "TimeLeft", "Severity", "IntendedSeverity");
		Save(typeof(DD2Event), "Ongoing");
		Save(typeof(NPC), "LunarApocalypseIsUp", "TowerActiveSolar", "TowerActiveVortex", "TowerActiveNebula", "TowerActiveStardust", "MoonLordCountdown");
		Save(typeof(WorldGen), "spawnEye", "spawnHardBoss", "spawnMeteor");
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var (field, value) in savedFields)
		{
			field.SetValue(null, value);
		}
		savedFields.Clear();
	}

	[TestMethod]
	[DataRow(false, NetmodeID.SinglePlayer)]
	[DataRow(true, NetmodeID.Server)]
	public void EventEntrypointsAreBlockedOnClientAndDedicatedServer(bool dedicatedServer, int netMode)
	{
		SetWorld(1);
		Set(typeof(Main), nameof(Main.dedServ), dedicatedServer);
		Set(typeof(Main), nameof(Main.netMode), netMode);
		foreach (string field in new[] { nameof(Main.raining), nameof(Main.slimeRain), nameof(Main.pumpkinMoon), nameof(Main.snowMoon) })
		{
			Set(typeof(Main), field, false);
		}
		Set(typeof(Main), nameof(Main.invasionType), 0);
		Set(typeof(BirthdayParty), nameof(BirthdayParty.ManualParty), false);
		Set(typeof(LanternNight), nameof(LanternNight.ManualLanterns), false);
		Set(typeof(DD2Event), nameof(DD2Event.Ongoing), false);
		Set(typeof(Sandstorm), nameof(Sandstorm.Happening), false);
		Set(typeof(NPC), nameof(NPC.LunarApocalypseIsUp), false);
		Set(typeof(NPC), nameof(NPC.MoonLordCountdown), 0);
		var system = CreateSystem();
		system.Load();
		try
		{
			Main.StartRain();
			Main.StartSlimeRain(false);
			Main.startPumpkinMoon();
			Main.startSnowMoon();
			Main.StartInvasion(InvasionID.GoblinArmy);
			BirthdayParty.CheckMorning();
			LanternNight.CheckNight();
			BirthdayParty.ToggleManualParty();
			LanternNight.ToggleManualLanterns();
			DD2Event.StartInvasion();
			DD2Event.SummonCrystalDirect(0, 0, 0);
			Sandstorm.StartSandstorm();
			WorldGen.TriggerLunarApocalypse();
			WorldGen.StartImpendingDoom(100);
			WorldGen.dropMeteor();

			Assert.IsTrue(Main.ShouldNormalEventsBeAbleToStart(), "Vanilla interprets true as stopEvents.");
			Assert.IsFalse(NPC.LunarApocalypseIsUp);
			Assert.AreEqual(0, NPC.MoonLordCountdown);

			Assert.IsFalse(Main.raining);
			Assert.IsFalse(Main.slimeRain);
			Assert.IsFalse(Main.pumpkinMoon);
			Assert.IsFalse(Main.snowMoon);
			Assert.AreEqual(0, Main.invasionType);
			Assert.IsFalse(Main.CanStartInvasion(InvasionID.GoblinArmy));
			Assert.IsFalse(BirthdayParty.ManualParty);
			Assert.IsFalse(LanternNight.ManualLanterns);
			Assert.IsFalse(DD2Event.Ongoing);
			Assert.IsFalse(Sandstorm.Happening);
		}
		finally
		{
			system.Unload();
		}
	}

	[TestMethod]
	[DataRow(0, true)]
	[DataRow(1, false)]
	[DataRow(2, true)]
	public void ClearsActiveAndScheduledEventsOnlyInYggdrasil(int world, bool expectedActive)
	{
		SetWorld(world);
		foreach (string field in new[] { nameof(Main.bloodMoon), nameof(Main.eclipse), nameof(Main.slimeRain), nameof(Main.pumpkinMoon), nameof(Main.snowMoon), nameof(Main.raining), nameof(Main.halloween), nameof(Main.xMas) })
		{
			Set(typeof(Main), field, true);
		}
		Set(typeof(Main), nameof(Main.time), 1234d);
		Set(typeof(Main), nameof(Main.dayTime), true);
		Set(typeof(Main), nameof(Main.invasionType), InvasionID.GoblinArmy);
		Set(typeof(BirthdayParty), nameof(BirthdayParty.ManualParty), true);
		Set(typeof(BirthdayParty), nameof(BirthdayParty.GenuineParty), true);
		Set(typeof(LanternNight), nameof(LanternNight.ManualLanterns), true);
		Set(typeof(LanternNight), nameof(LanternNight.GenuineLanterns), true);
		Set(typeof(LanternNight), nameof(LanternNight.NextNightIsLanternNight), true);
		Set(typeof(Sandstorm), nameof(Sandstorm.Happening), true);
		Set(typeof(Sandstorm), nameof(Sandstorm.TimeLeft), 500d);
		Set(typeof(DD2Event), nameof(DD2Event.Ongoing), true);
		Set(typeof(Main), nameof(Main.windSpeedCurrent), 0.8f);
		Set(typeof(Main), nameof(Main.windSpeedTarget), 0.8f);
		Set(typeof(Star), nameof(Star.starfallBoost), 3f);
		Set(typeof(WorldGen), nameof(WorldGen.spawnMeteor), true);
		Set(typeof(WorldGen), nameof(WorldGen.spawnEye), true);
		Set(typeof(WorldGen), nameof(WorldGen.spawnHardBoss), 2);
		Set(typeof(NPC), nameof(NPC.LunarApocalypseIsUp), true);
		Set(typeof(NPC), nameof(NPC.MoonLordCountdown), 500);
		var system = CreateSystem();
		system.PreUpdateEntities();
		system.PostUpdateTime();

		Assert.AreEqual(expectedActive, Main.bloodMoon);
		Assert.AreEqual(expectedActive, Main.eclipse);
		Assert.AreEqual(expectedActive, Main.slimeRain);
		Assert.AreEqual(expectedActive, Main.pumpkinMoon);
		Assert.AreEqual(expectedActive, Main.snowMoon);
		Assert.AreEqual(expectedActive, Main.raining);
		Assert.AreEqual(expectedActive, Main.halloween);
		Assert.AreEqual(expectedActive, Main.xMas);
		Assert.AreEqual(expectedActive, BirthdayParty.ManualParty);
		Assert.AreEqual(expectedActive, BirthdayParty.GenuineParty);
		Assert.AreEqual(expectedActive, LanternNight.ManualLanterns);
		Assert.AreEqual(expectedActive, LanternNight.GenuineLanterns);
		Assert.AreEqual(expectedActive, LanternNight.NextNightIsLanternNight);
		Assert.AreEqual(expectedActive, Sandstorm.Happening);
		Assert.AreEqual(expectedActive ? 500d : 0d, Sandstorm.TimeLeft);
		Assert.AreEqual(expectedActive, DD2Event.Ongoing);
		Assert.AreEqual(expectedActive ? 0.8f : 0f, Main.windSpeedCurrent);
		Assert.AreEqual(expectedActive ? 0.8f : 0f, Main.windSpeedTarget);
		Assert.AreEqual(expectedActive ? 3f : 1f, Star.starfallBoost);
		Assert.AreEqual(expectedActive, WorldGen.spawnMeteor);
		Assert.AreEqual(expectedActive, WorldGen.spawnEye);
		Assert.AreEqual(expectedActive ? 2 : 0, WorldGen.spawnHardBoss);
		Assert.AreEqual(expectedActive, NPC.LunarApocalypseIsUp);
		Assert.AreEqual(expectedActive ? 500 : 0, NPC.MoonLordCountdown);
		Assert.AreEqual(expectedActive ? InvasionID.GoblinArmy : 0, Main.invasionType);
		Assert.AreEqual(1234d, Main.time, "Event suppression must preserve the day/night clock.");
		Assert.IsTrue(Main.dayTime);
	}

	[TestMethod]
	[DataRow(ItemID.BloodMoonStarter, false)]
	[DataRow(ItemID.SolarTablet, false)]
	[DataRow(ItemID.GoblinBattleStandard, false)]
	[DataRow(ItemID.PirateMap, false)]
	[DataRow(ItemID.SnowGlobe, false)]
	[DataRow(ItemID.PumpkinMoonMedallion, false)]
	[DataRow(ItemID.NaughtyPresent, false)]
	[DataRow(ItemID.DD2ElderCrystal, false)]
	[DataRow(ItemID.CelestialSigil, false)]
	[DataRow(ItemID.SuspiciousLookingEye, true)]
	[DataRow(ItemID.MechanicalEye, true)]
	public void EventSummoningItemsCannotBeUsedButNormalBossItemsCan(int itemType, bool allowed)
	{
		var globalItem = new YggdrasilWorldGlobalItem();
		var item = new Item { type = itemType, stack = 1 };
		SetWorld(1);
		Assert.AreEqual(allowed, globalItem.CanUseItem(item, null!));
		Assert.AreEqual(allowed, globalItem.ConsumeItem(item, null!), "Crystal stands consume directly before summoning.");
		SetWorld(0);
		Assert.IsTrue(globalItem.CanUseItem(item, null!));
		Assert.IsTrue(globalItem.ConsumeItem(item, null!));
		SetWorld(2);
		Assert.IsTrue(globalItem.CanUseItem(item, null!));
		Assert.IsTrue(globalItem.ConsumeItem(item, null!));
	}

	[TestMethod]
	[DataRow(0, false, true)]
	[DataRow(1, false, false)]
	[DataRow(1, true, true)]
	[DataRow(2, false, true)]
	public void ClearsInheritedEventSkiesOnlyOnTheYggdrasilClient(int world, bool dedicatedServer, bool expectedActive)
	{
		SetWorld(world);
		Set(typeof(Main), nameof(Main.dedServ), dedicatedServer);
		var skies = new SkyManager();
		Set(typeof(SkyManager), nameof(SkyManager.Instance), skies);
		var party = new PartySky();
		var lanterns = new LanternSky();
		var sandstorm = new SandstormSky();
		var slimes = new SlimeSky();
		typeof(PartySky).GetField("_active", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(party, true);
		typeof(LanternSky).GetField("_active", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(lanterns, true);
		typeof(SandstormSky).GetField("_isActive", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(sandstorm, true);
		typeof(SlimeSky).GetField("_isActive", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(slimes, true);
		skies["Party"] = party;
		skies["Lantern"] = lanterns;
		skies["Sandstorm"] = sandstorm;
		skies["Slime"] = slimes;

		CreateSystem().PreUpdateEntities();

		Assert.AreEqual(expectedActive, party.IsActive());
		Assert.AreEqual(expectedActive, lanterns.IsActive());
		Assert.AreEqual(expectedActive, sandstorm.IsActive());
		Assert.AreEqual(expectedActive, slimes.IsActive());
	}

	[TestMethod]
	public void DedicatedServerDoesNotAdvanceQueuedOldOnesArmyWaves()
	{
		SetWorld(1);
		Set(typeof(Main), nameof(Main.dedServ), true);
		Set(typeof(Main), nameof(Main.netMode), NetmodeID.Server);
		Set(typeof(Main), nameof(Main.npc), Enumerable.Range(0, Main.maxNPCs + 1).Select(i => new NPC { whoAmI = i }).ToArray());
		Set(typeof(DD2Event), nameof(DD2Event.Ongoing), false);
		Set(typeof(DD2Event), "_timeLeftUntilSpawningBegins", 2);
		var system = CreateSystem();
		system.Load();
		try
		{
			DD2Event.UpdateTime();

			var timer = typeof(DD2Event).GetField("_timeLeftUntilSpawningBegins", BindingFlags.Static | BindingFlags.NonPublic)!;
			Assert.AreEqual(2, (int)timer.GetValue(null)!, "Even an inactive army advances its queued wave on a vanilla dedicated server.");
		}
		finally
		{
			system.Unload();
		}
	}

	[TestMethod]
	[DataRow(1, NetmodeID.Server, -1, true)]
	[DataRow(1, NetmodeID.Server, -2, true)]
	[DataRow(1, NetmodeID.Server, -3, true)]
	[DataRow(1, NetmodeID.Server, -4, true)]
	[DataRow(1, NetmodeID.Server, -5, true)]
	[DataRow(1, NetmodeID.Server, -6, true)]
	[DataRow(1, NetmodeID.Server, -7, true)]
	[DataRow(1, NetmodeID.Server, -8, true)]
	[DataRow(1, NetmodeID.Server, -9, true)]
	[DataRow(1, NetmodeID.Server, -10, true)]
	[DataRow(1, NetmodeID.Server, -19, true)]
	[DataRow(1, NetmodeID.Server, short.MinValue, true)]
	[DataRow(1, NetmodeID.Server, NPCID.EyeofCthulhu, false)]
	[DataRow(1, NetmodeID.Server, -11, false)]
	[DataRow(1, NetmodeID.Server, -12, false)]
	[DataRow(1, NetmodeID.Server, -13, false)]
	[DataRow(1, NetmodeID.Server, -14, false)]
	[DataRow(1, NetmodeID.Server, -15, false)]
	[DataRow(1, NetmodeID.Server, -16, false)]
	[DataRow(1, NetmodeID.Server, -17, false)]
	[DataRow(1, NetmodeID.Server, -18, false)]
	[DataRow(0, NetmodeID.Server, -6, false)]
	[DataRow(2, NetmodeID.Server, -10, false)]
	[DataRow(1, NetmodeID.MultiplayerClient, -10, false)]
	public void ServerRejectsEventSummonPacketsWithoutConsumingTheReader(int world, int netMode, int summon, bool rejected)
	{
		SetWorld(world);
		Set(typeof(Main), nameof(Main.netMode), netMode);
		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			writer.Write(new byte[] { 7, 8, 9 });
			writer.Write((short)0);
			writer.Write((short)summon);
			writer.Write((byte)42);
		}
		stream.Position = 3;
		var reader = new BinaryReader(stream);
		using (reader)
		{
			byte message = MessageID.SpawnBossUseLicenseStartEvent;
			Assert.AreEqual(rejected, CreateSystem().HijackGetData(ref message, ref reader, 0));
			Assert.AreEqual(3L, stream.Position, "Later packet handlers must see the same payload position.");
			Assert.AreEqual((short)0, reader.ReadInt16());
			Assert.AreEqual((short)summon, reader.ReadInt16());
			Assert.AreEqual((byte)42, reader.ReadByte());
		}
	}

	private static ModSystem CreateSystem() => new YggdrasilWorldSystem();

	private void SetWorld(int world)
	{
		Set(typeof(SubworldSystem), "current", world switch { 1 => new YggdrasilWorld(), 2 => new RoomWorld(), _ => null });
	}

	private void Save(Type type, params string[] names)
	{
		foreach (string name in names)
		{
			var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!;
			savedFields.TryAdd(field, field.GetValue(null));
		}
	}

	private void Set(Type type, string name, object? value)
	{
		var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!;
		savedFields.TryAdd(field, field.GetValue(null));
		field.SetValue(null, value);
	}
}
