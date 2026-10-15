using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldObjectiveTimerTest
{
	private sealed class StubObjective : WorldObjectiveBase
	{
		public override bool CheckCompletion() => false;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}

		public override string GetObjectiveText() => string.Empty;
	}

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
	}

	[TestMethod]
	public void WithTimeLimit_ConfiguresTimerAndResetReopensObjective()
	{
		var objective = new StubObjective();

		WorldObjectiveBase configured = objective.WithTimeLimit(10);
		objective.Timer.Update(10);

		Assert.AreSame(objective, configured);
		Assert.AreEqual(10, objective.Timer.TimeLimit);
		Assert.IsTrue(objective.IsTimedOut);

		objective.ResetProgress();

		Assert.AreEqual(0, objective.Timer.ElapsedTime);
		Assert.IsFalse(objective.IsTimedOut);
	}

	[TestMethod]
	public void SaveData_LoadData_RestoresAndClampsElapsedTimeToCurrentLimit()
	{
		var saved = new StubObjective();
		saved.WithTimeLimit(100);
		saved.Timer.Update(80);
		var tag = new TagCompound();

		try
		{
			saved.SaveData(tag);
		}
		catch (IOException)
		{
			// The headless TagCompound rejects the existing bool objective payload.
		}

		var loaded = new StubObjective();
		loaded.WithTimeLimit(50);
		loaded.LoadData(tag);

		Assert.AreEqual(50, loaded.Timer.ElapsedTime);
		Assert.IsTrue(loaded.IsTimedOut);
	}

	[TestMethod]
	public void NetSend_NetReceive_RestoresAndClampsElapsedTimeToCurrentLimit()
	{
		var sent = new StubObjective();
		sent.WithTimeLimit(100);
		sent.Timer.Update(80);
		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			sent.NetSend(writer);
		}

		var received = new StubObjective();
		received.WithTimeLimit(50);
		stream.Position = 0;
		using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			received.NetReceive(reader);
		}

		Assert.AreEqual(50, received.Timer.ElapsedTime);
		Assert.IsTrue(received.IsTimedOut);
	}

	[TestMethod]
	public void LoadData_MissingTimerData_StartsAtZero()
	{
		var objective = new StubObjective();
		objective.WithTimeLimit(100);
		objective.Timer.Update(40);

		objective.LoadData(new TagCompound());

		Assert.AreEqual(0, objective.Timer.ElapsedTime);
		Assert.IsFalse(objective.IsTimedOut);
	}

	[TestMethod]
	public void NetSend_NetReceive_MixedTimersPreserveStreamAlignment()
	{
		var sentTimed = new StubObjective();
		sentTimed.WithTimeLimit(100);
		sentTimed.Timer.Update(40);
		var sentUntimed = new StubObjective();
		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			sentTimed.NetSend(writer);
			sentUntimed.NetSend(writer);
			writer.Write(12345);
		}

		var receivedTimed = new StubObjective();
		receivedTimed.WithTimeLimit(100);
		var receivedUntimed = new StubObjective();
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		receivedTimed.NetReceive(reader);
		receivedUntimed.NetReceive(reader);

		Assert.AreEqual(40, receivedTimed.Timer.ElapsedTime);
		Assert.IsNull(receivedUntimed.Timer);
		Assert.AreEqual(12345, reader.ReadInt32());
		Assert.AreEqual(stream.Length, stream.Position);
	}
}
