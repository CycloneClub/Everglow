using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

// TODO: 无名之人：界域行者/黑市商人的首次消失事件未实现，策划也未给出后续目标。
public sealed class NamelessPersonQuest : TownNpcQuest
{
	public override void Initialize() =>
		Objectives.Add(new WorldReachObjective(_ => false));

	public override int GiverNpcType => NPCID.None;

	public override bool CanOffer(WorldQuestManager manager) => false;
}
