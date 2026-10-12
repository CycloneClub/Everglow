using System.Reflection;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Yggdrasil;
using Everglow.Yggdrasil.YggdrasilTown.Quests;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldQuestUnlockAuthorityTest
{
	private static readonly FieldInfo CurrentWorld = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
	private int oldMode;
	private Subworld? oldWorld;
	private WorldQuestSystem oldSystem = null!;
	private IReadOnlyList<WorldQuestSystem> oldSystems = null!;
	private WorldQuestManager manager = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		oldMode = Main.netMode;
		oldWorld = (Subworld?)CurrentWorld.GetValue(null);
		oldSystem = ContentInstance<WorldQuestSystem>.Instance;
		oldSystems = ContentInstance<WorldQuestSystem>.Instances;
		Main.netMode = NetmodeID.SinglePlayer;
		CurrentWorld.SetValue(null, null);
		manager = new WorldQuestManager(new TestState());
		var system = new WorldQuestSystem();
		typeof(WorldQuestSystem).GetProperty(nameof(WorldQuestSystem.Manager))!.SetValue(system, manager);
		SetSystems(system, [system]);
	}

	[TestCleanup]
	public void Cleanup()
	{
		SetSystems(oldSystem, oldSystems);
		Main.netMode = oldMode;
		CurrentWorld.SetValue(null, oldWorld);
	}

	[DataTestMethod]
	[DataRow(NetmodeID.SinglePlayer, false)]
	[DataRow(NetmodeID.MultiplayerClient, false)]
	[DataRow(NetmodeID.MultiplayerClient, true)]
	[DataRow(NetmodeID.Server, false)]
	[DataRow(NetmodeID.Server, true)]
	public void CoreTransitionOnlyChecksQuestState(int netMode, bool inSubworld)
	{
		Main.netMode = netMode;
		CurrentWorld.SetValue(null, inSubworld ? new YggdrasilWorld() : null);
		var quest = new TestQuest();
		Assert.IsTrue(TryUnlock(quest));
		Assert.AreEqual(WorldQuestState.Active, quest.State);
		Assert.HasCount(1, quest.ActiveObjectives);
		Assert.IsFalse(TryUnlock(quest));
		Assert.HasCount(1, quest.ActiveObjectives);
	}

	[DataTestMethod]
	[DataRow(NetmodeID.MultiplayerClient, false)]
	[DataRow(NetmodeID.MultiplayerClient, true)]
	[DataRow(NetmodeID.Server, true)]
	public void NonAuthoritativeManagerDoesNotEvaluateOrUnlock(int netMode, bool inSubworld)
	{
		Main.netMode = netMode;
		CurrentWorld.SetValue(null, inSubworld ? new YggdrasilWorld() : null);
		var quest = new TestQuest();
		manager.AddQuest(quest);
		manager.Update();
		Assert.AreEqual(0, quest.ConditionChecks);
		Assert.AreEqual(WorldQuestState.Locked, quest.State);
		Assert.IsEmpty(quest.ActiveObjectives);
	}

	[DataTestMethod]
	[DataRow(NetmodeID.SinglePlayer)]
	[DataRow(NetmodeID.Server)]
	public void AuthoritativeManagerEvaluatesUnlockCondition(int netMode)
	{
		Main.netMode = netMode;
		var quest = new TestQuest { AllowUnlock = false };
		manager.AddQuest(quest);
		manager.Update();
		Assert.AreEqual(1, quest.ConditionChecks);
		Assert.AreEqual(WorldQuestState.Locked, quest.State);
	}

	[TestMethod]
	public void TownQuestPrerequisiteCanBeEvaluatedInMainWorld()
	{
		Main.netMode = NetmodeID.Server;
		Assert.IsTrue(new TestTownQuest().CanUnlock());
	}

	private static bool TryUnlock(WorldQuestBase quest) =>
		(bool)typeof(WorldQuestBase).GetMethod("UnlockCore", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(quest, null)!;

	private static void SetSystems(WorldQuestSystem system, IReadOnlyList<WorldQuestSystem> systems)
	{
		typeof(ContentInstance<WorldQuestSystem>).GetProperty(nameof(ContentInstance<WorldQuestSystem>.Instance))!.SetValue(null, system);
		typeof(ContentInstance<WorldQuestSystem>).GetProperty(nameof(ContentInstance<WorldQuestSystem>.Instances))!.SetValue(null, systems);
	}

	private sealed class TestState : IGameStateProvider
	{
		public double TimeForVisualEffects => 0;

		public bool GameMenu => false;

		public bool GameInactive => false;

		public bool GamePaused => false;
	}

	private sealed class TestTownQuest : TownNpcQuest
	{
		public override int GiverNpcType => NPCID.Guide;

		public override bool CanOffer(WorldQuestManager manager) => true;
	}

	private sealed class TestQuest : WorldQuestBase
	{
		public int ConditionChecks;
		public bool AllowUnlock = true;

		public TestQuest() => Objectives.Add(new PassiveObjective());

		public override bool CanUnlock()
		{
			ConditionChecks++;
			return AllowUnlock;
		}
	}

	private sealed class PassiveObjective : WorldObjectiveBase
	{
		public override bool CheckCompletion() => false;

		public override string GetObjectiveText() => string.Empty;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}
	}
}
