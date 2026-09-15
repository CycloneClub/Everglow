using Everglow.Yggdrasil.Common.Tiles;
using Everglow.Yggdrasil.Common.Walls;
using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Yggdrasil.KelpCurtain.Biomes;
using Everglow.Yggdrasil.KelpCurtain.Items.Accessories;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
using Everglow.Yggdrasil.KelpCurtain.Tiles;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.IRProbe;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.LightningMechanism;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.UnderwaterGuillotine;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.WaterDeliveryHoles;
using Everglow.Yggdrasil.KelpCurtain.Tiles.GeyserAirBuds;
using Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;
using Everglow.Yggdrasil.KelpCurtain.Walls;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Terraria.ObjectData;
using static Everglow.Commons.Utilities.TileUtils;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.WorldGeneration;

public class KelpCurtainGeneration
{
	/// <summary>
	/// Tile coord.
	/// </summary>
	public static int MazeUnderLake_TopY = -1;

	public static int MazeUnderLake_YggdrasilBlackChestShrineCount = 0;

	public static List<int> WaterDeliveryHoleTiles = new List<int>();

	public static List<int> MazeUnderLake_YggdrasilBlackRockChestContents = new List<int>();

	public static int UnderwaterTreasury_Bottom_Room_center_Y;

	public static Vector2 VampireMatCaveCenter;

	public static void BuildKelpCurtain()
	{
		Initialize();
		Main.statusText = "Kelp Curtain Bark Cliff...";

		// UnforcablePlaceAreaOfTile(20, 9600, 155, 10650, ModContent.TileType<DragonScaleWood>());
		//

		// PlaceRectangleAreaOfWall(20, 9600, 155, 10650, ModContent.WallType<DragonScaleWoodWall>());
		UnforcablePlaceAreaOfTile(Main.maxTilesX - 125, (int)(Main.maxTilesY * 0.72f), Main.maxTilesX - 20, (int)(Main.maxTilesY * 0.9f), ModContent.TileType<DragonScaleWood>());
		PlaceRectangleAreaOfWall(Main.maxTilesX - 125, (int)(Main.maxTilesY * 0.72f), Main.maxTilesX - 20, (int)(Main.maxTilesY * 0.9f), ModContent.WallType<DragonScaleWoodWall>());
		BuildBoundOf23Stratum();
		BuildDeathJadeLake();
		BuildTunnelTo2ndStratum();
		BuildMossyCavesLow();
		BuildMossyCavesHigh();
		GreenTundra();

		// ScarletGarden();
		IsleOfBloomBaseLand();
		MazeUnderLake();
		DragonPond();
		IsleOfBloom();
		UnderwaterTreasury();
		VampireMatCave();

		DeathJadeLakeBiome.GetLiquidSurfaceY();

		// IsleOfBloom();
		// BuildRainValley();
	}

	/// <summary>
	/// 初始化
	/// </summary>
	public static void Initialize()
	{
		WaterDeliveryHoleTiles = new List<int>();
		WaterDeliveryHoleTiles.Add(ModContent.TileType<WaterDeliveryHole>());
		WaterDeliveryHoleTiles.Add(ModContent.TileType<WaterDeliveryHole_V>());
		WaterDeliveryHoleTiles.Add(ModContent.TileType<WaterDeliveryHole_BottomLeft>());
		WaterDeliveryHoleTiles.Add(ModContent.TileType<WaterDeliveryHole_BottomRight>());
		WaterDeliveryHoleTiles.Add(ModContent.TileType<WaterDeliveryHole_TopLeft>());
		WaterDeliveryHoleTiles.Add(ModContent.TileType<WaterDeliveryHole_TopRight>());
	}

	/// <summary>
	/// 亡碧湖
	/// </summary>
	public static void BuildDeathJadeLake()
	{
		int startY = (int)(Main.maxTilesY * 0.85f);
		int startX = GenRand.Next(60, 70);
		startX += Main.maxTilesX / 2;
		while (startY < (int)(Main.maxTilesY * 0.9f))
		{
			startY++;
			Tile tile = SafeGetTile(startX, startY);
			if (tile.HasTile)
			{
				startY -= 20;
				break;
			}
		}
		int randY = GenRand.Next(512);
		int randX = GenRand.Next(512);
		int bankWidth = GenRand.Next(220, 240);
		int peakHeight = 0;

		// Lakeshore
		for (int step = 0; step < bankWidth; step++)
		{
			int height = (int)(step * step / 270f + GetPerlinPixelB((step + randX) % 512, randY) * 256f / 30f) - 24;
			int x = startX + step;
			int y = startY - height;
			int count = 0;
			while (!SafeGetTile(x, y).HasTile)
			{
				Tile tile = SafeGetTile(x, y);
				tile.TileType = (ushort)ModContent.TileType<OldMoss>();
				tile.HasTile = true;
				count++;
				if (count > 300)
				{
					break;
				}
				y++;
			}
			if (height > peakHeight)
			{
				peakHeight = height;
			}
		}
		int countX = 0;
		for (int step = startX + bankWidth; step < Main.maxTilesX - 20; step++)
		{
			countX++;
			int countY = -100;
			int curveY = (int)(20 + MathF.Pow((step - (startX + bankWidth)) / (float)Main.maxTilesX, 2.2f) * 2400);
			for (int y = startY - curveY - 100; y < Main.maxTilesY * 0.901f; y++)
			{
				countY++;
				int x = step;
				int type = ModContent.TileType<OldMoss>();
				int wallType = ModContent.WallType<OldMossWall>();
				float stoneValue = 0;
				if (countY <= 0)
				{
					stoneValue = countY / 12f;
				}
				if (countY is > 12 and < 48)
				{
					stoneValue = (countY - 12) / 12f;
				}
				if (countY >= 48)
				{
					stoneValue = 3f;
				}
				Vector2 transform = new Vector2(x, y).RotatedBy(0.55f);
				stoneValue += MeltingPixel[(int)MathF.Abs(transform.X) % 256, (int)MathF.Abs(transform.Y) % 256] / 175f;
				if (countY <= 0 && countX < 50)
				{
					stoneValue = -1;
				}

				if (stoneValue > 1)
				{
					type = ModContent.TileType<MossProneSandSoil>();
					wallType = ModContent.WallType<MossProneSandSoilWall>();
				}
				if (stoneValue > 2)
				{
					type = ModContent.TileType<YggdrasilGrayRock>();
					wallType = ModContent.WallType<MossProneSandSoilWall>();
				}
				if (!SafeGetTile(x, y).HasTile && stoneValue >= 0)
				{
					Tile tile = SafeGetTile(x, y);
					tile.TileType = (ushort)type;
					tile.HasTile = true;
					tile.WallType = (ushort)wallType;
				}
			}
		}

		// Ascending Road
		int lakePeakX = startX + bankWidth;
		randY = GenRand.Next(512);
		randX = GenRand.Next(512);
		for (int step = 0; step < 30; step++)
		{
			int thick = (int)((30 - step) * (30 - step) / 26d + GetPerlinPixelB((step + randX) % 512, randY) * 256f / 30f);
			for (int deltaY = 0; deltaY < thick; deltaY++)
			{
				int x = lakePeakX + step;
				int y = startY - peakHeight + deltaY;
				Tile tile = SafeGetTile(x, y);
				tile.TileType = (ushort)ModContent.TileType<OldMoss>();
				tile.HasTile = true;
			}
		}

		// Lake water
		for (int x = 50; x <= startX + bankWidth; x++)
		{
			int y = startY - peakHeight + 7;
			int count = 0;
			while (!SafeGetTile(x, y).HasTile)
			{
				count++;
				if (count > 300)
				{
					break;
				}
				if (x > KelpCurtainBiome.FindClosestStratumBoundPointX(y))
				{
					Tile tile = SafeGetTile(x, y);
					tile.LiquidType = LiquidID.Water;
					tile.LiquidAmount = 255;
				}
				y++;
			}
		}

		// Mossy Dock
		int liquidSurfaceY = startY - peakHeight + 7;
		Point dockTail = new Point((int)(Main.maxTilesX * 0.5f), liquidSurfaceY - 3);
		dockTail.X += CheckSpaceRight(dockTail);
		PlaceRectangleAreaOfBlock(dockTail.X - 18, dockTail.Y, dockTail.X + 2, dockTail.Y, ModContent.TileType<MossyDockWood>(), (int)TileUtils.TileChangeState.NoTile);
		PlaceRectangleAreaOfBlock(dockTail.X - 19, dockTail.Y - 1, dockTail.X + 2, dockTail.Y - 1, ModContent.TileType<MossyDockWood>(), (int)TileUtils.TileChangeState.NoTile);
		PlaceFrameImportantTilesAbove(dockTail.X - 19, dockTail.Y - 1, 3, 3, ModContent.TileType<BlackAwningBoatSign>());
		int stumpY = dockTail.Y - 2;
		int stumpX = dockTail.X - 16;
		for (int dx = 0; dx < 10; dx++)
		{
			var tile = SafeGetTile(stumpX + dx * 3, stumpY);
			if (!tile.HasTile)
			{
				tile.TileType = TileID.WoodenBeam;
				tile.HasTile = true;
			}
			if (dx % 4 == 0)
			{
				PlaceTileListTowardDownUntilCollide(new Point(stumpX + dx * 3, stumpY + 3), TileID.WoodenBeam);
			}
		}
	}

	/// <summary>
	/// 1，2层分界
	/// </summary>
	public static void BuildTunnelTo2ndStratum()
	{
		var checkPos = (FindSquamousShellTopLeft() + new Point(250, 5)).ToVector2();
		var checkVel = new Vector2(6, 0);
		float radius = 7f;
		for (int t = 0; t < 20; t++)
		{
			CircleTile(checkPos, radius + GenRand.NextFloat(-1.5f, 1.5f), -1, true);
			checkPos += checkVel;
		}
		checkVel = new Vector2(0, -7);
		radius = 24f;
		for (int t = 0; t < 30; t++)
		{
			CircleTile(checkPos, radius + GenRand.NextFloat(-3.5f, 3.5f), -1, true);
			radius += 0.2f;
			checkPos += checkVel;
		}
		checkPos += new Vector2(-120, 210);
		checkVel = new Vector2(12, 0);
		for (int t = 0; t < 13; t++)
		{
			GenerateStalactite(checkPos + new Vector2(0, 10), 6, Main.rand.NextFloat(24, 30), ModContent.TileType<StoneScaleWood>());
			checkPos += checkVel;
		}
		checkPos.X -= 45;
		checkPos.Y -= 30;
		int deltaY = CheckSpaceDown((int)checkPos.X, (int)checkPos.Y);
		checkPos.Y += deltaY - 2;
		for (int x = (int)(checkPos.X - 4); x <= (int)(checkPos.X + 5); x++)
		{
			var tile = SafeGetTile(x, (int)(checkPos.Y + 2));
			tile.TileType = (ushort)ModContent.TileType<OldMoss>();
			tile.HasTile = true;
		}

		// Place Geyser Air Buds.
		PlaceFrameImportantTiles((int)checkPos.X, (int)checkPos.Y, 2, 2, ModContent.TileType<GeyserAirBudsPlatform>());
		for (int t = 1; t < 16; t++)
		{
			Vector2 addPos = new Vector2((t % 2 - 0.5f) * 30 + 10 + Main.rand.NextFloat(-3, 3), -t * 24 + 10);
			Vector2 topPos = checkPos + addPos;
			GenerateStalactite(topPos, 6, Main.rand.NextFloat(12, 16), ModContent.TileType<OldMoss>());
			topPos.Y -= 10;
			int deltaYTop = CheckSpaceDown((int)topPos.X, (int)topPos.Y);
			topPos.Y += deltaYTop - 2;
			for (int x = (int)(topPos.X - 1); x <= (int)(topPos.X + 2); x++)
			{
				var tile = SafeGetTile(x, (int)(topPos.Y + 2));
				tile.ClearEverything();
				tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
				tile.TileType = (ushort)ModContent.TileType<OldMoss>();
				tile.HasTile = true;
				tile.Slope = SlopeType.Solid;
			}
			PlaceFrameImportantTiles((int)topPos.X, (int)topPos.Y, 2, 2, ModContent.TileType<GeyserAirBudsPlatform>());
		}
	}

	public static Point FindSquamousShellTopLeft()
	{
		for (int x = 500; x <= Main.maxTilesX - 500; x++)
		{
			for (int y = (int)(Main.maxTilesY * 0.89f); y <= (int)(Main.maxTilesY * 0.96f); y++)
			{
				Tile tile = SafeGetTile(x, y);
				if (tile.TileType == ModContent.TileType<SquamousShellSeal>())
				{
					return new Point(x - tile.TileFrameX / 18, y - tile.TileFrameY / 18);
				}
			}
		}
		return new Point(0, 0);
	}

	/// <summary>
	/// 低海拔苔穴
	/// </summary>
	public static void BuildMossyCavesLow()
	{
		int times = 500;
		for (int t = 0; t < times; t++)
		{
			int x = GenRand.Next((int)(Main.maxTilesX * 0.76f), (int)(Main.maxTilesX * 0.93f));
			int y = GenRand.Next((int)(Main.maxTilesY * 0.892f), (int)(Main.maxTilesY * 0.899f));
			DigAMossyCaveLow(x, y, GenRand.NextFloat(7, 8), GenRand.NextFloat(16, 42));
		}
		for (int t = 0; t < times; t++)
		{
			int x = GenRand.Next((int)(Main.maxTilesX * 0.76f), (int)(Main.maxTilesX * 0.93f));
			int y = GenRand.Next((int)(Main.maxTilesY * 0.887f), (int)(Main.maxTilesY * 0.8984f));
			Point dir = new Point(GenRand.Next(-2, 12), GenRand.Next(14, 45));
			Point p0 = new Point(x, y);
			Point p1 = new Point(x, y) + dir;
			if (To100NearestBlockDistance(p0.X, p0.Y) > 3 && To100NearestBlockDistance(p1.X, p1.Y) > 3)
			{
				ConnectMossyTunnel(p0, p1, GenRand.NextFloat(8, 10));
			}
		}
	}

	/// <summary>
	/// 高海拔苔穴
	/// </summary>
	public static void BuildMossyCavesHigh()
	{
		for (int x = (int)(Main.maxTilesX * 0.75); x < Main.maxTilesX - 20; x++)
		{
			for (int y = (int)(Main.maxTilesY * 0.877); y < (int)(Main.maxTilesY * 0.9); y++)
			{
				Vector2 origVec = new Vector2(x, y);
				Vector2 decayCenter = new Vector2(Main.maxTilesX * 0.78f, Main.maxTilesY * 0.89f);
				float distanceToDecayCenter = (decayCenter - origVec).Length();
				Vector2 transform = origVec.RotatedBy(MathHelper.PiOver4);
				float decayValue = GetMeltingPixel((int)transform.X, (int)transform.Y);
				if (distanceToDecayCenter < 250)
				{
					decayValue += (250 - distanceToDecayCenter) / 120f;
				}
				if (y < Main.maxTilesY * 0.88)
				{
					float lerpValue = (float)(Main.maxTilesY * 0.88 - y) / 63f;
					decayValue = (float)Utils.Lerp(decayValue, 0.6f, lerpValue);
				}
				var tile = SafeGetTile(x, y);
				if (!tile.HasTile && tile.WallType == WallID.None)
				{
					if (decayValue is > 0.2f and < 1f)
					{
						tile.HasTile = false;
						tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
					}
					if (decayValue is > 0.5f and < 0.7f)
					{
						tile.HasTile = true;
						tile.TileType = (ushort)ModContent.TileType<OldMoss>();
						tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
					}
				}
			}
		}
		for (int x = (int)(Main.maxTilesX * 0.6); x <= (int)(Main.maxTilesX * 0.75); x++)
		{
			for (int y = (int)(Main.maxTilesY * 0.877); y < (int)(Main.maxTilesY * 0.884); y++)
			{
				Vector2 origVec = new Vector2(x, y);
				Vector2 transform = origVec.RotatedBy(MathHelper.PiOver4);
				float decayValue = GetMeltingPixel((int)transform.X, (int)transform.Y);
				Vector2 decayCenter = new Vector2(Main.maxTilesX * 0.78f, Main.maxTilesY * 0.89f);
				float distanceToDecayCenter = (decayCenter - origVec).Length();
				if (distanceToDecayCenter < 250)
				{
					decayValue += (250 - distanceToDecayCenter) / 120f;
				}
				if (x <= (int)(Main.maxTilesX * 0.7))
				{
					decayValue += (float)(Main.maxTilesX * 0.7f - x) / 200f;
				}
				if (y < Main.maxTilesY * 0.88)
				{
					float lerpValue = (float)(Main.maxTilesY * 0.88 - y) / 63f;
					decayValue = (float)Utils.Lerp(decayValue, 0.6f, lerpValue);
				}
				else
				{
					decayValue += (y - Main.maxTilesY * 0.88f) / 80f;
				}
				var tile = SafeGetTile(x, y);
				if (!tile.HasTile && tile.WallType == WallID.None)
				{
					if (decayValue is > 0.2f and < 1f)
					{
						tile.HasTile = false;
						tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
					}
					if (decayValue is > 0.5f and < 0.7f)
					{
						tile.HasTile = true;
						tile.TileType = (ushort)ModContent.TileType<OldMoss>();
						tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
					}
				}
			}
		}
	}

