using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

public class TestGeneralLifeCycle : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives.Add(new TestGeneralQuestLifeCycleObjective());
	}

	public class TestGeneralQuestLifeCycleObjective : WorldObjectiveBase
	{
		public override bool CheckCompletion() => true;

		public override string GetObjectiveText() => string.Empty;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}
	}
}
