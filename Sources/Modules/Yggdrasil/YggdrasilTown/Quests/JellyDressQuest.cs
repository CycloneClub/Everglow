using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

// TODO: 别出心裁-球冻水波裙：裁衣匠斯卡帕、服装及配方/商店奖励未实现。
// 后续流程为击杀十只球冻并交付掉落物（数量待定）、击败球冻王并交膜、等待一晚后领取成果。
public sealed class JellyDressQuest : TownNpcQuest
{
	public override void Initialize() =>
		Objectives.Add(new WorldReachObjective(_ => false));

	public override int GiverNpcType => NPCID.None;

	public override bool CanOffer(WorldQuestManager manager) => false;
}
