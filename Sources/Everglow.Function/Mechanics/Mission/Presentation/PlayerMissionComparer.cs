using Everglow.Commons.Mechanics.Mission.Core;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;

namespace Everglow.Commons.Mechanics.Mission.Presentation;

public class PlayerMissionComparer : IComparer<PlayerMissionBase>
{
	public static readonly PlayerMissionComparer Instance = new PlayerMissionComparer();

	public int Compare(PlayerMissionBase x, PlayerMissionBase y)
	{
		if (x.State != y.State)
		{
			return x.State - y.State;
		}
		else if (x.Type != y.Type)
		{
			if (x.Type is MissionType.None)
			{
				return 1;
			}

			if (y.Type is MissionType.None)
			{
				return -1;
			}

			return x.Type - y.Type;
		}
		else
		{
			return string.Compare(x.DisplayName, y.DisplayName);
		}
	}
}
