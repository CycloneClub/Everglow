using Everglow.Commons.Mechanics.Quest.Core;
using Terraria.Localization;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

public abstract partial class PlayerQuestBase : ITagCompoundEntity
{
	/// <summary>
	/// 用于进行内部标识的名字，作用类似 ID
	/// </summary>
	public virtual string Name => GetType().Name;

	public virtual string LocalizationKey => $"Mods.Everglow.Quests.{Name}";

	/// <summary>
	/// 标识当前任务实例的持久化 ID
	/// </summary>
	public string InstanceId { get; private set; } = Guid.NewGuid().ToString("N");

	/// <summary>
	/// 用于外部显示的名字。默认读取 <see cref="LocalizationKey"/> 下的 DisplayName。
	/// </summary>
	public virtual string DisplayName => Text("DisplayName");

	/// <summary>
	/// 任务介绍
	/// </summary>
	public virtual string Description => Text("Description");

	/// <summary>
	/// 任务详情提示
	/// </summary>
	public virtual string Hint => Text("Hint");

	/// <summary>
	/// 任务可接取时的提示遮蔽方式
	/// </summary>
	public virtual QuestHideMode HideMode => QuestHideMode.None;

	/// <summary>
	/// 任务来源
	/// </summary>
	public virtual QuestSourceBase Source { get; private set; } = QuestSourceBase.Default;

	/// <summary>
	/// 次级任务来源
	/// </summary>
	public virtual QuestSourceBase SubSource { get; private set; } = null;

	/// <summary>
	/// 任务类型
	/// </summary>
	public virtual QuestType Type => QuestType.None;

	/// <summary>
	/// 任务奖励物品
	/// </summary>
	public virtual List<Item> RewardItems { get; init; }

	/// <summary>
	/// 是否由任务管理器自动检测完成并提交
	/// </summary>
	public virtual bool AutoComplete => false;

	/// <summary>
	/// 是否可放弃任务
	/// </summary>
	public virtual bool Cancellable => false;

	/// <summary>读取相对于任务本地化路径的文本，缺失时返回空字符串。</summary>
	protected string Text(string key, params object[] args)
	{
		string fullKey = LocalizationKey + "." + key;
		return Language.Exists(fullKey) ? Language.GetTextValue(fullKey, args) : string.Empty;
	}
}
