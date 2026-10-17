using Everglow.Commons.CustomTiles;
using Everglow.Yggdrasil.YggdrasilTown.CustomTiles;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.RandomNPC;

namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

internal enum TravelerStage
{
	Arriving,
	Planning,
	Walking,
	Activity,
	Waiting,
	Boarding,
	Alighting,
	Riding,
	Leaving,
}

/// <summary>The same trip, personality and view for a native NPC or a lightweight body.</summary>
internal sealed class TravelerJourney
{
	private List<Point> route;
	private readonly VFXPerson view;
	private readonly float speed = Main.rand.NextFloat(1.2f, 1.7f);
	private readonly TravelerArrival arrival;
	private TravelerPlace destination;
	private readonly Queue<string> recentPlaces = [];
	private int completedActivities;
	private int activityDuration;
	private int planDelay;
	private readonly int routeSeed;
	private readonly int pauseInterval = Main.rand.Next(300, 600);
	private readonly int arrivalDelay = Main.rand.Next(20, 70);
	private YggdrasilTown_NormalMetro train;
	private Vector2 aboardOffset;
	private Vector2 boardingDoorOffset;
	private Vector2 progressPosition;
	private int waypoint = 1;
	private int stageTime;
	private int age;
	private int blockedTime;
	private int yieldTime;
	private int movingTime;
	private bool visitedStop;
	private bool mayAlight;
	private float stepSpeed = 1;
	private float stepOffset;
	private bool jumping;
	private Vector2 jumpTarget;
	private Vector2 previousViewPosition;
	private float animationDistance;
	private bool dropping;
	private float dropSourceY;
	private bool recoveringPlatform;
	private int pauseTime;

	public TravelerJourney(Entity body, TravelerActor actor, List<Point> path, TravelerPlace destination,
		TravelerArrival arrival, TravelerPlace entrance, int seed, YggdrasilTown_NormalMetro arrivingTrain = null, Vector2? aboardFeet = null)
	{
		Body = body;
		Actor = actor;
		route = path;
		this.destination = destination;
		this.arrival = arrival;
		train = arrivingTrain;
		if (entrance != null)
		{
			recentPlaces.Enqueue(entrance.Key);
		}
		routeSeed = seed;
		Body.Bottom = TravelerTerrain.Feet(path[0]);
		if (aboardFeet.HasValue)
		{
			Body.Bottom = aboardFeet.Value;
			aboardOffset = Body.position - train.Position;
			Stage = TravelerStage.Riding;
			mayAlight = true;
			visitedStop = TravelerTerrain.TrainStopped(train);
		}
		progressPosition = Body.position;
		previousViewPosition = Body.position;
		view = YggdrasilTownPersonManager.CreatePerson(Body.position);
		view.Opacity = aboardFeet.HasValue || arrival == TravelerArrival.Street ? 1 : 0;
		view.UpdateController = UpdateView;
		view.DrawInForeground = () => InteriorTrain == null;
		Ins.VFXManager.Add(view);
	}

	public Entity Body { get; }

	public TravelerActor Actor { get; }

	public bool Active { get; private set; } = true;

	public TravelerStage Stage { get; private set; }

	public int Direction { get; private set; } = 1;

	public bool IsAttached => Stage == TravelerStage.Riding;

	// Keep the same owner during boarding/alighting, but only once physically inside.
	internal YggdrasilTown_NormalMetro InteriorTrain => Active && train?.Active == true
		&& (IsAttached || WalkingTrain != null)
		&& Body.Center.X >= train.Position.X && Body.Center.X < train.Position.X + train.Size.X
		&& Body.Center.Y >= train.Position.Y - 146 && Body.Center.Y < train.Position.Y ? train : null;

	internal void DrawInTrain(SpriteBatch spriteBatch)
	{
		if (view.Active && view.Visible)
		{
			view.DrawInWorld(spriteBatch);
		}
	}

	internal bool CanAlight(YggdrasilTown_NormalMetro candidate) => Active && IsAttached && mayAlight && ReferenceEquals(train, candidate);

	internal bool Aboard(YggdrasilTown_NormalMetro candidate) => Active && IsAttached && ReferenceEquals(train, candidate);

	internal float WalkSpeed => speed;

	internal void BeginAlighting(List<Point> path, TravelerPlace landing)
	{
		Actor?.ReleaseSupport();
		destination = landing;
		SetRoute(path);
		mayAlight = false;
		Body.velocity = Vector2.Zero;
		if (Body is NPC npc)
		{
			npc.noTileCollide = false;
		}
		ChangeStage(TravelerStage.Alighting);
	}

