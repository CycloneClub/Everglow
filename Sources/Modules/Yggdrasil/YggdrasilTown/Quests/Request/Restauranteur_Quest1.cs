using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class Restauranteur_Quest1 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.Add(new WorldGiveObjective(GiverNpcType, ModContent.ItemType<GrassCarp>(), 5)
				.WithDialogue(d => d.WithText("Dialogue.Objective1Request").WithCompletionText("Dialogue.Objective1Success")));

		RewardItems.Add(new Item(ItemID.GoldCoin, 1));
	}

	private int GiverNpcType => ModContent.NPCType<Restauranteur>();

	public override QuestSourceBase Source => YggdrasilQuestSources.Restauranteur;

	public override bool CanUnlock() => true;
}
