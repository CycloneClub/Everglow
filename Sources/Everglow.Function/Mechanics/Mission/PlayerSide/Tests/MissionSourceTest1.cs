using Everglow.Commons.Mechanics.Mission.Core;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Tests;

internal class MissionSourceTest1 : MissionSourceBase
{
	public static readonly MissionSourceTest1 Instance = new MissionSourceTest1();

	private MissionSourceTest1()
	{
	}

	public override Texture2D Texture => ModAsset.AnnaTheGuard.Value;

	public override string Name => "测试A";
}
