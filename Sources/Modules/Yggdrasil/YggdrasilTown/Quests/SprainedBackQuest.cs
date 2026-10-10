using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

// TODO: 万花油的物品及配方未实现，当前任务只处理原料交付。
public sealed class SprainedBackQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType))
			// 花粉未标倍数，按一份；两种材料可分别交付，全部完成后再汇报。
			.AddParallel(
				new WorldGiveObjective(GiverNpcType, ModContent.ItemType<UnstablePollen>(), 1),
				new WorldGiveObjective(GiverNpcType, ModContent.ItemType<CaterpillarJuice>(), 2))
			.Add(new WorldTalkObjective(GiverNpcType));
	}

	public override int GiverNpcType => ModContent.NPCType<CanteenMaid>();

	public override QuestSourceBase Source => Betty;

	public override bool CanOffer(WorldQuestManager manager) => manager.GetQuest<SelfRelianceQuest>()?.State == WorldQuestState.Completed;
}
