using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class TestDeltaSyncObjectiveTest
{
	[TestMethod]
	public void NeedDeltaSync_RequestsSynchronizationThroughBaseReference()
	{
		WorldObjectiveBase objective = new TestDeltaSyncObjective();

		Assert.IsTrue(objective.NeedDeltaSync);
		Assert.IsTrue(((IDeltaSyncObjective)objective).NeedDeltaSync);
	}

	[TestMethod]
	[DataRow(false, 123)]
	[DataRow(true, 456)]
	public void Send_ThroughBaseReferenceWritesObjectivePayload(bool mainProgress, int expectedValue)
	{
		WorldObjectiveBase objective = new TestDeltaSyncObjective();
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);

		if (mainProgress)
		{
			objective.SendMain(writer);
		}
		else
		{
			objective.SendDelta(writer);
		}

		Assert.AreEqual(sizeof(int), stream.Length);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		Assert.AreEqual(expectedValue, reader.ReadInt32());
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Receive_ThroughBaseReferenceConsumesOnlyObjectivePayload(bool mainProgress)
	{
		WorldObjectiveBase objective = new TestDeltaSyncObjective();
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(37);
		writer.Write(89);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);

		if (mainProgress)
		{
			objective.ReceiveMain(reader);
		}
		else
		{
			objective.ReceiveDelta(reader);
		}

		Assert.AreEqual(sizeof(int), stream.Position);
		Assert.AreEqual(89, reader.ReadInt32());
	}
}
