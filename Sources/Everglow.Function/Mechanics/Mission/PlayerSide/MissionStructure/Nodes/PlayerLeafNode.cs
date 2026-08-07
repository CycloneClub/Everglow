using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.MissionStructure.Nodes;

public class PlayerLeafNode : PlayerObjectiveNodeBase
{
	public PlayerLeafNode(PlayerObjectiveBase objective)
	{
		Objective = objective ?? throw new InvalidDataException("Objective must not be null.");
	}

	public PlayerObjectiveBase Objective { get; }

	public override bool Completed => Objective.Completed;

	public override float Progress => Objective.Progress;

	public override List<PlayerObjectiveBase> FindAllEntrances() => Objective.Completed ? [] : [Objective];

	public override void Update() => Objective.Update();

	public override bool CheckCompletion() => !Objective.Completed && Objective.CheckCompletion();

	public override void Complete() => Objective.Complete();

	public override void ResetProgress() => Objective.ResetProgress();

	public override void SaveData(TagCompound tag)
	{
		Objective.SaveData(tag);
		tag.Add(StructuralCompletionStateSaveKey, Objective.Completed ? 1 : 0);
	}

	public override void LoadData(TagCompound tag)
	{
		Objective.LoadData(tag);
		if (tag.TryGet<int>(StructuralCompletionStateSaveKey, out var completed))
		{
			Objective.RestoreStructuralCompletionState(completed != 0);
		}
	}

	public override void GetObjectivesIcon(MissionIconGroup iconGroup) => Objective.GetObjectivesIcon(iconGroup);

	public override void GetObjectivesText(List<string> lines) => Objective.GetObjectivesText(lines);
}
