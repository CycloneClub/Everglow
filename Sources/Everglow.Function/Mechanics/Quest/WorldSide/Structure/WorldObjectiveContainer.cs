using System.Diagnostics.CodeAnalysis;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Structure.Nodes;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Structure;

/// <summary>
/// Represents a linear structural objective container.
/// Manages structural nodes, objective lifecycle transitions, persistence, and netcode.
/// </summary>
public class WorldObjectiveContainer
{
	/// <summary>
	/// Fired when the current structural node requires completion.
	/// </summary>
	public event Action<WorldObjectiveNodeBase> OnNodeCompleted;

	/// <summary>
	/// Fired when a node becomes the active node.
	/// </summary>
	public event Action<WorldObjectiveNodeBase> OnObjectiveActivated;

	/// <summary>
	/// Fired when the previous activated node is no longer active.
	/// </summary>
	public event Action OnObjectiveDeactivated;

	/// <summary>
	/// Fired when an objective requires delta-sync transmission.
	/// </summary>
	public event Action<IDeltaSyncObjective> OnMPSyncTriggered;

	/// <summary>
	/// Fired when the active node changes due to snapshot synchronization.
	/// </summary>
	public event Action<WorldObjectiveNodeBase> OnObjectiveSynced;

	/// <summary>
	/// Fired when an active objective reaches its time limit.
	/// </summary>
	public event Action<WorldObjectiveBase> OnObjectiveTimedOut;

	private readonly List<WorldObjectiveNodeBase> _nodes = [];
	private readonly List<WorldObjectiveBase> _objectives = [];

	/// <summary>
	/// Gets all structural nodes in linear order.
	/// </summary>
	public IReadOnlyList<WorldObjectiveNodeBase> AllNodes => _nodes;

	/// <summary>
	/// Gets all objectives in the order they were added.
	/// </summary>
	public IReadOnlyList<WorldObjectiveBase> AllObjectives => _objectives;

	/// <summary>
	/// Gets the currently active structural node.
	/// </summary>
	public WorldObjectiveNodeBase Current { get; private set; }

	public bool RecoveredInvalidState { get; private set; }

	public float Progress => AllNodes.Average(n => n.Progress);

	/// <summary>
	/// Gets whether all structural nodes have been completed.
	/// </summary>
	public bool Completed => FindCurrentNode() == null;

	#region DSL

	/// <summary>
	/// Adds a single objective and wraps it in a leaf node.
	/// </summary>
	/// <param name="objective">The objective to add.</param>
	/// <returns>The current container instance.</returns>
	public WorldObjectiveContainer Add(WorldObjectiveBase objective)
	{
		if (objective is null)
		{
			throw new InvalidDataException("Input must not be null.");
		}

		objective.ObjectiveID = _objectives.Count;
		_objectives.Add(objective);

		var node = new WorldLeafNode(objective);
		_nodes.Add(node);
		return this;
	}

	/// <summary>
	/// Adds a parallel structural node containing multiple objectives.
	/// All objectives must be completed for the node to finish.
	/// </summary>
	/// <param name="objectives">The objectives to include in the parallel node.</param>
	/// <returns>The current container instance.</returns>
	public WorldObjectiveContainer AddParallel(params WorldObjectiveBase[] objectives)
	{
		if (objectives is null)
		{
			throw new InvalidDataException("Input must not be null.");
		}

		foreach (var o in objectives)
		{
			o.ObjectiveID = _objectives.Count;
			_objectives.Add(o);
		}

		var node = new WorldParallelNode(objectives.ToList());
		_nodes.Add(node);
		return this;
	}

	/// <summary>
	/// Adds an optional structural node containing multiple objectives.
	/// Any single objective completing will complete the node.
	/// </summary>
	/// <param name="objectives">The objectives to include in the optional node.</param>
	/// <returns>The current container instance.</returns>
	public WorldObjectiveContainer AddOptional(params WorldObjectiveBase[] objectives)
	{
		if (objectives is null)
		{
			throw new InvalidDataException("Input must not be null.");
		}

		foreach (var o in objectives)
		{
			o.ObjectiveID = _objectives.Count;
			_objectives.Add(o);
		}

		var node = new WorldOptionalNode(objectives.ToList());
		_nodes.Add(node);
		return this;
	}