	public int RouteSeed => routeSeed;

	public string DestinationKey => Stage is TravelerStage.Planning or TravelerStage.Riding ? null : destination.Key;

	internal bool ReservesStandingPoint(Point cell) => Active && DestinationKey != null
		&& Math.Abs(destination.Cell.X - cell.X) < 2 && Math.Abs(destination.Cell.Y - cell.Y) < 3;

	public bool RecentlyVisited(string key) => recentPlaces.Contains(key);

	public float Preference(TravelerActivity activity) => 0.5f + (uint)(routeSeed ^ ((int)activity * 7919)) % 7 * 0.3f;

	private YggdrasilTown_NormalMetro WalkingTrain => Stage is TravelerStage.Boarding or TravelerStage.Alighting
		|| (Stage == TravelerStage.Arriving && arrival == TravelerArrival.Train) ? train : null;

	public bool FallThroughPlatforms => dropping && (recoveringPlatform
		? Body.Bottom.Y + Math.Max(Body.velocity.Y, 0) < dropSourceY
		: TravelerRules.FallThroughSourcePlatform(Body.Bottom.Y, dropSourceY));

	public void Kill()
	{
		Active = false;
		view.Active = false;
		view.UpdateController = null;
		view.DrawInForeground = null;
		if (Body is NPC npc)
		{
			npc.active = false;
		}
	}

	public void Update()
	{
		if (!Active || !TownTravelerSystem.Enabled)
		{
			Kill();
			return;
		}
		age++;
		stageTime++;
		if (!IsAttached && TravelerActivityRules.CanRetire(age, TownTravelerSystem.Visible(Body)))
		{
			Kill();
			return;
		}
		if (IsAttached)
		{
			Ride();
			return;
		}

		bool grounded = TravelerTerrain.Grounded(Body);
		Body.velocity.X = 0;
		if (Stage == TravelerStage.Planning && !recoveringPlatform && TravelerTerrain.TryPlatformRecovery(Body, out float landingY))
		{
			recoveringPlatform = true;
			dropping = true;
			dropSourceY = landingY;
		}
		if (recoveringPlatform)
		{
			// A changed floor or an intervening moving surface must not trap the recovery state.
			if (grounded || Body.Bottom.Y > dropSourceY + 2)
			{
				recoveringPlatform = false;
				dropping = false;
				planDelay = 0;
			}
			else
			{
				Body.velocity.Y = Math.Min(Body.velocity.Y + TravelerNavigation.Gravity, 8);
				Actor?.Move(FallThroughPlatforms);
				return;
			}
		}
		if (grounded && stageTime % 60 == 0 && Stage is TravelerStage.Walking or TravelerStage.Activity or TravelerStage.Waiting
			&& !TownTravelerSystem.ValidPlace(destination))
		{
			ChangeStage(TravelerStage.Planning);
		}
		switch (Stage)
		{
			case TravelerStage.Arriving:
				view.Opacity = arrival == TravelerArrival.Street ? 1 : MathHelper.Clamp(stageTime / 30f, 0, 1);
				if (arrival == TravelerArrival.Pylon && stageTime <= 35)
				{
					Dust.NewDust(Body.position, Body.width, Body.height, DustID.MagicMirror, 0, -1.5f, 100);
					Lighting.AddLight(Body.Center, new Vector3(0.35f, 0.65f, 0.9f) * (1 - stageTime / 36f));
				}
				if (arrival == TravelerArrival.Train && !TravelerTerrain.TrainStopped(train))
				{
					RecoverFromTrain();
				}
				else if (stageTime >= (arrival == TravelerArrival.Train ? 30 : (arrival == TravelerArrival.Pylon ? 90 : 30) + arrivalDelay))
				{
					ChangeStage(arrival == TravelerArrival.Train ? TravelerStage.Alighting : TravelerStage.Walking);
				}
				break;
			case TravelerStage.Planning:
				if (grounded && TravelerTerrain.TryStandingRecovery(Body, out float standingX))
				{
					Direction = Math.Sign(standingX - Body.Bottom.X);
					Body.velocity.X = MathHelper.Clamp(standingX - Body.Bottom.X, -speed, speed);
				}
				else if (grounded && --planDelay <= 0)
				{
					ChooseNextActivity();
				}
				break;
			case TravelerStage.Walking:
				if (MoveAlongRoute(grounded))
				{
					ReachDestination();
				}
				break;
			case TravelerStage.Activity:
				Direction = destination.Focus.X >= Body.Center.X ? 1 : -1;
				if (stageTime >= activityDuration)
				{
					completedActivities++;
					Remember(destination.Key);
					ChangeStage(TravelerStage.Planning);
				}
				break;
			case TravelerStage.Waiting:
				WaitForTrain();
				break;
			case TravelerStage.Boarding:
				Board(grounded);
				break;
			case TravelerStage.Alighting:
				if (!TravelerTerrain.TrainStopped(train) || train.StopTimer >= train.StopTimeMax - 30)
				{
					RecoverFromTrain();
				}
				else if (MoveAlongRoute(grounded))
				{
					TownTravelerSystem.RecordAlighting();
					train = null;
					ChangeStage(TravelerStage.Planning);
				}
				break;
			case TravelerStage.Leaving:
				if (!TownTravelerSystem.ValidPlace(destination)
					|| !TravelerRules.ReachedWaypoint(Body.Bottom, TravelerTerrain.Feet(destination.Cell), grounded))
				{
					view.Opacity = 1;
					ChangeStage(TravelerStage.Planning);
					break;
				}
				if (destination.Activity == TravelerActivity.LeavePylon && stageTime < 35)
				{
					Dust.NewDust(Body.position, Body.width, Body.height, DustID.MagicMirror, 0, -1.5f, 100);
				}
				view.Opacity = MathHelper.Clamp(1 - stageTime / 60f, 0, 1);
				if (stageTime >= 60)
				{
					Kill();
					return;
				}
				break;
		}

		if (!IsAttached)
		{
			Body.velocity.Y = Math.Min(Body.velocity.Y + TravelerNavigation.Gravity, 8);
			if (grounded && !dropping)
			{
				Collision.StepUp(ref Body.position, ref Body.velocity, Body.width, Body.height, ref stepSpeed, ref stepOffset);
				TravelerTerrain.StepOntoSurface(Body, WalkingTrain);
			}
			stepOffset *= 0.8f;
			Actor?.Move(FallThroughPlatforms);
		}
	}

