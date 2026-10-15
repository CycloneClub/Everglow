using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldQuestManagerTest
{
	private int _originalNetMode;

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalNetMode = Main.netMode;
		Main.netMode = Terraria.ID.NetmodeID.SinglePlayer;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.netMode = _originalNetMode;
	}

	private class TestStateProvider : IGameStateProvider
	{
		public double TimeForVisualEffects { get; set; }

		public bool GameMenu => false;

		public bool GameInactive => false;

		public bool GamePaused => false;
	}

	private sealed class CheckingQuest : WorldQuestBase
	{
		public string NameValue { get; init; }

		public override string Name => NameValue;

		public CheckingQuest()
		{
			Objectives.Add(new PassiveObjective());
		}

		public void SetState(WorldQuestState state) => State = state;
	}

	private sealed class PassiveObjective : WorldObjectiveBase
	{
		public override bool CheckCompletion() => false;

		public override string GetObjectiveText() => string.Empty;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}
	}

	[TestMethod]
	public void Unload_ClearsQuestEventSubscriptions()
	{
		var manager = new WorldQuestManager(new TestStateProvider());
		var quest = new CheckingQuest { NameValue = "Unload" };
		manager.AddQuest(quest);
		int statusUpdateCount = 0;
		manager.QuestStatusUpdated += _ => statusUpdateCount++;

		manager.Unload();
		manager.OnQuestStatusUpdated(quest);

		Assert.AreEqual(0, statusUpdateCount);
	}

	[TestMethod]
	public void NetReceive_PublishesAfterSnapshotApplied()
	{
		var source = new CheckingQuest { NameValue = "Snapshot" };
		source.SetState(WorldQuestState.Failed);
		var target = new CheckingQuest { NameValue = "Snapshot" };
		var manager = new WorldQuestManager(new TestStateProvider());
		manager.AddQuest(target);
		var statusUpdates = new List<QuestIdentity>();
		var objectiveUpdates = new List<QuestIdentity>();
		manager.QuestStatusUpdated += identity =>
		{
			statusUpdates.Add(identity);
			Assert.AreEqual(WorldQuestState.Failed, target.State);
		};
		manager.QuestObjectiveUpdated += objectiveUpdates.Add;

		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			source.NetSend(writer);
		}
		stream.Position = 0;
		using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			manager.NetReceive(reader);
		}

		var expectedIdentity = new QuestIdentity(QuestSide.World, target.Name, target.Name);
		Assert.HasCount(1, statusUpdates);
		Assert.HasCount(1, objectiveUpdates);
		Assert.AreEqual(expectedIdentity, statusUpdates[0]);
		Assert.AreEqual(expectedIdentity, objectiveUpdates[0]);
	}

	[TestMethod]
	public void LoadData_PublishesAfterSnapshotApplied()
	{
		var target = new CheckingQuest { NameValue = "Snapshot" };
		var missing = new CheckingQuest { NameValue = "Missing" };
		missing.SetState(WorldQuestState.Active);
		var manager = new WorldQuestManager(new TestStateProvider());
		manager.AddQuest(target);
		manager.AddQuest(missing);
		var statusUpdates = new List<QuestIdentity>();
		var objectiveUpdates = new List<QuestIdentity>();
		manager.QuestStatusUpdated += identity =>
		{
			statusUpdates.Add(identity);
			Assert.AreEqual(WorldQuestState.Failed, target.State);
		};
		manager.QuestObjectiveUpdated += objectiveUpdates.Add;
		var questData = new Terraria.ModLoader.IO.TagCompound
		{
			[nameof(WorldQuestBase.State)] = (int)WorldQuestState.Failed,
		};
		var managerData = new Terraria.ModLoader.IO.TagCompound
		{
			[target.Name] = questData,
		};

		manager.LoadData(managerData);

		QuestIdentity[] expectedIdentities =
		[
			new(QuestSide.World, target.Name, target.Name),
			new(QuestSide.World, missing.Name, missing.Name),
		];
		CollectionAssert.AreEqual(expectedIdentities, statusUpdates);
		CollectionAssert.AreEqual(expectedIdentities, objectiveUpdates);
		Assert.AreEqual(WorldQuestState.Locked, missing.State);
	}

}
