using Everglow.Commons.Mechanics.Mission.UI;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide;

public class PlayerMissionSystem : ModSystem
{
	public override void Load()
	{
		PlayerMissionManager.Load();
	}

	public override void Unload()
	{
		PlayerMissionManager.UnLoad();
		MissionContainer.Instance?.Unload();
	}
}
