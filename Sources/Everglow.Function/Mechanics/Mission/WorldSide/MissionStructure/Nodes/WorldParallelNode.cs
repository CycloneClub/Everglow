using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.MissionStructure.Nodes;

public class WorldParallelNode : WorldObjectiveNodeBase
{
	public WorldParallelNode(List<WorldObjectiveBase> objectives)
	{
		if (objectives.Count == 0)
		{
			throw new InvalidDataException("Objectives must have at least 1 children.");
		}

		_objectives = objectives;
	}

	private readonly List<WorldObjectiveBase> _objectives;

	public override bool Completed => _objectives.All(o => o.Completed);

	public override float Progress => _objectives.Count > 0
		? _objectives.Average(o => o.Progress)
		: 1f;

	public override List<WorldObjectiveBase> FindAllEntrances() => _objectives.Where(o => !o.Completed).ToList();

	public override bool CheckCompletion() => _objectives.Any(o => !o.Completed && o.CheckCompletion());

	public override void Update()
	{
		foreach (var o in _objectives)
		{
			if (!o.Completed && !o.CheckCompletion())
			{
				o.Update();
			}
		}
	}

	public override void Complete()
	{
		foreach (var o in _objectives)
		{
			if (!o.Completed && o.CheckCompletion())
			{
				o.Complete();
			}
		}
	}

	public override void ResetProgress()
	{
		foreach (var o in _objectives)
		{
			o.ResetProgress();
		}
	}

	public override void SaveData(TagCompound tag)
	{
		for (int i = 0; i < _objectives.Count; i++)
		{
			var o = _objectives[i];
			var ot = new TagCompound();
			o.SaveData(ot);
			tag.Add(i.ToString(), ot);
		}
	}

	public override void LoadData(TagCompound tag)
	{
		for (int i = 0; i < _objectives.Count; i++)
		{
			if (tag.TryGet<TagCompound>(i.ToString(), out var oTag))
			{
				var o = _objectives[i];
				o.LoadData(oTag);
			}
		}
	}

	public override void NetSend(BinaryWriter bw)
	{
		foreach (var o in _objectives)
		{
			o.NetSend(bw);
		}
	}

	public override void NetReceive(BinaryReader br)
	{
		foreach (var o in _objectives)
		{
			o.NetReceive(br);
		}
	}
}
