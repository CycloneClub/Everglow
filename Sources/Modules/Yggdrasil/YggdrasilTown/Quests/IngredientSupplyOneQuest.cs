using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class IngredientSupplyOneQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".IntroDialogue")).WithDescription(Text(Name + ".IntroObjective")))
			// TODO: 策划中的晚饭尚未指定物品；交付流程先独立完成。
			.Add(new WorldGiveObjective(GiverNpcType, ModContent.ItemType<GrassCarp>(), 5, Text(Name + ".IntroDialogue"), Text(Name + ".Completion"))
				.WithDescription(Text(Name + ".DeliveryDescription")));
	}

	public override int GiverNpcType => ModContent.NPCType<Restauranteur>();

	public override QuestSourceBase Source => Rolle;

	public override bool CanOffer(WorldQuestManager manager) => true;
}
