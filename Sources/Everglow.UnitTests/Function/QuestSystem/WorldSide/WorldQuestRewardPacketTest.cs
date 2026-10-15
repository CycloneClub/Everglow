using Everglow.Commons.Mechanics.Quest.WorldSide.Packets;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class WorldQuestRewardPacketTest
{
	[TestMethod]
	public void ObjectiveRetryRequest_RoundTripsOnlyQuestNameAndObjectiveId()
	{
		var sent = new ObjectiveRetryRequestPacket("TimedQuest", 7);
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);

		sent.Send(writer);
		writer.Flush();
		stream.Position = 0;
		var received = new ObjectiveRetryRequestPacket();
		using var reader = new BinaryReader(stream);
		received.Receive(reader, whoAmI: 23);

		Assert.AreEqual("TimedQuest", received.QuestName);
		Assert.AreEqual(7, received.ObjectiveId);
		Assert.AreEqual(stream.Length, stream.Position);
	}

	[TestMethod]
	public void ClaimRequest_RoundTripsOnlyQuestName()
	{
		var sent = new QuestClaimRewardPacket("RewardQuest");
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);

		sent.Send(writer);
		writer.Flush();
		stream.Position = 0;
		using var payloadReader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		Assert.AreEqual("RewardQuest", payloadReader.ReadString());
		Assert.AreEqual(stream.Length, stream.Position);

		stream.Position = 0;
		var received = new QuestClaimRewardPacket();
		using var receiveReader = new BinaryReader(stream);
		received.Receive(receiveReader, whoAmI: 23);
		Assert.AreEqual("RewardQuest", received.QuestName);
	}

	[TestMethod]
	public void Grant_RoundTripsDeliveryTarget()
	{
		var sent = new QuestGiveRewardPacket("RewardQuest", 4, "Alice");
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		sent.Send(writer);
		writer.Flush();
		stream.Position = 0;
		var received = new QuestGiveRewardPacket();
		using var reader = new BinaryReader(stream);

		received.Receive(reader, whoAmI: -1);

		Assert.AreEqual("RewardQuest", received.QuestName);
		Assert.AreEqual(4, received.PlayerWhoAmI);
		Assert.AreEqual("Alice", received.ExpectedPlayerName);
		Assert.AreEqual(stream.Length, stream.Position);
	}

}
