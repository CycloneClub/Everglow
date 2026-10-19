namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

[Flags]
public enum CrowdRetirementReason
{
	None = 0,
	Threat = 1,
	Blocked = 2,
	Departure = 4,
}

/// <summary>Simulation-time retirement observations and one reversible visual fade. Never drives behavior.</summary>
public sealed class CrowdRetirement
{
	private int threatTicks;
	private int movementTicks;
	private int departureTicks;
	private int movementRecoveries;
	private bool fading;
	private int transitionTicks;
	private float transitionStart = 1;

	public CrowdRetirementReason Reasons { get; private set; }
	public float Opacity { get; private set; } = 1;
	public bool Complete => Opacity <= 0;
	public int DepartureFailures { get; private set; }
	public int MovementTicks => movementTicks;

	public void ObserveThreat(bool alarmed)
	{
		threatTicks = alarmed ? Math.Min(threatTicks + 1, CrowdSettings.ThreatRetirementTicks) : 0;
		SetReason(CrowdRetirementReason.Threat, threatTicks >= CrowdSettings.ThreatRetirementTicks);
	}

	public void RecordMovementRecovery() => movementRecoveries = Math.Min(movementRecoveries + 1, CrowdSettings.RetirementAttempts);

	public void RecordDepartureFailure() => DepartureFailures = Math.Min(DepartureFailures + 1, CrowdSettings.RetirementAttempts);

	public void ObserveMovement(bool required, bool suspended, bool progressed)
	{
		if (progressed)
		{
			ResolveMovement();
		}
		else if (required && !suspended)
		{
			movementTicks = Math.Min(movementTicks + 1, CrowdSettings.MovementRetirementTicks);
		}
		SetReason(CrowdRetirementReason.Blocked, movementTicks >= CrowdSettings.MovementRetirementTicks
			&& movementRecoveries >= CrowdSettings.RetirementAttempts);
	}

	public void ObserveDeparture(bool intended, bool suspended, bool progressed)
	{
		if (!intended || progressed)
		{
			ResolveDeparture();
		}
		else if (!suspended)
		{
			departureTicks = Math.Min(departureTicks + 1, CrowdSettings.DepartureRetirementTicks);
		}
		SetReason(CrowdRetirementReason.Departure, departureTicks >= CrowdSettings.DepartureRetirementTicks
			&& DepartureFailures >= CrowdSettings.RetirementAttempts);
	}

	public void ResolveMovement()
	{
		movementTicks = movementRecoveries = 0;
		SetReason(CrowdRetirementReason.Blocked, false);
	}

	public void ResolveDeparture()
	{
		departureTicks = DepartureFailures = 0;
		SetReason(CrowdRetirementReason.Departure, false);
	}

	public void SetReason(CrowdRetirementReason reason, bool active)
	{
		Reasons = active ? Reasons | reason : Reasons & ~reason;
	}

	/// <summary>Advance once per simulation tick, after all reasons have been observed.</summary>
	public void Update()
	{
		if (Complete)
		{
			return;
		}
		bool shouldFade = Reasons != CrowdRetirementReason.None;
		if (shouldFade != fading)
		{
			fading = shouldFade;
			transitionStart = Opacity;
			transitionTicks = 0;
		}
		int duration = fading ? CrowdSettings.RetirementFadeTicks : CrowdSettings.RetirementRestoreTicks;
		transitionTicks = Math.Min(transitionTicks + 1, duration);
		Opacity = MathHelper.SmoothStep(transitionStart, fading ? 0 : 1, transitionTicks / (float)duration);
	}
}