	/// <summary>
	/// 碧绿苔原
	/// </summary>
	public static void GreenTundra()
	{
		Vector2 checkPos = new Vector2(Main.maxTilesX * 0.25f, Main.maxTilesY * 0.88f);
		Vector2 vel = new Vector2(3, -1.5f);
		int maxStep = 500;
		float radius = 4f;
		for (int t = 0; t < maxStep; t++)
		{
			CircleTile(checkPos, radius, ModContent.TileType<OldMoss>());
			CircleWall(checkPos, radius - 1, ModContent.WallType<OldMossWall>());
			vel = vel * 0.98f + new Vector2(4, 0) * 0.02f;
			checkPos += vel;
			radius = radius * 0.99f + 34 * 0.01f;
			if (checkPos.X > Main.maxTilesX * 0.96f)
			{
				break;
			}
		}

		// maxStep = 60;
		// for (int t = 0; t < maxStep; t++)
		// {
		// Vector2 check = new Vector2(GenRand.NextFloat(Main.maxTilesX * 0.75f, Main.maxTilesX * 0.94f), GenRand.NextFloat(Main.maxTilesY * 0.874f, Main.maxTilesY * 0.878f));
		// DigTunnel(check.X, check.Y, GenRand.NextFloat(-1, 1), 1, GenRand.Next(12, 65), GenRand.Next(4, 8));
		// }
		for (int x = (int)(Main.maxTilesX * 0.75f); x <= (int)(Main.maxTilesX * 0.96f); x++)
		{
			int y = (int)(Main.maxTilesY * 0.8795f);
			if (To100NearestBlockDistance(x, y) >= 3)
			{
				int middleX = x - CheckSpaceLeft(x, y) + (CheckSpaceRight(x, y) + CheckSpaceLeft(x, y)) / 2;
				x = middleX;
				float width = (CheckSpaceRight(x, y) + CheckSpaceLeft(x, y)) / 2;
				width = Math.Min((CheckSpaceRight(x, y) + CheckSpaceLeft(x, y)) / 2, 7);
				DigGreenTundraTunnel(x, y, width, new Vector2(-1, -1).RotatedByRandom(0.3), 0);
				x += CheckSpaceLeft(x, y) + 3;
			}
		}
	}

	/// <summary>
	/// 碧绿苔原专用的挖隧道
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="width"></param>
	/// <param name="velocity"></param>
	/// <param name="depth"></param>
	public static void DigGreenTundraTunnel(int x, int y, float width, Vector2 velocity, int depth)
	{
		if (depth >= 3)
		{
			return;
		}
		Vector2 checkPos = new Vector2(x, y);
		Vector2 vel = velocity.NormalizeSafe() * 2.6f;
		int maxStep = 400;
		for (int t = 0; t < maxStep; t++)
		{
			CircleTile(checkPos, width, -1, true);
			checkPos += vel;
			vel = vel.RotatedBy((GetPerlinPixelR(checkPos.X, checkPos.Y) - 60f / 255f) * 0.06f);
			if (!SafeGetTile(checkPos + vel.NormalizeSafe() * (width + 2)).HasTile && t < maxStep - 5 && t > 20)
			{
				maxStep = t + 4;
			}
			if (checkPos.Y < (int)(Main.maxTilesY * 0.875f) || checkPos.Y > (int)(Main.maxTilesY * 0.88f))
			{
				return;
			}
		}
	}

	/// <summary>
	/// 森雨幽谷
	/// </summary>
	public static void BuildRainValley()
	{
		int startY = (int)(Main.maxTilesY * 0.85f);
		int randY = GenRand.Next(512);
		int randX = GenRand.Next(512);
		while (startY < (int)(Main.maxTilesY * 0.89f))
		{
			startY++;
			Tile tile = SafeGetTile(Main.maxTilesX / 2, startY);
			if (tile.HasTile)
			{
				break;
			}
		}
		startY -= 200;
		for (int y = startY; y > (int)(Main.maxTilesY * 0.80f); y--)
		{
			for (int x = Main.maxTilesX / 2; x <= Main.maxTilesX - 20; x++)
			{
				int dense = (int)(GetPerlinPixelB(x / 4 + randX, y + randY) * 256);
				if (dense > 160)
				{
					Tile tile = SafeGetTile(x, y);
					tile.TileType = (ushort)ModContent.TileType<OldMoss>();
					tile.HasTile = true;
				}
			}
		}
	}

	#region IsleOfBloom

	/// <summary>
	/// 夭华洲
	/// </summary>
	public static void IsleOfBloom()
	{
		int startY = (int)(Main.maxTilesY * 0.88f);
		startY += CheckSpaceDown((int)(Main.maxTilesX * 0.31f), startY) - 20;
		Point tilePos = new Point((int)(Main.maxTilesX * 0.31f), startY);
		List<Point> area =
		[
			tilePos + new Point(-130, 0),
			tilePos + new Point(130, 0),
			tilePos + new Point(150, 120),
			tilePos + new Point(-150, 120),
		];
		area = GetPolygonAreaOfTilePos(area);
		foreach (var pos in area)
		{
			var checkPoint = pos;
			var tile = SafeGetTile(checkPoint);
			if (!tile.HasTile || tile.TileType != ModContent.TileType<DarkLakeBottomMud>())
			{
				continue;
			}
			if (pos.Y - tilePos.Y > 105 + GetLargeSmokeTexturePixelB(pos.X * 3, pos.Y * 3) * 15f)
			{
				continue;
			}
			tile.LiquidAmount = 0;
			float value0 = GetPerlinPixelG(pos.X, pos.Y) * 12;
			if (pos.Y - tilePos.Y < 20 + value0)
			{
				tile.TileType = (ushort)ModContent.TileType<OldMoss>();
			}
			else
			{
				tile.TileType = (ushort)ModContent.TileType<MossProneSandSoil>();
			}
			float value1 = GetPerlinPixelR(pos.X, pos.Y);
			float value2 = GetPerlinPixelR(pos.Y, pos.X);
			if (pos.Y - tilePos.Y > value1 * 3 + value0)
			{
				tile.HasTile = true;
			}
			else
			{
				tile.HasTile = false;
			}
			if (pos.Y - tilePos.Y > value2 * 3 + value0 + 16)
			{
				if (pos.Y - tilePos.Y > value2 * 4 + value0 * 1.2f + 24)
				{
					tile.WallType = (ushort)ModContent.WallType<MossProneSandSoilWall>();
				}
				else
				{
					tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
				}
			}
		}

		// Middle Cave Shaft
		for (int y = -10; y <= 70; y += 2)
		{
			float radius = (60 - y) / 3f;
			radius = MathF.Max(radius, 10) * 1.4f;
			PlaceCircleAreaOfBlockWithRandomNoise(tilePos + new Point(0, y), radius, ModContent.TileType<OldMoss>(), 3, (int)TileChangeState.HasTile);
			PlaceCircleAreaOfBlockWithRandomNoise(tilePos + new Point(0, y), radius - 3, -1, 3, (int)TileChangeState.Forceful);
			PlaceCircleAreaOfWallWithRandomNoise(tilePos + new Point(0, y), radius - 1, ModContent.WallType<OldMossWall>(), 3, (int)TileChangeState.HasWall);
		}

		// Cave Visual Effects
		int wallStart = -51;
		for (int y = -10; y < 200; y++)
		{
			var tile = SafeGetTile(tilePos + new Point(0, y));
			if (tile.WallType > WallID.None && wallStart == -51)
			{
				wallStart = y;
				break;
			}
		}
		var rayTile = SafeGetTile(tilePos + new Point(0, wallStart + 5));
		rayTile.WallType = (ushort)ModContent.WallType<OldMossWall>();
		rayTile.TileType = (ushort)ModContent.TileType<IsleOfBloom_CaveRay>();
		rayTile.HasTile = true;

		// SubFloor Cave
		List<Vector2> Cave0_Bound = new List<Vector2>();
		List<Vector2> Cave0 = new List<Vector2>();
		int cave0Y = 75;
		int caveHeight = 200;
		YggdrasilWorld.KelpCurtain_IsleOfBloom_CaveCenter = tilePos + new Point(0, cave0Y);
		for (int x = -130; x <= 130; x++)
		{
			float height = 130 - MathF.Abs(x);
			height = Math.Clamp(height, 0, caveHeight + 96);
			float value2 = GetPerlinPixelR(x * 2, cave0Y) * 64;
			height += value2;
			Cave0_Bound.Add(new Vector2(x * 16, cave0Y * 16 - height));
			height -= 64;
			height = Math.Clamp(height, 0, caveHeight);
			value2 = GetPerlinPixelB(x * 2 + 260, cave0Y + 40) * 64;
			height += value2;
			if (Math.Abs(x) < 120)
			{
				Cave0.Add(new Vector2(x * 16, cave0Y * 16 - height));
			}
		}
		for (int x = 130; x >= -130; x--)
		{
			float height = 130 - MathF.Abs(x);
			height = Math.Clamp(height, 0, caveHeight + 96);
			float value2 = GetPerlinPixelB(x * 2, cave0Y + 30) * 64;
			height += value2;
			Cave0_Bound.Add(new Vector2(x * 16, cave0Y * 16 + height));
			height -= 64;
			height = Math.Clamp(height, 0, caveHeight);
			value2 = GetPerlinPixelB(x * 2 + 260, cave0Y + 70) * 64;
			height += value2;
			if (Math.Abs(x) < 120)
			{
				Cave0.Add(new Vector2(x * 16, cave0Y * 16 + height));
			}
		}
		PlacePolygonAreaOfBlockWithOffset(Cave0_Bound, tilePos.ToWorldCoordinates(), ModContent.TileType<OldMoss>(), (int)TileChangeState.HasTile);
		PlacePolygonBoundOfBlock(Cave0_Bound, ModContent.TileType<OldMoss>(), 64, (int)TileChangeState.HasTile);
		PlacePolygonAreaOfBlockWithOffset(Cave0, tilePos.ToWorldCoordinates(), -1, (int)TileChangeState.Forceful);
		SmoothTile_XXYY(tilePos.X - 150, tilePos.Y - 60, tilePos.X + 150, tilePos.Y + 150);
		List<Point> caveTiles = GetPolygonAreaOfTilePos(Cave0_Bound);
		IsleOfBloom_CaveKelpMoss(caveTiles);

		// Bamboo
		IsleOfBloom_PlantBamboo(tilePos);

		// Side peach
		for (int y = -3; y <= 40; y += 2)
		{
			if (GenRand.NextBool(4))
			{
				int checkX = 0;
				int direction = -1;
				if (GenRand.NextBool())
				{
					direction = 1;
				}
				for (int x = 0; x < 23; x++)
				{
					var tile = SafeGetTile(tilePos.X + x * direction, tilePos.Y + y);
					checkX = (x - 1) * direction;
					if (tile.HasTile)
					{
						break;
					}
				}
				if (MathF.Abs(checkX) < 21)
				{
					if (direction == -1)
					{
						if (CheckSpaceRight(tilePos.X + checkX, tilePos.Y + y) > 11)
						{
							PlaceFrameImportantTiles(tilePos.X + checkX, tilePos.Y + y, 10, 1, ModContent.TileType<IslePeachTree_side>(), 180);
							IsleOfBloom_FillBlockHorizontally(tilePos + new Point(checkX - 1, y), true);
						}
					}
					else
					{
						if (CheckSpaceLeft(tilePos.X + checkX, tilePos.Y + y) > 11)
						{
							PlaceFrameImportantTiles(tilePos.X + checkX - 10, tilePos.Y + y, 10, 1, ModContent.TileType<IslePeachTree_side>(), 0);
							IsleOfBloom_FillBlockHorizontally(tilePos + new Point(checkX, y), false);
						}
					}
					break;
				}
			}
		}

		// Peach
		IsleOfBloom_PlantPeachTree_Surface(tilePos);

		// Small Peach
		IsleOfBloom_PlantSmallPeachTree_Surface(tilePos);

		// Wall Peach
		IsleOfBloom_PlantPeachTree_ShaftWall(tilePos);

		// Left wing and cave
		IsleOfBloom_LeftWingAndCave();
		IsleOfBloom_PlantBamboo_LeftWing(tilePos);

		// Float Stone Island
		IsleOfBloom_FloatStoneIsland(tilePos);
	}

	public static void IsleOfBloomBaseLand()
	{
		int lakeSurfaceY = (int)(Main.maxTilesY * 0.88);
		int lakeCenterX = (int)(Main.maxTilesX * 0.4);
		lakeSurfaceY += CheckWaterSurfaceDown(lakeCenterX, lakeSurfaceY);
		int xBoundLeft = (int)(Main.maxTilesX * 0.22f);
		int xBoundRight = (int)(Main.maxTilesX * 0.4f);
		for (int x = xBoundLeft; x < xBoundRight; x++)
		{
			int lakeBottomY = (int)(Main.maxTilesY * 0.88);
			lakeBottomY += CheckSpaceDown(lakeCenterX, lakeBottomY);
			float xLength = xBoundRight - xBoundLeft;
			float height = (MathF.Sin((x - xBoundLeft) / xLength * MathHelper.TwoPi - MathHelper.PiOver2) + 1) * 465f;
			float heightMax = lakeBottomY - lakeSurfaceY + GetPerlinPixelG(x, 15) * 16 + 32;
			height = MathF.Min(heightMax, height);
			for (int y = 0; y < height; y++)
			{
				var tile = SafeGetTile(x, lakeBottomY - y);
				tile.TileType = (ushort)ModContent.TileType<DarkLakeBottomMud>();
				if (y < height - 5)
				{
					tile.wall = (ushort)ModContent.WallType<DarkLakeBottomMudWall>();
				}
				tile.HasTile = true;
			}
		}
	}

	public static void IsleOfBloom_PlantPeachTree_ShaftWall(Point tilePos)
	{
		List<Vector2> peachPos = new List<Vector2>();
		for (int t = 0; t < 999; t++)
		{
			int x = GenRand.Next(-30, 31);
			int y = GenRand.Next(-10, 101);
			bool valid = true;
			foreach (var pos in peachPos)
			{
				if (new Vector2(x - pos.X, y - pos.Y).Length() < 7)
				{
					valid = false;
					break;
				}
			}
			if (valid)
			{
				var checkPoint = tilePos + new Point(x, y);
				var tile = SafeGetTile(checkPoint);
				float toSidePeach = ToNearestTypeOfTile(tilePos.X + x, tilePos.Y + y, ModContent.TileType<IslePeachTree_side>(), 5).Length();
				if (tile.WallType == ModContent.WallType<OldMossWall>() && !tile.HasTile && toSidePeach > 80)
				{
					if (toSidePeach < 130)
					{
						Main.NewText(toSidePeach);
					}
					tile.TileType = (ushort)ModContent.TileType<IslePeachTree_wall_large>();
					tile.HasTile = true;
					peachPos.Add(new Vector2(x, y));
					if (peachPos.Count > 10)
					{
						break;
					}
				}
			}
		}

		for (int t = 0; t < 999; t++)
		{
			int x = GenRand.Next(-30, 31);
			int y = GenRand.Next(-10, 101);
			bool valid = true;
			foreach (var pos in peachPos)
			{
				if (new Vector2(x - pos.X, y - pos.Y).Length() < 4)
				{
					valid = false;
					break;
				}
			}
			if (valid)
			{
				var checkPoint = tilePos + new Point(x, y);
				var tile = SafeGetTile(checkPoint);
				float toSidePeach = ToNearestTypeOfTile(tilePos.X + x, tilePos.Y + y, ModContent.TileType<IslePeachTree_side>(), 5).Length();
				if (tile.WallType == ModContent.WallType<OldMossWall>() && !tile.HasTile && toSidePeach > 80)
				{
					if (toSidePeach < 130)
					{
						Main.NewText(toSidePeach);
					}
					tile.TileType = (ushort)ModContent.TileType<IslePeachTree_wall_medium>();
					tile.HasTile = true;
					peachPos.Add(new Vector2(x, y));
					if (peachPos.Count > 22)
					{
						break;
					}
				}
			}
		}

		for (int t = 0; t < 999; t++)
		{
			int x = GenRand.Next(-30, 31);
			int y = GenRand.Next(-10, 101);
			bool valid = true;
			foreach (var pos in peachPos)
			{
				if (new Vector2(x - pos.X, y - pos.Y).Length() < 3)
				{
					valid = false;
					break;
				}
			}
			if (valid)
			{
				var checkPoint = tilePos + new Point(x, y);
				var tile = SafeGetTile(checkPoint);
				float toSidePeach = ToNearestTypeOfTile(tilePos.X + x, tilePos.Y + y, ModContent.TileType<IslePeachTree_side>(), 5).Length();
				if (tile.WallType == ModContent.WallType<OldMossWall>() && !tile.HasTile && toSidePeach > 80)
				{
					if (toSidePeach < 130)
					{
						Main.NewText(toSidePeach);
					}
					tile.TileType = (ushort)ModContent.TileType<IslePeachTree_wall_small>();
					tile.HasTile = true;
					peachPos.Add(new Vector2(x, y));
					if (peachPos.Count > 40)
					{
						break;
					}
				}
			}
		}
	}

	public static void IsleOfBloom_PlantPeachTree_Surface(Point tilePos)
	{
		List<int> peachPosX = new List<int>();
		for (int t = 0; t < 999; t++)
		{
			int x = GenRand.Next(-30, 31);
			bool valid = true;
			if (Math.Abs(x) < 15)
			{
				valid = false;
			}
			foreach (var oldX in peachPosX)
			{
				if (Math.Abs(x - oldX) < 7)
				{
					valid = false;
					break;
				}
			}
			if (valid)
			{
				int surfaceY = 0;
				bool safe = false;
				for (int y = 0; y <= 30; y++)
				{
					var checkPoint = tilePos + new Point(x, y);
					var tile = SafeGetTile(checkPoint);
					if (IsTileSolid(tile))
					{
						surfaceY = y - 1;
						safe = true;
						break;
					}
				}
				if (safe)
				{
					float value2 = GetFixedRandomNumber(x, surfaceY, 4);
					for (int j = -1; j <= 1 + value2; j++)
					{
						var checkPoint = tilePos + new Point(x, surfaceY - j);
						var tile = SafeGetTile(checkPoint);
						if (j >= 0)
						{
							tile.TileType = (ushort)ModContent.TileType<IslePeachTree_medium>();
							tile.HasTile = true;
						}
						else
						{
							IsleOfBloom_FillBlockBelow(checkPoint);
						}
					}
					peachPosX.Add(x);
					if (peachPosX.Count > 2)
					{
						break;
					}
				}
			}
		}
	}

