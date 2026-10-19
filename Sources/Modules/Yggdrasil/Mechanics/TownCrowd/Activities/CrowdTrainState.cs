namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

public sealed class CrowdTrainLoad
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

public sealed class CrowdStopBatch
{
	private int attemptedSides;
	private int arrivals;
	private int nextAttemptTimer;

	public bool TryDispatch(int passengerLimit, int planningBudget, out int side)
	{
		side = 0;
		if (previousTimer < nextAttemptTimer || !attempted || attemptedSides >= 12 || arrivals >= passengerLimit || planningBudget < 2)
		{
			return false;
		}
		side = attemptedSides++ % 2 == 0 ? -1 : 1;
		nextAttemptTimer = previousTimer + 30;
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
