namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>Physical route progress, independent of replanning and waypoint index resets.</summary>
public sealed class CrowdProgress
{
	private const int HistoryLimit = 64;
	private readonly Queue<Point> history = [];
	private Vector2 anchor;
	private bool initialized;

	public void Reset()
	{
		history.Clear();
		initialized = false;
	}

	public bool Observe(Vector2 position, Vector2 target, bool advancedWaypoint)
	{
		var cell = new Point((int)MathF.Floor(position.X / 16), (int)MathF.Floor(position.Y / 16));
		if (!initialized)
		{
			initialized = true;
			anchor = position;
			history.Enqueue(cell);
			return false;
		}
		float minimum = CrowdSettings.MovementProgressDistance;
		if (Vector2.DistanceSquared(anchor, position) < minimum * minimum || history.Contains(cell)
			|| (!advancedWaypoint && Vector2.Distance(anchor, target) - Vector2.Distance(position, target) < minimum))
		{
			return false;
		}
		anchor = position;
		history.Enqueue(cell);
		if (history.Count > HistoryLimit)
		{
			history.Dequeue();
		}
		return true;
	}
}