	private void Remember(string key)
	{
		recentPlaces.Enqueue(key);
		while (recentPlaces.Count > 2)
		{
			recentPlaces.Dequeue();
		}
	}

	private void ChooseNextActivity()
	{
		planDelay = Main.rand.Next(90, 180);
		bool departing = age > 10800 || (completedActivities >= 3 && Main.rand.NextBool(3));
		bool planned = TownTravelerSystem.TryPlan(TravelerTerrain.Cell(Body.Bottom), this, departing, out var next, out var path);
		if (!planned && departing)
		{
			// An unavailable exit is not a reason to freeze in view. Continue visiting and retry later.
			planned = TownTravelerSystem.TryPlan(TravelerTerrain.Cell(Body.Bottom), this, false, out next, out path);
		}
		if (planned)
		{
			destination = next;
			SetRoute(path);
			ChangeStage(TravelerStage.Walking);
		}
		else if (stageTime > 1800)
		{
			// A small disconnected plaza may have only two activities. Retry after a long pause.
			recentPlaces.Clear();
		}
	}

	private void ReachDestination()
	{
		if (!TownTravelerSystem.ValidPlace(destination)
			|| !TravelerRules.ReachedWaypoint(Body.Bottom, TravelerTerrain.Feet(destination.Cell), TravelerTerrain.Grounded(Body)))
		{
			ChangeStage(TravelerStage.Planning);
			return;
		}
		if (destination.Activity == TravelerActivity.BoardTrain)
		{
			ChangeStage(TravelerStage.Waiting);
		}
		else if (destination.Activity == TravelerActivity.LeaveStreet)
		{
			if (!TownTravelerSystem.Visible(Body))
			{
				Kill();
			}
			else
			{
				ChangeStage(TravelerStage.Planning);
			}
		}
		else if (TravelerActivityRules.IsExit(destination.Activity))
		{
			ChangeStage(TravelerStage.Leaving);
		}
		else
		{
			activityDuration = Main.rand.Next(TownTravelerSystem.Options.ActivityMinTicks, TownTravelerSystem.Options.ActivityMaxTicks + 1);
			ChangeStage(TravelerStage.Activity);
		}
	}

	private void RecoverFromTrain()
	{
		if (TravelerTerrain.OnTrain(Body, train))
		{
			AttachToTrain(false);
		}
		else
		{
			train = null;
			ChangeStage(TravelerStage.Planning);
		}
	}

