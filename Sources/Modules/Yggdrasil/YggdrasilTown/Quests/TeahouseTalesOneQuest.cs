using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class TeahouseTalesOneQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".IntroDialogue")).WithDescription(Text(Name + ".IntroObjective")))
			// TODO: 无主肉傀尚无 NPC 实现，不能用无关怪物完成这个目标。
			.Add(new WorldReachObjective(_ => false, Text(Name + ".Objective")).WithDescription(Text(Name + ".DefeatDescription")))
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".Completion")).WithDescription(Text(Name + ".ReportDescription")));
	}

	public override int GiverNpcType => ModContent.NPCType<TeahouseLady>();

	public override QuestSourceBase Source => Schorl;

	public override bool CanOffer(WorldQuestManager manager) => true;
}
