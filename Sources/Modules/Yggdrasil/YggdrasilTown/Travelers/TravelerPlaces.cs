using Everglow.Yggdrasil.Common.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Kitchen.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.OriginPylon;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.Pylons;

namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

internal enum TravelerArrival
{
	Street,
	Building,
	Pylon,
	Train,
}

internal sealed record TravelerPlace(TravelerActivity Activity, Point Cell, string Key, Vector2 Focus,
	Point? TileOrigin = null, int TileType = 0, int NpcIndex = -1, int NpcType = 0)
{
	public int Capacity => Activity is TravelerActivity.BoardTrain or TravelerActivity.Scenery ? 3 : 2;
}

/// <summary>Explicit public entrances and dynamic, reachable street activities. No player room transitions.</summary>
internal sealed class TravelerPlaces
{
	private sealed record Landmark(Point Origin, int Type, int Width, int Height);

	private readonly List<Landmark> doors = [];
	private readonly List<Landmark> pylons = [];
	private readonly List<Landmark> stations = [];
	private readonly Dictionary<int, Point> doorSizes = [];
	private readonly HashSet<int> pylonTypes = [];
	private readonly HashSet<int> stationTypes = [];
	private int scanX = 10;
	private int scanY;
	private int cooldown;

	public string Status => $"{doors.Count} entrances, {pylons.Count} pylons, {stations.Count} stations";

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
		pylonTypes.Clear();
		pylonTypes.UnionWith([ModContent.TileType<OriginPylon>(), ModContent.TileType<TownGatePylon>(),
			ModContent.TileType<HolographicPylon>(), ModContent.TileType<UnregisteredPylon>(), ModContent.TileType<YggdrasilWorldPylon>()]);
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
			var tile = Main.tile[scanX, scanY];
			if (tile.HasUnactuatedTile && tile.TileFrameX == 0 && tile.TileFrameY == 0)
			{
				var point = new Point(scanX, scanY);
				if (doorSizes.TryGetValue(tile.TileType, out var size))
				{
					Add(doors, new Landmark(point, tile.TileType, size.X, size.Y));
				}
				if (pylonTypes.Contains(tile.TileType))
				{
					Add(pylons, new Landmark(point, tile.TileType, 1, 1));
				}
				if (stationTypes.Contains(tile.TileType))
				{
					Add(stations, new Landmark(point, tile.TileType, 1, 1));
				}
			}
			if (++scanX >= Main.maxTilesX - 10)
			{
				scanX = 10;
				scanY++;
			}
		}
	}

	private static void Add(List<Landmark> list, Landmark marker)
	{
		if (list.Count < 128 && !list.Contains(marker))
		{
			list.Add(marker);
		}
	}

	private static bool Valid(Landmark marker)
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

	private static bool SocialAvailable(NPC npc) => npc.active && npc.friendly
		&& npc.ModNPC is TownNPC_LiveInYggdrasil town && !town.ArenaFighting && !town.Talking && town.Peace()
		&& npc.velocity.LengthSquared() < 1;

	public bool Valid(TravelerPlace place)
	{
		if (!TravelerTerrain.CanStand(place.Cell))
		{
			return false;
		}
		if (place.NpcIndex >= 0)
		{
			var npc = Main.npc[place.NpcIndex];
			return npc.type == place.NpcType && SocialAvailable(npc) && Vector2.DistanceSquared(npc.Bottom, place.Focus) < 48 * 48
				&& Collision.CanHitLine(TravelerTerrain.Feet(place.Cell) - new Vector2(0, 24), 1, 1, npc.Center, 1, 1);
		}
		if (place.TileOrigin is Point origin)
		{
			if (place.Activity is TravelerActivity.Browse or TravelerActivity.EnterBuilding && !doorSizes.ContainsKey(place.TileType))
			{
				return false;
			}
			var size = doorSizes.GetValueOrDefault(place.TileType, new Point(1, 1));
			return Valid(new Landmark(origin, place.TileType, size.X, size.Y));
		}
		return place.Activity != TravelerActivity.LeaveStreet || !ViewBounds.Contains(TravelerTerrain.Feet(place.Cell).ToPoint());
	}

	public IEnumerable<TravelerPlace> Entrances(Vector2 near, bool pylon)
	{
		foreach (var marker in (pylon ? pylons : doors).Where(marker => Valid(marker)
			&& Vector2.DistanceSquared(marker.Origin.ToWorldCoordinates(), near) < 2400 * 2400))
		{
			Vector2 focus = marker.Origin.ToWorldCoordinates() + (pylon
				? (marker.Type == ModContent.TileType<OriginPylon>() ? new Vector2(64, 192) : new Vector2(16, 64))
				: new Vector2(marker.Width * 8 - 8, marker.Height * 16 - 8));
			if (TravelerTerrain.TryGround(focus, out var cell, pylon ? 5 : 2, pylon ? 48 : 2, upward: 3)
				&& (pylon || Math.Abs(TravelerTerrain.Feet(cell).Y - focus.Y) <= 24))
			{
				yield return new TravelerPlace(pylon ? TravelerActivity.LeavePylon : TravelerActivity.EnterBuilding,
					cell, $"tile:{marker.Origin}", focus, marker.Origin, marker.Type);
			}
		}
	}

	public bool TryStandingPlace(TravelerPlace place, Func<Point, bool> occupied, out TravelerPlace standingPlace)
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
		if (TravelerActivityRules.IsExit(place.Activity) && place.Activity != TravelerActivity.BoardTrain)
		{
			return false;
		}
		int outward = TravelerTerrain.Feet(place.Cell).X < place.Focus.X ? -1 : 1;
		for (int distance = 2; distance <= 4; distance += 2)
		{
			for (int side = -1; side <= 1; side += 2)
			{
				if (place.Activity == TravelerActivity.BoardTrain && side != outward)
				{
					continue;
				}
				var cell = new Point(place.Cell.X + distance * side, place.Cell.Y);
				var alternative = place with { Cell = cell };
				// Keep the group on one continuous floor; a free point beyond a wall or gap is a different place.
				bool connected = Enumerable.Range(1, distance).All(offset =>
					TravelerTerrain.CanStand(new Point(place.Cell.X + offset * side, place.Cell.Y)));
				if (connected && !occupied(cell) && Valid(alternative))
				{
					standingPlace = alternative;
					return true;
				}
			}
		}
		return false;
	}

	public IEnumerable<TravelerPlace> Candidates(Vector2 from, bool departing)
	{
		foreach (var entrance in Entrances(from, false))
		{
			yield return departing ? entrance : entrance with { Activity = TravelerActivity.Browse };
		}
		var plazaEntrances = Entrances(from, true).ToList();
		foreach (var pylon in plazaEntrances)
		{
			yield return departing ? pylon : pylon with { Activity = TravelerActivity.Scenery };
		}
		if (departing)
		{
			foreach (var station in stations.Where(Valid))
			{
				var focus = new Vector2((station.Origin.X + 2) * 16, (station.Origin.Y + 5) * 16);
				if (Vector2.DistanceSquared(from, focus) < 2800 * 2800 && TryStation(focus, from.X, out var cell))
				{
					yield return new TravelerPlace(TravelerActivity.BoardTrain, cell, $"station:{station.Origin}", focus, station.Origin, station.Type);
				}
			}
		}
		else
		{
			foreach (var npc in Main.npc.Where(npc => SocialAvailable(npc) && TownBounds.Contains(npc.Center.ToPoint())
				&& Vector2.DistanceSquared(npc.Bottom, from) < 1600 * 1600).Take(8))
			{
				var near = npc.Bottom + new Vector2(from.X < npc.Center.X ? -64 : 64, 0);
				if (TravelerTerrain.TryGround(near, out var cell, 2, 2, upward: 2))
				{
					yield return new TravelerPlace(TravelerActivity.Socialize, cell, $"npc:{npc.whoAmI}:{npc.type}", npc.Bottom, NpcIndex: npc.whoAmI, NpcType: npc.type);
				}
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
			if (TravelerTerrain.TryGround(near, out var cell, 4, 12)
				&& (TownBounds.Contains(TravelerTerrain.Feet(cell).ToPoint()) || (!departing && plazaEntrances.Any(plaza =>
					Math.Abs(TravelerTerrain.Feet(cell).Y - TravelerTerrain.Feet(plaza.Cell).Y) <= 96
					&& Math.Abs(TravelerTerrain.Feet(cell).X - TravelerTerrain.Feet(plaza.Cell).X) <= 640)))
				&& (!departing || !ViewBounds.Contains(TravelerTerrain.Feet(cell).ToPoint())))
			{
				yield return new TravelerPlace(departing ? TravelerActivity.LeaveStreet : TravelerActivity.Scenery,
					cell, $"street:{cell.X / 8}:{cell.Y / 4}", TravelerTerrain.Feet(cell));
			}
		}
	}

	public static bool TryStation(Vector2 station, float fromX, out Point cell)
	{
		float side = fromX < station.X ? -1 : 1;
		return TravelerTerrain.TryGround(station + new Vector2(side * 912, -16), out cell, 6, 0, upward: TravelerNavigation.MaxDropTiles)
			&& Math.Abs(TravelerTerrain.Feet(cell).X - station.X) >= 864;
	}
}
