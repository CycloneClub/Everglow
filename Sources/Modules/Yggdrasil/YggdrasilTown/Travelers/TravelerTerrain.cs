using Everglow.Commons.CustomTiles;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Commons.Physics.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.CustomTiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

internal static class TravelerTerrain
{
	public static Vector2 Feet(Point cell) => new(cell.X * 16 + 8, cell.Y * 16 + 16);

	public static Point Cell(Vector2 feet) => new((int)MathF.Floor(feet.X / 16), (int)MathF.Floor((feet.Y - 1) / 16));

	public static bool IsClear(Point cell) => IsClear(cell, null);

	public static bool IsClear(Point cell, YggdrasilTown_NormalMetro boardingTrain)
	{
		if (!WorldGen.InWorld(cell.X, cell.Y, 5))
		{
			return false;
		}
		// Only the selected, stopped train can temporarily extend the walking surface.
		for (int y = cell.Y - 2; y <= cell.Y; y++)
		{
			var tile = Main.tile[cell.X, y];
			if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
			{
				return false;
			}
			if (Hazard(tile) || tile.LiquidAmount >= 128 || (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Lava))
			{
				return false;
			}
		}
		var box = new AABB(Feet(cell) - new Vector2(8, 48), new Vector2(16, 47));
		// A moving deck is still an obstacle, even when it cannot be a planned support.
		return !ColliderManager.Instance.OfType<BoxEntity>().Any(tile => tile.Active && tile.Intersect(box));
	}

	public static bool CanStand(Point cell) => CanStand(cell, null);

	public static bool CanStand(Point cell, YggdrasilTown_NormalMetro boardingTrain)
	{
		if (!IsClear(cell, boardingTrain))
		{
			return false;
		}
		var tile = Main.tile[cell.X, cell.Y + 1];
		if (Hazard(tile) || (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Lava))
		{
			return false;
		}
		// Grid nodes require an exact floor height; half blocks and slopes need a richer surface model.
		if (TravelerRules.SupportsGridLanding(tile.HasUnactuatedTile,
			TravelerRules.IsSolidFloor(Main.tileSolid[tile.TileType], Main.tileSolidTop[tile.TileType], tile.TileFrameY), tile.IsHalfBlock, (int)tile.Slope))
		{
			return true;
		}
		var feet = Feet(cell);
		return Surfaces(boardingTrain).Any(box => TravelerRules.SurfaceSupports(feet, 16, box.Position, box.Size,
			ReferenceEquals(box, boardingTrain)));
	}

	public static bool CanDropThrough(Point cell, YggdrasilTown_NormalMetro boardingTrain = null)
	{
		if (!CanStand(cell, boardingTrain))
		{
			return false;
		}
		var tile = Main.tile[cell.X, cell.Y + 1];
		var feet = Feet(cell);
		return tile.HasUnactuatedTile && Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0
			&& !Surfaces(boardingTrain).Any(box => feet.X + 8 > box.Box.Left && feet.X - 8 < box.Box.Right
				&& Math.Abs(box.Box.Top - feet.Y) <= 2);
	}

	public static bool TrainStopped(YggdrasilTown_NormalMetro train) => train?.Active == true
		&& train.State == (int)YggdrasilTown_NormalMetro.BehaviorType.Stop && train.Velocity.LengthSquared() <= 0.0025f;

	private static IEnumerable<BoxEntity> Surfaces(YggdrasilTown_NormalMetro boardingTrain) =>
		ColliderManager.Instance.OfType<BoxEntity>().Where(box => box.Active && box.Velocity == Vector2.Zero
			&& (box is not YggdrasilTown_NormalMetro || (ReferenceEquals(box, boardingTrain) && TrainStopped(boardingTrain))));

	private static bool Hazard(Tile tile) => tile.HasUnactuatedTile
		&& (TileID.Sets.TouchDamageImmediate[tile.TileType] > 0 || TileID.Sets.TouchDamageHot[tile.TileType]
			|| TileID.Sets.TouchDamageBleeding[tile.TileType]);

	public static List<Point> FindRoute(Point start, Point goal, int seed, YggdrasilTown_NormalMetro boardingTrain = null)
	{
		bool Clear(Point point) => point.X >= Math.Min(start.X, goal.X) - 32 && point.X <= Math.Max(start.X, goal.X) + 32
			&& point.Y >= Math.Min(start.Y, goal.Y) - 12 && point.Y <= Math.Max(start.Y, goal.Y) + 12
			&& IsClear(point, boardingTrain);
		return TravelerNavigation.FindPath(start, goal, point => CanStand(point, boardingTrain), Clear, 3000,
			point => CanDropThrough(point, boardingTrain), boardingTrain == null ? seed : 0);
	}

