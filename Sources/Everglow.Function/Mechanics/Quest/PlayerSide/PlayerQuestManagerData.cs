using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide;

public class PlayerQuestManagerData
{
	private Dictionary<int, int> nPCKillCounter;
	private List<PlayerQuestBase> quests;

	public IReadOnlyDictionary<int, int> NPCKillCounter => nPCKillCounter;

	public IReadOnlyList<PlayerQuestBase> Quests => quests;

	private PlayerQuestManagerData()
	{
	}

	public PlayerQuestManagerData(
		Dictionary<int, int> nPCKillCounter,
		List<PlayerQuestBase> quests)
	{
		this.nPCKillCounter = nPCKillCounter;
		this.quests = quests;
	}
}
