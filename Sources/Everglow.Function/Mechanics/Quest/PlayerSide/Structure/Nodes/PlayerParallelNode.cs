using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Structure.Nodes;

public class PlayerParallelNode : PlayerObjectiveNodeBase
{
	private readonly List<PlayerObjectiveBase> _objectives;

	public PlayerParallelNode(List<PlayerObjectiveBase> objectives)
	{
		if (objectives.Count == 0)
		{
			throw new InvalidDataException("Objectives must have at least 1 child.");
		}

		_objectives = objectives;
		Objectives = _objectives.AsReadOnly();
	}

	internal IReadOnlyList<PlayerObjectiveBase> Objectives { get; }

	public override bool Completed => _objectives.All(o => o.Completed);

	public override float Progress => _objectives.Average(o => o.Progress);

	public override List<PlayerObjectiveBase> FindAllEntrances() => _objectives.Where(o => !o.Completed).ToList();

	public override bool CheckCompletion() => _objectives.Any(o => o.CanProgress && o.CheckCompletion());

	public override void Update()
	{
		foreach (var objective in _objectives)
		{
			if (objective.CanProgress && !objective.CheckCompletion())
			{
				objective.Update();
			}
		}
	}

	public override void Complete()
	{
		foreach (var objective in _objectives)
		{
			if (objective.CanProgress && objective.CheckCompletion())
			{
				objective.Complete();
			}
		}
	}

	public override void ResetProgress()
	{
		foreach (var objective in _objectives)
		{
			objective.ResetProgress();
		}
	}

	public override void SaveData(TagCompound tag)
	{
		PlayerQuestBase.SaveObjectives(tag, _objectives);
		SaveCompletionStates(tag, _objectives);
	}

	public override void LoadData(TagCompound tag)
	{
		PlayerQuestBase.LoadObjectives(tag, _objectives, useObjectiveID: false);
		LoadCompletionStates(tag, _objectives);
	}

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		foreach (var objective in _objectives)
		{
			objective.GetObjectivesIcon(iconGroup);
		}
	}
}
