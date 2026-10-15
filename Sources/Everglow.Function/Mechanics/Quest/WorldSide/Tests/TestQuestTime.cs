using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

public class TestQuestTime : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives.Add(new TestQuestTimeObjective().WithTimeLimit(600));
	}

	public class TestQuestTimeObjective : WorldObjectiveBase
	{
		public override bool CheckCompletion() => false;

		public override string GetObjectiveText() => string.Empty;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
		}
	}
}
