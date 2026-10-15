using Everglow.Commons.Mechanics.Quest.Core;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class QuestTimerTest
{
	[TestMethod]
	[DataRow(0)]
	[DataRow(-1)]
	public void Constructor_RejectsNonPositiveLimit(int timeLimit)
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new QuestTimer(timeLimit));
	}

	[TestMethod]
	public void Update_ClampsAtLimitAndReportsOnlyFirstExpiration()
	{
		var timer = new QuestTimer(100);

		Assert.IsFalse(timer.Update(40));
		Assert.AreEqual(40, timer.ElapsedTime);
		Assert.AreEqual(60, timer.RemainingTime);
		Assert.IsFalse(timer.IsExpired);

		Assert.IsTrue(timer.Update(80));
		Assert.AreEqual(100, timer.ElapsedTime);
		Assert.AreEqual(0, timer.RemainingTime);
		Assert.IsTrue(timer.IsExpired);

		Assert.IsFalse(timer.Update(20));
		Assert.AreEqual(100, timer.ElapsedTime);
	}

	[TestMethod]
	public void Update_UsesOverflowSafeAddition()
	{
		var timer = new QuestTimer(int.MaxValue);

		Assert.IsFalse(timer.Update(int.MaxValue - 1));
		Assert.IsTrue(timer.Update(int.MaxValue));
		Assert.AreEqual(int.MaxValue, timer.ElapsedTime);
	}

	[TestMethod]
	public void Update_RejectsNegativeElapsedFrames()
	{
		var timer = new QuestTimer(100);

		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => timer.Update(-1));
		Assert.AreEqual(0, timer.ElapsedTime);
	}

	[TestMethod]
	public void Reset_ReopensExpiredTimer()
	{
		var timer = new QuestTimer(10);
		timer.Update(10);

		timer.Reset();

		Assert.AreEqual(0, timer.ElapsedTime);
		Assert.AreEqual(10, timer.RemainingTime);
		Assert.IsFalse(timer.IsExpired);
		Assert.IsTrue(timer.Update(10));
	}
}
