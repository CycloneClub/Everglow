using Everglow.Commons.CustomTiles;
using Everglow.Yggdrasil.YggdrasilTown.CustomTiles;
using Terraria.Localization;

namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

internal enum CrowdStage
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
	Fleeing,
	Conversing,
}

/// <summary>A lightweight resident's intent, safe route and visual presentation.</summary>
internal sealed class CrowdAgent
{
	private readonly CrowdAppearance view;
	private readonly CrowdArrival arrival;
	private readonly int routeSeed;
	private readonly float speed = Main.rand.NextFloat(1.2f, 1.7f);
	private readonly int pauseInterval = Main.rand.Next(300, 600);
	private readonly int arrivalDelay = Main.rand.Next(20, 70);
	private readonly Queue<string> recentPlaces = [];
	private readonly CrowdAvoidance avoidance = new();
	private readonly CrowdRetirement retirement = new();
	private readonly CrowdProgress movementProgress = new();
	private Vector2 movementTarget;
	private bool movementPending;
	private bool advancedWaypoint;
	private bool waitingForBudget;
	private YggdrasilTown_NormalMetro progressTrain;

	private CrowdPlace destination;
	private int stageTime;
	private int age;
	private int completedActivities;
	private int activityDuration;
	private int planDelay;
	private bool departureRequested;

	private List<Point> route;
	private int waypoint = 1;
	private int yieldTime;
	private int pauseTime;
	private float stepSpeed = 1;
	private float stepOffset;
	private bool jumping;
	private Vector2 jumpTarget;
	private bool dropping;
	private float dropSourceY;
	private bool recoveringPlatform;

	private YggdrasilTown_NormalMetro train;
	private Vector2 aboardOffset;
	private Vector2 boardingDoorOffset;
	private int movingTime;
	private bool visitedStop;
	private bool mayAlight;

	private int escapeDirection;
	private int escapeAttempts;
	private CrowdStage interruptedStage;
	private int interruptedStageTime;
	private long nextConversation;
	private Vector2 previousViewPosition;
	private float animationDistance;

	public CrowdAgent(CrowdBody actor, List<Point> path, CrowdPlace destination,
		CrowdArrival arrival, CrowdPlace entrance, int seed, YggdrasilTown_NormalMetro arrivingTrain = null, Vector2? aboardFeet = null)
	{
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
		Body.Bottom = CrowdTerrain.Feet(path[0]);
		if (arrival == CrowdArrival.Existing)
		{
			Stage = CrowdStage.Walking;
		}
		if (aboardFeet.HasValue)
		{
			Body.Bottom = aboardFeet.Value;
			aboardOffset = Body.position - train.Position;
			Stage = CrowdStage.Riding;
			mayAlight = true;
			visitedStop = CrowdTerrain.TrainStopped(train);
		}
		previousViewPosition = Body.position;
		view = CrowdAppearance.Create(Body.position);
		view.Opacity = aboardFeet.HasValue || arrival is CrowdArrival.Street or CrowdArrival.Existing ? 1 : 0;
		view.UpdateController = UpdateView;
		view.DrawInForeground = () => InteriorTrain == null;
		UpdateView(view);
		Ins.VFXManager.Add(view);
	}

	public Entity Body => Actor.Body;

	public CrowdBody Actor { get; }

	public CrowdIntent Intent { get; internal set; }

	internal bool IsSitting => !IsEvacuating && Stage == CrowdStage.Activity && CurrentActivity == CrowdActivity.Sit;

	internal bool IsEvacuating => avoidance.Alarmed || Stage == CrowdStage.Fleeing;

	internal CrowdActivity? CurrentActivity => Stage == CrowdStage.Conversing ? CrowdActivity.Socialize : destination?.Activity;

	internal bool CanConverse => Active && !IsEvacuating && InteriorTrain == null && !jumping && !dropping
		&& TownCrowdSystem.Clock >= nextConversation && CrowdTerrain.Grounded(Body)
		&& (Stage is CrowdStage.Planning or CrowdStage.Walking
			|| (Stage == CrowdStage.Activity && destination?.Activity is CrowdActivity.Scenery or CrowdActivity.Browse));

	internal void PauseForConversation()
	{
		interruptedStage = Stage;
		interruptedStageTime = stageTime;
		Stage = CrowdStage.Conversing;
		stageTime = 0;
		Body.velocity.X = 0;
	}

