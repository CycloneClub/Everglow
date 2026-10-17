namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

/// <summary>Pure rules shared by both experimental traveler backends.</summary>
public static class TravelerRules
{
	public static bool SupportsGridLanding(bool active, bool solidOrPlatform, bool halfBlock, int slope) =>
		active && solidOrPlatform && !halfBlock && slope == 0;

	public static bool ReachedWaypoint(Vector2 feet, Vector2 waypoint, bool grounded) => grounded
		&& Math.Abs(feet.X - waypoint.X) <= 0.5f && Math.Abs(feet.Y - waypoint.Y) <= 2;

	public static bool CanRun(int netMode, bool inYggdrasil) => netMode == 0 && inYggdrasil;

	public static bool TryStepHeight(Vector2 feet, float width, float strideX, Vector2 surface, Vector2 size, out float rise)
	{
		rise = feet.Y - surface.Y;
		if (!(rise > 0 && rise <= 16.1f))
		{
			return false;
		}
		float left = feet.X - width / 2;
		float right = feet.X + width / 2;
		return (strideX > 0 && right <= surface.X + 0.1f && right + strideX > surface.X)
			|| (strideX < 0 && left >= surface.X + size.X - 0.1f && left + strideX < surface.X + size.X);
	}

	public static bool FallThroughSourcePlatform(float feetY, float sourceY) => feetY < sourceY + 4;

	public static bool SurfaceSupports(Vector2 feet, float width, Vector2 surface, Vector2 size, bool allowEdgeOverlap) =>
		Math.Abs(feet.Y - surface.Y) <= 2 && (allowEdgeOverlap
			? feet.X + width / 2 > surface.X && feet.X - width / 2 < surface.X + size.X
			: feet.X >= surface.X + width / 2 && feet.X <= surface.X + size.X - width / 2);

	public static bool IsSolidFloor(bool solid, bool solidTop, int frameY) => solidTop ? frameY == 0 : solid;

	public static bool CanBoard(bool active, bool stopped, Vector2 velocity, int stopTimer, int stopDuration, Vector2 feet, Vector2 door)
	{
		return active && stopped && velocity.LengthSquared() <= 0.0025f
			&& stopTimer >= 30 && stopTimer < stopDuration - 90
			&& Math.Abs(feet.X - door.X) <= 20 && Math.Abs(feet.Y - door.Y) <= 20;
	}
}
