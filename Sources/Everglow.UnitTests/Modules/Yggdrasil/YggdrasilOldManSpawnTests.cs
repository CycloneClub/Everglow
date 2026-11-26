using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.SubSpace;
using Everglow.Yggdrasil;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class YggdrasilOldManSpawnTests
{
	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
	}

	[TestMethod]
	[DataRow(0, false, NetmodeID.SinglePlayer, 1)]
	[DataRow(1, false, NetmodeID.SinglePlayer, 0)]
	[DataRow(2, false, NetmodeID.SinglePlayer, 1)]
	[DataRow(3, false, NetmodeID.SinglePlayer, 0)]
	[DataRow(0, true, NetmodeID.SinglePlayer, 0)]
	[DataRow(1, true, NetmodeID.SinglePlayer, 0)]
	[DataRow(1, false, NetmodeID.Server, 0)]
	public void TownSpawnSuppressesVanillaArrivalsOnlyInYggdrasil(int world, bool downedSkeletron, int netMode, int expectedOldMen)
	{
		var currentWorld = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
		var spawnCounter = typeof(Main).GetField("checkForSpawns", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
		var spawn = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.Static | BindingFlags.NonPublic)!;
		object? oldWorld = currentWorld.GetValue(null);
		var oldOrigin = RoomWorld.OriginalWorld;
		object? oldCounter = spawnCounter.GetValue(null);
		var oldNpcs = Main.npc;
		var oldPlayers = Main.player;
		var oldCanSpawn = Main.townNPCCanSpawn;
		var oldBestiary = Main.BestiaryDB;
		var oldRandom = Main.rand;
		int oldNetMode = Main.netMode;
		bool oldDedServ = Main.dedServ;
		bool oldDowned = NPC.downedBoss3;
		int oldDungeonX = Main.dungeonX;
		int oldDungeonY = Main.dungeonY;
		int oldPriority = WorldGen.prioritizedTownNPCType;
		double oldRate = Main.desiredWorldTilesUpdateRate;
		CreativePowerManager.Initialize();
		var freezeTime = CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>();
		bool frozen = freezeTime.Enabled;
		var system = new YggdrasilWorldSystem();
		system.Load();
		try
		{
			Assert.IsFalse(frozen, "The spawn test requires world updates to be enabled.");
			currentWorld.SetValue(null, world switch { 1 => new YggdrasilWorld(), 2 or 3 => new RoomWorld(), _ => null });
			RoomWorld.OriginalWorld = world == 3 ? new YggdrasilWorld() : null;
			Main.npc = Enumerable.Range(0, Main.maxNPCs + 1).Select(i => new NPC { whoAmI = i }).ToArray();
			Main.player = Enumerable.Range(0, Main.maxPlayers + 1).Select(_ => (Player)RuntimeHelpers.GetUninitializedObject(typeof(Player))).ToArray();
			Main.townNPCCanSpawn = new bool[oldCanSpawn.Length];
			Main.townNPCCanSpawn[NPCID.Guide] = true;
			Main.BestiaryDB = new BestiaryDatabase();
			Main.rand = new UnifiedRandom(1);
			Main.netMode = netMode;
			Main.dedServ = true;
			if (world is 2 or 3)
			{
				system.OnWorldLoad();
				Assert.AreEqual(world == 2, Main.townNPCCanSpawn[NPCID.Guide]);
			}
			Main.desiredWorldTilesUpdateRate = 1;
			Main.dungeonX = 100;
			Main.dungeonY = 100;
			NPC.downedBoss3 = downedSkeletron;
			spawnCounter.SetValue(null, 7200);

			spawn.Invoke(null, null);

			Assert.AreEqual(expectedOldMen, Main.npc.Count(npc => npc.active && npc.type == NPCID.OldMan));
			if (world is 1 or 3)
			{
				Assert.IsFalse(Main.townNPCCanSpawn.Take(NPCID.Count).Any(eligible => eligible));
				Assert.AreEqual(0, WorldGen.prioritizedTownNPCType);
			}
			else
			{
				Assert.IsTrue(Main.townNPCCanSpawn[NPCID.Guide]);
			}
			Assert.AreEqual(downedSkeletron, NPC.downedBoss3, "The hook must not modify boss progression.");
		}
		finally
		{
			system.Unload();
			currentWorld.SetValue(null, oldWorld);
			RoomWorld.OriginalWorld = oldOrigin;
			spawnCounter.SetValue(null, oldCounter);
			Main.npc = oldNpcs;
			Main.player = oldPlayers;
			Main.townNPCCanSpawn = oldCanSpawn;
			Main.BestiaryDB = oldBestiary;
			Main.rand = oldRandom;
			Main.netMode = oldNetMode;
			Main.dedServ = oldDedServ;
			Main.desiredWorldTilesUpdateRate = oldRate;
			Main.dungeonX = oldDungeonX;
			Main.dungeonY = oldDungeonY;
			WorldGen.prioritizedTownNPCType = oldPriority;
			NPC.downedBoss3 = oldDowned;
		}
	}
}
