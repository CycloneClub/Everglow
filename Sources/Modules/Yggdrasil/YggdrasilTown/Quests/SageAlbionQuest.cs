using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

// TODO: 贤者阿尔比恩：会长 NPC 未实现；接待员与会长之间的对话不能用其他 NPC 顶替。
// 后续流程为接待、拜访会长、返回接待处领委托、击败龙鳞古壳、向会长报告。
public sealed class SageAlbionQuest : TownNpcQuest
{
	public override void Initialize() =>
		Objectives.Add(new WorldReachObjective(_ => false));

	public override int GiverNpcType => NPCID.None;

	public override bool CanOffer(WorldQuestManager manager) => false;

	public override QuestType Type => QuestType.MainStory;
}
