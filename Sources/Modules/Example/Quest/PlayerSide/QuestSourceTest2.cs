using Everglow.Commons.Mechanics.Quest.Core;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

internal class QuestSourceTest2 : QuestSourceBase
{
	public static readonly QuestSourceTest2 Instance = new QuestSourceTest2();

	private QuestSourceTest2()
	{
	}

	public override Texture2D Texture => ModAsset.Point.Value;

	public override string Name => "测试B";
}
