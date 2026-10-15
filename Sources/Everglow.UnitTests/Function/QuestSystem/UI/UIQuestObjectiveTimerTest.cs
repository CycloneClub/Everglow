using Everglow.Commons.Mechanics.Quest.UI.UIElements;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class UIQuestObjectiveTimerTest
{
	[TestMethod]
	public void RemainingRatio_UsesRealTimerValuesAndClampsToUnitRange()
	{
		var hourglass = new UIQuestHourglassTimer { MaxTime = 100, Timer = 25 };

		Assert.AreEqual(0.25f, hourglass.RemainingRatio);

		hourglass.Timer = 120;
		Assert.AreEqual(1f, hourglass.RemainingRatio);

		hourglass.Timer = -1;
		Assert.AreEqual(0f, hourglass.RemainingRatio);

		hourglass.MaxTime = 0;
		Assert.AreEqual(0f, hourglass.RemainingRatio);
	}

}
