using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

public class TestDeltaSyncObjective : WorldObjectiveBase, IDeltaSyncObjective
{
	public bool NeedDeltaSync => true;

	public override bool CheckCompletion() => false;

	public override string GetObjectiveText() => string.Empty;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
	}

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

public class TestDeltaSyncQuest : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives.Add(new TestDeltaSyncObjective());
	}
}
