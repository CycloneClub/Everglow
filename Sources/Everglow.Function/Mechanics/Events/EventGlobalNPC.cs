namespace Everglow.Commons.Mechanics.Events;

public sealed class EventGlobalNPC : GlobalNPC
{
	public override void OnKill(NPC npc) => EventSystem.NotifyNPCKilled(npc);
}
