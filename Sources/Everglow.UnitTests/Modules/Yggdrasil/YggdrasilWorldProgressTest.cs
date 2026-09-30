using System.Reflection;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.YggdrasilTown.Quests;
using Terraria.ModLoader.IO;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.SquamousShell;
using SubworldLibrary;
using Terraria;
using Terraria.ID;

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
	private readonly YggdrasilWorldSystem system = new();

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		oldCopiedData = CopiedData.GetValue(null);
		CopiedData.SetValue(null, new TagCompound());
		oldMode = Main.netMode;
		oldDedServ = Main.dedServ;
		oldWorld = (Subworld?)CurrentWorld.GetValue(null);
		oldDowned = YggdrasilWorldSystem.DownedSquamousShell;
		Main.netMode = NetmodeID.SinglePlayer;
		Main.dedServ = true;
		CurrentWorld.SetValue(null, new YggdrasilWorld());
		system.ClearWorld();
	}

	[TestCleanup]
	public void Cleanup()
	{
		using var stream = new MemoryStream([oldDowned ? (byte)1 : (byte)0]);
		system.ClearWorld();
		system.NetReceive(new BinaryReader(stream));
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