	internal void FaceConversation(Entity partner)
	{
		if (Math.Abs(partner.Center.X - Body.Center.X) > 4)
		{
			Direction = Math.Sign(partner.Center.X - Body.Center.X);
		}
	}

	internal void ResumeAfterConversation(bool completed)
	{
		nextConversation = TownCrowdSystem.Clock + CrowdConversation.CooldownTicks;
		if (Stage != CrowdStage.Conversing)
		{
			return;
		}
		Stage = interruptedStage;
		stageTime = interruptedStageTime;
		if (completed)
		{
			completedActivities++;
		}
	}

	public string DisplayName => Language.GetTextValue(CrowdNames.KeyFor(routeSeed));

	internal bool CanShowNameAt(Vector2 worldMouse) => !IsEvacuating && Active && view.Active && view.Visible
		&& view.EffectiveOpacity > 0.25f && (InteriorTrain == null || InteriorTrain.ShellAlpha <= 0.1f)
		&& new Rectangle((int)view.Position.X - 16, (int)view.Position.Y - 4, 32, 56).Contains(worldMouse.ToPoint());

	public bool Active { get; private set; } = true;

	public CrowdStage Stage { get; private set; }

	public int Direction { get; private set; } = 1;

	public bool IsAttached => Stage == CrowdStage.Riding;

	// Keep the same owner during boarding/alighting, but only once physically inside.
	internal YggdrasilTown_NormalMetro InteriorTrain => Active && train?.Active == true
		&& (IsAttached || WalkingTrain != null)
		&& Body.Center.X >= train.Position.X && Body.Center.X < train.Position.X + train.Size.X
		&& Body.Center.Y >= train.Position.Y - 146 && Body.Center.Y < train.Position.Y ? train : null;

	internal void DrawInTrain(SpriteBatch spriteBatch)
	{
		if (view.Active && view.Visible)
		{
			view.DrawAppearance(spriteBatch);
		}
	}

	internal bool CanAlight(YggdrasilTown_NormalMetro candidate) => Active && IsAttached && mayAlight && ReferenceEquals(train, candidate);

	internal bool Aboard(YggdrasilTown_NormalMetro candidate) => Active && IsAttached && ReferenceEquals(train, candidate);

	internal float WalkSpeed => speed;

	internal void BeginAlighting(List<Point> path, CrowdPlace landing)
	{
		Actor.ReleaseSupport();
		destination = landing;
		SetRoute(path);
		mayAlight = false;
		Body.velocity = Vector2.Zero;
		ChangeStage(CrowdStage.Alighting);
	}

	public int RouteSeed => routeSeed;

	public string DestinationKey => Stage is CrowdStage.Planning or CrowdStage.Riding or CrowdStage.Fleeing ? null : destination?.Key;

	internal bool ReservesStandingPoint(Point cell) => Active && DestinationKey != null
		&& Math.Abs(destination.Cell.X - cell.X) < 2 && Math.Abs(destination.Cell.Y - cell.Y) < 3;

	public bool RecentlyVisited(string key) => recentPlaces.Contains(key);

	public float Preference(CrowdActivity activity) => 0.5f + (uint)(routeSeed ^ ((int)activity * 7919)) % 7 * 0.3f;

	private YggdrasilTown_NormalMetro WalkingTrain => Stage is CrowdStage.Boarding or CrowdStage.Alighting ? train : null;

	public bool FallThroughPlatforms => dropping && (recoveringPlatform
		? Body.Bottom.Y + Math.Max(Body.velocity.Y, 0) < dropSourceY
		: CrowdRules.FallThroughSourcePlatform(Body.Bottom.Y, dropSourceY));

	public void Kill()
	{
		if (!Active)
		{
			return;
		}
		CrowdConversation.End(this);
		Active = false;
		Actor.ReleaseSupport();
		train = null;
		destination = null;
		view.Active = false;
		view.Visible = false;
		view.UpdateController = null;
		view.DrawInForeground = null;
	}

	public void Update()
	{
		if (!Active)
		{
			return;
		}
		if (!TownCrowdSystem.Enabled)
		{
			Kill();
			return;
		}
		if (Main.gamePaused)
		{
			return;
		}
		advancedWaypoint = false;
		UpdateBehavior();
		if (Active)
		{
			UpdateRetirement();
		}
	}

	private bool WantsToLeave => departureRequested || Intent != CrowdIntent.VisitTown;

	private bool FollowingExit => Stage is CrowdStage.Walking or CrowdStage.Boarding or CrowdStage.Leaving or CrowdStage.Waiting
		&& destination != null && CrowdActivityRules.IsExit(destination.Activity);

