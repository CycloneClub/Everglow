using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.MissionStructure.Nodes;

/// <summary>
/// Player-mission-only structural objective node.
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

	public abstract void GetObjectivesIcon(MissionIconGroup iconGroup);

	public abstract void GetObjectivesText(List<string> lines);

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