	public static void IsleOfBloom_PlantSmallPeachTree_Surface(Point tilePos)
	{
		List<int> peachPosX = new List<int>();
		for (int t = 0; t < 999; t++)
		{
			int x = GenRand.Next(-60, 61);
			bool valid = true;
			foreach (var oldX in peachPosX)
			{
				if (Math.Abs(x - oldX) < 3)
				{
					valid = false;
					break;
				}
			}
			if (MathF.Abs(x) < 20)
			{
				valid = false;
			}

			if (valid)
			{
				int surfaceY = 0;
				bool safe = false;
				for (int y = 0; y <= 50; y++)
				{
					var checkPoint = tilePos + new Point(x, y);
					var tile = SafeGetTile(checkPoint);
					surfaceY = y;
					float toSidePeach = ToNearestTypeOfTile(tilePos.X + x, tilePos.Y + y, ModContent.TileType<IslePeachTree_medium>(), 3).Length();
					if (tile.HasTile && tile.TileType == ModContent.TileType<OldMoss>() && toSidePeach > 48)
					{
						safe = true;
						break;
					}
				}
				if (safe)
				{
					PlaceFrameImportantTilesAbove(tilePos.X + x, tilePos.Y + surfaceY, 1, 2, ModContent.TileType<IslePeachTree_small>());
					IsleOfBloom_FillBlockBelow(tilePos + new Point(x, surfaceY));
					peachPosX.Add(x);
					if (peachPosX.Count > 5)
					{
						break;
					}
				}
			}
		}
	}

	public static void IsleOfBloom_LeftWingAndCave()
	{
		Point p0 = new Point(Main.maxTilesX / 2, (int)(Main.maxTilesY * 0.8857143f));
		p0.Y += CheckWaterSurfaceDown(p0.X, p0.Y) - 20;
		int waterSurface = p0.Y + 20;
		IsleOfBloom_RightSideDock(waterSurface);
		int maze_TopY = waterSurface + 45;
		p0.X += -CheckSpaceLeft(p0.X, p0.Y) + 30;
		Point startP0 = p0;
		List<Point> leftWing = new List<Point>();
		leftWing.Add(p0);
		for (int t = 1; t <= 10; t++)
		{
			Point p1 = p0 + new Point(t * 10, (int)(GetLargeSmokeTexturePixelG(t * 4f + p0.X, p0.Y) * 26f) - 13);
			leftWing.Add(p1);
			if (t == 10)
			{
				p0 = p1;
			}
			else
			{
				GenerateStalactite(p1.ToVector2() + new Vector2(GenRand.NextFloat(-3, 3), 0), GenRand.NextFloat(3, 6), GenRand.NextFloat(8, 24), ModContent.TileType<YggdrasilGrayRock>());
				if (GenRand.NextBool())
				{
					GenerateStalactite(p1.ToVector2() + new Vector2(5 + GenRand.NextFloat(-3, 3), 0), GenRand.NextFloat(3, 6), GenRand.NextFloat(8, 24), ModContent.TileType<YggdrasilGrayRock>());
				}
			}
		}
		int rightBoundBottom = p0.X;
		p0 += new Point(16, -16);
		leftWing.Add(p0);
		for (int t = 0; t <= 6; t++)
		{
			Point p1 = p0 + new Point(-t * 10, (int)(GetLargeSmokeTexturePixelG(t * 4f + p0.X, p0.Y) * 12f) - 6);
			if (t > 3)
			{
				int value = t - 3;
				p1.Y -= value * value * 3;
			}
			leftWing.Add(p1);
			if (t == 6)
			{
				p0 = p1;
			}
		}
		p0 += new Point(100, -100);
		Point rightPeak = p0;
		leftWing.Add(p0);
		p0 += new Point(-180, 110);
		Point leftTail = p0;
		leftWing.Add(p0);
		for (int t = 1; t <= 20; t++)
		{
			Point wallPosLerp = PointLerp(p0, startP0, t / 20f);
			wallPosLerp.X += (int)(GetLargeSmokeTexturePixelB(t * 3, waterSurface) * 22 - 11);
			leftWing.Add(wallPosLerp);
		}
		PlacePolygonAreaOfBlock(leftWing, ModContent.TileType<YggdrasilGrayRock>());
		List<Point> wingTiles = GetPolygonAreaOfTilePos(leftWing);
		foreach (var pos in wingTiles)
		{
			Vector2 line0 = rightPeak.ToVector2();
			Vector2 line1 = leftTail.ToVector2();
			float distance = MathUtils.PointToLineDistance(line0, line1, pos.ToVector2());
			float value0 = 10f / (distance + 1f);
			value0 = MathF.Pow(value0, 0.5f);
			float value1 = GetCellPixel(pos.X * 4, pos.Y * 4) * 10f;
			value1 *= value1 / 5f;
			if (value0 < 1f)
			{
				value1 *= value0;
			}
			if (value0 + value1 > 6f)
			{
				var tile = SafeGetTile(pos);
				if (tile.HasTile && tile.TileType == ModContent.TileType<YggdrasilGrayRock>())
				{
					tile.TileType = (ushort)ModContent.TileType<OldMoss>();
					tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
				}
			}
		}

		Point wall_p0 = p0 + new Point(-16, 16);
		List<Point> leftWing_Wall = new List<Point>();
		leftWing_Wall.Add(wall_p0);
		wall_p0.X += 8;
		wall_p0.Y = maze_TopY;
		leftWing_Wall.Add(wall_p0);
		wall_p0.X += 130;
		leftWing_Wall.Add(wall_p0);
		Point wall_Right_destination = new Point(rightBoundBottom, waterSurface - 20);
		for (int t = 0; t <= 20; t++)
		{
			Point wallPosLerp = PointLerp(wall_p0, wall_Right_destination, t / 20f);
			wallPosLerp.X += (int)(GetLargeSmokeTexturePixelB(t, waterSurface) * 22 - 11);
			leftWing_Wall.Add(wallPosLerp);

			// Arrive at the right entrance of the cave.
			if (t == 20)
			{
				wall_p0 = wallPosLerp;
			}
		}
		wall_p0 -= new Point(65, 65);
		leftWing_Wall.Add(wall_p0);
		PlacePolygonAreaOfWall(leftWing_Wall, ModContent.WallType<YggdrasilGrayRockWall>(), (int)TileChangeState.NoWall);
		for (int t = 0; t < 5; t++)
		{
			IsleOfBloom_SoilPlatform(new Point(leftTail.X + t * 20, leftTail.Y - 20 - t * 20), 55);
		}
	}

	public static void IsleOfBloom_SoilPlatform(Point tilePos, float maxLength)
	{
		tilePos.X += CheckSpaceRight(tilePos) - 3;
		int minusY = 0;
		for (int dx = 0; dx < maxLength; dx++)
		{
			if (GenRand.NextBool(3))
			{
				minusY += 1;
			}
			var pos = tilePos + new Point(-dx, minusY);
			if (maxLength - dx <= 1)
			{
				tilePos = pos;
			}
			int height = CheckSpaceDown(pos);
			if (height < 36)
			{
				for (int dy = 0; dy < height; dy++)
				{
					var tile = SafeGetTile(pos + new Point(0, dy));
					if (!tile.HasTile)
					{
						if (dy > GetPerlinPixelG(pos.X, dy) * 5)
						{
							if (dy > GetPerlinPixelR(pos.X, dy + 5) * 5 + 10)
							{
								tile.TileType = (ushort)ModContent.TileType<MossProneSandSoil>();
							}
							else
							{
								tile.TileType = (ushort)ModContent.TileType<OldMoss>();
							}
							tile.HasTile = true;
						}
						if (dy > GetPerlinPixelR(pos.X, dy) * 5 + 3)
						{
							if (dy > GetPerlinPixelB(pos.X, dy + 5) * 5 + 13)
							{
								tile.WallType = (ushort)ModContent.WallType<MossProneSandSoilWall>();
							}
							else
							{
								tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
							}
							tile.HasTile = true;
						}
					}
					else
					{
						break;
					}
				}
			}
			else
			{
				tilePos = pos;
				break;
			}
		}
		PlaceCircleAreaOfBlockWithRandomNoise(tilePos + new Point(0, 18), 12, ModContent.TileType<MossProneSandSoil>(), 10, (int)TileChangeState.NoTile);
		PlaceCircleAreaOfWallWithRandomNoise(tilePos + new Point(0, 18), 9, (ushort)ModContent.WallType<MossProneSandSoilWall>(), 10, (int)TileChangeState.NoTile);
		PlaceCircleAreaOfBlockWithRandomNoise(tilePos + new Point(0, 18), 18, ModContent.TileType<OldMoss>(), 10, (int)TileChangeState.NoTile);
		PlaceCircleAreaOfWallWithRandomNoise(tilePos + new Point(0, 18), 15, (ushort)ModContent.WallType<OldMossWall>(), 10, (int)TileChangeState.NoTile);
	}

	public static void IsleOfBloom_PlantBamboo(Point tilePos)
	{
		List<int> bambooPosX = new List<int>();
		for (int t = 0; t < 999; t++)
		{
			int x = GenRand.Next(-110, 111);
			if (MathF.Abs(x) > 30)
			{
				bool valid = true;
				foreach (var oldX in bambooPosX)
				{
					if (Math.Abs(x - oldX) < 3)
					{
						valid = false;
						break;
					}
				}
				if (valid)
				{
					int surfaceY = 0;
					for (int y = 0; y <= 40; y++)
					{
						var checkPoint = tilePos + new Point(x, y);
						var tile = SafeGetTile(checkPoint);
						if (tile.HasTile)
						{
							surfaceY = y - 1;
							break;
						}
					}
					if (surfaceY <= 39)
					{
						float value2 = GetFixedRandomNumber(x, surfaceY, 12);
						for (int j = -1; j <= 27 + value2; j++)
						{
							var checkPoint = tilePos + new Point(x, surfaceY - j);
							var tile = SafeGetTile(checkPoint);
							if (j >= 0)
							{
								tile.TileType = (ushort)ModContent.TileType<IsleBamboo>();
								tile.HasTile = true;
							}
							else
							{
								tile.TileType = (ushort)ModContent.TileType<OldMoss>();
								tile.HasTile = true;
								tile.IsHalfBlock = false;
								tile.Slope = SlopeType.Solid;
							}
						}
						bambooPosX.Add(x);
						if (bambooPosX.Count > 36)
						{
							break;
						}
					}
				}
			}
		}
	}

	public static void IsleOfBloom_PlantBamboo_LeftWing(Point tilePos)
	{
		tilePos += new Point(90, -140);
		List<int> bambooPosX = new List<int>();
		for (int t = 0; t < 999; t++)
		{
			int x = GenRand.Next(0, 190);
			if (MathF.Abs(x) > 30)
			{
				bool valid = true;
				foreach (var oldX in bambooPosX)
				{
					if (Math.Abs(x - oldX) < 3)
					{
						valid = false;
						break;
					}
				}
				if (valid)
				{
					int surfaceY = CheckSpaceDown(tilePos + new Point(x, 0)) - 1;

					float value2 = GetFixedRandomNumber(x, surfaceY, 12);
					for (int j = -1; j <= 27 + value2; j++)
					{
						var checkPoint = tilePos + new Point(x, surfaceY - j);
						var tile = SafeGetTile(checkPoint);
						if (j >= 0)
						{
							tile.TileType = (ushort)ModContent.TileType<IsleBamboo>();
							tile.HasTile = true;
						}
						else
						{
							tile.TileType = (ushort)ModContent.TileType<OldMoss>();
							tile.HasTile = true;
							tile.IsHalfBlock = false;
							tile.Slope = SlopeType.Solid;
						}
					}
					bambooPosX.Add(x);
					if (bambooPosX.Count > 36)
					{
						break;
					}
				}
			}
		}
	}

	public static void IsleOfBloom_FillBlockBelow(Point pos)
	{
		for (int x = -1; x <= 1; x++)
		{
			for (int y = 0; y < 100; y++)
			{
				var tile = SafeGetTile(pos.X + x, pos.Y + y);
				if (!tile.HasTile || (tile.HasTile && (tile.IsHalfBlock || tile.Slope != SlopeType.Solid)))
				{
					tile.TileType = (ushort)ModContent.TileType<OldMoss>();
					tile.HasTile = true;
					tile.IsHalfBlock = false;
					tile.Slope = SlopeType.Solid;
				}
				else
				{
					break;
				}
			}
		}
	}

	public static void IsleOfBloom_FillBlockHorizontally(Point pos, bool towardLeft)
	{
		int dir = 1;
		if (towardLeft)
		{
			dir = -1;
		}
		for (int y = -1; y <= 1; y++)
		{
			for (int x = 0; x < 100; x++)
			{
				var tile = SafeGetTile(pos.X + x * dir, pos.Y + y);
				if (!tile.HasTile || (tile.HasTile && (tile.IsHalfBlock || tile.Slope != SlopeType.Solid)))
				{
					tile.TileType = (ushort)ModContent.TileType<OldMoss>();
					tile.HasTile = true;
					tile.IsHalfBlock = false;
					tile.Slope = SlopeType.Solid;
				}
				else
				{
					break;
				}
			}
		}
	}

	public static void IsleOfBloom_RightSideDock(int waterSurfaceY)
	{
		int liquidSurfaceY = waterSurfaceY;
		Point dockTail = new Point((int)(Main.maxTilesX * 0.5f), liquidSurfaceY - 3);
		dockTail.X -= CheckSpaceLeft(dockTail);
		PlaceRectangleAreaOfBlock(dockTail.X + 28, dockTail.Y, dockTail.X - 2, dockTail.Y, ModContent.TileType<MossyDockWood>(), (int)TileUtils.TileChangeState.NoTile);
		PlaceRectangleAreaOfBlock(dockTail.X + 29, dockTail.Y - 1, dockTail.X - 2, dockTail.Y - 1, ModContent.TileType<MossyDockWood>(), (int)TileUtils.TileChangeState.NoTile);
		PlaceFrameImportantTilesAbove(dockTail.X + 27, dockTail.Y - 1, 3, 3, ModContent.TileType<BlackAwningBoatSign>(), 54);
		int stumpY = dockTail.Y - 2;
		int stumpX = dockTail.X + 26;
		for (int dx = 0; dx < 10; dx++)
		{
			var tile = SafeGetTile(stumpX - dx * 3, stumpY);
			if (!tile.HasTile)
			{
				tile.TileType = TileID.WoodenBeam;
				tile.HasTile = true;
			}
			if (dx % 4 == 0)
			{
				PlaceTileListTowardDownUntilCollide(new Point(stumpX - dx * 3, stumpY + 3), TileID.WoodenBeam);
			}
		}
	}

	public static void IsleOfBloom_FloatStoneIsland(Point tilePos)
	{
		int count = 0;
		for (int t = 0; t < 999; t++)
		{
			var checkPos = tilePos + new Point(GenRand.Next(-160, 161), GenRand.Next(-110, -10));
			Point nearest = FindNearestTileWithin100Range(checkPos);
			float distance = PointDistance(checkPos, nearest);
			float radius = GenRand.NextFloat(7, 24);
			if (distance < radius + 12)
			{
				continue;
			}
			List<Point> tiles = GetCircleAreaOfTilePosWithRandomNoise(checkPos, radius, 6);
			if (tiles.Count <= 0)
			{
				continue;
			}
			Point centroid = GetCentroid(tiles);
			List<int> bambooX = new List<int>();
			foreach (var pos in tiles)
			{
				var tile = SafeGetTile(pos);
				if (pos.Y < centroid.Y - GetLargeSmokeTexturePixelR(pos.X * 8, pos.Y * 8) * 12)
				{
					tile.TileType = (ushort)ModContent.TileType<OldMoss>();
				}
				else
				{
					tile.TileType = (ushort)ModContent.TileType<YggdrasilGrayRock>();
				}
				tile.HasTile = true;
				var tileAbove = SafeGetTile(pos + new Point(0, -1));
				if (tileAbove.HasTile && tileAbove.TileType == ModContent.TileType<IsleBamboo>())
				{
					for (int j = 1; j < 999; j++)
					{
						var tile_above_above = SafeGetTile(pos.X, pos.Y - j);
						if (tile_above_above.HasTile && tile_above_above.TileType == ModContent.TileType<IsleBamboo>())
						{
							tile_above_above.HasTile = false;
						}
						else
						{
							break;
						}
					}
				}
				if (GenRand.NextBool(4))
				{
					if (!tileAbove.HasTile)
					{
						bool safe = true;
						foreach (var b_x in bambooX)
						{
							if (Math.Abs(pos.X - b_x) < 3)
							{
								safe = false;
								break;
							}
						}
						if (safe)
						{
							int maxHeight = CheckSpaceUp(pos + new Point(0, -1));
							maxHeight = Math.Min(maxHeight, GenRand.Next(24, 40));
							for (int j = 0; j < maxHeight; j++)
							{
								var bambooTile = SafeGetTile(pos + new Point(0, -1) + new Point(0, -j));
								bambooTile.TileType = (ushort)ModContent.TileType<IsleBamboo>();
								bambooTile.HasTile = true;
							}
							bambooX.Add(pos.X);
						}
					}
				}
			}
			List<Point> walls = GetCircleAreaOfTilePosWithRandomNoise(checkPos, radius - 2);
			foreach (var pos in walls)
			{
				var tile = SafeGetTile(pos);
				if (tile.TileType == ModContent.TileType<OldMoss>())
				{
					tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
				}
				if (tile.TileType == ModContent.TileType<YggdrasilGrayRock>())
				{
					tile.WallType = (ushort)ModContent.WallType<YggdrasilGrayRockWall>();
				}
			}
			count++;
			if (count >= 10)
			{
				break;
			}
		}
	}

