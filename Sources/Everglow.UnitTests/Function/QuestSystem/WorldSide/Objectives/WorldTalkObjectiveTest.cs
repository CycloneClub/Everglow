using Terraria.Localization;
using System.Reflection;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldTalkObjectiveTest
{
	private LanguageManager oldLanguage = null!;

	private Player[] oldPlayers = null!;
	private NPC[] oldNpcs = null!;
	private int oldPlayerIndex;
	private int oldMode;
	private string oldChat = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		oldLanguage = LanguageManager.Instance;
		LanguageManager.Instance = (LanguageManager)Activator.CreateInstance(typeof(LanguageManager), true)!;
		Language.GetOrRegister("Tests.WorldTalk.NPCText", () => "A quest for you.");
		oldPlayers = Main.player;
		oldNpcs = Main.npc;
		oldPlayerIndex = Main.myPlayer;
		oldMode = Main.netMode;
		oldChat = Main.npcChatText;
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
		LanguageManager.Instance = oldLanguage;
		Main.player = oldPlayers;
		Main.npc = oldNpcs;
		Main.myPlayer = oldPlayerIndex;
		Main.netMode = oldMode;
		Main.npcChatText = oldChat;
	}

	private static void TalkTo(int index) => typeof(Player).GetProperty(nameof(Player.talkNPC))!.SetValue(Main.LocalPlayer, index);

	[TestMethod]
	[DataRow(NetmodeID.SinglePlayer)]
	[DataRow(NetmodeID.MultiplayerClient)]
	public void MatchingLocalConversationShowsTextWithoutClientCompletingWorld(int mode)
	{
		Main.netMode = mode;
		var objective = new WorldTalkObjective(NPCID.Guide) { LocalizationKey = "Tests.WorldTalk" };
		TalkTo(0);
		objective.Update();
		Assert.AreEqual("A quest for you.", Main.npcChatText);
		Assert.AreEqual(mode == NetmodeID.SinglePlayer, objective.Talked);
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(1)]
	public void AnotherPlayersConversationDoesNotReplaceLocalDialogue(int localNpc)
	{
		TalkTo(localNpc);
		Main.player[1].active = true;
		typeof(Player).GetProperty(nameof(Player.talkNPC))!.SetValue(Main.player[1], 0);
		var objective = new WorldTalkObjective(NPCID.Guide) { LocalizationKey = "Tests.WorldTalk" };
		objective.Update();
		Assert.IsTrue(objective.Talked);
		Assert.AreEqual("Original dialogue", Main.npcChatText);
	}

	[TestMethod]
	public void UnconfiguredOrServerObjectiveDoesNotReplaceDialogue()
	{
		TalkTo(0);
		new WorldTalkObjective(NPCID.Guide).Update();
		Assert.AreEqual("Original dialogue", Main.npcChatText);
		Main.netMode = NetmodeID.Server;
		new WorldTalkObjective(NPCID.Guide) { LocalizationKey = "Tests.WorldTalk" }.Update();
		Assert.AreEqual("Original dialogue", Main.npcChatText);
	}

	[TestMethod]
	public void CompletionSnapshotDoesNotReplaceDialogue()
	{
		var sender = new WorldTalkObjective(NPCID.Guide) { LocalizationKey = "Tests.WorldTalk" };
		TalkTo(0);
		sender.Update();
		sender.Complete();
		var receiver = new WorldTalkObjective(NPCID.Guide) { LocalizationKey = "Tests.WorldTalk" };
		Main.netMode = NetmodeID.MultiplayerClient;
		Main.npcChatText = "Original dialogue";
		using var stream = new MemoryStream();
		sender.NetSend(new BinaryWriter(stream));
		stream.Position = 0;
		receiver.NetReceive(new BinaryReader(stream));
		Assert.IsTrue(receiver.Talked);
		Assert.AreEqual("Original dialogue", Main.npcChatText);
	}
}
