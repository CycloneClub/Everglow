using Everglow.Yggdrasil.Common.Tiles;
using Everglow.Yggdrasil.YggdrasilTown;
using Everglow.Yggdrasil.YggdrasilTown.Kitchen.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.OriginPylon;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

internal enum CrowdArrival
{
	Street,
	Building,
	Pylon,
	Train,
	Existing,
}

internal sealed record CrowdPlace(CrowdActivity Activity, Point Cell, string Key, Vector2 Focus,
	Point? TileOrigin = null, int TileType = 0)
{
	public int Capacity => Activity is CrowdActivity.BoardTrain or CrowdActivity.Scenery ? 3
		: Activity == CrowdActivity.Sit ? 1 : 2;
}

/// <summary>Explicit public entrances and dynamic, reachable street activities. No player room transitions.</summary>
internal sealed class CrowdPlaces
{
	private sealed record CrowdLandmark(Point Origin, int Type, int Width, int Height);

	private readonly List<CrowdLandmark> doors = [];
	private readonly List<CrowdLandmark> pylons = [];
	private readonly List<CrowdLandmark> stations = [];
	private readonly Dictionary<int, Point> doorSizes = [];
	private readonly HashSet<int> stationTypes = [];
	private int scanX = 10;
	private int scanY;
	private int cooldown;
	private int guildType = -1;

	public static Rectangle TownBounds
	{
		get
		{
			var town = YggdrasilTownCentralSystem.TownArea;
			return new Rectangle(town.X * 16, town.Y * 16, town.Width * 16, town.Height * 16);
		}
	}

	public static Rectangle ViewBounds
	{
		get
		{
			var view = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
			view.Inflate(160, 160);
			return view;
		}
	}

	public void Initialize()
	{
		doors.Clear();
		pylons.Clear();
		stations.Clear();
		doorSizes.Clear();
		doorSizes.Add(ModContent.TileType<YggdrasilGravelStoreGate>(), new Point(5, 5));
		doorSizes.Add(ModContent.TileType<CanteenDoor>(), new Point(5, 4));
		guildType = ModContent.TileType<MarbleGate>();
		doorSizes.Add(guildType, new Point(1, 1));
		stationTypes.Clear();
		stationTypes.UnionWith([ModContent.TileType<MetroStationSign_PylonSquare>(), ModContent.TileType<MetroStationSign_YggdrasilTown>()]);
		scanX = 10;
		scanY = Math.Max(10, Main.maxTilesY - 2000);
		cooldown = 0;
	}

	public void Update()
	{
		if (cooldown-- > 0)
		{
			return;
		}
		for (int examined = 0; examined < 8192; examined++)
		{
			if (scanY >= Main.maxTilesY - 10)
			{
				scanY = Math.Max(10, Main.maxTilesY - 2000);
				scanX = 10;
				cooldown = 1800;
				doors.RemoveAll(marker => !Valid(marker));
				pylons.RemoveAll(marker => !Valid(marker));
				stations.RemoveAll(marker => !Valid(marker));
				break;
			}
			Inspect(scanX, scanY);
			if (++scanX >= Main.maxTilesX - 10)
			{
				scanX = 10;
				scanY++;
			}
		}
	}

	public void DiscoverNear(Rectangle view)
	{
		// Initial local life cannot wait for the background sweep to reach a newly visited plaza.
		// At most 128 × 96 tiles, once per new camera region, independent of world size.
		int centerX = view.Center.X / 16;
		int centerY = view.Center.Y / 16;
		for (int x = Math.Max(5, centerX - 64); x < Math.Min(Main.maxTilesX - 5, centerX + 64); x++)
		{
			for (int y = Math.Max(5, centerY - 48); y < Math.Min(Main.maxTilesY - 5, centerY + 48); y++)
			{
				Inspect(x, y);
			}
		}
	}

