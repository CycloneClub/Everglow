using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.MissionStructure.Nodes;

public class WorldBranchNode : WorldObjectiveNodeBase
{
	private readonly List<List<WorldObjectiveBase>> _branches;
	private int _selected = -1;      // Selected branch index (-1 = not selected yet)
	private int _indexInBranch = 0;  // Current objective index inside selected branch

	internal bool InvalidStateDetected { get; private set; }

	public WorldBranchNode(List<List<WorldObjectiveBase>> branches)
	{
		if (branches.Count == 0)
		{
			throw new InvalidDataException("Branches cannot be empty.");
		}

		foreach (var branch in branches)
		{
			if (branch.Count < 1)
			{
				throw new InvalidDataException("Each branch must contain at least one objective.");
			}
		}

		_branches = branches;
	}

	public override bool Completed =>
		_selected >= 0 &&
		_indexInBranch >= _branches[_selected].Count; // Completed when selected branch is fully consumed

	public override float Progress
	{
		get
		{
			if (_selected < 0)
			{
				// Before selection: progress = max head progress
				return _branches.Max(b => b[0].Progress);
			}

			var branch = _branches[_selected];
			if (_indexInBranch >= branch.Count)
			{
				return 1f;
			}

			return branch[_indexInBranch].Progress; // Progress of current objective in selected branch
		}
	}

	public override List<WorldObjectiveBase> FindAllEntrances()
	{
		if (_selected < 0)
		{
			// Before selection: all branch heads are active
			return _branches.Select(b => b[0]).ToList();
		}

		var branch = _branches[_selected];
		if (_indexInBranch < branch.Count)
		{
			// After selection: only current objective is active
			return new List<WorldObjectiveBase> { branch[_indexInBranch] };
		}

		return [];
	}

	public override void Update()
	{
		if (_selected < 0)
		{
			// Update all branch heads until one completes
			foreach (var b in _branches)
			{
				var head = b[0];
				if (!head.Completed && !head.CheckCompletion())
				{
					head.Update();
				}
			}
		}
		else
		{
			// Update current objective in selected branch
			var branch = _branches[_selected];
			if (_indexInBranch < branch.Count)
			{
				var obj = branch[_indexInBranch];
				if (!obj.Completed && !obj.CheckCompletion())
				{
					obj.Update();
				}
			}
		}
	}

	public override bool CheckCompletion()
	{
		if (_selected < 0)
		{
			// Before selection: any head completing triggers selection
			return _branches.Any(b => !b[0].Completed && b[0].CheckCompletion());
		}

		var branch = _branches[_selected];
		if (_indexInBranch >= branch.Count)
		{
			return true; // Branch fully completed
		}

		return branch[_indexInBranch].CheckCompletion(); // Check current objective
	}

	public override void Complete()
	{
		if (_selected < 0)
		{
			// Select the branch whose head completed
			for (int i = 0; i < _branches.Count; i++)
			{
				var head = _branches[i][0];
				if (!head.Completed && head.CheckCompletion())
				{
					head.Complete();
					_selected = i;
					_indexInBranch = 1; // Move to next objective in selected branch
					return;
				}
			}
		}
		else
		{
			// Complete current objective in selected branch
			var branch = _branches[_selected];
			if (_indexInBranch < branch.Count)
			{
				var obj = branch[_indexInBranch];
				if (obj.CheckCompletion())
				{
					obj.Complete();
					_indexInBranch++; // Advance to next objective
				}
			}
		}
	}

	public override void ResetProgress()
	{
		_selected = -1;
		_indexInBranch = 0;

		// Reset all objectives in all branches
		foreach (var branch in _branches)
		{
			foreach (var obj in branch)
			{
				obj.ResetProgress();
			}
		}
	}

	public override void SaveData(TagCompound tag)
	{
		tag["Selected"] = _selected;
		tag["Index"] = _indexInBranch;

		// Save each objective's data
		for (int i = 0; i < _branches.Count; i++)
		{
			for (int j = 0; j < _branches[i].Count; j++)
			{
				var t = new TagCompound();
				_branches[i][j].SaveData(t);
				tag[$"{i}_{j}"] = t;
			}
		}
	}

	public override void LoadData(TagCompound tag)
	{
		int selected = tag.GetInt("Selected");
		int indexInBranch = tag.GetInt("Index");

		// Load each objective's data
		for (int i = 0; i < _branches.Count; i++)
		{
			for (int j = 0; j < _branches[i].Count; j++)
			{
				if (tag.TryGet<TagCompound>($"{i}_{j}", out var t))
				{
					_branches[i][j].LoadData(t);
				}
			}
		}

		RestoreCursorOrReset(selected, indexInBranch);
	}

	internal bool HasValidCursor(TagCompound tag) => IsCursorValid(tag.GetInt("Selected"), tag.GetInt("Index"));

	public override void NetSend(BinaryWriter bw)
	{
		bw.Write(_selected);
		bw.Write(_indexInBranch);

		// Send all objectives in all branches
		foreach (var branch in _branches)
		{
			foreach (var obj in branch)
			{
				obj.NetSend(bw);
			}
		}
	}

	public override void NetReceive(BinaryReader br)
	{
		int selected = br.ReadInt32();
		int indexInBranch = br.ReadInt32();

		// Receive all objectives in all branches
		foreach (var branch in _branches)
		{
			foreach (var obj in branch)
			{
				obj.NetReceive(br);
			}
		}

		RestoreCursorOrReset(selected, indexInBranch);
	}

	private void RestoreCursorOrReset(int selected, int indexInBranch)
	{
		if (IsCursorValid(selected, indexInBranch))
		{
			_selected = selected;
			_indexInBranch = indexInBranch;
			return;
		}

		InvalidStateDetected = true;
		ResetProgress();
	}

	internal void ClearInvalidState() => InvalidStateDetected = false;

	private bool IsCursorValid(int selected, int indexInBranch)
	{
		if (selected == -1)
		{
			return indexInBranch == 0;
		}

		return selected >= 0 && selected < _branches.Count
			&& indexInBranch >= 1 && indexInBranch <= _branches[selected].Count;
	}
}
