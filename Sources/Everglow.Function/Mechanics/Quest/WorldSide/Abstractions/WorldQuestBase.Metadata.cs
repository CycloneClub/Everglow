using Everglow.Commons.Mechanics.Quest.Core;
using Terraria.Localization;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

public abstract partial class WorldQuestBase
{
	public virtual string Name => GetType().Name;

	public virtual string LocalizationKey => $"Mods.Everglow.Quests.{Name}";

	public virtual string DisplayName => Text("DisplayName");

	public virtual string Description => Text("Description");

	public virtual string Hint => Text("Hint");

	public virtual QuestHideMode HideMode => QuestHideMode.None;

	public virtual QuestType Type => QuestType.None;

	public virtual QuestSourceBase Source => QuestSourceBase.Default;

	public List<Item> RewardItems { get; protected set; } = [];

	public virtual int TimeLimit => 0;

	public virtual bool CanUnlock() => true;

	public virtual void Initialize()
	{
	}

	/// <summary>读取相对于任务本地化路径的文本，缺失时返回空字符串。</summary>
	protected string Text(string key, params object[] args)
	{
		string fullKey = LocalizationKey + "." + key;
		return Language.Exists(fullKey) ? Language.GetTextValue(fullKey, args) : string.Empty;
	}
}
