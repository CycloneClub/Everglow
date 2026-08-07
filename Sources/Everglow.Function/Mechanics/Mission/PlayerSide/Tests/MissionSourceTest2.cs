using Everglow.Commons.Mechanics.Mission.Core;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Tests;

internal class MissionSourceTest2 : MissionSourceBase
{
	public static readonly MissionSourceTest2 Instance = new MissionSourceTest2();

	private MissionSourceTest2()
	{
	}

	public override Texture2D Texture => ModAsset.Point.Value;

	public override string Name => "测试B";
}
