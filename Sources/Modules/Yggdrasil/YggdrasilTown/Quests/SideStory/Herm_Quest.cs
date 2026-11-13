using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Utilities;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Pets;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.SideStory;

public sealed class Herm_Quest : WorldQuestBase
{
	public enum HermStage : byte
	{
		Unmet,
		CardIssued,
		Searching,
		Found,
		LampRecovered,
		Reported,
	}

	public static Herm_Quest Instance => WorldQuestManager.Instance?.GetQuest<Herm_Quest>() as Herm_Quest;

	public const int SearchDuration = 7 * 24 * 60 * 60;

	public override QuestType Type => QuestType.SideStory;

	private WorldTalkObjective introduction;
	private WorldTalkObjective farewell;
	private WorldTalkObjective takeExtinguishedLamp;
	private WorldTalkObjective report;
	private WorldObjectiveBase lateSearch;

	public WorldObjectiveBase Search { get; private set; }

	public WorldGiveObjective Delivery { get; private set; }

	public bool SearchExpired => Search?.IsTimedOut == true;

	public bool HasMet => State != WorldQuestState.Locked;

	public bool FoundAlive => Search.Completed;

	public HermStage Stage => report.Completed ? HermStage.Reported
		: farewell.Completed || takeExtinguishedLamp.Completed ? HermStage.LampRecovered
		: Search.Completed || lateSearch.Completed ? HermStage.Found
		: Delivery.Completed ? HermStage.Searching
		: introduction.Completed ? HermStage.CardIssued : HermStage.Unmet;

	public override void Initialize()
	{
		int hermType = ModContent.NPCType<Herm>();
		int counterType = ModContent.NPCType<QuestCounter_Consultant>();
		introduction = new WorldTalkObjective(hermType).WithDialogue(d => d
			.WithText("Dialogue.Intro0", "Dialogue.Intro1", "Dialogue.Intro2", "Dialogue.Intro3", "Dialogue.Intro4")
			.WithCompletionText("Dialogue.Waiting"));
		Delivery = new WorldGiveObjective(counterType, ModContent.ItemType<HermCommuterCard>(), 1).WithDialogue(d => d
			.WithText("Dialogue.CounterCard")
			.WithConfirmButtonText("Dialogue.DeliverCard")
			.WithUnavailableText("Dialogue.MissingCard")
			.WithCompletionText("Dialogue.CounterSearch"));
		farewell = new WorldTalkObjective(hermType).WithDialogue(d => d
			.WithText("Dialogue.Farewell0", "Dialogue.Farewell1", "Dialogue.Farewell2", "Dialogue.Farewell3")
			.WithCompletionText("Dialogue.Silence"));
		takeExtinguishedLamp = new WorldTalkObjective(hermType).WithDialogue(d => d
			.WithText("Dialogue.Corpse")
			.WithConfirmButtonText("Dialogue.TakeLamp")
			.WithCompletionText("Dialogue.Silence"));
		report = new WorldTalkObjective(counterType).WithDialogue(d => d
			.WithText("Dialogue.Report0", "Dialogue.Report1")
			.WithConfirmButtonText("Dialogue.Report")
			.WithCompletionText("Dialogue.CounterThanks"));
		Search = new WorldReachObjective(player => player.TalkNPC?.ModNPC is Herm { IsInMine: true }).WithTimeLimit(SearchDuration, retriable: false);
		lateSearch = new WorldReachObjective(player => SearchExpired && player.TalkNPC?.ModNPC is Herm { IsInMine: true });
		Objectives
			.Add(introduction.WithRewards(new Item(ModContent.ItemType<HermCommuterCard>())))
			.Add(Delivery)
			.AddBranch(
				[Search, farewell.WithRewards(new Item(ModContent.ItemType<Badr>()))],
				[lateSearch, takeExtinguishedLamp.WithRewards(new Item(ModContent.ItemType<ExtinguishedBadr>()))])
			.Add(report);

		RewardItems.Add(new Item(ModContent.ItemType<MiningPowerPickaxe>()));
	}

	public static void ApplyValidatedMeeting()
	{
		if ((!NetUtils.IsSingle && !NetUtils.IsMainServer) || Instance is not { HasMet: false } quest)
		{
			return;
		}
		// Unlock 已负责广播任务快照；这里只通知本地展示层状态变化。
		quest.Unlock();
		WorldQuestManager.Instance.OnQuestStatusUpdated(quest);
	}

	public override bool CanUnlock() => false;
}
