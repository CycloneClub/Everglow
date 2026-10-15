using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class PlayerQuestManagerTest
{
	private bool _originalDedServ;
	private bool _originalGameMenu;
	private bool _originalGameInactive;
	private double _originalTimeForVisualEffects;

	private sealed class StubQuest : PlayerQuestBase
	{
		public string NameValue { get; init; } = nameof(StubQuest);

		public int UpdateCount { get; private set; }

		public override string Name => NameValue;

		public override string DisplayName => NameValue;

		public override bool CheckComplete() => false;

		public override void Update() => UpdateCount++;

		public override void OnCheckCompleteChange()
		{
		}
	}

	private sealed class HookQuest : PlayerQuestBase
	{
		public int ActivateHookCount { get; private set; }

		public override string DisplayName => nameof(HookQuest);

		public override void Activate()
		{
			base.Activate();
			ActivateHookCount++;
		}
	}

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalDedServ = Terraria.Main.dedServ;
		_originalGameMenu = Terraria.Main.gameMenu;
		_originalGameInactive = Terraria.Main.gameInactive;
		_originalTimeForVisualEffects = Terraria.Main.timeForVisualEffects;
		Terraria.Main.dedServ = true;
		Terraria.Main.gameMenu = false;
		Terraria.Main.gameInactive = false;
		Terraria.Main.timeForVisualEffects = PlayerQuestManager.UpdateInterval;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Terraria.Main.dedServ = _originalDedServ;
		Terraria.Main.gameMenu = _originalGameMenu;
		Terraria.Main.gameInactive = _originalGameInactive;
		Terraria.Main.timeForVisualEffects = _originalTimeForVisualEffects;
	}

	[TestMethod]
	public void MoveQuest_TransitionsAndActivatesQuest()
	{
		var quest = new HookQuest();
		var manager = new PlayerQuestManager();
		manager.AddQuest(quest, PlayerQuestState.Available, showText: false);

		manager.ChangeQuestState(quest, PlayerQuestState.Available, PlayerQuestState.Accepted);

		Assert.AreEqual(PlayerQuestState.Accepted, quest.State);
		Assert.AreEqual(1, quest.ActivateHookCount);
	}

	[TestMethod]
	public void Unload_ClearsQuestEventSubscriptions()
	{
		var manager = new PlayerQuestManager();
		int addedCount = 0;
		manager.QuestAdded += _ => addedCount++;

		manager.Unload();
		manager.AddQuest(new StubQuest(), PlayerQuestState.Available, showText: false);

		Assert.AreEqual(0, addedCount);
	}

	[TestMethod]
	public void AddQuest_PublishesAddedIdentityAfterInsertion()
	{
		var quest = new StubQuest();
		var manager = new PlayerQuestManager();
		QuestIdentity? publishedIdentity = null;
		manager.QuestAdded += identity =>
		{
			publishedIdentity = identity;
			Assert.AreSame(quest, manager.GetQuest(identity.DefinitionId));
		};

		manager.AddQuest(quest, PlayerQuestState.Available, showText: false);

		Assert.AreEqual(new QuestIdentity(QuestSide.Player, quest.Name, quest.InstanceId), publishedIdentity);
	}

	[TestMethod]
	public void RemoveQuest_PublishesRemovedIdentityOnce()
	{
		var quest = new StubQuest();
		var manager = new PlayerQuestManager();
		manager.ApplyData(new PlayerQuestManagerData([], [quest]));
		var publishedIdentities = new List<QuestIdentity>();
		manager.QuestRemoved += publishedIdentities.Add;

		bool removed = manager.RemoveQuest(quest.Name);
		bool repeated = manager.RemoveQuest(quest.Name);

		Assert.IsTrue(removed);
		Assert.IsFalse(repeated);
		Assert.HasCount(1, publishedIdentities);
		Assert.AreEqual(new QuestIdentity(QuestSide.Player, quest.Name, quest.InstanceId), publishedIdentities[0]);
		Assert.IsNull(manager.GetQuest(quest.Name));
	}

	[TestMethod]
	public void MoveQuest_PublishesStatusIdentityAfterTransition()
	{
		var quest = new HookQuest();
		var manager = new PlayerQuestManager();
		manager.AddQuest(quest, PlayerQuestState.Available, showText: false);
		QuestIdentity? publishedIdentity = null;
		manager.QuestStatusUpdated += identity =>
		{
			publishedIdentity = identity;
			Assert.AreEqual(PlayerQuestState.Accepted, quest.State);
		};

		manager.ChangeQuestState(quest, PlayerQuestState.Available, PlayerQuestState.Accepted);

		Assert.AreEqual(new QuestIdentity(QuestSide.Player, quest.Name, quest.InstanceId), publishedIdentity);
	}

	[TestMethod]
	public void Update_PublishesObjectiveIdentityForAcceptedQuest()
	{
		var quest = new StubQuest { State = PlayerQuestState.Accepted };
		var manager = new PlayerQuestManager();
		manager.ApplyData(new PlayerQuestManagerData([], [quest]));
		var publishedIdentities = new List<QuestIdentity>();
		manager.QuestObjectiveUpdated += publishedIdentities.Add;

		manager.Update();

		Assert.AreEqual(1, quest.UpdateCount);
		Assert.HasCount(1, publishedIdentities);
		Assert.AreEqual(new QuestIdentity(QuestSide.Player, quest.Name, quest.InstanceId), publishedIdentities[0]);
	}

	[TestMethod]
	public void MutableState_IsIsolatedAcrossManagerInstances()
	{
		var firstQuest = new StubQuest();
		var secondQuest = new StubQuest();
		var first = new PlayerQuestManager();
		var second = new PlayerQuestManager();

		first.ApplyData(new PlayerQuestManagerData(
			new Dictionary<int, int> { [NPCID.BlueSlime] = 3 },
			[firstQuest]));
		second.ApplyData(new PlayerQuestManagerData(
			new Dictionary<int, int> { [NPCID.Zombie] = 7 },
			[secondQuest]));

		Assert.AreSame(firstQuest, first.GetQuest(nameof(StubQuest)));
		Assert.AreSame(secondQuest, second.GetQuest(nameof(StubQuest)));
		Assert.AreEqual(3, first.NPCKillCounter[NPCID.BlueSlime]);
		Assert.IsFalse(second.NPCKillCounter.ContainsKey(NPCID.BlueSlime));
	}

}
