using Everglow.Yggdrasil.Mechanics.TownCrowd;
using Everglow.Commons.CustomTiles.Core;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class CrowdRulesTests
{
	[TestMethod]
	public void SolidTopFurnitureOnlySupportsItsTopFrame()
	{
		Assert.IsTrue(CrowdRules.IsSolidFloor(false, true, 0));
		Assert.IsFalse(CrowdRules.IsSolidFloor(false, true, 18));
		Assert.IsFalse(CrowdRules.IsSolidFloor(true, true, 36));
		Assert.IsTrue(CrowdRules.IsSolidFloor(true, false, 18));
		Assert.IsFalse(CrowdRules.IsSolidFloor(false, false, 0));
	}

	[TestMethod]
	[DataRow(-0.5f)]
	[DataRow(0.5f)]
	public void FractionalTrainStopHasWalkableBoardingRouteUnderLowCeiling(float offset)
	{
		var train = new BoxEntity { Position = new Vector2(32 + offset, 48), Size = new Vector2(128, 16) };
		Vector2 Feet(Point p) => new(p.X * 16 + 8, p.Y * 16 + 16);
		bool Clear(Point p)
		{
			var feet = Feet(p);
			return p.X >= 0 && p.X <= 12 && feet.Y >= 48 && feet.Y <= 64
				&& !(feet.Y > train.Position.Y && feet.X + 8 > train.Box.Left && feet.X - 8 < train.Box.Right);
		}
		bool Stand(Point p) => Clear(p) && (Feet(p).Y == 64 || CrowdRules.SurfaceSupports(Feet(p), 16, train.Position, train.Size, true));
		foreach (int startX in new[] { 0, 12 })
		{
			var path = CrowdNavigation.FindPath(new(startX, 3), new(4, 2), Stand, Clear);
			Assert.IsTrue(path.Count > 0, "A fractionally aligned 16px train step must not require a high jump.");
			for (int i = 1; i < path.Count; i++)
			{
				Assert.IsTrue(CrowdNavigation.CanWalkBetween(path[i - 1], path[i], Clear, Stand));
			}
		}
	}

	[TestMethod]
	[DataRow(512f)]
	[DataRow(331200f)]
	public void CrowdStepsOntoCustomTrainFloorAndReachesDoor(float floorY)
	{
		var train = new BoxEntity { Position = new Vector2(32, floorY - 16), Size = new Vector2(128, 16) };
		var body = new BoxImpl { Position = new Vector2(0, floorY - 48), Size = new Vector2(16, 48), Gravity = 1 };
		var door = new Vector2(72, train.Position.Y);
		for (int tick = 0; tick < 100; tick++)
		{
			var feet = body.Position + new Vector2(8, 48);
			body.Velocity = new Vector2(Math.Clamp(door.X - feet.X, -1.5f, 1.5f), 0.3f);
			if (CrowdRules.TryStepHeight(feet, 16, body.Velocity.X, train.Position, train.Size, out float rise))
			{
				body.Position -= new Vector2(0, rise);
			}
			Vector2 stride = body.Velocity;
			stride.Y = Math.Min(stride.Y, floorY - body.Position.Y - 48);
			if (train.Collision(body, stride, out var collision))
			{
				stride = collision.Stride;
			}
			body.Position += stride;
		}
		Assert.IsTrue(CrowdRules.CanBoard(true, true, Vector2.Zero, 100, 600,
			body.Position + new Vector2(8, 48), door), "The traveler must physically reach the deck, not stop against its side.");
		Assert.AreEqual(train.Position.Y, body.Position.Y + 48, 0.1f);
	}

	[TestMethod]
	public void CustomFloorStepRejectsHighWallsAndDistantSurfaces()
	{
		Assert.IsFalse(CrowdRules.TryStepHeight(new Vector2(24, 64), 16, 1.5f, new Vector2(32, 32), new Vector2(128, 16), out _));
		Assert.IsFalse(CrowdRules.TryStepHeight(new Vector2(8, 64), 16, 1.5f, new Vector2(32, 48), new Vector2(128, 16), out _));
	}

	[TestMethod]
	public void GridLandingRejectsHalfBlocksAndSlopesWithDifferentSurfaceHeight()
	{
		Assert.IsTrue(CrowdRules.SupportsGridLanding(true, true, false, 0));
		Assert.IsFalse(CrowdRules.SupportsGridLanding(true, true, true, 0));
		Assert.IsFalse(CrowdRules.SupportsGridLanding(true, true, false, 1));
		Assert.IsFalse(CrowdRules.SupportsGridLanding(false, true, false, 0));
		Assert.IsFalse(CrowdRules.SupportsGridLanding(true, false, false, 0));
	}

	[TestMethod]
	public void WaypointsRequireLandingAtThePlannedTakeoffPosition()
	{
		Assert.IsTrue(CrowdRules.ReachedWaypoint(Vector2.Zero, Vector2.Zero, true));
		Assert.IsFalse(CrowdRules.ReachedWaypoint(new Vector2(9, 0), Vector2.Zero, true));
		Assert.IsFalse(CrowdRules.ReachedWaypoint(new Vector2(0, 9), Vector2.Zero, true));
		Assert.IsFalse(CrowdRules.ReachedWaypoint(Vector2.Zero, Vector2.Zero, false));
	}

	[TestMethod]
	public void BoardingRequiresStoppedActiveTrainAndNearbyDoor()
	{
		Assert.IsTrue(CrowdRules.CanBoard(true, true, Vector2.Zero, 60, 600, new Vector2(12, 4), Vector2.Zero));
		Assert.IsFalse(CrowdRules.CanBoard(false, true, Vector2.Zero, 60, 600, Vector2.Zero, Vector2.Zero));
		Assert.IsFalse(CrowdRules.CanBoard(true, false, Vector2.Zero, 60, 600, Vector2.Zero, Vector2.Zero));
		Assert.IsFalse(CrowdRules.CanBoard(true, true, Vector2.UnitX, 60, 600, Vector2.Zero, Vector2.Zero));
		Assert.IsFalse(CrowdRules.CanBoard(true, true, Vector2.Zero, 60, 600, new Vector2(100, 0), Vector2.Zero));
		Assert.IsFalse(CrowdRules.CanBoard(true, true, Vector2.Zero, 60, 600, new Vector2(0, 80), Vector2.Zero));
	}

	[TestMethod]
	public void BoardingWindowExcludesArrivalAndDeparture()
	{
		Assert.IsFalse(CrowdRules.CanBoard(true, true, Vector2.Zero, 0, 600, Vector2.Zero, Vector2.Zero));
		Assert.IsTrue(CrowdRules.CanBoard(true, true, Vector2.Zero, 30, 600, Vector2.Zero, Vector2.Zero));
		Assert.IsFalse(CrowdRules.CanBoard(true, true, Vector2.Zero, 510, 600, Vector2.Zero, Vector2.Zero));
	}

	[TestMethod]
	public void BoardingRejectsNonFinitePositionOrSpeed()
	{
		Assert.IsFalse(CrowdRules.CanBoard(true, true, new Vector2(float.NaN, 0), 60, 600, Vector2.Zero, Vector2.Zero));
		Assert.IsFalse(CrowdRules.CanBoard(true, true, Vector2.Zero, 60, 600, new Vector2(float.NaN, 0), Vector2.Zero));
	}

	[TestMethod]
	public void ExperimentsRequireSinglePlayerAndYggdrasil()
	{
		Assert.IsTrue(CrowdRules.CanRun(0, true));
		Assert.IsFalse(CrowdRules.CanRun(1, true));
		Assert.IsFalse(CrowdRules.CanRun(2, true));
		Assert.IsFalse(CrowdRules.CanRun(0, false));
	}
}