	public static void IsleOfBloom_CaveKelpMoss(List<Point> tiles)
	{
		foreach (var pos in tiles)
		{
			var tile = SafeGetTile(pos);
			if (GenRand.NextBool(2))
			{
				var tile_below = SafeGetTile(pos + new Point(0, 1));
				if (tile.HasTile && tile.TileType == ModContent.TileType<OldMoss>() && !tile_below.HasTile)
				{
					tile.Slope = SlopeType.Solid;
					tile.IsHalfBlock = false;
					var length = CheckSpaceDown(pos + new Point(0, 1));
					length = Math.Min(length, GenRand.Next(5, 15));
					for (int j = 0; j < length; j++)
					{
						var tile_kelp = SafeGetTile(pos + new Point(0, 1) + new Point(0, j));
						tile_kelp.TileType = (ushort)ModContent.TileType<KelpMoss>();
						tile_kelp.HasTile = true;
					}
				}
			}
			if (tile.WallType != ModContent.WallType<OldMossWall>())
			{
				if (GetPerlinPixelR(pos.X, pos.Y) > 0.8f)
				{
					tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
				}
				else
				{
					tile.WallType = WallID.None;
				}
			}
		}
	}

	#endregion

	#region MazeUnderLake

	/// <summary>
	/// 水下迷宫
	/// </summary>
	public static void MazeUnderLake()
	{
		int lakeSurfaceY = (int)(Main.maxTilesY * 0.88);
		int lakeCenterX = (int)(Main.maxTilesX * 0.4);
		lakeSurfaceY += CheckWaterSurfaceDown(lakeCenterX, lakeSurfaceY);
		int xBoundLeft = (int)(Main.maxTilesX * 0.34f);
		int xBoundRight = (int)(Main.maxTilesX * 0.64f);
		lakeCenterX = (int)(Main.maxTilesX * 0.5);
		int lakeBottomYHalfX = (int)(Main.maxTilesY * 0.88);
		lakeBottomYHalfX += CheckSpaceDown(lakeCenterX, lakeBottomYHalfX);
		int yBoundTop = lakeSurfaceY + 45;
		MazeUnderLake_TopY = yBoundTop;
		int yBoundBottom = lakeBottomYHalfX + 15;
		MazeUnderLake_YggdrasilBlackChestShrineCount = 0;
		MazeUnderLake_YggdrasilBlackRockChestContents = new List<int>();

		// We need a bound of Yggdrasil Black Rock.
		List<Vector2> MazeBoundPolygon = new List<Vector2>();
		int distance_Center_Left = CheckSpaceLeft(lakeCenterX, yBoundTop);
		int distance_Center_Right = CheckSpaceRight(lakeCenterX, yBoundTop);
		MazeBoundPolygon.Add(new Vector2(lakeCenterX - distance_Center_Left, yBoundTop) * 16);
		for (int dx = lakeCenterX - distance_Center_Left + 5; dx < lakeCenterX + distance_Center_Right; dx += 10)
		{
			int depthOfLake = CheckSpaceDown(dx, yBoundTop);
			MazeBoundPolygon.Add(new Vector2(dx, yBoundTop + depthOfLake) * 16);
		}
		MazeBoundPolygon.Add(new Vector2(lakeCenterX + distance_Center_Right, yBoundTop) * 16);
		PlacePolygonBoundOfBlock(MazeBoundPolygon, ModContent.TileType<YggdrasilBlackRock>(), 40, (int)TileChangeState.Forceful);
		PlacePolygonBoundOfWall(MazeBoundPolygon, ModContent.WallType<YggdrasilBlackRockWall>(), 40, (int)TileChangeState.Forceful);

		// Random seed Points
		List<Point> seeds = GenerateRandomSeeds(xBoundLeft - 30, xBoundRight + 30, yBoundTop - 30, yBoundBottom + 30, 180, 25);
		for (int x = xBoundLeft; x < xBoundRight; x++)
		{
			for (int y = yBoundTop; y < yBoundBottom; y++)
			{
				// Exist a projection. SeedMap is not TileMap.
				var tile = SafeGetTile(x, y);
				if (!tile.HasTile)
				{
					if (MazeUnderLake_IsEdgePoint(x, y, seeds))
					{
						tile.TileType = (ushort)ModContent.TileType<YggdrasilBlackRock>();
						tile.HasTile = true;
					}
					if (MazeUnderLake_IsEdgePoint(x, y, seeds, 1.8f))
					{
						tile.WallType = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
					}
				}
			}
		}

		for (int x = xBoundLeft; x < xBoundRight; x++)
		{
			for (int y = yBoundTop - 10; y <= yBoundTop + 10; y++)
			{
				int value = y - yBoundTop;
				var tile = SafeGetTile(x, y);
				if (!tile.HasTile)
				{
					if (value > -GetPerlinPixelG(x, y) * 4f - 4f && value < GetPerlinPixelR(x, y) * 4f + 4f)
					{
						tile.TileType = (ushort)ModContent.TileType<YggdrasilBlackRock>();
						tile.HasTile = true;
						if (value > -GetPerlinPixelG(x, y) * 4f - 3f && value < GetPerlinPixelR(x, y) * 4f + 3f)
						{
							tile.WallType = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
						}
					}
				}
			}
		}
		Dictionary<Point, List<Point>> holes = new Dictionary<Point, List<Point>>();
		Point mediumSeedPos = default;
		float minDisToCenter = new Vector2(xBoundRight - xBoundLeft, yBoundBottom - yBoundTop).Length();
		float min_seeds_Y = float.MaxValue;
		float max_seeds_Y = 0;
		float avg_seeds_Y = 0;

		// Connect seeds with holes, and connect seeds with each other.
		foreach (var pos in seeds)
		{
			avg_seeds_Y += pos.Y;
			min_seeds_Y = Math.Min(min_seeds_Y, pos.Y);
			max_seeds_Y = Math.Max(max_seeds_Y, pos.Y);
			float boundRange = 8;
			Vector2 center = new Vector2(xBoundLeft + xBoundRight, yBoundTop + yBoundBottom) * 0.5f;
			float toCenterDis = new Vector2(pos.X - center.X, pos.Y - center.Y).Length();
			if (toCenterDis < minDisToCenter)
			{
				minDisToCenter = toCenterDis;
				mediumSeedPos = pos;
			}
			if (pos.X >= xBoundLeft + boundRange && pos.X <= xBoundRight - boundRange && pos.Y <= yBoundBottom - boundRange && pos.Y >= yBoundTop + boundRange && !SafeGetTile(pos).HasTile)
			{
				MazeUnderLake_AddNewConnection(pos, holes, seeds);
			}
		}
		avg_seeds_Y /= seeds.Count;
		if (mediumSeedPos != default)
		{
			int maxStep = 10;
			for (int t = 0; t < maxStep; t++)
			{
				List<Point> connectedWithMediumSeeds = MazeUnderLake_GetAllConnectedPoints(holes, seeds, new List<Point>(), mediumSeedPos, 0);
				foreach (var pos in connectedWithMediumSeeds)
				{
					bool noTileWithin5x5 = true;
					for (int dx = -2; dx <= 2; dx++)
					{
						for (int dy = -2; dy <= 2; dy++)
						{
							Tile surroundTile = SafeGetTile(pos.X + dx, pos.Y + dy);
							if (surroundTile.HasTile)
							{
								noTileWithin5x5 = false;
								break;
							}
						}
						if (!noTileWithin5x5)
						{
							break;
						}
					}
					if (noTileWithin5x5)
					{
						MazeUnderLake_AddNewConnectionWithMediumNet(pos, holes, seeds, connectedWithMediumSeeds);
					}
				}
			}
		}

		// Fill remaining gaps during generation. This step is important to avoid too many small holes in the maze.
		foreach (var pos in seeds)
		{
			bool inTheArea = MathUtils.IsPointInPolygon(MazeBoundPolygon, pos.ToWorldCoordinates());
			if (!inTheArea)
			{
				continue;
			}
			bool flag0 = holes.ContainsKey(pos);
			if (flag0)
			{
				continue;
			}
			bool canFill = true;
			foreach (var des in seeds)
			{
				bool flag2 = holes.ContainsKey(des) && holes[des].Contains(pos);
				if (flag2)
				{
					canFill = false;
					break;
				}
			}
			if (canFill)
			{
				List<Point> cellArea = BFSContinueEmpty(pos, false, 1024);
				foreach (var cell_pos in cellArea)
				{
					MazeUnderLake_FillYggdrasilBlackRock(cell_pos.X, cell_pos.Y);
				}
			}
		}
		for (int x = xBoundLeft; x < xBoundRight; x += 5)
		{
			for (int y = yBoundTop; y <= yBoundBottom; y += 5)
			{
				List<Point> cellArea = BFSContinueEmpty(new Point(x, y), false, 25);
				if (cellArea.Count < 25 && cellArea.Count > 0)
				{
					foreach (var pos in cellArea)
					{
						MazeUnderLake_FillYggdrasilBlackRock(pos.X, pos.Y);
					}
				}
			}
		}

		// Build rooms based on seeds.
		foreach (var pos in seeds)
		{
			bool inTheArea = MathUtils.IsPointInPolygon(MazeBoundPolygon, pos.ToWorldCoordinates());
			if (!inTheArea)
			{
				continue;
			}

			List<Point> tiles = BFSContinueEmpty(pos, false, 1536, WaterDeliveryHoleTiles);
			if (pos.Y <= avg_seeds_Y)
			{
				if (GenRand.NextBool(5))
				{
					MazeUnderLake_RedAlgaeRoom(tiles);
				}
				else
				{
					MazeUnderLake_BuildDesolateRoom(tiles);
				}
			}
			else
			{
				if (GenRand.NextBool(4))
				{
					MazeUnderLake_RedAlgaeRoom(tiles);
				}
				else
				{
					if (pos.Y <= avg_seeds_Y * 0.75f + max_seeds_Y * 0.25f + GenRand.NextFloat(-120, 120))
					{
						MazeUnderLake_BuildSpongeRoom(tiles);
					}
					else
					{
						MazeUnderLake_BuildFluorescentHydraRoom(tiles);
					}
				}
			}
		}

		// Entrance of the maze. This is an important part for players to find the maze and get into it.
		float xMiddle = xBoundLeft + xBoundRight;
		xMiddle /= 2;
		int successCount = 0;
		List<int> successedX = new List<int>();
		for (int k = 0; k < 999; k++)
		{
			bool success = false;
			float randomX = GenRand.NextFloat(-120, 120);
			float checkX = xMiddle + randomX;
			float avg_x = -1;
			Point checkPos = new Vector2(checkX, yBoundTop + 11).ToPoint();
			List<Point> checkRoom = BFSContinueEmpty(checkPos, false, 1536, WaterDeliveryHoleTiles);
			if (checkRoom.Count > 300 && checkRoom.Count < 1536)
			{
				foreach (var pos in checkRoom)
				{
					var tile = SafeGetTile(pos);
					if (WaterDeliveryHoleTiles.Contains(tile.TileType))
					{
						success = true;
					}
					avg_x += pos.X;
				}
			}
			if (success)
			{
				avg_x /= checkRoom.Count;
				bool canBuildEntrance = true;
				foreach (var x in successedX)
				{
					if (Math.Abs(avg_x - x) < 24)
					{
						canBuildEntrance = false;
						break;
					}
				}
				if (canBuildEntrance)
				{
					int surfaceY = yBoundTop - 20;
					surfaceY += CheckSpaceDown((int)avg_x, surfaceY);
					Vector2 entranceCenter = new Vector2(avg_x, surfaceY + 2) * 16;
					float tilt = GenRand.NextFloat(-0.8f, 0.8f);
					Vector2 top_Vertex = new Vector2(0, -320).RotatedBy(tilt) + entranceCenter;
					Vector2 left_Vertex = new Vector2(-720, 0) + entranceCenter;
					Vector2 right_Vertex = new Vector2(720, 0) + entranceCenter;
					List<Vector2> curve_polygon = new List<Vector2>();
					curve_polygon.Add(left_Vertex);
					for (int m = 0; m < 20; m++)
					{
						float value = m / 20f;
						Vector2 lerpPos = Vector2.Lerp(left_Vertex, top_Vertex, value);
						lerpPos = Vector2.Lerp(lerpPos, entranceCenter, MathF.Sin(value * MathHelper.Pi) * 0.5f);
						curve_polygon.Add(lerpPos);
					}
					curve_polygon.Add(top_Vertex);
					for (int m = 0; m < 20; m++)
					{
						float value = m / 20f;
						Vector2 lerpPos = Vector2.Lerp(top_Vertex, right_Vertex, value);
						lerpPos = Vector2.Lerp(lerpPos, entranceCenter, MathF.Sin(value * MathHelper.Pi) * 0.5f);
						curve_polygon.Add(lerpPos);
					}
					curve_polygon.Add(right_Vertex);
					PlacePolygonAreaOfBlock(curve_polygon, ModContent.TileType<YggdrasilBlackRock>(), (int)TileChangeState.NoWall);
					for (int m = 0; m < curve_polygon.Count; m++)
					{
						var pos = curve_polygon[m];
						pos.Y = pos.Y * 0.9f + entranceCenter.Y * 0.1f;
						curve_polygon[m] = pos;
					}
					PlacePolygonAreaOfWall(curve_polygon, ModContent.WallType<YggdrasilBlackRockWall>(), (int)TileChangeState.Forceful);
					Vector2 des = Vector2.Lerp(top_Vertex, entranceCenter, 1.7f);
					PlaceLineBlock(des, Vector2.Lerp(top_Vertex, entranceCenter, -0.2f), 48, -1, (int)TileChangeState.Forceful);

					Vector2 entranceVertice_RoomSide = Vector2.Lerp(top_Vertex, entranceCenter, 0.4f);
					Point des_point = des.ToTileCoordinates();
					float roomLeft = des_point.X - CheckSpaceLeft(des_point.X, des_point.Y);
					roomLeft *= 16;
					roomLeft += 8;
					float roomRight = des_point.X + CheckSpaceRight(des_point.X, des_point.Y);
					roomRight *= 16;
					roomRight += 8;

					// Main.NewText(CheckSpaceLeft(des_point.X, des_point.Y) + CheckSpaceRight(des_point.X, des_point.Y));
					List<Vector2> killTile_polygon = new List<Vector2>();
					killTile_polygon.Add(new Vector2(roomLeft, des.Y));
					for (int m = 0; m < 20; m++)
					{
						float value = m / 20f;
						Vector2 lerpPos = Vector2.Lerp(new Vector2(roomLeft, des.Y), entranceVertice_RoomSide, value);
						lerpPos = Vector2.Lerp(lerpPos, entranceCenter, MathF.Sin(value * MathHelper.Pi) * 0.5f);
						killTile_polygon.Add(lerpPos);
					}
					killTile_polygon.Add(entranceVertice_RoomSide);
					for (int m = 0; m < 20; m++)
					{
						float value = m / 20f;
						Vector2 lerpPos = Vector2.Lerp(entranceVertice_RoomSide, new Vector2(roomRight, des.Y), value);
						lerpPos = Vector2.Lerp(lerpPos, entranceCenter, MathF.Sin(value * MathHelper.Pi) * 0.5f);
						killTile_polygon.Add(lerpPos);
					}
					killTile_polygon.Add(new Vector2(roomRight, des.Y));
					PlacePolygonAreaOfBlock(killTile_polygon, -1, (int)TileChangeState.SolidBlock);

					successCount++;
					successedX.Add((int)avg_x);
				}
			}
			if (successCount > 3)
			{
				break;
			}
		}

		// Fill remaining gaps at the bottom of the maze.
		for (int x = xBoundLeft; x < xBoundRight; x += 4)
		{
			for (int y = (int)(avg_seeds_Y * 0.5f + max_seeds_Y * 0.5f); y <= max_seeds_Y; y += 4)
			{
				var tile = SafeGetTile(x, y);
				if (!tile.HasTile)
				{
					List<Point> emptySpace = BFSContinueEmpty(new Point(x, y), false, 3072, WaterDeliveryHoleTiles);
					bool shouldFill = true;
					foreach (var pos in emptySpace)
					{
						var checkHole = SafeGetTile(pos);
						if (WaterDeliveryHoleTiles.Contains(checkHole.TileType))
						{
							shouldFill = false;
							break;
						}
					}
					if (shouldFill)
					{
						foreach (var pos in emptySpace)
						{
							var checkTile = SafeGetTile(pos);
							checkTile.TileType = (ushort)ModContent.TileType<YggdrasilBlackRock>();
							checkTile.wall = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
							checkTile.HasTile = true;
						}
					}
				}
			}
			List<int> noSolidMazeTile = new List<int>(WaterDeliveryHoleTiles);
			noSolidMazeTile.Add(ModContent.TileType<CrimsonMoonAlgea>());
			noSolidMazeTile.Add(ModContent.TileType<CrimsonMoonAlgea_fruit>());
			noSolidMazeTile.Add(ModContent.TileType<JadeLakeBloodVineAlgea>());
			noSolidMazeTile.Add(ModContent.TileType<JadeLakeRedAlgae>());
			noSolidMazeTile.Add(ModContent.TileType<AgedOxygenTank>());
			noSolidMazeTile.Add(ModContent.TileType<BrokenOxygenTank>());
			noSolidMazeTile.Add(ModContent.TileType<AbandonedFishingNet>());
			noSolidMazeTile.Add(ModContent.TileType<AbandonedShrimpCage>());
			noSolidMazeTile.Add(ModContent.TileType<AlgaeCoveredChest>());
			noSolidMazeTile.Add(ModContent.TileType<FishSkeleton>());
			noSolidMazeTile.Add(ModContent.TileType<FluorescentHydraTree>());
			noSolidMazeTile.Add(ModContent.TileType<JadeLakeGreenAlgae>());
			noSolidMazeTile.Add(ModContent.TileType<HydraBud1x1>());
			noSolidMazeTile.Add(ModContent.TileType<HydraBud3x3>());
			noSolidMazeTile.Add(ModContent.TileType<JadeLakeSponge>());
			noSolidMazeTile.Add(ModContent.TileType<UnderwaterTentDebris>());
			for (int y = (int)avg_seeds_Y; y <= max_seeds_Y; y += 4)
			{
				var tile = SafeGetTile(x, y);
				if (tile.WallType == WallID.None)
				{
					List<Point> emptySpace = BFSContinueEmpty(new Point(x, y), false, 3072, noSolidMazeTile);
					foreach (var pos in emptySpace)
					{
						var checkTile = SafeGetTile(pos);
						if (checkTile.wall == 0 && pos.Y > avg_seeds_Y + GetPerlinPixelG(pos.X, pos.Y) * 30)
						{
							checkTile.wall = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
						}
					}
				}
			}
		}

		// Cover the top of the maze with DecaySandSoil.
		for (int x = xBoundLeft; x < xBoundRight; x++)
		{
			int topCheckY = yBoundTop - 20;
			var tile = SafeGetTile(x, topCheckY);
			if (!IsTileSolid(tile))
			{
				topCheckY += CheckSpaceDown(x, topCheckY);
			}
			else
			{
				for (int dy = 1; dy < 100; dy++)
				{
					var checkTile = SafeGetTile(x, topCheckY + dy);
					if (!IsTileSolid(checkTile))
					{
						topCheckY += dy;
						break;
					}
				}
			}
			float shrink = 1f;
			if (topCheckY > yBoundTop)
			{
				shrink = (yBoundTop + 10 - topCheckY) / 10f;
				shrink = Math.Clamp(shrink, 0f, 1f);
			}
			int topSandY = topCheckY - (int)(7 - (GetPerlinPixelR(x, yBoundTop) * 5) * shrink);
			for (int y = topSandY; y <= topCheckY + 2; y++)
			{
				var sandTile = SafeGetTile(x, y);
				if (y > topSandY)
				{
					if (!IsTileSolid(sandTile))
					{
						if (y < topCheckY)
						{
							sandTile.TileType = (ushort)ModContent.TileType<DecaySandSoil>();
							sandTile.HasTile = true;
							if (y > topSandY + 1)
							{
								sandTile.wall = (ushort)ModContent.WallType<DecaySandSoilWall>();
							}
						}
					}
				}
				else
				{
					if (GenRand.NextBool(4))
					{
						int height = GenRand.Next(4, 30);
						for (int dy = 0; dy < height; dy++)
						{
							var algaeTile = SafeGetTile(x, y - dy);
							if (!IsTileSolid(algaeTile) && algaeTile.LiquidAmount > 254)
							{
								algaeTile.TileType = (ushort)ModContent.TileType<JadeLakeSargassum>();
								algaeTile.HasTile = true;
							}
							else
							{
								break;
							}
						}
					}
				}
			}
		}
		SmoothTile_XXYY(xBoundLeft - 10, yBoundTop - 40, xBoundRight + 10, yBoundTop);
	}

