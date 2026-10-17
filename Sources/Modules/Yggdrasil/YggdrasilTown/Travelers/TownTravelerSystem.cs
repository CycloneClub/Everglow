using Everglow.Commons.CustomTiles;
using Everglow.Yggdrasil.YggdrasilTown.CustomTiles;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

public enum TravelerMode
{
	Off,
	NPC,
	Actors,
}

/// <summary>Opt-in town life shared by native NPC and lightweight travelers.</summary>
public class TownTravelerSystem : ModSystem
{
	private static readonly List<TravelerJourney> Travelers = [];
	private static readonly TravelerPlaces Places = new();
	private static readonly Dictionary<YggdrasilTown_NormalMetro, TravelerStopBatch> Stops = [];
	private static readonly Dictionary<YggdrasilTown_NormalMetro, TravelerTrainLoad> TrainLoads = [];
	private static readonly Dictionary<string, int> TrainRejections = [];
	private static readonly Dictionary<string, long> EntranceCooldowns = [];
	private static int spawnTimer;
	private static int boarded;
	private static int disembarked;
	private static int rejectedRoutes;
	private static int planningBudget;
	private static long clock;

	public static TravelerMode Mode { get; private set; }

	public static TravelerOptions Options { get; internal set; } = new();

	public static int Population => Travelers.Count;

	internal static void DrawTrainPassengers(YggdrasilTown_NormalMetro train, SpriteBatch spriteBatch)
	{
		foreach (var person in Travelers)
		{
			if (ReferenceEquals(person.InteriorTrain, train))
			{
				person.DrawInTrain(spriteBatch);
			}
		}
	}

	public static bool Enabled => Mode != TravelerMode.Off && !Main.dedServ
		&& TravelerRules.CanRun(Main.netMode, YggdrasilWorld.InYggdrasil);

	public static string Status => $"{Mode}: {Travelers.Count} travelers, {Places.Status}; boarded={boarded}, "
		+ $"disembarked={disembarked}, rejected routes={rejectedRoutes}. "
		+ string.Join(", ", Travelers.GroupBy(person => person.Stage).Select(group => $"{group.Key}={group.Count()}"))
		+ "; train checks: " + string.Join(", ", TrainRejections.Select(pair => $"{pair.Key}={pair.Value}"));

	public static void ToggleMode(TravelerMode mode)
	{
		if (Main.dedServ)
		{
			return;
		}
		if (!TravelerRules.CanRun(Main.netMode, YggdrasilWorld.InYggdrasil))
		{
			Main.NewText("Travelers require single-player Yggdrasil / 请在单人模式进入天穹树。", Color.Orange);
			return;
		}
		SetMode(Mode == mode ? TravelerMode.Off : mode);
		Main.NewText(Mode switch
		{
			TravelerMode.NPC => "NPC travelers enabled / 已开启 NPC 旅客。",
			TravelerMode.Actors => "Lightweight travelers enabled / 已开启轻量旅客。",
			_ => "Town travelers disabled / 已关闭并清理旅客。",
		}, Color.LightSkyBlue);
	}

	public static void SetMode(TravelerMode mode)
	{
		Clear();
		Mode = mode;
		if (mode != TravelerMode.Off)
		{
			Places.Initialize();
		}
	}

	public override void OnWorldLoad() => Clear();

	public override void OnWorldUnload() => Clear();

	public override void Unload() => Clear();

	public override void PreUpdateNPCs()
	{
		// All NPC and actor journeys share a bounded pathfinding budget.
		planningBudget = 3;
		clock++;
	}

	public override void PostUpdateNPCs()
	{
		if (!Enabled)
		{
			if (Mode != TravelerMode.Off || Travelers.Count != 0)
			{
				Clear();
			}
			return;
		}
		foreach (var person in Travelers)
		{
			if (person.Actor != null)
			{
				person.Update();
			}
			else if (person.Body is NPC npc && !npc.active)
			{
				person.Kill();
			}
		}
		Travelers.RemoveAll(person => !person.Active);
		Places.Update();
		ReceiveTrainPassengers();
		if (++spawnTimer >= Options.SpawnIntervalTicks)
		{
			spawnTimer = 0;
			TrySpawn();
		}
	}