	private void UpdateRetirement()
	{
		bool resting = Stage is CrowdStage.Activity or CrowdStage.Conversing or CrowdStage.Waiting or CrowdStage.Riding or CrowdStage.Arriving;
		bool suspended = resting || waitingForBudget || (Stage == CrowdStage.Walking && pauseTime > 0);
		bool moving = Stage is CrowdStage.Walking or CrowdStage.Boarding or CrowdStage.Alighting or CrowdStage.Fleeing
			|| (Stage == CrowdStage.Planning && movementPending) || recoveringPlatform;
		var reference = WalkingTrain;
		if (!ReferenceEquals(reference, progressTrain))
		{
			// Train motion is not walking progress; changing coordinate spaces is not progress either.
			movementProgress.Reset();
			progressTrain = reference;
		}
		var offset = reference?.Position ?? Vector2.Zero;
		bool progressed = moving && !suspended && movementPending
			&& movementProgress.Observe(Body.Bottom - offset, movementTarget - offset, advancedWaypoint);
		retirement.ObserveThreat(avoidance.Alarmed);
		retirement.ObserveMovement(moving, suspended, progressed);
		retirement.ObserveDeparture(WantsToLeave, suspended, progressed && FollowingExit);
		retirement.Update();
		view.RetirementOpacity = retirement.Opacity;
		if (retirement.Complete)
		{
			Kill();
		}
	}

	private void ResolveMovement()
	{
		retirement.ResolveMovement();
		movementPending = false;
		waitingForBudget = false;
		movementProgress.Reset();
	}

	private void RecoverToward(Vector2 target)
	{
		movementPending = true;
		movementTarget = target;
		waitingForBudget = false;
		if (age % CrowdSettings.RecoveryAttemptTicks == 0)
		{
			retirement.RecordMovementRecovery();
		}
	}

	private void UpdateBehavior()
	{
		age++;
		stageTime++;
		if (IsAttached)
		{
			Ride();
			return;
		}
		if (!UpdateSafety(out bool grounded))
		{
			return;
		}
		Body.velocity.X = 0;
		if (grounded && CanConverse && (TownCrowdSystem.Clock + (uint)routeSeed) % CrowdConversation.OpportunityInterval == 0)
		{
			TryConversation();
		}
		if (UpdatePlatformRecovery(grounded))
		{
			return;
		}
		UpdateStage(grounded);
		if (Active && !IsAttached)
		{
			MoveBody(grounded);
		}
	}

	private bool UpdateSafety(out bool grounded)
	{
		grounded = false;
		bool threatened = CrowdThreats.Dangerous(Body.Bottom);
		if (threatened && WalkingTrain != null && CrowdTerrain.OnTrain(Body, train))
		{
			// Remain aboard rather than jump from a moving deck or alight into a fight.
			AttachToTrain();
			return false;
		}
		if (avoidance.Update(threatened, CrowdThreats.Clearance(Body.Bottom) > 320, TownCrowdSystem.Visible(Body),
			CrowdSettings.OffscreenTicks))
		{
			Kill();
			return false;
		}
		// Let a planned return finish outside the camera too. Reclaim only stranded
		// offscreen visitors after a long unsuccessful planning period.
		if (!avoidance.PendingRetirement && ((Stage == CrowdStage.Planning && stageTime > 1800
			&& CrowdActivityRules.CanRetire(age, TownCrowdSystem.Visible(Body)))
			|| CrowdActivityRules.CanRetireAfterFailedDeparture(age, TownCrowdSystem.Visible(Body), retirement.DepartureFailures)))
		{
			Kill();
			return false;
		}
		grounded = CrowdTerrain.Grounded(Body);
		if (avoidance.Alarmed)
		{
			CrowdConversation.End(this);
			if (Stage != CrowdStage.Fleeing && grounded)
			{
				escapeDirection = CrowdThreats.AwayDirection(Body.Bottom, Direction == 0 ? 1 : Direction);
				escapeAttempts = 0;
				train = null;
				view.Opacity = 1;
				SetRoute([]);
				planDelay = 0;
				ChangeStage(CrowdStage.Fleeing);
			}
		}
		else if (Stage == CrowdStage.Fleeing && grounded)
		{
			SetRoute([]);
			planDelay = 0;
			ChangeStage(CrowdStage.Planning);
		}
		return true;
	}

