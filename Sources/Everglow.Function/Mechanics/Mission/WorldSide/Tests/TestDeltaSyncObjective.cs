using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Tests;

public class TestDeltaSyncObjective : WorldObjectiveBase, IDeltaSyncObjective
{
	public bool NeedDeltaSync => true;

	public override bool CheckCompletion() => false;

	public override void GetObjectivesText() => throw new NotImplementedException();

	public void ReceiveDelta(BinaryReader br)
	{
		var value = br.ReadInt32();
		// Console.WriteLine(value);
	}

	public void ReceiveMain(BinaryReader br)
	{
		var value = br.ReadInt32();
		// Console.WriteLine(value);
	}

	public void SendDelta(BinaryWriter bw)
	{
		bw.Write(123);
	}

	public void SendMain(BinaryWriter bw)
	{
		bw.Write(456);
	}
}

public class TestDeltaSyncMission : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives.Add(new TestDeltaSyncObjective());
	}
}
