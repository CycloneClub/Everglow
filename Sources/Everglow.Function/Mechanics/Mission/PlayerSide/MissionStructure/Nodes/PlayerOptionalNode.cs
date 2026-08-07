using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.MissionStructure.Nodes;

public class PlayerOptionalNode : PlayerObjectiveNodeBase
{
	private readonly List<PlayerObjectiveBase> _objectives;

	public PlayerOptionalNode(List<PlayerObjectiveBase> objectives)
	{
		if (objectives.Count == 0)
		{
			throw new InvalidDataException("Objectives must have at least 1 child.");
		}

		_objectives = objectives;
	}

	public override bool Completed => _objectives.Any(o => o.Completed);

	public override float Progress => _objectives.Max(o => o.Progress);

	public override List<PlayerObjectiveBase> FindAllEntrances() => _objectives.Where(o => !o.Completed).ToList();

	public override bool CheckCompletion() => _objectives.Any(o => !o.Completed && o.CheckCompletion());

	public override void Update()
	{
		foreach (var objective in _objectives)
		{
			if (!objective.Completed && !objective.CheckCompletion())
			{
				objective.Update();
			}
		}
	}

	public override void Complete()
	{
		foreach (var objective in _objectives)
		{
			if (!objective.Completed && objective.CheckCompletion())
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
		PlayerMissionBase.SaveObjectives(tag, _objectives);
		SaveCompletionStates(tag, _objectives);
	}

	public override void LoadData(TagCompound tag)
	{
		PlayerMissionBase.LoadObjectives(tag, _objectives, useObjectiveID: false);
		LoadCompletionStates(tag, _objectives);
	}

	public override void GetObjectivesIcon(MissionIconGroup iconGroup)
	{
		foreach (var objective in _objectives)
		{
			objective.GetObjectivesIcon(iconGroup);
		}
	}

	public override void GetObjectivesText(List<string> lines)
	{
		foreach (var objective in _objectives)
		{
			objective.GetObjectivesText(lines);
		}
	}
}
