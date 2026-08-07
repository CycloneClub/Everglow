using Everglow.Commons.Mechanics.Mission.Core;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

public interface IMissionMetadata
{
	/// <summary>
	/// Internal ID of the mission, used for serialization and identification. Should be unique across all missions.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets the display name associated with the object.
	/// </summary>
	public string DisplayName { get; }

	public string Description { get; }

	/// <summary>
	/// Gets the type of the mission associated with this instance.
	/// </summary>
	public MissionType Type { get; }

	public MissionSourceBase Source { get; }

	public List<Item> RewardItems { get; }

	public int TimeLimit { get; }

	public bool Visible { get; }

	public bool CanUnlock();

	/// <summary>
	/// Called for mission metadata initialization. Use this to set up objectives, rewards, etc.
	/// </summary>
	public void Initialize();
}
