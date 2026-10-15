using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Structure.Nodes;

/// <summary>
/// Leaf node wrapping a single objective.
/// </summary>
public class WorldLeafNode : WorldObjectiveNodeBase
{
	public WorldLeafNode(WorldObjectiveBase obj)
	{
		if (obj is null)
		{
			throw new InvalidDataException("Objective must be not null.");
		}

		Objective = obj;
	}

	internal WorldObjectiveBase Objective { get; }

	public override bool Completed => Objective.Completed;

	public override float Progress => Objective.Progress;

	public override List<WorldObjectiveBase> FindAllEntrances() =>
		Objective.Completed ? [] : [Objective];

	public override void Update()
	{
		if (Objective.CanProgress)
		{
			Objective.Update();
		}
	}

	public override bool CheckCompletion() => Objective.CanProgress && Objective.CheckCompletion();

	public override void Complete() => Objective.Complete();

	public override void ResetProgress() => Objective.ResetProgress();

	public override void GetObjectivesIcon(QuestIconGroup iconGroup) => Objective.GetObjectivesIcon(iconGroup);

	public override void SaveData(TagCompound tag) => Objective.SaveData(tag);

	public override void LoadData(TagCompound tag) => Objective.LoadData(tag);

	public override void NetSend(BinaryWriter bw) => Objective.NetSend(bw);

	public override void NetReceive(BinaryReader br) => Objective.NetReceive(br);
}
