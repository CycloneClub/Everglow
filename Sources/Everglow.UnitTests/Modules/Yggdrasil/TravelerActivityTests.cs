using Everglow.Yggdrasil.YggdrasilTown.Travelers;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class TravelerActivityTests
{
	[TestMethod]
	public void LifetimeOnlyReclaimsTravelersOutsideView()
	{
		Assert.IsFalse(TravelerActivityRules.CanRetire(10801, true));
		Assert.IsFalse(TravelerActivityRules.CanRetire(900, false));
		Assert.IsTrue(TravelerActivityRules.CanRetire(10801, false));
	}

	[TestMethod]
	public void TrainLoadOnlyCountsSuccessfulCreationAndResetsOnceAfterOffscreenDeparture()
	{
		var load = new TravelerTrainLoad();
		load.Observe(false, false, false);
		Assert.AreEqual(0, load.Loaded);
		load.RecordLoaded();
		load.Observe(false, false, true);
		Assert.AreEqual(1, load.Loaded);
		load.Observe(true, true, true);
		load.Observe(false, true, false);
		Assert.AreEqual(1, load.Loaded);
		load.Observe(false, false, true);
		Assert.AreEqual(1, load.Loaded);
		load.Observe(false, false, false);
		Assert.AreEqual(0, load.Loaded);
		load.RecordLoaded();
		load.Observe(false, false, false);
		Assert.AreEqual(1, load.Loaded);
	}

	[TestMethod]
	public void TrainSlotsAreDistinctAndSmallLoadsCanReachAnExitDuringOneStop()
	{
		var offsets = Enumerable.Range(0, 12).Select(index => TravelerTrainLoad.SeatOffset(index, 1696)).ToArray();
		Assert.AreEqual(12, offsets.Distinct().Count());
		Assert.IsTrue(offsets.All(x => x >= 16 && x <= 1680));
		Assert.IsTrue(TravelerActivityRules.CanFinishTrainWalk(true, 30, 600, (int)((offsets[0] + 64) / 1.2f)));
		Assert.AreEqual(1696 - offsets[0], offsets[1]);
	}

	[TestMethod]
	public void StreetSpawnRequiresTownRoadOutsideViewAndReachableActivity()
	{
		var town = new Rectangle(0, 0, 2000, 1000);
		var view = new Rectangle(500, 0, 1000, 1000);
		Assert.IsTrue(TravelerActivityRules.CanSpawnOnStreet(new Vector2(200, 500), town, view, true));
		Assert.IsFalse(TravelerActivityRules.CanSpawnOnStreet(new Vector2(1000, 500), town, view, true));
		Assert.IsFalse(TravelerActivityRules.CanSpawnOnStreet(new Vector2(-200, 500), town, view, true));
		Assert.IsFalse(TravelerActivityRules.CanSpawnOnStreet(new Vector2(200, 500), town, view, false));
	}

	[TestMethod]
	public void TownActivitiesDoNotRequireDepartureIntent()
	{
		Assert.IsTrue(TravelerActivityRules.Weight(TravelerActivity.Browse, false, false, 0, 2, 300, 1) > 0);
		Assert.AreEqual(0f, TravelerActivityRules.Weight(TravelerActivity.BoardTrain, false, false, 0, 2, 300, 1));
		Assert.AreEqual(0f, TravelerActivityRules.Weight(TravelerActivity.Browse, false, true, 0, 2, 300, 1));
		Assert.AreEqual(0f, TravelerActivityRules.Weight(TravelerActivity.Browse, false, false, 2, 2, 300, 1));
		Assert.IsTrue(TravelerActivityRules.Weight(TravelerActivity.BoardTrain, true, false, 0, 2, 300, 1) > 0);
	}

	[TestMethod]
	public void ArrivalBatchOccursOncePerStopAndCanRepeatAtNextStop()
	{
		var batch = new TravelerStopBatch();
		Assert.IsFalse(batch.TryBegin(true, 12, 600));
		Assert.IsTrue(batch.TryBegin(true, 30, 600));
		Assert.IsFalse(batch.TryBegin(true, 31, 600));
		Assert.IsFalse(batch.TryBegin(true, 400, 600));
		Assert.IsFalse(batch.TryBegin(false, 600, 600));
		Assert.IsTrue(batch.TryBegin(true, 30, 600));
	}

	[TestMethod]
	public void LateArrivalCannotStartDisembarking()
	{
		Assert.IsFalse(new TravelerStopBatch().TryBegin(true, 520, 600));
		Assert.IsTrue(TravelerActivityRules.CanFinishTrainWalk(true, 30, 600, 400));
		Assert.IsFalse(TravelerActivityRules.CanFinishTrainWalk(true, 300, 600, 400));
		Assert.IsFalse(TravelerActivityRules.CanFinishTrainWalk(false, 30, 600, 10));
	}

	[TestMethod]
	public void TrainDispatchRetriesFailuresButSpacesPassengersAndCapsSuccessfulBatch()
	{
		var batch = new TravelerStopBatch();
		batch.TryBegin(true, 30, 600);
		Assert.IsTrue(batch.TryDispatch(2, 3, out int first));
		Assert.AreEqual(-1, first);
		batch.TryBegin(true, 31, 600);
		Assert.IsFalse(batch.TryDispatch(2, 3, out _));
		batch.TryBegin(true, 60, 600);
		Assert.IsTrue(batch.TryDispatch(2, 3, out int second));
		Assert.AreEqual(1, second);
		batch.RecordArrival();
		batch.TryBegin(true, 90, 600);
		Assert.IsFalse(batch.TryDispatch(2, 1, out _));
		Assert.IsTrue(batch.TryDispatch(2, 3, out _));
		batch.RecordArrival();
		batch.TryBegin(true, 120, 600);
		Assert.IsFalse(batch.TryDispatch(2, 3, out _));
	}
}
