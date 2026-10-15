using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Structure.Nodes;

/// <summary>
/// Player-quest-only structural objective node.
/// </summary>
public abstract class PlayerObjectiveNodeBase
{
	protected const string StructuralCompletionStateSaveKey = "StructuralCompletionState";

	public abstract bool Completed { get; }

	public abstract float Progress { get; }

	public abstract List<PlayerObjectiveBase> FindAllEntrances();

	public abstract void Update();

	public abstract bool CheckCompletion();

	public abstract void Complete();

	public abstract void ResetProgress();

	public abstract void SaveData(TagCompound tag);

	public abstract void LoadData(TagCompound tag);

	public abstract void GetObjectivesIcon(QuestIconGroup iconGroup);

	protected static void SaveCompletionStates(TagCompound tag, IEnumerable<PlayerObjectiveBase> objectives)
	{
		tag.Add(StructuralCompletionStateSaveKey, objectives.Select(objective => objective.Completed ? 1 : 0).ToList());
	}

	protected static void LoadCompletionStates(TagCompound tag, IEnumerable<PlayerObjectiveBase> objectives)
	{
		if (!tag.TryGet<IList<int>>(StructuralCompletionStateSaveKey, out var states))
		{
			return;
		}

		int index = 0;
		foreach (var objective in objectives)
		{
			if (index >= states.Count)
			{
				break;
			}

			objective.RestoreStructuralCompletionState(states[index++] != 0);
		}
	}
}
