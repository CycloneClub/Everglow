using Everglow.Commons.Mechanics.Quest.WorldSide;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class Warden_Quest4 : WardenInvasionQuest
{
	protected override int Stage => 3;

	public override bool CanUnlock() => WorldQuestManager.Instance.GetQuest<Warden_Quest3>()?.State == WorldQuestState.Completed;
}
