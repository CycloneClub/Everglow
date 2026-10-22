using Everglow.Commons.CustomTiles;
using Everglow.Yggdrasil.YggdrasilTown.CustomTiles;
using Terraria.GameInput;

namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>Client-configured town life simulated without native NPC slots.</summary>
public class TownCrowdSystem : ModSystem
{
	private static readonly List<CrowdAgent> Travelers = [];
	private static readonly CrowdPlaces Places = new();
	private static readonly Dictionary<YggdrasilTown_NormalMetro, CrowdStopBatch> Stops = [];
	private static readonly Dictionary<YggdrasilTown_NormalMetro, CrowdTrainLoad> TrainLoads = [];
	private static readonly Dictionary<string, long> EntranceCooldowns = [];
	private static readonly CrowdRegions Regions = new();
	private static Rectangle previousView;
	private static int spawnTimer;
	private static int planningBudget;
	private static long clock;

	public static bool IsRunning { get; private set; }

	public static CrowdSettings Settings { get; private set; } = CrowdSettings.ForDensity(CrowdDensity.Standard);

	internal static long Clock => clock;

	internal static int PlanningBudget => planningBudget;

	internal static IEnumerable<CrowdAgent> ConversationPeers(CrowdAgent visitor) => Travelers
		.Where(peer => peer != visitor && peer.CanConverse && CrowdConversation.InRange(visitor.Body, peer.Body))
		.OrderBy(peer => Vector2.DistanceSquared(visitor.Body.Center, peer.Body.Center));

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

	public static bool Enabled => IsRunning && !Main.dedServ
		&& CrowdRules.CanRun(Main.netMode, YggdrasilWorld.InYggdrasil);

	private static void UpdateConfiguration(CrowdConfig config)
	{
		if (Main.dedServ || !config.Enabled || !CrowdRules.CanRun(Main.netMode, YggdrasilWorld.InYggdrasil))
		{
			if (IsRunning || Travelers.Count != 0)
			{
				Clear();
			}
			return;
		}
		Settings = CrowdSettings.ForDensity(config.Density);
		if (!IsRunning)
		{
			Places.Initialize();
			// Enabling while looking at a street must not materialize its initial population.
			previousView = CurrentView;
			Regions.Observe(previousView, clock);
			IsRunning = true;
		}
	}

	public override void OnWorldLoad() => Clear();

	public override void OnWorldUnload() => Clear();

	public override void Unload() => Clear();

	public override void PostDrawInterface(SpriteBatch spriteBatch)
	{
		if (!Enabled || Main.gameMenu || !Main.hasFocus || Main.mapFullscreen || Main.playerInventory
			|| Main.blockInput || Main.LocalPlayer.dead || Main.LocalPlayer.mouseInterface)
		{
			return;
		}
		var mouse = Vector2.Transform(
			new Vector2(PlayerInput.MouseInfo.X, PlayerInput.MouseInfo.Y),
			Matrix.Invert(Main.GameViewMatrix.TransformationMatrix)) + Main.screenPosition;
		for (int i = Travelers.Count - 1; i >= 0; i--)
		{
			if (Travelers[i].CanShowNameAt(mouse))
			{
				Main.instance.MouseText(Travelers[i].DisplayName);
				break;
			}
		}
	}

	public override void PreUpdateNPCs()
	{
		planningBudget = 3;
		clock++;
	}

	private static Rectangle CurrentView => CrowdLifeRules.CameraBounds(
		Main.screenPosition,
		new Vector2(Main.screenWidth, Main.screenHeight), Main.GameViewMatrix.TransformationMatrix);

	public override void PostDrawTiles()
	{
		if (!Enabled || Main.gameMenu)
		{
			return;
		}
		// Observe the final camera before people draw (PostDrawDusts), including teleportation and zoom.
		var view = CurrentView;
		var exposed = previousView;
		exposed.Inflate(32, 56);
		previousView = view;
		var fresh = Regions.Observe(view, clock);
		if (fresh.Length == 0 || Settings.InitialFill == 0 || !HasCapacity)
		{
			return;
		}
		Places.DiscoverNear(view);
		foreach (var region in fresh.OrderBy(region => Vector2.DistanceSquared(region.Center.ToVector2(), Main.LocalPlayer.Center)))
		{
			int existing = Travelers.Count(person => person.Active && region.Contains(person.Body.Bottom.ToPoint()));
			for (int count = existing; count < Settings.InitialFill && HasCapacity && planningBudget > 0; count++)
			{
				for (int attempt = 0; attempt < 8 && planningBudget > 0; attempt++)
				{
					var near = new Vector2(Main.rand.Next(region.Left, region.Right), Main.rand.Next(region.Top, region.Bottom));
					if (CrowdTerrain.TryGround(near, out var source, 3, 24)
						&& !exposed.Contains(CrowdTerrain.Feet(source).ToPoint())
						&& region.Contains(CrowdTerrain.Feet(source).ToPoint()) && Places.CanSeed(CrowdTerrain.Feet(source))
						&& TrySpawnFrom(source, CrowdArrival.Existing, null))
					{
						break;
					}
				}
			}
		}
	}

