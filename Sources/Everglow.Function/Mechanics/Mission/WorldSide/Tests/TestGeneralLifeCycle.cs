using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Tests;

public class TestGeneralLifeCycle : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives.Add(new TestGeneralMissionLifeCycleObjective());
	}

	public class TestGeneralMissionLifeCycleObjective : WorldObjectiveBase
	{
		public override bool CheckCompletion() => true;

		public override void GetObjectivesText() => throw new NotImplementedException();
	}
}
