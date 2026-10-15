using System.Reflection;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class PlayerQuestBaseBehaviorTest
{
	private PlayerQuestSystem _system;
	private PlayerQuestSystem _originalSystem;
	private IReadOnlyList<PlayerQuestSystem> _originalSystems;
	private PlayerQuestManager _manager;

	private sealed class StubQuest : PlayerQuestBase
	{
		public override string DisplayName => nameof(StubQuest);
	}

	[TestInitialize]
	public void Initialize()
	{
		_originalSystem = ContentInstance<PlayerQuestSystem>.Instance;
		_originalSystems = ContentInstance<PlayerQuestSystem>.Instances;
		_system = new PlayerQuestSystem();
		_manager = new PlayerQuestManager();
		SetManager(_manager);
		SetContentInstances(_system, [_system]);
	}

	[TestCleanup]
	public void Cleanup()
	{
		_manager.Clear();
		SetContentInstances(_originalSystem, _originalSystems);
	}

	[TestMethod]
	public void OnExpire_TransitionsAcceptedQuestToFailed()
	{
		var quest = new StubQuest();
		_manager.AddQuest(quest, PlayerQuestState.Accepted, showText: false);

		quest.OnExpire();

		Assert.AreEqual(PlayerQuestState.Failed, quest.State);
	}

	[TestMethod]
	public void Reset_ClearsElapsedTime()
	{
		var quest = new StubQuest { Time = 120 };

		quest.Reset();

		Assert.AreEqual(0, quest.Time);
	}

	private void SetManager(PlayerQuestManager manager)
	{
		PropertyInfo managerProperty = typeof(PlayerQuestSystem).GetProperty(nameof(PlayerQuestSystem.Manager))!;
		managerProperty.SetValue(_system, manager);
	}

	private static void SetContentInstances(PlayerQuestSystem instance, IReadOnlyList<PlayerQuestSystem> instances)
	{
		Type contentInstanceType = typeof(ContentInstance<PlayerQuestSystem>);
		contentInstanceType.GetProperty(nameof(ContentInstance<PlayerQuestSystem>.Instance))!
			.GetSetMethod(nonPublic: true)!
			.Invoke(null, [instance]);
		contentInstanceType.GetProperty(nameof(ContentInstance<PlayerQuestSystem>.Instances))!
			.GetSetMethod(nonPublic: true)!
			.Invoke(null, [instances]);
	}
}