	public static void MazeUnderLake_BuildSpongeRoom(List<Point> tiles)
	{
		int maxY = 0;
		foreach (var pos in tiles)
		{
			maxY = Math.Max(maxY, pos.Y);
		}
		foreach (var pos in tiles)
		{
			Tile tile = SafeGetTile(pos);
			if (pos.Y > maxY - 7 + GetPerlinPixelB(pos.X, pos.Y) && !tile.HasTile)
			{
				tile.TileType = (ushort)ModContent.TileType<HumicMud>();
				tile.HasTile = true;
				PlaceWallAround(pos, (ushort)ModContent.WallType<DarkLakeBottomMudWall>(), true, false);
			}
		}
		foreach (var pos in tiles)
		{
			Tile tile = SafeGetTile(pos);
			if (GetPerlinPixelB(pos.X, pos.Y * 4) > 0.55f && !tile.HasTile)
			{
				tile.TileType = (ushort)ModContent.TileType<RichOxygenSponge>();
				tile.WallType = (ushort)ModContent.WallType<RichOxygenSpongeWall>();
				tile.HasTile = true;
			}
		}
		MazeUnderLake_ClearChannel(tiles);
		foreach (var pos in tiles)
		{
			if (GenRand.NextBool(7))
			{
				Tile tile = SafeGetTile(pos);
				Tile tile_top = SafeGetTile(pos + new Point(0, -1));
				if (tile.HasTile && !tile_top.HasTile && Collision.IsWorldPointSolid(pos.ToWorldCoordinates()))
				{
					tile_top.TileType = (ushort)ModContent.TileType<JadeLakeSponge>();
					tile_top.TileFrameY = 0;
					tile_top.TileFrameX = (short)(GenRand.Next(12) * 30);
					tile_top.HasTile = true;
				}
			}
		}
		foreach (var pos in tiles)
		{
			Tile tile = SafeGetTile(pos);
			if (GetPerlinPixelR(pos.X, pos.Y * 4) > 0.2f && tile.WallType == WallID.None)
			{
				tile.WallType = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
			}
		}
		SmoothTile_List(tiles);
	}

	public static void MazeUnderLake_BuildFluorescentHydraRoom(List<Point> tiles)
	{
		int maxY = 0;
		foreach (var pos in tiles)
		{
			maxY = Math.Max(maxY, pos.Y);
		}
		foreach (var pos in tiles)
		{
			Tile tile = SafeGetTile(pos);
			if (pos.Y > maxY - 7 + GetPerlinPixelB(pos.X, pos.Y) && !tile.HasTile)
			{
				tile.TileType = (ushort)ModContent.TileType<HydraMud>();
				tile.HasTile = true;
				PlaceWallAround(pos, (ushort)ModContent.WallType<DarkLakeBottomMudWall>(), true, false);
			}
		}
		MazeUnderLake_ClearChannel(tiles);
		List<Point> towardUp_Mud = new List<Point>();
		List<Point> towardUp_Solid = new List<Point>();
		foreach (var pos in tiles)
		{
			var tile = SafeGetTile(pos);
			var tile_up = SafeGetTile(pos.X, pos.Y - 1);
			if (tile.HasTile && tile.TileType == ModContent.TileType<HydraMud>() && !tile_up.HasTile && !towardUp_Mud.Contains(pos))
			{
				towardUp_Mud.Add(pos);
			}
			if (tile.HasTile && Collision.IsWorldPointSolid(pos.ToWorldCoordinates()) && !tile_up.HasTile && !towardUp_Solid.Contains(pos))
			{
				towardUp_Solid.Add(pos);
			}
		}
		List<int> treePosX = new List<int>();
		foreach (var pos in towardUp_Mud)
		{
			int distanceToTop = CheckSpaceUp(pos.X, pos.Y - 1);
			distanceToTop = Math.Min(distanceToTop, 12);
			if (GenRand.NextBool(4) && distanceToTop > 4)
			{
				bool safeToPlace = true;
				foreach (var x in treePosX)
				{
					if (Math.Abs(x - pos.X) <= 4)
					{
						safeToPlace = false;
						break;
					}
				}
				if (safeToPlace)
				{
					treePosX.Add(pos.X);
					int height = GenRand.Next(4, distanceToTop);
					for (int j = 1; j <= height; j++)
					{
						var f_tree = SafeGetTile(pos.X, pos.Y - j);
						f_tree.TileType = (ushort)ModContent.TileType<FluorescentHydraTree>();
						f_tree.HasTile = true;
					}
				}
			}
		}
		foreach (var pos in tiles)
		{
			Tile tile = SafeGetTile(pos);
			if (tile.WallType == WallID.None)
			{
				tile.WallType = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
			}
			if (GenRand.NextBool(12) && !tile.HasTile)
			{
				tile.TileType = (ushort)ModContent.TileType<HydraBudWall>();
				tile.HasTile = true;
			}
		}
		foreach (var pos in towardUp_Solid)
		{
			if (GenRand.NextBool(3))
			{
				if (CanPlaceMultiAtTopTowardsUpRight(pos.X, pos.Y, 3, 3))
				{
					PlaceFrameImportantTilesAbove(pos.X, pos.Y, 3, 3, ModContent.TileType<HydraBud3x3>(), 54 * GenRand.Next(4));
				}
			}
		}
		foreach (var pos in towardUp_Solid)
		{
			Tile tile = SafeGetTile(pos + new Point(0, -1));
			if (GenRand.NextBool(12) && !tile.HasTile)
			{
				tile.TileType = (ushort)ModContent.TileType<HydraBud1x1>();
				tile.TileFrameX = (short)(GenRand.Next(6) * 28);
				tile.HasTile = true;
			}
		}
	}

	public static void MazeUnderLake_BuildDesolateRoom(List<Point> tiles)
	{
		int maxY = 0;
		foreach (var pos in tiles)
		{
			maxY = Math.Max(maxY, pos.Y);
		}
		Vector2 center = default;
		foreach (var pos in tiles)
		{
			center += pos.ToVector2();
			Tile tile = SafeGetTile(pos);
			if (pos.Y > maxY - 7 + GetPerlinPixelB(pos.X, pos.Y) && !tile.HasTile)
			{
				tile.TileType = (ushort)ModContent.TileType<HumicMud>();
				tile.HasTile = true;
				PlaceWallAround(pos, (ushort)ModContent.WallType<DarkLakeBottomMudWall>(), true, false);
			}
		}
		center *= 1f / tiles.Count;
		MazeUnderLake_ClearChannel(tiles);

		if (MazeUnderLake_YggdrasilBlackChestShrineCount < 8)
		{
			if (MazeUnderLake_BuildChestShrine(center.ToPoint()))
			{
				MazeUnderLake_YggdrasilBlackChestShrineCount++;
			}
			else if (MazeUnderLake_BuildChestShrine(center.ToPoint() + new Point(0, -1)))
			{
				MazeUnderLake_YggdrasilBlackChestShrineCount++;
			}
			else if (MazeUnderLake_BuildChestShrine(center.ToPoint() + new Point(0, 1)))
			{
				MazeUnderLake_YggdrasilBlackChestShrineCount++;
			}
		}

		List<Point> towardUp_Mud = new List<Point>();
		foreach (var pos in tiles)
		{
			var tile = SafeGetTile(pos);
			var tile_up = SafeGetTile(pos.X, pos.Y - 1);
			if (tile.HasTile && tile.TileType == ModContent.TileType<HumicMud>() && !tile_up.HasTile && !towardUp_Mud.Contains(pos))
			{
				towardUp_Mud.Add(pos);
			}
		}
		foreach (var pos in towardUp_Mud)
		{
			int distanceToTop = CheckSpaceUp(pos.X, pos.Y - 1);
			if (GenRand.NextBool(4) && distanceToTop > 1)
			{
				ushort algaeType = (ushort)ModContent.TileType<JadeLakeGreenAlgae>();
				int height = GenRand.Next(1, distanceToTop);
				for (int j = 1; j <= height; j++)
				{
					var algeeTile = SafeGetTile(pos.X, pos.Y - j);
					algeeTile.TileType = algaeType;
					algeeTile.HasTile = true;
				}
			}
			if (GenRand.NextBool(8) && distanceToTop > 1)
			{
				ushort algaeType = (ushort)ModContent.TileType<JadeLakeSargassum>();
				int height = GenRand.Next(1, distanceToTop);
				for (int j = 1; j <= height; j++)
				{
					var algeeTile = SafeGetTile(pos.X, pos.Y - j);
					algeeTile.TileType = algaeType;
					algeeTile.HasTile = true;
				}
			}
		}
		foreach (var pos in towardUp_Mud)
		{
			if (GenRand.NextBool(8))
			{
				if (CanPlaceMultiAtTopTowardsUpRight(pos.X, pos.Y, 3, 2))
				{
					switch (GenRand.Next(2))
					{
						case 0:
							PlaceFrameImportantTilesAbove(pos.X, pos.Y, 3, 2, ModContent.TileType<UnderwaterTentDebris>(), 54 * GenRand.Next(4));
							break;
						case 1:
							PlaceFrameImportantTilesAbove(pos.X, pos.Y, 3, 2, ModContent.TileType<FishSkeleton>(), 54 * GenRand.Next(6));
							break;
					}
				}
			}
		}
		foreach (var pos in towardUp_Mud)
		{
			if (GenRand.NextBool(3))
			{
				if (CanPlaceMultiAtTopTowardsUpRight(pos.X, pos.Y, 2, 2))
				{
					switch (GenRand.Next(3))
					{
						case 0:
							PlaceFrameImportantTilesAbove(pos.X, pos.Y, 2, 2, ModContent.TileType<BrokenOxygenTank>(), 36 * GenRand.Next(4));
							break;
						case 1:
							PlaceFrameImportantTilesAbove(pos.X, pos.Y, 2, 2, ModContent.TileType<AgedOxygenTank>(), 36 * GenRand.Next(4));
							break;
						case 2:
							PlaceFrameImportantTilesAbove(pos.X, pos.Y, 2, 2, ModContent.TileType<AbandonedShrimpCage>(), 36 * GenRand.Next(6));
							break;
					}
				}
			}
		}
		foreach (var pos in towardUp_Mud)
		{
			if (GenRand.NextBool(8))
			{
				if (CanPlaceMultiAtTopTowardsUpRight(pos.X, pos.Y, 1, 2))
				{
					PlaceFrameImportantTilesAbove(pos.X, pos.Y, 1, 2, ModContent.TileType<AbandonedFishingNet>(), 18 * GenRand.Next(3));
				}
			}
		}
	}

	public static void MazeUnderLake_RedAlgaeRoom(List<Point> tiles)
	{
		int maxY = 0;
		foreach (var pos in tiles)
		{
			maxY = Math.Max(maxY, pos.Y);
		}
		foreach (var pos in tiles)
		{
			Tile tile = SafeGetTile(pos);
			if (pos.Y > maxY - 7 + GetPerlinPixelB(pos.X, pos.Y) && !tile.HasTile)
			{
				tile.TileType = (ushort)ModContent.TileType<DarkLakeBottomMud>();
				tile.HasTile = true;
				PlaceWallAround(pos, (ushort)ModContent.WallType<DarkLakeBottomMudWall>(), true, false);
			}
		}
		MazeUnderLake_ClearChannel(tiles);

		List<Point> towardUp_Mud = new List<Point>();
		foreach (var pos in tiles)
		{
			var tile = SafeGetTile(pos);
			var tile_up = SafeGetTile(pos.X, pos.Y - 1);
			if (tile.HasTile && tile.TileType == ModContent.TileType<DarkLakeBottomMud>() && !tile_up.HasTile && !towardUp_Mud.Contains(pos))
			{
				towardUp_Mud.Add(pos);
			}
		}
		float avg_mud_x = -1;
		foreach (var pos in towardUp_Mud)
		{
			avg_mud_x += pos.X;
		}
		avg_mud_x /= towardUp_Mud.Count;
		float closestToAvg = towardUp_Mud.Count;
		Point targetForCenterPlant = default;
		if (avg_mud_x != -1)
		{
			foreach (var pos in towardUp_Mud)
			{
				float disToAvg = Math.Abs(pos.X - avg_mud_x);
				if (disToAvg < closestToAvg)
				{
					closestToAvg = disToAvg;
					targetForCenterPlant = pos;
				}
			}
		}
		if (targetForCenterPlant != default)
		{
			int distanceToTop = CheckSpaceUp(targetForCenterPlant.X, targetForCenterPlant.Y - 1);
			int height = distanceToTop / 2;
			for (int j = 1; j <= height; j++)
			{
				var algeeTile = SafeGetTile(targetForCenterPlant.X, targetForCenterPlant.Y - j);
				algeeTile.TileType = (ushort)ModContent.TileType<CrimsonMoonAlgea>();
				if (j == height)
				{
					algeeTile.TileType = (ushort)ModContent.TileType<CrimsonMoonAlgea_fruit>();
				}
				algeeTile.HasTile = true;
			}
		}
		foreach (var pos in towardUp_Mud)
		{
			int distanceToTop = CheckSpaceUp(pos.X, pos.Y - 1);
			if (GenRand.NextBool(4) && distanceToTop > 1)
			{
				int height = GenRand.Next(1, distanceToTop);
				for (int j = 1; j <= height; j++)
				{
					var algeeTile = SafeGetTile(pos.X, pos.Y - j);
					algeeTile.TileType = (ushort)ModContent.TileType<JadeLakeBloodVineAlgea>();
					algeeTile.HasTile = true;
				}
			}
		}

		List<Point> side_tiles = new List<Point>();
		foreach (var pos in tiles)
		{
			var tile = SafeGetTile(pos);
			var pos_up = new Point(pos.X, pos.Y - 1);
			var pos_down = new Point(pos.X, pos.Y + 1);
			var pos_left = new Point(pos.X - 1, pos.Y);
			var pos_right = new Point(pos.X + 1, pos.Y);
			bool flag = !tiles.Contains(pos_up) || !tiles.Contains(pos_down) || !tiles.Contains(pos_left) || !tiles.Contains(pos_right);
			if (tile.HasTile && flag)
			{
				bool flag1 = !side_tiles.Contains(pos_up) || !side_tiles.Contains(pos_down) || !side_tiles.Contains(pos_left) || !side_tiles.Contains(pos_right);
				if (flag1)
				{
					side_tiles.AddRange(BFSSurface(pos));
				}
			}
		}
		side_tiles = side_tiles.Distinct().ToList();
		foreach (var pos in side_tiles)
		{
			var tile = SafeGetTile(pos);
			if (tile.TileType != ModContent.TileType<DarkLakeBottomMud>() && tile.TileType != ModContent.TileType<YggdrasilBlackRock>())
			{
				continue;
			}
			var tile_up = SafeGetTile(pos.X, pos.Y - 1);
			var tile_down = SafeGetTile(pos.X, pos.Y + 1);
			var tile_left = SafeGetTile(pos.X - 1, pos.Y);
			var tile_right = SafeGetTile(pos.X + 1, pos.Y);
			if (!GenRand.NextBool(3))
			{
				if (!tile_up.HasTile)
				{
					tile_up.TileType = (ushort)ModContent.TileType<JadeLakeRedAlgae>();
					tile_up.HasTile = true;
				}
			}
			if (!GenRand.NextBool(3))
			{
				if (!tile_down.HasTile)
				{
					tile_down.TileType = (ushort)ModContent.TileType<JadeLakeRedAlgae>();
					tile_down.HasTile = true;
				}
			}
			if (!GenRand.NextBool(3))
			{
				if (!tile_left.HasTile)
				{
					tile_left.TileType = (ushort)ModContent.TileType<JadeLakeRedAlgae>();
					tile_left.HasTile = true;
				}
			}
			if (!GenRand.NextBool(8))
			{
				if (!tile_right.HasTile)
				{
					tile_right.TileType = (ushort)ModContent.TileType<JadeLakeRedAlgae>();
					tile_right.HasTile = true;
				}
			}
		}
		foreach (var pos in tiles)
		{
			if (GetPerlinPixelR(pos.X * 3, pos.Y * 3) > 0.3f)
			{
				var tile = SafeGetTile(pos);
				tile.WallType = (ushort)ModContent.WallType<DarkLakeBottomMudWall>();
				if (GenRand.NextBool(5) && !tile.HasTile)
				{
					tile.TileType = (ushort)ModContent.TileType<JadeLakeRedAlgae>();
					tile.HasTile = true;
				}
			}
		}
	}

