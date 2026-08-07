using Everglow.Commons.Mechanics.Mission.Core;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;

public abstract partial class PlayerMissionBase : ITagCompoundEntity
{
	/// <summary>
	/// 用于进行内部标识的名字，作用类似 ID
	/// </summary>
	public virtual string Name => GetType().Name;

	/// <summary>
	/// 用于外部显示的名字
	/// </summary>
	public abstract string DisplayName { get; }

	/// <summary>
	/// 任务介绍
	/// </summary>
	public virtual string Description { get; } = string.Empty;

	/// <summary>
	/// 任务来源
	/// </summary>
	public virtual MissionSourceBase Source { get; private set; } = MissionSourceBase.Default;

	/// <summary>
	/// 次级任务来源
	/// </summary>
	public virtual MissionSourceBase SubSource { get; private set; } = null;

	/// <summary>
	/// 任务类型
	/// </summary>
	public virtual MissionType Type => MissionType.None;

	/// <summary>
	/// 任务奖励物品
	/// </summary>
	public virtual List<Item> RewardItems { get; init; }

	/// <summary>
	/// 是否显示在任务列表中
	/// </summary>
	public virtual bool IsVisible { get; set; } = true;

	/// <summary>
	/// 是否由任务管理器自动检测完成并提交
	/// </summary>
	public virtual bool AutoComplete => false;

	/// <summary>
	/// 是否可放弃任务
	/// </summary>
	public virtual bool Cancellable => false;

}