	/// <summary>
	/// Adds a branching structural node. Each list represents a branch.
	/// The first objective of each branch is active simultaneously.
	/// Completing any branch head locks the branch and continues along that branch.
	/// </summary>
	/// <param name="branches">The branches to add, each represented as a list of objectives.</param>
	/// <returns>The current container instance.</returns>
	public WorldObjectiveContainer AddBranch(params List<WorldObjectiveBase>[] branches)
	{
		if (branches is null)
		{
			throw new InvalidDataException("Input must not be null.");
		}

		foreach (var branch in branches)
		{
			foreach (var o in branch)
			{
				o.ObjectiveID = _objectives.Count;
				_objectives.Add(o);
			}
		}

		var node = new WorldBranchNode(branches.Select(b => b.ToList()).ToList());
		_nodes.Add(node);
		return this;
	}

	#endregion

	#region Query

	/// <summary>
	/// Finds the first structural node that has not yet been completed.
	/// </summary>
	/// <returns>The current active node, or null if all nodes are completed.</returns>
	public WorldObjectiveNodeBase FindCurrentNode()
	{
		foreach (var node in _nodes)
		{
			if (!node.Completed)
			{
				return node;
			}
		}
		return null;
	}

	/// <summary>
	/// Returns all active objectives of the current node for this frame.
	/// </summary>
	/// <returns>A collection of active objectives.</returns>
	public IEnumerable<WorldObjectiveBase> FindCurrentObjectives()
	{
		var cur = FindCurrentNode();
		if (cur != null && !cur.Completed)
		{
			return cur.FindAllEntrances();
		}

		return [];
	}

	#endregion

	#region Presentation

