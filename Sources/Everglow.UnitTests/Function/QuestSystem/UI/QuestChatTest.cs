using System.Reflection;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using QuestHooks = Everglow.Commons.Mechanics.Quest.Hooks;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class QuestChatTest
{
	private Player[] oldPlayers = null!;
	private NPC[] oldNpcs = null!;
	private LanguageManager oldLanguage = null!;
	private WorldQuestSystem oldWorld = null!;
	private PlayerQuestSystem oldPlayer = null!;
	private int oldMode;
	private int oldPlayerIndex;
	private int oldShop;
	private bool oldMenu;
	private bool oldServer;
	private string oldChat = null!;
	private WorldQuestManager world = null!;
	private PlayerQuestManager player = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		QuestChat.CloseChat();
		oldPlayers = Main.player;
		oldNpcs = Main.npc;
		oldMode = Main.netMode;
		oldPlayerIndex = Main.myPlayer;
		oldShop = Main.npcShop;
		oldMenu = Main.gameMenu;
		oldServer = Main.dedServ;
		oldChat = Main.npcChatText;
		oldLanguage = LanguageManager.Instance;
		oldWorld = ContentInstance<WorldQuestSystem>.Instance;
		oldPlayer = ContentInstance<PlayerQuestSystem>.Instance;
		LanguageManager.Instance = (LanguageManager)Activator.CreateInstance(typeof(LanguageManager), true)!;
		foreach (string key in new[] { "First", "Last", "Confirm", "Thanks", "Missing", "Next" })
		{
			Language.GetOrRegister("Tests.Dialogue." + key, () => key);
		}
		Main.player = Enumerable.Range(0, Main.maxPlayers + 1).Select(_ => new Player()).ToArray();
		Main.npc = new NPC[Main.maxNPCs + 1];
		Main.npc[0] = new NPC { active = true, type = NPCID.Guide, netID = NPCID.Guide };
		Main.npc[1] = new NPC { active = true, type = NPCID.Merchant, netID = NPCID.Merchant };
		Main.myPlayer = 0;
		Main.LocalPlayer.active = true;
		Main.netMode = NetmodeID.SinglePlayer;
		Main.dedServ = false;
		Main.gameMenu = false;
		Main.npcShop = 0;
		Main.npcChatText = "Original";
		TalkTo(0);
		world = new WorldQuestManager();
		player = new PlayerQuestManager();
		var worldSystem = new WorldQuestSystem();
		typeof(WorldQuestSystem).GetProperty(nameof(WorldQuestSystem.Manager))!.SetValue(worldSystem, world);
		SetSystem(worldSystem);
		var playerSystem = new PlayerQuestSystem();
		typeof(PlayerQuestSystem).GetProperty(nameof(PlayerQuestSystem.Manager))!.SetValue(playerSystem, player);
		SetSystem(playerSystem);
	}

	[TestCleanup]
	public void Cleanup()
	{
		QuestChat.CloseChat();
		SetSystem(oldWorld);
		SetSystem(oldPlayer);
		LanguageManager.Instance = oldLanguage;
		Main.player = oldPlayers;
		Main.npc = oldNpcs;
		Main.myPlayer = oldPlayerIndex;
		Main.netMode = oldMode;
		Main.npcShop = oldShop;
		Main.gameMenu = oldMenu;
		Main.dedServ = oldServer;
		Main.npcChatText = oldChat;
	}

	[TestMethod]
	public void WorldTalkWaitsForFinalButtonAndDoesNotOverwriteReplyWhenNextGoalActivates()
	{
		var talk = new WorldTalkObjective(NPCID.Guide);
		talk.WithDialogue(d => d.WithText("First", "Last").WithConfirmButtonText("Confirm").WithCompletionText("Thanks"));
		var next = new WorldTalkObjective(NPCID.Guide);
		AddWorld(talk, next);
		next.WithDialogue(d => d.WithText("Next"));
		talk.Update();
		Assert.IsFalse(talk.Talked);
		Assert.AreEqual("Original", Main.npcChatText);
		Click();
		Assert.AreEqual("First", Main.npcChatText);
		Click();
		Assert.AreEqual("Last", Main.npcChatText);
		Assert.IsFalse(talk.Talked);
		Click();
		Assert.IsTrue(talk.Talked);
		Assert.AreEqual("Thanks", Main.npcChatText);
		next.Update();
		Assert.AreEqual("Thanks", Main.npcChatText);
		Click();
		Assert.IsFalse(QuestChat.IsChattingWith(Main.npc[0]));
	}

	[TestMethod]
	[DataRow(NetmodeID.SinglePlayer)]
	[DataRow(NetmodeID.MultiplayerClient)]
	public void WorldGiveWaitsForClickConsumesOnceAndClientWaitsForSnapshot(int mode)
	{
		var give = new WorldGiveObjective(NPCID.Guide, ItemID.Wood, 2);
		give.WithDialogue(d => d.WithText("First").WithUnavailableText("Missing").WithCompletionText("Thanks"));
		AddWorld(give);
		Main.netMode = mode;
		give.Update();
		Assert.IsFalse(give.Given);
		Click();
		Click();
		Assert.AreEqual("Missing", Main.npcChatText);
		Main.LocalPlayer.inventory[0] = new Item { type = ItemID.Wood, stack = 4 };
		Click();
		Assert.AreEqual(2, Main.LocalPlayer.inventory[0].stack);
		Assert.AreEqual(mode == NetmodeID.SinglePlayer, give.Given);
		if (mode == NetmodeID.MultiplayerClient)
		{
			Assert.AreNotEqual("Thanks", Main.npcChatText);
			Click();
			using var stream = new MemoryStream([1]);
			give.ReceiveMain(new BinaryReader(stream));
			new QuestHooks.QuestSystem().PostUpdatePlayers();
			Assert.AreEqual("Thanks", Main.npcChatText);
		}
		Click();
		Assert.AreEqual(2, Main.LocalPlayer.inventory[0].stack);
	}

	[TestMethod]
	public void ClosingSwitchingNpcAndUsingShopCancelWithoutCompleting()
	{
		var talk = new WorldTalkObjective(NPCID.Guide);
		talk.WithDialogue(d => d.WithText("First", "Last"));
		AddWorld(talk);
		Click();
		Click();
		TalkTo(-1);
		new QuestHooks.QuestSystem().PostUpdatePlayers();
		Assert.IsFalse(talk.Talked);
		TalkTo(0);
		Click();
		Assert.AreEqual("First", Main.npcChatText);
		TalkTo(1);
		new QuestHooks.QuestSystem().PostUpdatePlayers();
		Assert.IsFalse(QuestChat.IsChattingWith(Main.npc[0]));
		TalkTo(0);
		Click();
		Assert.IsFalse(QuestChat.OnChatButtonClicked(Main.npc[0], firstButton: true));
		Assert.IsFalse(QuestChat.IsChattingWith(Main.npc[0]));
		Assert.IsFalse(talk.Talked);
	}

	[TestMethod]
	public void PlayerTalkAndGiveUseSameEntryAndConsumeOnlyAtConfirmation()
	{
		var talk = new TalkNPCObjective(NPCID.Guide);
		talk.WithDialogue(d => d.WithText("First"));
		var give = new GiveItemObjective([ItemID.Wood], 2, NPCID.Guide);
		give.WithDialogue(d => d.WithText("Last").WithCompletionText("Thanks"));
		var quest = new LocalQuest();
		quest.Objectives.Add(talk).Add(give);
		player.AddQuest(quest, PlayerQuestState.Accepted, showText: false);
		Assert.IsFalse(talk.CheckCompletion());
		Click();
		Click();
		Assert.IsTrue(talk.CheckCompletion());
		quest.Objectives.Update(quest);
		Main.LocalPlayer.inventory[0] = new Item { type = ItemID.Wood, stack = 4 };
		give.Update();
		Assert.IsFalse(give.CheckCompletion());
		Click();
		Click();
		Assert.AreEqual(2, Main.LocalPlayer.inventory[0].stack);
		quest.Objectives.Update(quest);
		Assert.AreEqual(2, Main.LocalPlayer.inventory[0].stack);
		Assert.IsTrue(give.Completed);
		Assert.AreEqual("Thanks", Main.npcChatText);
	}

	[TestMethod]
	public void TimedOutObjectiveRejectsOpenSessionSubmission()
	{
		var talk = new WorldTalkObjective(NPCID.Guide);
		talk.WithDialogue(d => d.WithText("First")).WithTimeLimit(1);
		AddWorld(talk);
		Click();
		talk.Timer.Update(1);
		Click();
		Assert.IsFalse(talk.Talked);
		Assert.IsFalse(QuestChat.IsChattingWith(Main.npc[0]));
	}

	[TestMethod]
	public void WorldTalkDoesNotAutoCompleteOnServerAndClientConfirmationUsesExistingDelta()
	{
		var talk = new WorldTalkObjective(NPCID.Guide);
		talk.WithDialogue(d => d.WithText("First").WithCompletionText("Thanks"));
		AddWorld(talk);
		Main.netMode = NetmodeID.Server;
		talk.Update();
		Assert.IsFalse(talk.Talked);
		Main.netMode = NetmodeID.MultiplayerClient;
		Click();
		Click();
		Assert.IsFalse(talk.Talked);
		Assert.IsTrue(talk.NeedDeltaSync);
		using var delta = new MemoryStream();
		talk.SendDelta(new BinaryWriter(delta));
		delta.Position = 0;
		var server = new WorldTalkObjective(NPCID.Guide);
		server.WithDialogue(d => d.WithText("First"));
		server.ReceiveDelta(new BinaryReader(delta));
		Assert.IsTrue(server.Talked);
		using var snapshot = new MemoryStream();
		server.NetSend(new BinaryWriter(snapshot));
		snapshot.Position = 0;
		talk.NetReceive(new BinaryReader(snapshot));
		new QuestHooks.QuestSystem().PostUpdatePlayers();
		Assert.AreEqual("Thanks", Main.npcChatText);
	}

	[TestMethod]
	public void ButtonSlotAndMissingCandidatePreserveNpcActions()
	{
		string button = "Shop";
		QuestChat.SetChatButton(Main.npc[0], ref button);
		Assert.AreEqual("Shop", button);
		var talk = new WorldTalkObjective(NPCID.Guide);
		talk.WithDialogue(d => d.WithText("First"));
		AddWorld(talk);
		Assert.IsTrue(QuestChat.OnChatButtonClicked(Main.npc[0], firstButton: true, useSecondButton: false));
		Assert.AreEqual("First", Main.npcChatText);
		Assert.IsFalse(QuestChat.OnChatButtonClicked(Main.npc[0], firstButton: false, useSecondButton: false));
		Assert.IsFalse(talk.Talked);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void TimeoutAndRetrySnapshotDoNotReplayPreviousConfirmation(bool give)
	{
		WorldObjectiveBase Create() => give
			? new WorldGiveObjective(NPCID.Guide, ItemID.Wood, 1)
				.WithDialogue(d => d.WithText("First")).WithTimeLimit(10)
			: new WorldTalkObjective(NPCID.Guide)
				.WithDialogue(d => d.WithText("First")).WithTimeLimit(10);
		var client = Create();
		var server = Create();
		Main.netMode = NetmodeID.MultiplayerClient;
		Main.LocalPlayer.inventory[0] = new Item { type = ItemID.Wood, stack = 2 };
		Assert.IsTrue(((IQuestDialogueObjective)client).TrySubmitObjective(Main.LocalPlayer));
		Assert.IsTrue(client.NeedDeltaSync);
		server.Timer.Update(10);
		Snapshot(server, client);
		Assert.IsTrue(client.IsTimedOut);
		server.ResetProgress();
		Snapshot(server, client);
		TalkTo(-1);
		client.Update();
		Assert.IsFalse(client.NeedDeltaSync, "An expired attempt must not upload its previous confirmation after retry.");
		Assert.IsFalse(client.CheckCompletion());
	}

	private static void Snapshot(WorldObjectiveBase source, WorldObjectiveBase target)
	{
		using var stream = new MemoryStream();
		source.NetSend(new BinaryWriter(stream));
		stream.Position = 0;
		target.NetReceive(new BinaryReader(stream));
	}

	[TestMethod]
	public void RemoteProgressBeforeNodeAdvanceCancelsUnsubmittedDialogue()
	{
		var give = new WorldGiveObjective(NPCID.Guide, ItemID.Wood, 1);
		give.WithDialogue(d => d.WithText("First").WithCompletionText("Thanks"));
		AddWorld(give);
		Main.netMode = NetmodeID.MultiplayerClient;
		Main.LocalPlayer.inventory[0] = new Item { type = ItemID.Wood, stack = 2 };
		Click();
		using var stream = new MemoryStream([1]);
		give.ReceiveMain(new BinaryReader(stream));
		Assert.IsFalse(give.Completed, "Simulate progress received before the container advances.");
		Click();
		Assert.IsFalse(QuestChat.IsChattingWith(Main.npc[0]));
		Assert.AreNotEqual("Thanks", Main.npcChatText);
		Assert.AreEqual(2, Main.LocalPlayer.inventory[0].stack);
		string button = "Original button";
		QuestChat.SetChatButton(Main.npc[0], ref button);
		Assert.AreEqual("Original button", button);
	}

	[TestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void UnconfiguredTalkAndGiveRequireConfirmation(bool worldSide, bool give)
	{
		Main.LocalPlayer.inventory[0] = new Item { type = ItemID.Wood, stack = 4 };
		if (worldSide)
		{
			WorldObjectiveBase objective = give ? new WorldGiveObjective(NPCID.Guide, ItemID.Wood, 2) : new WorldTalkObjective(NPCID.Guide);
			objective.Update();
			Assert.IsFalse(objective.CheckCompletion());
			Assert.AreEqual(4, Main.LocalPlayer.inventory[0].stack);
			Assert.IsTrue(((IQuestDialogueObjective)objective).TrySubmitObjective(Main.LocalPlayer));
			Assert.IsTrue(objective.CheckCompletion());
		}
		else
		{
			PlayerObjectiveBase objective = give ? new GiveItemObjective([ItemID.Wood], 2, NPCID.Guide) : new TalkNPCObjective(NPCID.Guide);
			objective.Update();
			Assert.IsFalse(objective.CheckCompletion());
			Assert.AreEqual(4, Main.LocalPlayer.inventory[0].stack);
			Assert.IsTrue(((IQuestDialogueObjective)objective).TrySubmitObjective(Main.LocalPlayer));
			Assert.IsTrue(objective.CheckCompletion());
		}
		Assert.AreEqual(give ? 2 : 4, Main.LocalPlayer.inventory[0].stack);
		Assert.AreEqual("Original", Main.npcChatText);
	}

	[TestMethod]
	public void UnconfiguredObjectiveCanBeConfirmedThroughTheNpcButton()
	{
		var talk = new DefaultTalkObjective();
		AddWorld(talk);
		Click();
		Assert.AreEqual("Confirm this objective", Main.npcChatText);
		Assert.IsFalse(talk.Talked);
		Click();
		Assert.IsTrue(talk.Talked);
	}

	private sealed class DefaultTalkObjective() : WorldTalkObjective(NPCID.Guide)
	{
		public override string GetChat() => "Confirm this objective";
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DefaultGivePromptUsesItemTagsInTheNativeChatBox(bool worldSide)
	{
		var npcNames = (LocalizedText[])typeof(Lang).GetField("_npcNameCache", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
		var oldNpcName = npcNames[NPCID.Guide];
		try
		{
			npcNames[NPCID.Guide] = Language.GetOrRegister("Tests.Guide", () => "Guide");
			Language.GetOrRegister("Mods.Everglow.QuestSystem.Objectives.Give", () => "Give {0} {1} {3}");
			if (worldSide)
			{
				AddWorld(new WorldGiveObjective(NPCID.Guide, ItemID.Wood, 2));
			}
			else
			{
				var quest = new LocalQuest();
				quest.Objectives.Add(new GiveItemObjective([ItemID.Wood], 2, NPCID.Guide));
				player.AddQuest(quest, PlayerQuestState.Accepted, showText: false);
			}
			Click();
			StringAssert.Contains(Main.npcChatText, $"[i/s2:{ItemID.Wood}]");
			Assert.IsFalse(Main.npcChatText.Contains("ItemDrawer", StringComparison.Ordinal));
			Assert.IsTrue(QuestChat.IsChattingWith(Main.npc[0]));
		}
		finally
		{
			npcNames[NPCID.Guide] = oldNpcName;
		}
	}

	private void AddWorld(params WorldObjectiveBase[] objectives)
	{
		var quest = new SharedQuest();
		foreach (var objective in objectives)
		{
			quest.Objectives.Add(objective);
		}
		world.AddQuest(quest);
		quest.Unlock();
	}

	private static void Click() => Assert.IsTrue(QuestChat.OnChatButtonClicked(Main.npc[0], firstButton: false));

	private static void TalkTo(int index) => typeof(Player).GetProperty(nameof(Player.talkNPC))!.SetValue(Main.LocalPlayer, index);

	private static void SetSystem<T>(T system)
		where T : class => typeof(ContentInstance<T>).GetProperty(nameof(ContentInstance<T>.Instance))!.SetValue(null, system);

	private sealed class SharedQuest : WorldQuestBase
	{
		public override string LocalizationKey => "Tests.Dialogue";
	}

	private sealed class LocalQuest : PlayerQuestBase
	{
		public override string LocalizationKey => "Tests.Dialogue";

		public override string DisplayName => "Local dialogue test";
	}
}
