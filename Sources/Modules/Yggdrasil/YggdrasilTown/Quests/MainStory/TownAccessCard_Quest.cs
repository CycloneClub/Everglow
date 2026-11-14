using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.MainStory;

public class TownAccessCard_Quest : PlayerQuestBase
{
	public override QuestType Type => QuestType.MainStory;

	public TownAccessCard_Quest()
	{
		Objectives
			.Add(new TalkNPCObjective(ModContent.NPCType<QuestCounter_Consultant>())
				.WithDialogue(d => d
					.WithText("Dialogue.Intro")
					.WithCompletionText("Dialogue.Complete")));

		RewardItems.Add(new Item(ModContent.ItemType<YggdrasilTownAccessCard>(), 1));
	}
}