	private void Inspect(int x, int y)
	{
		var tile = Main.tile[x, y];
		if (!tile.HasUnactuatedTile || tile.TileFrameX != 0 || tile.TileFrameY != 0)
		{
			return;
		}
		var point = new Point(x, y);
		if (doorSizes.TryGetValue(tile.TileType, out var size))
		{
			Add(doors, new CrowdLandmark(point, tile.TileType, size.X, size.Y));
		}
		if (tile.TileType == ModContent.TileType<OriginPylon>())
		{
			Add(pylons, new CrowdLandmark(point, tile.TileType, 8, 12));
		}
		if (stationTypes.Contains(tile.TileType))
		{
			Add(stations, new CrowdLandmark(point, tile.TileType, 1, 1));
		}
	}

	private static void Add(List<CrowdLandmark> list, CrowdLandmark marker)
	{
		if (list.Count < 128 && !list.Contains(marker))
		{
			list.Add(marker);
		}
	}

	private static bool Valid(CrowdLandmark marker)
	{
		for (int x = 0; x < marker.Width; x++)
		{
			for (int y = 0; y < marker.Height; y++)
			{
				if (!WorldGen.InWorld(marker.Origin.X + x, marker.Origin.Y + y, 5))
				{
					return false;
				}
				var tile = Main.tile[marker.Origin.X + x, marker.Origin.Y + y];
				if (!tile.HasUnactuatedTile || tile.TileType != marker.Type || tile.TileFrameX != x * 18 || tile.TileFrameY != y * 18)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool Valid(CrowdPlace place)
	{
		if (!CrowdTerrain.CanStand(place.Cell))
		{
			return false;
		}
		if (place.Activity == CrowdActivity.Sit)
		{
			return CrowdSeats.Available(place.Cell);
		}
		if (place.TileOrigin is Point origin)
		{
			if (place.Activity is CrowdActivity.Browse or CrowdActivity.EnterBuilding or CrowdActivity.EnterGuild && !doorSizes.ContainsKey(place.TileType))
			{
				return false;
			}
			if ((place.Activity == CrowdActivity.EnterGuild && place.TileType != guildType)
				|| (place.TileType == guildType && !CrowdLifeRules.InsideGuildOpening(origin, CrowdTerrain.Feet(place.Cell))))
			{
				return false;
			}
			var size = doorSizes.GetValueOrDefault(place.TileType, new Point(1, 1));
			return Valid(new CrowdLandmark(origin, place.TileType, size.X, size.Y));
		}
		return place.Activity != CrowdActivity.LeaveStreet || !ViewBounds.Contains(CrowdTerrain.Feet(place.Cell).ToPoint());
	}

	public IEnumerable<CrowdPlace> Entrances(Vector2 near, bool pylon)
	{
		foreach (var marker in (pylon ? pylons : doors).Where(marker => (!pylon || marker.Type == ModContent.TileType<OriginPylon>()) && Valid(marker)
			&& Vector2.DistanceSquared(marker.Origin.ToWorldCoordinates(), near) < 2400 * 2400))
		{
			bool guild = marker.Type == guildType;
			Vector2 focus = guild ? new Vector2(marker.Origin.X * 16 + 56, marker.Origin.Y * 16 + 128)
				: marker.Origin.ToWorldCoordinates() + (pylon
				? new Vector2(64, 192)
				: new Vector2(marker.Width * 8 - 8, marker.Height * 16 - 8));
			if (CrowdTerrain.TryGround(focus, out var cell, pylon ? 5 : 2, pylon ? 48 : 2, upward: 3)
				&& (pylon || Math.Abs(CrowdTerrain.Feet(cell).Y - focus.Y) <= 24)
				&& (!guild || CrowdLifeRules.InsideGuildOpening(marker.Origin, CrowdTerrain.Feet(cell))))
			{
				yield return new CrowdPlace(pylon ? CrowdActivity.LeavePylon : guild ? CrowdActivity.EnterGuild : CrowdActivity.EnterBuilding,
					cell, $"tile:{marker.Origin}", focus, marker.Origin, marker.Type);
			}
		}
	}

	public bool CanSeed(Vector2 feet) => TownBounds.Contains(feet.ToPoint()) || Entrances(feet, true).Any(pylon =>
		Math.Abs(feet.X - CrowdTerrain.Feet(pylon.Cell).X) <= 640 && Math.Abs(feet.Y - CrowdTerrain.Feet(pylon.Cell).Y) <= 96);

	public bool TryStandingPlace(CrowdPlace place, Func<Point, bool> occupied, out CrowdPlace standingPlace)
	{
		standingPlace = place;
		if (!Valid(place))
		{
			return false;
		}
		if (!occupied(place.Cell))
		{
			return true;
		}
		// Entrance animations must remain at their actual entrance, not fade through a nearby wall.
		if (place.Activity == CrowdActivity.Sit || (CrowdActivityRules.IsExit(place.Activity) && place.Activity != CrowdActivity.BoardTrain))
		{
			return false;
		}
		int outward = CrowdTerrain.Feet(place.Cell).X < place.Focus.X ? -1 : 1;
		for (int distance = 2; distance <= 4; distance += 2)
		{
			for (int side = -1; side <= 1; side += 2)
			{
				if (place.Activity == CrowdActivity.BoardTrain && side != outward)
				{
					continue;
				}
				var cell = new Point(place.Cell.X + distance * side, place.Cell.Y);
				var alternative = place with { Cell = cell };
				// Keep the group on one continuous floor; a free point beyond a wall or gap is a different place.
				bool connected = Enumerable.Range(1, distance).All(offset =>
					CrowdTerrain.CanStand(new Point(place.Cell.X + offset * side, place.Cell.Y)));
				if (connected && !occupied(cell) && Valid(alternative))
				{
					standingPlace = alternative;
					return true;
				}
			}
		}
		return false;
	}

	public IEnumerable<CrowdPlace> Candidates(Vector2 from, bool departing)
	{
		foreach (var entrance in Entrances(from, false))
		{
			yield return departing ? entrance : entrance with { Activity = CrowdActivity.Browse };
		}
		var plazaEntrances = Entrances(from, true).ToList();
		foreach (var pylon in plazaEntrances)
		{
			yield return departing ? pylon : pylon with { Activity = CrowdActivity.Scenery };
		}
		if (departing)
		{
			foreach (var station in stations.Where(Valid))
			{
				var focus = new Vector2((station.Origin.X + 2) * 16, (station.Origin.Y + 5) * 16);
				if (Vector2.DistanceSquared(from, focus) < 2800 * 2800 && TryStation(focus, from.X, out var cell))
				{
					yield return new CrowdPlace(CrowdActivity.BoardTrain, cell, $"station:{station.Origin}", focus, station.Origin, station.Type);
				}
			}
		}
		else
		{
			foreach (var seat in CrowdSeats.Nearby(from))
			{
				yield return seat;
			}
		}
		for (int i = 0; i < 8; i++)
		{
			Vector2 near = from + new Vector2(Main.rand.Next(8, 42) * 16 * (Main.rand.NextBool() ? 1 : -1), Main.rand.Next(-8, 9) * 16);
			if (departing)
			{
				var view = ViewBounds;
				near.X = Main.rand.NextBool() ? view.Left - Main.rand.Next(48, 192) : view.Right + Main.rand.Next(48, 192);
			}
			if (CrowdTerrain.TryGround(near, out var cell, 4, 12)
				&& (TownBounds.Contains(CrowdTerrain.Feet(cell).ToPoint()) || (!departing && plazaEntrances.Any(plaza =>
					Math.Abs(CrowdTerrain.Feet(cell).Y - CrowdTerrain.Feet(plaza.Cell).Y) <= 96
					&& Math.Abs(CrowdTerrain.Feet(cell).X - CrowdTerrain.Feet(plaza.Cell).X) <= 640)))
				&& (!departing || !ViewBounds.Contains(CrowdTerrain.Feet(cell).ToPoint())))
			{
				yield return new CrowdPlace(departing ? CrowdActivity.LeaveStreet : CrowdActivity.Scenery,
					cell, $"street:{cell.X / 8}:{cell.Y / 4}", CrowdTerrain.Feet(cell));
			}
		}
	}

	public static bool TryStation(Vector2 station, float fromX, out Point cell)
	{
		float side = fromX < station.X ? -1 : 1;
		return CrowdTerrain.TryGround(station + new Vector2(side * 912, -16), out cell, 6, 0, upward: CrowdNavigation.MaxDropTiles)
			&& Math.Abs(CrowdTerrain.Feet(cell).X - station.X) >= 864;
	}
}
