using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class CanteenMaid_Quest1 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.AddParallel(
				new WorldGiveObjective(GiverNpcType, ModContent.ItemType<UnstablePollen>(), 1),
				new WorldGiveObjective(GiverNpcType, ModContent.ItemType<CaterpillarJuice>(), 2))
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective3")));

		RewardItems.Add(new Item(ItemID.GoldCoin, 1));
	}

	private int GiverNpcType => ModContent.NPCType<CanteenMaid>();

	public override QuestSourceBase Source => YggdrasilQuestSources.CanteenMaid;

	public override bool CanUnlock() => WorldQuestManager.Instance.GetQuest<Restauranteur_Quest2>()?.State == WorldQuestState.Completed;
}
