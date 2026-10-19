using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Commons.CustomTiles;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.CustomTiles;
using Everglow.Yggdrasil.Mechanics.TownCrowd;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.OriginPylon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class CrowdTownPlanningTests
{
	private sealed class CrowdChanceRandom(int roll) : UnifiedRandom(42)
	{
		public override int Next(int maxValue) => maxValue == 100 ? roll : base.Next(maxValue);
	}

	private readonly List<Action> restore = [];

	private static CrowdRetirement RetirementOf(object traveler) => (CrowdRetirement)traveler.GetType()
		.GetField("retirement", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(traveler)!;

	[TestMethod]
	public void SightseeingBudgetWaitPausesRetirementButKeepsActualExitFailures()
	{
		EnableJourneys();
		ResetThreats();
		var traveler = CreateWalkingActor(new Vector2(1608, 1024));
		var type = traveler.GetType();
		type.GetProperty("Intent")!.SetValue(traveler, CrowdIntent.ReturnHome);
		type.GetProperty("Stage")!.SetValue(traveler, Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Planning"));
		typeof(TownCrowdSystem).GetField("planningBudget", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, 0);
		for (int attempt = 0; attempt < 3; attempt++)
		{
			type.GetMethod("ChooseNextActivity", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(traveler, null);
		}
		Assert.AreEqual(3, RetirementOf(traveler).DepartureFailures, "This plaza has no return pylon; each rejected return is a real failure.");
		Assert.AreEqual(true, type.GetField("waitingForBudget", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(traveler));
		Assert.IsTrue((int)type.GetField("planDelay", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(traveler)! < 5);
		for (int tick = 0; tick < 3600; tick++)
		{
			type.GetMethod("UpdateRetirement", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(traveler, null);
		}
		Assert.AreEqual(CrowdRetirementReason.None, RetirementOf(traveler).Reasons);
	}

	[TestMethod]
	public void EscapeProgressDoesNotEraseAnInterruptedExitFailure()
	{
		var traveler = CreateWalkingActor(new Vector2(1608, 1024));
		var type = traveler.GetType();
		var body = (Entity)type.GetProperty("Body")!.GetValue(traveler)!;
		type.GetProperty("Intent")!.SetValue(traveler, CrowdIntent.ReturnHome);
		type.GetProperty("Stage")!.SetValue(traveler, Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Fleeing"));
		type.GetField("destination", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(traveler,
			Activator.CreateInstance(PlaceType, CrowdActivity.LeavePylon, new Point(110, 63), "exit", new Vector2(1768, 1024), null, 0)!);
		type.GetField("movementPending", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(traveler, true);
		type.GetField("movementTarget", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(traveler, new Vector2(1200, 1024));
		var retirement = RetirementOf(traveler);
		for (int i = 0; i < 3; i++) { retirement.RecordDepartureFailure(); }
		for (int tick = 0; tick < 1800; tick++) { retirement.ObserveDeparture(true, false, false); }
		var observe = type.GetMethod("UpdateRetirement", BindingFlags.Instance | BindingFlags.NonPublic)!;
		observe.Invoke(traveler, null);
		body.position.X -= 20;
		observe.Invoke(traveler, null);
		Assert.AreEqual(0, retirement.MovementTicks, "Escape movement is legitimate physical progress.");
		Assert.AreEqual(3, retirement.DepartureFailures, "Escaping along a different route is not progress on the interrupted exit.");
		Assert.IsTrue(retirement.Reasons.HasFlag(CrowdRetirementReason.Departure));
	}

	[TestMethod]
	[DataRow("Activity", false)]
	[DataRow("Conversing", false)]
	[DataRow("Waiting", false)]
	[DataRow("Riding", false)]
	[DataRow("Planning", true)]
	public void NormalRestsAndBudgetWaitsNeverAccumulateRetirementTime(string stage, bool budgetWait)
	{
		var traveler = CreateWalkingActor(new Vector2(1608, 1024));
		var type = traveler.GetType();
		type.GetProperty("Stage")!.SetValue(traveler, Enum.Parse(type.GetProperty("Stage")!.PropertyType, stage));
		type.GetProperty("Intent")!.SetValue(traveler, CrowdIntent.ReturnHome);
		type.GetField("waitingForBudget", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(traveler, budgetWait);
		type.GetField("movementPending", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(traveler, true);
		var retirement = RetirementOf(traveler);
		for (int i = 0; i < 3; i++)
		{
			retirement.RecordMovementRecovery();
			retirement.RecordDepartureFailure();
		}
		for (int tick = 0; tick < 3600; tick++)
		{
			type.GetMethod("UpdateRetirement", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(traveler, null);
		}
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
		Assert.AreEqual(1f, retirement.Opacity);
	}

	[TestMethod]
	public void PhysicalProgressRestoresOpacityWithoutChangingRouteStageOrVelocity()
	{
		var traveler = CreateWalkingActor(new Vector2(1608, 1024));
		var type = traveler.GetType();
		var body = (Entity)type.GetProperty("Body")!.GetValue(traveler)!;
		body.velocity = new Vector2(1.5f, 0);
		type.GetField("movementPending", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(traveler, true);
		type.GetField("movementTarget", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(traveler, new Vector2(1800, 1024));
		var retirement = RetirementOf(traveler);
		for (int i = 0; i < 3; i++) { retirement.RecordMovementRecovery(); }
		for (int tick = 0; tick < 900; tick++) { retirement.ObserveMovement(true, false, false); }
		for (int tick = 0; tick < 60; tick++) { retirement.Update(); }
		var route = type.GetField("route", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(traveler);
		var observe = type.GetMethod("UpdateRetirement", BindingFlags.Instance | BindingFlags.NonPublic)!;
		observe.Invoke(traveler, null);
		float low = retirement.Opacity;
		body.position.X += 20;
		observe.Invoke(traveler, null);
		Assert.AreEqual(CrowdRetirementReason.None, retirement.Reasons);
		Assert.IsTrue(retirement.Opacity > low && retirement.Opacity < 1);
		for (int tick = 0; tick < 29; tick++) { observe.Invoke(traveler, null); }
		Assert.AreEqual(1f, retirement.Opacity);
		Assert.AreSame(route, type.GetField("route", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(traveler));
		Assert.AreEqual("Walking", type.GetProperty("Stage")!.GetValue(traveler)!.ToString());
		Assert.AreEqual("original", type.GetProperty("DestinationKey")!.GetValue(traveler));
		Assert.AreEqual(new Vector2(1.5f, 0), body.velocity);
	}

	[TestMethod]
	public void ReplanningWithoutPhysicalProgressEventuallyRetiresBlockedWalker()
	{
		EnableJourneys();
		ResetThreats();
		var traveler = CreateWalkingActor(new Vector2(1608, 1024));
		var type = traveler.GetType();
		var body = (Entity)type.GetProperty("Body")!.GetValue(traveler)!;
		bool beganFading = false;
		for (int tick = 0; tick < 22 * 60; tick++)
		{
			// Simulate an obstruction unknown to the tile route planner.
			body.Bottom = new Vector2(1608, 1024);
			new TownCrowdSystem().PreUpdateNPCs();
			type.GetMethod("Update")!.Invoke(traveler, null);
			beganFading |= RetirementOf(traveler).Reasons.HasFlag(CrowdRetirementReason.Blocked);
		}
		Assert.IsTrue(beganFading);
		Assert.AreEqual(false, type.GetProperty("Active")!.GetValue(traveler));
	}

	[TestMethod]
	public void EnclosedReturnerRetiresAfterRealFailedDepartureAttempts()
	{
		EnableJourneys();
		ResetThreats();
		var traveler = CreateWalkingActor(new Vector2(1608, 1024));
		var type = traveler.GetType();
		type.GetProperty("Intent")!.SetValue(traveler, CrowdIntent.ReturnHome);
		type.GetProperty("Stage")!.SetValue(traveler, Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Planning"));
		for (int y = 0; y < 64; y++)
		{
			foreach (int x in new[] { 98, 102 })
			{
				var tile = Main.tile[x, y];
				tile.HasTile = true;
				tile.TileType = TileID.Stone;
			}
		}
		bool beganFading = false;
		for (int tick = 0; tick < 33 * 60; tick++)
		{
			new TownCrowdSystem().PreUpdateNPCs();
			type.GetMethod("Update")!.Invoke(traveler, null);
			beganFading |= RetirementOf(traveler).Reasons.HasFlag(CrowdRetirementReason.Departure);
		}
		Assert.IsTrue(beganFading);
		Assert.AreEqual(false, type.GetProperty("Active")!.GetValue(traveler));
	}

	[TestMethod]
	public void FadeKeepsConversationUntilCompletionThenCleansBothPartnersAndBubbleOnce()
	{
		EnableJourneys();
		ResetThreats();
		var first = CreateWalkingActor(new Vector2(1608, 1024));
		var second = CreateWalkingActor(new Vector2(1656, 1024));
		var type = first.GetType();
		var clear = ConversationType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic)!;
		clear.Invoke(null, null);
		restore.Add(() => clear.Invoke(null, null));
		Assert.IsTrue((bool)ConversationType.GetMethod("BeginPeers", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [first, second, 600])!);
		var bubbles = (Dictionary<int, Terraria.GameContent.UI.EmoteBubble>)typeof(Terraria.GameContent.UI.EmoteBubble)
			.GetField("byID", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var oldBubbles = bubbles.Keys.ToHashSet();
		restore.Add(() => { foreach (int id in bubbles.Keys.Where(id => !oldBubbles.Contains(id)).ToArray()) { bubbles.Remove(id); } });
		var retirement = RetirementOf(first);
		for (int i = 0; i < 3; i++) { retirement.RecordDepartureFailure(); }
		for (int tick = 0; tick < 1800; tick++) { retirement.ObserveDeparture(true, false, false); }
		type.GetProperty("Intent")!.SetValue(first, CrowdIntent.ReturnHome);
		var view = (CrowdAppearance)type.GetField("view", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(first)!;
		view.UpdateController = _ => { };
		view.DrawInForeground = () => true;
		for (int tick = 0; tick < 119; tick++)
		{
			new TownCrowdSystem().PreUpdateNPCs();
			type.GetMethod("Update")!.Invoke(first, null);
			Assert.AreEqual("Conversing", type.GetProperty("Stage")!.GetValue(first)!.ToString());
			Assert.AreEqual("Conversing", type.GetProperty("Stage")!.GetValue(second)!.ToString());
		}
		Assert.IsTrue(view.RetirementOpacity > 0 && view.RetirementOpacity < 0.01f);
		Assert.IsTrue(bubbles.Values.Any(bubble => !oldBubbles.Contains(bubble.WhoAmI) && bubble.lifeTime > 0));
		new TownCrowdSystem().PreUpdateNPCs();
		type.GetMethod("Update")!.Invoke(first, null);
		Assert.AreEqual(false, type.GetProperty("Active")!.GetValue(first));
		Assert.AreEqual("Walking", type.GetProperty("Stage")!.GetValue(second)!.ToString());
		Assert.IsFalse(bubbles.Values.Any(bubble => !oldBubbles.Contains(bubble.WhoAmI) && bubble.lifeTime > 0));
		Assert.IsFalse(view.Active || view.Visible);
		Assert.IsNull(view.UpdateController);
		Assert.IsNull(view.DrawInForeground);
		Assert.IsNull(type.GetProperty("DestinationKey")!.GetValue(first));
		Assert.IsFalse((bool)type.GetMethod("ReservesStandingPoint", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(first, [new Point(110, 63)])!);
		long peerCooldown = (long)type.GetField("nextConversation", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(second)!;
		new TownCrowdSystem().PreUpdateNPCs();
		type.GetMethod("Kill")!.Invoke(first, null);
		type.GetMethod("Update")!.Invoke(first, null);
		Assert.AreEqual(peerCooldown, type.GetField("nextConversation", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(second));
	}

	[TestMethod]
	public void PausedSimulationDoesNotAdvanceFadeOrBehavior()
	{
		EnableJourneys();
		var traveler = CreateWalkingActor(new Vector2(1608, 1024));
		var type = traveler.GetType();
		var retirement = RetirementOf(traveler);
		retirement.SetReason(CrowdRetirementReason.Threat, true);
		for (int tick = 0; tick < 60; tick++) { retirement.Update(); }
		bool oldPaused = Main.gamePaused;
		restore.Add(() => Main.gamePaused = oldPaused);
		Main.gamePaused = true;
		for (int tick = 0; tick < 300; tick++) { type.GetMethod("Update")!.Invoke(traveler, null); }
		Assert.AreEqual(0.5f, retirement.Opacity, 0.0001f);
		Assert.AreEqual(0, type.GetField("age", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(traveler));
	}

	[TestMethod]
	public void TrappedVisibleTravelerRetiresAfterSustainedThreat()
	{
		EnableJourneys();
		ResetThreats();
		AddEnemy(new Vector2(1800, 1024));
		CaptureThreats(1);
		var traveler = CreateWalkingActor(new Vector2(1608, 1024));
		var type = traveler.GetType();
		var body = (Entity)type.GetProperty("Body")!.GetValue(traveler)!;
		for (int tick = 0; tick < 12 * 60 + 2; tick++)
		{
			body.Bottom = new Vector2(1608, 1024);
			new TownCrowdSystem().PreUpdateNPCs();
			type.GetMethod("Update")!.Invoke(traveler, null);
		}
		Assert.AreEqual(false, type.GetProperty("Active")!.GetValue(traveler), "A continuously threatened traveler trapped on screen must eventually retire.");
	}

	[TestMethod]
	public void DensityChangeKeepsExistingActivitiesAndDisablingCleansUpTravelers()
	{
		EnableJourneys();
		ResetThreats();
		var settings = typeof(TownCrowdSystem).GetProperty(nameof(TownCrowdSystem.Settings))!;
		var oldSettings = settings.GetValue(null);
		restore.Add(() => settings.SetValue(null, oldSettings));
		var travelers = (System.Collections.IList)typeof(TownCrowdSystem).GetField("Travelers", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var previous = travelers.Cast<object>().ToArray();
		travelers.Clear();
		restore.Add(() => { travelers.Clear(); foreach (var person in previous) { travelers.Add(person); } });
		var traveler = CreateWalkingActor(new Vector2(1608, 1024));
		travelers.Add(traveler);
		var apply = typeof(TownCrowdSystem).GetMethod("UpdateConfiguration", BindingFlags.Static | BindingFlags.NonPublic)!;
		foreach (var density in new[] { CrowdDensity.Busy, CrowdDensity.Sparse })
		{
			apply.Invoke(null, [new CrowdConfig { Density = density }]);
			Assert.AreSame(CrowdSettings.ForDensity(density), TownCrowdSystem.Settings);
			Assert.AreSame(traveler, travelers[0]);
			Assert.AreEqual("Walking", traveler.GetType().GetProperty("Stage")!.GetValue(traveler)!.ToString());
			Assert.AreEqual("original", traveler.GetType().GetProperty("DestinationKey")!.GetValue(traveler));
		}
		apply.Invoke(null, [new CrowdConfig { Enabled = false }]);
		Assert.IsFalse(TownCrowdSystem.IsRunning);
		Assert.AreEqual(0, travelers.Count);
		Assert.AreEqual(false, traveler.GetType().GetProperty("Active")!.GetValue(traveler));
	}

	[TestMethod]
	[DataRow(false, NetmodeID.MultiplayerClient)]
	[DataRow(true, NetmodeID.Server)]
	public void ClientConfigurationCannotEnableMultiplayerCrowds(bool server, int netMode)
	{
		EnableJourneys();
		Main.dedServ = server;
		Main.netMode = netMode;
		var apply = typeof(TownCrowdSystem).GetMethod("UpdateConfiguration", BindingFlags.Static | BindingFlags.NonPublic)!;
		apply.Invoke(null, [new CrowdConfig()]);
		Assert.IsFalse(TownCrowdSystem.IsRunning);
	}
	private static Type ConversationType => typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdConversation")!;
	private static Type PlacesType => typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdPlaces")!;
	private static Type PlaceType => typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdPlace")!;
	private static Type ThreatsType => typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdThreats")!;

	private void ResetThreats()
	{
		ThreatsType.GetMethod("Clear")!.Invoke(null, null);
		restore.Add(() => ThreatsType.GetMethod("Clear")!.Invoke(null, null));
	}

	private static void CaptureThreats(long clock)
	{
		typeof(TownCrowdSystem).GetField("clock", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, clock);
		ThreatsType.GetMethod("Update")!.Invoke(null, [clock]);
	}

	private static bool SpawnSafe(Vector2 point) => (bool)ThreatsType.GetMethod("SpawnAllowed")!.Invoke(null, [point])!;

	private NPC AddEnemy(Vector2 feet, bool boss = false)
	{
		var npc = Main.npc[0];
		npc.active = true;
		npc.friendly = false;
		npc.damage = 20;
		npc.life = 100;
		npc.width = 20;
		npc.height = 40;
		npc.type = NPCID.Zombie;
		npc.boss = boss;
		npc.Bottom = feet;
		return npc;
	}

	[TestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void ArenaResidentsOnlyBlockSpawnsDuringActualFight(bool started, bool failed)
	{
		ResetThreats();
		var town = (Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs.InnKeeper)RuntimeHelpers.GetUninitializedObject(
			typeof(Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs.InnKeeper));
		town.ArenaFighting = true;
		town.StartedFight = started;
		town.Fail = failed;
		var npc = AddEnemy(new Vector2(1608, 1024), true);
		npc.friendly = true;
		typeof(NPC).GetProperty(nameof(NPC.ModNPC))!.SetValue(npc, town);
		CaptureThreats(100);
		Assert.AreEqual(!started || failed, SpawnSafe(npc.Bottom));
		Assert.IsTrue(SpawnSafe(npc.Bottom + new Vector2(6000, 0)), "Combat restrictions are local.");
	}

	[TestMethod]
	public void TownEventBlocksLocalArrivalsThenRecoversWithoutBurst()
	{
		ResetThreats();
		var instance = typeof(ContentInstance<Everglow.Yggdrasil.YggdrasilTown.Events.DrunkenMinerInvasion>).GetProperty("Instance")!;
		var old = instance.GetValue(null);
		restore.Add(() => instance.SetValue(null, old));
		var invasion = new Everglow.Yggdrasil.YggdrasilTown.Events.DrunkenMinerInvasion();
		instance.SetValue(null, invasion);
		var active = typeof(Everglow.Commons.Mechanics.Events.ModEvent).GetProperty("Active")!;
		active.SetValue(invasion, true);
		var area = (Rectangle)PlacesType.GetProperty("TownBounds")!.GetValue(null)!;
		Vector2 inside = area.Center.ToVector2();
		Vector2 outside = new(area.Right + 1000, area.Bottom + 1000);
		Assert.IsFalse(SpawnSafe(inside), "An event activated after the snapshot must block births immediately.");
		CaptureThreats(100);
		Assert.IsFalse(SpawnSafe(inside));
		Assert.IsTrue(SpawnSafe(outside));
		active.SetValue(invasion, false);
		CaptureThreats(106);
		Assert.IsFalse(SpawnSafe(inside));
		CaptureThreats(400);
		Assert.IsTrue(SpawnSafe(inside));
		ThreatsType.GetMethod("RecordArrival")!.Invoke(null, [inside]);
		Assert.IsFalse(SpawnSafe(inside), "Several spawn paths in one frame must share the recovery throttle.");
		CaptureThreats(580);
		Assert.IsTrue(SpawnSafe(inside));
	}

	[TestMethod]
	public void EscapeRouteMovesAwayWithoutCrossingTheMonster()
	{
		ResetThreats();
		AddEnemy(new Vector2(1768, 1024));
		CaptureThreats(100);
		new TownCrowdSystem().PreUpdateNPCs();
		object?[] args = [new Point(100, 63), 1, 42, null, 0];
		Assert.IsTrue((bool)typeof(TownCrowdSystem).GetMethod("TryEscape", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, args)!);
		var path = (List<Point>)args[3]!;
		Assert.IsTrue(path[^1].X < 100);
		Assert.IsTrue(path.All(point => point.X <= 100 && point.Y == 63));
		Assert.IsFalse((bool)ThreatsType.GetMethod("EscapeAllowed")!.Invoke(null, [new Vector2(1608, 1024), new Vector2(1768, 1024)])!);
	}

	[TestMethod]
	public void AllBirthSourcesRejectBossAreaBeforeAllocatingGraphics()
	{
		ResetThreats();
		AddEnemy(new Vector2(1608, 1024), true);
		CaptureThreats(100);
		var oldMode = TownCrowdSystem.IsRunning;
		restore.Add(() => typeof(TownCrowdSystem).GetProperty("IsRunning")!.SetValue(null, oldMode));
		typeof(TownCrowdSystem).GetProperty("IsRunning")!.SetValue(null, true);
		var arrival = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdArrival")!;
		var spawn = typeof(TownCrowdSystem).GetMethod("TrySpawnFrom", BindingFlags.Static | BindingFlags.NonPublic)!;
		foreach (string source in new[] { "Street", "Building", "Pylon", "Existing" })
		{
			Assert.IsFalse((bool)spawn.Invoke(null, [new Point(100, 63), Enum.Parse(arrival, source), null])!, source);
		}
		var create = typeof(TownCrowdSystem).GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic)!;
		Assert.IsFalse((bool)create.Invoke(null, [new List<Point> { new(100, 63) }, null, Enum.Parse(arrival, "Train"), null,
			new YggdrasilTown_NormalMetro(), new Vector2(1608, 1024), CrowdIntent.VisitTown])!);
	}

	[TestMethod]
	public void EscapeCanUseShortPlatformBeforeItsSafeEdge()
	{
		ResetThreats();
		for (int x = 5; x < 94; x++)
		{
			var tile = Main.tile[x, 64];
			tile.HasTile = false;
		}
		AddEnemy(new Vector2(1768, 1024));
		CaptureThreats(100);
		new TownCrowdSystem().PreUpdateNPCs();
		object?[] args = [new Point(100, 63), -1, 42, null, 0];
		Assert.IsTrue((bool)typeof(TownCrowdSystem).GetMethod("TryEscape", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, args)!);
		var path = (List<Point>)args[3]!;
		Assert.IsTrue(path[^1].X is >= 94 and < 100);
		Assert.IsTrue(path.All(point => point.X >= 94 && point.Y == 63));
	}

	[TestMethod]
	public void FleeingCanRecenterItsFeetBeforePlanningAtALedge()
	{
		ResetThreats();
		var floor = Main.tile[100, 64];
		floor.HasTile = false;
		AddEnemy(new Vector2(1448, 1024));
		CaptureThreats(100);
		var body = new NPC { width = 16, height = 48 };
		body.Bottom = new Vector2(1608.003f, 1024);
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string name, object value) => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		SetJourneyBody(journey, body);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Fleeing"));
		Set("route", new List<Point>());
		Set("speed", 1.5f);
		Set("escapeDirection", 1);
		new TownCrowdSystem().PreUpdateNPCs();
		type.GetMethod("Flee", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, [true]);
		Assert.IsTrue(body.velocity.X > 0, "A partially supported start must recenter on its supported neighbor before pathfinding.");
	}

	private void EnableJourneys()
	{
		var oldPlayers = Main.player;
		restore.Add(() => Main.player = oldPlayers);
		Main.player = [];
		var current = typeof(SubworldLibrary.SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
		var oldWorld = current.GetValue(null);
		var oldMode = TownCrowdSystem.IsRunning;
		int oldNetMode = Main.netMode;
		bool oldServer = Main.dedServ;
		restore.Add(() =>
		{
			current.SetValue(null, oldWorld);
			typeof(TownCrowdSystem).GetProperty("IsRunning")!.SetValue(null, oldMode);
			Main.netMode = oldNetMode;
			Main.dedServ = oldServer;
		});
		current.SetValue(null, new Everglow.Yggdrasil.YggdrasilWorld());
		typeof(TownCrowdSystem).GetProperty("IsRunning")!.SetValue(null, true);
		Main.netMode = NetmodeID.SinglePlayer;
		Main.dedServ = false;
		Main.screenPosition = new Vector2(1000, 500);
		Main.screenWidth = 1200;
		Main.screenHeight = 900;
	}

	private static void SetJourneyBody(object journey, Entity body)
	{
		var actorType = journey.GetType().Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdBody")!;
		var actor = Activator.CreateInstance(actorType, true)!;
		actorType.GetField("<Body>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, body);
		journey.GetType().GetField("<Actor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, actor);
		journey.GetType().GetField("avoidance", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, new CrowdAvoidance());
		journey.GetType().GetField("retirement", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, new CrowdRetirement());
		journey.GetType().GetField("movementProgress", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, new CrowdProgress());
		journey.GetType().GetField("view", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, new CrowdAppearance { Active = true, Visible = true });
	}

	private static object CreateWalkingActor(Vector2 feet)
	{
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string name, object value) => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		var actorType = type.Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdBody")!;
		var actor = Activator.CreateInstance(actorType, true)!;
		var body = (Entity)actorType.GetProperty("Body")!.GetValue(actor)!;
		body.Bottom = feet;
		SetJourneyBody(journey, body);
		Set("<Actor>k__BackingField", actor);
		Set("<Active>k__BackingField", true);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Walking"));
		Set("view", new CrowdAppearance { Active = true, Visible = true });
		Set("route", new List<Point> { new(100, 63), new(110, 63) });
		Set("waypoint", 1);
		Set("destination", Activator.CreateInstance(PlaceType, CrowdActivity.Scenery, new Point(110, 63), "original", new Vector2(1768, 1024), null, 0)!);
		Set("recentPlaces", new Queue<string>());
		Set("speed", 1.5f);
		Set("pauseInterval", 400);
		Set("routeSeed", 42);
		return journey;
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(1)]
	public void EvacuationWalksOffSolidWoodPlatformWithoutRecenterOscillation(int direction)
	{
		ResetThreats();
		EnableJourneys();
		bool solid = Main.tileSolid[TileID.WoodBlock];
		restore.Add(() => Main.tileSolid[TileID.WoodBlock] = solid);
		Main.tileSolid[TileID.WoodBlock] = true;
		for (int x = 94; x <= 106; x++)
		{
			var tile = Main.tile[x, 62];
			tile.HasTile = true;
			tile.TileType = TileID.WoodBlock;
		}
		var journey = CreateWalkingActor(new Vector2(1608, 992));
		var body = (Entity)journey.GetType().GetProperty("Body")!.GetValue(journey)!;
		AddEnemy(new Vector2(1608 - direction * 200, 992));
		for (int tick = 100; tick < 340; tick++)
		{
			CaptureThreats(tick);
			new TownCrowdSystem().PreUpdateNPCs();
			journey.GetType().GetMethod("Update")!.Invoke(journey, null);
		}
		Assert.AreEqual(1024f, body.Bottom.Y, 0.1f, "Must descend past the platform edge onto the ground one air tile below.");
		Assert.IsTrue((body.Bottom.X - 1608) * direction > 160, "Must continue escaping instead of alternately recentering and stepping off.");
	}

	[TestMethod]
	[DataRow("horizontal")]
	[DataRow("vertical")]
	[DataRow("wall")]
	[DataRow("danger")]
	public void TravelersConverseInPlaceAndResumeTheirOriginalRoutes(string arrangement)
	{
		ResetThreats();
		EnableJourneys();
		var clear = ConversationType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic)!;
		clear.Invoke(null, null);
		restore.Add(() => clear.Invoke(null, null));
		var first = CreateWalkingActor(new Vector2(1608, 1024));
		Vector2 secondFeet = arrangement == "vertical" ? new Vector2(1608, 960) : new Vector2(1656, 1024);
		if (arrangement == "vertical")
		{
			bool solid = Main.tileSolid[TileID.Platforms], top = Main.tileSolidTop[TileID.Platforms];
			restore.Add(() => { Main.tileSolid[TileID.Platforms] = solid; Main.tileSolidTop[TileID.Platforms] = top; });
			Main.tileSolid[TileID.Platforms] = Main.tileSolidTop[TileID.Platforms] = true;
			var tile = Main.tile[100, 60];
			tile.HasTile = true;
			tile.TileType = TileID.Platforms;
		}
		if (arrangement == "wall")
		{
			for (int y = 58; y <= 63; y++)
			{
				var tile = Main.tile[102, y];
				tile.HasTile = true;
				tile.TileType = TileID.Stone;
			}
		}
		var second = CreateWalkingActor(secondFeet);
		if (arrangement == "danger")
		{
			AddEnemy(new Vector2(1768, 1024));
		}
		CaptureThreats(100);
		var begin = ConversationType.GetMethod("BeginPeers", BindingFlags.Static | BindingFlags.NonPublic);
		Assert.IsNotNull(begin, "Lightweight peers need a shared conversation activity.");
		bool started = (bool)begin.Invoke(null, [first, second, 600])!;
		Assert.AreEqual(arrangement is "horizontal" or "vertical", started);
		if (!started) { return; }
		var type = first.GetType();
		Assert.AreEqual("Conversing", type.GetProperty("Stage")!.GetValue(first)!.ToString());
		Assert.AreEqual("Conversing", type.GetProperty("Stage")!.GetValue(second)!.ToString());
		Assert.IsFalse((bool)begin.Invoke(null, [first, second, 600])!, "A traveler cannot join two simultaneous conversations.");
		var bubbles = (Dictionary<int, Terraria.GameContent.UI.EmoteBubble>)typeof(Terraria.GameContent.UI.EmoteBubble)
			.GetField("byID", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var oldBubbles = bubbles.Keys.ToHashSet();
		restore.Add(() => { foreach (int id in bubbles.Keys.Where(id => !oldBubbles.Contains(id)).ToArray()) { bubbles.Remove(id); } });
		var continueChat = ConversationType.GetMethod("ContinueNearby", BindingFlags.Static | BindingFlags.NonPublic)!;
		for (int turn = 0; turn < 2; turn++)
		{
			CaptureThreats(100 + turn * 120);
			Assert.IsTrue((bool)continueChat.Invoke(null, [first])!);
			var bubble = bubbles.Values.Single(bubble => !oldBubbles.Contains(bubble.WhoAmI) && bubble.lifeTime > 0);
			Assert.AreSame(type.GetProperty("Body")!.GetValue(turn == 0 ? first : second), bubble.anchor.entity);
		}
		CaptureThreats(700);
		type.GetMethod("Update")!.Invoke(first, null);
		foreach (var person in new[] { first, second })
		{
			Assert.AreEqual("Walking", type.GetProperty("Stage")!.GetValue(person)!.ToString());
			Assert.AreEqual("original", type.GetProperty("DestinationKey")!.GetValue(person));
		}
		Assert.IsFalse((bool)begin.Invoke(null, [first, second, 600])!, "Cooldown must prevent immediate repeat conversations.");
	}

	[TestMethod]
	[DataRow(99, false)]
	[DataRow(0, true)]
	public void WalkingOpportunityUsesTunedChanceAndDangerInterruptsBothPeers(int roll, bool converses)
	{
		ResetThreats();
		EnableJourneys();
		var clear = ConversationType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic)!;
		clear.Invoke(null, null);
		restore.Add(() => clear.Invoke(null, null));
		var travelers = (System.Collections.IList)typeof(TownCrowdSystem).GetField("Travelers", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var previous = travelers.Cast<object>().ToArray();
		travelers.Clear();
		restore.Add(() => { travelers.Clear(); foreach (var person in previous) { travelers.Add(person); } });
		var first = CreateWalkingActor(new Vector2(1608, 1024));
		var second = CreateWalkingActor(new Vector2(1656, 1024));
		travelers.Add(first);
		travelers.Add(second);
		var bubbles = (Dictionary<int, Terraria.GameContent.UI.EmoteBubble>)typeof(Terraria.GameContent.UI.EmoteBubble).GetField("byID", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var oldBubbles = bubbles.Keys.ToHashSet();
		restore.Add(() => { foreach (int id in bubbles.Keys.Where(id => !oldBubbles.Contains(id)).ToArray()) { bubbles.Remove(id); } });
		Main.rand = new CrowdChanceRandom(roll);
		CaptureThreats(77); // PreUpdateNPCs advances to the 120-tick opportunity for seed 42.
		new TownCrowdSystem().PreUpdateNPCs();
		var type = first.GetType();
		type.GetMethod("Update")!.Invoke(first, null);
		Assert.AreEqual(converses, type.GetProperty("Stage")!.GetValue(first)!.ToString() == "Conversing");
		if (!converses) { return; }
		AddEnemy(new Vector2(1800, 1024));
		CaptureThreats(84);
		type.GetMethod("Update")!.Invoke(first, null);
		Assert.AreEqual("Fleeing", type.GetProperty("Stage")!.GetValue(first)!.ToString());
		Assert.AreEqual("Walking", type.GetProperty("Stage")!.GetValue(second)!.ToString());
		Assert.IsFalse(bubbles.Values.Any(bubble => !oldBubbles.Contains(bubble.WhoAmI) && bubble.lifeTime > 0));
	}

	[TestMethod]
	[DataRow(-1, 0)]
	[DataRow(1, 0)]
	[DataRow(1, 27)]
	public void ChairVisualAnchorMatchesInstalledPlayerSittingHelper(int direction, int style)
	{
		bool old = TileID.Sets.CanBeSatOnForPlayers[TileID.Chairs];
		restore.Add(() => TileID.Sets.CanBeSatOnForPlayers[TileID.Chairs] = old);
		TileID.Sets.CanBeSatOnForPlayers[TileID.Chairs] = true;
		for (int y = 62; y <= 63; y++)
		{
			var tile = Main.tile[100, y];
			tile.HasTile = true;
			tile.TileType = TileID.Chairs;
			tile.TileFrameX = (short)(direction == -1 ? 0 : 18);
			tile.TileFrameY = (short)(style * 40 + (y - 62) * 18);
		}
		var player = new Player { width = 16, height = 48, direction = direction };
		player.Bottom = new Vector2(1608, 1024);
		Assert.IsTrue(Terraria.GameContent.PlayerSittingHelper.GetSittingTargetInfo(player, 100, 63, out _, out var target, out var visual, out _));
		player.sitting.isSitting = true;
		player.sitting.offsetForSeat = visual;
		player.sitting.GetSittingOffsetInfo(player, out var posOffset, out float seatOffset);
		var seats = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdSeats")!;
		object?[] args = [player, new Point(100, 63), null];
		var actual = (Vector2)seats.GetMethod("VisualOffset")!.Invoke(null, args)!;
		Assert.AreEqual(target - player.Bottom + posOffset, actual);
		Assert.AreEqual(seatOffset, (float)args[2]!);
	}

	[TestMethod]
	public void SeatedActorActuallyWalksAwayAndRecoversAfterContinuousSafety()
	{
		ResetThreats();
		EnableJourneys();
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string name, object value) => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		var actorType = type.Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdBody")!;
		var actor = Activator.CreateInstance(actorType, true)!;
		var body = (Entity)actorType.GetProperty("Body")!.GetValue(actor)!;
		body.Bottom = new Vector2(1608, 1024);
		SetJourneyBody(journey, body);
		Set("<Actor>k__BackingField", actor);
		Set("<Active>k__BackingField", true);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Activity"));
		Set("destination", Activator.CreateInstance(PlaceType, CrowdActivity.Sit, new Point(100, 63), "seat", body.Bottom, null, 0)!);
		Set("view", new CrowdAppearance { Active = true, Visible = true });
		Set("route", new List<Point>());
		Set("recentPlaces", new Queue<string>());
		Set("speed", 1.5f);
		Set("pauseInterval", 400);
		Set("routeSeed", 42);
		Assert.IsTrue((bool)type.GetProperty("IsSitting", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(journey)!);
		var enemy = AddEnemy(new Vector2(1768, 1024));
		var update = type.GetMethod("Update")!;
		for (int tick = 100; tick < 220; tick++)
		{
			CaptureThreats(tick);
			new TownCrowdSystem().PreUpdateNPCs();
			update.Invoke(journey, null);
			Assert.IsTrue(body.Bottom.X <= 1608 && Math.Abs(body.Bottom.Y - 1024) < 0.1f);
		}
		Assert.IsTrue(body.Bottom.X < 1500, "The evacuation state must move the actual lightweight body, not only plan a route.");
		Assert.AreEqual("Fleeing", type.GetProperty("Stage")!.GetValue(journey)!.ToString());
		Assert.IsFalse((bool)type.GetProperty("IsSitting", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(journey)!);
		enemy.active = false;
		CaptureThreats(226);
		// Freeze new planning so recovery can be observed without selecting another activity.
		typeof(TownCrowdSystem).GetField("planningBudget", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, 0);
		for (int i = 0; i < 179; i++)
		{
			update.Invoke(journey, null);
		}
		Assert.AreEqual("Fleeing", type.GetProperty("Stage")!.GetValue(journey)!.ToString());
		update.Invoke(journey, null);
		Assert.AreEqual("Planning", type.GetProperty("Stage")!.GetValue(journey)!.ToString());
		Assert.IsTrue((bool)type.GetProperty("Active")!.GetValue(journey)!);
	}

	[TestMethod]
	[DataRow("Riding")]
	[DataRow("Alighting")]
	public void ThreatenedPassengerStaysOnDeckAndFollowsMovingTrain(string stage)
	{
		ResetThreats();
		EnableJourneys();
		AddEnemy(new Vector2(1768, 1024), true);
		CaptureThreats(100);
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string name, object value) => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		var body = new NPC { width = 16, height = 48 };
		body.Bottom = new Vector2(1608, 1008);
		var train = new YggdrasilTown_NormalMetro { Active = true, Position = new Vector2(1200, 1008), Size = new Vector2(1696, 16), State = (int)YggdrasilTown_NormalMetro.BehaviorType.Move };
		SetJourneyBody(journey, body);
		Set("<Active>k__BackingField", true);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, stage));
		Set("train", train);
		Set("aboardOffset", body.position - train.Position);
		Set("view", new CrowdAppearance());
		var update = type.GetMethod("Update")!;
		update.Invoke(journey, null);
		Assert.AreEqual("Riding", type.GetProperty("Stage")!.GetValue(journey)!.ToString());
		Vector2 before = body.position;
		train.Position += new Vector2(100, 0);
		update.Invoke(journey, null);
		Assert.AreEqual(before + new Vector2(100, 0), body.position);
		Assert.AreEqual(Vector2.Zero, body.velocity);
	}

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		var oldTiles = Main.tile;
		int oldWidth = Main.maxTilesX;
		int oldHeight = Main.maxTilesY;
		var oldCenter = YggdrasilTownBiome.BiomeCenter;
		var oldNpcs = Main.npc;
		var oldProjectiles = Main.projectile;
		var oldRandom = Main.rand;
		var oldScreen = Main.screenPosition;
		int oldScreenWidth = Main.screenWidth;
		int oldScreenHeight = Main.screenHeight;
		bool solid = Main.tileSolid[TileID.Stone];
		restore.Add(() =>
		{
			Main.tile = oldTiles;
			Main.maxTilesX = oldWidth;
			Main.maxTilesY = oldHeight;
			Main.npc = oldNpcs;
			Main.projectile = oldProjectiles;
			Main.rand = oldRandom;
			Main.screenPosition = oldScreen;
			Main.screenWidth = oldScreenWidth;
			Main.screenHeight = oldScreenHeight;
			Main.tileSolid[TileID.Stone] = solid;
			YggdrasilTownBiome.BiomeCenter = oldCenter;
		});
		Main.maxTilesX = 256;
		Main.maxTilesY = 128;
		Main.tile = (Tilemap)typeof(Tilemap).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
			null, [typeof(ushort), typeof(ushort)], null)!.Invoke([(ushort)256, (ushort)128]);
		Main.npc = Enumerable.Range(0, Main.maxNPCs).Select(_ => new NPC()).ToArray();
		Main.projectile = [];
		var invasionInstance = typeof(ContentInstance<Everglow.Yggdrasil.YggdrasilTown.Events.DrunkenMinerInvasion>).GetProperty("Instance")!;
		var oldInvasion = invasionInstance.GetValue(null);
		restore.Add(() => invasionInstance.SetValue(null, oldInvasion));
		invasionInstance.SetValue(null, new Everglow.Yggdrasil.YggdrasilTown.Events.DrunkenMinerInvasion());
		Main.rand = new UnifiedRandom(42);
		Main.tileSolid[TileID.Stone] = true;
		YggdrasilTownBiome.BiomeCenter = new Vector2(257, 191) * 16;
		for (int x = 5; x < 251; x++)
		{
			var tile = Main.tile[x, 64];
			tile.HasTile = true;
			tile.TileType = TileID.Stone;
		}
		var manager = new ColliderManager();
		typeof(ColliderManager).GetField("rigidbodies", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(manager, new List<RigidEntity>());
		var instance = typeof(ContentInstance<ColliderManager>).GetProperty(nameof(ContentInstance<ColliderManager>.Instance))!;
		var oldManager = instance.GetValue(null);
		restore.Add(() => instance.SetValue(null, oldManager));
		instance.SetValue(null, manager);
		// Use a fresh catalog without registering content or loading graphics.
		var catalog = typeof(TownCrowdSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		foreach (string name in new[] { "doors", "pylons", "stations" })
		{
			var list = (System.Collections.IList)PlacesType.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(catalog)!;
			var snapshot = list.Cast<object>().ToArray();
			list.Clear();
			restore.Add(() => { list.Clear(); foreach (var entry in snapshot) { list.Add(entry); } });
		}
		foreach (string name in new[] { "clock", "planningBudget" })
		{
			var field = typeof(TownCrowdSystem).GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!;
			var previous = field.GetValue(null);
			restore.Add(() => field.SetValue(null, previous));
		}
	}

	[TestCleanup]
	public void Cleanup()
	{
		foreach (var action in restore.AsEnumerable().Reverse())
		{
			action();
		}
		restore.Clear();
	}

	private static bool Plan(out List<Point>? path, Point? source = null, string? exclude = null, bool departing = false)
	{
		new TownCrowdSystem().PreUpdateNPCs();
		object?[] args = [source ?? new Point(100, 63), null, departing, null, null, exclude, CrowdIntent.VisitTown];
		bool success = (bool)typeof(TownCrowdSystem).GetMethod("TryPlan", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, args)!;
		path = args[4] as List<Point>;
		return success;
	}

	[TestMethod]
	[DataRow("deep")]
	[DataRow("lava")]
	[DataRow("partial")]
	[DataRow("ascending")]
	[DataRow("air")]
	public void PlatformRecoveryRejectsUnsafeOrUnrelatedDrops(string condition)
	{
		bool oldSolid = Main.tileSolid[TileID.Platforms];
		bool oldTop = Main.tileSolidTop[TileID.Platforms];
		restore.Add(() => { Main.tileSolid[TileID.Platforms] = oldSolid; Main.tileSolidTop[TileID.Platforms] = oldTop; });
		Main.tileSolid[TileID.Platforms] = Main.tileSolidTop[TileID.Platforms] = true;
		var slope = Main.tile[100, 62];
		slope.HasTile = condition != "air";
		slope.TileType = TileID.Platforms;
		slope.Slope = SlopeType.SlopeDownLeft;
		var npc = new NPC { width = 16, height = 48 };
		npc.Bottom = new Vector2(condition == "partial" ? 1610 : 1608, 992);
		if (condition == "deep")
		{
			var floor = Main.tile[100, 64];
			floor.HasTile = false;
		}
		if (condition == "partial")
		{
			var floor = Main.tile[101, 64];
			floor.HasTile = false;
		}
		if (condition == "lava")
		{
			var lava = Main.tile[100, 63];
			lava.LiquidType = LiquidID.Lava;
			lava.LiquidAmount = 255;
		}
		if (condition == "ascending")
		{
			npc.velocity.Y = -1;
		}
		var terrain = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdTerrain")!;
		object?[] args = [npc, 0f];
		Assert.IsFalse((bool)terrain.GetMethod("TryPlatformRecovery")!.Invoke(null, args)!);
	}

	[TestMethod]
	[DataRow(1040f, 1024f)]
	[DataRow(1008f, 1024f)]
	[DataRow(1008f, 1012f)]
	public void RecoveryEndsWhenLandingChangesOrBodyPassesTarget(float targetY, float feetY)
	{
		var current = typeof(SubworldLibrary.SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
		var oldWorld = current.GetValue(null);
		var oldMode = TownCrowdSystem.IsRunning;
		int oldNetMode = Main.netMode;
		bool oldServer = Main.dedServ;
		restore.Add(() =>
		{
			current.SetValue(null, oldWorld);
			typeof(TownCrowdSystem).GetProperty("IsRunning")!.SetValue(null, oldMode);
			Main.netMode = oldNetMode;
			Main.dedServ = oldServer;
		});
		current.SetValue(null, new Everglow.Yggdrasil.YggdrasilWorld());
		typeof(TownCrowdSystem).GetProperty("IsRunning")!.SetValue(null, true);
		Main.netMode = NetmodeID.SinglePlayer;
		Main.dedServ = false;
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string name, object value) => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		var npc = new NPC { width = 16, height = 48 };
		npc.Bottom = new Vector2(1608, feetY);
		SetJourneyBody(journey, npc);
		Set("<Active>k__BackingField", true);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Planning"));
		Set("recoveringPlatform", true);
		Set("dropping", true);
		Set("dropSourceY", targetY);
		Set("recentPlaces", new Queue<string>());
		// Force a deferred plan so this tests the recovery transition, not destination selection.
		typeof(TownCrowdSystem).GetField("planningBudget", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, 0);
		type.GetMethod("Update")!.Invoke(journey, null);
		Assert.IsFalse((bool)type.GetField("recoveringPlatform", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(journey)!);
		Assert.IsFalse((bool)type.GetProperty("FallThroughPlatforms")!.GetValue(journey)!);
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(1)]
	public void GroundedLedgeRecoveryReturnsToAdjacentSupportedCell(int direction)
	{
		var floor = Main.tile[100, 64];
		floor.HasTile = false;
		var npc = new NPC { width = 16, height = 48 };
		npc.Bottom = new Vector2(1608 + direction * 0.003f, 1024);
		var terrain = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdTerrain")!;
		Assert.IsTrue((bool)terrain.GetMethod("Grounded")!.Invoke(null, [npc])!);
		var recover = terrain.GetMethod("TryStandingRecovery");
		Assert.IsNotNull(recover);
		object?[] args = [npc, 0f];
		Assert.IsTrue((bool)recover.Invoke(null, args)!);
		Assert.AreEqual(1608f + direction * 16, args[1]);
		for (int i = 0; i < 16; i++)
		{
			npc.velocity = new Vector2(Math.Clamp((float)args[1]! - npc.Bottom.X, -1.5f, 1.5f), CrowdNavigation.Gravity);
			npc.velocity = Collision.TileCollision(npc.position, npc.velocity, npc.width, npc.height);
			npc.position += npc.velocity;
		}
		Assert.AreEqual(1024f, npc.Bottom.Y);
		Assert.AreEqual((float)args[1]!, npc.Bottom.X);
	}

	[TestMethod]
	[DataRow("air")]
	[DataRow("deep")]
	[DataRow("hazard")]
	public void StandingRecoveryRejectsUnsupportedOrUnsafeNudges(string condition)
	{
		for (int x = 99; x <= 101; x++)
		{
			var floor = Main.tile[x, 64];
			floor.HasTile = x == 99 && condition != "deep";
		}
		var npc = new NPC { width = 16, height = 48 };
		npc.Bottom = new Vector2(1607.997f, condition == "air" ? 1020 : 1024);
		if (condition == "hazard")
		{
			var lava = Main.tile[99, 63];
			lava.LiquidType = LiquidID.Lava;
			lava.LiquidAmount = 255;
		}
		var terrain = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdTerrain")!;
		object?[] args = [npc, 0f];
		Assert.IsFalse((bool)terrain.GetMethod("TryStandingRecovery")!.Invoke(null, args)!);
	}

	[TestMethod]
	public void PlannerAvoidsAStandingPositionReservedByAnotherActivity()
	{
		Main.rand = new UnifiedRandom(42);
		Assert.IsTrue(Plan(out var firstPath));
		var cell = firstPath![^1];
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var resident = RuntimeHelpers.GetUninitializedObject(type);
		type.GetProperty("Active")!.SetValue(resident, true);
		type.GetProperty("Stage")!.SetValue(resident, Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Activity"));
		type.GetField("destination", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(resident,
			Activator.CreateInstance(PlaceType, CrowdActivity.Scenery, cell, "different-activity", cell.ToVector2() * 16, null, 0));
		var travelers = (System.Collections.IList)typeof(TownCrowdSystem).GetField("Travelers", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		travelers.Add(resident);
		restore.Add(() => travelers.Remove(resident));
		Main.rand = new UnifiedRandom(42);
		Assert.IsTrue(Plan(out var nextPath));
		Assert.IsTrue(Math.Abs(nextPath![^1].X - cell.X) >= 2 || Math.Abs(nextPath[^1].Y - cell.Y) >= 3,
			"Distinct activity keys must not allow two people to stand on the same ground coordinate.");
	}

	[TestMethod]
	public void SharedActivityUsesDistinctSupportedStandingPositions()
	{
		var catalog = typeof(TownCrowdSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var method = PlacesType.GetMethod("TryStandingPlace")!;
		Assert.IsNotNull(method, "The activity catalog needs safe standing-space selection.");
		var place = Activator.CreateInstance(PlaceType, CrowdActivity.Scenery, new Point(100, 63), "shared", new Vector2(1608, 1024), null, 0)!;
		var reserved = new List<Point> { new(100, 63) };
		bool Occupied(Point cell) => reserved.Any(other => Math.Abs(other.X - cell.X) < 2 && Math.Abs(other.Y - cell.Y) < 3);
		for (int i = 0; i < 2; i++)
		{
			object?[] args = [place, (Func<Point, bool>)Occupied, null];
			Assert.IsTrue((bool)method.Invoke(catalog, args)!);
			var cell = (Point)PlaceType.GetProperty("Cell")!.GetValue(args[2])!;
			Assert.IsFalse(Occupied(cell));
			Assert.AreEqual(63, cell.Y);
			Assert.AreEqual("shared", PlaceType.GetProperty("Key")!.GetValue(args[2]));
			reserved.Add(cell);
		}
	}

	[TestMethod]
	public void StandingAlternativesStayOnTheSameSideOfAnInterveningWall()
	{
		for (int y = 56; y < 64; y++)
		{
			var tile = Main.tile[99, y];
			tile.HasTile = true;
			tile.TileType = TileID.Stone;
		}
		var catalog = typeof(TownCrowdSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var place = Activator.CreateInstance(PlaceType, CrowdActivity.Scenery, new Point(100, 63), "wall-side", new Vector2(1608, 1024), null, 0)!;
		object?[] args = [place, (Func<Point, bool>)(cell => cell == new Point(100, 63)), null];
		Assert.IsTrue((bool)PlacesType.GetMethod("TryStandingPlace")!.Invoke(catalog, args)!);
		Assert.AreEqual(new Point(102, 63), PlaceType.GetProperty("Cell")!.GetValue(args[2]),
			"The free point behind a wall must not hide a reachable point beside the original activity.");
	}

	[TestMethod]
	public void StandingAlternativesCannotUseMissingFloorOrMoveAnExit()
	{
		var catalog = typeof(TownCrowdSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var method = PlacesType.GetMethod("TryStandingPlace")!;
		Assert.IsNotNull(method);
		foreach (var activity in new[] { CrowdActivity.EnterBuilding, CrowdActivity.LeavePylon })
		{
			var exit = Activator.CreateInstance(PlaceType, activity, new Point(100, 63), "fixed", new Vector2(1608, 1024), null, 0)!;
			object?[] exitArgs = [exit, (Func<Point, bool>)(cell => cell == new Point(100, 63)), null];
			Assert.IsFalse((bool)method.Invoke(catalog, exitArgs)!, "A free neighboring floor cannot move an entrance animation.");
		}
		for (int x = 94; x <= 106; x++)
		{
			if (x != 100)
			{
				var tile = Main.tile[x, 64];
				tile.HasTile = false;
			}
		}
		var place = Activator.CreateInstance(PlaceType, CrowdActivity.Scenery, new Point(100, 63), "fixed", new Vector2(1608, 1024), null, 0)!;
		object?[] args = [place, (Func<Point, bool>)(cell => cell == new Point(100, 63)), null];
		Assert.IsFalse((bool)method.Invoke(catalog, args)!);
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(1)]
	public void WaitingPositionsExpandAwayFromTheTrain(int side)
	{
		var catalog = typeof(TownCrowdSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var method = PlacesType.GetMethod("TryStandingPlace")!;
		Assert.IsNotNull(method);
		var place = Activator.CreateInstance(PlaceType, CrowdActivity.BoardTrain, new Point(100, 63), "station", new Vector2(1608 - side * 912, 1040), null, 0)!;
		object?[] args = [place, (Func<Point, bool>)(cell => cell == new Point(100, 63)), null];
		Assert.IsTrue((bool)method.Invoke(catalog, args)!);
		var cell = (Point)PlaceType.GetProperty("Cell")!.GetValue(args[2])!;
		Assert.IsTrue((cell.X - 100) * side >= 2, "Waiting space must not expand inward onto the train deck.");
	}

	[TestMethod]
	public void ActualTownPlannerWorksWithoutStationAndChoosesBothDirections()
	{
		bool left = false;
		bool right = false;
		for (int seed = 0; seed < 24; seed++)
		{
			Main.rand = new UnifiedRandom(seed);
			if (Plan(out var path))
			{
				Assert.IsNotNull(path);
				Assert.IsTrue(path.Count > 1);
				Assert.IsTrue(path.All(point => point.Y == 63));
				left |= path[^1].X < 100;
				right |= path[^1].X > 100;
			}
		}
		Assert.IsTrue(left && right, "Town activities must be reachable in both directions without any station or pylon.");
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void OrdinaryActivityCannotOccupyTrainDeck(bool moving)
	{
		var train = new YggdrasilTown_NormalMetro
		{
			Position = new Vector2(1200, 1008), Size = new Vector2(600, 16), Active = true,
			State = (int)(moving ? YggdrasilTown_NormalMetro.BehaviorType.Move : YggdrasilTown_NormalMetro.BehaviorType.Stop),
			Velocity = moving ? Vector2.UnitX : Vector2.Zero,
		};
		((List<RigidEntity>)typeof(ColliderManager).GetField("rigidbodies", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(ColliderManager.Instance)!).Add(train);
		var terrain = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdTerrain")!;
		var stand = terrain.GetMethod("CanStand")!;
		Assert.IsFalse((bool)stand.Invoke(null, [new Point(100, 63), null])!, "Normal activities must not overlap the solid deck even when it is not a walkable support.");
		if (!moving)
		{
			Assert.IsTrue((bool)stand.Invoke(null, [new Point(100, 62), train])!);
		}
	}

	[TestMethod]
	[DataRow(1440)]
	[DataRow(1342)]
	public void CrowdOnTrainRemainsVisibleIncludingAtDoor(int x)
	{
		Main.screenPosition = new Vector2(400, 0);
		Main.screenWidth = 2400;
		Main.screenHeight = 1400;
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var body = new NPC { width = 16, height = 48, position = new Vector2(x, 960) };
		var train = new YggdrasilTown_NormalMetro { Position = new Vector2(1200, 1008), Size = new Vector2(600, 16), Active = true };
		var view = new CrowdAppearance { RetirementOpacity = 0.4f };
		SetJourneyBody(journey, body);
		type.GetField("train", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, train);
		type.GetField("view", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, view);
		type.GetField("aboardOffset", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, body.position - train.Position);
		type.GetField("stageTime", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, 60);
		type.GetMethod("Ride", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, null);
		Assert.AreEqual(1f, view.Opacity, "Passengers must remain visible throughout their ride, including at a door.");
		Assert.AreEqual(0.4f, view.EffectiveOpacity, "Riding must preserve the independent retirement multiplier.");
	}

	[TestMethod]
	[DataRow("Riding", 1400, 960, true)]
	[DataRow("Boarding", 1400, 960, true)]
	[DataRow("Alighting", 1400, 960, true)]
	[DataRow("Boarding", 1170, 960, false)]
	[DataRow("Alighting", 1810, 960, false)]
	[DataRow("Boarding", 1400, 800, false)]
	[DataRow("Alighting", 1400, 1040, false)]
	[DataRow("Walking", 1400, 960, false)]
	public void PassengerUsesTrainInteriorLayerOnlyWhileInsideItsCarriage(string stage, int x, int y, bool inside)
	{
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var body = new NPC { width = 16, height = 48, position = new Vector2(x, y) };
		var train = new YggdrasilTown_NormalMetro { Position = new Vector2(1200, 1008), Size = new Vector2(600, 16), Active = true };
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		SetJourneyBody(journey, body);
		Set("<Active>k__BackingField", true);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, stage));
		Set("train", train);
		var interior = type.GetProperty("InteriorTrain", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(interior, "Passengers need a carriage-owned draw layer instead of the late foreground VFX layer.");
		foreach (float shellAlpha in new[] { 0f, 0.5f, 1f })
		{
			train.ShellAlpha = shellAlpha;
			Assert.AreEqual(inside ? train : null, interior.GetValue(journey), "The shell must occlude passengers through draw order, not passenger opacity.");
		}
		train.Active = false;
		Assert.IsNull(interior.GetValue(journey));
		train.Active = true;
		Set("<Active>k__BackingField", false);
		Assert.IsNull(interior.GetValue(journey));
	}

	[TestMethod]
	public void ExitRequiresTravelerToActuallyReachEndpoint()
	{
		var tile = Main.tile[40, 60];
		tile.HasTile = true;
		tile.TileType = TileID.Rope;
		var place = Activator.CreateInstance(PlaceType, CrowdActivity.LeavePylon, new Point(42, 63), "pylon", new Vector2(680, 1024), new Point(40, 60), (int)TileID.Rope)!;
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var body = new NPC { width = 16, height = 48, position = new Vector2(1000, 976) };
		SetJourneyBody(journey, body);
		type.GetField("destination", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, place);
		type.GetMethod("ReachDestination", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, null);
		Assert.AreEqual("Planning", type.GetProperty("Stage")!.GetValue(journey)!.ToString(), "A displaced traveler cannot fade out at an endpoint it has not reached.");
	}

	[TestMethod]
	public void LightweightNameHoverRespectsVisibilityTrainShellAndActualDrawPosition()
	{
		var assembly = typeof(TownCrowdSystem).Assembly;
		var type = assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var actor = Activator.CreateInstance(assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdBody")!, true)!;
		var body = (Entity)actor.GetType().GetProperty("Body")!.GetValue(actor)!;
		body.position = new Vector2(1400, 960);
		var view = new CrowdAppearance { Position = new Vector2(1408, 960), Opacity = 1, Active = true, Visible = true };
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		SetJourneyBody(journey, body);
		Set("<Actor>k__BackingField", actor);
		Set("<Active>k__BackingField", true);
		Set("view", view);
		var hover = type.GetMethod("CanShowNameAt", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(hover);
		bool At(Vector2 point) => (bool)hover.Invoke(journey, [point])!;
		Assert.IsTrue(At(new Vector2(1408, 980)));
		Assert.IsFalse(At(new Vector2(1500, 980)));
		view.Opacity = 0.05f;
		Assert.IsFalse(At(new Vector2(1408, 980)));
		view.Opacity = 1;
		var train = new YggdrasilTown_NormalMetro { Position = new Vector2(1200, 1008), Size = new Vector2(600, 16), Active = true, ShellAlpha = 1 };
		Set("train", train);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Riding"));
		Assert.IsFalse(At(new Vector2(1408, 980)), "An opaque train must not reveal passengers through a name tooltip.");
		train.ShellAlpha = 0;
		Assert.IsTrue(At(new Vector2(1408, 980)));
		view.Visible = false;
		Assert.IsFalse(At(new Vector2(1408, 980)));
	}

	[TestMethod]
	[DataRow("valid")]
	[DataRow("destroyed")]
	[DataRow("displaced")]
	[DataRow("airborne")]
	public void PylonReturnCompletesOnlyAtAnIntactReachedExit(string scenario)
	{
		bool oldServer = Main.dedServ;
		restore.Add(() => Main.dedServ = oldServer);
		Main.dedServ = true; // Exercise the real departure state without graphics or particle creation.
		var tile = Main.tile[40, 60];
		tile.HasTile = scenario != "destroyed";
		tile.TileType = TileID.Rope;
		var place = Activator.CreateInstance(PlaceType, CrowdActivity.LeavePylon, new Point(42, 63), "pylon",
			new Vector2(680, 1024), new Point(40, 60), (int)TileID.Rope)!;
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var body = new NPC { active = true, width = 16, height = 48, position = new Vector2(scenario == "displaced" ? 900 : 672, 976) };
		var view = new CrowdAppearance { RetirementOpacity = 0.4f };
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		SetJourneyBody(journey, body);
		Set("<Active>k__BackingField", true);
		Set("<Stage>k__BackingField", Enum.Parse(type.GetProperty("Stage")!.PropertyType, "Leaving"));
		Set("destination", place);
		Set("view", view);
		Set("stageTime", 75);
		var leave = type.GetMethod("Leave", BindingFlags.Instance | BindingFlags.NonPublic)!;
		leave.Invoke(journey, [scenario != "airborne"]);
		Assert.IsTrue(body.active);
		Assert.AreEqual(scenario == "valid" ? 0.5f : 1f, view.Opacity, 0.001f);
		Assert.AreEqual(scenario == "valid" ? 0.2f : 0.4f, view.EffectiveOpacity, 0.001f);
		if (scenario == "valid")
		{
			Set("stageTime", 90);
			leave.Invoke(journey, [true]);
			Assert.IsFalse((bool)type.GetProperty("Active")!.GetValue(journey)!);
			Assert.IsFalse(view.Active);
		}
		else
		{
			Assert.AreEqual("Planning", type.GetProperty("Stage")!.GetValue(journey)!.ToString());
		}
	}

	[TestMethod]
	public void IncomingPassengerSurvivesOffscreenAndTracksFractionalTrainMovement()
	{
		Main.screenPosition = Vector2.Zero;
		Main.screenWidth = 800;
		Main.screenHeight = 600;
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		var body = new NPC { active = true, width = 16, height = 48 };
		var train = new YggdrasilTown_NormalMetro { Active = true, Position = new Vector2(2000.5f, 1008), Size = new Vector2(1696, 16) };
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		SetJourneyBody(journey, body);
		Set("<Active>k__BackingField", true);
		Set("train", train);
		Set("view", new CrowdAppearance());
		Set("aboardOffset", new Vector2(100, -48));
		var ride = type.GetMethod("Ride", BindingFlags.Instance | BindingFlags.NonPublic)!;
		for (int i = 0; i < 200; i++)
		{
			train.Position += new Vector2(0.25f, 0);
			ride.Invoke(journey, null);
		}
		Assert.IsTrue((bool)type.GetProperty("Active")!.GetValue(journey)!);
		Assert.AreEqual(train.Position + new Vector2(100, -48), body.position);
		train.State = (int)YggdrasilTown_NormalMetro.BehaviorType.Stop;
		ride.Invoke(journey, null);
		train.State = (int)YggdrasilTown_NormalMetro.BehaviorType.Move;
		for (int i = 0; i < 180; i++)
		{
			ride.Invoke(journey, null);
		}
		Assert.IsFalse((bool)type.GetProperty("Active")!.GetValue(journey)!);
	}

	[TestMethod]
	public void LightweightPassengerDoesNotApplyTrainDisplacementTwiceAfterRiding()
	{
		var assembly = typeof(TownCrowdSystem).Assembly;
		var type = assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var actorType = assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdBody")!;
		var actor = Activator.CreateInstance(actorType, true)!;
		var body = (Entity)actorType.GetProperty("Body")!.GetValue(actor)!;
		body.position = new Vector2(1342, 960);
		var train = new YggdrasilTown_NormalMetro { Active = true, Position = new Vector2(1200, 1008), Size = new Vector2(1696, 16) };
		actorType.GetField("ground", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, train);
		actorType.GetField("groundPosition", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(actor, train.Position);
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		SetJourneyBody(journey, body);
		Set("<Actor>k__BackingField", actor);
		Set("train", train);
		Set("view", new CrowdAppearance());
		type.GetMethod("AttachToTrain", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, null);
		train.Position += new Vector2(400, 0);
		type.GetMethod("Ride", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, null);
		Vector2 aboard = body.position;
		actorType.GetMethod("Move")!.Invoke(actor, [false]);
		Assert.AreEqual(aboard, body.position, "The first detached movement must not replay the train's entire journey.");
	}

	[TestMethod]
	public void DepartureCanReachBeyondWideViewportInsteadOfWaitingForever()
	{
		Main.screenPosition = new Vector2(408, 0);
		Main.screenWidth = 2400;
		Main.screenHeight = 1400;
		bool found = false;
		for (int i = 0; i < 8; i++)
		{
			if (Plan(out var path, departing: true))
			{
				found = true;
				Assert.IsNotNull(path);
				int x = path[^1].X * 16 + 8;
				Assert.IsTrue(x < 248 || x > 2968);
			}
		}
		Assert.IsTrue(found);
	}

	[TestMethod]
	public void EnclosedTravelerDoesNotPlanThroughWallsToReachActivity()
	{
		foreach (int x in new[] { 97, 103 })
		{
			for (int y = 5; y < 64; y++)
			{
				var tile = Main.tile[x, y];
				tile.HasTile = true;
				tile.TileType = TileID.Stone;
			}
		}
		Assert.IsFalse(Plan(out _));
	}

	[TestMethod]
	public void EntranceCannotRemainValidAfterItsApproachFloorIsRemoved()
	{
		var catalog = Activator.CreateInstance(PlacesType, true)!;
		var origin = new Point(40, 60);
		((Dictionary<int, Point>)PlacesType.GetField("doorSizes", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(catalog)!).Add(TileID.Rope, new Point(5, 4));
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 4; y++)
			{
				var tile = Main.tile[40 + x, 60 + y];
				tile.HasTile = true;
				tile.TileType = TileID.Rope;
				tile.TileFrameX = (short)(x * 18);
				tile.TileFrameY = (short)(y * 18);
			}
		}
		var place = Activator.CreateInstance(PlaceType, CrowdActivity.EnterBuilding, new Point(42, 63), "door", new Vector2(680, 1024), origin, (int)TileID.Rope)!;
		var valid = PlacesType.GetMethod("Valid", BindingFlags.Instance | BindingFlags.Public)!;
		Assert.IsTrue((bool)valid.Invoke(catalog, [place])!);
		var floor = Main.tile[42, 64];
		floor.HasTile = false;
		Assert.IsFalse((bool)valid.Invoke(catalog, [place])!, "A surviving door tile cannot authorize fading out above a removed floor.");
		floor.HasTile = true;
		var doorPart = Main.tile[44, 63];
		doorPart.HasTile = false;
		Assert.IsFalse((bool)valid.Invoke(catalog, [place])!, "An incomplete multi-tile door must be rejected.");
	}

	[TestMethod]
	public void OnlyGiantOriginPylonIsCataloguedAndSmallPylonsCannotSeedOrOfferActivities()
	{
		YggdrasilTownBiome.BiomeCenter += new Vector2(10000, 0);
		var instance = typeof(ContentInstance<OriginPylon>).GetProperty("Instance")!;
		var previous = instance.GetValue(null);
		restore.Add(() => instance.SetValue(null, previous));
		var giant = new OriginPylon();
		typeof(ModTile).GetProperty("Type")!.SetValue(giant, (ushort)TileID.Rope);
		instance.SetValue(null, giant);
		var catalog = Activator.CreateInstance(PlacesType, true)!;
		var inspect = PlacesType.GetMethod("Inspect", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var list = (System.Collections.IList)PlacesType.GetField("pylons", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(catalog)!;
		var small = Main.tile[100, 59];
		small.HasTile = true;
		small.TileType = TileID.TeleportationPylon;
		inspect.Invoke(catalog, [100, 59]);
		Assert.AreEqual(0, list.Count);
		var markerType = PlacesType.GetNestedType("CrowdLandmark", BindingFlags.NonPublic)!;
		list.Add(Activator.CreateInstance(markerType, new Point(100, 59), (int)TileID.TeleportationPylon, 1, 1)!);
		var entrances = (System.Collections.IEnumerable)PlacesType.GetMethod("Entrances")!.Invoke(catalog, [new Vector2(1608, 1024), true])!;
		Assert.AreEqual(0, entrances.Cast<object>().Count(), "Even a stale small-pylon entry must not generate an arrival or return target.");
		Assert.IsFalse((bool)PlacesType.GetMethod("CanSeed")!.Invoke(catalog, [new Vector2(1608, 1024)])!);
		list.Clear();
		for (int x = 100; x < 108; x++)
		{
			for (int y = 50; y < 62; y++)
			{
				var tile = Main.tile[x, y];
				tile.HasTile = true;
				tile.TileType = TileID.Rope;
				tile.TileFrameX = (short)((x - 100) * 18);
				tile.TileFrameY = (short)((y - 50) * 18);
			}
		}
		inspect.Invoke(catalog, [100, 50]);
		Assert.AreEqual(1, list.Count);
		Assert.AreEqual(1, entrances.Cast<object>().Count());
		Assert.IsTrue((bool)PlacesType.GetMethod("CanSeed")!.Invoke(catalog, [new Vector2(1672, 1024)])!);
	}

	[TestMethod]
	public void NativeResidentsDoNotBecomeConversationPartners()
	{
		ResetThreats();
		EnableJourneys();
		var npc = Main.npc[0];
		npc.active = npc.friendly = npc.townNPC = true;
		npc.width = 18;
		npc.height = 40;
		npc.Bottom = new Vector2(1640, 1024);
		npc.aiStyle = NPCAIStyleID.Passive;
		var journey = CreateWalkingActor(new Vector2(1608, 1024));
		Assert.IsFalse(typeof(GlobalNPC).IsAssignableFrom(ConversationType));
		Assert.IsFalse((bool)ConversationType.GetMethod("TryNearby", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [journey])!);
		Assert.AreEqual("Walking", journey.GetType().GetProperty("Stage")!.GetValue(journey)!.ToString());
	}

	[TestMethod]
	public void PylonPlazaOffersLocalActivityBeforeDepartingForTown()
	{
		YggdrasilTownBiome.BiomeCenter += new Vector2(10000, 0);
		var pylonInstance = typeof(ContentInstance<OriginPylon>).GetProperty(nameof(ContentInstance<OriginPylon>.Instance))!;
		var previous = pylonInstance.GetValue(null);
		restore.Add(() => pylonInstance.SetValue(null, previous));
		var pylon = new OriginPylon();
		typeof(ModTile).GetProperty(nameof(ModTile.Type))!.SetValue(pylon, (ushort)TileID.Rope);
		pylonInstance.SetValue(null, pylon);
		var origin = new Point(100, 50);
		var tile = Main.tile[100, 50];
		tile.HasTile = true;
		tile.TileType = TileID.Rope;
		var catalog = typeof(TownCrowdSystem).GetField("Places", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var list = (System.Collections.IList)PlacesType.GetField("pylons", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(catalog)!;
		var markerType = PlacesType.GetNestedType("CrowdLandmark", BindingFlags.NonPublic)!;
		list.Add(Activator.CreateInstance(markerType, origin, (int)TileID.Rope, 1, 1)!);
		bool found = false;
		for (int i = 0; i < 8; i++)
		{
			found |= Plan(out _, new Point(104, 63), $"tile:{origin}");
		}
		Assert.IsTrue(found, "A pylon arrival must have a safe plaza activity even when the town is elsewhere.");
	}

	[TestMethod]
	public void ChairUsesBottomAnchorAndYieldsToPlayerAndResident()
	{
		var dataField = typeof(Terraria.ObjectData.TileObjectData).GetField("_data", BindingFlags.Static | BindingFlags.NonPublic)!;
		var oldData = dataField.GetValue(null);
		bool oldSeating = TileID.Sets.CanBeSatOnForNPCs[TileID.Chairs];
		var oldPlayers = Main.player;
		restore.Add(() => { dataField.SetValue(null, oldData); TileID.Sets.CanBeSatOnForNPCs[TileID.Chairs] = oldSeating; Main.player = oldPlayers; });
		var data = new Terraria.ObjectData.TileObjectData();
		typeof(Terraria.ObjectData.TileObjectData).GetMethod("SetupBaseObject", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(data, null);
		data.Width = 1;
		data.Height = 2;
		data.CoordinateHeights = [16, 18];
		data.CoordinateWidth = 16;
		data.CoordinatePadding = 2;
		data.CoordinatePaddingFix = new Terraria.DataStructures.Point16(0, 2);
		var datas = Enumerable.Repeat<Terraria.ObjectData.TileObjectData>(null!, TileID.Count).ToList();
		datas[TileID.Chairs] = data;
		dataField.SetValue(null, datas);
		TileID.Sets.CanBeSatOnForNPCs[TileID.Chairs] = true;
		Main.player = [new Player { active = true }];
		for (int y = 62; y <= 63; y++)
		{
			var tile = Main.tile[100, y];
			tile.HasTile = true;
			tile.TileType = TileID.Chairs;
			tile.TileFrameY = (short)((y - 62) * 18);
		}
		var seats = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdSeats")!;
		object?[] args = [new Point(100, 62), default(Point)];
		Assert.IsTrue((bool)seats.GetMethod("TryFind")!.Invoke(null, args)!);
		Assert.AreEqual(new Point(100, 63), args[1]);
		bool Available() => (bool)seats.GetMethod("Available")!.Invoke(null, [new Point(100, 63)])!;
		Assert.IsTrue(Available());
		Main.player[0].Bottom = new Vector2(1608, 1008);
		Main.player[0].sitting.isSitting = true;
		Assert.IsFalse(Available());
		Main.player[0].sitting.isSitting = false;
		Main.npc[0].active = Main.npc[0].townNPC = true;
		Main.npc[0].Bottom = new Vector2(1608, 1024);
		Assert.IsFalse(Available());
		Main.npc[0].active = false;
		var top = Main.tile[100, 62];
		top.HasTile = false;
		Assert.IsFalse(Available(), "A missing chair half must cancel sitting.");
	}

	[TestMethod]
	public void NewReturnerCannotFallBackToAnUnrelatedSightseeingRoute()
	{
		Main.rand = new CrowdChanceRandom(0);
		var mode = typeof(TownCrowdSystem).GetField("<IsRunning>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic)!;
		var oldMode = mode.GetValue(null);
		restore.Add(() => mode.SetValue(null, oldMode));
		mode.SetValue(null, true);
		new TownCrowdSystem().PreUpdateNPCs();
		var arrivalType = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdArrival")!;
		var spawn = typeof(TownCrowdSystem).GetMethod("TrySpawnFrom", BindingFlags.Static | BindingFlags.NonPublic)!;
		Assert.IsFalse((bool)spawn.Invoke(null, [new Point(100, 63), Enum.Parse(arrivalType, "Existing"), null])!,
			"No pylons exist in this fixture: creating a returner must fail before allocating a body or a VFX.");
	}

	[TestMethod]
	public void ExhaustedPlanningBudgetDoesNotCountAsFailedDeparture()
	{
		Main.screenPosition = new Vector2(800, 600);
		Main.screenWidth = 800;
		Main.screenHeight = 600;
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		SetJourneyBody(journey, new NPC { width = 16, height = 48, position = new Vector2(1600, 976) });
		Set("recentPlaces", new Queue<string>());
		Set("departureRequested", true);
		typeof(TownCrowdSystem).GetField("planningBudget", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, 0);
		type.GetMethod("ChooseNextActivity", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, null);
		Assert.AreEqual(0, ((CrowdRetirement)type.GetField("retirement", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(journey)!).DepartureFailures);
		Assert.IsTrue((int)type.GetField("planDelay", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(journey)! < 5);
	}

	[TestMethod]
	public void GuildUseSharesDoorCooldownAndDoesNotConsumeItOnRejectedAttempt()
	{
		var cooldowns = (Dictionary<string, long>)typeof(TownCrowdSystem).GetField("EntranceCooldowns", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		const string key = "test:guild-cooldown";
		var enter = typeof(TownCrowdSystem).GetMethod("TryEnterGuild", BindingFlags.Static | BindingFlags.NonPublic)!;
		var clock = typeof(TownCrowdSystem).GetField("clock", BindingFlags.Static | BindingFlags.NonPublic)!;
		try
		{
			clock.SetValue(null, 1L);
			Assert.IsTrue((bool)enter.Invoke(null, [key])!);
			clock.SetValue(null, 2L);
			Assert.IsFalse((bool)enter.Invoke(null, [key])!);
			Assert.AreEqual(1L, cooldowns[key]);
			clock.SetValue(null, 1L + CrowdSettings.DoorCooldownTicks);
			Assert.IsTrue((bool)enter.Invoke(null, [key])!);
		}
		finally
		{
			cooldowns.Remove(key);
		}
	}

	[TestMethod]
	public void GuildEntranceUsesSceneOpeningAndRejectsBlockedOrMissingAnchor()
	{
		var catalog = Activator.CreateInstance(PlacesType, true)!;
		PlacesType.GetField("guildType", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(catalog, (int)TileID.Rope);
		((Dictionary<int, Point>)PlacesType.GetField("doorSizes", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(catalog)!).Add(TileID.Rope, new Point(1, 1));
		var origin = new Point(40, 56);
		var tile = Main.tile[40, 56];
		tile.HasTile = true;
		tile.TileType = TileID.Rope;
		PlacesType.GetMethod("DiscoverNear")!.Invoke(catalog, [new Rectangle(600, 850, 200, 200)]);
		var entrances = ((System.Collections.IEnumerable)PlacesType.GetMethod("Entrances")!.Invoke(catalog, [new Vector2(700, 1024), false])!).Cast<object>().ToArray();
		Assert.AreEqual(1, entrances.Length);
		var place = entrances[0];
		Assert.AreEqual(CrowdActivity.EnterGuild, PlaceType.GetProperty("Activity")!.GetValue(place));
		Assert.AreEqual(new Point(43, 63), PlaceType.GetProperty("Cell")!.GetValue(place));
		var valid = PlacesType.GetMethod("Valid", BindingFlags.Instance | BindingFlags.Public)!;
		Assert.IsTrue((bool)valid.Invoke(catalog, [place])!);
		var wall = Main.tile[43, 62];
		wall.HasTile = true;
		wall.TileType = TileID.Stone;
		Assert.IsFalse((bool)valid.Invoke(catalog, [place])!);
		wall.HasTile = false;
		tile.HasTile = false;
		Assert.IsFalse((bool)valid.Invoke(catalog, [place])!);
	}

	[TestMethod]
	public void SightseeingFallbackDoesNotEraseRepeatedlyFailedReturns()
	{
		Main.screenPosition = Vector2.Zero;
		Main.screenWidth = 4096;
		Main.screenHeight = 2048; // Every safe street point is visible, and this plaza has no entrance.
		var type = typeof(TownCrowdSystem).Assembly.GetType("Everglow.Yggdrasil.Mechanics.TownCrowd.CrowdAgent")!;
		var journey = RuntimeHelpers.GetUninitializedObject(type);
		void Set(string field, object value) => type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(journey, value);
		SetJourneyBody(journey, new NPC { width = 16, height = 48, position = new Vector2(1600, 976) });
		Set("recentPlaces", new Queue<string>());
		Set("departureRequested", true);
		Set("age", 12000);
		for (int i = 0; i < 3; i++)
		{
			new TownCrowdSystem().PreUpdateNPCs();
			type.GetMethod("ChooseNextActivity", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(journey, null);
			Assert.AreEqual("Walking", type.GetProperty("Stage")!.GetValue(journey)!.ToString());
		}
		Assert.AreEqual(3, ((CrowdRetirement)type.GetField("retirement", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(journey)!).DepartureFailures);
		Assert.IsTrue((bool)type.GetField("departureRequested", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(journey)!);
	}
}
