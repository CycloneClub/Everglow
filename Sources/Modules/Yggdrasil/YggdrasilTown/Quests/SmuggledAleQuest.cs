using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class SmuggledAleQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldGiveObjective(GiverNpcType, ItemID.Ale, 5)
				.WithDialogue(d => d
					.WithText("Dialogue.Request")
					.WithCompletionText("Dialogue.Success"))
				.WithRewards(new Item(ItemID.GoldCoin, 3), new Item(ItemID.Gel, 25)));

		RewardItems.Add(new Item(ModContent.ItemType<GoodNeighborCoin>(), 10));
	}

	public override int GiverNpcType => ModContent.NPCType<Guard_Of_YggdrasilTown>();

	public override QuestSourceBase Source => Anna;

	public override bool CanOffer(WorldQuestManager manager) => true;
}