	private static bool HasCapacity => Travelers.Count < Options.PopulationLimit(Mode)
		&& (Mode != TravelerMode.NPC || Main.npc.Count(npc => npc.active) < 160);

	internal static bool Visible(Entity body) => TravelerPlaces.ViewBounds.Intersects(body.Hitbox);

	internal static bool ValidPlace(TravelerPlace place) => Places.Valid(place);

	internal static List<Point> FindRoute(Point start, Point goal, int seed, YggdrasilTown_NormalMetro train = null)
	{
		if (planningBudget <= 0)
		{
			return null;
		}
		planningBudget--;
		var path = TravelerTerrain.FindRoute(start, goal, seed, train);
		if (path.Count == 0)
		{
			rejectedRoutes++;
		}
		return path;
	}

	internal static bool TryPlan(Point start, TravelerJourney traveler, bool departing, out TravelerPlace destination,
		out List<Point> path, string exclude = null)
	{
		destination = null;
		path = null;
		Vector2 from = TravelerTerrain.Feet(start);
		bool Occupied(Point cell) => Travelers.Any(other => other != traveler && other.ReservesStandingPoint(cell));
		var candidates = Places.Candidates(from, departing)
			.Select(place => Places.TryStandingPlace(place, Occupied, out var standingPlace) ? standingPlace : null)
			.Where(place => place != null)
			.Select(place => (Place: place, Weight: TravelerActivityRules.Weight(place.Activity, departing,
				place.Key == exclude || (traveler?.RecentlyVisited(place.Key) ?? false),
				Travelers.Count(other => other != traveler && other.Active && other.DestinationKey == place.Key),
				place.Capacity, Vector2.Distance(from, TravelerTerrain.Feet(place.Cell)), traveler?.Preference(place.Activity) ?? 1)))
			.Where(candidate => candidate.Weight > 0 && (departing || Vector2.DistanceSquared(from, TravelerTerrain.Feet(candidate.Place.Cell)) >= 96 * 96))
			.Select(candidate => (candidate.Place, Order: -Math.Log(Math.Max(0.001, Main.rand.NextDouble())) / candidate.Weight))
			.OrderBy(candidate => candidate.Order).Take(6);
		foreach (var candidate in candidates)
		{
			path = FindRoute(start, candidate.Place.Cell, traveler?.RouteSeed ?? Main.rand.Next(1, int.MaxValue));
			if (path == null)
			{
				return false;
			}
			if (path.Count > 0)
			{
				destination = candidate.Place;
				return true;
			}
		}
		return false;
	}

