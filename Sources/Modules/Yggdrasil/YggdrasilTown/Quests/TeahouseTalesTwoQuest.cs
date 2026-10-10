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
			.Add(new WorldTalkObjective(GiverNpcType))
			// TODO: 二层的调查地点、教团造物及完成规则尚未确定。
			.Add(new WorldReachObjective(_ => false))
			.Add(new WorldTalkObjective(GiverNpcType));
	}

	public override int GiverNpcType => ModContent.NPCType<TeahouseLady>();

	public override QuestSourceBase Source => Schorl;

	public override bool CanOffer(WorldQuestManager manager) => YggdrasilWorldSystem.EnteredKelpCurtain;
}
