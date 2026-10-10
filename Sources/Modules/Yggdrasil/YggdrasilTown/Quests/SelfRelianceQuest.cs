using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class SelfRelianceQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".IntroDialogue")).WithDescription(Text(Name + ".IntroObjective")))
			.Add(new WorldCollectItemObjective(ModContent.ItemType<LampFruit>(), 12).WithDescription(Text(Name + ".CollectDescription")))
			// TODO: 灯果汁腌草鱼及配方尚未实现；灯果只检查持有，不消耗。
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".Completion")).WithDescription(Text(Name + ".ReportDescription")));
	}

	public override int GiverNpcType => ModContent.NPCType<Restauranteur>();

	public override QuestSourceBase Source => Rolle;

	public override bool CanOffer(WorldQuestManager manager) => YggdrasilWorldSystem.DownedKingJellyBall || YggdrasilWorldSystem.DownedSquamousShell;
}
