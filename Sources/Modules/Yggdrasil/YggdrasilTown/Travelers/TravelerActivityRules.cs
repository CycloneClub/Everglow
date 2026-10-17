namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

public enum TravelerActivity
{
	Browse,
	Socialize,
	Scenery,
	BoardTrain,
	EnterBuilding,
	LeavePylon,
	LeaveStreet,
}

public static class TravelerActivityRules
{
	public static bool CanRetire(int age, bool visible) => age > 10800 && !visible;

	public static bool CanSpawnOnStreet(Vector2 feet, Rectangle town, Rectangle view, bool connected) =>
		connected && town.Contains(feet.ToPoint()) && !view.Contains(feet.ToPoint());

	public static bool IsExit(TravelerActivity activity) => activity >= TravelerActivity.BoardTrain;

	public static float Weight(TravelerActivity activity, bool departing, bool recent, int occupied, int capacity, float distance, float preference)
	{
		if (recent || occupied >= capacity || IsExit(activity) != departing)
		{
			return 0;
		}
		return Math.Max(0.1f, preference) / (1 + Math.Max(0, distance) / 480) / (1 + occupied * 2);
	}

	public static bool CanFinishTrainWalk(bool stopped, int timer, int duration, int travelTicks) =>
		stopped && timer >= 30 && travelTicks >= 0 && travelTicks + 90 < duration - timer;
}

public sealed class TravelerTrainLoad
{
	private bool departedStop;

	public int Loaded { get; private set; }

	// Stable slots alternate from the two exits inward, independent of the live load setting.
	public static float SeatOffset(int index, float width) => index % 2 == 0 ? 96 + index / 2 * 128 : width - 96 - index / 2 * 128;

	public void RecordLoaded() => Loaded++;

	public void Observe(bool stopped, bool visible, bool hasRiders)
	{
		if (stopped)
		{
			departedStop = true;
		}
		else if (departedStop && !visible && !hasRiders)
		{
			Loaded = 0;
			departedStop = false;
		}
	}
}

public sealed class TravelerStopBatch
{
	private int attemptedSides;
	private int arrivals;
	private int nextAttemptTimer;

	public bool TryDispatch(int passengerLimit, int planningBudget, out int side)
	{
		side = 0;
		if (previousTimer < nextAttemptTimer || !TryNextSide(passengerLimit, planningBudget, out side))
		{
			return false;
		}
		nextAttemptTimer = previousTimer + 30;
		return true;
	}

	public bool TryNextSide(int passengerLimit, int planningBudget, out int side)
	{
		side = 0;
		if (!attempted || attemptedSides >= 12 || arrivals >= passengerLimit || planningBudget < 2)
		{
			return false;
		}
		side = attemptedSides++ % 2 == 0 ? -1 : 1;
		return true;
	}

	public void RecordArrival() => arrivals++;

	private bool attempted;
	private int previousTimer;

	public bool TryBegin(bool stopped, int timer, int duration)
	{
		if (!stopped || timer < previousTimer)
		{
			attempted = false;
			attemptedSides = 0;
			arrivals = 0;
			nextAttemptTimer = 0;
		}
		previousTimer = timer;
		if (!stopped || attempted || timer < 30 || timer >= duration - 90)
		{
			return false;
		}
		attempted = true;
		return true;
	}
}
