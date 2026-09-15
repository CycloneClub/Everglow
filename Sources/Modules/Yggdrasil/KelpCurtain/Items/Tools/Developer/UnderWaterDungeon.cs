using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.IRProbe;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.LightningMechanism;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.UnderwaterGuillotine;
using Everglow.Yggdrasil.KelpCurtain.Walls;
using static Everglow.Commons.Utilities.TileUtils;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Tools.Developer;

public class UnderWaterDungeon : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 30;
		Item.useTurn = true;
		Item.useAnimation = 4;
		Item.useTime = 4;
		Item.autoReuse = false;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public Point OldMousePos = default;

	public override void HoldItem(Player player) => base.HoldItem(player);

	public override bool CanUseItem(Player player)
	{
		Point point = Main.MouseWorld.ToTileCoordinates();
		GenerateUnderWaterDungeon(point.X, point.Y);
		return false;
	}

	public static void GenerateUnderWaterDungeon(int i, int j)
	{
		KillRectangleAreaOfTile(i - 800, j - 170, i + 80, j + 70);
		KillRectangleAreaOfWall(i - 800, j - 170, i + 80, j + 70);
		KillRectangleAreaOfLiquid(i - 800, j - 170, i + 80, j + 70);
		KillRectangleAreaOfWire(i - 800, j - 170, i + 80, j + 70);
		int cellHeight = 22;
		int cellWidth = 34;
		int cellDistance = 80;
		int tunnelHeight = 16;
		for (int xRoom = 0; xRoom < 1; xRoom++)
		{
			int centerX = i - xRoom * cellDistance;
			int centerY = j;
			BrickRoom(centerX, centerY, cellWidth, cellHeight);

			// Trigger of the trap #0
			Point irPos = new Point(i - xRoom * cellDistance - cellWidth + 9, j);
			Tile irProbeTile = SafeGetTile(irPos);
			irProbeTile.TileType = (ushort)ModContent.TileType<IRProbe_Normal>();
			irProbeTile.TileFrameX = 36;
			irProbeTile.HasTile = true;

			// Trap #0
			Point underwaterGuillotinePos = new Point(i - xRoom * cellDistance - 4, j - cellHeight + 9);
			UnderwaterGuillotine underwaterGuillotine = TileLoader.GetTile(ModContent.TileType<UnderwaterGuillotine>()) as UnderwaterGuillotine;
			if (underwaterGuillotine is not null)
			{
				underwaterGuillotine.PlaceOriginAtTopLeft(underwaterGuillotinePos.X, underwaterGuillotinePos.Y);
			}

			// Wire #0
			ConnectWire(underwaterGuillotinePos, irPos);

			// trigger #1
			irPos = new Point(i - xRoom * cellDistance - 12, j - cellHeight + 9);
			irProbeTile = SafeGetTile(irPos);
			irProbeTile.TileType = (ushort)ModContent.TileType<IRProbe_90_Degree_Scan>();
			irProbeTile.TileFrameX = 18;
			irProbeTile.HasTile = true;

			// Trap #1
			underwaterGuillotinePos = new Point(i - xRoom * cellDistance - 22, j - cellHeight + 9);
			if (underwaterGuillotine is not null)
			{
				underwaterGuillotine.PlaceOriginAtTopLeft(underwaterGuillotinePos.X, underwaterGuillotinePos.Y);
			}

			// Wire #1
			ConnectWire(underwaterGuillotinePos, irPos, false, true);

			// trigger #2
			irPos = new Point(i - xRoom * cellDistance + 12, j - cellHeight + 9);
			irProbeTile = SafeGetTile(irPos);
			irProbeTile.TileType = (ushort)ModContent.TileType<IRProbe_90_Degree_Scan_Reverse>();
			irProbeTile.TileFrameX = 18;
			irProbeTile.HasTile = true;

			// Trap #2
			underwaterGuillotinePos = new Point(i - xRoom * cellDistance + 15, j - cellHeight + 9);
			if (underwaterGuillotine is not null)
			{
				underwaterGuillotine.PlaceOriginAtTopLeft(underwaterGuillotinePos.X, underwaterGuillotinePos.Y);
			}

			// Wire #2
			ConnectWire(underwaterGuillotinePos, irPos, false, false, true);

			// Lightings
			for (int t = 0; t < 4; t++)
			{
				int moveX = 6;
				int moveY = 6;
				switch (t)
				{
					case 0:
						moveX = 6;
						moveY = 6;
						break;
					case 1:
						moveX = 20;
						moveY = 7;
						break;
					case 2:
						moveX = 11;
						moveY = 8;
						break;
					case 3:
						moveX = 7;
						moveY = 11;
						break;
				}
				int thick = 8;
				int lampY = centerY - cellHeight + thick + moveY;
				int lampX0 = centerX - cellWidth + thick + moveX;
				int lampX1 = centerX + cellWidth - thick - moveX;
				Tile lamp0 = SafeGetTile(lampX0, lampY);
				Tile lamp1 = SafeGetTile(lampX1, lampY);
				lamp0.TileType = (ushort)ModContent.TileType<NoctilucentFluoriteLump>();
				lamp0.HasTile = true;

				lamp1.TileType = (ushort)ModContent.TileType<NoctilucentFluoriteLump>();
				lamp1.HasTile = true;
			}
		}
		for (int xRoom = 0; xRoom < 1; xRoom++)
		{
			if (xRoom < 1)
			{
				int y = j + (cellHeight - tunnelHeight / 2) - 3;
				ConnectWaterErodedBrickTunnel(i - xRoom * cellDistance - (cellWidth - 7), y, i - xRoom * cellDistance - cellDistance + (cellWidth - 7), y, tunnelHeight);
				Tile lamp0 = SafeGetTile(i - xRoom * cellDistance - (cellWidth - 7) - 0, y);
				lamp0.TileType = (ushort)ModContent.TileType<NoctilucentFluoriteLump>();
				lamp0.HasTile = true;
			}
		}
		BrickRoom(i - 1 * cellDistance - 20, j - 90, cellWidth + 30, cellHeight);
		int checkXRoom2 = i - 1 * cellDistance - 20 - (cellWidth + 10);
		int checkYRoom2 = j - 95;
		for (int k = 0; k < 30; k++)
		{
			int checkX = checkXRoom2 + k * 15;
			int checkY = checkYRoom2 + (k % 2 == 0 ? 3 : 0);
			Tile lamp = SafeGetTile(checkX, checkY);
			if ((!lamp.HasTile || lamp.TileType != ModContent.TileType<WaterErodedGreenBrick>()) && lamp.wall == ModContent.WallType<WaterErodedGreenBrickWall_Fixed>())
			{
				lamp.TileType = (ushort)ModContent.TileType<NoctilucentFluoriteLump>();
				lamp.HasTile = true;
			}
			else
			{
				break;
			}
		}

		ConnectWaterErodedBrickTunnel_Serrated(new Point(i - 1 * cellDistance - 40, j - 64), new Point(i - 38, j + 9), 12, 8);
	}

	public static void ConnectWire(Point point0, Point point1, bool red = true, bool green = false, bool blue = false, bool yellow = false)
	{
		int distanceX = Math.Abs((point1 - point0).X);
		int distanceY = Math.Abs((point1 - point0).Y);
		Point check = point0;
		int dirX = 1;
		int dirY = 1;
		if (point1.X < point0.X)
		{
			dirX = -1;
		}
		if (point1.Y < point0.Y)
		{
			dirY = -1;
		}
		for (int y = 0; y < distanceY; y++)
		{
			Tile tile = SafeGetTile(check);
			tile.RedWire = red;
			tile.GreenWire = green;
			tile.BlueWire = blue;
			tile.YellowWire = yellow;
			check.Y += dirY;
		}
		for (int x = 0; x <= distanceX; x++)
		{
			Tile tile = SafeGetTile(check);
			tile.RedWire = red;
			tile.GreenWire = green;
			tile.BlueWire = blue;
			tile.YellowWire = yellow;
			check.X += dirX;
		}
	}

	public static void BrickRoom(int centerX, int centerY, int halfWidth, int halfHeight)
	{
		int thick = 8;

		// Main structure
		for (int x = centerX - halfWidth; x <= centerX + halfWidth; x++)
		{
			for (int y = centerY - halfHeight; y <= centerY + halfHeight; y++)
			{
				Tile tile = SafeGetTile(x, y);
				int boundValue = halfWidth - Math.Abs(x - centerX);
				int boundValue2 = halfHeight - Math.Abs(y - centerY);
				boundValue = Math.Min(boundValue, boundValue2);
				if (boundValue <= thick)
				{
					tile.TileType = (ushort)ModContent.TileType<WaterErodedGreenBrick>();
					tile.HasTile = true;
				}
				if (boundValue > thick)
				{
					tile.HasTile = false;
					if (boundValue > 1)
					{
						tile.WallType = (ushort)ModContent.WallType<WaterErodedGreenBrickWall_Fixed>();
						tile.liquid = (byte)LiquidID.Water;
						tile.LiquidAmount = 255;
					}
				}
			}
		}

		// Alga
		for (int algaX = 0; algaX < halfWidth * 2; algaX++)
		{
			int height = WorldGen.genRand.Next(-12, 18);
			if (height > 0)
			{
				Tile algaBottom = SafeGetTile(centerX - halfWidth + algaX, centerY + halfHeight - thick);
				if (algaBottom.HasTile)
				{
					for (int algaY = 0; algaY < height; algaY++)
					{
						Tile algaTile = SafeGetTile(centerX - halfWidth + algaX, centerY + halfHeight - thick - algaY - 1);
						if (algaTile.HasTile)
						{
							break;
						}
						else
						{
							algaTile.TileType = (ushort)ModContent.TileType<JadeLakeGreenAlgae>();
							algaTile.HasTile = true;
						}
					}
				}
			}
		}

		// Drain(bubbles for breathe)
		Point outlet = new Point(centerX - 2, centerY);
		PlaceFrameImportantTiles(outlet.X, outlet.Y, 4, 4, ModContent.TileType<DrainOutlet>());
	}

	/// <summary>
	/// Create a tunnel with water-erodeed brick side between 2 points.
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="width"></param>
	public static void ConnectWaterErodedBrickTunnel(Point p0, Point p1, float width)
	{
		ConnectWaterErodedBrickTunnel(p0.X, p0.Y, p1.X, p1.Y, width);
	}

	/// <summary>
	/// Create a tunnel with water-erodeed brick side between 2 points.
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="width"></param>
	public static void ConnectWaterErodedBrickTunnel(int x0, int y0, int x1, int y1, float width)
	{
		int sideThick = 6;
		int maxStep = (int)(new Vector2(x1, y1) - new Vector2(x0, y0)).Length();
		Vector2 dir = Vector2.Normalize(new Vector2(x1, y1) - new Vector2(x0, y0));
		Vector2 checkPoint = new Vector2(x0, y0);
		float halfWidth = width / 2f;
		for (int s = 0; s < maxStep; s++)
		{
			checkPoint += dir;
			for (int x = (int)(-halfWidth); x <= halfWidth; x++)
			{
				for (int y = (int)(-halfWidth); y <= halfWidth; y++)
				{
					Vector2 checkDir = new Vector2(x, y);
					bool shouldKill = false;
					if (checkDir.Length() < 3f)
					{
						shouldKill = true;
					}
					else
					{
						Vector2 normalCheckDir = checkDir.NormalizeSafe();
						if (MathF.Abs(Vector2.Dot(normalCheckDir, dir)) <= 0.15f)
						{
							shouldKill = true;
						}
					}
					if (shouldKill)
					{
						Tile tile = SafeGetTile((int)(x + checkPoint.X), (int)(y + checkPoint.Y));
						float distance = checkDir.Length();
						if (distance < halfWidth - (sideThick - 1))
						{
							tile.HasTile = false;
							tile.WallType = (ushort)ModContent.WallType<WaterErodedGreenBrickWall_Fixed>();
							tile.liquid = (byte)LiquidID.Water;
							tile.LiquidAmount = 255;
						}
						if (distance >= halfWidth - (sideThick - 1))
						{
							tile.HasTile = true;
							tile.TileType = (ushort)ModContent.TileType<WaterErodedGreenBrick>();
							if (distance < halfWidth)
							{
								tile.WallType = (ushort)ModContent.WallType<WaterErodedGreenBrickWall_Fixed>();
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Create a tunnel with water-erodeed brick side between 2 points.Traversing altitute by creating a zigzag path.
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="width"></param>
	public static void ConnectWaterErodedBrickTunnel_Serrated(int x0, int y0, int x1, int y1, float width, float sideThick)
	{
		ConnectWaterErodedBrickTunnel_Serrated(new Point(x0, y0), new Point(x1, y1), width, sideThick);
	}

	/// <summary>
	/// Create a tunnel with water-erodeed brick side between 2 points.Traversing altitute by creating a zigzag path.
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="width"></param>
	public static void ConnectWaterErodedBrickTunnel_Serrated(Point point0, Point point1, float width, float sideThick)
	{
		// Calculate the shape
		float centerThick = width - sideThick;
		int distanceY = Math.Abs((point1 - point0).Y);
		Point check = point0;
		int dirY = 1;
		if (point1.Y < point0.Y)
		{
			dirY = -1;
		}
		int jaggedX = 0;
		int jaggedDir = (int)((WorldGen.genRand.Next(2) - 0.5f) * 2);

		// Calclate the trail
		List<Point> trailPos = new List<Point>();
		List<int> inflectionPoint = new List<int>();
		List<int> lumpLampPoint = new List<int>();
		List<int> breathePoint = new List<int>();
		List<int> platformPoint = new List<int>();
		for (int y = 0; y < distanceY; y++)
		{
			trailPos.Add(check + new Point(jaggedX, 0));
			check.Y += dirY;
			jaggedX += jaggedDir * 3;
			if (Math.Abs(jaggedX) > 24 && jaggedX * jaggedDir > 0)
			{
				jaggedDir *= -1;
				inflectionPoint.Add(y);
				lumpLampPoint.Add(y);
				platformPoint.Add(y);
				breathePoint.Add(y - 8);
			}
		}
		check += new Point(jaggedX, 0);
		int dirX = 1;
		if (point1.X < check.X)
		{
			dirX = -1;
		}
		int distanceX = Math.Abs((point1 - check).X);
		for (int x = 0; x <= distanceX; x++)
		{
			trailPos.Add(check);
			check.X += dirX;
			if (x % 24 == 16)
			{
				lumpLampPoint.Add(trailPos.Count);
			}
			if (x == 3)
			{
				breathePoint.Add(trailPos.Count);
			}
		}

		// Build the Exterior
		for (int s = 0; s < trailPos.Count; s++)
		{
			PlaceSquareAreaOfBlock(trailPos[s], (int)width, ModContent.TileType<WaterErodedGreenBrick>());
			PlaceSquareAreaOfWall(trailPos[s], (int)width - 1, ModContent.WallType<WaterErodedGreenBrickWall_Fixed>());
		}

		// Build the inner Tunnel
		for (int s = 0; s < trailPos.Count; s++)
		{
			PlaceSquareAreaOfBlock(trailPos[s], (int)centerThick, -1);
			PlaceSquareAreaOfLiquid(trailPos[s], (int)width - 1, LiquidID.Water);
		}

		// Traps
		for (int s = 0; s < trailPos.Count; s++)
		{
			// Lightning Mechanism
			if (inflectionPoint.Contains(s))
			{
				Point point = trailPos[s];
				Tile checkLeft = SafeGetTile(point + new Point(-8, 3));
				Tile checkRight = SafeGetTile(point + new Point(8, 3));
				Point probePos = point + new Point(0, -7);
				if (checkLeft.HasTile)
				{
					PlaceFrameImportantTiles(point.X - 7, point.Y + 3, 5, 3, ModContent.TileType<UnderwaterLightningMechanism_H>(), 90);
					probePos += new Point(7, 0);
					ConnectWire(probePos, new Point(point.X - 3, point.Y + 4));
				}
				if (checkRight.HasTile)
				{
					PlaceFrameImportantTiles(point.X + 3, point.Y + 3, 5, 3, ModContent.TileType<UnderwaterLightningMechanism_H>());
					probePos += new Point(-7, 0);
					ConnectWire(probePos, new Point(point.X + 3, point.Y + 4));
				}
				Tile irProbe = SafeGetTile(probePos);
				Tile irProbeTop = SafeGetTile(probePos + new Point(0, -1));
				if (!irProbe.HasTile && irProbeTop.HasTile)
				{
					irProbe.TileType = (ushort)ModContent.TileType<IRProbe_90_Degree_Scan>();
					irProbe.HasTile = true;
					irProbe.TileFrameX = 18;
				}
			}

			// Lamps
			if (lumpLampPoint.Contains(s))
			{
				Point point = trailPos[s] + new Point(0, -1);
				Tile tile = SafeGetTile(point);
				if (!tile.HasTile)
				{
					tile.TileType = (ushort)ModContent.TileType<NoctilucentFluoriteLump>();
					tile.HasTile = true;
				}
			}

			// Drain ourlets
			if (breathePoint.Contains(s))
			{
				Point point = trailPos[s] + new Point(-2, 1);
				PlaceFrameImportantTiles(point.X, point.Y, 4, 4, ModContent.TileType<DrainOutlet>());
			}

			// Platforms
			if (platformPoint.Contains(s))
			{
				Point point = trailPos[s] + new Point(0, 1);
				Tile checkLeft = SafeGetTile(point + new Point(-8, 2));
				Tile checkRight = SafeGetTile(point + new Point(8, 2));
				if (checkLeft.HasTile)
				{
					int length = CheckSpaceLeft(point.X, point.Y);
					for (int x = 1; x <= 3; x++)
					{
						Point pointCheck = point + new Point(-length, 0) + new Point(x, 0);
						WorldGen.PlaceTile(pointCheck.X, pointCheck.Y, TileID.Platforms, false, true, -1, 9);
					}
					Point pointChest = point + new Point(-length, 0) + new Point(1, -1);

					// TODO:Enable this.
					// WorldGenMisc.PlaceChest(pointChest.X, pointChest.Y, ModContent.TileType<WaterErodedRustyCopperChest>(), new List<Item>(), 0);
					length = CheckSpaceRight(point.X, point.Y);
					for (int j = 0; j < 10; j++)
					{
						point = trailPos[s] + new Point(0, 1 + 2 * j);
						for (int x = 1; x <= 2; x++)
						{
							Point pointCheck = point + new Point(length, 0) + new Point(-x, 0);
							if (SafeGetTile(pointCheck).HasTile)
							{
								j = 20;
								break;
							}
							WorldGen.PlaceTile(pointCheck.X, pointCheck.Y, TileID.Platforms, false, false, -1, 9);
						}
					}
				}
				if (checkRight.HasTile)
				{
					int length = CheckSpaceRight(point.X, point.Y);
					for (int x = 1; x <= 3; x++)
					{
						Point pointCheck = point + new Point(length, 0) + new Point(-x, 0);
						WorldGen.PlaceTile(pointCheck.X, pointCheck.Y, TileID.Platforms, false, true, -1, 9);
					}
					Point pointChest = point + new Point(length, 0) + new Point(-2, -1);

					// TODO:Enable this.
					// WorldGenMisc.PlaceChest(pointChest.X, pointChest.Y, ModContent.TileType<WaterErodedRustyCopperChest>(), new List<Item>(), 0);
					length = CheckSpaceLeft(point.X, point.Y);
					for (int j = 0; j < 10; j++)
					{
						point = trailPos[s] + new Point(0, 1 + 2 * j);
						for (int x = 1; x <= 2; x++)
						{
							Point pointCheck = point + new Point(-length, 0) + new Point(x, 0);
							if (SafeGetTile(pointCheck).HasTile)
							{
								j = 20;
								break;
							}
							WorldGen.PlaceTile(pointCheck.X, pointCheck.Y, TileID.Platforms, false, true, -1, 9);
						}
					}
				}
			}
		}
		int overLength = 15;
		if (dirX < 0)
		{
			KillRectangleAreaOfTile(trailPos[^1].X - (int)width - overLength, trailPos[^1].Y - (int)centerThick, trailPos[^1].X, trailPos[^1].Y + (int)centerThick);
			PlaceRectangleAreaOfWall(trailPos[^1].X - (int)width - overLength, trailPos[^1].Y - (int)centerThick, trailPos[^1].X, trailPos[^1].Y + (int)centerThick, ModContent.WallType<WaterErodedGreenBrickWall_Fixed>());
			PlaceRectangleAreaOfLiquid(trailPos[^1].X - (int)width - overLength, trailPos[^1].Y - (int)centerThick, trailPos[^1].X, trailPos[^1].Y + (int)centerThick, LiquidID.Water);
		}
		else
		{
			KillRectangleAreaOfTile(trailPos[^1].X, trailPos[^1].Y - (int)centerThick, trailPos[^1].X + (int)width + overLength, trailPos[^1].Y + (int)centerThick);
			PlaceRectangleAreaOfWall(trailPos[^1].X, trailPos[^1].Y - (int)centerThick, trailPos[^1].X + (int)width + overLength, trailPos[^1].Y + (int)centerThick, ModContent.WallType<WaterErodedGreenBrickWall_Fixed>());
			PlaceRectangleAreaOfLiquid(trailPos[^1].X, trailPos[^1].Y - (int)centerThick, trailPos[^1].X + (int)width + overLength, trailPos[^1].Y + (int)centerThick, LiquidID.Water);
		}
		KillRectangleAreaOfTile(trailPos[0].X - (int)centerThick, trailPos[0].Y - (int)width, trailPos[0].X + (int)centerThick, trailPos[0].Y);
		PlaceRectangleAreaOfWall(trailPos[0].X - (int)centerThick, trailPos[0].Y - (int)width, trailPos[0].X + (int)centerThick, trailPos[0].Y, ModContent.WallType<WaterErodedGreenBrickWall_Fixed>());
		PlaceRectangleAreaOfLiquid(trailPos[0].X - (int)centerThick, trailPos[0].Y - (int)width, trailPos[0].X + (int)centerThick, trailPos[0].Y, LiquidID.Water);
	}

	public static void ClearRectangleAreaExclude(int x0, int y0, int x1, int y1, int excludeType)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				if (tile.TileType != excludeType)
				{
					tile.ClearEverything();
				}
			}
		}
	}

	public override bool? UseItem(Player player)
	{
		return base.UseItem(player);
	}
}
