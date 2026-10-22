namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

public enum CrowdIntent
{
	VisitTown,
	ReturnHome,
	EnterGuild,
	TakeTrain,
}

public static class CrowdLifeRules
{
	// Pylon arrivals travel to town first; local activities begin after disembarking.
	public static CrowdIntent ChooseIntent(bool arrivingThroughPylon, bool leavingGuild, int roll, int returnPercent, int guildPercent)
	{
		if (arrivingThroughPylon)
		{
			return CrowdIntent.TakeTrain;
		}
		if (roll < returnPercent)
		{
			return CrowdIntent.ReturnHome;
		}
		return !leavingGuild && roll < returnPercent + guildPercent ? CrowdIntent.EnterGuild : CrowdIntent.VisitTown;
	}

	public static bool InsideGuildOpening(Point origin, Vector2 feet) => feet.X >= origin.X * 16 + 16
		&& feet.X <= origin.X * 16 + 96 && feet.Y >= origin.Y * 16 + 64 && feet.Y <= origin.Y * 16 + 128;

	public static Rectangle CameraBounds(Vector2 screenPosition, Vector2 screenSize, Matrix transform)
	{
		var inverse = Matrix.Invert(transform);
		var top = Vector2.Transform(Vector2.Zero, inverse) + screenPosition;
		var bottom = Vector2.Transform(screenSize, inverse) + screenPosition;
		return new Rectangle((int)MathF.Floor(top.X), (int)MathF.Floor(top.Y),
			(int)MathF.Ceiling(bottom.X - top.X), (int)MathF.Ceiling(bottom.Y - top.Y));
	}
}

/// <summary>Bounded exposure history. A full cache skips seeding rather than refilling recently seen space.</summary>
public sealed class CrowdRegions
{
	private readonly Dictionary<Point, long> seen = [];

	public int Count => seen.Count;

	public void Clear() => seen.Clear();

	public Rectangle[] Observe(Rectangle view, long clock)
	{
		var fresh = new List<Rectangle>();
		for (int x = (int)MathF.Floor(view.Left / 768f); x <= (int)MathF.Floor((view.Right - 1) / 768f); x++)
		{
			for (int y = (int)MathF.Floor(view.Top / 384f); y <= (int)MathF.Floor((view.Bottom - 1) / 384f); y++)
			{
				var key = new Point(x, y);
				if (!seen.ContainsKey(key))
				{
					if (seen.Count >= 128)
					{
						var stale = seen.Where(pair => clock - pair.Value > 3600
							&& !view.Intersects(Bounds(pair.Key))).OrderBy(pair => pair.Value).Take(1).ToArray();
						if (stale.Length == 0)
						{
							continue;
						}
						seen.Remove(stale[0].Key);
					}
					fresh.Add(Bounds(key));
				}
				seen[key] = clock;
			}
		}
		return fresh.ToArray();
	}

	private static Rectangle Bounds(Point key) => new(key.X * 768, key.Y * 384, 768, 384);
}
