using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class QuestPresentationServiceTest
{
	private int _originalNetMode;

	private sealed class StubPlayerQuest : PlayerQuestBase
	{
		public string NameValue { get; init; } = nameof(StubPlayerQuest);

		public bool CompleteValue { get; set; }

		public bool PreCompleteValue { get; set; } = true;

		public override string Name => NameValue;

		public override string DisplayName => NameValue;

		public override bool CheckComplete() => CompleteValue;

		public override bool PreComplete() => PreCompleteValue;
	}

	private sealed class StubWorldQuest : WorldQuestBase
	{
		public string NameValue { get; init; } = nameof(StubWorldQuest);

		public override string Name => NameValue;

		public override string Hint => QuestHintText.Masked;

		public override float Progress => 0f;
	}

	private sealed class TimedWorldQuest : WorldQuestBase
	{
		public TimedWorldQuest()
		{
			Objective = new TimedWorldObjective();
			Objective.WithTimeLimit(WorldQuestManager.UpdateInterval);
			Objectives.Add(Objective);
		}

		public override string Name => nameof(TimedWorldQuest);

		public TimedWorldObjective Objective { get; }

		public void ActivateAndExpire()
		{
			State = WorldQuestState.Active;
			Activate();
			Objectives.UpdateNode();
		}
	}

	private sealed class TimedWorldObjective : WorldObjectiveBase
	{
		public override bool CheckCompletion() => false;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}

		public override string GetObjectiveText() => string.Empty;
	}

	private sealed class StubGameStateProvider : IGameStateProvider
	{
		public double TimeForVisualEffects => 0;

		public bool GameMenu => false;

		public bool GameInactive => false;

		public bool GamePaused => false;
	}

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalNetMode = Main.netMode;
		Main.netMode = NetmodeID.SinglePlayer;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.netMode = _originalNetMode;
	}

	[TestMethod]
	public void GetAll_ReturnsCoherentPlayerAndWorldEntries()
	{
		var player = new StubPlayerQuest { State = PlayerQuestState.Available };
		var world = new StubWorldQuest();
		var context = CreateService([player], [world]);

		IReadOnlyList<QuestPresentationEntry> entries = context.Service.GetAll();

		Assert.HasCount(2, entries);
		QuestPresentationEntry playerEntry = entries.Single(entry => entry.View.Identity.Side == QuestSide.Player);
		QuestPresentationEntry worldEntry = entries.Single(entry => entry.View.Identity.Side == QuestSide.World);
		Assert.AreEqual(player.InstanceId, playerEntry.View.Identity.InstanceId);
		Assert.HasCount(1, playerEntry.Actions);
		Assert.AreEqual(playerEntry.View.Identity, playerEntry.Actions[0].Quest);
		Assert.AreEqual(world.Name, worldEntry.View.Identity.DefinitionId);
		Assert.AreEqual(world.Name, worldEntry.View.Identity.InstanceId);
	}

	[TestMethod]
	public void TryGet_RequiresCompleteCurrentIdentity()
	{
		var player = new StubPlayerQuest { State = PlayerQuestState.Available };
		var world = new StubWorldQuest();
		var context = CreateService([player], [world]);
		var playerIdentity = new QuestIdentity(QuestSide.Player, player.Name, player.InstanceId);
		var stalePlayerIdentity = playerIdentity with { InstanceId = Guid.NewGuid().ToString("N") };
		var worldIdentity = new QuestIdentity(QuestSide.World, world.Name, world.Name);
		var mismatchedWorldIdentity = worldIdentity with { InstanceId = "runtime-index" };

		bool foundPlayer = context.Service.TryGet(playerIdentity, out QuestPresentationEntry playerEntry);
		bool foundStalePlayer = context.Service.TryGet(stalePlayerIdentity, out _);
		bool foundWorld = context.Service.TryGet(worldIdentity, out QuestPresentationEntry worldEntry);
		bool foundMismatchedWorld = context.Service.TryGet(mismatchedWorldIdentity, out _);
		bool foundWrongCase = context.Service.TryGet(playerIdentity with { DefinitionId = player.Name.ToLowerInvariant() }, out _);

		Assert.IsTrue(foundPlayer);
		Assert.AreEqual(playerIdentity, playerEntry.View.Identity);
		Assert.IsFalse(foundStalePlayer);
		Assert.IsTrue(foundWorld);
		Assert.AreEqual(worldIdentity, worldEntry.View.Identity);
		Assert.IsFalse(foundMismatchedWorld);
		Assert.IsFalse(foundWrongCase);
	}

	[TestMethod]
	public void MissingIdentity_ReturnsNoEntryAndRejectsAction()
	{
		var context = CreateService([], []);
		var identity = new QuestIdentity(QuestSide.Player, "missing", Guid.NewGuid().ToString("N"));

		Assert.IsFalse(context.Service.TryGet(identity, out _));
		Assert.IsFalse(context.Service.TryExecute(new QuestAction(identity, QuestActionType.Accept)));
	}

	[TestMethod]
	public void ReturnedEntry_RemainsUnchangedAfterManagerMutation()
	{
		var player = new StubPlayerQuest { State = PlayerQuestState.Available };
		var context = CreateService([player], []);
		QuestPresentationEntry oldEntry = context.Service.GetAll().Single();

		context.PlayerManager.ChangeQuestState(player, PlayerQuestState.Available, PlayerQuestState.Accepted);
		QuestPresentationEntry currentEntry = context.Service.GetAll().Single();

		Assert.AreEqual(QuestViewState.Available, oldEntry.View.State);
		Assert.HasCount(1, oldEntry.Actions);
		Assert.AreEqual(QuestActionType.Accept, oldEntry.Actions[0].Type);
		Assert.AreEqual(QuestViewState.Active, currentEntry.View.State);
		Assert.IsEmpty(currentEntry.Actions);
	}

	[TestMethod]
	public void TryExecute_DispatchesAndRevalidatesAction()
	{
		var executable = new StubPlayerQuest
		{
			NameValue = "Executable",
			State = PlayerQuestState.Available,
		};
		var rejected = new StubPlayerQuest
		{
			NameValue = "Rejected",
			State = PlayerQuestState.Accepted,
			CompleteValue = true,
			PreCompleteValue = false,
		};
		var context = CreateService([executable, rejected], []);
		QuestAction accept = PlayerQuestActionAdapter.GetActions(executable).Single();
		QuestAction submit = PlayerQuestActionAdapter.GetActions(rejected).Single();
		QuestAction stale = accept with
		{
			Quest = accept.Quest with { InstanceId = Guid.NewGuid().ToString("N") },
		};

		bool executed = context.Service.TryExecute(accept);
		bool staleExecuted = context.Service.TryExecute(stale);
		bool rejectedExecuted = context.Service.TryExecute(submit);

		Assert.IsTrue(executed);
		Assert.AreEqual(PlayerQuestState.Accepted, executable.State);
		Assert.IsFalse(staleExecuted);
		Assert.IsFalse(rejectedExecuted);
		Assert.AreEqual(PlayerQuestState.Accepted, rejected.State);
	}

	[TestMethod]
	public void TryExecute_DispatchesObjectiveRetryAction()
	{
		var world = new TimedWorldQuest();
		world.ActivateAndExpire();
		var context = CreateService([], [world]);
		var identity = new QuestIdentity(QuestSide.World, world.Name, world.Name);
		var action = new QuestAction(identity, QuestActionType.Retry, world.Objective.ObjectiveID);

		bool retried = context.Service.TryExecute(action);

		Assert.IsTrue(retried);
		Assert.AreEqual(0, world.Objective.Timer.ElapsedTime);
		Assert.IsFalse(world.Objective.IsTimedOut);
	}

	[TestMethod]
	public void TryExecute_RejectsInvalidObjectiveRetryActions()
	{
		var world = new TimedWorldQuest();
		world.ActivateAndExpire();
		var context = CreateService([], [world]);
		var identity = new QuestIdentity(QuestSide.World, world.Name, world.Name);
		var playerIdentity = new QuestIdentity(QuestSide.Player, world.Name, world.Name);
		int objectiveId = world.Objective.ObjectiveID;

		Assert.IsFalse(context.Service.TryExecute(new QuestAction(playerIdentity, QuestActionType.Retry, objectiveId)));
		Assert.IsFalse(context.Service.TryExecute(new QuestAction(identity with { InstanceId = "stale" }, QuestActionType.Retry, objectiveId)));
		Assert.IsFalse(context.Service.TryExecute(new QuestAction(identity, QuestActionType.Retry, objectiveId.ToString())));
		Assert.IsTrue(world.Objective.IsTimedOut);
	}

	private static (
		QuestPresentationService Service,
		PlayerQuestManager PlayerManager) CreateService(
		IEnumerable<PlayerQuestBase> playerQuests,
		IEnumerable<WorldQuestBase> worldQuests)
	{
		var playerManager = new PlayerQuestManager();
		playerManager.ApplyData(new PlayerQuestManagerData([], playerQuests.ToList()));
		var worldManager = new WorldQuestManager(new StubGameStateProvider());
		foreach (WorldQuestBase quest in worldQuests)
		{
			worldManager.AddQuest(quest);
		}
		var service = new QuestPresentationService(
			playerManager,
			new PlayerQuestActions(playerManager),
			worldManager,
			new WorldQuestActions(worldManager));
		return (service, playerManager);
	}
}