	public static bool Grounded(Entity body) => Math.Abs(body.velocity.Y) < 0.05f
		&& (Collision.TileCollision(body.position, new Vector2(0, 2), body.width, body.height).Y < 1.9f
			|| ColliderManager.Instance.OfType<BoxEntity>().Any(box => box.Active && body.Right.X > box.Box.Left
				&& body.Left.X < box.Box.Right && Math.Abs(body.Bottom.Y - box.Box.Top) <= 2));

	public static bool TryPlatformRecovery(Entity body, out float landingY)
	{
		landingY = 0;
		var origin = Cell(body.Bottom);
		if (Math.Abs(body.velocity.Y) >= 0.05f || CanStand(origin))
		{
			return false;
		}
		int left = (int)MathF.Floor(body.Left.X / 16);
		int right = (int)MathF.Floor((body.Right.X - 0.01f) / 16);
		if (!WorldGen.InWorld(left, origin.Y - 2, 5) || !WorldGen.InWorld(right, origin.Y + TravelerNavigation.MaxDropTiles + 1, 5))
		{
			return false;
		}
		bool slope = false;
		for (int x = left; x <= right; x++)
		{
			for (int y = origin.Y; y <= origin.Y + 1; y++)
			{
				var tile = Main.tile[x, y];
				slope |= tile.HasUnactuatedTile && Main.tileSolidTop[tile.TileType] && tile.Slope != SlopeType.Solid;
			}
		}
		if (!slope)
		{
			return false;
		}
		for (int y = origin.Y; y <= origin.Y + TravelerNavigation.MaxDropTiles; y++)
		{
			bool safeFloor = true;
			for (int x = left; x <= right; x++)
			{
				if (!IsClear(new Point(x, y)))
				{
					return false;
				}
				var floor = Main.tile[x, y + 1];
				safeFloor &= floor.HasUnactuatedTile
					&& TravelerRules.IsSolidFloor(Main.tileSolid[floor.TileType], Main.tileSolidTop[floor.TileType], floor.TileFrameY)
					&& floor.Slope == SlopeType.Solid && !floor.IsHalfBlock && !Hazard(floor) && floor.LiquidAmount == 0;
			}
			float targetY = (y + 1) * 16;
			if (safeFloor && targetY > body.Bottom.Y && targetY - body.Bottom.Y <= TravelerNavigation.MaxDropTiles * 16)
			{
				// Native slope collision can hold NPCs above a platform that grid navigation cannot use.
				// Fall-through ends before the next collision step reaches this first safe floor.
				landingY = targetY;
				return true;
			}
		}
		return false;
	}

	public static bool TryStandingRecovery(Entity body, out float targetX)
	{
		targetX = 0;
		var origin = Cell(body.Bottom);
		if (!Grounded(body) || CanStand(origin) || !IsClear(origin) || Math.Abs(Feet(origin).Y - body.Bottom.Y) > 2)
		{
			return false;
		}
		for (int direction = -1; direction <= 1; direction += 2)
		{
			var candidate = new Point(origin.X + direction, origin.Y);
			var feet = Feet(candidate);
			// Native collision can rest the edge of a body on the adjacent cell. Move back
			// over that support without crossing a gap or changing the standing height.
			if (feet.X + 8 > body.Left.X && feet.X - 8 < body.Right.X && CanStand(candidate))
			{
				targetX = feet.X;
				return true;
			}
		}
		return false;
	}

	public static bool OnTrain(Entity body, YggdrasilTown_NormalMetro train) => train?.Active == true
		&& body.Left.X >= train.Box.Left && body.Right.X <= train.Box.Right && Math.Abs(body.Bottom.Y - train.Box.Top) <= 2;

	public static void StepOntoSurface(Entity body, YggdrasilTown_NormalMetro boardingTrain)
	{
		foreach (var surface in Surfaces(boardingTrain))
		{
			if (!TravelerRules.TryStepHeight(body.Bottom, body.width, body.velocity.X, surface.Position, surface.Size, out float rise))
			{
				continue;
			}
			var raised = body.position - new Vector2(0, rise);
			var next = raised + new Vector2(body.velocity.X, 0);
			// Check both ends of the short step before raising the body, as vanilla StepUp does.
			if (!Collision.SolidCollision(raised, body.width, body.height - 1)
				&& !Collision.SolidCollision(next, body.width, body.height - 1))
			{
				body.position = raised;
				body.velocity.Y = 0;
			}
		}
	}

	public static bool TryGround(Vector2 near, out Point cell, int horizontal = 12, int vertical = 24,
		YggdrasilTown_NormalMetro boardingTrain = null, int upward = 4)
	{
		var origin = Cell(near);
		for (int distance = 0; distance <= horizontal; distance++)
		{
			for (int dy = -upward; dy <= vertical; dy++)
			{
				for (int sign = -1; sign <= 1; sign += 2)
				{
					cell = new Point(origin.X + distance * sign, origin.Y + dy);
					if (CanStand(cell, boardingTrain))
					{
						return true;
					}
				}
			}
		}
		cell = default;
		return false;
	}
}
