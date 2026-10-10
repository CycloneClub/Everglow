using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class TestDeltaSyncObjectiveTest
{
	[TestMethod]
	public void ReceiveDelta_MergesContributionsWithoutReversingCompletion()
	{
		WorldObjectiveBase objective = new WorldReachObjective(_ => false, "Reach the destination");
		IDeltaSyncObjective sync = objective;
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(true);
		writer.Write(false);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);

		sync.ReceiveDelta(reader);
		Assert.IsTrue(objective.CheckCompletion());
		sync.ReceiveDelta(reader);
		Assert.IsTrue(objective.CheckCompletion());
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void MainSnapshot_RoundTripsAuthoritativeCompletion(bool reached)
	{
		WorldObjectiveBase authority = new WorldReachObjective(_ => false, "Reach the destination");
		using var contribution = new MemoryStream();
		new BinaryWriter(contribution).Write(reached);
		contribution.Position = 0;
		authority.ReceiveDelta(new BinaryReader(contribution));

		using var stream = new MemoryStream();
		authority.SendMain(new BinaryWriter(stream));
		Assert.AreEqual(1L, stream.Length);
		stream.Position = 0;
		WorldObjectiveBase receiver = new WorldReachObjective(_ => false, "Reach the destination");
		receiver.ReceiveMain(new BinaryReader(stream));
		Assert.AreEqual(reached, receiver.CheckCompletion());
	}

	[TestMethod]
	public void ReceiveMain_ReplacesStaleLocalCompletion()
	{
		WorldObjectiveBase objective = new WorldReachObjective(_ => false, "Reach the destination");
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(true);
		writer.Write(false);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);

		objective.ReceiveDelta(reader);
		Assert.IsTrue(objective.CheckCompletion());
		objective.ReceiveMain(reader);
		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void ReceiveDelta_ConsumesOnlyItsOwnPayload()
	{
		WorldObjectiveBase objective = new WorldReachObjective(_ => false, "Reach the destination");
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(true);
		writer.Write(89);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);

		objective.ReceiveDelta(reader);

		Assert.IsTrue(objective.CheckCompletion());
		Assert.AreEqual(1L, stream.Position);
		Assert.AreEqual(89, reader.ReadInt32());
	}
}