	public void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		foreach (WorldObjectiveNodeBase node in _nodes)
		{
			node.GetObjectivesIcon(iconGroup);
		}
	}

	#endregion

	#region Lifecycle

	/// <summary>
	/// Activates the current node and fires activation events.
	/// </summary>
	public void Activate()
	{
		Current = FindCurrentNode();
		OnObjectiveActivated.Invoke(Current);
	}

	/// <summary>
	/// Deactivates the current node and fires deactivation events.
	/// </summary>
	public void Deactivate()
	{
		OnObjectiveDeactivated.Invoke();
	}

	/// <summary>
	/// Forces a transition to the next active node.
	/// </summary>
	private void CheckActiveNode()
	{
		OnObjectiveDeactivated.Invoke();
		Current = FindCurrentNode();
		OnObjectiveActivated.Invoke(Current);
	}

	/// <summary>
	/// Applies a snapshot-based lifecycle correction without executing completion logic.
	/// Used for multiplayer synchronization.
	/// </summary>
	/// <param name="activeBefore">Whether the node was active before the snapshot.</param>
	/// <param name="activeAfter">Whether the node should be active after the snapshot.</param>
	public void ApplyObjectiveSnapshot(bool activeBefore, bool activeAfter)
	{
		if (Current == FindCurrentNode())
		{
			if (activeBefore == activeAfter)
			{
				return;
			}

			if (!activeBefore && activeAfter)
			{
				OnObjectiveActivated.Invoke(Current);
			}
			else if (activeBefore && !activeAfter)
			{
				OnObjectiveDeactivated.Invoke();
			}
		}
		else
		{
			var oldObjective = Current;
			Current = FindCurrentNode();
			OnObjectiveSynced?.Invoke(Current);

			if (activeBefore && activeAfter)
			{
				OnObjectiveDeactivated.Invoke();
				OnObjectiveActivated.Invoke(Current);
			}
			else if (!activeBefore && activeAfter)
			{
				OnObjectiveActivated.Invoke(Current);
			}
			else if (activeBefore && !activeAfter)
			{
				OnObjectiveDeactivated.Invoke();
			}
		}
	}

	/// <summary>
	/// Updates the active node and checks for completion conditions.
	/// </summary>
	public void UpdateNode()
	{
		if (Current.Completed)
		{
			CheckActiveNode();
			return;
		}

		var activeAtStart = Current.FindAllEntrances()
			.Where(objective => objective.CanProgress)
			.ToArray();
		Current.Update();

		if (Current.CheckCompletion())
		{
			CompleteNode();
		}

		IEnumerable<WorldObjectiveBase> activeAfterUpdate = FindCurrentObjectives();
		foreach (WorldObjectiveBase objective in activeAtStart)
		{
			if (activeAfterUpdate.Contains(objective)
				&& objective.CanProgress
				&& objective.Timer?.Update(WorldQuestManager.UpdateInterval) == true)
			{
				if (objective.NeedDeltaSync)
				{
					OnMPSyncTriggered?.Invoke(objective);
				}

				OnObjectiveTimedOut?.Invoke(objective);
			}
		}
	}

	/// <summary>
	/// Completes the current node and fires completion events.
	/// </summary>
	public void CompleteNode()
	{
		if (CompleteNodeCore())
		{
			OnNodeCompleted.Invoke(Current);
		}
	}

	/// <summary>
	/// Core structural completion logic.
	/// </summary>
	/// <returns>True if the node was completed; otherwise false.</returns>
	private bool CompleteNodeCore()
	{
		if (Current is null)
		{
			return false;
		}

		Current.Complete();
		OnObjectiveDeactivated.Invoke();

		Current = FindCurrentNode();
		OnObjectiveActivated.Invoke(Current);

		return true;
	}

	/// <summary>
	/// Resets all nodes and restores the first node as active.
	/// </summary>
	public void ResetProgress()
	{
		Current = AllNodes[0];
		foreach (var node in AllNodes)
		{
			node.ResetProgress();
		}
	}

	#endregion

	#region Persistence

	private const string ObjectivesSaveKey = "Objectives";

	/// <summary>
	/// Loads objective data for each structural node.
	/// </summary>
	/// <param name="tag">The tag compound containing saved data.</param>
	public void LoadData(TagCompound tag)
	{
		RecoveredInvalidState = false;
		if (tag.TryGet<IList<TagCompound>>(ObjectivesSaveKey, out var oTags))
		{
			for (int i = 0; i < oTags.Count && i < AllNodes.Count; i++)
			{
				if (AllNodes[i] is WorldBranchNode branch && !branch.HasValidCursor(oTags[i]))
				{
					RecoveredInvalidState = true;
					return;
				}
			}

			for (int i = 0; i < oTags.Count && i < AllNodes.Count; i++)
			{
				AllNodes[i].LoadData(oTags[i]);
			}
		}
	}

	/// <summary>
	/// Saves objective data for each structural node.
	/// </summary>
	/// <param name="tag">The tag compound to write data into.</param>
	public void SaveData(TagCompound tag)
	{
		var oTags = new List<TagCompound>();
		foreach (var o in AllNodes)
		{
			var ot = new TagCompound();
			o.SaveData(ot);
			oTags.Add(ot);
		}
		tag.Add(ObjectivesSaveKey, oTags);
	}

	#endregion

	#region Netcode

	/// <summary>
	/// Sends netcode data for all structural nodes.
	/// </summary>
	/// <param name="bw">The binary writer used for sending data.</param>
	public void NetSend(BinaryWriter bw)
	{
		foreach (var node in AllNodes)
		{
			node.NetSend(bw);
		}
	}

	/// <summary>
	/// Receives netcode data for all structural nodes.
	/// </summary>
	/// <param name="br">The binary reader used for receiving data.</param>
	public void NetReceive(BinaryReader br)
	{
		BeginStateRestore();
		foreach (var node in AllNodes)
		{
			node.NetReceive(br);
		}
		CompleteStateRestore();
	}

	private void BeginStateRestore()
	{
		RecoveredInvalidState = false;
		foreach (var branch in AllNodes.OfType<WorldBranchNode>())
		{
			branch.ClearInvalidState();
		}
	}

	private void CompleteStateRestore()
	{
		RecoveredInvalidState = AllNodes.OfType<WorldBranchNode>().Any(branch => branch.InvalidStateDetected);
	}

	/// <summary>
	/// Triggers delta-sync events for all active objectives that require synchronization.
	/// </summary>
	public void OnMPSync()
	{
		foreach (var current in FindCurrentObjectives())
		{
			if (current.CanProgress
				&& current is IDeltaSyncObjective deltaSync
				&& deltaSync.NeedDeltaSync)
			{
				OnMPSyncTriggered.Invoke(deltaSync);
			}
		}
	}

	#endregion
}
