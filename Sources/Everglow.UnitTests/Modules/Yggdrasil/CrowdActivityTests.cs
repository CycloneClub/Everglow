using Everglow.Yggdrasil.Mechanics.TownCrowd;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class CrowdActivityTests
{
	[TestMethod]
	public void SittingIsAnActivityAndConversationHasItsOwnLongerDuration()
	{
		Assert.IsTrue(Enum.TryParse<CrowdActivity>("Sit", out var sitting));
		Assert.IsFalse(CrowdActivityRules.IsExit(sitting));
		Assert.AreEqual(8 * 60, CrowdSettings.ChatMinTicks);
		Assert.AreEqual(15 * 60, CrowdSettings.ChatMaxTicks);
	}

	[TestMethod]
	public void RepeatedlyBlockedReturnsCanRetireOffscreenWithoutInterruptingAPlannedExit()
	{
		var retire = typeof(CrowdActivityRules).GetMethod("CanRetireAfterFailedDeparture");
		Assert.IsNotNull(retire);
		bool Check(int age, bool visible, int failures) => (bool)retire.Invoke(null, [age, visible, failures])!;
		Assert.IsTrue(Check(12000, false, 3));
		Assert.IsFalse(Check(12000, true, 3));
		Assert.IsFalse(Check(12000, false, 0));
		Assert.IsFalse(Check(12000, false, 2));
		Assert.IsFalse(Check(6000, false, 3));
	}

	[TestMethod]
	public void ReturnIntentPersistsAndVisitorsHaveDifferentStayDurations()
	{
		var decision = typeof(CrowdActivityRules).GetMethod("ShouldDepart");
		Assert.IsNotNull(decision);
		bool Decide(bool returning, int age, int visits, int seed) => (bool)decision.Invoke(null, [returning, age, visits, seed])!;
		Assert.IsTrue(Decide(true, 100, 0, 42));
		Assert.IsFalse(Decide(false, 300, 0, 42));
		Assert.IsFalse(Decide(false, 300, 5, 42));
		Assert.IsTrue(Decide(false, 10801, 0, 42));
		Assert.IsTrue(Decide(false, 3000, 5, 42));
		Assert.IsTrue(Enumerable.Range(0, 6000).Select(seed => Decide(false, 6000, 0, seed)).Distinct().Count() == 2);
	}

	[TestMethod]
	public void ReturningVisitorsCanReuseTheirArrivalPylonAndPreferItToADoor()
	{
		float pylon = CrowdActivityRules.Weight(CrowdActivity.LeavePylon, true, true, 0, 2, 300, 1);
		float door = CrowdActivityRules.Weight(CrowdActivity.EnterBuilding, true, false, 0, 2, 300, 1);
		Assert.IsTrue(pylon > door, "Returning home must remain possible even when this pylon was the last sightseeing stop.");
		Assert.AreEqual(0f, CrowdActivityRules.Weight(CrowdActivity.LeavePylon, true, true, 2, 2, 300, 1));
		Assert.AreEqual(0f, CrowdActivityRules.Weight(CrowdActivity.LeavePylon, false, false, 0, 2, 300, 1));
	}

	[TestMethod]
	public void PylonDepartureWaitsBeforeFadingAndDoorDepartureKeepsItsShortFade()
	{
		var opacity = typeof(CrowdActivityRules).GetMethod("DepartureOpacity");
		Assert.IsNotNull(opacity);
		float At(CrowdActivity activity, int tick) => (float)opacity.Invoke(null, [activity, tick])!;
		Assert.AreEqual(1f, At(CrowdActivity.LeavePylon, 0));
		Assert.AreEqual(1f, At(CrowdActivity.LeavePylon, 59));
		Assert.AreEqual(0.5f, At(CrowdActivity.LeavePylon, 75), 0.001f);
		Assert.AreEqual(0f, At(CrowdActivity.LeavePylon, 90));
		Assert.AreEqual(0f, At(CrowdActivity.LeavePylon, 120));
		Assert.AreEqual(0.5f, At(CrowdActivity.EnterBuilding, 30), 0.001f);
	}

	[TestMethod]
	public void CrowdNamePoolIsVariedAndStableForTheSameIdentity()
	{
		var names = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdNames");
		Assert.IsNotNull(names);
		var key = names.GetMethod("KeyFor");
		Assert.IsNotNull(key);
		string Key(int seed) => (string)key.Invoke(null, [seed])!;
		var keys = Enumerable.Range(0, 256).Select(Key).ToArray();
		Assert.IsTrue(keys.Distinct().Count() >= 24);
		Assert.IsTrue(keys.All(value => value.StartsWith("Mods.Everglow.TownCrowds.Names.")));
		Assert.AreEqual(Key(931), Key(931));
		Assert.IsFalse(string.IsNullOrWhiteSpace(Key(int.MinValue)));
	}

	[TestMethod]
	public void LifetimeOnlyReclaimsTravelersOutsideView()
	{
		Assert.IsFalse(CrowdActivityRules.CanRetire(10801, true));
		Assert.IsFalse(CrowdActivityRules.CanRetire(900, false));
		Assert.IsTrue(CrowdActivityRules.CanRetire(10801, false));
	}

	[TestMethod]
	public void TrainLoadOnlyCountsSuccessfulCreationAndResetsOnceAfterOffscreenDeparture()
	{
		var load = new CrowdTrainLoad();
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
		var offsets = Enumerable.Range(0, 12).Select(index => CrowdTrainLoad.SeatOffset(index, 1696)).ToArray();
		Assert.AreEqual(12, offsets.Distinct().Count());
		Assert.IsTrue(offsets.All(x => x >= 16 && x <= 1680));
		Assert.IsTrue(CrowdActivityRules.CanFinishTrainWalk(true, 30, 600, (int)((offsets[0] + 64) / 1.2f)));
		Assert.AreEqual(1696 - offsets[0], offsets[1]);
	}

	[TestMethod]
	public void StreetSpawnRequiresTownRoadOutsideViewAndReachableActivity()
	{
		var town = new Rectangle(0, 0, 2000, 1000);
		var view = new Rectangle(500, 0, 1000, 1000);
		Assert.IsTrue(CrowdActivityRules.CanSpawnOnStreet(new Vector2(200, 500), town, view));
		Assert.IsFalse(CrowdActivityRules.CanSpawnOnStreet(new Vector2(1000, 500), town, view));
		Assert.IsFalse(CrowdActivityRules.CanSpawnOnStreet(new Vector2(-200, 500), town, view));
	}

	[TestMethod]
	public void TownActivitiesDoNotRequireDepartureIntent()
	{
		Assert.IsTrue(CrowdActivityRules.Weight(CrowdActivity.Browse, false, false, 0, 2, 300, 1) > 0);
		Assert.AreEqual(0f, CrowdActivityRules.Weight(CrowdActivity.BoardTrain, false, false, 0, 2, 300, 1));
		Assert.AreEqual(0f, CrowdActivityRules.Weight(CrowdActivity.Browse, false, true, 0, 2, 300, 1));
		Assert.AreEqual(0f, CrowdActivityRules.Weight(CrowdActivity.Browse, false, false, 2, 2, 300, 1));
		Assert.IsTrue(CrowdActivityRules.Weight(CrowdActivity.BoardTrain, true, false, 0, 2, 300, 1) > 0);
	}

	[TestMethod]
	public void ArrivalBatchOccursOncePerStopAndCanRepeatAtNextStop()
	{
		var batch = new CrowdStopBatch();
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
		Assert.IsFalse(new CrowdStopBatch().TryBegin(true, 520, 600));
		Assert.IsTrue(CrowdActivityRules.CanFinishTrainWalk(true, 30, 600, 400));
		Assert.IsFalse(CrowdActivityRules.CanFinishTrainWalk(true, 300, 600, 400));
		Assert.IsFalse(CrowdActivityRules.CanFinishTrainWalk(false, 30, 600, 10));
	}

	[TestMethod]
	public void TrainDispatchRetriesFailuresButSpacesPassengersAndCapsSuccessfulBatch()
	{
		var batch = new CrowdStopBatch();
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