	private bool UpdatePlatformRecovery(bool grounded)
	{
		if (Stage == CrowdStage.Planning && !recoveringPlatform && CrowdTerrain.TryPlatformRecovery(Body, out float landingY))
		{
			recoveringPlatform = true;
			dropping = true;
			dropSourceY = landingY;
		}
		if (recoveringPlatform)
		{
			RecoverToward(new Vector2(Body.Bottom.X, dropSourceY));
			// A changed floor or an intervening moving surface must not trap the recovery state.
			if (grounded || Body.Bottom.Y > dropSourceY + 2)
			{
				recoveringPlatform = false;
				dropping = false;
				planDelay = 0;
			}
			else
			{
				Body.velocity.Y = Math.Min(Body.velocity.Y + CrowdNavigation.Gravity, 8);
				Actor.Move(FallThroughPlatforms);
				return true;
			}
		}
		return false;
	}

	private void UpdateStage(bool grounded)
	{
		if (grounded && stageTime % 60 == 0 && Stage is CrowdStage.Walking or CrowdStage.Activity or CrowdStage.Waiting
			&& !TownCrowdSystem.ValidPlace(destination))
		{
			ChangeStage(CrowdStage.Planning);
		}
		switch (Stage)
		{
			case CrowdStage.Conversing:
				Converse(grounded);
				break;
			case CrowdStage.Fleeing:
				Flee(grounded);
				break;
			case CrowdStage.Arriving:
				Arrive();
				break;
			case CrowdStage.Planning:
				Plan(grounded);
				break;
			case CrowdStage.Walking:
				if (MoveAlongRoute(grounded))
				{
					ReachDestination();
				}
				break;
			case CrowdStage.Activity:
				Stay(grounded);
				break;
			case CrowdStage.Waiting:
				WaitForTrain();
				break;
			case CrowdStage.Boarding:
				Board(grounded);
				break;
			case CrowdStage.Alighting:
				Alight(grounded);
				break;
			case CrowdStage.Leaving:
				Leave(grounded);
				break;
		}
	}

	private void Arrive()
	{
		view.Opacity = arrival == CrowdArrival.Street ? 1 : MathHelper.Clamp(stageTime / 30f, 0, 1);
		if (arrival == CrowdArrival.Pylon && stageTime <= 35)
		{
			Dust.NewDust(Body.position, Body.width, Body.height, DustID.MagicMirror, 0, -1.5f, 100);
			Lighting.AddLight(Body.Center, new Vector3(0.35f, 0.65f, 0.9f) * (1 - stageTime / 36f));
		}
		if (stageTime >= (arrival == CrowdArrival.Pylon ? 90 : 30) + arrivalDelay)
		{
			ChangeStage(CrowdStage.Walking);
		}
	}

	private void Plan(bool grounded)
	{
		if (grounded && CrowdTerrain.TryStandingRecovery(Body, out float standingX))
		{
			RecoverToward(new Vector2(standingX, Body.Bottom.Y));
			Direction = Math.Sign(standingX - Body.Bottom.X);
			Body.velocity.X = MathHelper.Clamp(standingX - Body.Bottom.X, -speed, speed);
		}
		else if (grounded && --planDelay <= 0)
		{
			ChooseNextActivity();
		}
	}

	private void Stay(bool grounded)
	{
		if (stageTime >= activityDuration)
		{
			completedActivities++;
			Remember(destination.Key);
			ChangeStage(CrowdStage.Planning);
			return;
		}
		Direction = destination.Focus.X >= Body.Center.X ? 1 : -1;
		if (destination.Activity == CrowdActivity.Sit)
		{
			if (!grounded || !CrowdSeats.Available(destination.Cell) || CrowdThreats.EnemyNear(Body.Center))
			{
				Remember(destination.Key);
				ChangeStage(CrowdStage.Planning);
				return;
			}
			Direction = CrowdSeats.Direction(Body, destination.Cell);
		}
	}

	private void Alight(bool grounded)
	{
		if (!CrowdTerrain.TrainStopped(train) || train.StopTimer >= train.StopTimeMax - 30)
		{
			RecoverFromTrain();
		}
		else if (MoveAlongRoute(grounded))
		{
			train = null;
			ChangeStage(CrowdStage.Planning);
		}
	}

	private void Converse(bool grounded)
	{
		if (!grounded)
		{
			CrowdConversation.End(this);
		}
		else if (!CrowdConversation.ContinueNearby(this) && Stage == CrowdStage.Conversing)
		{
			ResumeAfterConversation(false);
		}
	}

