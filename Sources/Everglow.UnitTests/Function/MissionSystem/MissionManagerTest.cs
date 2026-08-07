using Everglow.Commons.Mechanics.Mission.WorldSide;
using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.WorldSide.Objectives;

namespace Everglow.UnitTests.Function.MissionSystem;

[TestClass]
public class MissionManagerTest
{
	private class TestStateProvider : IGameStateProvider
	{
		public double TimeForVisualEffects { get; set; }

		public bool GameMenu => false;

		public bool GameInactive => false;

		public bool GamePaused => false;
	}

	private class TestMission1 : WorldMissionBase
	{
	}

	[TestMethod]
	public void GetMissionTest()
	{
		var manager = new WorldMissionManager(new TestStateProvider());
		manager.AddMission(new TestMission1());
		var m = manager.GetMission<TestMission1>();
		Assert.IsNotNull(m);
	}

	private class TestMission2 : WorldMissionBase
	{
		public TestMission2()
		{
			State = WorldMissionState.Active;
		}

		public override int TimeLimit => 20;

		public override void Initialize()
		{
			Objectives.Add(new WorldExploreObjective(1, (a) => false));
		}
	}

	[TestMethod]
	public void TimeLimitTest()
	{
		//var provider = new TestStateProvider();
		//provider.TimeForVisualEffects = 60;
		//var manager = new WorldMissionManager(provider);
		//manager.AddMission(new TestMission2());
		//var m = manager.GetMission<TestMission2>();
		//m.Initialize();
		//Assert.IsNotNull(m);

		//for (int i = 0; i < 30; i++)
		//{
		//	manager.Update();

		//	if (i < 20)
		//	{
		//		Assert.AreEqual(WorldMissionState.Active, m.State);
		//	}
		//	else
		//	{
		//		Assert.AreEqual(WorldMissionState.Failed, m.State);
		//	}
		//}
	}
}
