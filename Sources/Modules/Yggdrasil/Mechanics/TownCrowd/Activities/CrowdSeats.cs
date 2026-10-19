using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>Single-place chairs only; a floor route remains authoritative even while the view is seated.</summary>
internal static class CrowdSeats
{
	public static bool TryFind(Point tilePoint, out Point floor)
	{
		floor = default;
		if (!WorldGen.InWorld(tilePoint.X, tilePoint.Y, 5))
		{
			return false;
		}
		var tile = Main.tile[tilePoint];
		if (!tile.HasUnactuatedTile || !TileID.Sets.CanBeSatOnForNPCs[tile.TileType])
		{
			return false;
		}
		var data = TileObjectData.GetTileData(tile);
		if (data == null || data.Width != 1 || data.Height != 2)
		{
			return false;
		}
		floor = new Point(tilePoint.X, tilePoint.Y + (tile.TileFrameY % data.CoordinateFullHeight == 0 ? 1 : 0));
		var upper = Main.tile[floor.X, floor.Y - 1];
		var lower = Main.tile[floor];
		return upper.HasUnactuatedTile && lower.HasUnactuatedTile && upper.TileType == tile.TileType && lower.TileType == tile.TileType
			&& upper.TileFrameX == lower.TileFrameX && lower.TileFrameY - upper.TileFrameY == data.CoordinateHeights[0] + data.CoordinatePadding
			&& CrowdTerrain.CanStand(floor);
	}

	public static bool Available(Point floor) => TryFind(floor, out var actual) && actual == floor
		&& !Main.player.Any(player => player.active && player.sitting.isSitting
			&& Math.Abs(player.Bottom.X - CrowdTerrain.Feet(floor).X) < 24 && Math.Abs(player.Bottom.Y - CrowdTerrain.Feet(floor).Y) < 40)
		&& !Main.npc.Any(npc => npc.active && npc.townNPC
			&& Math.Abs(npc.Bottom.X - CrowdTerrain.Feet(floor).X) < 24 && Math.Abs(npc.Bottom.Y - CrowdTerrain.Feet(floor).Y) < 40);

	private static TileRestingInfo SittingInfo(Entity body, Point floor)
	{
		var tile = Main.tile[floor];
		// Match PlayerSittingHelper's chair defaults; mod chairs may replace the anchor and visual offsets.
		var visual = new Vector2(0, tile.TileType == TileID.Chairs && tile.TileFrameY / 40 == 27 ? 4 : 0);
		var info = new TileRestingInfo(body, new Point(floor.X, floor.Y - 1), visual, tile.TileFrameX == 0 ? -1 : 1, 6, Vector2.Zero, default);
		TileLoader.ModifySittingTargetInfo(floor.X, floor.Y, tile.TileType, ref info);
		return info;
	}

	public static int Direction(Entity body, Point floor) => SittingInfo(body, floor).TargetDirection < 0 ? -1 : 1;

	public static Vector2 VisualOffset(Entity body, Point floor, out float seatYOffset)
	{
		var info = SittingInfo(body, floor);
		seatYOffset = -4 + (int)info.VisualOffset.Y;
		return info.AnchorTilePosition.ToWorldCoordinates(8, 16) + new Vector2(info.TargetDirection * info.DirectionOffset, 0)
			+ info.FinalOffset + info.VisualOffset * new Vector2(info.TargetDirection, 1) - body.Bottom;
	}

	public static IEnumerable<CrowdPlace> Nearby(Vector2 from)
	{
		var center = CrowdTerrain.Cell(from);
		var found = new HashSet<Point>();
		for (int x = center.X - 28; x <= center.X + 28; x++)
		{
			for (int y = center.Y - 8; y <= center.Y + 8; y++)
			{
				if (TryFind(new Point(x, y), out var floor) && found.Add(floor) && Available(floor))
				{
					yield return new CrowdPlace(CrowdActivity.Sit, floor, $"chair:{floor}", CrowdTerrain.Feet(floor));
					if (found.Count >= 6)
					{
						yield break;
					}
				}
			}
		}
	}
}
