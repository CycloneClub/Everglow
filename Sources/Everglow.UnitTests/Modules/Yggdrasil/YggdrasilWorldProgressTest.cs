using System.Reflection;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.YggdrasilTown.Quests;
using Terraria.ModLoader.IO;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.SquamousShell;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.KingJellyBall;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class YggdrasilWorldProgressTest
{
	private static readonly FieldInfo CurrentWorld = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
	private static readonly FieldInfo CopiedData = typeof(SubworldSystem).GetField("copiedData", BindingFlags.Static | BindingFlags.NonPublic)!;
	private object? oldCopiedData;
	private int oldMode;
	private bool oldDedServ;
	private Subworld? oldWorld;
	private bool oldDowned;
	private bool oldKing;
	private bool oldKelp;
	private bool registeredBoolSerializer;
	private readonly YggdrasilWorldSystem system = new();

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		if (!TagSerializer.TryGetSerializer(typeof(bool), out _))
		{
			typeof(TagSerializer).GetMethod("AddSerializer", BindingFlags.Static | BindingFlags.NonPublic)!
				.Invoke(null, [new BoolTagSerializer()]);
			registeredBoolSerializer = true;
		}
		oldCopiedData = CopiedData.GetValue(null);
		CopiedData.SetValue(null, new TagCompound());
		oldMode = Main.netMode;
		oldDedServ = Main.dedServ;
		oldWorld = (Subworld?)CurrentWorld.GetValue(null);
		oldDowned = YggdrasilWorldSystem.DownedSquamousShell;
		oldKing = YggdrasilWorldSystem.DownedKingJellyBall;
		oldKelp = YggdrasilWorldSystem.EnteredKelpCurtain;
		Main.netMode = NetmodeID.SinglePlayer;
		Main.dedServ = true;
		CurrentWorld.SetValue(null, new YggdrasilWorld());
		system.ClearWorld();
	}

	[TestCleanup]
	public void Cleanup()
	{
		if (registeredBoolSerializer)
		{
			var serializers = (IDictionary<Type, TagSerializer>)typeof(TagSerializer)
				.GetField("serializers", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
			serializers.Remove(typeof(bool));
		}
		system.ClearWorld();
		YggdrasilWorldSystem.DownedSquamousShell = oldDowned;
		YggdrasilWorldSystem.DownedKingJellyBall = oldKing;
		YggdrasilWorldSystem.EnteredKelpCurtain = oldKelp;
		CopiedData.SetValue(null, oldCopiedData);
		Main.netMode = oldMode;
		Main.dedServ = oldDedServ;
		CurrentWorld.SetValue(null, oldWorld);
	}

	[TestMethod]
	public void ClientKillCannotRecordGlobalDefeat()
	{
		Main.netMode = NetmodeID.MultiplayerClient;
		new SquamousShell().OnKill();
		Assert.IsFalse(YggdrasilWorldSystem.DownedSquamousShell);
	}

	[TestMethod]
	public void SinglePlayerKillRecordsGlobalDefeat()
	{
		new SquamousShell().OnKill();
		Assert.IsTrue(YggdrasilWorldSystem.DownedSquamousShell);
		Assert.IsTrue(new DefendTownQuest().CanOffer(null!));
	}

	[TestMethod]
	public void SinglePlayerProgressSurvivesWorldTransition()
	{
		new SquamousShell().OnKill();
		var copy = (ICopyWorldData)system;
		copy.CopyMainWorldData();
		system.ClearWorld();
		CurrentWorld.SetValue(null, null);
		copy.ReadCopiedMainWorldData();
		Assert.IsTrue(YggdrasilWorldSystem.DownedSquamousShell);

		copy.CopyMainWorldData();
		system.ClearWorld();
		CurrentWorld.SetValue(null, new YggdrasilWorld());
		copy.ReadCopiedMainWorldData();
		Assert.IsTrue(YggdrasilWorldSystem.DownedSquamousShell);
	}

	[TestMethod]
	public void MainServerRejectsPlayerReport()
	{
		Main.netMode = NetmodeID.Server;
		CurrentWorld.SetValue(null, null);
		var handler = new YggdrasilProgressSyncPacket.YggdrasilProgressSyncPacketHandler();
		handler.Handle(RoundTrip(new YggdrasilProgressSyncPacket(true)), 3);
		Assert.IsFalse(YggdrasilWorldSystem.DownedSquamousShell);
	}

	[TestMethod]
	public void DownstreamPacketAppliesOnlyServerStateAndDoesNotRollbackProgress()
	{
		Main.netMode = NetmodeID.MultiplayerClient;
		var handler = new YggdrasilProgressSyncPacket.YggdrasilProgressSyncPacketHandler();
		var packet = RoundTrip(new YggdrasilProgressSyncPacket(true));
		handler.Handle(packet, 2);
		Assert.IsFalse(YggdrasilWorldSystem.DownedSquamousShell);
		handler.Handle(packet, -1);
		Assert.IsTrue(YggdrasilWorldSystem.DownedSquamousShell);
		handler.Handle(RoundTrip(new YggdrasilProgressSyncPacket(false)), -1);
		Assert.IsTrue(YggdrasilWorldSystem.DownedSquamousShell);
	}

	[TestMethod]
	public void SubserverReceivesMainProgressWithoutKeepingItsOwnSaveRecord()
	{
		Main.netMode = NetmodeID.Server;
		var handler = new YggdrasilProgressSyncPacket.YggdrasilProgressSyncPacketHandler();
		handler.Handle(RoundTrip(new YggdrasilProgressSyncPacket(true)), -1);
		Assert.IsTrue(YggdrasilWorldSystem.DownedSquamousShell);
		var tag = new TagCompound();
		system.SaveWorldData(tag);
		Assert.IsFalse(tag.ContainsKey(nameof(YggdrasilWorldSystem.DownedSquamousShell)));
	}

	[TestMethod]
	public void LateJoinReceivesProgressAndFreshWorldClearsIt()
	{
		new SquamousShell().OnKill();
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		system.NetSend(writer);
		system.ClearWorld();
		Main.netMode = NetmodeID.MultiplayerClient;
		stream.Position = 0;
		system.NetReceive(new BinaryReader(stream));
		Assert.IsTrue(YggdrasilWorldSystem.DownedSquamousShell);
		system.ClearWorld();
		Assert.IsFalse(YggdrasilWorldSystem.DownedSquamousShell);
	}

	[TestMethod]
	public void KingDefeatIsAuthoritativeAndUnlocksCookingQuest()
	{
		Main.netMode = NetmodeID.MultiplayerClient;
		new KingJellyBall().OnKill();
		Assert.IsFalse(YggdrasilWorldSystem.DownedKingJellyBall);
		Main.netMode = NetmodeID.SinglePlayer;
		new KingJellyBall().OnKill();
		Assert.IsTrue(YggdrasilWorldSystem.DownedKingJellyBall);
		Assert.IsTrue(new SelfRelianceQuest().CanOffer(null!));
	}

	[TestMethod]
	public void NewProgressSurvivesSaveWorldTransferAndLateJoin()
	{
		YggdrasilWorldSystem.DownedKingJellyBall = true;
		YggdrasilWorldSystem.EnteredKelpCurtain = true;
		var tag = new TagCompound();
		system.SaveWorldData(tag);
		system.ClearWorld();
		system.LoadWorldData(tag);
		AssertNewProgress();
		var copy = (ICopyWorldData)system;
		copy.CopyMainWorldData();
		system.ClearWorld();
		CurrentWorld.SetValue(null, null);
		copy.ReadCopiedMainWorldData();
		AssertNewProgress();
		using var stream = new MemoryStream();
		system.NetSend(new BinaryWriter(stream));
		system.ClearWorld();
		stream.Position = 0;
		system.NetReceive(new BinaryReader(stream));
		AssertNewProgress();
		system.ClearWorld();
		Assert.IsFalse(YggdrasilWorldSystem.DownedKingJellyBall);
		Assert.IsFalse(YggdrasilWorldSystem.EnteredKelpCurtain);
	}

	[TestMethod]
	public void NewProgressRejectsClientReportsAndMergesServerSnapshots()
	{
		Main.netMode = NetmodeID.MultiplayerClient;
		var handler = new YggdrasilProgressSyncPacket.YggdrasilProgressSyncPacketHandler();
		var packet = RoundTrip(new YggdrasilProgressSyncPacket(false, true, true));
		handler.Handle(packet, 2);
		Assert.IsFalse(YggdrasilWorldSystem.DownedKingJellyBall);
		Assert.IsFalse(YggdrasilWorldSystem.EnteredKelpCurtain);
		handler.Handle(packet, -1);
		handler.Handle(RoundTrip(new YggdrasilProgressSyncPacket(false)), -1);
		AssertNewProgress();
	}

	[TestMethod]
	public void KelpEntryUsesPlayerPositionAndIgnoresClientOrMainWorld()
	{
		var oldPlayers = Main.player;
		int oldMaxTilesY = Main.maxTilesY;
		var oldBounds = KelpCurtainBiome.StratumBoundCurve;
		var oldCamera = Main.screenPosition;
		float oldTimer = YggdrasilWorld.YggdrasilTimer;
		bool oldBloodMoon = Main.bloodMoon;
		bool oldSlimeRain = Main.slimeRain;
		try
		{
			Main.player = Enumerable.Range(0, Main.maxPlayers + 1).Select(_ => new Player()).ToArray();
			Main.player[0].active = true;
			Main.maxTilesY = 21000;
			KelpCurtainBiome.StratumBoundCurve = [];
			Main.screenPosition = Vector2.Zero;
			Main.player[0].Center = new Vector2(1000, 21000 * 0.8f * 16);
			Main.netMode = NetmodeID.MultiplayerClient;
			system.PostUpdateEverything();
			Assert.IsFalse(YggdrasilWorldSystem.EnteredKelpCurtain);
			Main.netMode = NetmodeID.SinglePlayer;
			CurrentWorld.SetValue(null, null);
			system.PostUpdateEverything();
			Assert.IsFalse(YggdrasilWorldSystem.EnteredKelpCurtain);
			CurrentWorld.SetValue(null, new YggdrasilWorld());
			system.PostUpdateEverything();
			Assert.IsTrue(YggdrasilWorldSystem.EnteredKelpCurtain);
			Main.player[0].Center = Vector2.Zero;
			system.PostUpdateEverything();
			Assert.IsTrue(YggdrasilWorldSystem.EnteredKelpCurtain);
		}
		finally
		{
			Main.player = oldPlayers;
			Main.maxTilesY = oldMaxTilesY;
			KelpCurtainBiome.StratumBoundCurve = oldBounds;
			Main.screenPosition = oldCamera;
			YggdrasilWorld.YggdrasilTimer = oldTimer;
			Main.bloodMoon = oldBloodMoon;
			Main.slimeRain = oldSlimeRain;
		}
	}

	private static void AssertNewProgress()
	{
		Assert.IsFalse(YggdrasilWorldSystem.DownedSquamousShell);
		Assert.IsTrue(YggdrasilWorldSystem.DownedKingJellyBall);
		Assert.IsTrue(YggdrasilWorldSystem.EnteredKelpCurtain);
	}

	private static YggdrasilProgressSyncPacket RoundTrip(YggdrasilProgressSyncPacket source)
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		source.Send(writer);
		stream.Position = 0;
		var packet = new YggdrasilProgressSyncPacket();
		packet.Receive(new BinaryReader(stream), -1);
		return packet;
	}
}