	public static void MazeUnderLake_ClearChannel(List<Point> tiles)
	{
		foreach (var pos in tiles)
		{
			Tile tile = SafeGetTile(pos);
			if (WaterDeliveryHoleTiles.Contains(tile.TileType))
			{
				if (tile == MazeUnderLake_WaterDeliveryHole_GetCenterTile(pos.X, pos.Y))
				{
					int dir = MazeUnderLake_WaterDeliveryHole_GetDirection(tile);
					Vector2 checkTilePos = tile.Center();
					Vector2 normal = new Vector2(8, 0).RotatedBy(MathHelper.PiOver4 * dir);
					checkTilePos += normal * 5;
					List<Vector2> shouldClearTilePos = new List<Vector2>();
					for (int k = 0; k < 32; k++)
					{
						checkTilePos += normal;
						shouldClearTilePos.Add(checkTilePos);
						if (SafeGetTile(checkTilePos.ToTileCoordinates()).TileType != ModContent.TileType<HumicMud>())
						{
							shouldClearTilePos.Add(checkTilePos + normal);
							shouldClearTilePos.Add(checkTilePos + normal * 2);
							shouldClearTilePos.Add(checkTilePos + normal * 3);
							break;
						}
					}
					foreach (var corePos in shouldClearTilePos)
					{
						KillCircleAreaOfBlockWithRandomNoiseInCertainTypeOfTile(corePos.ToTileCoordinates(), 3, new List<int> { ModContent.TileType<HumicMud>(), ModContent.TileType<DarkLakeBottomMud>(), ModContent.TileType<YggdrasilBlackRock>(), ModContent.TileType<RichOxygenSponge>(), ModContent.TileType<HydraMud>() }, -1, 0);
						KillCircleAreaOfWallWithRandomNoiseInCertainType(corePos.ToTileCoordinates(), 4, new List<int> { ModContent.WallType<RichOxygenSpongeWall>() }, -1, 0);
					}
				}
			}
		}
	}

	public static int MazeUnderLake_WaterDeliveryHole_GetDirection(Tile tile)
	{
		int targetType = tile.TileType;
		int dir = -1;
		int style = TileObjectData.GetTileStyle(tile);
		if (targetType == ModContent.TileType<WaterDeliveryHole>())
		{
			if (style == 0)
			{
				dir = 6;
			}
			else
			{
				dir = 2;
			}
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_V>())
		{
			if (style == 0)
			{
				dir = 4;
			}
			else
			{
				dir = 0;
			}
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_BottomRight>())
		{
			dir = 1;
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_BottomLeft>())
		{
			dir = 3;
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_TopLeft>())
		{
			dir = 5;
		}
		if (targetType == ModContent.TileType<WaterDeliveryHole_TopRight>())
		{
			dir = 7;
		}
		return dir;
	}

	public static Tile MazeUnderLake_WaterDeliveryHole_GetCenterTile(int i, int j)
	{
		Tile tile = SafeGetTile(i, j);
		int currentOffsetX = 0;
		int currentOffsetY = 0;
		bool fail = true;
		if (tile.TileType == ModContent.TileType<WaterDeliveryHole_V>())
		{
			currentOffsetX = -tile.TileFrameX / 18 + 1;
			if (tile.TileFrameX >= 36)
			{
				currentOffsetX = -(tile.TileFrameX % 36) / 18;
			}
			currentOffsetY = -tile.TileFrameY / 18 + 2;
			fail = false;
		}
		else if (tile.TileType == ModContent.TileType<WaterDeliveryHole>())
		{
			currentOffsetX = -(tile.TileFrameX % 90) / 18 + 2;
			currentOffsetY = -tile.TileFrameY / 18 + 1;
			fail = false;
		}
		else if (WaterDeliveryHoleTiles.Contains(tile.TileType))
		{
			int currentStyle = TileObjectData.GetTileStyle(tile);
			TileObjectData currentObjectData = TileObjectData.GetTileData(tile.TileType, currentStyle);
			currentOffsetX = -(tile.TileFrameX / 18 - currentStyle * currentObjectData.Width) + currentObjectData.Origin.X;
			currentOffsetY = -tile.TileFrameY / 18 + currentObjectData.Origin.Y;
			fail = false;
		}
		if (fail)
		{
			Main.NewText("Fail to access target", Color.Red);
		}
		return SafeGetTile(i + currentOffsetX, j + currentOffsetY);
	}

	public static void MazeUnderLake_AddNewConnectionWithMediumNet(Point pos, Dictionary<Point, List<Point>> holes, List<Point> seeds, List<Point> mediumNet, int count = 1)
	{
		int connectedCount = 0;
		if (holes.ContainsKey(pos))
		{
			connectedCount += holes[pos].Count;
		}
		List<Point> closeSeeds = new List<Point>();
		foreach (var otherPos in seeds)
		{
			if (otherPos != pos)
			{
				float dis = Vector2.Distance(new Vector2(pos.X, pos.Y), new Vector2(otherPos.X, otherPos.Y));
				Point des = otherPos;
				bool flag1 = holes.ContainsKey(pos) && holes[pos].Contains(des);
				bool flag2 = holes.ContainsKey(des) && holes[des].Contains(pos);
				bool flag3 = mediumNet.Contains(des);
				bool flag4 = des.Y < MazeUnderLake_TopY + 5;
				bool flag5 = BFSContinueEmpty(des, false, 10).Count < 10;
				bool flag6 = BFSContinueEmpty(pos, false, 10).Count < 10;
				if (dis < 50 && !flag1 && !flag2 && !flag3 && !flag4 && !flag5 && !flag6)
				{
					closeSeeds.Add(des);
				}
				if (flag2)
				{
					connectedCount++;
				}
			}
		}
		if (connectedCount >= 3)
		{
			return;
		}
		List<Point> connectedSeeds = new List<Point>();
		if (holes.ContainsKey(pos))
		{
			connectedSeeds = holes[pos];
		}
		for (int k = 0; k < count; k++)
		{
			if (closeSeeds.Count <= 0)
			{
				break;
			}
			int index = GenRand.Next(closeSeeds.Count);
			MazeUnderLake_ConnectDeliveryHole(pos.ToWorldCoordinates(), closeSeeds[index].ToWorldCoordinates());
			connectedSeeds.Add(closeSeeds[index]);
			closeSeeds.RemoveAt(index);
		}
		if (holes.ContainsKey(pos))
		{
			holes[pos] = connectedSeeds;
		}
		else
		{
			holes.Add(pos, connectedSeeds);
		}
	}

	public static void MazeUnderLake_AddNewConnection(Point pos, Dictionary<Point, List<Point>> holes, List<Point> seeds, int count = 1)
	{
		List<Point> closeSeeds = new List<Point>();
		foreach (var otherPos in seeds)
		{
			if (otherPos != pos)
			{
				float dis = Vector2.Distance(new Vector2(pos.X, pos.Y), new Vector2(otherPos.X, otherPos.Y));
				Point des = otherPos;
				bool flag1 = holes.ContainsKey(pos) && holes[pos].Contains(des);
				bool flag2 = holes.ContainsKey(des) && holes[des].Contains(pos);
				bool flag4 = des.Y < MazeUnderLake_TopY + 5;
				bool flag5 = BFSContinueEmpty(des, false, 10).Count < 10;
				bool flag6 = BFSContinueEmpty(pos, false, 10).Count < 10;
				if (dis < 33 && !flag1 && !flag2 && !flag4 && !flag5 && !flag6)
				{
					closeSeeds.Add(des);
				}
			}
		}
		List<Point> connectedSeeds = new List<Point>();
		if (holes.ContainsKey(pos))
		{
			connectedSeeds = holes[pos];
		}
		for (int k = 0; k < count; k++)
		{
			if (closeSeeds.Count <= 0)
			{
				break;
			}
			int index = GenRand.Next(closeSeeds.Count);
			MazeUnderLake_ConnectDeliveryHole(pos.ToWorldCoordinates(), closeSeeds[index].ToWorldCoordinates());
			connectedSeeds.Add(closeSeeds[index]);
			closeSeeds.RemoveAt(index);
		}
		if (holes.ContainsKey(pos))
		{
			holes[pos] = connectedSeeds;
		}
		else
		{
			holes.Add(pos, connectedSeeds);
		}
	}

	public static List<Point> MazeUnderLake_GetAllConnectedPoints(Dictionary<Point, List<Point>> connectMap, List<Point> seedMap, List<Point> checkedList, Point checkPos, int step)
	{
		List<Point> result = checkedList;
		if (step > 8)
		{
			return result;
		}
		List<Point> newPoint = new List<Point>();
		if (connectMap.ContainsKey(checkPos))
		{
			List<Point> subSeed = connectMap[checkPos];
			foreach (var sub in subSeed)
			{
				if (!result.Contains(sub) && !newPoint.Contains(sub))
				{
					newPoint.Add(sub);
				}
			}
		}
		foreach (var parentPos in connectMap.Keys)
		{
			if (connectMap[parentPos].Contains(checkPos))
			{
				if (!result.Contains(parentPos) && !newPoint.Contains(parentPos))
				{
					newPoint.Add(parentPos);
				}
			}
		}
		result.AddRange(newPoint);
		foreach (var point in newPoint)
		{
			List<Point> subNewPoint = MazeUnderLake_GetAllConnectedPoints(connectMap, seedMap, result, point, step + 1);
			foreach (var pos in subNewPoint)
			{
				if (!result.Contains(pos))
				{
					result.Add(pos);
				}
			}
		}
		return result;
	}

	public static void MazeUnderLake_ConnectDeliveryHole(Vector2 pos0, Vector2 pos1)
	{
		if (pos0 == pos1)
		{
			return;
		}
		Vector2 dir = (pos1 - pos0).SafeNormalize(Vector2.Zero);
		float rot = dir.ToRotation();

		int style = -1;
		if (MathF.Abs(rot) <= MathHelper.Pi * 1f / 8f)
		{
			style = 0;
		}
		else if (rot <= MathHelper.Pi * 3f / 8f && rot > MathHelper.Pi * 1f / 8f)
		{
			style = 1;
		}
		else if (rot <= MathHelper.Pi * 5f / 8f && rot > MathHelper.Pi * 3f / 8f)
		{
			style = 2;
		}
		else if (rot <= MathHelper.Pi * 7f / 8f && rot > MathHelper.Pi * 5f / 8f)
		{
			style = 3;
		}
		else if (MathF.Abs(rot) >= MathHelper.Pi * 7f / 8f)
		{
			style = 4;
		}
		else if (rot >= -MathHelper.Pi * 7f / 8f && rot < -MathHelper.Pi * 5f / 8f)
		{
			style = 5;
		}
		else if (rot >= -MathHelper.Pi * 5f / 8f && rot < -MathHelper.Pi * 3f / 8f)
		{
			style = 6;
		}
		else if (rot >= -MathHelper.Pi * 3f / 8f && rot < -MathHelper.Pi * 1f / 8f)
		{
			style = 7;
		}
		Vector2 checkPos = pos0;
		int penetrateState = 0;

		Point origin0 = default;
		Point origin1 = default;
		int penetrate1Count = 0;
		for (int t = 0; t < 400; t++)
		{
			if (t % 2 == 0)
			{
				checkPos.X += dir.X * 4f;
			}
			else
			{
				checkPos.Y += dir.Y * 4f;
			}
			Point tilePos = checkPos.ToTileCoordinates();
			if (penetrateState == 0)
			{
				if (MazeUnderLake_SpecialCollisionWithDeliveryHoleStyle(checkPos, style))
				{
					penetrateState = 1;
					origin0 = tilePos;
				}
			}

			if (penetrateState == 1)
			{ // Second delivery hole.
				penetrate1Count++;
				if (!MazeUnderLake_SpecialCollisionWithDeliveryHoleStyle(checkPos, style) && penetrate1Count >= 16)
				{
					origin1 = tilePos;
					break;
				}
			}
		}
		if (origin0 != default && origin1 != default)
		{
			WaterDeliveryHole_TopLeft waterDeliveryHole_TopLeft = TileLoader.GetTile(ModContent.TileType<WaterDeliveryHole_TopLeft>()) as WaterDeliveryHole_TopLeft;
			WaterDeliveryHole_BottomRight waterDeliveryHole_BottomRight = TileLoader.GetTile(ModContent.TileType<WaterDeliveryHole_BottomRight>()) as WaterDeliveryHole_BottomRight;
			WaterDeliveryHole_TopRight waterDeliveryHole_TopRight = TileLoader.GetTile(ModContent.TileType<WaterDeliveryHole_TopRight>()) as WaterDeliveryHole_TopRight;
			WaterDeliveryHole_BottomLeft waterDeliveryHole_BottomLeft = TileLoader.GetTile(ModContent.TileType<WaterDeliveryHole_BottomLeft>()) as WaterDeliveryHole_BottomLeft;
			switch (style)
			{
				case 0:
					PlaceFrameImportantTiles(origin0.X - 1, origin0.Y - 2, 2, 5, (ushort)ModContent.TileType<WaterDeliveryHole_V>(), 0, 0);
					PlaceRectangleAreaOfBlock_XYWH(origin0.X + 1, origin0.Y - 2, 1, 5, ModContent.TileType<YggdrasilBlackRock>());
					PlaceFrameImportantTiles(origin1.X, origin1.Y - 2, 2, 5, (ushort)ModContent.TileType<WaterDeliveryHole_V>(), 36, 0);
					PlaceRectangleAreaOfBlock_XYWH(origin1.X - 1, origin1.Y - 2, 1, 5, ModContent.TileType<YggdrasilBlackRock>());
					break;
				case 1:
					MazeUnderLake_CheckSuitableForSlopeHole(ref origin0, 3);
					waterDeliveryHole_TopLeft.PlaceAtTileObjectDataOrigin(origin0.X, origin0.Y);
					MazeUnderLake_CheckSuitableForSlopeHole(ref origin1, 1);
					waterDeliveryHole_BottomRight.PlaceAtTileObjectDataOrigin(origin1.X, origin1.Y);
					break;
				case 2:
					PlaceFrameImportantTiles(origin0.X - 2, origin0.Y - 1, 5, 2, (ushort)ModContent.TileType<WaterDeliveryHole>(), 0, 0);
					PlaceRectangleAreaOfBlock_XYWH(origin0.X - 2, origin0.Y + 1, 5, 1, ModContent.TileType<YggdrasilBlackRock>());
					PlaceFrameImportantTiles(origin1.X - 2, origin1.Y, 5, 2, (ushort)ModContent.TileType<WaterDeliveryHole>(), 90, 0);
					PlaceRectangleAreaOfBlock_XYWH(origin1.X - 2, origin1.Y - 1, 5, 1, ModContent.TileType<YggdrasilBlackRock>());
					break;
				case 3:
					MazeUnderLake_CheckSuitableForSlopeHole(ref origin0, 0);
					waterDeliveryHole_TopRight.PlaceAtTileObjectDataOrigin(origin0.X, origin0.Y);
					MazeUnderLake_CheckSuitableForSlopeHole(ref origin1, 2);
					waterDeliveryHole_BottomLeft.PlaceAtTileObjectDataOrigin(origin1.X, origin1.Y);
					break;
				case 4:
					PlaceFrameImportantTiles(origin0.X, origin0.Y - 2, 2, 5, (ushort)ModContent.TileType<WaterDeliveryHole_V>(), 36, 0);
					PlaceRectangleAreaOfBlock_XYWH(origin0.X - 1, origin0.Y - 2, 1, 5, ModContent.TileType<YggdrasilBlackRock>());
					PlaceFrameImportantTiles(origin1.X - 1, origin1.Y - 2, 2, 5, (ushort)ModContent.TileType<WaterDeliveryHole_V>(), 0, 0);
					PlaceRectangleAreaOfBlock_XYWH(origin1.X + 1, origin1.Y - 2, 1, 5, ModContent.TileType<YggdrasilBlackRock>());
					break;
				case 5:
					MazeUnderLake_CheckSuitableForSlopeHole(ref origin0, 1);
					waterDeliveryHole_BottomRight.PlaceAtTileObjectDataOrigin(origin0.X, origin0.Y);
					MazeUnderLake_CheckSuitableForSlopeHole(ref origin1, 3);
					waterDeliveryHole_TopLeft.PlaceAtTileObjectDataOrigin(origin1.X, origin1.Y);
					break;
				case 6:
					PlaceFrameImportantTiles(origin0.X - 2, origin0.Y, 5, 2, (ushort)ModContent.TileType<WaterDeliveryHole>(), 90, 0);
					PlaceRectangleAreaOfBlock_XYWH(origin0.X - 2, origin0.Y - 1, 5, 1, ModContent.TileType<YggdrasilBlackRock>());
					PlaceFrameImportantTiles(origin1.X - 2, origin1.Y - 1, 5, 2, (ushort)ModContent.TileType<WaterDeliveryHole>(), 0, 0);
					PlaceRectangleAreaOfBlock_XYWH(origin1.X - 2, origin1.Y + 1, 5, 1, ModContent.TileType<YggdrasilBlackRock>());
					break;
				case 7:
					MazeUnderLake_CheckSuitableForSlopeHole(ref origin0, 2);
					waterDeliveryHole_BottomLeft.PlaceAtTileObjectDataOrigin(origin0.X, origin0.Y);
					MazeUnderLake_CheckSuitableForSlopeHole(ref origin1, 0);
					waterDeliveryHole_TopRight.PlaceAtTileObjectDataOrigin(origin1.X, origin1.Y);
					break;
			}
			List<Vector2> polygon =
			[
				origin0.ToWorldCoordinates() + new Vector2(0, -48).RotatedBy(MathHelper.PiOver4 * style),
				origin1.ToWorldCoordinates() + new Vector2(0, -48).RotatedBy(MathHelper.PiOver4 * style),
				origin1.ToWorldCoordinates() + new Vector2(0, 48).RotatedBy(MathHelper.PiOver4 * style),
				origin0.ToWorldCoordinates() + new Vector2(0, 48).RotatedBy(MathHelper.PiOver4 * style),
			];
			PlacePolygonAreaOfBlock(polygon, ModContent.TileType<YggdrasilBlackRock>(), (int)TileChangeState.NoTile);
		}
	}

