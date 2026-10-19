namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>Density budgets and shared behavior tuning; only density is player configurable.</summary>
public sealed record CrowdSettings(int PopulationLimit, int SpawnIntervalTicks, int InitialFill,
	int TrainInitialPassengers, int TrainPassengers)
{
	public const int DoorCooldownTicks = 10 * 60;
	public const int PylonCooldownTicks = 60 * 60;
	public const int ActivityMinTicks = 3 * 60;
	public const int ActivityMaxTicks = 9 * 60;
	public const int ChatMinTicks = 8 * 60;
	public const int ChatMaxTicks = 15 * 60;
	public const int OffscreenTicks = 5 * 60;
	public const int ThreatRetirementTicks = 10 * 60;
	public const int MovementRetirementTicks = 15 * 60;
	public const int DepartureRetirementTicks = 30 * 60;
	public const int RetirementAttempts = 3;
	public const int RetirementFadeTicks = 2 * 60;
	public const int RetirementRestoreTicks = 30;
	public const int RecoveryAttemptTicks = 2 * 60;
	public const float MovementProgressDistance = 16;
	public const int ChatRangeTiles = 4;
	public const int ChatChancePercent = 40;
	public const int TransitDetourPercent = 5;
	public const int TransitPauseMinTicks = 2 * 60;
	public const int TransitPauseMaxTicks = 4 * 60;
	public const int ReturnPercent = 30;
	public const int GuildPercent = 30;
	public const int StreetWeight = 8;
	public const int BuildingWeight = 3;
	public const int PylonWeight = 1;
	public const int TotalSourceWeight = StreetWeight + BuildingWeight + PylonWeight;

	private static readonly CrowdSettings Sparse = new(24, 240, 1, 3, 1);
	private static readonly CrowdSettings Standard = new(48, 60, 2, 6, 2);
	private static readonly CrowdSettings Busy = new(96, 30, 3, 9, 3);

	public static CrowdSettings ForDensity(CrowdDensity density) => density switch
	{
		CrowdDensity.Sparse => Sparse,
		CrowdDensity.Busy => Busy,
		_ => Standard,
	};

	/// <summary>Street = 0, building = 1, pylon = 2.</summary>
	public static int ChooseSource(int roll) => roll < StreetWeight ? 0 : roll < StreetWeight + BuildingWeight ? 1 : 2;
}
