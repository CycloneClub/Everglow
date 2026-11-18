using Everglow.Commons.Mechanics.Quest.WorldSide;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class Warden_Quest3 : WardenInvasionQuest
{
	protected override int Stage => 2;

	public override bool CanUnlock() => WorldQuestManager.Instance.GetQuest<Warden_Quest2>()?.State == WorldQuestState.Completed;
}
