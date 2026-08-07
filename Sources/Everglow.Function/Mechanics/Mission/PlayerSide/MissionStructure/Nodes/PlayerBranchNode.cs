using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.MissionStructure.Nodes;

public class PlayerBranchNode : PlayerObjectiveNodeBase
{
	private readonly List<List<PlayerObjectiveBase>> _branches;
	private int _selected = -1;
	private int _indexInBranch;

	public PlayerBranchNode(List<List<PlayerObjectiveBase>> branches)
	{
		if (branches.Count == 0 || branches.Any(branch => branch.Count == 0))
		{
			throw new InvalidDataException("Branches must contain at least one objective.");
		}

		_branches = branches;
	}

	public override bool Completed => _selected >= 0 && _indexInBranch >= _branches[_selected].Count;

	public override float Progress
	{
		get
		{
			if (_selected < 0)
			{
				return _branches.Max(branch => branch[0].Progress);
			}

			return Completed ? 1f : _branches[_selected][_indexInBranch].Progress;
		}
	}

	public override List<PlayerObjectiveBase> FindAllEntrances()
	{
		if (_selected < 0)
		{
			return _branches.Select(branch => branch[0]).Where(objective => !objective.Completed).ToList();
		}

		return Completed ? [] : [_branches[_selected][_indexInBranch]];
	}

	public override void Update()
	{
		foreach (var objective in FindAllEntrances())
		{
			if (!objective.CheckCompletion())
			{
				objective.Update();
			}
		}
	}

	public override bool CheckCompletion() => FindAllEntrances().Any(objective => objective.CheckCompletion());

	public override void Complete()
	{
		if (_selected < 0)
		{
			for (int i = 0; i < _branches.Count; i++)
			{
				var objective = _branches[i][0];
				if (!objective.Completed && objective.CheckCompletion())
				{
					objective.Complete();
					_selected = i;
					_indexInBranch = 1;
					return;
				}
			}
		}
		else if (!Completed)
		{
			var objective = _branches[_selected][_indexInBranch];
			if (objective.CheckCompletion())
			{
				objective.Complete();
				_indexInBranch++;
			}
		}
	}

	public override void ResetProgress()
	{
		_selected = -1;
		_indexInBranch = 0;
		foreach (var objective in _branches.SelectMany(branch => branch))
		{
			objective.ResetProgress();
		}
	}

	public override void SaveData(TagCompound tag)
	{
		tag["Selected"] = _selected;
		tag["Index"] = _indexInBranch;
		for (int i = 0; i < _branches.Count; i++)
		{
			PlayerMissionBase.SaveObjectives(tag, _branches[i], i.ToString());
		}
		SaveCompletionStates(tag, _branches.SelectMany(branch => branch));
	}

	public override void LoadData(TagCompound tag)
	{
		_selected = tag.GetInt("Selected");
		_indexInBranch = tag.GetInt("Index");
		for (int i = 0; i < _branches.Count; i++)
		{
			PlayerMissionBase.LoadObjectives(tag, _branches[i], i.ToString(), useObjectiveID: false);
		}
		LoadCompletionStates(tag, _branches.SelectMany(branch => branch));
	}

	internal bool HasValidCursor(TagCompound tag)
	{
		int selected = tag.GetInt("Selected");
		int indexInBranch = tag.GetInt("Index");
		if (selected == -1)
		{
			return indexInBranch == 0;
		}

		return selected >= 0 && selected < _branches.Count
			&& indexInBranch >= 1 && indexInBranch <= _branches[selected].Count;
	}

	public override void GetObjectivesIcon(MissionIconGroup iconGroup)
	{
		foreach (var objective in _branches.SelectMany(branch => branch))
		{
			objective.GetObjectivesIcon(iconGroup);
		}
	}

	public override void GetObjectivesText(List<string> lines)
	{
		for (int branchIndex = 0; branchIndex < _branches.Count; branchIndex++)
		{
			var color = _selected < 0
				? "100,180,120,255"
				: branchIndex == _selected ? "100,255,100,255" : "100,100,100,255";
			foreach (var objective in _branches[branchIndex])
			{
				var objectiveLines = new List<string>();
				objective.GetObjectivesText(objectiveLines);
				lines.AddRange(objectiveLines.Select(line => $"[TextDrawer,Text='(Branch {branchIndex + 1})',Color='{color}'] {line}"));
			}
		}
	}
}
