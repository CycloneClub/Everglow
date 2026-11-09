using Everglow.Commons.Mechanics.Quest.Core;
using Terraria.Localization;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

public abstract partial class WorldQuestBase
{
	public virtual string Name => GetType().Name;

	public virtual string LocalizationKey => $"Mods.Everglow.Quests.{Name}";

	public virtual string DisplayName
	{
		get
		{
			string key = LocalizationKey + ".DisplayName";
			return Language.Exists(key) ? Language.GetTextValue(key) : string.Empty;
		}
	}

	public virtual string Description => string.Empty;

	public virtual string Hint => string.Empty;

	public virtual QuestHideMode HideMode => QuestHideMode.None;

	public virtual QuestType Type => QuestType.None;

	public virtual QuestSourceBase Source => QuestSourceBase.Default;

	public List<Item> RewardItems { get; protected set; } = [];

	public virtual int TimeLimit => 0;

	public virtual bool CanUnlock() => true;

	public virtual void Initialize()
	{
	}
}
