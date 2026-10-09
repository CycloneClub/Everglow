using System.Reflection;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Everglow.Commons.Mechanics.Quest.WorldSide;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldGiveObjectiveTest
{
	private Player[] oldPlayers = null!;
	private NPC[] oldNpcs = null!;
	private int oldPlayerIndex;
	private int oldMode;
	private string oldChat = null!;
	private WorldQuestSystem oldSystem = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		oldPlayers = Main.player;
		oldNpcs = Main.npc;
		oldPlayerIndex = Main.myPlayer;
		oldMode = Main.netMode;
		oldChat = Main.npcChatText;
		oldSystem = ContentInstance<WorldQuestSystem>.Instance;
		var system = new WorldQuestSystem();
		typeof(WorldQuestSystem).GetProperty(nameof(WorldQuestSystem.Manager))!.SetValue(system, new WorldQuestManager());
		typeof(ContentInstance<WorldQuestSystem>).GetProperty(nameof(ContentInstance<WorldQuestSystem>.Instance))!.SetValue(null, system);
		Main.player = Enumerable.Range(0, Main.maxPlayers + 1).Select(_ => new Player()).ToArray();
		Main.npc = new NPC[Main.maxNPCs + 1];
		Main.npc[0] = new NPC { type = NPCID.Guide, netID = NPCID.Guide, active = true };
		Main.npc[1] = new NPC { type = NPCID.Merchant, netID = NPCID.Merchant, active = true };
		Main.myPlayer = 0;
		Main.LocalPlayer.active = true;
		Main.netMode = NetmodeID.SinglePlayer;
		Main.npcChatText = "Original dialogue";
		TalkTo(-1);
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.player = oldPlayers;
		Main.npc = oldNpcs;
		Main.myPlayer = oldPlayerIndex;
		Main.netMode = oldMode;
		Main.npcChatText = oldChat;
		typeof(ContentInstance<WorldQuestSystem>).GetProperty(nameof(ContentInstance<WorldQuestSystem>.Instance))!.SetValue(null, oldSystem);
	}

	private static void TalkTo(int index) => typeof(Player).GetProperty(nameof(Player.talkNPC))!.SetValue(Main.LocalPlayer, index);

	[TestMethod]
	[DataRow(NetmodeID.SinglePlayer)]
	[DataRow(NetmodeID.MultiplayerClient)]
	public void DeliveryShowsRequestThenThanksAndConsumesOnlyOnce(int mode)
	{
		Main.netMode = mode;
		var objective = new WorldGiveObjective(NPCID.Guide, ItemID.Ale, 5, "Bring five ales.", "Thank you.");
		TalkTo(0);
		Main.LocalPlayer.inventory[0] = new Item { type = ItemID.Ale, stack = 4 };
		objective.Update();
		Assert.AreEqual("Bring five ales.", Main.npcChatText);
		Assert.AreEqual(4, Main.LocalPlayer.inventory[0].stack);

		Main.LocalPlayer.inventory[0].stack = 10;
		objective.Update();
		Assert.AreEqual("Thank you.", Main.npcChatText);
		Assert.AreEqual(5, Main.LocalPlayer.inventory[0].stack);
		Assert.AreEqual(mode == NetmodeID.SinglePlayer, objective.Given);
		objective.Update();
		Assert.AreEqual("Thank you.", Main.npcChatText);
		Assert.AreEqual(5, Main.LocalPlayer.inventory[0].stack);
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(1)]
	public void WrongNpcDoesNotChangeDialogueOrConsumeItems(int npcIndex)
	{
		TalkTo(npcIndex);
		Main.LocalPlayer.inventory[0] = new Item { type = ItemID.Ale, stack = 5 };
		new WorldGiveObjective(NPCID.Guide, ItemID.Ale, 5, "Bring five ales.", "Thank you.").Update();
		Assert.AreEqual("Original dialogue", Main.npcChatText);
		Assert.AreEqual(5, Main.LocalPlayer.inventory[0].stack);
	}

	[TestMethod]
	public void UnconfiguredOrServerObjectiveDoesNotReplaceDialogue()
	{
		TalkTo(0);
		new WorldGiveObjective(NPCID.Guide, ItemID.Ale, 5).Update();
		Assert.AreEqual("Original dialogue", Main.npcChatText);
		Main.netMode = NetmodeID.Server;
		Main.LocalPlayer.inventory[0] = new Item { type = ItemID.Ale, stack = 5 };
		new WorldGiveObjective(NPCID.Guide, ItemID.Ale, 5, "Bring five ales.", "Thank you.").Update();
		Assert.AreEqual("Original dialogue", Main.npcChatText);
		Assert.AreEqual(5, Main.LocalPlayer.inventory[0].stack);
	}

	[TestMethod]
	public void RemoteCompletionDoesNotThankNonContributorOrConsumeTheirItems()
	{
		Main.netMode = NetmodeID.MultiplayerClient;
		TalkTo(0);
		Main.LocalPlayer.inventory[0] = new Item { type = ItemID.Ale, stack = 5 };
		var objective = new WorldGiveObjective(NPCID.Guide, ItemID.Ale, 5, "Bring five ales.", "Thank you.");
		using var stream = new MemoryStream();
		new BinaryWriter(stream).Write(true);
		stream.Position = 0;
		objective.ReceiveMain(new BinaryReader(stream));
		objective.Update();
		Assert.IsTrue(objective.Given);
		Assert.AreEqual("Original dialogue", Main.npcChatText);
		Assert.AreEqual(5, Main.LocalPlayer.inventory[0].stack);
	}
}
