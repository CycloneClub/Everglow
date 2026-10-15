using Everglow.Commons.Mechanics.Quest.Core;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

public abstract partial class WorldQuestBase
{
	public virtual string Name => GetType().Name;

	public virtual string DisplayName => Name;

	public virtual string Description => string.Empty;

	public virtual string Hint => string.Empty;

	public virtual QuestType Type => QuestType.None;

	public virtual QuestSourceBase Source => QuestSourceBase.Default;

	public List<Item> RewardItems { get; protected set; } = [];

	public virtual int TimeLimit => 0;

	/// <summary>
	/// Represents the quest unlock condition is visible before unlocked.
	/// </summary>
	public virtual bool Visible => true;

	public virtual bool CanUnlock() => true;

	public virtual void Initialize()
	{
	}
}
