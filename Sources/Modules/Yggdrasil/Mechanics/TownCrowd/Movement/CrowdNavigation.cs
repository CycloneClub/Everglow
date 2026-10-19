namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

public static class CrowdNavigation
{
	public const float JumpSpeed = 2f;
	public const float JumpVelocity = -6.1f;
	public const float Gravity = 0.3f;
	public const int MaxDropTiles = 3;

	/// <summary>
	/// Finds supported landing cells, including both endpoints. A cell's feet are at
	/// (X * 16 + 8, Y * 16 + 16); isClear checks the complete 16 by 48 body.
	/// Delegates must reject cells outside their world or search region.
	/// </summary>
	public static List<Point> FindPath(Point start, Point goal, Func<Point, bool> canStand, Func<Point, bool> isClear, int maxVisited = 6000,
		Func<Point, bool> canDropThrough = null, int routeSeed = 0)
	{
		if (maxVisited <= 0)
		{
			return [];
		}
		var clearance = new Dictionary<Point, bool>();
		var support = new Dictionary<Point, bool>();
		bool Clear(Point point)
		{
			if (!clearance.TryGetValue(point, out bool result))
			{
				result = isClear(point);
				clearance.Add(point, result);
			}
			return result;
		}
		bool Stand(Point point)
		{
			if (!support.TryGetValue(point, out bool result))
			{
				result = Clear(point) && canStand(point);
				support.Add(point, result);
			}
			return result;
		}
		if (!Stand(start) || !Stand(goal))
		{
			return [];
		}

		var frontier = new PriorityQueue<Point, int>();
		var cost = new Dictionary<Point, int> { [start] = 0 };
		var previous = new Dictionary<Point, Point>();
		var visited = new HashSet<Point>();
		frontier.Enqueue(start, Distance(start, goal) * 10);
		while (visited.Count < maxVisited && frontier.TryDequeue(out var current, out _))
		{
			if (!visited.Add(current))
			{
				continue;
			}
			if (current == goal)
			{
				var path = new List<Point> { current };
				while (previous.TryGetValue(current, out var parent))
				{
					current = parent;
					path.Add(current);
				}
				path.Reverse();
				return path;
			}
			for (int dx = -3; dx <= 3; dx++)
			{
				for (int dy = -3; dy <= 3; dy++)
				{
					var next = new Point(current.X + dx, current.Y + dy);
					if (visited.Contains(next) || !Stand(next))
					{
						continue;
					}
					bool walk = CanWalkBetween(current, next, Clear, Stand, canDropThrough);
					bool drop = CanDropBetween(current, next, Clear, Stand, canDropThrough);
					if (!walk && !drop && (dx == 0 || !CanJumpBetween(current, next, Clear, Stand)))
					{
						continue;
					}
					int distance = Math.Abs(dx) + Math.Abs(dy);
					int nextCost = cost[current] + distance * (10 + Preference(next, routeSeed)) + (walk || drop ? 0 : 80);
					if (cost.TryGetValue(next, out int oldCost) && oldCost <= nextCost)
					{
						continue;
					}
					cost[next] = nextCost;
					previous[next] = current;
					frontier.Enqueue(next, nextCost + Distance(next, goal) * 10);
				}
			}
		}
		return [];
	}

	private static int Distance(Point from, Point to) => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

	private static int Preference(Point point, int seed)
	{
		if (seed == 0)
		{
			return 0;
		}
		// Stable per traveler and road section: variation never changes edge validity.
		uint hash = unchecked((uint)(seed * 73856093 ^ (point.X / 4) * 19349663 ^ point.Y * 83492791));
		hash ^= hash >> 13;
		return (int)(hash % 13);
	}

