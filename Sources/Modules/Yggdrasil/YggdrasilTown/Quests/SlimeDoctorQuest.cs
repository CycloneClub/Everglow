using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class SlimeDoctorQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldGiveObjective(GiverNpcType, ItemID.Gel, 10)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")));

		RewardItems.Add(new Item(ModContent.ItemType<GoodNeighborCoin>(), 10));
	}

	public override int GiverNpcType => ModContent.NPCType<Guard_of_YggdrasilTown>();

	public override QuestSourceBase Source => Anna;

	public override bool CanOffer(WorldQuestManager manager) =>
		manager.GetQuest<DefendTownQuest>() is DefendTownQuest quest
		&& quest.IsFirstInvasionCompleted();
}
