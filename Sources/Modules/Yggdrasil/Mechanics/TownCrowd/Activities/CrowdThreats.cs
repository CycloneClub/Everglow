using Everglow.Yggdrasil.YggdrasilTown.Events;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Terraria.GameContent.Events;

namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>Shared local danger snapshot, plus bounded post-battle spawn throttling.</summary>
internal static class CrowdThreats
{
	private sealed record CrowdThreat(Vector2 Center, float Radius, bool IgnoresWalls);
	private sealed class CrowdRecovery(long tick)
	{
		public long LastDanger = tick;
		public long NextArrival = tick + 300;
	}

	private static readonly List<CrowdThreat> Sources = [];
	private static readonly List<Rectangle> Areas = [];
	private static readonly Dictionary<Rectangle, CrowdRecovery> Recoveries = [];
	private static long nextScan;

	internal static bool FightingBoss(NPC npc) => npc.active && npc.boss && npc.life > 0
		&& (npc.ModNPC is TownNPC_LiveInYggdrasil town
			? town.ArenaFighting && town.StartedFight && !town.Fail : !npc.friendly);

	internal static bool Enemy(NPC npc) => npc.active && !npc.friendly && npc.life > 0 && npc.damage > 0
		&& !npc.townNPC && npc.type != NPCID.TargetDummy && !npc.CountsAsACritter;

	internal static bool EnemyNear(Vector2 center) => Main.npc.Any(npc => Enemy(npc)
		&& Vector2.DistanceSquared(npc.Center, center) < 480 * 480);

	public static void Update(long clock)
	{
		if (clock < nextScan)
		{
			return;
		}
		nextScan = clock + 6;
		Sources.Clear();
		Areas.Clear();
		foreach (var stale in Recoveries.Where(pair => clock - pair.Value.LastDanger >= 600).Select(pair => pair.Key).ToArray())
		{
			Recoveries.Remove(stale);
		}
		foreach (var npc in Main.npc)
		{
			bool boss = FightingBoss(npc);
			if (boss || Enemy(npc))
			{
				Sources.Add(new CrowdThreat(npc.Center, (boss ? 2000 : 480) + Math.Max(npc.width, npc.height) * 0.5f, boss));
				if (boss)
				{
					RememberBattle(new Rectangle((int)npc.Center.X - 2000, (int)npc.Center.Y - 2000, 4000, 4000), clock);
				}
			}
		}
		foreach (var projectile in Main.projectile)
		{
			if (projectile.active && projectile.hostile && projectile.damage > 0)
			{
				Sources.Add(new CrowdThreat(projectile.Center, 160, false));
			}
		}
		Areas.AddRange(EventAreas());
		foreach (var area in Areas)
		{
			RememberBattle(area, clock);
		}
	}

	private static IEnumerable<Rectangle> EventAreas()
	{
		if (ModContent.GetInstance<DrunkenMinerInvasion>().Active)
		{
			yield return CrowdPlaces.TownBounds;
		}
		int surface = Math.Max(0, (int)(Main.worldSurface * 16));
		if (Main.bloodMoon || Main.eclipse || Main.pumpkinMoon || Main.snowMoon)
		{
			yield return new Rectangle(0, 0, Main.maxTilesX * 16, surface + 320);
		}
		if (Main.invasionType > 0 && Main.invasionSize > 0)
		{
			yield return new Rectangle((int)(Main.invasionX * 16) - 3200, 0, 6400, surface + 800);
		}
		if (DD2Event.Ongoing)
		{
			foreach (var crystal in Main.npc.Where(npc => npc.active && npc.type == NPCID.DD2EterniaCrystal))
			{
				yield return new Rectangle((int)crystal.Center.X - 2400, (int)crystal.Center.Y - 1200, 4800, 2400);
			}
		}
	}

	private static void RememberBattle(Rectangle area, long clock)
	{
		// Quantized footprints remember a moving boss's recent battlefield without unbounded history.
		int left = (int)MathF.Floor(area.Left / 128f) * 128;
		int top = (int)MathF.Floor(area.Top / 128f) * 128;
		area = new Rectangle(left, top, (int)MathF.Ceiling(area.Right / 128f) * 128 - left,
			(int)MathF.Ceiling(area.Bottom / 128f) * 128 - top);
		if (!Recoveries.TryGetValue(area, out var recovery))
		{
			if (Recoveries.Count >= 128)
			{
				Recoveries.Remove(Recoveries.MinBy(pair => pair.Value.LastDanger).Key);
			}
			Recoveries.Add(area, recovery = new CrowdRecovery(clock));
		}
		recovery.LastDanger = clock;
		recovery.NextArrival = clock + 300;
	}

	public static bool Dangerous(Vector2 feet) => Areas.Any(area => area.Contains(feet.ToPoint())) || Sources.Any(source =>
		Vector2.DistanceSquared(feet, source.Center) <= source.Radius * source.Radius
		&& (source.IgnoresWalls || Collision.CanHitLine(feet - new Vector2(0, 24), 1, 1, source.Center, 1, 1)));

	public static float Clearance(Vector2 point)
	{
		float clearance = float.PositiveInfinity;
		foreach (var source in Sources)
		{
			clearance = Math.Min(clearance, Vector2.Distance(point, source.Center) - source.Radius);
		}
		foreach (var area in Areas)
		{
			float distance = AreaClearance(area, point);
			clearance = Math.Min(clearance, distance);
		}
		return clearance;
	}

	private static float AreaClearance(Rectangle area, Vector2 point) => area.Contains(point.ToPoint())
		? -Math.Min(Math.Min(point.X - area.Left, area.Right - point.X), Math.Min(point.Y - area.Top, area.Bottom - point.Y))
		: Vector2.Distance(point, Vector2.Clamp(point, area.TopLeft(), area.BottomRight()));

	public static bool EscapeAllowed(Vector2 from, Vector2 point) => Sources.All(source =>
		Vector2.Distance(point, source.Center) - source.Radius >= Math.Min(0, Vector2.Distance(from, source.Center) - source.Radius) - 16)
		&& Areas.All(area => AreaClearance(area, point) >= Math.Min(0, AreaClearance(area, from)) - 16);

	public static int AwayDirection(Vector2 from, int fallback)
	{
		var nearest = Sources.MinBy(source => Vector2.Distance(from, source.Center) - source.Radius);
		return nearest != null && Math.Abs(from.X - nearest.Center.X) > 8 ? Math.Sign(from.X - nearest.Center.X) : fallback;
	}

	public static bool SpawnAllowed(Vector2 point)
	{
		// Also check live major events: activation can occur after the current tick's shared snapshot.
		if (Dangerous(point) || EventAreas().Any(area => area.Contains(point.ToPoint()))
			|| Main.npc.Any(npc => FightingBoss(npc) && Vector2.DistanceSquared(point, npc.Center) <= 2000 * 2000))
		{
			return false;
		}
		return !Recoveries.Any(pair => pair.Key.Contains(point.ToPoint()) && TownCrowdSystem.Clock < pair.Value.NextArrival);
	}

	public static void RecordArrival(Vector2 point)
	{
		foreach (var pair in Recoveries.Where(pair => pair.Key.Contains(point.ToPoint())))
		{
			pair.Value.NextArrival = TownCrowdSystem.Clock + 180;
		}
	}

	public static void Clear()
	{
		Sources.Clear();
		Areas.Clear();
		Recoveries.Clear();
		nextScan = 0;
	}
}
