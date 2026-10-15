using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Structure.Nodes;

/// <summary>
/// Base class for all structural objective nodes.
/// A structural node defines how one or more <see cref="WorldObjectiveBase"/> instances
/// are grouped, updated, evaluated, and completed within the quest structure.
/// </summary>
public abstract class WorldObjectiveNodeBase
{
	/// <summary>
	/// Gets whether this structural node has been fully completed.
	/// Completion semantics depend on the specific node type
	/// (e.g., all objectives completed, any objective completed, selected branch completed, etc.).
	/// </summary>
	public abstract bool Completed { get; }

	/// <summary>
	/// Gets the progress value of this node (0–1).
	/// The meaning of progress depends on the node type
	/// (e.g., average progress, max progress, or single objective progress).
	/// </summary>
	public abstract float Progress { get; }

	/// <summary>
	/// Returns all currently active objectives for this frame.
	/// These represent the "entrances" of the node—objectives that should be updated,
	/// checked for completion, and considered active by the quest system.
	/// </summary>
	/// <returns>A list of active objectives.</returns>
	public abstract List<WorldObjectiveBase> FindAllEntrances();

	/// <summary>
	/// Performs per-frame update logic for this node.
	/// Typically updates all active objectives that are not yet completed
	/// and have not met their completion conditions.
	/// </summary>
	public abstract void Update();

	/// <summary>
	/// Checks whether this node's completion conditions have been met for this frame.
	/// This does not necessarily mean the node is structurally completed—only that
	/// a completion event should be triggered.
	/// </summary>
	/// <returns>True if completion conditions are met; otherwise false.</returns>
	public abstract bool CheckCompletion();

	/// <summary>
	/// Marks this node as structurally completed.
	/// Typically completes all objectives that have met their completion conditions.
	/// </summary>
	public abstract void Complete();

	/// <summary>
	/// Resets this node's progress and completion state.
	/// All child objectives should also reset their internal state.
	/// </summary>
	public abstract void ResetProgress();

	public abstract void GetObjectivesIcon(QuestIconGroup iconGroup);

	/// <summary>
	/// Saves node-specific data into the provided tag compound.
	/// Each node type is responsible for saving its own objective data.
	/// </summary>
	/// <param name="tag">The tag compound to write data into.</param>
	public abstract void SaveData(TagCompound tag);

	/// <summary>
	/// Loads node-specific data from the provided tag compound.
	/// Each node type is responsible for restoring its own objective data.
	/// </summary>
	/// <param name="tag">The tag compound containing saved data.</param>
	public abstract void LoadData(TagCompound tag);

	/// <summary>
	/// Sends node-specific netcode data.
	/// Each node type must serialize its own objective state.
	/// </summary>
	/// <param name="bw">Binary writer used for sending data.</param>
	public abstract void NetSend(BinaryWriter bw);

	/// <summary>
	/// Receives node-specific netcode data.
	/// Each node type must deserialize its own objective state.
	/// </summary>
	/// <param name="br">Binary reader used for receiving data.</param>
	public abstract void NetReceive(BinaryReader br);
}