	public override void PostUpdateNPCs()
	{
		UpdateConfiguration(Main.dedServ ? null : ModContent.GetInstance<CrowdConfig>());
		if (!Enabled)
		{
			return;
		}
		CrowdThreats.Update(clock);
		foreach (var person in Travelers)
		{
			person.Update();
		}
		Travelers.RemoveAll(person => !person.Active);
		Places.Update();
		ReceiveTrainPassengers();
		if (++spawnTimer >= Settings.SpawnIntervalTicks)
		{
			spawnTimer = 0;
			TrySpawn();
		}
	}

	private static bool HasCapacity => Travelers.Count < Settings.PopulationLimit;

	internal static bool Visible(Entity body) => CrowdPlaces.ViewBounds.Intersects(body.Hitbox);

	internal static bool ValidPlace(CrowdPlace place) => Places.Valid(place);

	internal static List<Point> FindRoute(Point start, Point goal, int seed, YggdrasilTown_NormalMetro train = null)
	{
		if (planningBudget <= 0)
		{
			return null;
		}
		planningBudget--;
		return CrowdTerrain.FindRoute(start, goal, seed, train,
			point => !CrowdThreats.Dangerous(CrowdTerrain.Feet(point)));
	}

	internal static bool TryPlan(Point start, CrowdAgent traveler, bool departing, out CrowdPlace destination,
		out List<Point> path, string exclude = null, CrowdIntent intent = CrowdIntent.VisitTown)
	{
		destination = null;
		path = [];
		Vector2 from = CrowdTerrain.Feet(start);
		bool Occupied(Point cell) => Travelers.Any(other => other != traveler && other.ReservesStandingPoint(cell));
		var candidates = Places.Candidates(from, departing)
			.Where(place => CrowdThreats.SpawnAllowed(CrowdTerrain.Feet(place.Cell)))
			.Where(place => intent switch
			{
				CrowdIntent.ReturnHome => place.Activity == CrowdActivity.LeavePylon,
				CrowdIntent.EnterGuild => place.Activity == CrowdActivity.EnterGuild,
				CrowdIntent.TakeTrain => place.Activity == CrowdActivity.BoardTrain,
				_ => true,
			})
			.Where(place => place.Activity != CrowdActivity.EnterGuild || EntranceReady(place.Key))
			.Select(place => Places.TryStandingPlace(place, Occupied, out var standingPlace) ? standingPlace : null)
			.Where(place => place != null && place.Key != exclude)
			.Select(place => (Place: place, Weight: CrowdActivityRules.Weight(place.Activity, departing,
				intent != CrowdIntent.TakeTrain && (traveler?.RecentlyVisited(place.Key) ?? false),
				Travelers.Count(other => other != traveler && other.Active && other.DestinationKey == place.Key),
				place.Capacity, Vector2.Distance(from, CrowdTerrain.Feet(place.Cell)), traveler?.Preference(place.Activity) ?? 1)))
			.Where(candidate => candidate.Weight > 0 && (departing || candidate.Place.Activity == CrowdActivity.Sit
				|| Vector2.DistanceSquared(from, CrowdTerrain.Feet(candidate.Place.Cell)) >= 96 * 96))
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

	internal static bool PlanIntent(Point start, CrowdAgent traveler, CrowdIntent intent,
		out CrowdPlace destination, out List<Point> path, string exclude = null)
		=> TryPlan(start, traveler, intent != CrowdIntent.VisitTown, out destination, out path, exclude, intent);

	internal static bool TryEscape(Point start, int preferredDirection, int seed, out List<Point> path, int attempt = 0)
	{
		path = [];
		Vector2 from = CrowdTerrain.Feet(start);
		float minimum = Math.Min(0, CrowdThreats.Clearance(from)) - 16;
		var candidates = new List<Point>();
		foreach (int distance in new[] { 12, 24, 40, 64, 2, 4, 8 })
		{
			foreach (int direction in new[] { preferredDirection, -preferredDirection })
			{
				if (CrowdTerrain.TryGround(from + new Vector2(direction * distance * 16, 0), out var cell, 2, 12)
					&& CrowdThreats.Clearance(CrowdTerrain.Feet(cell)) >= minimum + 16
					&& CrowdThreats.EscapeAllowed(from, CrowdTerrain.Feet(cell)))
				{
					candidates.Add(cell);
				}
			}
		}
		float Score(Point cell)
		{
			Vector2 feet = CrowdTerrain.Feet(cell);
			return Math.Min(4096, CrowdThreats.Clearance(feet)) - Vector2.Distance(feet, from) * 0.2f
				+ (CrowdPlaces.ViewBounds.Contains(feet.ToPoint()) ? 0 : 240)
				+ (Math.Sign(cell.X - start.X) == preferredDirection ? 64 : 0);
		}
		var ordered = candidates.Distinct().OrderByDescending(Score).ToArray();
		for (int index = 0; index < ordered.Length; index++)
		{
			var cell = ordered[(index + (int)((uint)attempt % (uint)ordered.Length)) % ordered.Length];
			if (planningBudget <= 0)
			{
				path = null;
				return false;
			}
			planningBudget--;
			path = CrowdTerrain.FindRoute(start, cell, seed, allowed: point => CrowdThreats.EscapeAllowed(from, CrowdTerrain.Feet(point)));
			if (path.Count > 1)
			{
				return true;
			}
		}
		return false;
	}

	internal static bool EntranceReady(string key) => !EntranceCooldowns.TryGetValue(key, out long last)
		|| clock - last >= CrowdSettings.DoorCooldownTicks;

	internal static bool TryEnterGuild(string key)
	{
		if (!EntranceReady(key))
		{
			return false;
		}
		EntranceCooldowns[key] = clock;
		return true;
	}

	private static bool TrySpawn()
	{
		if (!Enabled || !HasCapacity)
		{
			return false;
		}
		Point source;
		CrowdArrival arrival = CrowdArrival.Street;
		CrowdPlace entrance = null;
		// Pylons have their own rare attempt, never a fallback for failed street spawns.
		int sourceKind = CrowdSettings.ChooseSource(Main.rand.Next(CrowdSettings.TotalSourceWeight));
		if (sourceKind > 0)
		{
			arrival = (CrowdArrival)sourceKind;
			int cooldown = sourceKind == 2 ? CrowdSettings.PylonCooldownTicks : CrowdSettings.DoorCooldownTicks;
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
			var view = CrowdPlaces.ViewBounds;
			float x = Main.rand.NextBool() ? view.Left - Main.rand.Next(32, 240) : view.Right + Main.rand.Next(32, 240);
			var near = new Vector2(x, Main.LocalPlayer.Bottom.Y + Main.rand.Next(-12, 13) * 16);
			if (!CrowdTerrain.TryGround(near, out source, 8, 24)
				|| !CrowdActivityRules.CanSpawnOnStreet(CrowdTerrain.Feet(source), CrowdPlaces.TownBounds, view))
			{
				return false;
			}
		}
		return TrySpawnFrom(source, arrival, entrance);
	}

	private static bool TrySpawnFrom(Point source, CrowdArrival arrival, CrowdPlace entrance)
	{
		if (!CrowdThreats.SpawnAllowed(CrowdTerrain.Feet(source))
			|| Travelers.Any(person => Vector2.DistanceSquared(person.Body.Bottom, CrowdTerrain.Feet(source)) < 64 * 64))
		{
			return false;
		}
		var intent = CrowdLifeRules.ChooseIntent(
			arrival == CrowdArrival.Pylon,
			entrance?.Activity == CrowdActivity.EnterGuild, Main.rand.Next(100),
			CrowdSettings.ReturnPercent, CrowdSettings.GuildPercent);
		bool planned = PlanIntent(source, null, intent, out var destination, out var path, entrance?.Key);
		if (!planned || !Create(path, destination, arrival, entrance, null, intent: intent))
		{
			return false;
		}
		if (entrance != null)
		{
			EntranceCooldowns[entrance.Key] = clock;
		}
		return true;
	}

	private static bool Create(List<Point> path, CrowdPlace destination, CrowdArrival arrival,
		CrowdPlace entrance, YggdrasilTown_NormalMetro train, Vector2? aboardFeet = null, CrowdIntent intent = CrowdIntent.VisitTown)
	{
		Vector2 feet = aboardFeet ?? CrowdTerrain.Feet(path[0]);
		if (!HasCapacity || !CrowdThreats.SpawnAllowed(feet))
		{
			return false;
		}
		int seed = Main.rand.Next(1, int.MaxValue);
		var actor = new CrowdBody();
		Travelers.Add(new CrowdAgent(actor, path, destination, arrival, entrance, seed, train, aboardFeet) { Intent = intent });
		CrowdThreats.RecordArrival(feet);
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
				TrainLoads.Add(train, load = new CrowdTrainLoad());
			}
			var trainView = new Rectangle((int)train.Position.X - 172, (int)train.Position.Y - 146, (int)train.Size.X + 344, 162);
			load.Observe(CrowdTerrain.TrainStopped(train), CrowdPlaces.ViewBounds.Intersects(trainView), Travelers.Any(person => person.Aboard(train)));
			if (Vector2.DistanceSquared(train.Box.Center, Main.LocalPlayer.Center) > 3000 * 3000)
			{
				continue;
			}
			PopulateTrain(train, load);
			DispatchTrainPassengers(train);
		}
	}

	private static void PopulateTrain(YggdrasilTown_NormalMetro train, CrowdTrainLoad load)
	{
		int count = Settings.TrainInitialPassengers;
		for (int slot = 0; slot < 12 && load.Loaded < count && HasCapacity; slot++)
		{
			Vector2 feet = train.Position + new Vector2(CrowdTrainLoad.SeatOffset(slot, train.Size.X), 0);
			if (Travelers.Any(person => person.Active && Math.Abs(person.Body.Bottom.X - feet.X) < 32
				&& Math.Abs(person.Body.Bottom.Y - feet.Y) < 48))
			{
				continue;
			}
			if (!Create([CrowdTerrain.Cell(feet)], null, CrowdArrival.Train, null, train, feet))
			{
				break;
			}
			load.RecordLoaded();
		}
	}

	private static void DispatchTrainPassengers(YggdrasilTown_NormalMetro train)
	{
		if (!Stops.TryGetValue(train, out var batch))
		{
			Stops.Add(train, batch = new CrowdStopBatch());
		}
		if (!CrowdTerrain.TrainStopped(train))
		{
			batch.TryBegin(false, train.StopTimer, train.StopTimeMax);
			return;
		}
		batch.TryBegin(true, train.StopTimer, train.StopTimeMax);
		if (!batch.TryDispatch(Settings.TrainPassengers, planningBudget, out int side))
		{
			return;
		}
		// Reuse passengers already aboard. A full town does not block their disembarkation.
		var passenger = Travelers.Where(person => person.CanAlight(train))
			.OrderBy(person => side < 0 ? person.Body.Bottom.X : -person.Body.Bottom.X).FirstOrDefault();
		if (passenger == null)
		{
			return;
		}
		var station = new Vector2(train.StationX, train.Box.Top + 16);
		if (!CrowdPlaces.TryStation(station, station.X + side, out var landing))
		{
			return;
		}
		if (!CrowdThreats.SpawnAllowed(CrowdTerrain.Feet(landing)))
		{
			return;
		}
		var path = FindRoute(CrowdTerrain.Cell(passenger.Body.Bottom), landing, passenger.RouteSeed, train);
		if (path == null || path.Count == 0)
		{
			return;
		}
		if (!CrowdActivityRules.CanFinishTrainWalk(true, train.StopTimer,
			train.StopTimeMax, CrowdAgent.EstimateTravelTicks(path, passenger.WalkSpeed)))
		{
			return;
		}
		var intent = CrowdPlaces.ArrivalIntent(station);
		if (!PlanIntent(landing, passenger, intent, out _, out _))
		{
			return;
		}
		var destination = new CrowdPlace(CrowdActivity.Scenery, landing, $"alight:{landing}", CrowdTerrain.Feet(landing));
		passenger.Intent = intent;
		passenger.BeginAlighting(path, destination);
		CrowdThreats.RecordArrival(CrowdTerrain.Feet(landing));
		batch.RecordArrival();
	}

	internal static bool ObstacleAhead(CrowdAgent traveler, int direction)
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
		CrowdConversation.Clear();
		CrowdThreats.Clear();
		foreach (var person in Travelers)
		{
			person.Kill();
		}
		Travelers.Clear();
		Stops.Clear();
		TrainLoads.Clear();
		EntranceCooldowns.Clear();
		Regions.Clear();
		previousView = Rectangle.Empty;
		IsRunning = false;
		spawnTimer = 0;
		planningBudget = 0;
		clock = 0;
	}
}
