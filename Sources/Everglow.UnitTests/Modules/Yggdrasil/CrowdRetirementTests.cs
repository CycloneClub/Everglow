using Everglow.Yggdrasil.Mechanics.TownCrowd;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class CrowdRetirementTests
{
	private static void Fade(CrowdRetirement retirement, int ticks)
	{
		for (int tick = 0; tick < ticks; tick++)
		{
			retirement.Update();
		}
	}

	[TestMethod]
	public void ThreatUsesSustainedAvoidanceRecoveryRatherThanOneMissingDetection()
	{
		var avoidance = new CrowdAvoidance();
		var retirement = new CrowdRetirement();
		for (int tick = 0; tick < 600; tick++)
		{
			avoidance.Update(tick == 0, false, true, 300);
			retirement.ObserveThreat(avoidance.Alarmed);
			if (tick < 599)
			{
				Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
			}
		}
		Assert.AreEqual(CrowdRetirementReason.Threat, retirement.Reasons);
		for (int tick = 0; tick < 179; tick++)
		{
			avoidance.Update(false, true, true, 300);
		}
		retirement.ObserveThreat(avoidance.Alarmed);
		Assert.AreEqual(CrowdRetirementReason.Threat, retirement.Reasons);
		avoidance.Update(false, true, true, 300);
		retirement.ObserveThreat(avoidance.Alarmed);
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
	}

	[TestMethod]
	public void OverlappingReasonsShareFadeAndOnlyRecoverAfterLastReasonClears()
	{
		var retirement = new CrowdRetirement();
		retirement.SetReason(CrowdRetirementReason.Threat, true);
		Fade(retirement, 60);
		Assert.AreEqual(0.5f, retirement.Opacity, 0.0001f);
		retirement.SetReason(CrowdRetirementReason.Blocked, true);
		retirement.SetReason(CrowdRetirementReason.Threat, false);
		Fade(retirement, 30);
		Assert.IsTrue(retirement.Opacity < 0.2f, "Adding or replacing reasons must not restart the two-second fade.");
		float low = retirement.Opacity;
		retirement.SetReason(CrowdRetirementReason.Blocked, false);
		Fade(retirement, 15);
		Assert.AreEqual((low + 1) / 2, retirement.Opacity, 0.0001f);
		Fade(retirement, 15);
		Assert.AreEqual(1f, retirement.Opacity);
	}

	[TestMethod]
	public void RelapseFadesFromCurrentOpacityAndCompletionIsTerminal()
	{
		var retirement = new CrowdRetirement();
		retirement.SetReason(CrowdRetirementReason.Departure, true);
		Fade(retirement, 90);
		retirement.SetReason(CrowdRetirementReason.Departure, false);
		Fade(retirement, 10);
		float before = retirement.Opacity;
		retirement.SetReason(CrowdRetirementReason.Threat, true);
		Assert.AreEqual(before, retirement.Opacity);
		Fade(retirement, 60);
		Assert.AreEqual(before / 2, retirement.Opacity, 0.0001f);
		Fade(retirement, 59);
		Assert.IsFalse(retirement.Complete);
		Fade(retirement, 1);
		Assert.IsTrue(retirement.Complete);
		retirement.SetReason(CrowdRetirementReason.Threat, false);
		Fade(retirement, 30);
		Assert.AreEqual(0f, retirement.Opacity);
	}

	[TestMethod]
	public void BlockedMovementNeedsTimeAndActualRecoveriesAndResetsAfterProgress()
	{
		var retirement = new CrowdRetirement();
		for (int tick = 0; tick < 900; tick++)
		{
			retirement.ObserveMovement(true, false, false);
		}
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
		for (int attempt = 0; attempt < 3; attempt++)
		{
			retirement.RecordMovementRecovery();
		}
		retirement.ObserveMovement(true, false, false);
		Assert.AreEqual(CrowdRetirementReason.Blocked, retirement.Reasons);
		retirement.ObserveMovement(true, false, true);
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
		for (int tick = 0; tick < 1800; tick++)
		{
			retirement.ObserveMovement(true, false, false);
		}
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons, "Earlier recovery attempts must not carry into a new stall.");
	}

	[TestMethod]
	public void RestAndBudgetWaitsDoNotAccumulateMovementOrDepartureTime()
	{
		var retirement = new CrowdRetirement();
		for (int attempt = 0; attempt < 3; attempt++)
		{
			retirement.RecordMovementRecovery();
			retirement.RecordDepartureFailure();
		}
		for (int tick = 0; tick < 3600; tick++)
		{
			retirement.ObserveMovement(false, false, false);
			retirement.ObserveMovement(true, true, false);
			retirement.ObserveDeparture(true, true, false);
		}
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
		Assert.AreEqual(0, retirement.MovementTicks);
	}

	[TestMethod]
	public void DepartureRequiresFailuresAndActualExitProgressClearsAllHistory()
	{
		var retirement = new CrowdRetirement();
		for (int tick = 0; tick < 1800; tick++)
		{
			retirement.ObserveDeparture(true, false, false);
		}
		for (int attempt = 0; attempt < 2; attempt++)
		{
			retirement.RecordDepartureFailure();
		}
		retirement.ObserveDeparture(true, false, false);
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
		retirement.RecordDepartureFailure();
		retirement.ObserveDeparture(true, false, false);
		Assert.AreEqual(CrowdRetirementReason.Departure, retirement.Reasons);
		retirement.ObserveDeparture(true, false, true);
		Assert.AreEqual(0, retirement.DepartureFailures);
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
		for (int tick = 0; tick < 6000; tick++)
		{
			retirement.ObserveDeparture(true, false, tick % 30 == 0);
		}
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
	}

	[TestMethod]
	public void TinyMotionAndRepeatedTraversalOfSameNodesAreNotProgress()
	{
		var progress = new CrowdProgress();
		progress.Observe(Vector2.Zero, new Vector2(100, 0), false);
		for (int tick = 0; tick < 900; tick++)
		{
			Assert.IsFalse(progress.Observe(new Vector2(tick % 2 == 0 ? 7 : -7, 0), new Vector2(100, 0), true));
		}
		Assert.IsTrue(progress.Observe(new Vector2(16, 0), new Vector2(100, 0), true));
		for (int tick = 0; tick < 900; tick++)
		{
			Assert.IsFalse(progress.Observe(new Vector2(tick % 2 == 0 ? 0 : 16, 0), new Vector2(tick % 2 == 0 ? -100 : 100, 0), true));
		}
	}

	[TestMethod]
	public void RealTravelAndDetourWaypointsCountAsProgress()
	{
		var progress = new CrowdProgress();
		progress.Observe(Vector2.Zero, new Vector2(10000, 0), false);
		for (int node = 1; node < 200; node++)
		{
			Assert.IsTrue(progress.Observe(new Vector2(node * 16, 0), new Vector2(10000, 0), false));
		}
		Assert.IsTrue(progress.Observe(new Vector2(199 * 16, 16), Vector2.Zero, true), "Advancing along a checked detour need not reduce direct distance to the final destination.");
	}

	[TestMethod]
	public void ActivityAndRetirementOpacityMultiplyWithoutChangingActivityOpacity()
	{
		var appearance = new CrowdAppearance { Opacity = 0.4f, RetirementOpacity = 0.5f };
		Assert.AreEqual(0.2f, appearance.EffectiveOpacity, 0.0001f);
		appearance.RetirementOpacity = 1;
		Assert.AreEqual(0.4f, appearance.Opacity);
		Assert.AreEqual(0.4f, appearance.EffectiveOpacity);
	}
}
