using Everglow.Yggdrasil.YggdrasilTown.Travelers;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class TravelerNavigationTests
{
	[TestMethod]
	public void RoutePreferencesDoNotTurnFlatWalkingIntoRepeatedJumps()
	{
		for (int seed = 1; seed <= 16; seed++)
		{
			bool Clear(Point p) => p.X >= 0 && p.X <= 32 && p.Y >= 0 && p.Y <= 6;
			var path = TravelerNavigation.FindPath(new(0, 6), new(30, 6), p => Clear(p) && p.Y == 6, Clear, routeSeed: seed);
			Assert.AreEqual(31, path.Count, "A flat clear road should remain walking, even with personal route preferences.");
		}
	}

	[TestMethod]
	public void FindPath_PlatformCanDropToFirstSafeFloorWithoutWalkingToItsEdge()
	{
		bool Clear(Point p) => p.X >= 0 && p.X <= 12 && p.Y >= 0 && p.Y <= 6;
		bool Stand(Point p) => Clear(p) && (p.Y == 3 || p.Y == 6);
		var path = TravelerNavigation.FindPath(new(8, 3), new(8, 6), Stand, Clear, canDropThrough: p => p.Y == 3);
		CollectionAssert.AreEqual(new[] { new Point(8, 3), new Point(8, 6) }, path);
	}

	[TestMethod]
	public void FindPath_PlatformDropRequiresSafeFirstLandingAndBoundedHeight()
	{
		bool Clear(Point p) => p.X == 4 && p.Y >= 0 && p.Y <= 10;
		bool Stand(Point p) => Clear(p) && (p.Y == 3 || p.Y == 7);
		Assert.AreEqual(0, TravelerNavigation.FindPath(new(4, 3), new(4, 7), Stand, Clear, canDropThrough: _ => true).Count);
		Assert.AreEqual(0, TravelerNavigation.FindPath(new(4, 3), new(4, 6), p => p.Y is 3 or 6,
			p => Clear(p) && p.Y != 5, canDropThrough: _ => true).Count);
	}

	[TestMethod]
	public void FindPath_TravelersChooseDifferentSafePlatformExits()
	{
		bool Clear(Point p) => p.X >= 0 && p.X <= 20 && p.Y >= 0 && p.Y <= 6;
		bool Stand(Point p) => Clear(p) && (p.Y == 3 || p.Y == 6);
		var exits = new HashSet<int>();
		for (int seed = 1; seed <= 16; seed++)
		{
			var path = TravelerNavigation.FindPath(new(18, 3), new(2, 6), Stand, Clear,
				canDropThrough: p => p.Y == 3, routeSeed: seed);
			Assert.IsTrue(path.Count > 0);
			exits.Add(path.First(p => p.Y == 6).X);
			Assert.IsTrue(path.All(Stand));
		}
		Assert.IsTrue(exits.Count >= 3, "Different travelers should not always use the same platform exit.");
	}

	[TestMethod]
	public void FindPath_StackedPlatforms_WalksOffUpperEdgeBeforeDescending()
	{
		var start = new Point(4, 4);
		var goal = new Point(3, 6);
		bool Clear(Point point) => point.X >= 0 && point.X <= 8 && point.Y >= 0 && point.Y <= 6;
		bool Stand(Point point) => Clear(point) && (point.Y == 6 || (point.Y == 4 && point.X is 3 or 4));
		var path = TravelerNavigation.FindPath(start, goal, Stand, Clear);
		Assert.IsTrue(path.Count > 2, "Cannot walk directly through the upper platform to the floor beneath it.");
		Assert.AreEqual(goal, path[^1]);
		Assert.IsTrue(path.Any(point => point.Y == 6 && point.X is not (3 or 4)), "Descend outside the upper platform first.");
	}

	[TestMethod]
	public void FindPath_JumpCannotLandBelowAnInterceptingPlatform()
	{
		var start = new Point(1, 4);
		var goal = new Point(4, 6);
		bool Clear(Point point) => point.X >= 0 && point.X <= 8 && point.Y >= 0 && point.Y <= 6;
		bool Stand(Point point) => Clear(point) && (point == start || point == goal || point == new Point(4, 4));
		var path = TravelerNavigation.FindPath(start, goal, Stand, Clear);
		Assert.AreEqual(0, path.Count, "The descending body hits the upper platform before the requested landing.");
	}

	[TestMethod]
	public void FindPath_FlatGround_IncludesBothEndpoints()
	{
		var path = Find(new(1, 9), new(8, 9), (x, y) => y >= 10);
		Assert.AreEqual(new Point(1, 9), path[0]);
		Assert.AreEqual(new Point(8, 9), path[^1]);
		Assert.IsTrue(path.All(p => p.Y == 9));
	}

	[TestMethod]
	public void FindPath_UnsupportedStart_ReturnsEmpty()
	{
		Assert.AreEqual(0, Find(new(1, 8), new(8, 9), (x, y) => y >= 10).Count);
	}

	[TestMethod]
	public void FindPath_UnsupportedGoal_ReturnsEmpty()
	{
		Assert.AreEqual(0, Find(new(1, 9), new(8, 8), (x, y) => y >= 10).Count);
	}

	[TestMethod]
	public void FindPath_SameSupportedPoint_ReturnsOnePoint()
	{
		CollectionAssert.AreEqual(new[] { new Point(1, 9) }, Find(new(1, 9), new(1, 9), (x, y) => y >= 10));
	}

	[TestMethod]
	public void FindPath_FullHeightWall_ReturnsEmpty()
	{
		Assert.AreEqual(0, Find(new(1, 9), new(8, 9), (x, y) => y >= 10 || x == 4).Count);
	}

	[TestMethod]
	public void FindPath_TwoTileGap_JumpsToSupportedLanding()
	{
		var path = Find(new(1, 9), new(4, 9), (x, y) => y >= 10 && (x <= 1 || x >= 4));
		CollectionAssert.AreEqual(new[] { new Point(1, 9), new Point(4, 9) }, path);
	}

	[TestMethod]
	public void FindPath_FourTileHorizontalJump_ReturnsEmpty()
	{
		Assert.AreEqual(0, Find(new(1, 9), new(5, 9), (x, y) => y >= 10 && (x <= 1 || x >= 5)).Count);
	}

	[TestMethod]
	public void FindPath_OneTileStep_ClimbsOntoIt()
	{
		var path = Find(new(1, 9), new(5, 8), (x, y) => y >= (x >= 3 ? 9 : 10));
		Assert.AreEqual(new Point(5, 8), path[^1]);
	}

	[TestMethod]
	public void FindPath_ThreeTileRise_CanJumpOntoIt()
	{
		var path = Find(new(1, 9), new(4, 6), (x, y) => (x <= 1 && y >= 10) || (x >= 4 && y >= 7));
		Assert.AreEqual(new Point(4, 6), path[^1]);
	}

	[TestMethod]
	public void FindPath_FourTileRise_ReturnsEmpty()
	{
		Assert.AreEqual(0, Find(new(1, 9), new(4, 5), (x, y) => (x <= 1 && y >= 10) || (x >= 4 && y >= 6)).Count);
	}

	[TestMethod]
	public void FindPath_LowCeilingOverGap_BlocksJump()
	{
		Assert.AreEqual(0, Find(new(1, 9), new(4, 9), (x, y) => y == 6 || (y >= 10 && (x <= 1 || x >= 4))).Count);
	}

	[TestMethod]
	public void FindPath_LowCeilingOverFlatGround_AllowsWalking()
	{
		var path = Find(new(1, 9), new(4, 9), (x, y) => y == 6 || y >= 10);
		Assert.AreEqual(new Point(4, 9), path[^1]);
	}

	[TestMethod]
	public void FindPath_LowCeilingTouchedByActualJump_RejectsRoute()
	{
		// The real -6.1 velocity / 0.3 gravity trajectory rises almost four
		// cells; checking an idealized three-cell arch misses this ceiling.
		Assert.AreEqual(0, Find(new(1, 9), new(4, 9), (x, y) => y == 3 || (y >= 10 && (x <= 1 || x >= 4))).Count);
	}

	[TestMethod]
	public void FindPath_SolidTileInsideJumpArc_BlocksJump()
	{
		var start = new Point(1, 9);
		var goal = new Point(4, 9);
		// Only the two ledges support travelers; an obstacle within the body
		// sweep must reject the jump even though both endpoints are clear.
		var path = TravelerNavigation.FindPath(start, goal, p => p == start || p == goal,
			p => !(p.X == 2 && p.Y >= 7 && p.Y <= 9));
		Assert.AreEqual(0, path.Count);
	}

	[TestMethod]
	public void FindPath_ThreeTileDrop_CanReachGround()
	{
		var path = Find(new(1, 9), new(2, 12), (x, y) => y >= (x <= 1 ? 10 : 13));
		Assert.AreEqual(new Point(2, 12), path[^1]);
	}

	[TestMethod]
	public void FindPath_FourTileDrop_ReturnsEmpty()
	{
		Assert.AreEqual(0, Find(new(1, 9), new(2, 13), (x, y) => y >= (x <= 1 ? 10 : 14)).Count);
	}

	[TestMethod]
	public void FindPath_ExhaustedBudget_ReturnsEmpty()
	{
		Assert.AreEqual(0, Find(new(1, 9), new(8, 9), (x, y) => y >= 10, 1).Count);
	}

	[TestMethod]
	public void FindPath_ZeroBudget_DoesNotQueryWorld()
	{
		var path = TravelerNavigation.FindPath(new(1, 9), new(1, 9), _ => throw new AssertFailedException("No search budget."), _ => throw new AssertFailedException("No search budget."), 0);
		Assert.AreEqual(0, path.Count);
	}

	private static List<Point> Find(Point start, Point goal, Func<int, int, bool> solid, int maxVisited = 6000)
	{
		bool Clear(Point p) => p.X >= 0 && p.X < 12 && p.Y >= 2 && p.Y < 16 && !solid(p.X, p.Y) && !solid(p.X, p.Y - 1) && !solid(p.X, p.Y - 2);
		bool Stand(Point p) => Clear(p) && solid(p.X, p.Y + 1);
		return TravelerNavigation.FindPath(start, goal, Stand, Clear, maxVisited);
	}
}
