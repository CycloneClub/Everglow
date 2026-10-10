using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class ChangingTheInnQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".IntroDialogue")).WithDescription(Text(Name + ".IntroObjective")))
			// TODO: 策划尚未指定要交付的植物及数量。
			.Add(new WorldReachObjective(_ => false, Text(Name + ".Objective")).WithDescription(Text(Name + ".DeliveryDescription")))
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".Completion")).WithDescription(Text(Name + ".ReportDescription")));
	}

	public override int GiverNpcType => ModContent.NPCType<InnKeeper>();

	public override QuestSourceBase Source => Georg;

	public override bool CanOffer(WorldQuestManager manager) => YggdrasilWorldSystem.EnteredKelpCurtain;
}