	private bool MoveAlongRoute(bool grounded)
	{
		var surfaceTrain = WalkingTrain;
		bool Clear(Point cell) => TravelerTerrain.IsClear(cell, surfaceTrain);
		bool Stand(Point cell) => TravelerTerrain.CanStand(cell, surfaceTrain);
		bool Drop(Point cell) => TravelerTerrain.CanDropThrough(cell, surfaceTrain);
		while (waypoint < route.Count && TravelerRules.ReachedWaypoint(Body.Bottom, TravelerTerrain.Feet(route[waypoint]), grounded))
		{
			waypoint++;
			dropping = false;
		}
		if (waypoint >= route.Count)
		{
			return true;
		}
		if (grounded && !Stand(route[waypoint]))
		{
			// Terrain may change after planning. Do not step off an edge towards a missing landing.
			Replan(surfaceTrain);
			return false;
		}
		if (Stage == TravelerStage.Walking && grounded && !dropping)
		{
			if (stageTime % pauseInterval == 0)
			{
				pauseTime = Main.rand.Next(20, 65);
			}
			if (pauseTime > 0)
			{
				pauseTime--;
				return false;
			}
		}
		bool drop = TravelerNavigation.CanDropBetween(route[waypoint - 1], route[waypoint], Clear, Stand, Drop);
		if (drop && grounded)
		{
			dropping = true;
			dropSourceY = TravelerTerrain.Feet(route[waypoint - 1]).Y;
		}
		bool needsJump = !drop && !TravelerNavigation.CanWalkBetween(route[waypoint - 1], route[waypoint], Clear, Stand, Drop);
		if (grounded && needsJump && !TravelerNavigation.CanJumpBetween(route[waypoint - 1], route[waypoint], Clear, Stand))
		{
			Replan(surfaceTrain);
			return false;
		}
		WalkToward(TravelerTerrain.Feet(route[waypoint]), grounded, needsJump);
		if (stageTime % 120 == 0)
		{
			blockedTime = Vector2.DistanceSquared(progressPosition, Body.position) < 64 ? blockedTime + 120 : 0;
			progressPosition = Body.position;
			if (blockedTime >= 360)
			{
				Replan(surfaceTrain);
			}
		}
		return false;
	}

	private void Replan(YggdrasilTown_NormalMetro surfaceTrain)
	{
		Body.velocity.X = 0;
		var path = TownTravelerSystem.FindRoute(TravelerTerrain.Cell(Body.Bottom), route[^1], routeSeed, surfaceTrain);
		if (path == null)
		{
			return;
		}
		if (path.Count == 0)
		{
			RecoverFromTrain();
			return;
		}
		SetRoute(path);
	}

	private void SetRoute(List<Point> path)
	{
		route = path;
		waypoint = 1;
		blockedTime = 0;
		progressPosition = Body.position;
		dropping = false;
		jumping = false;
	}

	private void WalkToward(Vector2 target, bool grounded, bool needsJump = false)
	{
		if (jumping && !grounded)
		{
			// Keep the same ballistic motion that navigation checked; do not yield in midair.
			Body.velocity.X = MathHelper.Clamp(jumpTarget.X - Body.Bottom.X, -TravelerNavigation.JumpSpeed, TravelerNavigation.JumpSpeed);
			return;
		}
		jumping = false;
		float distance = target.X - Body.Bottom.X;
		if (Math.Abs(distance) > 2)
		{
			Direction = Math.Sign(distance);
		}
		bool occupied = TownTravelerSystem.ObstacleAhead(this, Direction);
		yieldTime = occupied ? yieldTime + 1 : 0;
		if (occupied && yieldTime < 60)
		{
			return;
		}
		Body.velocity.X = MathHelper.Clamp(distance, -speed, speed) * (occupied ? 0.65f : 1f);
		if (grounded && needsJump)
		{
			jumping = true;
			jumpTarget = target;
			Body.velocity.X = MathHelper.Clamp(distance, -TravelerNavigation.JumpSpeed, TravelerNavigation.JumpSpeed);
			Body.velocity.Y = TravelerNavigation.JumpVelocity;
		}
	}

	private void WaitForTrain()
	{
		if (stageTime > 3600)
		{
			Remember(destination.Key);
			ChangeStage(TravelerStage.Planning);
			return;
		}
		if (stageTime % 30 != 0)
		{
			return;
		}
		if (Vector2.DistanceSquared(Main.LocalPlayer.Center, Body.Center) < 160 * 160)
		{
			Direction = Main.LocalPlayer.Center.X >= Body.Center.X ? 1 : -1;
		}
		foreach (var candidate in ColliderManager.Instance.OfType<YggdrasilTown_NormalMetro>())
		{
			if (!TravelerTerrain.TrainStopped(candidate) || Math.Abs(candidate.StationX - destination.Focus.X) >= 24
				|| Math.Abs(candidate.Box.Top - (destination.Focus.Y - 16)) > 4 || candidate.StopTimer < 30)
			{
				continue;
			}
			Vector2 door = new(candidate.Position.X + candidate.Size.X * (Body.Bottom.X < candidate.Box.Center.X ? 0.25f : 0.75f), candidate.Box.Top);
			var path = TownTravelerSystem.FindRoute(TravelerTerrain.Cell(Body.Bottom), TravelerTerrain.Cell(door), routeSeed, candidate);
			if (path == null || path.Count == 0 || !TravelerActivityRules.CanFinishTrainWalk(true, candidate.StopTimer,
				candidate.StopTimeMax, EstimateTravelTicks(path, speed)))
			{
				continue;
			}
			train = candidate;
			boardingDoorOffset = door - candidate.Position;
			SetRoute(path);
			ChangeStage(TravelerStage.Boarding);
			break;
		}
	}

