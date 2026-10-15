using System.Reflection;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Packets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class ObjectiveDeltaSyncPacketTest
{
	private int _originalNetMode;
	private WorldQuestSystem _originalSystem;
	private IReadOnlyList<WorldQuestSystem> _originalSystems;
	private WorldQuestManager _manager;

	private sealed class TestQuest : WorldQuestBase
	{
		public TestQuest()
		{
			Objective = new TestObjective();
			Objective.WithTimeLimit(WorldQuestManager.UpdateInterval);
			Objectives.Add(Objective);
		}

		public TestObjective Objective { get; }

		public void SetActive()
		{
			State = WorldQuestState.Active;
			Activate();
		}
	}

	private sealed class TestObjective : WorldObjectiveBase
	{
		public bool FailOnReceive { get; set; }

		public int ReceivedProgress { get; private set; }

		public override bool CheckCompletion() => false;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}

		public override string GetObjectiveText() => string.Empty;

		public override void ReceiveDelta(BinaryReader reader)
		{
			ReceivedProgress += reader.ReadInt32();
			if (FailOnReceive)
			{
				Assert.Fail("Timed-out objective received delta progress.");
			}
		}
	}

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalNetMode = Main.netMode;
		Main.netMode = NetmodeID.SinglePlayer;
		_originalSystem = ContentInstance<WorldQuestSystem>.Instance;
		_originalSystems = ContentInstance<WorldQuestSystem>.Instances;
		var system = new WorldQuestSystem();
		_manager = new WorldQuestManager();
		SetManager(system, _manager);
		SetContentInstances(system, [system]);
	}

	[TestCleanup]
	public void Cleanup()
	{
		_manager.Reset();
		SetContentInstances(_originalSystem, _originalSystems);
		Main.netMode = _originalNetMode;
	}

	[TestMethod]
	public void Receive_DoesNotApplyDeltaAfterAuthoritativeObjectiveTimeout()
	{
		var quest = new TestQuest();
		_manager.AddQuest(quest);
		quest.SetActive();
		quest.Objectives.UpdateNode();
		quest.Objective.FailOnReceive = true;
		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			writer.Write(quest.Name);
			writer.Write(quest.Objective.ObjectiveID);
			writer.Write(7);
		}
		stream.Position = 0;

		using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			new ObjectiveDeltaSyncPacket_SubProgress().Receive(reader, whoAmI: 4);
		}

		Assert.IsTrue(quest.Objective.IsTimedOut);
		Assert.AreEqual(0, quest.Objective.ReceivedProgress);
	}

	private static void SetManager(WorldQuestSystem system, WorldQuestManager manager)
	{
		PropertyInfo managerProperty = typeof(WorldQuestSystem).GetProperty(nameof(WorldQuestSystem.Manager))!;
		managerProperty.SetValue(system, manager);
	}

	private static void SetContentInstances(WorldQuestSystem instance, IReadOnlyList<WorldQuestSystem> instances)
	{
		Type contentInstanceType = typeof(ContentInstance<WorldQuestSystem>);
		contentInstanceType.GetProperty(nameof(ContentInstance<WorldQuestSystem>.Instance))!
			.GetSetMethod(nonPublic: true)!
			.Invoke(null, [instance]);
		contentInstanceType.GetProperty(nameof(ContentInstance<WorldQuestSystem>.Instances))!
			.GetSetMethod(nonPublic: true)!
			.Invoke(null, [instances]);
	}
}