	public static bool TrySpawn(bool atCursor = false)
	{
		if (!Enabled || !HasCapacity)
		{
			return false;
		}
		Point source;
		TravelerArrival arrival = TravelerArrival.Street;
		TravelerPlace entrance = null;
		if (atCursor)
		{
			if (!TravelerTerrain.TryGround(Main.MouseWorld, out source))
			{
				return false;
			}
		}
		else
		{
			// Pylons have their own rare attempt, never a fallback for failed street spawns.
			int sourceKind = Options.ChooseSource(Main.rand.Next(Math.Max(1, Options.TotalSourceWeight)));
			if (sourceKind < 0)
			{
				return false;
			}
			if (sourceKind > 0)
			{
				arrival = (TravelerArrival)sourceKind;
				int cooldown = (int)(Options[sourceKind == 2 ? TravelerOption.PylonCooldownSeconds : TravelerOption.DoorCooldownSeconds] * 60);
				var entrances = Places.Entrances(Main.LocalPlayer.Center, sourceKind == 2)
					.Where(place => !EntranceCooldowns.TryGetValue(place.Key, out long lastSpawn) || clock - lastSpawn >= cooldown).ToList();
				if (entrances.Count == 0)
				{
					return false;
				}
				entrance = entrances[Main.rand.Next(entrances.Count)];
				source = entrance.Cell;
			}
			else
			{
				var view = TravelerPlaces.ViewBounds;
				float x = Main.rand.NextBool() ? view.Left - Main.rand.Next(32, 240) : view.Right + Main.rand.Next(32, 240);
				var near = new Vector2(x, Main.LocalPlayer.Bottom.Y + Main.rand.Next(-12, 13) * 16);
				if (!TravelerTerrain.TryGround(near, out source, 8, 24)
					|| !TravelerActivityRules.CanSpawnOnStreet(TravelerTerrain.Feet(source), TravelerPlaces.TownBounds, view, true))
				{
					return false;
				}
			}
		}
		if (Travelers.Any(person => Vector2.DistanceSquared(person.Body.Bottom, TravelerTerrain.Feet(source)) < 64 * 64)
			|| !TryPlan(source, null, false, out var destination, out var path, entrance?.Key))
		{
			return false;
		}
		if (!Create(path, destination, arrival, entrance, null))
		{
			return false;
		}
		if (entrance != null)
		{
			EntranceCooldowns[entrance.Key] = clock;
		}
		return true;
	}

	private static bool Create(List<Point> path, TravelerPlace destination, TravelerArrival arrival,
		TravelerPlace entrance, YggdrasilTown_NormalMetro train, Vector2? aboardFeet = null)
	{
		if (!HasCapacity)
		{
			return false;
		}
		int seed = Main.rand.Next(1, int.MaxValue);
		if (Mode == TravelerMode.Actors)
		{
			var actor = new TravelerActor();
			Travelers.Add(new TravelerJourney(actor.Body, actor, path, destination, arrival, entrance, seed, train, aboardFeet));
		}
		else
		{
			Vector2 feet = TravelerTerrain.Feet(path[0]);
			int index = NPC.NewNPC(new EntitySource_Misc("TownTravelerExperiment"), (int)feet.X, (int)feet.Y, ModContent.NPCType<TravelerNPC>());
			if (index >= Main.maxNPCs)
			{
				return false;
			}
			var npc = Main.npc[index];
			var journey = new TravelerJourney(npc, null, path, destination, arrival, entrance, seed, train, aboardFeet);
			npc.noTileCollide = journey.IsAttached;
			((TravelerNPC)npc.ModNPC).Journey = journey;
			Travelers.Add(journey);
		}
		return true;
	}