	internal static int EstimateTravelTicks(List<Point> path, float speed)
	{
		float ticks = 0;
		for (int i = 1; i < path.Count; i++)
		{
			int dx = Math.Abs(path[i].X - path[i - 1].X);
			int dy = Math.Abs(path[i].Y - path[i - 1].Y);
			float fallTicks = MathF.Sqrt(2 * dy * 16 / TravelerNavigation.Gravity);
			ticks += Math.Max(dx * 16 / speed, fallTicks) + (dx > 1 || path[i].Y < path[i - 1].Y - 1 ? 20 : 0);
		}
		return (int)Math.Ceiling(ticks);
	}

	private void Board(bool grounded)
	{
		if (!TravelerTerrain.TrainStopped(train)
			|| train.StopTimer >= train.StopTimeMax - 90)
		{
			if (TravelerTerrain.OnTrain(Body, train))
			{
				// A departing train is already carrying this person; never jump back off its deck.
				AttachToTrain();
				return;
			}
			train = null;
			ChangeStage(TravelerStage.Planning);
			return;
		}
		MoveAlongRoute(grounded);
		if (Stage == TravelerStage.Boarding && TravelerTerrain.OnTrain(Body, train)
			&& TravelerRules.CanBoard(train.Active, true, train.Velocity, train.StopTimer, train.StopTimeMax, Body.Bottom, train.Position + boardingDoorOffset))
		{
			AttachToTrain();
		}
	}

	private void AttachToTrain(bool boarding = true)
	{
		Actor?.ReleaseSupport();
		Body.velocity = Vector2.Zero;
		aboardOffset = Body.position - train.Position;
		visitedStop = true;
		mayAlight = false;
		if (boarding)
		{
			TownTravelerSystem.RecordBoarding();
		}
		ChangeStage(TravelerStage.Riding);
	}

	private void Ride()
	{
		if (train?.Active != true)
		{
			Kill();
			return;
		}
		Body.position = train.Position + aboardOffset;
		Body.velocity = Vector2.Zero;
		view.Opacity = 1;
		visitedStop |= TravelerTerrain.TrainStopped(train);
		if (train.State == (int)YggdrasilTown_NormalMetro.BehaviorType.Move)
		{
			mayAlight = true;
			if (visitedStop)
			{
				movingTime++;
			}
		}
		else
		{
			movingTime = 0;
		}
		if (movingTime >= 180 && !TownTravelerSystem.Visible(Body))
		{
			Kill();
		}
	}

	private void ChangeStage(TravelerStage stage)
	{
		Stage = stage;
		stageTime = 0;
		dropping = false;
	}

	private void UpdateView(VFXPerson person)
	{
		// CustomTile trains update after NPCs. Follow their final position for drawing too.
		if (IsAttached && train?.Active == true)
		{
			Body.position = train.Position + aboardOffset;
		}
		float traveled = Math.Abs(Body.position.X - previousViewPosition.X);
		previousViewPosition = Body.position;
		animationDistance += traveled;
		person.Active = Active && TownTravelerSystem.Enabled;
		person.Position = Body.position + new Vector2(Body.width / 2f, Body is NPC npc ? npc.gfxOffY : 0);
		person.Direction = Direction;
		person.Velocity = Body.velocity;
		person.State = traveled > 0.1f && !IsAttached ? (int)VFXPerson.MoveState.Walk : (int)VFXPerson.MoveState.Stand;
		person.Frame = person.State == (int)VFXPerson.MoveState.Walk ? 6 + (int)(animationDistance / 4) % 14 : 0;
		if (Math.Abs(Body.velocity.Y) > 0.5f && !IsAttached)
		{
			person.State = (int)VFXPerson.MoveState.Jump;
			person.Frame = 5;
		}
		person.Visible = person.Active && person.Opacity > 0.01f;
	}
}