	/// <summary>
	/// Check whether the position is suitable for placing a slope hole.
	/// </summary>
	/// <param name="oldPoint"></param>
	/// <param name="slopeType">0: TopRight 1: BottomRight 2: BottomLeft 3: TopLeft</param>
	/// <returns>A suitable point for placement.</returns>
	public static void MazeUnderLake_CheckSuitableForSlopeHole(ref Point oldPoint, int slopeType)
	{
		switch (slopeType)
		{
			case 0:
				for (int c = 0; c < 2; c++)
				{
					if (SafeGetTile(oldPoint.X - 1, oldPoint.Y).HasTile || SafeGetTile(oldPoint.X, oldPoint.Y + 1).HasTile)
					{
						if (c % 2 == 0)
						{
							oldPoint.X += 1;
						}
						else
						{
							oldPoint.Y -= 1;
						}
					}
				}
				break;
			case 1:
				for (int c = 0; c < 2; c++)
				{
					if (SafeGetTile(oldPoint.X - 1, oldPoint.Y).HasTile || SafeGetTile(oldPoint.X, oldPoint.Y - 1).HasTile)
					{
						if (c % 2 == 0)
						{
							oldPoint.X += 1;
						}
						else
						{
							oldPoint.Y += 1;
						}
					}
				}
				break;
			case 2:
				for (int c = 0; c < 2; c++)
				{
					if (SafeGetTile(oldPoint.X + 1, oldPoint.Y).HasTile || SafeGetTile(oldPoint.X, oldPoint.Y - 1).HasTile)
					{
						if (c % 2 == 0)
						{
							oldPoint.X -= 1;
						}
						else
						{
							oldPoint.Y += 1;
						}
					}
				}
				break;
			case 3:
				for (int c = 0; c < 2; c++)
				{
					if (SafeGetTile(oldPoint.X + 1, oldPoint.Y).HasTile || SafeGetTile(oldPoint.X, oldPoint.Y + 1).HasTile)
					{
						if (c % 2 == 0)
						{
							oldPoint.X -= 1;
						}
						else
						{
							oldPoint.Y -= 1;
						}
					}
				}
				break;
		}
	}

