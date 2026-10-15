using Everglow.Commons.Mechanics.Quest.PlayerSide.Structure;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

/// <summary>
/// 任务基类
/// </summary>
/// <remarks>
/// NOTE: 继承后必须保证存在一个无参构造函数
/// </remarks>
public abstract partial class PlayerQuestBase : ITagCompoundEntity
{
	/// <summary>
	/// 任务计时器存储键
	/// </summary>
	public const string TimeSaveKey = "QuestTime";

	/// <summary>
	/// 任务奖励物品来源
	/// </summary>
	public const string RewardItemsSourceContext = "Everglow.QuestSystem";

	protected PlayerQuestBase()
	{
		Objectives = new PlayerObjectiveContainer();
		RewardItems = [];
		Time = 0;
	}
}
