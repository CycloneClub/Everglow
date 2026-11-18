using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class CanteenMaid_Quest2 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.Add(new WorldKillNPCObjective(ModContent.NPCType<UmbrellaTailThief>(), 5))
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective2")));

		RewardItems.Add(new Item(ItemID.GoldCoin, 2));
	}

	private int GiverNpcType => ModContent.NPCType<CanteenMaid>();

	public override QuestSourceBase Source => YggdrasilQuestSources.CanteenMaid;

	public override bool CanUnlock() =>
		WorldQuestManager.Instance.GetQuest<CanteenMaid_Quest1>()?.State == WorldQuestState.Completed;
}
