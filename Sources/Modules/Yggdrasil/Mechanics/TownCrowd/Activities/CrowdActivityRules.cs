namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

public enum CrowdActivity
{
	Browse,
	Socialize,
	Scenery,
	BoardTrain,
	EnterBuilding,
	LeavePylon,
	LeaveStreet,
	EnterGuild,
	Sit,
}

public static class CrowdActivityRules
{
	public static bool ShouldDepart(bool requested, int age, int completedActivities, int seed) => requested
		|| age >= 5400 + (uint)seed % 5401
		|| (age >= 1800 && completedActivities >= 3 + (uint)seed % 3);

	public static float DepartureOpacity(CrowdActivity activity, int timer) => activity == CrowdActivity.LeavePylon
		? Math.Clamp(1 - (timer - 60) / 30f, 0, 1)
		: Math.Clamp(1 - timer / 60f, 0, 1);

	public static bool CanRetire(int age, bool visible) => age > 10800 && !visible;

	public static bool CanRetireAfterFailedDeparture(int age, bool visible, int failedAttempts) =>
		failedAttempts >= 3 && CanRetire(age, visible);

	public static bool CanSpawnOnStreet(Vector2 feet, Rectangle town, Rectangle view) =>
		town.Contains(feet.ToPoint()) && !view.Contains(feet.ToPoint());

	public static bool IsExit(CrowdActivity activity) => activity is CrowdActivity.BoardTrain or CrowdActivity.EnterBuilding
		or CrowdActivity.LeavePylon or CrowdActivity.LeaveStreet or CrowdActivity.EnterGuild;

	public static float Weight(CrowdActivity activity, bool departing, bool recent, int occupied, int capacity, float distance, float preference)
	{
		bool returningThroughPylon = departing && activity == CrowdActivity.LeavePylon;
		if ((recent && !returningThroughPylon) || occupied >= capacity || IsExit(activity) != departing)
		{
			return 0;
		}
		return (returningThroughPylon ? 8 : 1) * preference / (1 + distance / 480) / (1 + occupied * 2);
	}

	public static bool CanFinishTrainWalk(bool stopped, int timer, int duration, int travelTicks) =>
		stopped && timer >= 30 && travelTicks >= 0 && travelTicks + 90 < duration - timer;
}