	private void MoveBody(bool grounded)
	{
		Body.velocity.Y = Math.Min(Body.velocity.Y + CrowdNavigation.Gravity, 8);
		if (grounded && !dropping)
		{
			Collision.StepUp(ref Body.position, ref Body.velocity, Body.width, Body.height, ref stepSpeed, ref stepOffset);
			CrowdTerrain.StepOntoSurface(Body, WalkingTrain);
		}
		stepOffset *= 0.8f;
		Actor.Move(FallThroughPlatforms);
	}

	private bool TryConversation() => Main.rand.Next(100) < CrowdSettings.ChatChancePercent
		&& CrowdConversation.TryNearby(this);

	private void Flee(bool grounded)
	{
		// A checked route may deliberately walk off support onto a lower safe floor.
		// Recentring during that step pulls the body back onto the ledge every other tick.
		if ((route.Count <= 1 || waypoint >= route.Count) && grounded && CrowdTerrain.TryStandingRecovery(Body, out float standingX)
			&& CrowdThreats.EscapeAllowed(Body.Bottom, new Vector2(standingX, Body.Bottom.Y)))
		{
			RecoverToward(new Vector2(standingX, Body.Bottom.Y));
			Direction = Math.Sign(standingX - Body.Bottom.X);
			Body.velocity.X = MathHelper.Clamp(standingX - Body.Bottom.X, -speed, speed);
			return;
		}
		if (!grounded && jumping)
		{
			WalkToward(jumpTarget, false);
			return;
		}
		if (route.Count > 1 && waypoint < route.Count)
		{
			if (grounded && !CrowdThreats.EscapeAllowed(Body.Bottom, CrowdTerrain.Feet(route[waypoint])))
			{
				SetRoute([]);
				planDelay = 0;
			}
			else if (MoveAlongRoute(grounded))
			{
				SetRoute([]);
				planDelay = 30;
			}
			return;
		}
		if (grounded && --planDelay <= 0)
		{
			int budgetBefore = TownCrowdSystem.PlanningBudget;
			bool planned = TownCrowdSystem.TryEscape(CrowdTerrain.Cell(Body.Bottom), escapeDirection, routeSeed, out var path, escapeAttempts);
			waitingForBudget = path == null && TownCrowdSystem.PlanningBudget == budgetBefore;
			if (!waitingForBudget)
			{
				escapeAttempts++;
				retirement.RecordMovementRecovery();
			}
			planDelay = path == null ? 5 : 60;
			if (planned)
			{
				SetRoute(path);
				escapeDirection = Math.Sign(path[^1].X - path[0].X);
			}
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
		if (TryConversation())
		{
			return;
		}
		planDelay = Main.rand.Next(90, 180);
		departureRequested = CrowdActivityRules.ShouldDepart(departureRequested, age, completedActivities, routeSeed);
		if (departureRequested && Intent == CrowdIntent.EnterGuild)
		{
			Intent = CrowdIntent.ReturnHome;
		}
		bool departing = departureRequested || Intent == CrowdIntent.ReturnHome;
		var start = CrowdTerrain.Cell(Body.Bottom);
		CrowdPlace next;
		List<Point> path;
		int budgetBefore = TownCrowdSystem.PlanningBudget;
		bool planned = Intent == CrowdIntent.VisitTown
			? TownCrowdSystem.TryPlan(start, this, departing, out next, out path)
			: TownCrowdSystem.PlanIntent(start, this, Intent, out next, out path);
		if (!planned && path == null)
		{
			// An empty budget is not failure. Routes actually tried before exhaustion still count.
			planDelay = Main.rand.Next(1, 5);
			waitingForBudget = TownCrowdSystem.PlanningBudget == budgetBefore;
			if (!waitingForBudget)
			{
				if (movementPending) { retirement.RecordMovementRecovery(); }
				if (WantsToLeave) { retirement.RecordDepartureFailure(); }
			}
			return;
		}
		waitingForBudget = false;
		if (movementPending)
		{
			retirement.RecordMovementRecovery();
		}
		if (WantsToLeave && !planned)
		{
			// Finding a route alone cannot erase failures: the exit must actually make progress.
			retirement.RecordDepartureFailure();
		}
		if (!planned && (departing || Intent != CrowdIntent.VisitTown))
		{
			// An unavailable exit is not a reason to freeze in view. Continue visiting and retry later.
			int fallbackBudget = TownCrowdSystem.PlanningBudget;
			planned = TownCrowdSystem.TryPlan(CrowdTerrain.Cell(Body.Bottom), this, false, out next, out path);
			if (!planned && path == null)
			{
				waitingForBudget = TownCrowdSystem.PlanningBudget == fallbackBudget;
				planDelay = Main.rand.Next(1, 5);
				return;
			}
		}
		if (planned)
		{
			destination = next;
			SetRoute(path);
			ChangeStage(CrowdStage.Walking);
		}
		else if (stageTime > 1800)
		{
			// A small disconnected plaza may have only two activities. Retry after a long pause.
			recentPlaces.Clear();
		}
	}

	private void Leave(bool grounded)
	{
		if (!TownCrowdSystem.ValidPlace(destination)
			|| !CrowdRules.ReachedWaypoint(Body.Bottom, CrowdTerrain.Feet(destination.Cell), grounded))
		{
			if (WantsToLeave)
			{
				retirement.RecordDepartureFailure();
			}
			view.Opacity = 1;
			ChangeStage(CrowdStage.Planning);
			return;
		}
		bool pylon = destination.Activity == CrowdActivity.LeavePylon;
		if (destination.Activity == CrowdActivity.EnterGuild)
		{
			if (TownCrowdSystem.TryEnterGuild(destination.Key))
			{
				Kill();
			}
			return;
		}
		Direction = destination.Focus.X >= Body.Center.X ? 1 : -1;
		view.Opacity = CrowdActivityRules.DepartureOpacity(destination.Activity, stageTime);
		if (pylon && !Main.dedServ)
		{
			float charge = MathHelper.Clamp(stageTime / 60f, 0, 1) * view.Opacity;
			Lighting.AddLight(Body.Center, new Vector3(0.35f, 0.65f, 0.9f) * charge);
			if (stageTime % 3 == 0)
			{
				Dust.NewDust(Body.position, Body.width, Body.height, DustID.MagicMirror, 0, -1.5f, 100);
			}
			if (stageTime == 60)
			{
				for (int i = 0; i < 12; i++)
				{
					Dust.NewDust(Body.position, Body.width, Body.height, DustID.MagicMirror, 0, -2, 100);
				}
			}
		}
		if (stageTime >= (pylon ? 90 : 60))
		{
			Kill();
		}
	}

	private void ReachDestination()
	{
		if (!TownCrowdSystem.ValidPlace(destination)
			|| !CrowdRules.ReachedWaypoint(Body.Bottom, CrowdTerrain.Feet(destination.Cell), CrowdTerrain.Grounded(Body)))
		{
			if (WantsToLeave && FollowingExit)
			{
				retirement.RecordDepartureFailure();
			}
			ChangeStage(CrowdStage.Planning);
			return;
		}
		ResolveMovement();
		if (FollowingExit)
		{
			retirement.ResolveDeparture();
		}
		if (destination.Activity == CrowdActivity.BoardTrain)
		{
			ChangeStage(CrowdStage.Waiting);
		}
		else if (destination.Activity == CrowdActivity.LeaveStreet)
		{
			if (!TownCrowdSystem.Visible(Body))
			{
				Kill();
			}
			else
			{
				ChangeStage(CrowdStage.Planning);
			}
		}
		else if (CrowdActivityRules.IsExit(destination.Activity))
		{
			ChangeStage(CrowdStage.Leaving);
		}
		else
		{
			activityDuration = destination.Activity switch
			{
				CrowdActivity.Sit => Main.rand.Next(12 * 60, 24 * 60 + 1),
				_ => Main.rand.Next(CrowdSettings.ActivityMinTicks, CrowdSettings.ActivityMaxTicks + 1),
			};
			ChangeStage(CrowdStage.Activity);
		}
	}

	private void RecoverFromTrain()
	{
		if (CrowdTerrain.OnTrain(Body, train))
		{
			AttachToTrain();
		}
		else
		{
			train = null;
			ChangeStage(CrowdStage.Planning);
		}
	}

	private bool MoveAlongRoute(bool grounded)
	{
		movementPending = true;
		if (route.Count > 0)
		{
			movementTarget = CrowdTerrain.Feet(route[Math.Min(waypoint, route.Count - 1)]);
		}
		var surfaceTrain = WalkingTrain;
		bool Clear(Point cell) => CrowdTerrain.IsClear(cell)
			&& (Stage != CrowdStage.Fleeing || CrowdThreats.EscapeAllowed(Body.Bottom, CrowdTerrain.Feet(cell)));
		bool Stand(Point cell) => CrowdTerrain.CanStand(cell, surfaceTrain);
		bool Drop(Point cell) => CrowdTerrain.CanDropThrough(cell, surfaceTrain);
		while (waypoint < route.Count && CrowdRules.ReachedWaypoint(Body.Bottom, CrowdTerrain.Feet(route[waypoint]), grounded))
		{
			waypoint++;
			advancedWaypoint = true;
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
		if (Stage == CrowdStage.Walking && grounded && !dropping)
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
		bool drop = CrowdNavigation.CanDropBetween(route[waypoint - 1], route[waypoint], Clear, Stand, Drop);
		if (drop && grounded)
		{
			dropping = true;
			dropSourceY = CrowdTerrain.Feet(route[waypoint - 1]).Y;
		}
		bool needsJump = !drop && !CrowdNavigation.CanWalkBetween(route[waypoint - 1], route[waypoint], Clear, Stand, Drop);
		if (grounded && needsJump && !CrowdNavigation.CanJumpBetween(route[waypoint - 1], route[waypoint], Clear, Stand))
		{
			Replan(surfaceTrain);
			return false;
		}
		WalkToward(CrowdTerrain.Feet(route[waypoint]), grounded, needsJump);
		if (age % CrowdSettings.RecoveryAttemptTicks == 0 && retirement.MovementTicks >= CrowdSettings.RecoveryAttemptTicks)
		{
			Replan(surfaceTrain);
		}
		return false;
	}

	private void Replan(YggdrasilTown_NormalMetro surfaceTrain)
	{
		Body.velocity.X = 0;
		if (Stage == CrowdStage.Fleeing)
		{
			SetRoute([]);
			planDelay = 0;
			return;
		}
		var path = TownCrowdSystem.FindRoute(CrowdTerrain.Cell(Body.Bottom), route[^1], routeSeed, surfaceTrain);
		if (path == null)
		{
			waitingForBudget = true;
			return;
		}
		waitingForBudget = false;
		retirement.RecordMovementRecovery();
		if (path.Count == 0)
		{
			if (WantsToLeave && FollowingExit)
			{
				retirement.RecordDepartureFailure();
			}
			RecoverFromTrain();
			return;
		}
		SetRoute(path);
	}

	private void SetRoute(List<Point> path)
	{
		route = path;
		waypoint = 1;
		dropping = false;
		jumping = false;
		waitingForBudget = false;
	}

	private void WalkToward(Vector2 target, bool grounded, bool needsJump = false)
	{
		waitingForBudget = false;
		movementPending = true;
		movementTarget = target;
		if (jumping && !grounded)
		{
			// Keep the same ballistic motion that navigation checked; do not yield in midair.
			Body.velocity.X = MathHelper.Clamp(jumpTarget.X - Body.Bottom.X, -CrowdNavigation.JumpSpeed, CrowdNavigation.JumpSpeed);
			return;
		}
		jumping = false;
		float distance = target.X - Body.Bottom.X;
		if (Math.Abs(distance) > 2)
		{
			Direction = Math.Sign(distance);
		}
		bool occupied = TownCrowdSystem.ObstacleAhead(this, Direction);
		yieldTime = occupied ? yieldTime + 1 : 0;
		if (occupied && yieldTime < 60)
		{
			return;
		}
		float stride = Stage == CrowdStage.Fleeing ? speed * 1.25f : speed;
		Body.velocity.X = MathHelper.Clamp(distance, -stride, stride) * (occupied ? 0.65f : 1f);
		if (grounded && needsJump)
		{
			jumping = true;
			jumpTarget = target;
			Body.velocity.X = MathHelper.Clamp(distance, -CrowdNavigation.JumpSpeed, CrowdNavigation.JumpSpeed);
			Body.velocity.Y = CrowdNavigation.JumpVelocity;
		}
	}

	private void WaitForTrain()
	{
		if (stageTime > 3600)
		{
			Remember(destination.Key);
			ChangeStage(CrowdStage.Planning);
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
			if (!CrowdTerrain.TrainStopped(candidate) || Math.Abs(candidate.StationX - destination.Focus.X) >= 24
				|| Math.Abs(candidate.Box.Top - (destination.Focus.Y - 16)) > 4 || candidate.StopTimer < 30)
			{
				continue;
			}
			Vector2 door = new(candidate.Position.X + candidate.Size.X * (Body.Bottom.X < candidate.Box.Center.X ? 0.25f : 0.75f), candidate.Box.Top);
			var path = TownCrowdSystem.FindRoute(CrowdTerrain.Cell(Body.Bottom), CrowdTerrain.Cell(door), routeSeed, candidate);
			if (path == null || path.Count == 0 || !CrowdActivityRules.CanFinishTrainWalk(true, candidate.StopTimer,
				candidate.StopTimeMax, EstimateTravelTicks(path, speed)))
			{
				continue;
			}
			train = candidate;
			boardingDoorOffset = door - candidate.Position;
			SetRoute(path);
			ChangeStage(CrowdStage.Boarding);
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
			float fallTicks = MathF.Sqrt(2 * dy * 16 / CrowdNavigation.Gravity);
			ticks += Math.Max(dx * 16 / speed, fallTicks) + (dx > 1 || path[i].Y < path[i - 1].Y - 1 ? 20 : 0);
		}
		return (int)Math.Ceiling(ticks);
	}

	private void Board(bool grounded)
	{
		if (!CrowdTerrain.TrainStopped(train)
			|| train.StopTimer >= train.StopTimeMax - 90)
		{
			if (CrowdTerrain.OnTrain(Body, train))
			{
				// A departing train is already carrying this person; never jump back off its deck.
				AttachToTrain();
				return;
			}
			train = null;
			ChangeStage(CrowdStage.Planning);
			return;
		}
		MoveAlongRoute(grounded);
		if (Stage == CrowdStage.Boarding && CrowdTerrain.OnTrain(Body, train)
			&& CrowdRules.CanBoard(train.Active, true, train.Velocity, train.StopTimer, train.StopTimeMax, Body.Bottom, train.Position + boardingDoorOffset))
		{
			AttachToTrain();
		}
	}

	private void AttachToTrain()
	{
		ResolveMovement();
		retirement.ResolveDeparture();
		Actor.ReleaseSupport();
		Body.velocity = Vector2.Zero;
		aboardOffset = Body.position - train.Position;
		visitedStop = true;
		mayAlight = false;
		ChangeStage(CrowdStage.Riding);
	}

	private void Ride()
	{
		if (train?.Active != true)
		{
			Kill();
			return;
		}
		Body.position = train.Position + aboardOffset;
		// A passenger who boarded while fleeing still uses the existing sustained-safe recovery.
		if (avoidance.Alarmed && avoidance.Update(CrowdThreats.Dangerous(Body.Bottom), CrowdThreats.Clearance(Body.Bottom) > 320,
			TownCrowdSystem.Visible(Body), CrowdSettings.OffscreenTicks))
		{
			Kill();
			return;
		}
		Body.velocity = Vector2.Zero;
		view.Opacity = 1;
		visitedStop |= CrowdTerrain.TrainStopped(train);
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
		if (movingTime >= 180 && !TownCrowdSystem.Visible(Body))
		{
			Kill();
		}
	}

	private void ChangeStage(CrowdStage stage)
	{
		Stage = stage;
		stageTime = 0;
		dropping = false;
	}

	private void UpdateView(CrowdAppearance person)
	{
		// CustomTile trains update after NPCs. Follow their final position for drawing too.
		if (IsAttached && train?.Active == true)
		{
			Body.position = train.Position + aboardOffset;
		}
		float traveled = Math.Abs(Body.position.X - previousViewPosition.X);
		previousViewPosition = Body.position;
		animationDistance += traveled;
		person.Active = Active && TownCrowdSystem.Enabled;
		person.Position = Body.position + new Vector2(Body.width / 2f, 0);
		person.Direction = Direction;
		Body.direction = Direction;
		person.Sitting = IsSitting;
		if (person.Sitting)
		{
			person.Position += CrowdSeats.VisualOffset(Body, destination.Cell, out person.SeatYOffset);
		}
		person.State = traveled > 0.1f && !IsAttached ? (int)CrowdAppearance.CrowdMoveState.Walk : (int)CrowdAppearance.CrowdMoveState.Stand;
		person.Frame = person.State == (int)CrowdAppearance.CrowdMoveState.Walk ? 6 + (int)(animationDistance / 4) % 14 : 0;
		if (Math.Abs(Body.velocity.Y) > 0.5f && !IsAttached)
		{
			person.State = (int)CrowdAppearance.CrowdMoveState.Jump;
			person.Frame = 5;
		}
		person.Visible = person.Active && person.EffectiveOpacity > 0.01f;
	}
}