	private static void ReceiveTrainPassengers()
	{
		foreach (var stale in TrainLoads.Keys.Where(train => !train.Active).ToArray())
		{
			TrainLoads.Remove(stale);
		}
		foreach (var stale in Stops.Keys.Where(train => !train.Active).ToArray())
		{
			Stops.Remove(stale);
		}
		foreach (var train in ColliderManager.Instance.OfType<YggdrasilTown_NormalMetro>().Where(train => train.Active))
		{
			if (!TrainLoads.TryGetValue(train, out var load))
			{
				TrainLoads.Add(train, load = new TravelerTrainLoad());
			}
			var trainView = new Rectangle((int)train.Position.X - 172, (int)train.Position.Y - 146, (int)train.Size.X + 344, 162);
			load.Observe(TravelerTerrain.TrainStopped(train), TravelerPlaces.ViewBounds.Intersects(trainView), Travelers.Any(person => person.Aboard(train)));
			if (Vector2.DistanceSquared(train.Box.Center, Main.LocalPlayer.Center) > 3000 * 3000)
			{
				continue;
			}
			int count = (int)Options[TravelerOption.TrainInitialPassengers];
			for (int slot = 0; slot < 12 && load.Loaded < count && HasCapacity; slot++)
			{
				Vector2 feet = train.Position + new Vector2(TravelerTrainLoad.SeatOffset(slot, train.Size.X), 0);
				if (Travelers.Any(person => person.Active && Math.Abs(person.Body.Bottom.X - feet.X) < 32
					&& Math.Abs(person.Body.Bottom.Y - feet.Y) < 48))
				{
					continue;
				}
				if (!Create([TravelerTerrain.Cell(feet)], null, TravelerArrival.Train, null, train, feet))
				{
					break;
				}
				load.RecordLoaded();
			}
			if (!Stops.TryGetValue(train, out var batch))
			{
				Stops.Add(train, batch = new TravelerStopBatch());
			}
			if (!TravelerTerrain.TrainStopped(train))
			{
				batch.TryBegin(false, train.StopTimer, train.StopTimeMax);
				continue;
			}
			batch.TryBegin(true, train.StopTimer, train.StopTimeMax);
			if (!batch.TryDispatch((int)Options[TravelerOption.TrainPassengers], planningBudget, out int side))
			{
				continue;
			}
			// Reuse passengers already aboard. A full town does not block their disembarkation.
			var passenger = Travelers.Where(person => person.CanAlight(train))
				.OrderBy(person => side < 0 ? person.Body.Bottom.X : -person.Body.Bottom.X).FirstOrDefault();
			if (passenger == null)
			{
				RejectTrain("no eligible rider");
				continue;
			}
			var station = new Vector2(train.StationX, train.Box.Top + 16);
			if (!TravelerPlaces.TryStation(station, station.X + side, out var landing))
			{
				RejectTrain("no safe landing");
				continue;
			}
			var path = FindRoute(TravelerTerrain.Cell(passenger.Body.Bottom), landing, passenger.RouteSeed, train);
			if (path == null || path.Count == 0)
			{
				RejectTrain(path == null ? "planning budget" : "blocked deck route");
				continue;
			}
			if (!TravelerActivityRules.CanFinishTrainWalk(true, train.StopTimer,
				train.StopTimeMax, TravelerJourney.EstimateTravelTicks(path, passenger.WalkSpeed)))
			{
				RejectTrain("insufficient stop time");
				continue;
			}
			if (!TryPlan(landing, null, false, out _, out _))
			{
				RejectTrain("no reachable town activity");
				continue;
			}
			var destination = new TravelerPlace(TravelerActivity.Scenery, landing, $"alight:{landing}", TravelerTerrain.Feet(landing));
			passenger.BeginAlighting(path, destination);
			batch.RecordArrival();
		}
	}

	private static void RejectTrain(string reason) => TrainRejections[reason] = TrainRejections.GetValueOrDefault(reason) + 1;

	internal static void RecordAlighting() => disembarked++;

	internal static void RecordBoarding() => boarded++;

	internal static bool ObstacleAhead(TravelerJourney traveler, int direction)
	{
		bool Ahead(Entity other) => !ReferenceEquals(other, traveler.Body)
			&& (other.Center.X - traveler.Body.Center.X) * direction > 0
			&& (other.Center.X - traveler.Body.Center.X) * direction < 28
			&& Math.Abs(other.Bottom.Y - traveler.Body.Bottom.Y) < 24;
		return Main.player.Any(player => player.active && !player.dead && Ahead(player))
			|| Main.npc.Any(npc => npc.active && Ahead(npc))
			|| Travelers.Any(other => other.Active && !other.IsAttached && Ahead(other.Body));
	}

	private static void Clear()
	{
		foreach (var person in Travelers)
		{
			person.Kill();
		}
		Travelers.Clear();
		Stops.Clear();
		TrainLoads.Clear();
		TrainRejections.Clear();
		EntranceCooldowns.Clear();
		Mode = TravelerMode.Off;
		spawnTimer = 0;
		boarded = 0;
		disembarked = 0;
		rejectedRoutes = 0;
		planningBudget = 0;
		clock = 0;
	}
}
