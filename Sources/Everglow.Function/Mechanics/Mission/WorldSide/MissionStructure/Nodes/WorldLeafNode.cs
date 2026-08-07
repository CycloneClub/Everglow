using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.MissionStructure.Nodes;

/// <summary>
/// Leaf node wrapping a single objective.
/// </summary>
public class WorldLeafNode : WorldObjectiveNodeBase
{
	public WorldObjectiveBase Objective;

	public WorldLeafNode(WorldObjectiveBase obj)
	{
		if (obj is null)
		{
			throw new InvalidDataException("Objective must be not null.");
		}

		Objective = obj;
	}

	public override bool Completed => Objective.Completed;

	public override float Progress => Objective.Progress;

	public override List<WorldObjectiveBase> FindAllEntrances() =>
		Objective.Completed ? [] : [Objective];

	public override void Update() => Objective.Update();

	public override bool CheckCompletion() => Objective.CheckCompletion();

	public override void Complete() => Objective.Complete();

	public override void ResetProgress() => Objective.ResetProgress();

	public override void SaveData(TagCompound tag) => Objective.SaveData(tag);

	public override void LoadData(TagCompound tag) => Objective.LoadData(tag);

	public override void NetSend(BinaryWriter bw) => Objective.NetSend(bw);

	public override void NetReceive(BinaryReader br) => Objective.NetReceive(br);
}
