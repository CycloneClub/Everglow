using Everglow.Commons.Mechanics.Quest.WorldSide;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class Warden_Quest2 : WardenInvasionQuest
{
	protected override int Stage => 1;

	public override bool CanUnlock() => WorldQuestManager.Instance.GetQuest<Warden_Quest1>()?.State == WorldQuestState.Completed;
}
