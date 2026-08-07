using Everglow.Commons.Mechanics.Mission.Core;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

public abstract partial class WorldMissionBase : IMissionMetadata
{
	public virtual string Name => GetType().Name;

	public virtual string DisplayName => Name;

	public string Description { get; }

	public MissionType Type { get; }

	public MissionSourceBase Source { get; }

	public List<Item> RewardItems { get; protected set; }

	public virtual int TimeLimit => 0;

	/// <summary>
	/// Represents the mission unlock condition is visible before unlocked.
	/// </summary>
	public virtual bool Visible => true;

	public virtual bool CanUnlock() => true;

	public virtual void Initialize()
	{
	}
}
