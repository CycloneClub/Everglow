using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class TroublesomeGuestsQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".IntroDialogue")).WithDescription(Text(Name + ".IntroObjective")))
			// TODO: 策划未明确海盗类型、击杀数量及解锁条件。
			.Add(new WorldReachObjective(_ => false, Text(Name + ".Objective")).WithDescription(Text(Name + ".DefeatDescription")))
			.Add(new WorldTalkObjective(GiverNpcType, Text(Name + ".Completion")).WithDescription(Text(Name + ".ReportDescription")));
	}

	public override int GiverNpcType => ModContent.NPCType<CanteenMaid>();

	public override QuestSourceBase Source => Betty;

	public override bool CanOffer(WorldQuestManager manager) => false;
}
