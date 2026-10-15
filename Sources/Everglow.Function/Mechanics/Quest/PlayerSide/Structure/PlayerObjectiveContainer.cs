using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Structure.Nodes;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Structure;

/// <summary>
/// Player-side objective structure and lifecycle owner.
/// </summary>
public class PlayerObjectiveContainer
{
	private const string StructuralObjectivesSaveKey = "StructuralObjectives";

	private readonly List<PlayerObjectiveNodeBase> _nodes = [];
	private readonly List<PlayerObjectiveBase> _objectives = [];
	private readonly List<PlayerObjectiveBase> _activeObjectives = [];

	public PlayerObjectiveBase this[int index] => _objectives[index];

	/// <summary>
	/// All leaf objectives in DSL declaration order.
	/// </summary>
	public List<PlayerObjectiveBase> AllObjectives => _objectives;

	public IReadOnlyList<PlayerObjectiveNodeBase> AllNodes => _nodes;

	/// <summary>
	/// The active structural node, or <see langword="null"/> when the quest is complete.
	/// </summary>
	public PlayerObjectiveNodeBase Current { get; private set; }

	/// <summary>
	/// A compatibility view of the first active leaf objective.
	/// </summary>
	public PlayerObjectiveBase CurrentObjective => _activeObjectives.FirstOrDefault();

	/// <summary>
	/// All currently active leaf objectives. Callers should use this instead of traversing structure nodes.
	/// </summary>
	public IReadOnlyList<PlayerObjectiveBase> ActiveObjectives => _activeObjectives;

	public bool RecoveredInvalidState { get; private set; }

	public bool Completed => FindCurrentNode() is null;

	public float Progress => _nodes.Count == 0 ? 1f : _nodes.Average(node => node.Progress);

	#region DSL

	public PlayerObjectiveContainer Add(PlayerObjectiveBase objective)
	{
		Register(objective);
		_nodes.Add(new PlayerLeafNode(objective));
		return this;
	}

	public PlayerObjectiveContainer AddParallel(params PlayerObjectiveBase[] objectives)
	{
		RegisterRange(objectives);
		_nodes.Add(new PlayerParallelNode(objectives.ToList()));
		return this;
	}

	public PlayerObjectiveContainer AddOptional(params PlayerObjectiveBase[] objectives)
	{
		RegisterRange(objectives);
		_nodes.Add(new PlayerOptionalNode(objectives.ToList()));
		return this;
	}

	public PlayerObjectiveContainer AddBranch(params List<PlayerObjectiveBase>[] branches)
	{
		if (branches is null)
		{
			throw new InvalidDataException("Branches must not be null.");
		}

		foreach (var branch in branches)
		{
			RegisterRange(branch);
		}

		_nodes.Add(new PlayerBranchNode(branches.Select(branch => branch.ToList()).ToList()));
		return this;
	}

	#endregion

	#region Query

	public PlayerObjectiveNodeBase FindCurrentNode() => _nodes.FirstOrDefault(node => !node.Completed);

	/// <summary>
	/// Returns all active leaf objectives without exposing the node representation.
	/// </summary>
	public IEnumerable<PlayerObjectiveBase> FindCurrentObjectives()
	{
		var current = FindCurrentNode();
		return current is null ? [] : current.FindAllEntrances();
	}

	#endregion

	#region Lifecycle

	public void Activate(PlayerQuestBase quest)
	{
		Deactivate();
		Current = FindCurrentNode();
		ActivateCurrent(quest);
	}

	public void Deactivate()
	{
		foreach (var objective in _activeObjectives)
		{
			objective.Deactivate();
		}

		_activeObjectives.Clear();
	}

	/// <summary>
	/// Updates the active node and returns whether at least one leaf objective completed.
	/// </summary>
	public bool Update(PlayerQuestBase quest)
	{
		if (Current is null)
		{
			return false;
		}

		var activeAtStart = _activeObjectives.ToArray();
		Current.Update();
		bool completed = Current.CheckCompletion();
		if (completed)
		{
			Current.Complete();
			Deactivate();
			Current = FindCurrentNode();
			ActivateCurrent(quest);
		}

		foreach (var objective in activeAtStart)
		{
			if (_activeObjectives.Contains(objective)
				&& objective.CanProgress
				&& objective.Timer?.Update(PlayerQuestManager.UpdateInterval) == true)
			{
				objective.Deactivate();
				_activeObjectives.Remove(objective);
			}
		}

		return completed;
	}

	public void ResetProgress()
	{
		Deactivate();
		foreach (var node in _nodes)
		{
			node.ResetProgress();
		}

		Current = FindCurrentNode();
	}

	private void ActivateCurrent(PlayerQuestBase quest)
	{
		if (Current is null)
		{
			return;
		}

		foreach (var objective in Current.FindAllEntrances())
		{
			if (!objective.CanProgress)
			{
				continue;
			}

			objective.Activate(quest);
			_activeObjectives.Add(objective);
		}
	}

	#endregion

	#region Presentation

	public void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		foreach (var node in _nodes)
		{
			node.GetObjectivesIcon(iconGroup);
		}
	}

	#endregion

	#region Persistence

	public void SaveData(TagCompound tag)
	{
		var nodeTags = new List<TagCompound>();
		foreach (var node in _nodes)
		{
			var nodeTag = new TagCompound();
			node.SaveData(nodeTag);
			nodeTags.Add(nodeTag);
		}

		tag.Add(StructuralObjectivesSaveKey, nodeTags);
	}

	public void LoadData(TagCompound tag)
	{
		RecoveredInvalidState = false;
		if (tag.TryGet<IList<TagCompound>>(StructuralObjectivesSaveKey, out var nodeTags))
		{
			for (int i = 0; i < nodeTags.Count && i < _nodes.Count; i++)
			{
				if (_nodes[i] is PlayerBranchNode branch && !branch.HasValidCursor(nodeTags[i]))
				{
					RecoveredInvalidState = true;
					Current = FindCurrentNode();
					return;
				}
			}

			for (int i = 0; i < nodeTags.Count && i < _nodes.Count; i++)
			{
				_nodes[i].LoadData(nodeTags[i]);
			}
		}
		else
		{
			// Flat saves created before structural nodes map directly to declaration-order leaves.
			PlayerQuestBase.LoadObjectives(tag, _objectives);
		}

		Current = FindCurrentNode();
	}

	#endregion

	private void RegisterRange(IEnumerable<PlayerObjectiveBase> objectives)
	{
		if (objectives is null)
		{
			throw new InvalidDataException("Objectives must not be null.");
		}

		foreach (var objective in objectives)
		{
			Register(objective);
		}
	}

	private void Register(PlayerObjectiveBase objective)
	{
		if (objective is null)
		{
			throw new InvalidDataException("Objective must not be null.");
		}

		objective.ObjectiveID = _objectives.Count;
		objective.OnInitialize();
		_objectives.Add(objective);
	}
}
