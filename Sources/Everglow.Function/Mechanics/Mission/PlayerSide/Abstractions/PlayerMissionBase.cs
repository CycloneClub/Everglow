using Everglow.Commons.Mechanics.Mission.PlayerSide.MissionStructure;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;

/// <summary>
/// 任务基类
/// </summary>
/// <remarks>
/// NOTE: 继承后必须保证存在一个无参构造函数
/// </remarks>
public abstract partial class PlayerMissionBase : ITagCompoundEntity
{
	/// <summary>
	/// 任务计时器存储键
	/// </summary>
	public const string TimeSaveKey = "MissionTime";

	/// <summary>
	/// 任务奖励物品来源
	/// </summary>
	public const string RewardItemsSourceContext = "Everglow.MissionSystem";

	protected PlayerMissionBase()
	{
		Objectives = new PlayerStructuralObjectiveContainer();
		RewardItems = [];
		Time = 0;
	}
}
