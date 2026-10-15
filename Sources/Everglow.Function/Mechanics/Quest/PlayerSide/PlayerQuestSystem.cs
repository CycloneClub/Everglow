namespace Everglow.Commons.Mechanics.Quest.PlayerSide;

public class PlayerQuestSystem : ModSystem
{
	public PlayerQuestManager Manager { get; private set; }

	public PlayerQuestActions Actions { get; private set; }

	public override void Load()
	{
		Manager = new PlayerQuestManager();
		Actions = new PlayerQuestActions(Manager);
		Manager.Load();
	}

	public override void Unload()
	{
		Manager?.Unload();
		Manager = null;
		Actions = null;
	}
}
