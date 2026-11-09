using Everglow.Commons.Mechanics.Quest.Core.Dialogue;

namespace Everglow.Commons.Mechanics.Quest.Hooks;

public sealed class QuestSystem : ModSystem
{
	public override void PostUpdatePlayers() => QuestChat.Update();

	public override void OnWorldUnload() => QuestChat.CloseChat();

	public override void Unload() => QuestChat.CloseChat();
}
