using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class TeahouseTalesTwoQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".IntroDialogue")).WithDescription(Text(Name + ".IntroObjective")))
			// TODO: 二层的调查地点、教团造物及完成规则尚未确定。
			.Add(new WorldReachObjective(_ => false, Text(Name + ".Objective")).WithDescription(Text(Name + ".InvestigateDescription")))
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".Completion")).WithDescription(Text(Name + ".ReportDescription")));
	}

	public override int GiverNpcType => ModContent.NPCType<TeahouseLady>();

	public override QuestSourceBase Source => Schorl;

	public override bool CanOffer(WorldQuestManager manager) => YggdrasilWorldSystem.EnteredKelpCurtain;
}
