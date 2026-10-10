using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

// TODO: 迷失的矿工 → 巴德尔之光：赫姆、受伤/死亡形态、通勤卡与公会接待未接入。
// 后续流程为交卡、七日内寻人（超时转为尸体/矿灯复燃分支）、向公会报告并获得动力镐。
public sealed class LostMinerQuest : TownNpcQuest
{
	public override void Initialize() =>
		Objectives
			.Add(new WorldReachObjective(_ => false))
			.Add(new WorldReachObjective(_ => false));

	public override int GiverNpcType => NPCID.None;

	public override bool CanOffer(WorldQuestManager manager) => false;
}
