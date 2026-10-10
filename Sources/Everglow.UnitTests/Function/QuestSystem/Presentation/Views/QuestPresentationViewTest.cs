using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class QuestPresentationViewTest
{
	[TestMethod]
	public void RemainingTime_IsClampedToZero()
	{
		var active = new QuestView { ElapsedTime = 40, TimeLimit = 100 };
		var expired = new QuestView { ElapsedTime = 120, TimeLimit = 100 };

		Assert.AreEqual(60, active.RemainingTime);
		Assert.AreEqual(0, expired.RemainingTime);
	}
}
