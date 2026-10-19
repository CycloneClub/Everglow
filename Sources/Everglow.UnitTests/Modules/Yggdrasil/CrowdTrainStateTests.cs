using Everglow.Yggdrasil.Mechanics.TownCrowd;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class CrowdTrainStateTests
{
	[TestMethod]
	public void TwoTrainPassengersCanUseSeparatePlanningFrames()
	{
		var batch = new CrowdStopBatch();
		Assert.IsTrue(batch.TryBegin(true, 30, 600));
		Assert.IsTrue(batch.TryDispatch(2, 3, out int first));
		Assert.AreEqual(-1, first);
		batch.RecordArrival();
		batch.TryBegin(true, 60, 600);
		Assert.IsFalse(batch.TryDispatch(2, 1, out _));
		Assert.IsFalse(batch.TryDispatch(1, 3, out _));
		Assert.IsTrue(batch.TryDispatch(2, 3, out int second));
		Assert.AreEqual(1, second);
		batch.RecordArrival();
		batch.TryBegin(true, 90, 600);
		Assert.IsFalse(batch.TryDispatch(2, 3, out _));
		batch.TryBegin(false, 600, 600);
		Assert.IsTrue(batch.TryBegin(true, 30, 600));
		Assert.IsTrue(batch.TryDispatch(1, 3, out _));
	}
}
