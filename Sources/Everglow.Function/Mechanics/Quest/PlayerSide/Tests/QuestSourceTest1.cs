using Everglow.Commons.Mechanics.Quest.Core;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

internal class QuestSourceTest1 : QuestSourceBase
{
	public static readonly QuestSourceTest1 Instance = new QuestSourceTest1();

	private QuestSourceTest1()
	{
	}

	public override Texture2D Texture => ModAsset.AnnaTheGuard.Value;

	public override string Name => "测试A";
}
