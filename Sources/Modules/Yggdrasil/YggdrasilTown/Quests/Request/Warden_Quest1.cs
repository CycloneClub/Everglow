namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class Warden_Quest1 : WardenInvasionQuest
{
	protected override int Stage => 0;

	public override bool CanUnlock() => YggdrasilWorldSystem.DownedSquamousShell;

	public bool IsFirstInvasionCompleted() => Objectives.AllNodes[1].Completed;
}
