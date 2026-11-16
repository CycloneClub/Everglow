using System.Reflection;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Quests.SideStory;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Everglow.Yggdrasil.YggdrasilTown.Quests.SideStory.Herm_Quest;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class HermQuestIntegrationTest
{
	private static readonly FieldInfo CurrentWorld = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
	private int oldMode;
	private Player[] oldPlayers = null!;
	private NPC[] oldNpcs = null!;
	private int oldPlayerIndex;
	private Subworld? oldWorld;
	private WorldQuestSystem oldSystem = null!;
	private IReadOnlyList<WorldQuestSystem> oldSystems = null!;
	private WorldQuestManager manager = null!;
	private Herm_Quest lost = null!;
	private bool registeredBoolSerializer;

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
		oldMode = Main.netMode;
		oldPlayers = Main.player;
		oldNpcs = Main.npc;
		oldPlayerIndex = Main.myPlayer;
		Main.player = Enumerable.Range(0, Main.maxPlayers + 1).Select(_ => new Player()).ToArray();
		Main.npc = new NPC[Main.maxNPCs + 1];
		Main.myPlayer = 0;
		oldWorld = (Subworld?)CurrentWorld.GetValue(null);
		oldSystem = ContentInstance<WorldQuestSystem>.Instance;
		oldSystems = ContentInstance<WorldQuestSystem>.Instances;
		Main.netMode = NetmodeID.SinglePlayer;
		CurrentWorld.SetValue(null, null);
		manager = new WorldQuestManager(new TestState());
		var system = new WorldQuestSystem();
		typeof(WorldQuestSystem).GetProperty(nameof(WorldQuestSystem.Manager))!.SetValue(system, manager);
		SetSystems(system, [system]);
		lost = new Herm_Quest();
		manager.AddQuest(lost);
		manager.Initialize();
		foreach (var objective in lost.Objectives.AllObjectives)
		{
			objective.RewardItems.Clear(); // Headless tests verify progression; item spawning requires loaded content.
		}
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
		SetSystems(oldSystem, oldSystems);
		Main.netMode = oldMode;
		Main.player = oldPlayers;
		Main.npc = oldNpcs;
		Main.myPlayer = oldPlayerIndex;
		CurrentWorld.SetValue(null, oldWorld);
	}

	[TestMethod]
	[DataRow(NetmodeID.SinglePlayer, true)]
	[DataRow(NetmodeID.MultiplayerClient, false)]
	public void PausedConversationUnlocksOnlyInSinglePlayer(int mode, bool expected)
	{
		bool oldPaused = Main.gamePaused;
		var oldLanguage = LanguageManager.Instance;
		try
		{
			Main.netMode = mode;
			Main.gamePaused = true;
			LanguageManager.Instance = (LanguageManager)Activator.CreateInstance(typeof(LanguageManager), true)!;
			var herm = new Herm();
			typeof(ModType).GetProperty(nameof(ModType.Mod))!.SetValue(herm, new TestMod());
			typeof(ModNPC).GetProperty("Entity")!.SetValue(herm, new NPC());
			herm.GetChat(); // Auto Pause does not run NPC AI while this dialogue is open.
			Assert.AreEqual(expected, lost.HasMet);
		}
		finally
		{
			Main.gamePaused = oldPaused;
			LanguageManager.Instance = oldLanguage;
		}
	}

	[TestMethod]
	[DataRow(NetmodeID.SinglePlayer, false, 0f, true)]
	[DataRow(NetmodeID.SinglePlayer, true, 0f, false)]
	[DataRow(NetmodeID.SinglePlayer, false, 241f, false)]
	[DataRow(NetmodeID.MultiplayerClient, false, 0f, false)]
	public void MinerAiUnlocksOnlyForNearbyLivingConversation(int mode, bool dead, float distance, bool expected)
	{
		CurrentWorld.SetValue(null, new YggdrasilWorld());
		Main.netMode = mode;
		Main.LocalPlayer.active = true;
		Main.LocalPlayer.dead = dead;
		var herm = new Herm();
		var npc = new NPC { active = true, whoAmI = 0 };
		typeof(ModNPC).GetProperty("Entity")!.SetValue(herm, npc);
		Main.npc[0] = npc;
		Main.LocalPlayer.Center = npc.Center + new Microsoft.Xna.Framework.Vector2(distance, 0);
		typeof(Player).GetProperty(nameof(Player.talkNPC))!.SetValue(Main.LocalPlayer, 0);
		int statusChanges = 0;
		manager.QuestStatusUpdated += _ => statusChanges++;
		for (int i = 0; i < 120; i++)
		{
			herm.AI();
		}
		Assert.AreEqual(expected, lost.HasMet);
		Assert.AreEqual(expected ? 1 : 0, statusChanges);
		Assert.AreEqual(HermStage.Unmet, lost.Stage, "Meeting alone must not issue the card.");
	}

	[TestMethod]
	public void IntroductionRequiresConfirmationThroughQuestDialogue()
	{
		MeetMiner();
		var objective = lost.Objectives.AllObjectives[0];
		Assert.IsInstanceOfType<IQuestDialogueObjective>(objective);
		var dialogue = (IQuestDialogueObjective)objective;
		Main.LocalPlayer.active = true;
		Main.npc[0] = new NPC { type = dialogue.NPCType, active = true };
		typeof(Player).GetProperty(nameof(Player.talkNPC))!.SetValue(Main.LocalPlayer, 0);
		var session = new QuestDialogueSession(
			dialogue.Dialogue,
			() => dialogue.TrySubmitObjective(Main.LocalPlayer), objective.CheckCompletion, () => lost.ActiveObjectives.Contains(objective));
		for (int i = 0; i < 4; i++)
		{
			session.Continue();
			Tick();
			Assert.AreEqual(HermStage.Unmet, lost.Stage);
		}
		session.Continue();
		Tick();
		Assert.AreEqual(HermStage.CardIssued, lost.Stage);
	}

	[TestMethod]
	public void SearchUsesExistingObjectiveTimerAndStaysActiveAtDeadline()
	{
		AcceptCard();
		Assert.AreEqual(0, lost.Search.Timer.ElapsedTime);
		Submit(lost.Delivery);
		Assert.AreEqual(604800, lost.Search.Timer.TimeLimit);
		SetSearchTime(lost.Search.Timer.TimeLimit - WorldQuestManager.UpdateInterval);
		manager.Update();
		Assert.AreEqual(604800, lost.Search.Timer.ElapsedTime);
		Assert.IsTrue(lost.Search.IsTimedOut);
		Assert.IsFalse(lost.CanRetryObjective(lost.Search.ObjectiveID));
		Assert.AreEqual(WorldQuestState.Active, lost.State);
		manager.Update();
		Assert.AreEqual(604800, lost.Search.Timer.ElapsedTime);
	}

	[TestMethod]
	public void FirstConversationOffersQuestBeforeTheCardIsIssued()
	{
		int statusChanges = 0;
		manager.QuestStatusUpdated += _ => statusChanges++;
		MeetMiner();
		Assert.AreEqual(WorldQuestState.Active, lost.State);
		Assert.IsFalse(lost.Objectives.AllObjectives[0].Completed);
		Assert.AreEqual(HermStage.Unmet, lost.Stage);
		Herm_Quest.ApplyValidatedMeeting();
		Assert.AreEqual(1, statusChanges, "Repeated meetings must not unlock or notify twice.");
		Assert.HasCount(1, manager.Quests);
	}

	[TestMethod]
	public void SingleQuestContinuesThroughAllStagesWithoutFailingOnTimeout()
	{
		AcceptCard();
		Assert.AreEqual(WorldQuestState.Active, lost.State);
		Assert.HasCount(7, lost.Objectives.AllObjectives);
		Submit(lost.Delivery);
		Assert.AreEqual(WorldQuestState.Active, lost.State);
		Assert.IsTrue(lost.Objectives.AllObjectives[1].Completed);
		Assert.IsFalse(lost.Objectives.AllObjectives[2].Completed);
		SetSearchTime(Herm_Quest.SearchDuration);
		manager.Update();
		Assert.AreEqual(WorldQuestState.Active, lost.State);
		FindMiner();
		Assert.IsFalse(lost.Search.Completed);
		Assert.IsTrue(lost.Objectives.AllObjectives[4].Completed);
		Assert.IsFalse(lost.Objectives.AllObjectives[3].Completed, "Finding Herm must not skip his dialogue/lamp interaction.");
		SubmitDialogue(lost.FoundAlive ? 3 : 5);
		Assert.IsTrue(lost.Objectives.AllObjectives[5].Completed);
		Assert.AreEqual(WorldQuestState.Active, lost.State);
		SubmitDialogue(6);
		Assert.AreEqual(WorldQuestState.Completed, lost.State);
		Assert.IsFalse(lost.FoundAlive);
	}

	[TestMethod]
	public void QuestSaveRestoresBuiltInClockAndEnding()
	{
		AcceptCard();
		Submit(lost.Delivery);
		SetSearchTime(Herm_Quest.SearchDuration);
		var tag = new TagCompound();
		manager.SaveData(tag);
		manager.LoadData(tag);
		Assert.AreEqual(WorldQuestState.Active, lost.State);
		Assert.AreEqual(Herm_Quest.SearchDuration, lost.Search.Timer.ElapsedTime);
		FindMiner();
		var found = new TagCompound();
		manager.SaveData(found);
		manager.LoadData(found);
		Assert.IsFalse(lost.FoundAlive);
		Assert.AreEqual(HermStage.Found, lost.Stage);
	}

	[TestMethod]
	public void NetworkSnapshotPreservesDeadlineAndResolvedBranch()
	{
		AcceptCard();
		Submit(lost.Delivery);
		SetSearchTime(Herm_Quest.SearchDuration - 1);
		FindMiner();
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true);
		lost.NetSend(writer);
		stream.Position = 0;
		var copy = new Herm_Quest();
		copy.Initialize();
		copy.NetReceive(new BinaryReader(stream));
		Assert.AreEqual(HermStage.Found, copy.Stage);
		Assert.IsTrue(copy.FoundAlive);
		Assert.AreEqual(Herm_Quest.SearchDuration - 1, copy.Search.Timer.ElapsedTime);
		Assert.AreEqual(WorldQuestState.Active, copy.State);
	}

	[TestMethod]
	public void ClientCannotSubmitTrustedInteraction()
	{
		Main.netMode = NetmodeID.Server;
		new HermValidatedInteractionPacket.Handler().Handle(new HermValidatedInteractionPacket(), 0);
		Assert.AreEqual(WorldQuestState.Locked, lost.State);
	}

	private void Submit(WorldObjectiveBase objective)
	{
		using var stream = new MemoryStream([1]);
		objective.ReceiveDelta(new BinaryReader(stream));
		Tick();
	}

	private void FindMiner()
	{
		// Simulate both native reach signals arriving; the branch and timer decide the ending.
		foreach (int id in new[] { 2, 4 })
		{
			using var stream = new MemoryStream([1]);
			lost.Objectives.AllObjectives[id].ReceiveDelta(new BinaryReader(stream));
		}
		Tick();
	}

	private void Tick()
	{
		for (int i = 0; i < 4; i++)
		{
			manager.Update();
		}
	}

	private void MeetMiner()
	{
		Herm_Quest.ApplyValidatedMeeting();
		Assert.AreEqual(WorldQuestState.Active, lost.State);
		Tick();
	}

	private void AcceptCard()
	{
		MeetMiner();
		SubmitDialogue(0);
	}

	private void SubmitDialogue(int objectiveId)
	{
		var dialogue = (IQuestDialogueObjective)lost.Objectives.AllObjectives[objectiveId];
		Main.LocalPlayer.active = true;
		Main.npc[0] = new NPC { type = dialogue.NPCType, active = true };
		typeof(Player).GetProperty(nameof(Player.talkNPC))!.SetValue(Main.LocalPlayer, 0);
		Assert.IsTrue(dialogue.TrySubmitObjective(Main.LocalPlayer));
		Tick();
		Assert.IsTrue(lost.Objectives.AllObjectives[objectiveId].Completed);
	}

	[TestMethod]
	[DataRow(604799, true)]
	[DataRow(604800, false)]
	public void EncounterUsesTaskTimerBoundaryAndStopsClock(int elapsed, bool foundAlive)
	{
		AcceptCard();
		Submit(lost.Delivery);
		SetSearchTime(elapsed);
		FindMiner();
		Assert.AreEqual(HermStage.Found, lost.Stage);
		Assert.AreEqual(foundAlive, lost.FoundAlive);
		Assert.AreEqual(elapsed, lost.Search.Timer.ElapsedTime);
	}

	[TestMethod]
	public void LateSearchOnlyDetectsMinerAfterDeadline()
	{
		AcceptCard();
		Submit(lost.Delivery);
		var herm = new Herm();
		var npc = new NPC { active = true };
		npc.ai[0] = 1;
		typeof(ModNPC).GetProperty("Entity")!.SetValue(herm, npc);
		typeof(NPC).GetProperty(nameof(NPC.ModNPC))!.SetValue(npc, herm);
		Main.npc[0] = npc;
		typeof(Player).GetProperty(nameof(Player.talkNPC))!.SetValue(Main.LocalPlayer, 0);
		var lateSearch = lost.Objectives.AllObjectives[4];
		lateSearch.Update();
		Assert.IsFalse(lateSearch.CheckCompletion());
		Assert.AreEqual(HermStage.Searching, lost.Stage);
		SetSearchTime(Herm_Quest.SearchDuration);
		lateSearch.Update();
		Assert.IsTrue(lateSearch.CheckCompletion());
		Tick();
		Assert.AreEqual(HermStage.Found, lost.Stage);
		Assert.IsFalse(lost.FoundAlive);
	}

	private void SetSearchTime(int time)
	{
		lost.Search.Timer.Reset();
		lost.Search.Timer.Update(time);
	}

	private static void SetSystems(WorldQuestSystem system, IReadOnlyList<WorldQuestSystem> systems)
	{
		typeof(ContentInstance<WorldQuestSystem>).GetProperty(nameof(ContentInstance<WorldQuestSystem>.Instance))!.SetValue(null, system);
		typeof(ContentInstance<WorldQuestSystem>).GetProperty(nameof(ContentInstance<WorldQuestSystem>.Instances))!.SetValue(null, systems);
	}

	private sealed class TestMod : Mod
	{
		public override string Name => "Everglow";
	}

	private sealed class TestState : IGameStateProvider
	{
		public double TimeForVisualEffects => 0;

		public bool GameMenu => false;

		public bool GameInactive => false;

		public bool GamePaused => false;
	}
}
