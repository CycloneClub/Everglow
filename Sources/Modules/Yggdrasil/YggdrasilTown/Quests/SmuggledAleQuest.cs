using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class SmuggledAleQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType))
			.Add(new WorldGiveObjective(GiverNpcType, ItemID.Ale, 5)
				.WithRewards(new Item(ItemID.GoldCoin, 3), new Item(ItemID.Gel, 25)));
	}

	public override int GiverNpcType => ModContent.NPCType<Guard_of_YggdrasilTown>();

	public override QuestSourceBase Source => Anna;

	public override bool CanOffer(WorldQuestManager manager) => true;
}