	public static bool CanDropBetween(Point from, Point to, Func<Point, bool> isClear,
		Func<Point, bool> canStand, Func<Point, bool> canDropThrough)
	{
		if (from.X != to.X || to.Y <= from.Y || to.Y - from.Y > MaxDropTiles
			|| canDropThrough?.Invoke(from) != true || !canStand(to))
		{
			return false;
		}
		for (int y = from.Y + 1; y <= to.Y; y++)
		{
			var cell = new Point(from.X, y);
			if (!isClear(cell) || (y < to.Y && canStand(cell)))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>Checks a supported edge's walking clearance; callers verify support separately.</summary>
	public static bool CanWalkBetween(Point from, Point to, Func<Point, bool> isClear, Func<Point, bool> canStand = null,
		Func<Point, bool> isPlatform = null)
	{
		if (Math.Abs(to.X - from.X) != 1 || to.Y < from.Y - 1 || to.Y > from.Y + MaxDropTiles || !isClear(from) || !isClear(to))
		{
			return false;
		}
		if (to.Y < from.Y)
		{
			if (isPlatform?.Invoke(to) == true)
			{
				// StepUp does not lift these bodies onto one-way platforms. Use the checked jump instead.
				return false;
			}
			// Lift before moving onto a one-cell step, rather than cutting its corner.
			return isClear(new Point(from.X, to.Y)) && isClear(to);
		}
		// Move off the edge first, then check every cell of the bounded drop.
		for (int y = from.Y; y <= to.Y; y++)
		{
			if (!isClear(new Point(to.X, y)))
			{
				return false;
			}
			// A one-way platform is clear to the body, but still catches a descending traveler.
			if (y < to.Y && canStand?.Invoke(new Point(to.X, y)) == true)
			{
				return false;
			}
		}
		return true;
	}

	public static bool CanJumpBetween(Point from, Point to, Func<Point, bool> isClear, Func<Point, bool> canStand)
	{
		if (Math.Abs(to.X - from.X) > 3 || Math.Abs(to.Y - from.Y) > MaxDropTiles || !canStand(to))
		{
			return false;
		}
		// Match the controller: gravity precedes movement and horizontal motion
		// stops at the landing column while the traveler waits to descend.
		float x = 0;
		float y = 0;
		float targetX = (to.X - from.X) * 16;
		float targetY = (to.Y - from.Y) * 16;
		float velocityY = JumpVelocity;
		for (int tick = 0; tick < 80; tick++)
		{
			float oldX = x;
			float oldY = y;
			x += Math.Clamp(targetX - x, -JumpSpeed, JumpSpeed);
			velocityY += Gravity;
			y += velocityY;
			bool lands = velocityY > 0 && x == targetX && oldY <= targetY && y >= targetY;
			if (lands)
			{
				// Terrain collision stops the final frame at the floor. Including
				// the overshoot would reject the supporting ground itself.
				y = targetY;
			}
			if (velocityY > 0)
			{
				for (int column = (int)Math.Floor(Math.Min(oldX, x) / 16); column <= (int)Math.Ceiling(Math.Max(oldX, x) / 16); column++)
				{
					for (int level = (int)Math.Ceiling(oldY / 16); level <= (int)Math.Floor(y / 16); level++)
					{
						var landing = new Point(from.X + column, from.Y + level);
						if (canStand(landing) && !(lands && landing.Y == to.Y))
						{
							return false;
						}
					}
				}
			}
			// Enclose the complete per-frame body sweep, including both columns
			// occupied between tile centers, so thin obstacles cannot be skipped.
			for (int cellX = from.X + (int)Math.Floor(Math.Min(oldX, x) / 16); cellX <= from.X + (int)Math.Ceiling(Math.Max(oldX, x) / 16); cellX++)
			{
				for (int cellY = from.Y + (int)Math.Floor(Math.Min(oldY, y) / 16); cellY <= from.Y + (int)Math.Ceiling(Math.Max(oldY, y) / 16); cellY++)
				{
					if (!isClear(new Point(cellX, cellY)))
					{
						return false;
					}
				}
			}
			if (lands)
			{
				return true;
			}
			if (velocityY > 0 && y > targetY)
			{
				return false;
			}
		}
		return false;
	}
}
