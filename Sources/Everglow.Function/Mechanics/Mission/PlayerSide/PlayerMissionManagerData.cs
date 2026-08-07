using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide;

public class PlayerMissionManagerData
{
	private Dictionary<int, int> nPCKillCounter;
	private List<PlayerMissionBase> missionPools;

	public IReadOnlyDictionary<int, int> NPCKillCounter => nPCKillCounter;

	public IReadOnlyList<PlayerMissionBase> MissionPools => missionPools;

	private PlayerMissionManagerData()
	{
	}

	public PlayerMissionManagerData(
		Dictionary<int, int> nPCKillCounter,
		List<PlayerMissionBase> missionPools)
	{
		this.nPCKillCounter = nPCKillCounter;
		this.missionPools = missionPools;
	}
}