	public static bool MazeUnderLake_SpecialCollisionWithDeliveryHoleStyle(Vector2 checkPos, int style)
	{
		switch (style)
		{
			case 0:
				if (Collision.SolidCollision(checkPos - new Vector2(8, 40), 16, 80))
				{
					return true;
				}
				break;
			case 1:
				if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16) || Collision.SolidCollision(checkPos - new Vector2(8) + new Vector2(32, -32), 16, 16) || Collision.SolidCollision(checkPos - new Vector2(8) + new Vector2(-32, 32), 16, 16))
				{
					return true;
				}
				break;
			case 2:
				if (Collision.SolidCollision(checkPos - new Vector2(40, 8), 80, 16))
				{
					return true;
				}
				break;
			case 3:
				if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16) || Collision.SolidCollision(checkPos - new Vector2(8) + new Vector2(-32, -32), 16, 16) || Collision.SolidCollision(checkPos - new Vector2(8) + new Vector2(32, 32), 16, 16))
				{
					return true;
				}
				break;
			case 4:
				if (Collision.SolidCollision(checkPos - new Vector2(8, 40), 16, 80))
				{
					return true;
				}
				break;
			case 5:
				if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16) || Collision.SolidCollision(checkPos - new Vector2(8) + new Vector2(32, -32), 16, 16) || Collision.SolidCollision(checkPos - new Vector2(8) + new Vector2(-32, 32), 16, 16))
				{
					return true;
				}
				break;
			case 6:
				if (Collision.SolidCollision(checkPos - new Vector2(40, 8), 80, 16))
				{
					return true;
				}
				break;
			case 7:
				if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16) || Collision.SolidCollision(checkPos - new Vector2(8) + new Vector2(-32, -32), 16, 16) || Collision.SolidCollision(checkPos - new Vector2(8) + new Vector2(32, 32), 16, 16))
				{
					return true;
				}
				break;
		}
		return false;
	}

	public static void MazeUnderLake_FillYggdrasilBlackRock(int i, int j)
	{
		Tile tile = SafeGetTile(i, j);
		if (!tile.HasTile)
		{
			tile.TileType = (ushort)ModContent.TileType<YggdrasilBlackRock>();
			tile.HasTile = true;
		}
	}

	public static bool MazeUnderLake_BuildChestShrine(Point pos)
	{
		Point tilePos = pos;
		Point buildPos = pos;
		int dir = 1;
		if (GenRand.NextBool())
		{
			dir = -1;
		}
		int space = CheckSpaceLeft(tilePos.X, tilePos.Y);
		if (dir == -1)
		{
			space = CheckSpaceRight(tilePos.X, tilePos.Y);
		}
		buildPos.X -= space * dir;
		Tile safeCheck = SafeGetTile(buildPos.X - dir, buildPos.Y);
		if (safeCheck.TileType != ModContent.TileType<YggdrasilBlackRock>())
		{
			return false;
		}
		int randomAddPosX = GenRand.Next(0, 4);
		buildPos.X += randomAddPosX * dir;
		if (dir == 1)
		{
			if (GetUniformTile(buildPos.X, buildPos.Y - 9, 10, 10) == -1)
			{
				QuickBuild(buildPos.X, buildPos.Y - 9, ModAsset.MazeUnderLake_ChestRoom_withLamps10x10_Path, false);
				MazeUnderLake_PlaceYggdrasilBlackRockUntilHitTile(buildPos + new Point(0 + 2, 1));
				MazeUnderLake_PlaceYggdrasilBlackRockUntilHitTile(buildPos + new Point(0 + 7, 1));
				PlaceRectangleAreaOfLiquid_XYWH(buildPos.X, buildPos.Y - 9, 10, 10, LiquidID.Water);
				PlaceLineBlock(buildPos, buildPos + new Point(-randomAddPosX, 0), 1, ModContent.TileType<YggdrasilBlackRock>());
				FillChestXYWH(buildPos.X, buildPos.Y - 9, 10, 10, MazeUnderLake_GetRandomChestContents());
				return true;
			}
		}
		else
		{
			if (GetUniformTile(buildPos.X - 10, buildPos.Y - 9, 10, 10) == -1)
			{
				QuickBuild(buildPos.X - 10, buildPos.Y - 9, ModAsset.MazeUnderLake_ChestRoom_withLamps10x10_Path, false);
				MazeUnderLake_PlaceYggdrasilBlackRockUntilHitTile(buildPos + new Point(-10 + 2, 1));
				MazeUnderLake_PlaceYggdrasilBlackRockUntilHitTile(buildPos + new Point(-10 + 7, 1));
				PlaceRectangleAreaOfLiquid_XYWH(buildPos.X - 10, buildPos.Y - 9, 10, 10, LiquidID.Water);
				PlaceLineBlock(buildPos, buildPos + new Point(randomAddPosX, 0), 1, ModContent.TileType<YggdrasilBlackRock>());
				FillChestXYWH(buildPos.X - 10, buildPos.Y - 9, 10, 10, MazeUnderLake_GetRandomChestContents());
				return true;
			}
		}
		return false;
	}

	public static void MazeUnderLake_PlaceYggdrasilBlackRockUntilHitTile(Point pos)
	{
		for (int dy = 0; dy < 100; dy++)
		{
			var checkPos = pos + new Point(0, dy);
			var checkTile = SafeGetTile(checkPos);
			if (Main.tileSolid[checkTile.TileType] && checkTile.HasTile)
			{
				break;
			}
			checkTile.wall = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
		}
	}

	public static List<Item> MazeUnderLake_GetRandomChestContents()
	{
		List<Item> chestContents = new List<Item>();
		int maxTypes = 6;
		int index = GenRand.Next(maxTypes);
		int timeTried = 0;
		if (MazeUnderLake_YggdrasilBlackChestShrineCount < maxTypes)
		{
			while (MazeUnderLake_YggdrasilBlackRockChestContents.Contains(index))
			{
				timeTried++;
				index = GenRand.Next(maxTypes);
				if (timeTried > 10)
				{
					break;
				}
			}
		}

		MazeUnderLake_YggdrasilBlackRockChestContents.Add(index);
		switch (index)
		{
			case 0:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<AntiCorrosiveSole>(), 1));
				break;
			case 1:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<KelpCurtain.Items.Placeables.AlgaeExtractor_Item>(), 1));
				break;
			case 2:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<BladeOfGreenMoss>(), 1));
				break;
			case 3:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<GreenSungloStaff>(), 1));
				break;
			case 4:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<GreenThornBallLauncher>(), 1));
				break;
			case 5:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<CorrodedPearl>(), 1));
				break;
		}
		chestContents.AddRange(NormalChestContents_KelpCurtain());
		return chestContents;
	}

	/// <summary>
	/// True if the point is close to multiple seeds, which means it's likely to be at the edge of the Voronoi cell and suitable for placing delivery holes.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="seeds"></param>
	/// <param name="edgeThreshold"></param>
	/// <returns></returns>
	public static bool MazeUnderLake_IsEdgePoint(int x, int y, List<Point> seeds, float edgeThreshold = 2.5f)
	{
		// 计算到所有种子点的距离平方（避免开方运算，提高效率）
		List<(int SeedIndex, long DistanceSquared)> distances = new List<(int, long)>();

		for (int i = 0; i < seeds.Count; i++)
		{
			long dx = x - seeds[i].X;
			long dy = y - seeds[i].Y;
			long distSq = dx * dx + dy * dy; // 距离平方
			distances.Add((i, distSq));
		}

		// 排序获取最近的两个种子点
		distances.Sort((a, b) => a.DistanceSquared.CompareTo(b.DistanceSquared));

		// 如果最近两个种子点的距离差小于阈值，则视为边缘
		// 阈值可调整：值越小边缘越细，值越大边缘越粗
		double minDist = Math.Sqrt(distances[0].DistanceSquared);
		double secondMinDist = Math.Sqrt(distances[1].DistanceSquared);

		return (secondMinDist - minDist) < edgeThreshold;
	}

	#endregion

	/// <summary>
	/// 龙潭
	/// </summary>
	public static void DragonPond()
	{
		int sandLayerTopY = (int)(Main.maxTilesY * 0.893f);
		int sandLayerBottomY = (int)(Main.maxTilesY * 0.9f);
		int leftX = (int)(Main.maxTilesX * 0.08f);
		int RightX = (int)(Main.maxTilesX * 0.26f);
		for (int x = leftX; x <= RightX; x++)
		{
			float xDuration = Math.Abs((x - leftX) / (float)(RightX - leftX));
			float deltaY = Main.maxTilesY * 0.003f * MathF.Pow(xDuration, 0.5f);
			sandLayerTopY = (int)(Main.maxTilesY * 0.893f + deltaY);
			for (int y = sandLayerBottomY; y >= sandLayerTopY; y--)
			{
				Tile tile = SafeGetTile(x, y);
				if (!tile.HasTile)
				{
					tile.TileType = (ushort)ModContent.TileType<DecaySandSoil>();
					tile.HasTile = true;
				}
				if (y > sandLayerTopY + 3)
				{
					tile.WallType = (ushort)ModContent.WallType<DecaySandSoilWall>();
				}
			}
		}
		sandLayerTopY = (int)(Main.maxTilesY * 0.893f);
		SmoothTile_XXYY(leftX, sandLayerTopY, RightX, sandLayerBottomY);
		for (int x = leftX; x <= RightX; x++)
		{
			for (int y = sandLayerTopY - 10; y < sandLayerBottomY; y++)
			{
				Tile tile = SafeGetTile(x, y);
				if (tile.HasTile)
				{
					if (GenRand.NextBool(2) && tile.Slope == SlopeType.Solid)
					{
						for (int algeaY = 0; algeaY < 90; algeaY++)
						{
							if (Main.rand.Next(90) < algeaY)
							{
								break;
							}
							Tile algea = SafeGetTile(x, y - algeaY - 1);
							if (!algea.HasTile)
							{
								algea.TileType = (ushort)ModContent.TileType<JadeLakeSargassum>();
								algea.HasTile = true;
							}
						}
					}
					break;
				}
			}
		}
	}

	/// <summary>
	/// 水下宝库
	/// </summary>
	public static void UnderwaterTreasury()
	{
		int center_x = (int)(Main.maxTilesX * 0.32f);
		int center_y = (int)(Main.maxTilesY * 0.895f);
		for (int dy = 0; dy < 1000; dy++)
		{
			var tile = SafeGetTile(center_x, center_y + dy);
			if (tile.HasTile && tile.TileType == ModContent.TileType<StoneScaleWood>())
			{
				center_y += dy - 14;
				break;
			}
		}
		UnderwaterTreasury_Bottom_Room_center_Y = center_y;
		UnderwaterTreasury_GenerateUnderWaterDungeon(center_x, center_y);
	}

	public static void UnderwaterTreasury_GenerateUnderWaterDungeon(int i, int j)
	{
		int cellHeight = 22;
		int cellWidth = 34;
		int cellDistance = 80;
		int tunnelHeight = 16;
		for (int xRoom = 0; xRoom < 1; xRoom++)
		{
			int centerX = i - xRoom * cellDistance;
			int centerY = j;
			UnderwaterTreasury_BrickRoom(centerX, centerY, cellWidth, cellHeight);

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
				UnderwaterTreasury_ConnectWaterErodedBrickTunnel(i - xRoom * cellDistance - (cellWidth - 7), y, i - xRoom * cellDistance - cellDistance + (cellWidth - 7), y, tunnelHeight);
				Tile lamp0 = SafeGetTile(i - xRoom * cellDistance - (cellWidth - 7) - 0, y);
				lamp0.TileType = (ushort)ModContent.TileType<NoctilucentFluoriteLump>();
				lamp0.HasTile = true;
			}
		}
		UnderwaterTreasury_BrickRoom(i - 1 * cellDistance - 20, j - 90, cellWidth + 30, cellHeight);
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

		UnderwaterTreasury_ConnectWaterErodedBrickTunnel_Serrated(new Point(i - 1 * cellDistance - 40, j - 64), new Point(i - 38, j + 9), 12, 8);
	}

	public static void UnderwaterTreasury_BrickRoom(int centerX, int centerY, int halfWidth, int halfHeight)
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
	public static void UnderwaterTreasury_ConnectWaterErodedBrickTunnel(Point p0, Point p1, float width)
	{
		UnderwaterTreasury_ConnectWaterErodedBrickTunnel(p0.X, p0.Y, p1.X, p1.Y, width);
	}

	/// <summary>
	/// Create a tunnel with water-erodeed brick side between 2 points.
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="width"></param>
	public static void UnderwaterTreasury_ConnectWaterErodedBrickTunnel(int x0, int y0, int x1, int y1, float width)
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
	public static void UnderwaterTreasury_ConnectWaterErodedBrickTunnel_Serrated(int x0, int y0, int x1, int y1, float width, float sideThick)
	{
		UnderwaterTreasury_ConnectWaterErodedBrickTunnel_Serrated(new Point(x0, y0), new Point(x1, y1), width, sideThick);
	}

	/// <summary>
	/// Create a tunnel with water-erodeed brick side between 2 points.Traversing altitute by creating a zigzag path.
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="width"></param>
	public static void UnderwaterTreasury_ConnectWaterErodedBrickTunnel_Serrated(Point point0, Point point1, float width, float sideThick)
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

	/// <summary>
	/// The cave of Vampire Mat
	/// </summary>
	public static void VampireMatCave()
	{
		// Tunnel to the Maze;
		int x = (int)(Main.maxTilesX * 0.388f);
		int y = (int)(Main.maxTilesY * 0.894f);
		Point entrance = new Point(x, y);
		List<Point> cellRoom = BFSContinueEmpty(entrance, false, 1536, WaterDeliveryHoleTiles);
		if (cellRoom.Count < 100)
		{
			for (int k = 0; k < 100; k++)
			{
				int dk = GenRand.Next(16) - 8;
				Point newEntrance = entrance + new Point(dk + GenRand.Next(-3, 4), dk * 4 + GenRand.Next(-3, 4));
				cellRoom = BFSContinueEmpty(newEntrance, false, 1536, WaterDeliveryHoleTiles);
				if (cellRoom.Count >= 100)
				{
					break;
				}
			}
		}
		bool hasDeliveryHole = false;
		foreach (var pos in cellRoom)
		{
			var tile = SafeGetTile(pos);
			if (WaterDeliveryHoleTiles.Contains(tile.TileType))
			{
				hasDeliveryHole = true;
				break;
			}
		}
		Point room_centroid = GetCentroid(cellRoom);
		if (!hasDeliveryHole)
		{
			MazeUnderLake_ConnectDeliveryHole(room_centroid.ToWorldCoordinates(), (room_centroid + new Point(1000, 0)).ToWorldCoordinates());
		}

		// Cave of Vampire Mat.
		int cave_x = (int)(Main.maxTilesX * 0.3435f);
		int cave_y = (int)(Main.maxTilesY * 0.894f);
		Point caveCenter = new Point(cave_x, cave_y);
		VampireMatCaveCenter = caveCenter.ToWorldCoordinates();
		List<Point> cave = GetCircleAreaOfTilePosWithRandomNoise(caveCenter, 54, 3);
		List<Point> cave_bound = GetCircleAreaOfTilePosWithRandomNoise(caveCenter, 60, 3);
		foreach (var pos in cave_bound)
		{
			var tile = SafeGetTile(pos);
			if (tile.TileType == ModContent.TileType<DarkLakeBottomMud>() || tile.TileType == ModContent.TileType<MossProneSandSoil>())
			{
				if (cave.Contains(pos))
				{
					ChangeTile(tile, -1, (int)TileChangeState.Forceful);
					if ((pos.ToWorldCoordinates() - VampireMatCaveCenter).Length() < 55 * 16)
					{
						tile.WallType = WallID.None;
					}
					else
					{
						tile.WallType = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
					}
				}
				else
				{
					ChangeTile(tile, ModContent.TileType<YggdrasilBlackRock>(), (int)TileChangeState.Forceful);
					tile.WallType = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
				}
			}
			tile.LiquidType = LiquidID.Water;
			tile.LiquidAmount = 255;
		}

		PlaceLineBlock(caveCenter, room_centroid, 12 * 16, ModContent.TileType<YggdrasilBlackRock>(), (int)TileChangeState.SolidBlock);
		PlaceLineWall(caveCenter, room_centroid, 12 * 16, ModContent.WallType<YggdrasilBlackRockWall>(), (int)TileChangeState.SolidBlock);
		PlaceLineBlock(caveCenter, room_centroid, 8 * 16, -1, (int)TileChangeState.SolidBlock);

		// Board Sign
		Point boardSignBottom = room_centroid + new Point(3, 0);
		for (int k = 0; k < 60; k++)
		{
			var tile = SafeGetTile(boardSignBottom);
			if (IsTileSolid(boardSignBottom))
			{
				break;
			}
			boardSignBottom.Y += 1;
		}
		boardSignBottom.Y -= 1;
		VampireMatCave_BoardSign vMCBS = TileLoader.GetTile(ModContent.TileType<VampireMatCave_BoardSign>()) as VampireMatCave_BoardSign;
		if (vMCBS is not null)
		{
			vMCBS.PlaceAtTileObjectDataOrigin(boardSignBottom.X, boardSignBottom.Y);
		}

		// Hanging Sign
		Point hangingSignTop = room_centroid + new Point(-2, 0);
		hangingSignTop.Y -= CheckSpaceUp(hangingSignTop);
		VampireMatCave_HangingSign vMCHS = TileLoader.GetTile(ModContent.TileType<VampireMatCave_HangingSign>()) as VampireMatCave_HangingSign;
		if (vMCHS is not null)
		{
			vMCHS.PlaceAtTileObjectDataOrigin(hangingSignTop.X, hangingSignTop.Y);
		}

		// Barnacle room.
		Point barnacle_room_center = new Point((int)(Main.maxTilesX * 0.361f), UnderwaterTreasury_Bottom_Room_center_Y);
		Point dungeon_bound = barnacle_room_center;
		for (int dx = 0; dx < 300; dx++)
		{
			var tile = SafeGetTile(dungeon_bound.X, dungeon_bound.Y);
			if (tile.HasTile && tile.TileType == ModContent.TileType<WaterErodedGreenBrick>())
			{
				break;
			}
			dungeon_bound.X--;
		}
		barnacle_room_center = dungeon_bound;
		List<Point> barnaclePolygonRoom = new List<Point>();
		for (int dx = 0; dx <= 40; dx += 5)
		{
			barnaclePolygonRoom.Add(barnacle_room_center + new Point(dx, (int)(-14 - 6 * MathF.Sin(dx / 40f * MathHelper.Pi * 1.5f))));
		}
		for (int dx = 40; dx >= 0; dx -= 5)
		{
			barnaclePolygonRoom.Add(barnacle_room_center + new Point(dx, (int)(14 + 6 * MathF.Sin(dx / 40f * MathHelper.Pi * 1.5f))));
		}
		PlacePolygonAreaOfBlock(barnaclePolygonRoom, -1, (int)TileChangeState.Forceful);
		PlacePolygonBoundOfBlock(barnaclePolygonRoom, ModContent.TileType<SharpBarnacleLayer>(), 48, (int)TileChangeState.NoTile);
		PlacePolygonAreaOfWall(barnaclePolygonRoom, ModContent.WallType<SharpBarnacleWall>(), (int)TileChangeState.Forceful);
		List<Point> barnacleRoom = GetPolygonAreaOfTilePos(barnaclePolygonRoom);
		foreach (var pos in barnacleRoom)
		{
			var tile = SafeGetTile(pos);
			tile.LiquidType = LiquidID.Water;
			tile.LiquidAmount = 255;
		}
		var gGCBCT = TileLoader.GetTile(ModContent.TileType<GiantGhostClawBarnacleCollideTile>()) as GiantGhostClawBarnacleCollideTile;
		if (gGCBCT != null)
		{
			gGCBCT.PlaceOriginAtTopLeft(barnacle_room_center.X, barnacle_room_center.Y - 12);
		}

		// Tunnel to the cave of Vampire Mat.
		List<Vector2> path_to_vampire_cave = new List<Vector2>();
		Vector2 checkPos = (barnacle_room_center + new Point(37, 0)).ToWorldCoordinates();
		Vector2 checkVel = new Vector2(128, 0);
		for (int k = 0; k < 40; k++)
		{
			path_to_vampire_cave.Add(checkPos);
			checkPos += checkVel;
			Vector2 toTarget = caveCenter.ToWorldCoordinates() - checkPos;
			if (toTarget.Length() < 120)
			{
				break;
			}
			toTarget = toTarget.NormalizeSafe();
			checkVel = checkVel * 0.92f + toTarget * 128 * 0.08f;
		}
		PlaceCurveBlock(path_to_vampire_cave, 16 * 16, ModContent.TileType<YggdrasilBlackRock>(), (int)TileChangeState.HasTile);
		PlaceCurveWall(path_to_vampire_cave, 16 * 16, ModContent.WallType<YggdrasilBlackRockWall>(), (int)TileChangeState.HasTile);
		PlaceCurveBlock(path_to_vampire_cave, 12 * 16, -1, (int)TileChangeState.HasTile);
		PlaceCurveLiquid(path_to_vampire_cave, 16 * 16, LiquidID.Water, (int)TileChangeState.HasTile);

		// Add DrainOutlet for player breathing.
		for (int k = 0; k < 5; k++)
		{
			Point tilePos = caveCenter + new Vector2(0, -480).RotatedBy(k / 5f * MathHelper.TwoPi).ToTileCoordinates();
			PlaceFrameImportantTilesAtTileObjectDataOrigin(tilePos, ModContent.TileType<DrainOutlet>());
			for (int dx = -3; dx <= 4; dx++)
			{
				Point platformPos = tilePos + new Point(dx, 4);
				var tile = SafeGetTile(platformPos);
				tile.TileType = (ushort)ModContent.TileType<YggdrasilBlackRock_NoctilucentFluoriteLump_Platform>();
				tile.TileFrameX = 0;
				tile.TileFrameY = 0;
				tile.HasTile = true;
				if (dx == -3)
				{
					tile.TileFrameX = 36;
					tile.TileFrameY = 0;
				}
				if (dx == 4)
				{
					tile.TileFrameX = 18;
					tile.TileFrameY = 0;
				}
			}
		}
		for (int k = 0; k < 21; k++)
		{
			Point tilePos = caveCenter + new Vector2(0, -808).RotatedBy(k / 21f * MathHelper.TwoPi).ToTileCoordinates();
			var tile = SafeGetTile(tilePos);
			if (!tile.HasTile)
			{
				tile.TileType = (ushort)ModContent.TileType<NoctilucentFluoriteLump>();
				tile.HasTile = true;
			}
		}
	}

	/// <summary>
	/// Build a mossy cave(horizental cave, long and flat), with moss side, radius at lease = 10.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="height"></param>
	/// <param name="radius"></param>
	public static void DigAMossyCaveLow(int i, int j, float height, float radius)
	{
		Tile firstCheck = SafeGetTile(i, j);
		if (!firstCheck.HasTile || EmbeddingDepth(i, j, 10) < 10)
		{
			return;
		}
		float hValue = 1f;

		// Right Cave
		int maxStepRight = (int)(radius + 3);
		for (int step = 0; step < maxStepRight; step++)
		{
			if (step > maxStepRight - 13)
			{
				hValue = (maxStepRight - 3 - step) / 10f;
			}
			int x = i + step;
			if (maxStepRight == (int)(radius + 3))
			{
				int xCheck = x + 13;
				float hCheck = 1f;
				if (step + 13 > maxStepRight - 10)
				{
					hCheck = (maxStepRight - step - 13) / 10f;
				}
				for (int h = (int)(-height * hCheck); h <= height * hCheck; h++)
				{
					int y = j + h;
					Tile tile = SafeGetTile(xCheck, y);
					if (!tile.HasTile)
					{
						maxStepRight = step + 13;
						break;
					}
				}
			}
			for (int h = (int)(-height * hValue - 3); h <= height * hValue + 3; h++)
			{
				int y = j + h;
				Tile tile = SafeGetTile(x, y);
				if (MathF.Abs(h) < height * hValue)
				{
					tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
					tile.HasTile = false;
				}
				else
				{
					tile.HasTile = true;
					tile.TileType = (ushort)ModContent.TileType<OldMoss>();
					tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
				}
			}
		}
		hValue = 1f;

		// Left Cave
		int maxStepLeft = (int)(radius + 3);
		for (int step = 0; step < maxStepLeft + 3; step++)
		{
			if (step > maxStepLeft - 13)
			{
				hValue = (maxStepLeft - 3 - step) / 10f;
			}
			int x = i - step;
			if (maxStepLeft == (int)(radius + 3))
			{
				int xCheck = x - 13;
				float hCheck = 1f;
				if (step + 13 > maxStepLeft - 10)
				{
					hCheck = (maxStepLeft - step - 13) / 10f;
				}
				for (int h = (int)(-height * hCheck); h <= height * hCheck; h++)
				{
					int y = j + h;
					Tile tile = SafeGetTile(xCheck, y);
					if (!tile.HasTile)
					{
						maxStepLeft = step + 13;
						break;
					}
				}
			}
			for (int h = (int)(-height * hValue - 3); h <= height * hValue + 3; h++)
			{
				int y = j + h;
				Tile tile = SafeGetTile(x, y);
				if (MathF.Abs(h) < height * hValue)
				{
					tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
					tile.HasTile = false;
				}
				else
				{
					tile.HasTile = true;
					tile.TileType = (ushort)ModContent.TileType<OldMoss>();
					tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
				}
			}
		}
	}

	/// <summary>
	/// Create a tunnel with moss side between 2 points.
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="width"></param>
	public static void ConnectMossyTunnel(int x0, int y0, int x1, int y1, float width)
	{
		int maxStep = (int)(new Vector2(x1, y1) - new Vector2(x0, y0)).Length();
		Vector2 dir = Vector2.Normalize(new Vector2(x1, y1) - new Vector2(x0, y0));
		Vector2 checkPoint = new Vector2(x0, y0);
		float halfWidth = width / 2f;
		for (int s = 0; s < maxStep; s++)
		{
			checkPoint += dir;
			for (int x = (int)(-halfWidth); x < halfWidth; x++)
			{
				for (int y = (int)(-halfWidth); y < halfWidth; y++)
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
						if (tile.HasTile)
						{
							if (checkDir.Length() < halfWidth - 1)
							{
								tile.HasTile = false;
								tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
							}
							else
							{
								tile.HasTile = true;
								tile.TileType = (ushort)ModContent.TileType<OldMoss>();
								tile.WallType = (ushort)ModContent.WallType<OldMossWall>();
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Create a tunnel with moss side between 2 points.
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="width"></param>
	public static void ConnectMossyTunnel(Point p0, Point p1, float width)
	{
		ConnectMossyTunnel(p0.X, p0.Y, p1.X, p1.Y, width);
	}

	public static int CheckWaterSurfaceDown(int x, int y)
	{
		int count = 0;
		int x0 = x;
		int y0 = y;
		if (y0 > Main.maxTilesY || y0 < 0)
		{
			return count;
		}
		while (!SafeGetTile(x0, y0).HasTile && SafeGetTile(x0, y0).LiquidAmount <= 0)
		{
			if (y0 > Main.maxTilesY)
			{
				break;
			}
			y0++;
			count++;
		}
		return count;
	}

	/// <summary>
	/// 刺苔庭园/朽木王庭
	/// </summary>
	public static void MattedMossCourt()
	{
	}

	/// <summary>
	/// 2，3层分界
	/// </summary>
	public static void BuildBoundOf23Stratum()
	{
		int startY = (int)(Main.maxTilesY * 0.75);
		Vector2 checkPos = new Vector2(Main.maxTilesX - 20, startY);
		Vector2 checkVel = new Vector2(0, 16);
		float omega = 0f;
		bool joint = false;
		for (int step = 0; step < 264; step++)
		{
			if (checkPos.Y <= Main.maxTilesY * 0.91)
			{
				CircleTile(checkPos, 30, ModContent.TileType<DragonScaleWood>());
				CircleWall(checkPos, 28, ModContent.WallType<DragonScaleWoodWall>());
			}
			KelpCurtainBiome.StratumBoundCurve.Add(checkPos.ToPoint());
			checkPos += checkVel;
			checkVel = checkVel.RotatedBy(omega);
			if (!joint)
			{
				omega += 0.00013f;
				if (omega > 0.02f)
				{
					joint = true;
				}
			}
			else
			{
				if (omega > -0.018f)
				{
					omega -= 0.006f;
				}
			}
		}
	}

	public static void UnforcablePlaceAreaOfTile(int x0, int y0, int x1, int y1, int type)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				if (!tile.HasTile)
				{
					tile.TileType = (ushort)type;
					tile.HasTile = true;
				}
			}
		}
	}

	public static List<Item> NormalChestContents_KelpCurtain()
	{
		List<Item> contents = new List<Item>();

		// Gold coin
		if (WorldGen.genRand.NextBool(5))
		{
			contents.Add(new Item(setDefaultsToType: ItemID.GoldCoin, WorldGen.genRand.Next(3, 7)));
		}

		// Potion
		int potionType = 1;
		switch (WorldGen.genRand.Next(6))
		{
			case 0:
				potionType = ItemID.WarmthPotion;
				break;
			case 1:
				potionType = ItemID.GillsPotion;
				break;
			case 2:
				potionType = ItemID.WaterWalkingPotion;
				break;
			case 3:
				potionType = ItemID.SpelunkerPotion;
				break;
			case 4:
				potionType = ItemID.MiningPotion;
				break;
			case 5:
				potionType = ItemID.RecallPotion;
				break;
		}
		contents.Add(new Item(setDefaultsToType: potionType, WorldGen.genRand.Next(3, 6)));

		// yggdrasil Amber
		if (WorldGen.genRand.NextBool(5))
		{
			contents.Add(new Item(setDefaultsToType: ModContent.ItemType<DevilHeartIronBar_Item>(), WorldGen.genRand.Next(5, 15)));
		}
		return contents;
	}
}
