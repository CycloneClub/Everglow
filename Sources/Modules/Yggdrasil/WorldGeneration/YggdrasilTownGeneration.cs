using Everglow.CagedDomain.Tiles;
using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.Common.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing.FishingRods;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Pets;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Liquids;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.CyanVine;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood.Furniture;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Walls;
using Everglow.Yggdrasil.YggdrasilTown.Walls.TwilightForest;
using ModLiquidLib.ModLoader;
using ReLogic.Utilities;
using static Everglow.Commons.Utilities.TileUtils;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.WorldGeneration;

public class YggdrasilTownGeneration
{
	public static void BuildYggdrasilTown()
	{
		Initialize();
		Main.statusText = "Yggdrasil Town Bark Cliff...";

		PlaceRectangleAreaOfBlock(20, Main.maxTilesY - 350, 155, Main.maxTilesY, ModContent.TileType<StoneScaleWood>());
		PlaceRectangleAreaOfBlock(Main.maxTilesX - 155, Main.maxTilesY - 350, Main.maxTilesX - 20, Main.maxTilesY, ModContent.TileType<StoneScaleWood>());
		PlaceRectangleAreaOfBlock(0, Main.maxTilesY - 350, Main.maxTilesX, Main.maxTilesY, ModContent.TileType<StoneScaleWood>());

		PlaceRectangleAreaOfWall(20, Main.maxTilesY - 350, 155, Main.maxTilesY, ModContent.WallType<StoneDragonScaleWoodWall>());
		PlaceRectangleAreaOfWall(Main.maxTilesX - 155, Main.maxTilesY - 350, Main.maxTilesX - 20, Main.maxTilesY, ModContent.WallType<StoneDragonScaleWoodWall>());
		PlaceRectangleAreaOfWall(0, Main.maxTilesY - 300, Main.maxTilesX, Main.maxTilesY, ModContent.WallType<StoneDragonScaleWoodWall>());

		Main.statusText = "Filling Midnight Bayou With Mud...";
		BuildMidnightBayou();

		Main.statusText = "Giant Cavenours Pillars...";

		BuildGiantYggdrasilPosts();
		Main.statusText = "Building Twilight Castle...";

		BuildTwilightRelic();
		Main.statusText = "Constructing LampWood Forest Mesa...";

		BuildLampWoodLand();
		Main.statusText = "Planting Twilight Crystal Forest...";

		BuildTwilightLand();
		Main.statusText = "Carving the Heavenly Portal...";
		BuildHeavenlyPortal();

		Main.statusText = "Constructing the Yggdrasil Town Below...";
		BuildMainTown();

		Main.statusText = "Mineralizing...";
		BuildTangledSubmine();

		Main.statusText = "Engraving Cage of Challengers...";
		BuildStoneCageOfChallenges();

		Main.statusText = "Growing Jelly Ball Hotbed...";
		BuildJellyBallHotbed();

		Main.statusText = "Smoothing Yggdrasil Town...";

		SmoothYggdrasilTown();
	}

	// public static int AzureGrottoCenterX;
	public static Vector2 TwilightRelicCenter = new Vector2(1050, 20000);

	public static Vector2 LifeLampWoodRootPos => new Vector2(Main.maxTilesX - 100, Main.maxTilesY - 560);

	public static Point YggdrasilTownTopLeft;

	public static List<YggdrasilTownStreetElement> StreetConstructorsSheet;
	public static List<YggdrasilTownStreetElement> InDoorChineseStyleHangingSheet;
	public static List<int> TwilightBonusList;
	public static List<int> VanillaJuniorGems;
	public static List<Vector2> TwilightBubbleCenters = new List<Vector2>();

	/// <summary>
	/// 初始化
	/// </summary>
	public static void Initialize()
	{
		TwilightBonusList = new List<int>
		{
			ModContent.ItemType<BloodTearCrystalCrown>(),
			ModContent.ItemType<CelesteStoneWaistPendant>(),
			ModContent.ItemType<CyanVineRing>(),
			ModContent.ItemType<SpicyShield>(),
			ModContent.ItemType<DarkMassacreDagger>(),
			ModContent.ItemType<AmberFlowerHook>(),
		};
		AddTwilightLegacyBonusContain(TwilightBonusList, 14);
		VanillaJuniorGems = new List<int>()
		{
			ItemID.Ruby,
			ItemID.Diamond,
			ItemID.Amber,
			ItemID.Sapphire,
			ItemID.Topaz,
			ItemID.Amethyst,
			ItemID.Emerald,
		};
	}

	/// <summary>
	/// 黑沉沼泽
	/// </summary>
	public static void BuildMidnightBayou()
	{
		Point center = new Point(1400, Main.maxTilesY - 360);
		int radious = 300;

		// 沼泽本体
		for (int x = (int)(center.X - radious * 1.5); x < (int)(center.X + radious * 1.5); x++)
		{
			for (int y = center.Y - 60; y < center.Y + radious; y++)
			{
				Tile tile = SafeGetTile(x, y);
				float color = PerlinPixel2[Math.Clamp((int)(x - center.X + radious * 1.5f) / 3, 0, 1024), Math.Clamp((y - center.Y + radious) / 2, 0, 1024)] / 255f;
				float distance = new Vector2((x - center.X) * 0.6667f, y - center.Y).Length() / 200f;
				if (color + distance > 1.5f)
				{
					tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
					tile.HasTile = true;
					if (color + distance > 1.6f)
					{
						tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
					}
				}
				else
				{
					tile.ClearEverything();
				}
				if (y > center.Y + 30)
				{
					if (!tile.HasTile && tile.LiquidAmount == 0)
					{
						tile.LiquidType = LiquidLoader.LiquidType<DarkSludgeLiquid>();
						tile.LiquidAmount = byte.MaxValue;
					}
				}
			}
		}

		// 确定左右界限
		// if (AzureGrottoCenterX > 600)
		// {
		// deltaX = -40;
		// }
		int leftBound = 1250;
		int rightBound = 1440;
		int startY = Main.maxTilesY - 368;
		KillRectangleAreaOfTile(leftBound, startY - 10, rightBound, startY);

		// 源晶塔
		// PlaceFrameImportantTiles(1395, startY - 37, 8, 12, ModContent.TileType<OriginPylon>());
		PlaceFrameImportantTiles(1257, startY, 3, 1, ModContent.TileType<StoneBridgeTile>(), 0, 18);
		PlaceFrameImportantTiles(1253, startY + 7, 7, 1, ModContent.TileType<StoneBridgeTile>(), 36, 36);
		for (int x = leftBound + 5; x < rightBound - 5; x++)
		{
			if (x % 20 == 0)
			{
				PlaceFrameImportantTiles(x, startY, 20, 1, ModContent.TileType<StoneBridgeTile>(), 0, 0);

				PlaceFrameImportantTiles(x, startY + 7, 20, 1, ModContent.TileType<StoneBridgeTile>(), 36, 36);
			}
		}
		PlaceFrameImportantTiles(1440, startY, 8, 1, ModContent.TileType<StoneBridgeTile>(), 0, 54);

		PlaceFrameImportantTiles(1440, startY - 3, 4, 3, ModContent.TileType<DilapidatedDangerSigns4x3>(), 0, 0);

		PlaceFrameImportantTiles(1440, startY + 7, 12, 1, ModContent.TileType<StoneBridgeTile>(), 36, 36);

		PlaceFrameImportantTiles(1470, startY + 7, 16, 1, ModContent.TileType<StoneBridgeTile>(), 0, 72);
	}

	/// <summary>
	/// 巨型天穹石柱
	/// </summary>
	public static void BuildGiantYggdrasilPosts()
	{
		int x0CoordPerlin = GenRand.Next(1024);
		int y0CoordPerlin = GenRand.Next(1024);
		int upBound = Main.maxTilesY - 2080;

		// 7次循环
		for (int i = 2; i < 9; i++)
		{
			Vector2 point = new Vector2(GenRand.Next(-40, 40) + i * 200, Main.maxTilesY - 460);
			int radious = GenRand.Next(50, 60);
			if (i == 3 || i == 6)
			{
				radious = (int)(radious * 2.5);
			}
			Vector2 velocity = new Vector2(0, -6).RotatedBy(GenRand.NextFloat(-0.2f, 0.2f));

			// 魔法数字特殊处理
			if (i == 2)
			{
				velocity = new Vector2(0, -6).RotatedBy(-0.3);
			}
			if (i == 8)
			{
				velocity = new Vector2(0, -6).RotatedBy(0.3);
			}
			if (i == 4)
			{
				point.X += 100;
			}
			if (i == 5)
			{
				continue;
			}
			if (i == 6)
			{
				point.X += 100;
			}
			List<Vector2> oldPoints = new List<Vector2>();
			for (int step = 0; step < 400; step++)
			{
				point += velocity;
				oldPoints.Add(point);
				if (SafeGetTile((int)(point + velocity * 30).X, (int)(point + velocity).Y).HasTile)
				{
					point -= velocity;
					velocity.X *= -1;
					point += velocity;
				}
				if (point.X < 50 || point.X > 1950)
				{
					point -= velocity;
					velocity.X *= -1;
					point += velocity;
				}
				if (point.Y < upBound)
				{
					break;
				}

				// 隧道制造洞穴
				if (step > 50)
				{
					Vector2 point2 = point - velocity * 24 + velocity.RotatedBy(GenRand.NextFloat(MathHelper.TwoPi)) * 16;
					DigTunnel(point2.X, point2.Y, GenRand.NextFloat(-1, 1), GenRand.NextFloat(0, 1), GenRand.Next(27, (int)(radious * 1.1f)), GenRand.Next(3, (int)(radious * 0.14f)));
					if (radious < 60)
					{
						point2 = point - velocity * 24 + velocity.RotatedBy(GenRand.NextFloat(MathHelper.TwoPi)) * 6;
						DigTunnel(point2.X, point2.Y, GenRand.NextFloat(-1, 1), GenRand.NextFloat(0, 1), GenRand.Next(27, (int)(radious * 2.4f)), GenRand.Next(3, (int)(radious * 0.14f)));
					}
				}

				// 通过圆面积累加堆砌出柱体
				for (int dx = -radious; dx <= radious; dx++)
				{
					for (int dy = -radious / 8; dy <= radious / 8; dy++)
					{
						Vector2 v0 = new Vector2(dx, dy);
						int x = (int)point.X + dx;
						int y = (int)point.Y + dy;

						float aValue = GetLargeSmokeTexturePixelB(x + x0CoordPerlin, y + y0CoordPerlin) * 32f;
						if (v0.Length() < radious - aValue)
						{
							Tile tile = SafeGetTile(x, y);
							tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
							if (v0.Length() < radious - aValue - 2)
							{
								tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
							}
							tile.HasTile = true;
						}
					}
				}
			}
			for (int a = 0; a < oldPoints.Count; a++)
			{
				MinerizationCyanOreCircle(oldPoints[a], radious);
				if (a % 10 == 0)
				{
					int leftX = (int)(oldPoints[a].X - radious);
					int rightX = (int)(oldPoints[a].X + radious);
					int upY = (int)(oldPoints[a].Y - radious);
					int downY = (int)(oldPoints[a].Y + radious);

					// SmoothTile_XXYY(leftX, upY, rightX, downY);
				}
				FillLiquid(oldPoints[a] * 16 + new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(16))).RotatedByRandom(MathHelper.TwoPi) * radious);
			}
		}
	}

	/// <summary>
	/// 登天石拱
	/// </summary>
	public static void BuildHeavenlyPortal()
	{
		Point center = new Point(1400, Main.maxTilesY - 360);
		int radious = 300;

		for (int x = (int)(center.X - radious * 1.5); x < (int)(center.X + radious * 1.5); x++)
		{
			for (int y = center.Y - radious; y < center.Y - 60; y++)
			{
				Tile tile = SafeGetTile(x, y);
				float color = PerlinPixel2[Math.Clamp((int)(x - center.X + radious * 1.5f) / 3, 0, 1024), Math.Clamp((y - center.Y + radious) / 2, 0, 1024)] / 255f;
				float distance = new Vector2((x - center.X) * 0.6667f, y - center.Y).Length() / 200f;
				if (color + distance is > 1.5f and < 2f)
				{
					tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
					tile.HasTile = true;
					if (color + distance > 1.6f)
					{
						tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
					}
				}
				else if (color + distance < 1.5f)
				{
					tile.ClearEverything();
				}
			}
		}
	}

	/// <summary>
	/// 靛琉璃海, Expired
	/// </summary>
	public static void BuildAzureGrotto()
	{
	}

	/// <summary>
	/// 千回矿道
	/// </summary>
	public static void BuildTangledSubmine()
	{
		Minerization(40, Main.maxTilesY - 820, Main.maxTilesX / 2 + 50, Main.maxTilesY - 50);
		for (int i = 0; i < 200; i++)
		{
			Point pos = new Point(Main.rand.Next(40, Main.maxTilesX / 2 + 50), Main.rand.Next(Main.maxTilesY - 700, Main.maxTilesY - 20));
			FillLiquid(pos);
		}
	}

	/// <summary>
	/// 青缎矿化
	/// </summary>
	/// <param name="leftX"></param>
	/// <param name="upY"></param>
	/// <param name="rightX"></param>
	/// <param name="downY"></param>
	public static void Minerization(int leftX, int upY, int rightX, int downY)
	{
		float area = (downY - upY) * (rightX - leftX) / 180000f;
		for (int x = 0; x < 110 * area; x++)
		{
			DigTunnelAvoidYggdrasilTown(GenRand.NextFloat(leftX, rightX), GenRand.NextFloat(upY, downY), GenRand.NextFloat(-1, 1), GenRand.NextFloat(0, 1), GenRand.Next(27, 72), GenRand.Next(3, 7), ModContent.TileType<StoneScaleWood>(), false, ModContent.WallType<StoneDragonScaleWoodWall>());
		}
		for (int x = 0; x < 30 * area; x++)
		{
			DigTunnelAvoidYggdrasilTown(GenRand.NextFloat(leftX, rightX), GenRand.NextFloat(upY, downY), GenRand.NextFloat(-1, 1), GenRand.NextFloat(0, 1), GenRand.Next(27, 72), GenRand.Next(3, 7), ModContent.TileType<StoneScaleWood>(), false, ModContent.WallType<StoneDragonScaleWoodWall>());
		}
		for (int x = 0; x < 30 * area; x++)
		{
			DigTunnelAvoidYggdrasilTown(GenRand.NextFloat(leftX, rightX), GenRand.NextFloat(upY, downY), GenRand.NextFloat(-1, 1), GenRand.NextFloat(-1, 1), GenRand.Next(81, 144), GenRand.Next(8, 12), ModContent.TileType<StoneScaleWood>(), false, ModContent.WallType<StoneDragonScaleWoodWall>());
		}
		for (int x = leftX; x < rightX; x++)
		{
			for (int y = upY; y < downY; y++)
			{
				Tile tile = SafeGetTile(x, y);
				Tile tileUp = SafeGetTile(x, y - 1);
				Tile tileUp1 = SafeGetTile(x - 1, y - 1);
				Tile tileUp2 = SafeGetTile(x - 2, y - 1);
				Tile tileUp3 = SafeGetTile(x - 3, y - 1);
				Tile tileUp4 = SafeGetTile(x - 4, y - 1);
				Tile tileMiddleUpUp = SafeGetTile(x - 2, y - 2);
				Tile tileLeft1 = SafeGetTile(x - 1, y);
				Tile tileLeft2 = SafeGetTile(x - 2, y);
				Tile tileLeft3 = SafeGetTile(x - 3, y);
				Tile tileLeft4 = SafeGetTile(x - 4, y);
				if (tile.LiquidAmount > 0 || tileUp.LiquidAmount > 0 || tileUp1.LiquidAmount > 0 || tileUp2.LiquidAmount > 0 || tileUp3.LiquidAmount > 0 || tileUp4.LiquidAmount > 0 || tileLeft1.LiquidAmount > 0 || tileLeft1.LiquidAmount > 0 || tileLeft2.LiquidAmount > 0 || tileLeft3.LiquidAmount > 0 || tileLeft4.LiquidAmount > 0)
				{
					continue;
				}
				if (tileLeft1.HasTile && tileLeft2.HasTile && tileLeft3.HasTile && tileLeft4.HasTile && tile.HasTile && !tileUp.HasTile && !tileUp4.HasTile! && !tileMiddleUpUp.HasTile)
				{
					if (tileLeft1.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft2.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft3.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft4.TileType == ModContent.TileType<StoneScaleWood>() && tile.TileType == ModContent.TileType<StoneScaleWood>())
					{
						if (GenRand.NextBool(3))
						{
							PlaceLargeCyanVineOre(x - 4, y - 3);
						}
					}
				}
				if (tileLeft1.HasTile && tileLeft2.HasTile && tile.HasTile && !tileUp2.HasTile && !tileMiddleUpUp.HasTile)
				{
					if (tileLeft1.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft2.TileType == ModContent.TileType<StoneScaleWood>() && tile.TileType == ModContent.TileType<StoneScaleWood>())
					{
						if (GenRand.NextBool(12))
						{
							PlaceMiddleCyanVineOre(x - 2, y - 2);
						}
					}
				}
				if (tileLeft1.HasTile && tileLeft2.HasTile && tile.HasTile && !tileUp1.HasTile && !tileMiddleUpUp.HasTile)
				{
					if (tileLeft1.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft2.TileType == ModContent.TileType<StoneScaleWood>() && tile.TileType == ModContent.TileType<StoneScaleWood>())
					{
						if (GenRand.NextBool(12))
						{
							PlaceSmallCyanVineOre(x - 2, y - 2);
						}
					}
				}
				if (!tileLeft1.HasTile && tileUp.HasTile && tileUp1.HasTile && tileUp2.HasTile)
				{
					if (tileUp1.TileType == ModContent.TileType<StoneScaleWood>() && tileUp2.TileType == ModContent.TileType<StoneScaleWood>() && tileUp.TileType == ModContent.TileType<StoneScaleWood>())
					{
						if (GenRand.NextBool(6))
						{
							PlaceSmallUpCyanVineOre(x - 2, y);
						}
					}
				}
				if (!tile.HasTile && !tileLeft4.HasTile && tileUp.HasTile && tileUp1.HasTile && tileUp2.HasTile && tileUp3.HasTile && tileUp4.HasTile)
				{
					if (tileUp1.TileType == ModContent.TileType<StoneScaleWood>() && tileUp2.TileType == ModContent.TileType<StoneScaleWood>() && tileUp.TileType == ModContent.TileType<StoneScaleWood>())
					{
						if (GenRand.NextBool(3))
						{
							PlaceLargeUpCyanVineOre(x - 4, y);
						}
					}
				}
			}
		}
		for (int x = leftX; x < rightX; x++)
		{
			for (int y = upY; y < downY; y++)
			{
				if (GenRand.NextBool(1500))
				{
					Tile tile = SafeGetTile(x, y);
					Tile tileUp = SafeGetTile(x, y - 1);
					Tile tileDown = SafeGetTile(x, y + 1);
					Tile tileLeft = SafeGetTile(x - 1, y);
					Tile tileRight = SafeGetTile(x + 1, y);
					if (tile.HasTile && tile.TileType == ModContent.TileType<StoneScaleWood>())
					{
						if (tileUp.LiquidAmount <= 0 && tileDown.LiquidAmount <= 0 && tileLeft.LiquidAmount <= 0 && tileRight.LiquidAmount <= 0)
						{
							WorldGen.TileRunner(x, y, GenRand.NextFloat(2f, 6f), GenRand.Next(4, 18), ModContent.TileType<CyanVineStone>());
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 青缎矿化圆
	/// </summary>
	/// <param name="center"></param>
	/// <param name="range"></param>
	public static void MinerizationCyanOreCircle(Vector2 center, float range)
	{
		int leftX = (int)(center.X - range);
		int rightX = (int)(center.X + range);
		int upY = (int)(center.Y - range);
		int downY = (int)(center.Y + range);
		for (int x = leftX; x < rightX; x++)
		{
			for (int y = upY; y < downY; y++)
			{
				if ((center - new Vector2(x, y)).Length() <= range)
				{
					Tile tile = SafeGetTile(x, y);
					Tile tileUp = SafeGetTile(x, y - 1);
					Tile tileUp1 = SafeGetTile(x - 1, y - 1);
					Tile tileUp2 = SafeGetTile(x - 2, y - 1);
					Tile tileUp3 = SafeGetTile(x - 3, y - 1);
					Tile tileUp4 = SafeGetTile(x - 4, y - 1);
					Tile tileLeft1 = SafeGetTile(x - 1, y);
					Tile tileLeft2 = SafeGetTile(x - 2, y);
					Tile tileLeft3 = SafeGetTile(x - 3, y);
					Tile tileLeft4 = SafeGetTile(x - 4, y);
					if (tile.LiquidAmount > 0 || tileUp.LiquidAmount > 0 || tileUp1.LiquidAmount > 0 || tileUp2.LiquidAmount > 0 || tileUp3.LiquidAmount > 0 || tileUp4.LiquidAmount > 0 || tileLeft1.LiquidAmount > 0 || tileLeft1.LiquidAmount > 0 || tileLeft2.LiquidAmount > 0 || tileLeft3.LiquidAmount > 0 || tileLeft4.LiquidAmount > 0)
					{
						continue;
					}
					if (tileLeft1.HasTile && tileLeft2.HasTile && tileLeft3.HasTile && tileLeft4.HasTile && tile.HasTile && !tileUp.HasTile && !tileUp4.HasTile)
					{
						if (tileLeft1.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft2.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft3.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft4.TileType == ModContent.TileType<StoneScaleWood>() && tile.TileType == ModContent.TileType<StoneScaleWood>())
						{
							if (GenRand.NextBool(3))
							{
								if (CanPlaceMultiAtTopTowardsUpRight(x - 4, y, 5, 3))
								{
									PlaceLargeCyanVineOre(x - 4, y - 3);
								}
							}
						}
					}
					if (tileLeft1.HasTile && tileLeft2.HasTile && tile.HasTile && !tileUp2.HasTile)
					{
						if (tileLeft1.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft2.TileType == ModContent.TileType<StoneScaleWood>() && tile.TileType == ModContent.TileType<StoneScaleWood>())
						{
							if (GenRand.NextBool(12))
							{
								if (CanPlaceMultiAtTopTowardsUpRight(x - 2, y + 1, 4, 3))
								{
									PlaceMiddleCyanVineOre(x - 2, y - 2);
								}
							}
						}
					}
					if (tileLeft1.HasTile && tileLeft2.HasTile && tile.HasTile && !tileUp1.HasTile)
					{
						if (tileLeft1.TileType == ModContent.TileType<StoneScaleWood>() && tileLeft2.TileType == ModContent.TileType<StoneScaleWood>() && tile.TileType == ModContent.TileType<StoneScaleWood>())
						{
							if (GenRand.NextBool(12))
							{
								if (CanPlaceMultiAtTopTowardsUpRight(x - 2, y, 3, 2))
								{
									PlaceSmallCyanVineOre(x - 2, y - 2);
								}
							}
						}
					}
					if (!tileLeft1.HasTile && tileUp.HasTile && tileUp1.HasTile && tileUp2.HasTile)
					{
						if (tileUp1.TileType == ModContent.TileType<StoneScaleWood>() && tileUp2.TileType == ModContent.TileType<StoneScaleWood>() && tileUp.TileType == ModContent.TileType<StoneScaleWood>())
						{
							if (GenRand.NextBool(6))
							{
								if (CanPlaceMultiBenethTowardsDownRight(x - 2, y - 1, 3, 2))
								{
									PlaceSmallUpCyanVineOre(x - 2, y);
								}
							}
						}
					}
					if (!tile.HasTile && !tileLeft4.HasTile && tileUp.HasTile && tileUp1.HasTile && tileUp2.HasTile && tileUp3.HasTile && tileUp4.HasTile)
					{
						if (tileUp1.TileType == ModContent.TileType<StoneScaleWood>() && tileUp2.TileType == ModContent.TileType<StoneScaleWood>() && tileUp.TileType == ModContent.TileType<StoneScaleWood>())
						{
							if (GenRand.NextBool(3))
							{
								if (CanPlaceMultiBenethTowardsDownRight(x - 4, y - 1, 5, 3))
								{
									PlaceLargeUpCyanVineOre(x - 4, y);
								}
							}
						}
					}
				}
			}
		}
		for (int x = leftX; x < rightX; x++)
		{
			for (int y = upY; y < downY; y++)
			{
				if ((center - new Vector2(x, y)).Length() <= range)
				{
					if (GenRand.NextBool(3000))
					{
						Tile tile = SafeGetTile(x, y);
						Tile tileUp = SafeGetTile(x, y - 1);
						Tile tileDown = SafeGetTile(x, y + 1);
						Tile tileLeft = SafeGetTile(x - 1, y);
						Tile tileRight = SafeGetTile(x + 1, y);
						if (tile.HasTile && tile.TileType == ModContent.TileType<StoneScaleWood>())
						{
							if (tileUp.LiquidAmount <= 0 && tileDown.LiquidAmount <= 0 && tileLeft.LiquidAmount <= 0 && tileRight.LiquidAmount <= 0)
							{
								WorldGen.TileRunner(x, y, GenRand.NextFloat(2f, 6f), GenRand.Next(4, 18), ModContent.TileType<CyanVineStone>());
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Digtunnel Avoid YggdrasilTown.
	/// </summary>
	/// <param name="X"></param>
	/// <param name="Y"></param>
	/// <param name="xDir"></param>
	/// <param name="yDir"></param>
	/// <param name="Steps"></param>
	/// <param name="Size"></param>
	/// <param name="Wet"></param>
	/// <returns></returns>
	public static Vector2D DigTunnelAvoidYggdrasilTown(double x, double y, double xDir, double yDir, int steps, int size, int type, bool wet = false, int wallType = -1)
	{
		Rectangle townArea = new Rectangle(230, Main.maxTilesY - 600, 706, 275);
		int embedTownDepth = 0;
		double startX = x;
		double startY = y;
		try
		{
			double xVel = 0.0;
			double yVel = 0.0;
			double checkSize = size;
			startX = Utils.Clamp(startX, checkSize + 1.0, Main.maxTilesX - checkSize - 1.0);
			startY = Utils.Clamp(startY, checkSize + 1.0, Main.maxTilesY - checkSize - 1.0);
			for (int i = 0; i < steps; i++)
			{
				for (int j = (int)(startX - checkSize); j <= startX + checkSize; j++)
				{
					for (int k = (int)(startY - checkSize); k <= startY + checkSize; k++)
					{
						if (Math.Abs(j - startX) + Math.Abs(k - startY) < checkSize * (1.0 + GenRand.Next(-10, 11) * 0.005) && j >= 20 && j < Main.maxTilesX - 20 && k >= 20 && k < Main.maxTilesY - 20)
						{
							if (j >= townArea.X && j <= townArea.X + townArea.Width && k >= townArea.Y && k <= townArea.Y + townArea.Height)
							{
								embedTownDepth++;
								Vector2 distanceOval = townArea.Center.ToVector2() - new Vector2(j, k);
								distanceOval.Y *= townArea.Width / (float)townArea.Height;
								if (distanceOval.Length() < 300)
								{
									return new Vector2D(startX, startY);
								}
							}
							Tile tile = SafeGetTile(j, k);
							if (ChestSafe(j, k))
							{
								if (tile.TileType == type)
								{
									tile.active(active: false);
									if (wet)
									{
										tile.liquid = byte.MaxValue;
									}
									if (wallType != -1)
									{
										tile.WallType = (ushort)wallType;
									}
								}
								else
								{
									return new Vector2D(startX, startY);
								}
							}
							Tile tileSafe = SafeGetTile((int)(j + (xVel + xDir) * 3), (int)(k + (yVel + yDir) * 3));
							if (tile.TileType != type)
							{
								return new Vector2D(startX, startY);
							}
							if (embedTownDepth > 30)
							{
								return new Vector2D(startX, startY);
							}
						}
						if (!(j >= 20 && j < Main.maxTilesX - 20 && k >= 20 && k < Main.maxTilesY - 20))
						{
							return new Vector2D(startX, startY);
						}
					}
				}

				checkSize += GenRand.Next(-50, 51) * 0.03;
				if (checkSize < size * 0.6)
				{
					checkSize = size * 0.6;
				}

				if (checkSize > size * 2)
				{
					checkSize = size * 2;
				}

				xVel += GenRand.Next(-20, 21) * 0.01;
				yVel += GenRand.Next(-20, 21) * 0.01;
				if (xVel < -1.0)
				{
					xVel = -1.0;
				}

				if (xVel > 1.0)
				{
					xVel = 1.0;
				}

				if (yVel < -1.0)
				{
					yVel = -1.0;
				}

				if (yVel > 1.0)
				{
					yVel = 1.0;
				}

				startX += (xDir + xVel) * 0.6;
				startY += (yDir + yVel) * 0.6;
			}
		}
		catch
		{
		}

		return new Vector2D(startX, startY);
	}

	/// <summary>
	/// 石化古道, Expired
	/// </summary>
	public static void BuildFossilizedMineRoad()
	{
	}

	/// <summary>
	/// 下天穹镇
	/// </summary>
	public static void BuildMainTown()
	{
		// 圆壳罩住天穹镇
		Point topLeft = new Point(20, Main.maxTilesY - 800);
		int length = 1100;
		int height = 490;
		int x0CoordPerlin = GenRand.Next(1024);
		int y0CoordPerlin = GenRand.Next(1024);
		float thick = 30;
		for (int i = 0; i < length; i++)
		{
			float value = 1 - i / (float)length;
			value = MathF.Pow(value, 0.4f) * height;
			if (i > 300)
			{
				thick += (i - 300) / 800f * 0.25f;
			}
			for (int j = 0; j < height; j++)
			{
				Point pos = new Point(i, j) + topLeft;
				Tile tile = SafeGetTile(pos);
				float noiseValueUp = GetLargeSmokeTexturePixelG(i + x0CoordPerlin, j + y0CoordPerlin) * 0.5f;

				// float noiseValueDown = PerlinPixelG[(i + x0CoordPerlin) % 1024, (j + 50 + y0CoordPerlin) % 1024] / 255f * 0.5f;
				if (ChestSafe(pos.X, pos.Y))
				{
					if (j > height - value + noiseValueUp * 15)
					{
						tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
						tile.HasTile = true;
					}
					if (j > height - value + noiseValueUp * 15 + 3/* && j < height - value + thick + noiseValueDown * 25 - 3*/)
					{
						tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
					}
				}
			}
		}
		length = 400;
		height = 400;
		for (int i = 0; i < length; i++)
		{
			float value = i / (float)length;
			value = height - MathF.Pow(value, 1.8f) * height;
			for (int j = 0; j < height; j++)
			{
				Point pos = new Point(i, j) + topLeft;
				Tile tile = SafeGetTile(pos);
				float noiseValue = GetLargeSmokeTexturePixelG(i + x0CoordPerlin, j + y0CoordPerlin) * 0.5f;
				if (j >= height - value + noiseValue * 25)
				{
					tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
					tile.HasTile = true;
				}
				if (j >= height - value + noiseValue * 25 + 3)
				{
					tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
				}
			}
		}
		YggdrasilTownTopLeft = new Point(230, Main.maxTilesY - 600);
		QuickBuild(YggdrasilTownTopLeft.X, YggdrasilTownTopLeft.Y, ModAsset.YggdrasilTown_New_706x275_Path);

		// Railway
		for (int t = 0; t <= 120; t++)
		{
			int dx = t * 3;
			int x = YggdrasilTownTopLeft.X + 706 + dx;
			int y = YggdrasilTownTopLeft.Y + 189;
			if (t == 120)
			{
				y -= 15;
				QuickBuild(x, y, ModAsset.OriginalPylonStation231x31_Path);
				break;
			}
			var checkPos = new Point(x + 2, y + 16);
			var checkTile = SafeGetTile(checkPos);
			if (checkTile.HasTile)
			{
				if (t % 15 == 14)
				{
					QuickBuild(x, y, ModAsset.MetroTunnelPiece_WithLamp3x16_Path);
				}
				else
				{
					QuickBuild(x, y, ModAsset.MetroTunnelPiece3x16_Path);
				}
			}
			else
			{
				if (t % 9 == 4)
				{
					QuickBuild(x, y, ModAsset.MetroBridgePiece_withPier3x15_Path);
					for (int dy = 0; dy < 300; dy++)
					{
						var tile = SafeGetTile(x + 1, y + dy + 15);
						if (tile.WallType > WallID.None)
						{
							break;
						}
						else
						{
							ChangeWall(tile, ModContent.WallType<IronTrackScaffolding>(), (int)TileChangeState.Forceful);
						}
					}
				}
				else
				{
					QuickBuild(x, y, ModAsset.MetroBridgePiece3x15_Path);
				}
			}
		}
	}

	/// <summary>
	/// 球冻温床
	/// </summary>
	public static void BuildJellyBallHotbed()
	{
		int upBound = Main.maxTilesY - 500;
		int bottomBound = Main.maxTilesY - 40;
		int leftBound = Main.maxTilesX - 350;
		int rightBound = Main.maxTilesX;
		Vector2 Center = new Vector2((leftBound + rightBound) / 2f, (upBound + bottomBound) / 2f - 120f);
		float a = 150;
		float b = 83;
		for (int x = leftBound; x <= rightBound; x++)
		{
			for (int y = upBound; y <= bottomBound; y++)
			{
				Tile tile = SafeGetTile(x, y);
				Vector2 toCenter = Center - new Vector2(x, y);
				float r = a - b * MathF.Sin(toCenter.ToRotation());
				toCenter.Y /= 1.2f;
				float valueNoise = GetLargeSmokeTexturePixelG((int)((toCenter.ToRotation() + MathHelper.TwoPi + 0.5f) * 400), (int)(toCenter.Length() * 0.7f));
				float valueNoiseSecretion = GetLargeSmokeTexturePixelB((int)((toCenter.ToRotation() + MathHelper.TwoPi + 0.5f) * 400), (int)(toCenter.Length() * 0.6f));
				float clearRange = 90f;
				float boundThick = 60f;
				if (toCenter.Length() > r)
				{
					valueNoise += 1;
					valueNoiseSecretion += 1;
				}
				else if (toCenter.Length() > r - boundThick)
				{
					valueNoise += 1 + (toCenter.Length() - r) / boundThick;
					valueNoiseSecretion += 1 + (toCenter.Length() - r) / boundThick;
				}
				else if (toCenter.Length() < clearRange)
				{
					valueNoise -= (clearRange - toCenter.Length()) / clearRange;
					valueNoiseSecretion -= (clearRange - toCenter.Length()) / clearRange;
				}
				if (valueNoise < 0.6f)
				{
					tile.HasTile = false;
				}
				else if (valueNoise <= 1)
				{
					tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
					tile.HasTile = true;
				}
				if (valueNoiseSecretion >= 0.5f && !tile.HasTile && valueNoise <= 1)
				{
					tile.TileType = (ushort)ModContent.TileType<JellyBallSecretion>();
					tile.HasTile = true;
				}
				float valueNoise2 = GetLargeSmokeTexturePixelG(x, y);
				if (y < upBound + 30)
				{
					valueNoise2 += (upBound + 30 - y) / 30f;
				}
				if (toCenter.Y > -120)
				{
					if (valueNoise > 1 && x > Center.X)
					{
						if (valueNoise2 < 0.5f)
						{
							if (!tile.HasTile)
							{
								tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
								tile.HasTile = true;
							}
						}
					}
				}
			}
		}

		for (int x = leftBound; x <= rightBound; x++)
		{
			for (int y = upBound; y <= bottomBound; y++)
			{
				Vector2 toCenter = Center - new Vector2(x, y);
				toCenter.Y /= 1.2f;
				float r = a - b * MathF.Sin(toCenter.ToRotation());
				if (toCenter.Length() <= r)
				{
					if (GenRand.NextBool(22))
					{
						CrawlCarpetOfTile(x, y, GenRand.Next(20, 70), 5, ModContent.TileType<JellyBallSecretion>(), GenRand.NextBool());
					}
				}
			}
		}

		// SmoothTile_XXYY(leftBound, upBound, rightBound, bottomBound);
		string mapIOPath = ModAsset.HotbedObervatory_66x44_Path;
		QuickBuild(rightBound - 100, (int)Center.Y, mapIOPath);

		for (int x = leftBound; x <= rightBound; x++)
		{
			for (int y = upBound; y <= bottomBound; y++)
			{
				Tile tile = SafeGetTile(x, y);
				Vector2 toCenter = Center - new Vector2(x, y);
				float r = a - b * MathF.Sin(toCenter.ToRotation());
				toCenter.Y /= 1.2f;
				float valueNoise = GetLargeSmokeTexturePixelG((int)((toCenter.ToRotation() + MathHelper.TwoPi + 0.5f) * 400), (int)(toCenter.Length() * 0.7f));
				float valueNoiseWall = CellPixel[(int)((toCenter.ToRotation() + MathHelper.TwoPi + 0.5f) * 400) % 512, (int)(toCenter.Length() * 0.7f) % 512] / 255f;
				float valueNoiseWallWood = GetLargeSmokeTexturePixelR((int)((toCenter.ToRotation() + MathHelper.TwoPi + 0.5f) * 400), (int)(toCenter.Length() * 0.7f));
				float clearRange = 90f;
				float boundThick = 60f;
				if (toCenter.Length() > r)
				{
					valueNoise += 1;
				}
				else if (toCenter.Length() > r - boundThick)
				{
					valueNoise += 1 + (toCenter.Length() - r) / boundThick;
				}
				else if (toCenter.Length() < clearRange)
				{
					valueNoise -= (clearRange - toCenter.Length()) / clearRange;
				}
				if (valueNoiseWall < 0.5f)
				{
					if (valueNoise <= 1 && (tile.WallType == (ushort)ModContent.WallType<StoneDragonScaleWoodWall>() || tile.WallType == (ushort)ModContent.WallType<JellyBallSecretionWall>() || tile.WallType == (ushort)ModContent.WallType<DarkForestSoilWall>()))
					{
						tile.WallType = WallID.None;
					}
				}
				else if (valueNoise <= 1)
				{
					tile.WallType = (ushort)ModContent.WallType<JellyBallSecretionWall>();
				}
				if (valueNoiseWallWood >= 0.5f && valueNoise <= 1)
				{
					tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
				}
				float valueNoise2 = GetLargeSmokeTexturePixelG(x, y);
				if (y < upBound + 30)
				{
					valueNoise2 += (upBound + 30 - y) / 30f;
				}
				if (toCenter.Y > -120)
				{
					if (valueNoise > 1 && x > Center.X)
					{
						if (valueNoise2 < 0.45f)
						{
							if (tile.WallType == WallID.None)
							{
								tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 灯木森林
	/// </summary>
	public static void BuildLampWoodLand()
	{
		int upBound = Main.maxTilesY - 1660;
		int bottomBound = Main.maxTilesY - 480;
		int countLamp = 0;
		List<Vector2> roomPositions = new List<Vector2>();
		for (int i = 0; i < 1000; i++)
		{
			int x0CoordPerlin = GenRand.Next(1024);
			int y0CoordPerlin = GenRand.Next(1024);

			// 随机取点
			int checkX = GenRand.Next(50, Main.maxTilesX - 49);
			int checkY = GenRand.Next(upBound, bottomBound);

			// 如果上下左右都有大于100的空间且不在中心遗迹区
			if (CheckSpaceWidth(checkX, checkY) > 100 && CheckSpaceDown(checkX, checkY) > 100 && CheckSpaceUp(checkX, checkY) > 100 && (new Vector2(checkX, checkY) - TwilightRelicCenter).Length() > 400 && checkX < Main.maxTilesX - 200 && checkY < Main.maxTilesY - 620)
			{
				// 计入一个森林平台数量
				countLamp++;
				int startX = Math.Max(0, checkX - CheckSpaceLeft(checkX, checkY) - 60);
				int endX = Math.Min(checkX + CheckSpaceRight(checkX, checkY) + 60, Main.maxTilesX);

				// 大致铺设一遍地形
				for (int x = startX; x <= endX; x++)
				{
					for (int y = checkY - 30; y <= checkY + 30; y++)
					{
						Tile tile = SafeGetTile(x, y);
						float addXValue = 0;
						float addYValue = GetLargeSmokeTexturePixelG(x + x0CoordPerlin, y + y0CoordPerlin) * 0.5f;
						if (tile.WallType == ModContent.WallType<StoneDragonScaleWoodWall>())
						{
							addXValue = EmbeddingDepthOfTileType(x, y, ModContent.TileType<StoneScaleWood>(), 10) / 20f;
						}
						float value = addYValue + Math.Abs(checkY - y) / 30f + addXValue;
						int type = ModContent.TileType<DarkForestSoil>();
						if (value > 0.9)
						{
							type = ModContent.TileType<DarkForestGrass>();
						}
						if (value > 1)
						{
							continue;
						}
						tile.TileType = (ushort)type;
						tile.HasTile = true;
						if (value < 0.94)
						{
							tile.WallType = (ushort)ModContent.WallType<DarkForestSoilWall>();
						}
					}
				}

				// 精细铺设边角
				for (int x = startX - 10; x <= endX + 10; x++)
				{
					for (int y = checkY - 40; y <= checkY + 40; y++)
					{
						int depth = EmbeddingDepthOfTileType(x, y, ModContent.TileType<DarkForestGrass>());
						if (depth <= 4 && depth > 0)
						{
							if (TerrianSurfaceDiscontinuity(x, y, 4) > 0.30f)
							{
								for (int t = -7; t <= 7; t++)
								{
									Vector2 pos = new Vector2(x, y);
									Vector2 vel = TerrianSurfaceNormal(x, y, 4, ModContent.TileType<FemaleLampWood>()).RotatedBy(t / 10f);
									for (int u = 0; u < 5; u++)
									{
										pos += vel;
										Tile tile = SafeGetTile((int)pos.X, (int)pos.Y);
										tile.TileType = (ushort)ModContent.TileType<FemaleLampWood>();
										tile.HasTile = true;
									}
								}
							}
						}
					}
				}

				// 铺设物替换为草
				for (int x = startX - 20; x <= endX + 20; x++)
				{
					for (int y = checkY - 50; y <= checkY + 50; y++)
					{
						Tile tile = SafeGetTile(x, y);
						if (tile.TileType == (ushort)ModContent.TileType<FemaleLampWood>())
						{
							tile.TileType = (ushort)ModContent.TileType<DarkForestGrass>();
							tile.HasTile = true;
						}
					}
				}

				// 点缀岩石
				for (int x = startX - 10; x <= endX + 10; x++)
				{
					for (int y = checkY - 40; y <= checkY + 40; y++)
					{
						float aValue = GetLargeSmokeTexturePixelR(x + x0CoordPerlin, y + y0CoordPerlin) * 0.5f;
						float bValue = Math.Abs(checkY - y) / 120f - 0.1f + Math.Max(0, Math.Abs(checkX - x) / 120f - 0.6f);
						if (aValue + bValue < 0.2f)
						{
							Tile tile = SafeGetTile(x, y);
							tile.TileType = (ushort)ModContent.TileType<YggdrasilGrayRock>();
							tile.HasTile = true;
						}
					}
				}

				// 平坦化
				// SmoothTile_XXYY(startX - 10, checkY - 30, endX + 10, checkY - 25);
				// SmoothTile_XXYY(startX - 10, checkY + 25, endX + 10, checkY + 30);

				// 房子
				int countCell = 0;
				int roomCenterX = 0;
				bool built = false;
				while (countCell < 100)
				{
					countCell++;
					int minX = startX + 40;
					int maxX = endX - 40;
					if (minX > maxX)
					{
						(minX, maxX) = (maxX, minX);
					}
					int x = GenRand.Next(minX, maxX);
					roomCenterX = x;
					for (int y = checkY - 25; y > checkY - 30; y--)
					{
						string mapIOPath = string.Empty;
						switch (Main.rand.Next(8))
						{
							case 0:
								mapIOPath = ModAsset.LampWood_Legacy_1_17x13_Path;
								break;
							case 1:
								mapIOPath = ModAsset.LampWood_Legacy_2_22x14_Path;
								break;
							case 2:
								mapIOPath = ModAsset.LampWood_Legacy_3_18x14_Path;
								break;
							case 3:
								mapIOPath = ModAsset.LampWood_Legacy_4_26x22_Path;
								break;
							case 4:
								mapIOPath = ModAsset.LampWood_Legacy_5_17x8_Path;
								break;
							case 5:
								mapIOPath = ModAsset.LampWood_Legacy_6_16x11_Path;
								break;
							case 6:
								mapIOPath = ModAsset.LampWood_Legacy_7_23x15_Path;
								break;
							case 7:
								mapIOPath = ModAsset.LampWood_Legacy_8_16x16_Path;
								break;
						}

						var mapIO = new MapIO(x, y);
						int roomHeight = mapIO.ReadHeight(ModIns.Mod.GetFileStream(mapIOPath));
						int roomWidth = mapIO.ReadWidth(ModIns.Mod.GetFileStream(mapIOPath));
						int halfWidth = roomWidth / 2;
						if (x - halfWidth < 50 || x + halfWidth > Main.maxTilesX - 50)
						{
							continue;
						}
						bool canBuild = true;
						int xj = 0;
						int yj = 0;

						for (int j = 0; j < 50; j++)
						{
							Tile topLeft = SafeGetTile(x + xj - halfWidth, y + yj - roomHeight);
							Tile topRight = SafeGetTile(x + xj + halfWidth, y + yj - roomHeight);
							Tile bottomLeft = SafeGetTile(x + xj - halfWidth, y + yj);
							Tile bottomRight = SafeGetTile(x + xj + halfWidth, y + yj);

							if (!topLeft.HasTile && !topRight.HasTile &&
								bottomLeft.HasTile && (bottomLeft.TileType == ModContent.TileType<DarkForestGrass>() || bottomLeft.TileType == ModContent.TileType<DarkForestSoil>()) &&
								bottomRight.HasTile && (bottomRight.TileType == ModContent.TileType<DarkForestGrass>() || bottomRight.TileType == ModContent.TileType<DarkForestSoil>()))
							{
								break;
							}
							else
							{
								yj += 1;
							}
							if (j == 49)
							{
								canBuild = false;
								break;
							}
						}
						foreach (Vector2 point in roomPositions)
						{
							if ((point - new Vector2(x, y + yj)).Length() < 100)
							{
								canBuild = false;
								break;
							}
						}
						if (canBuild)
						{
							built = true;
							roomPositions.Add(new Vector2(x, y + yj));
						}
						else
						{
							continue;
						}

						QuickBuild(x, y + yj - roomHeight, mapIOPath);
						FillLampWoodChestXYWH(x, y + yj - roomHeight, roomWidth, roomHeight);
						for (int xi = x; xi < x + roomWidth; xi++)
						{
							for (int yi = y + yj; yi < y + yj + 5; yi++)
							{
								Tile tileBottomGrass = SafeGetTile(xi, yi);
								tileBottomGrass.TileType = (ushort)ModContent.TileType<DarkForestGrass>();
								tileBottomGrass.HasTile = true;
							}
						}
					}
					if (built)
					{
						break;
					}
				}

				// 罐子
				for (int x = startX; x <= endX; x++)
				{
					for (int y = checkY - 40; y <= checkY + 10; y++)
					{
						float valueG = GetLargeSmokeTexturePixelG(x * 2.24f, y * 2.24f);
						Tile tile0 = SafeGetTile(x, y);
						Tile tile1 = SafeGetTile(x + 1, y);
						Tile tile2 = SafeGetTile(x, y + 1);
						Tile tile3 = SafeGetTile(x + 1, y + 1);
						Tile tile4 = SafeGetTile(x, y - 1);
						Tile tile5 = SafeGetTile(x + 1, y - 1);

						// 罐子
						if (valueG > 0.4f)
						{
							if (!tile0.HasTile && !tile1.HasTile && !tile4.HasTile && !tile5.HasTile && tile2.HasTile && tile3.HasTile && tile2.Slope == SlopeType.Solid && (tile2.TileType == ModContent.TileType<DarkForestGrass>() || tile2.TileType == ModContent.TileType<DarkForestSoil>()) && tile3.Slope == SlopeType.Solid && (tile3.TileType == ModContent.TileType<DarkForestGrass>() || tile3.TileType == ModContent.TileType<DarkForestSoil>()))
							{
								if (GenRand.NextBool(3))
								{
									if (GenRand.NextBool(2))
									{
										PlaceFrameImportantTiles(x, y - 1, 2, 2, ModContent.TileType<LampWoodPot>(), GenRand.Next(6) * 36);
									}
									else
									{
										PlaceFrameImportantTiles(x, y - 1, 2, 2, TileID.Pots, GenRand.Next(3) * 36, 36);
									}
								}
							}
						}
					}
				}

				// 生命灯木
				if (countLamp > 12)
				{
					Vector2 checkTrunk = new Vector2(Main.maxTilesX - 100, Main.maxTilesY - 560);

					// Lamp wood mesa
					// 低于某个点位则填满泥土
					int radiusI = 240;
					Vector2 mesaOffset = new Vector2(0, -60);
					for (int x0 = -radiusI; x0 <= radiusI; x0++)
					{
						for (int y0 = -radiusI; y0 <= radiusI; y0++)
						{
							Tile tile = SafeGetTile(checkTrunk + mesaOffset + new Vector2(x0, y0));
							float aValue = GetLargeSmokeTexturePixelR(x0 + x0CoordPerlin, y0 + y0CoordPerlin);
							if (new Vector2(x0, y0).Length() <= radiusI - aValue * 10)
							{
								if (y0 > radiusI * 0.4f + aValue * 5)
								{
									tile.TileType = (ushort)ModContent.TileType<DarkForestGrass>();
									tile.HasTile = true;
								}
								if (y0 > radiusI * 0.41f + aValue * 5)
								{
									tile.TileType = (ushort)ModContent.TileType<DarkForestSoil>();
									tile.HasTile = true;
									tile.WallType = (ushort)ModContent.WallType<DarkForestSoilWall>();
								}
							}
						}
					}

					List<(Vector2 TrunkPos, float Width)> trunkPoints = new List<(Vector2, float)>();
					checkTrunk = new Vector2(Main.maxTilesX - 100, Main.maxTilesY - 500);
					LifeLampTreeStructure(trunkPoints, checkTrunk, new Vector2(0, -10), 50, 12);
					for (int t = 0; t < trunkPoints.Count; t++)
					{
						Vector2 trunkPoint = trunkPoints[t].TrunkPos;
						float jointWidth = trunkPoints[t].Width;
						CircleTile(trunkPoint, jointWidth, ModContent.TileType<FemaleLampWood>());
						CircleWall(trunkPoint, Math.Max(jointWidth - 1, 0), ModContent.WallType<FemaleLampWoodWall>());
					}
					for (int t = 0; t < trunkPoints.Count; t++)
					{
						Vector2 trunkPoint = trunkPoints[t].TrunkPos;
						float jointWidth = trunkPoints[t].Width;
						CircleTile(trunkPoint, Math.Max(jointWidth - 3, 0), -1, true);
					}
					for (int t = 5; t < trunkPoints.Count; t++)
					{
						Vector2 trunkPoint = trunkPoints[t].TrunkPos;
						float jointWidth = trunkPoints[t].Width + Main.rand.NextFloat(3) - 2;
						Vector2 placePos = trunkPoint + new Vector2(jointWidth * (GenRand.NextBool(2) ? -1 : 1), 0).RotatedByRandom(MathHelper.TwoPi);
						float distanceToLeaves = To100NearestTileTypeBlockDistance((int)placePos.X, (int)placePos.Y, ModContent.TileType<FemaleLampLeaves>());
						float distanceToWood = To100NearestTileTypeBlockDistance((int)placePos.X, (int)placePos.Y, ModContent.TileType<FemaleLampWood>());
						if (distanceToLeaves > 7 && jointWidth > 2 && distanceToWood <= 2)
						{
							Tile tile = SafeGetTile(placePos);
							if (tile.WallType != ModContent.WallType<FemaleLampWoodWall>())
							{
								tile.TileType = (ushort)ModContent.TileType<FemaleLampLeaves>();
								tile.HasTile = true;
							}
						}
					}
					break;
				}
			}
		}
	}

	/// <summary>
	/// Fill all lampwood chest by given area if exist.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public static void FillLampWoodChestXYWH(int x, int y, int width, int height)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				WorldGenMisc.TryFillChest(x + i, y + j, LampWoodChestContents());
			}
		}
	}

	/// <summary>
	/// Fill all twilight chest by given area if exist.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public static void FillTwilightChestXYWH(int x, int y, int width, int height)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				WorldGenMisc.TryFillChest(x + i, y + j, TwilightNormalChestContents());
			}
		}
	}

	/// <summary>
	/// 创建一个生命灯树
	/// </summary>
	/// <param name="trunkPoints"></param>
	/// <param name="startPoint"></param>
	/// <param name="maxStep"></param>
	/// <param name="width"></param>
	public static void LifeLampTreeStructure(List<(Vector2 TrunkPos, float Width)> trunkPoints, Vector2 startPoint, Vector2 velocity, float maxStep, float width)
	{
		Vector2 checkTrunk = startPoint;
		float maxWidth = width;
		float direction = GenRand.NextBool(2) ? -1 : 1;
		float omega = 0;
		for (int checkMax = 5; checkMax < 100; checkMax++)
		{
			Vector2 checkPoint = startPoint + velocity * width / 24f * checkMax;
			bool end = false;
			foreach (var point in trunkPoints)
			{
				if ((checkPoint - point.TrunkPos).Length() < width)
				{
					end = true;
					break;
				}
			}
			if (end)
			{
				if (maxStep > checkMax)
				{
					maxStep = checkMax;
				}
				break;
			}
		}
		for (int step = 0; step < maxStep; step++)
		{
			trunkPoints.Add((checkTrunk, width));
			checkTrunk += (velocity + velocity.RotatedBy(MathHelper.PiOver2) * GenRand.NextFloat(-0.2f, 0.2f)) * width / 24f;

			// 超过步数收缩
			if (step > maxStep * 0.6f)
			{
				width = maxWidth * (1 - MathF.Cos((maxStep - step) / (maxStep * 0.4f) * MathF.PI)) * 0.5f;
			}

			// 超过距离回旋
			if ((checkTrunk - startPoint).Length() > 10 * width)
			{
				if (MathF.Abs(omega) < 0.1f)
				{
					omega += direction * 0.01f;
				}
				maxWidth *= 0.995f;
			}
			velocity = velocity.RotatedBy(omega);
			width = Math.Min(width, maxWidth);
			if (width < 2)
			{
				break;
			}
			if (MathF.Abs(omega) >= 0.01f)
			{
				velocity *= 0.99f;
			}
			if (velocity.Length() < 5)
			{
				break;
			}
			if (step > maxStep * 0.2f)
			{
				if (!GenRand.NextBool(3) && trunkPoints.Count < 3000 && width > 5)
				{
					Vector2 newVel = velocity.RotatedBy(GenRand.NextFloat(0.8f, 1.3f) * (GenRand.NextBool(2) ? -1 : 1));
					LifeLampTreeStructure(trunkPoints, checkTrunk + newVel * 0.2f, newVel, (maxStep - step) * GenRand.NextFloat(0.6f, 3.7f), width * GenRand.NextFloat(0.35f, 0.55f));
				}
			}
		}
	}

	/// <summary>
	/// 挑战者石牢
	/// </summary>
	public static void BuildStoneCageOfChallenges()
	{
		int buildX = Main.maxTilesX / 2 - 300;
		int buildY = Main.maxTilesY - 1960;
		int x0CoordPerlin = GenRand.Next(1024);
		int y0CoordPerlin = GenRand.Next(1024);

		// 清扫周围物块腾出空间
		for (int x = 0; x < 450; x += 30)
		{
			CircleTileWithRandomNoise(new Vector2(buildX + x, buildY), 90, -1, 20, true);
		}
		CircleTileWithRandomNoise(new Vector2(buildX + 480, buildY), 90, ModContent.TileType<StoneScaleWood>(), 30, true);

		// 第一层与第二层交界处的穹顶
		for (int x = 0; x <= Main.maxTilesX - 1; x += 1)
		{
			for (int y = Main.maxTilesY - 2400; y <= Main.maxTilesY - 1800; y += 1)
			{
				float aValue = GetLargeSmokeTexturePixelR(Math.Abs(x * 4.3f + x0CoordPerlin), Math.Abs(y * 4.3f + y0CoordPerlin));
				float bValue = MathF.Abs((y - Main.maxTilesY + 2100) / 100f);
				float cValue = (x - Main.maxTilesX / 2f) / (Main.maxTilesX / 2f);
				cValue *= cValue;
				if (bValue > cValue)
				{
					bValue = -1 + (bValue - cValue) * 4;
				}
				else
				{
					bValue = -1;
				}
				if (aValue + bValue < 0.8f)
				{
					Tile tile = SafeGetTile(x, y);
					tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
					if (aValue + bValue < 0.75f)
					{
						tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
					}
					tile.HasTile = true;
				}
			}
		}

		// 电梯间
		// QuickBuild(buildX, buildY, ModAsset.LiftRoomOfChallengerHall40x22_Path);
		int step2X = buildX + 40;
		int step2Y = buildY + 3;
		PlaceRectangleAreaOfBlock(step2X, step2Y, step2X + 480, step2Y + 9, ModContent.TileType<StoneScaleWood>());
		KillRectangleAreaOfTile(step2X, step2Y + 2, step2X + 480, step2Y + 7);

		// 上半部分石壁
		for (int x = step2X; x <= step2X + 480; x += 1)
		{
			for (int y = Main.maxTilesY - 2100; y <= step2Y; y += 1)
			{
				float aValue = GetLargeSmokeTexturePixelR(Math.Abs(x * 4.3f + x0CoordPerlin), Math.Abs(y * 4.3f + y0CoordPerlin));
				float bValue = MathF.Abs((y - step2Y) / 100f);
				float cValue = (x - step2X + 20) / 50f;
				cValue *= cValue;
				cValue = MathF.Pow(cValue, 0.9f);
				if (aValue + bValue < cValue)
				{
					Tile tile = SafeGetTile(x, y);
					tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
					tile.HasTile = true;
				}
			}
		}

		// 上半部分挖空
		for (int x = step2X; x <= step2X + 480; x += 1)
		{
			for (int y = Main.maxTilesY - 2200; y <= step2Y + 1; y += 1)
			{
				float aValue = GetLargeSmokeTexturePixelR(Math.Abs(x * 4.3f + x0CoordPerlin), Math.Abs(y * 4.3f + y0CoordPerlin));
				float bValue = MathF.Abs((y - step2Y) / 70f);
				float cValue = (x - step2X - 240) / 200f;
				cValue *= cValue;
				cValue = 1 - cValue;
				cValue = MathF.Pow(cValue, 0.9f);
				if (aValue * 0.3f + bValue < cValue)
				{
					Tile tile = SafeGetTile(x, y);
					tile.HasTile = false;
				}
			}
		}

		// 下半部分石壁
		for (int x = step2X; x <= step2X + 480; x += 1)
		{
			for (int y = step2Y + 7; y <= step2Y + 500; y += 1)
			{
				float bValue = GetLargeSmokeTexturePixelR(Math.Abs(x * 4.3f + x0CoordPerlin), Math.Abs(y * 4.3f + y0CoordPerlin));
				int thick = y - step2Y - 7;
				float value = (x - step2X) / 480f;
				value *= value;
				value = MathF.Pow(value, 0.7f);
				if (thick > value * 300)
				{
					break;
				}
				if (thick > value * 240)
				{
					bValue += (thick - value * 240) / 60f;
				}
				Tile tile = SafeGetTile(x, y);
				if (tile.HasTile)
				{
					bValue += 1f;
				}
				if (tile.WallType > WallID.None)
				{
					bValue += 0.5f;
					if (x > step2X + 400)
					{
						bValue += (x - step2X - 400) / 80f;
					}
				}
				if (bValue < 1f)
				{
					bool shouldFill = true;
					foreach (var pos in TwilightBubbleCenters)
					{
						if ((new Vector2(x, y) - pos).Length() < 90)
						{
							shouldFill = false;
						}
					}
					if (shouldFill)
					{
						tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
						tile.HasTile = true;
					}
				}
			}
		}

		PlaceFrameImportantTiles(step2X + 236, step2Y - 3, 20, 10, ModContent.TileType<SquamousShellSeal>());
	}

	/// <summary>
	/// 暮光之地
	/// </summary>
	public static void BuildTwilightLand()
	{
		int upBound = Main.maxTilesY - 1800;
		int bottomBound = Main.maxTilesY - 760;
		int count = 0;
		List<Vector2> twilightCellPoses = new List<Vector2>();

		// 匍匐暮光草甸
		for (int i = 0; i < 1000; i++)
		{
			int x = GenRand.Next(20, Main.maxTilesX - 19);
			int y = GenRand.Next(upBound, bottomBound);
			if ((new Vector2(x, y) - TwilightRelicCenter).Length() > 300)
			{
				CrawlCarpetOfTypeTile(x, y, GenRand.Next(150, 450), 12, ModContent.TileType<TwilightGrassBlock>(), ModContent.TileType<StoneScaleWood>());
				bool canBuildCell = true;
				foreach (var pos in twilightCellPoses)
				{
					if ((pos - new Vector2(x, y)).Length() < 60)
					{
						canBuildCell = false;
						break;
					}
				}
				if (canBuildCell)
				{
					BuildTwilightCellRoom(x, y);
					twilightCellPoses.Add(new Vector2(x, y));
				}
			}
		}
		TwilightBubbleCenters = new List<Vector2>();

		// 随机取点300次，但是只生成俩地形
		for (int i = 0; i < 600; i++)
		{
			int x = GenRand.Next(20, Main.maxTilesX - 19);
			int y = GenRand.Next(upBound, bottomBound);
			int range = GenRand.Next(90, 200);
			if ((new Vector2(x, y) - LifeLampWoodRootPos).Length() > 300 + range && (new Vector2(x, y) - TwilightRelicCenter).Length() > 300 + range)
			{
				Vector2 basePos = new Vector2(x, y);
				bool canBuild = true;
				foreach (Vector2 v in TwilightBubbleCenters)
				{
					// 空出中心并且与另外一个点保持距离
					if ((basePos - v).Length() < 420)
					{
						canBuild = false;
						break;
					}
				}
				if (!canBuild)
				{
					continue;
				}
				TwilightBubbleCenters.Add(basePos);
				count++;
				CircleTileWithRandomNoise(basePos, range, ModContent.TileType<StoneScaleWood>(), 30);
				CircleTileWithRandomNoise(basePos, range - 20, -1, 30, true);

				// 低于某个点位则填满泥土
				int x0CoordPerlin = GenRand.Next(1024);
				int y0CoordPerlin = GenRand.Next(1024);
				int radiusI = range - 15;
				int y1 = (int)(radiusI * 0.5f + 5);
				for (int x0 = -radiusI; x0 <= radiusI; x0++)
				{
					for (int y0 = -radiusI; y0 <= radiusI; y0++)
					{
						Tile tile = SafeGetTile(basePos + new Vector2(x0, y0));
						Tile tileUp = SafeGetTile(basePos + new Vector2(x0, y0 - 1));
						float aValue = GetLargeSmokeTexturePixelR(x0 + x0CoordPerlin, y0 + y0CoordPerlin);
						if (new Vector2(x0, y0).Length() <= radiusI - aValue * 10)
						{
							if (!TileID.Sets.BasicChest[tile.TileType] && !TileID.Sets.BasicChest[tileUp.TileType])
							{
								if (y0 > radiusI * 0.5f + aValue * 5)
								{
									tile.TileType = (ushort)ModContent.TileType<TwilightGrassBlock>();
									tile.HasTile = true;
								}
								if (y0 > radiusI * 0.51f + aValue * 5)
								{
									tile.TileType = (ushort)ModContent.TileType<DarkForestSoil>();
									tile.HasTile = true;
									tile.WallType = (ushort)ModContent.WallType<DarkForestSoilWall>();
								}
							}
						}
					}

					// 泡中暮光森林平台建造零星遗迹
					bool canBuildRoom = x0 == (int)(-radiusI * 0.45f - 10) || x0 == (int)(radiusI * 0.45f - 10);
					var posCell = basePos + new Vector2(x0, y1);
					string mapIOPath = string.Empty;
					switch (GenRand.Next(3))
					{
						case 0:
							mapIOPath = ModAsset.TF_RelicFarm54x15_Path;
							break;
						case 1:
							mapIOPath = ModAsset.TF_RelicBuilding23x15_Path;
							break;
						case 2:
							mapIOPath = ModAsset.TF_RelicGrave18x7_Path;
							break;
					}
					var mapIO = new MapIO((int)posCell.X, (int)posCell.Y);
					int roomHeight = mapIO.ReadHeight(ModIns.Mod.GetFileStream(mapIOPath));
					int roomWidth = mapIO.ReadWidth(ModIns.Mod.GetFileStream(mapIOPath));
					if (x < 100 || x + roomWidth > Main.maxTilesX - 100)
					{
						canBuildRoom = false;
					}
					if (canBuildRoom)
					{
						QuickBuild((int)posCell.X, (int)posCell.Y - roomHeight, mapIOPath);
						FillLampWoodChestXYWH((int)posCell.X, (int)posCell.Y - roomHeight, roomWidth, roomHeight);
						twilightCellPoses.Add(posCell);
						for (int h = -4; h <= roomWidth; h++)
						{
							for (int v = 0; v < 4; v++)
							{
								var tile = SafeGetTile((int)posCell.X + h, (int)posCell.Y - 2 + v);
								if (!tile.HasTile)
								{
									tile.TileType = (ushort)ModContent.TileType<TwilightGrassBlock>();
									tile.HasTile = true;
								}
							}
						}
					}
				}

				// 圆壳结构下面穿破
				for (int j = 0; j < 4; j++)
				{
					DigTunnel(basePos.X, basePos.Y - range * 0.74f - j * 0.09f, GenRand.NextFloat(-0.2f, 0.2f), -1, GenRand.Next(127, 143), GenRand.Next(5, 8));
				}
				for (int j = 0; j < 6; j++)
				{
					DigTunnel(basePos.X, basePos.Y + range * 0.44f + j * 0.09f, GenRand.NextFloat(-0.5f, 0.5f), 1, GenRand.Next(127, 143), GenRand.Next(5, 8));
				}

				// 种树
				for (int x0 = -radiusI; x0 <= radiusI; x0++)
				{
					for (int y0 = -radiusI; y0 <= radiusI; y0++)
					{
						int height = GenRand.Next(7, 60);
						Tile tile = SafeGetTile(basePos + new Vector2(x0, y0));
						if (tile.TileType == ModContent.TileType<TwilightGrassBlock>())
						{
							if (GenRand.NextBool(3) && CanPlaceMultiAtTopTowardsUpRight((int)basePos.X + x0 - 3, (int)basePos.Y + y0, 8, height))
							{
								TreePlacer.BuildTwilightTree((int)basePos.X + x0, (int)basePos.Y + y0 - 1, height);
							}
						}
					}
				}
			}
			if (count > 12)
			{
				break;
			}
		}
	}

	/// <summary>
	/// 中心暮光之地附带遗迹
	/// </summary>
	public static void BuildTwilightRelic()
	{
		int range = GenRand.Next(330, 340);

		// 大轮廓
		CircleTileWithRandomNoise(TwilightRelicCenter, range, ModContent.TileType<StoneScaleWood>(), 30);
		CircleTileWithRandomNoise(TwilightRelicCenter, range - 40, -1, 30, true);

		// 低于某个点位则填满泥土
		int x0CoordPerlin = GenRand.Next(1024);
		int y0CoordPerlin = GenRand.Next(1024);
		int radiusI = range - 35;
		for (int x0 = -radiusI; x0 <= radiusI; x0++)
		{
			for (int y0 = -radiusI; y0 <= radiusI; y0++)
			{
				Tile tile = SafeGetTile(TwilightRelicCenter + new Vector2(x0, y0));
				float aValue = GetLargeSmokeTexturePixelR(x0 + x0CoordPerlin, y0 + y0CoordPerlin);
				if (new Vector2(x0, y0).Length() <= radiusI - aValue * 10)
				{
					if (y0 > radiusI * 0.5f + aValue * 5)
					{
						tile.TileType = (ushort)ModContent.TileType<TwilightGrassBlock>();
						tile.HasTile = true;
					}
					if (y0 > radiusI * 0.51f + aValue * 5)
					{
						tile.TileType = (ushort)ModContent.TileType<DarkForestSoil>();
						tile.HasTile = true;
						tile.WallType = (ushort)ModContent.WallType<DarkForestSoilWall>();
					}
				}
			}
		}
		int centerX = (int)TwilightRelicCenter.X;
		int centerY = (int)TwilightRelicCenter.Y;

		// 中心建筑遗迹 150 * 300
		// 风化外壳
		for (int x = centerX - 80; x <= centerX + 80; x += 1)
		{
			for (int y = centerY - 103; y <= centerY + 200; y += 1)
			{
				float aValue = GetLargeSmokeTexturePixelR(Math.Abs(x * 4.3f + x0CoordPerlin), Math.Abs(y * 4.3f + y0CoordPerlin));
				if (aValue < 0.4f)
				{
					Tile tile = SafeGetTile(x, y);
					tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick>();
					tile.HasTile = true;
				}
			}
		}
		for (int x = centerX - 120; x <= centerX + 120; x += 1)
		{
			for (int y = centerY + 160; y <= centerY + 260; y += 1)
			{
				float aValue = GetLargeSmokeTexturePixelR(Math.Abs(x * 4.3f + x0CoordPerlin), Math.Abs(y * 4.3f + y0CoordPerlin));
				if (aValue < 0.4f)
				{
					Tile tile = SafeGetTile(x, y);
					tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick>();
					tile.HasTile = true;
				}
			}
		}
		PlaceRectangleAreaOfBlock(centerX - 75, centerY - 100, centerX + 75, centerY + 230, ModContent.TileType<GreenRelicBrick>());
		PlaceRectangleAreaOfBlock(centerX - 105, centerY + 160, centerX + 105, centerY + 250, ModContent.TileType<GreenRelicBrick>());
		PlaceRectangleAreaOfWall(centerX - 78, centerY - 98, centerX + 78, centerY + 244, ModContent.WallType<GreenRelicWall>());

		// 顶部方波
		for (int x = -75; x < 75; x += 20)
		{
			PlaceRectangleAreaOfBlock(centerX + x, centerY - 108, centerX + 10 + x, centerY - 100, ModContent.TileType<GreenRelicBrick>());
		}

		// 种树
		for (int x0 = -radiusI; x0 <= radiusI; x0++)
		{
			for (int y0 = -radiusI; y0 <= radiusI; y0++)
			{
				int height = GenRand.Next(7, 60);
				Tile tile = SafeGetTile(TwilightRelicCenter + new Vector2(x0, y0));
				if (tile.TileType == ModContent.TileType<TwilightGrassBlock>())
				{
					if (GenRand.NextBool(3) && CanPlaceMultiAtTopTowardsUpRight(centerX + x0 - 3, centerY + y0, 8, height))
					{
						TreePlacer.BuildTwilightTree(centerX + x0, centerY + y0 - 1, height);
					}
				}
			}
		}

		// 房间布设
		int directionGate = GenRand.NextBool() ? 1 : -1;
		for (int x = 0; x < 3; x++)
		{
			if (x == 1)
			{
				continue;
			}
			for (int y = 0; y < 10; y++)
			{
				int roomOriginX = centerX - 75 + x * 50 + 25;
				int roomOriginY = centerY - 100 + y * 25 + 12;
				string[] randomRooms = new string[]
				{
					ModAsset.TCRoom_Rd_00_StoneCave_40x21_Path,
					ModAsset.TCRoom_Rd_01_BloodyChurch_40x21_Path,
					ModAsset.TCRoom_Rd_02_Desert_40x21_Path,
					ModAsset.TCRoom_Rd_03_WaterSluice_40x21_Path,
					ModAsset.TCRoom_Rd_04_Forest_40x21_Path,
					ModAsset.TCRoom_Rd_05_DarkDragon_40x21_Path,
				};
				if (x == 2)
				{
					randomRooms = new string[]
					{
						ModAsset.TCRoom_Ld_00_StoneCave_40x21_Path,
						ModAsset.TCRoom_Ld_01_BloodyChurch_40x21_Path,
						ModAsset.TCRoom_Ld_02_Desert_40x21_Path,
						ModAsset.TCRoom_Ld_03_WaterSluice_40x21_Path,
						ModAsset.TCRoom_Ld_04_Forest_40x21_Path,
						ModAsset.TCRoom_Ld_05_DarkDragon_40x21_Path,
					};
				}
				int roomType = GenRand.Next(randomRooms.Length);
				QuickBuild(roomOriginX - 20, roomOriginY - 10, randomRooms[roomType]);
				int coreItemType = -1;
				switch (roomType)
				{
					case 0:
						coreItemType = ModContent.ItemType<RockQuake>();
						break;
					case 1:
						coreItemType = ModContent.ItemType<BloodySwamp>();
						break;
					case 2:
						coreItemType = ModContent.ItemType<SpringOfQuicksand>();
						break;
					case 3:
						coreItemType = ModContent.ItemType<MechanismHalberd>();
						break;
					case 4:
						coreItemType = ModContent.ItemType<WiltedForestLamp>();
						break;
					case 5:
						coreItemType = ModContent.ItemType<MagicOfLightAndShadow>();
						break;
					default:
						coreItemType = ItemID.DirtBlock;
						break;
				}
				List<Item> contents = [new Item(setDefaultsToType: coreItemType), .. NormalChestContents()];
				FillChestXYWH(roomOriginX - 20, roomOriginY - 10, 40, 21, contents);

				// 房间通道
				if (y == 9 && directionGate + 1 == x)
				{
					// 外侧大门
					if (directionGate == -1)
					{
						KillRectangleAreaOfTile(roomOriginX - 30, roomOriginY + 2, roomOriginX - 18, roomOriginY + 9);
					}
					else
					{
						KillRectangleAreaOfTile(roomOriginX + 18, roomOriginY + 2, roomOriginX + 30, roomOriginY + 9);
					}
				}

				// 对内侧开门
				if (x == 0)
				{
					KillRectangleAreaOfTile(roomOriginX + 18, roomOriginY + 2, roomOriginX + 31, roomOriginY + 9);
					PlaceRectangleAreaOfBlock(roomOriginX + 19, roomOriginY + 2, roomOriginX + 22, roomOriginY + 6, ModContent.TileType<GreenRelicBrick>());

					// PlaceFrameImportantTiles(roomOriginX + 20, roomOriginY + 7, 1, 3, TileID.ClosedDoor, 0, 918);
				}
				if (x == 2)
				{
					KillRectangleAreaOfTile(roomOriginX - 31, roomOriginY + 2, roomOriginX - 18, roomOriginY + 9);
					PlaceRectangleAreaOfBlock(roomOriginX - 22, roomOriginY + 2, roomOriginX - 19, roomOriginY + 6, ModContent.TileType<GreenRelicBrick>());

					// PlaceFrameImportantTiles(roomOriginX - 20, roomOriginY + 7, 1, 3, TileID.ClosedDoor, 0, 918);
				}

				// PlaceTwilightLegacyBiomeChest(roomOriginX, roomOriginY + 9);
			}
		}

		// 清理中央垂直通道
		KillRectangleAreaOfTile(centerX - 18, centerY - 100 + 12 - 9, centerX + 18, centerY + 146);
		PlaceFrameImportantTiles(centerX - 1, centerY + 144, 3, 3, ModContent.TileType<GreenRelicSlotTable>());

		// 副塔 60 * 120
		int directionSubTower = GenRand.NextBool() ? 1 : -1;
		int subTowerCenterX = centerX + directionSubTower * 150;
		int subTowerCenterY = centerY + GenRand.Next(-30, 10);

		// 风化外壳
		for (int x = subTowerCenterX - 35; x <= subTowerCenterX + 35; x += 1)
		{
			for (int y = subTowerCenterY - 63; y <= subTowerCenterY + 63; y += 1)
			{
				float aValue = GetLargeSmokeTexturePixelR(Math.Abs(x * 4.3f + x0CoordPerlin), Math.Abs(y * 4.3f + y0CoordPerlin));
				if (aValue < 0.4f)
				{
					Tile tile = SafeGetTile(x, y);
					tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick>();
					tile.HasTile = true;
				}
			}
		}
		PlaceRectangleAreaOfBlock(subTowerCenterX - 30, subTowerCenterY - 60, subTowerCenterX + 30, subTowerCenterY + 60, ModContent.TileType<GreenRelicBrick>());
		PlaceRectangleAreaOfWall(subTowerCenterX - 28, subTowerCenterY - 58, subTowerCenterX + 28, subTowerCenterY + 58, ModContent.WallType<GreenRelicWall>());
		for (int x = -30; x < 30; x += 20)
		{
			PlaceRectangleAreaOfBlock(subTowerCenterX + x, subTowerCenterY - 68, subTowerCenterX + 10 + x, subTowerCenterY - 60, ModContent.TileType<GreenRelicBrick>());
		}
		KillRectangleAreaOfTile(subTowerCenterX - 26, subTowerCenterY - 56, subTowerCenterX + 26, subTowerCenterY + 56);

		// 连接廊道
		int subTowerFloor = subTowerCenterY + 56;
		int roomTailX = centerX - 50 + (directionSubTower + 1) * 50 + 19 * directionSubTower;
		int subTowerHeadX = subTowerCenterX - 28 * directionSubTower;
		if (directionSubTower < 0)
		{
			subTowerHeadX -= 1;
		}
		for (int y = 0; y < 10; y++)
		{
			int roomOriginY = centerY - 100 + y * 25 + 12;
			if (roomOriginY > subTowerFloor)
			{
				int minX = Math.Min(roomTailX, subTowerHeadX);
				int maxX = Math.Max(roomTailX, subTowerHeadX);
				for (int x = minX; x <= maxX; x++)
				{
					float lerpValue = (x - minX) / (float)(maxX - minX);
					if (directionSubTower > 0)
					{
						lerpValue = 1 - lerpValue;
					}
					float value = (float)Utils.Lerp(subTowerFloor, roomOriginY + 8, lerpValue);
					PlaceRectangleAreaOfBlock(x, (int)value - 12, x + 1, (int)value + 4, ModContent.TileType<GreenRelicBrick>());
					PlaceRectangleAreaOfWall(x, (int)value - 11, x + 1, (int)value + 3, ModContent.WallType<GreenRelicWall>());
					KillRectangleAreaOfTile(x, (int)value - 8, x + 1, (int)value + 1);
				}
				break;
			}
		}

		// 铺设激光网
		for (int x = 0; x < 6; x++)
		{
			for (int y = 0; y < 11; y++)
			{
				int checkX = subTowerCenterX - 25 + x * 10;
				int checkY = subTowerCenterY - 55 + y * 10;
				Tile tile = SafeGetTile(checkX, checkY);
				if ((x + y) % 2 == 1)
				{
					tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick_Trap>();
				}
				else
				{
					tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick>();
				}
				tile.HasTile = true;
			}
		}

		// 激光镀层和奖励砖
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 11; y++)
			{
				int checkX = subTowerCenterX - 20 + x * 10;
				int checkY = subTowerCenterY - 50 + y * 10;
				Tile tile = SafeGetTile(checkX, checkY);
				tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick_plating>();
				if (x == 2 && y == 0)
				{
					tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick_BonusKey>();
				}
				if (x == 0 && y == 3)
				{
					tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick_BonusKey>();
				}
				if (x == 4 && y == 3)
				{
					tile.TileType = (ushort)ModContent.TileType<GreenRelicBrick_BonusKey>();
				}
				tile.HasTile = true;
				if (tile.TileType == (ushort)ModContent.TileType<GreenRelicBrick_plating>())
				{
					tile.TileFrameX = (short)(18 * GenRand.Next(2));
				}
				else
				{
					tile.TileFrameX = 0;
				}
				if (x == 2 && y == 10)
				{
					WorldGenMisc.PlaceChest(checkX, checkY + 6, ModContent.TileType<RustBronzeTreasureChest_Lock>(), new List<Item>() { new Item(setDefaultsToType: ModContent.ItemType<CelticKeyStone>()) }, 0);
				}
			}
		}

		// 地下金库
		for (int subY = 0; subY < 50; subY++)
		{
			int y = centerY + subY + 180;
			float lengthX = MathF.Pow(subY / 20f, 0.3f);
			lengthX = Math.Clamp(lengthX, 0, 1);
			KillRectangleAreaOfTile(centerX - (int)(60 * lengthX), y, centerX + (int)(60 * lengthX), y);
		}
		WorldGenMisc.PlaceChest(centerX, centerY + 229, 21, new List<Item>() { new Item(setDefaultsToType: ModContent.ItemType<RingOfMatter>()) }, 1);
		for (int x0 = -60; x0 < 61; x0++)
		{
			int y = centerY + 180;
			for (int y0 = 10; y0 < 50; y0++)
			{
				Tile tile = SafeGetTile(centerX + x0, y + y0);
				if (!tile.HasTile)
				{
					float height = 0.15f;
					float value = Math.Abs(x0) * height + GenRand.NextFloat(1f);
					if (y0 > value + 50 - 60 * height)
					{
						tile.TileType = TileID.GoldCoinPile;
						tile.HasTile = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// Smooth whole stratum.
	/// </summary>
	public static void SmoothYggdrasilTown()
	{
		// SmoothTile_XXYY(0, (int)(Main.maxTilesY * 0.9), Main.maxTilesX, Main.maxTilesY);
	}

	/// <summary>
	/// 建造暮光之地火柴盒
	/// </summary>
	public static void BuildTwilightCellRoom(int x, int y)
	{
		string mapIOPath = string.Empty;
		switch (GenRand.Next(5))
		{
			case 0:
				mapIOPath = ModAsset.TF_RelicStitedBuilding26x20_Path;
				break;
			case 1:
				mapIOPath = ModAsset.TF_RelicStitedBuilding26x23_Path;
				break;
			case 2:
				mapIOPath = ModAsset.TF_RelicStitedBuilding26x19_Path;
				break;
			case 3:
				mapIOPath = ModAsset.TF_RelicStitedBuilding23x20_Path;
				break;
			case 4:
				mapIOPath = ModAsset.TF_RelicStitedBuilding24x19_Path;
				break;
		}

		var mapIO = new MapIO(x, y);
		int roomHeight = mapIO.ReadHeight(ModIns.Mod.GetFileStream(mapIOPath));
		int roomWidth = mapIO.ReadWidth(ModIns.Mod.GetFileStream(mapIOPath));
		int halfWidth = roomWidth / 2;

		bool canBuild = true;
		if (x < 50 || x + roomWidth > Main.maxTilesX - 50)
		{
			canBuild = false;
		}
		if (canBuild)
		{
			int minHeight = 200;
			for (int i = x; i <= x + roomWidth; i++)
			{
				int height = CheckSpaceDown(i, y);
				if (height < minHeight)
				{
					minHeight = height;
				}
			}
			if (minHeight > 20)
			{
				canBuild = false;
			}
			if (minHeight <= 1)
			{
				int topHeight = CheckSpaceUp(x + halfWidth, y - roomHeight);
				if (topHeight < 5)
				{
					canBuild = false;
				}
			}
		}
		if (canBuild)
		{
			if (To100NearestTileTypeBlockDistance(x, y, ModContent.TileType<TwilightGrassBlock>()) > 50)
			{
				canBuild = false;
			}
			if (!ChestSafeArea(x, y - roomHeight, roomWidth, roomHeight))
			{
				canBuild = false;
			}
		}
		bool leftwall = SafeGetTile(x, y - roomHeight).wall == ModContent.WallType<StoneDragonScaleWoodWall>();
		bool rightwall = SafeGetTile(x + roomWidth, y - roomHeight).wall == ModContent.WallType<StoneDragonScaleWoodWall>();
		if (canBuild)
		{
			QuickBuild(x, y - roomHeight, mapIOPath);
			FillTwilightChestXYWH(x, y - roomHeight, roomWidth, roomHeight);
			if (leftwall)
			{
				for (int i = x; i <= x + halfWidth; i++)
				{
					for (int j = y - roomHeight; j <= y; j++)
					{
						var tile = SafeGetTile(i, j);
						if (tile.WallType <= WallID.None)
						{
							tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
						}
					}
				}
			}
			if (rightwall)
			{
				for (int i = x + halfWidth; i <= x + roomWidth; i++)
				{
					for (int j = y - roomHeight; j <= y; j++)
					{
						var tile = SafeGetTile(i, j);
						if (tile.WallType <= WallID.None)
						{
							tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
						}
					}
				}
			}

			// Lengthen the post of cell rooms
			int maxHeight = GenRand.Next(10, 30);
			for (int i = x; i <= x + roomWidth; i++)
			{
				var tile = SafeGetTile(i, y - 2);
				if (tile.TileType == TileID.WoodenBeam)
				{
					for (int j = 0; j < 30; j++)
					{
						var tile2 = SafeGetTile(i, y + j);
						if (tile2.HasTile && tile2.TileType != TileID.WoodenBeam)
						{
							break;
						}
						else
						{
							if (ChestSafe(i, y + j))
							{
								tile2.TileType = TileID.WoodenBeam;
								tile2.HasTile = true;
							}
						}
						if (j >= maxHeight)
						{
							int leftDis = CheckSpaceLeft(i, y + j + 1);
							int rightDis = CheckSpaceRight(i, y + j + 1);
							if (leftDis > rightDis)
							{
								for (int h = 0; h < 100; h++)
								{
									var tile3 = SafeGetTile(i + h, y + j);
									if (tile3.HasTile && tile2.TileType != TileID.WoodenBeam)
									{
										break;
									}
									else
									{
										if (ChestSafe(i + h, y + j))
										{
											tile3.TileType = (ushort)ModContent.TileType<TwilightEucalyptusWood>();
											tile3.HasTile = true;
										}
									}
								}
							}
							else
							{
								for (int h = 0; h < 100; h++)
								{
									var tile3 = SafeGetTile(i - h, y + j);
									if (tile3.HasTile && tile2.TileType != TileID.WoodenBeam)
									{
										break;
									}
									else
									{
										if (ChestSafe(i - h, y + j))
										{
											tile3.TileType = (ushort)ModContent.TileType<TwilightEucalyptusWood>();
											tile3.HasTile = true;
										}
									}
								}
							}
							break;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 建造一个灯柱
	/// </summary>
	public static void PlaceChineseStyleLampPost(int x, int y, int style, int ai0 = 0, int ai1 = 0)
	{
		switch (style)
		{
			case 0:
				PlaceFrameImportantTiles(x, y - 5, 1, 5, ModContent.TileType<DoubleArmsChineseStreetLamp>());
				break;
			case 1:
				PlaceRectangleAreaOfWall(x, y - 5, x, y, WallID.BambooFence);
				Tile tile0 = SafeGetTile(x, y - 5);
				int tile1Dir = x + 1;
				if (ai0 % 2 == 0)
				{
					tile1Dir = x - 1;
				}
				Tile tile1 = SafeGetTile(tile1Dir, y - 5);
				tile0.TileType = TileID.Platforms;
				tile0.frameY = 786;
				tile0.HasTile = true;
				tile1.TileType = TileID.Platforms;
				tile1.frameY = 786;
				tile1.HasTile = true;
				int lanternType = 1620;
				if (ai1 % 4 == 0)
				{
					lanternType = 936;
				}
				PlaceFrameImportantTiles(tile1Dir, y - 4, 1, 2, TileID.HangingLanterns, 0, lanternType);
				break;
			case 2:
				int lampStyle = 918;
				int lampStyleX = 0;
				if (ai0 % 12 == 1)
				{
					lampStyle = 324;
				}
				if (ai0 % 12 == 2)
				{
					lampStyle = 432;
				}
				if (ai0 % 12 == 3)
				{
					lampStyle = 756;
				}
				if (ai0 % 12 == 4)
				{
					lampStyle = 0;
				}
				if (ai0 % 12 == 5)
				{
					lampStyle = 108;
					lampStyleX = 36;
				}
				PlaceFrameImportantTiles(x, y - 3, 1, 3, TileID.Lamps, lampStyleX, lampStyle);
				break;
		}
	}

	/// <summary>
	/// 建造一个中式的民居
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="endX"></param>
	/// <param name="endY"></param>
	public static void CreateChineseFolkBox(int startX, int startY, int endX, int endY)
	{
		int width = Math.Abs(endX - startX);
		int height = Math.Abs(endY - startY);
		int squire = width * height;
		if (!GenRand.NextBool(4))
		{
			switch (WorldGen.genRand.Next(5))
			{
				case 0:
					if (width >= 26 && height > 10)
					{
						switch (WorldGen.genRand.Next(2))
						{
							case 0:
								QuickBuild(startX, endY - 11, ModAsset.MapIOs_1FolkHouseofChineseStyleTypeA28x11_Path);
								break;
							case 1:
								QuickBuild(startX, endY - 11, ModAsset.MapIOs_1FolkHouseofChineseStyleTypeB28x11_Path);
								break;
						}
						return;
					}
					else
					{
						break;
					}

				case 1:
					if (width >= 20 && height > 8)
					{
						switch (WorldGen.genRand.Next(2))
						{
							case 0:
								QuickBuild(startX, endY - 8, ModAsset.MapIOs_3SmithyTypeA22x8_Path);
								break;
							case 1:
								QuickBuild(startX, endY - 8, ModAsset.MapIOs_3SmithyTypeB22x8_Path);
								break;
						}
						return;
					}
					else
					{
						break;
					}

				case 2:
					if (width >= 26 && height > 10)
					{
						switch (WorldGen.genRand.Next(4))
						{
							case 0:
								QuickBuild(startX, endY - 11, ModAsset.MapIOs_2FolkHouseofWoodAndStoneStrutureTypeA28x11_Path);
								break;
							case 1:
								QuickBuild(startX, endY - 11, ModAsset.MapIOs_2FolkHouseofWoodAndStoneStrutureTypeB28x11_Path);
								break;
							case 2:
								QuickBuild(startX, endY - 11, ModAsset.MapIOs_2FolkHouseofWoodStoneStrutureTypeA28x11_Path);
								break;
							case 3:
								QuickBuild(startX, endY - 11, ModAsset.MapIOs_2FolkHouseofWoodStoneStrutureTypeB28x11_Path);
								break;
						}
						return;
					}
					else
					{
						break;
					}

				case 3:
					if (width >= 20 && height > 9)
					{
						switch (WorldGen.genRand.Next(4))
						{
							case 0:
								QuickBuild(startX, endY - 10, ModAsset.MapIOs_4FolkHouseofWoodStrutureTypeA22x10_Path);
								break;
							case 1:
								QuickBuild(startX, endY - 10, ModAsset.MapIOs_4FolkHouseofWoodStrutureTypeB22x10_Path);
								break;
							case 2:
								QuickBuild(startX, endY - 10, ModAsset.MapIOs_4FolkHouseofWoodStrutureTypeC22x10_Path);
								break;
							case 3:
								QuickBuild(startX, endY - 10, ModAsset.MapIOs_4FolkHouseofWoodStrutureTypeD22x10_Path);
								break;
						}
						return;
					}
					else
					{
						break;
					}

				case 4:
					if (width >= 21 && height > 12)
					{
						switch (WorldGen.genRand.Next(3))
						{
							case 0:
								QuickBuild(startX, endY - 13, ModAsset.MapIOs_5TwoStoriedFolkHouseTypeA23x13_Path);
								break;
							case 1:
								QuickBuild(startX, endY - 13, ModAsset.MapIOs_5TwoStoriedFolkHouseTypeB23x13_Path);
								break;
							case 2:
								QuickBuild(startX, endY - 13, ModAsset.MapIOs_5TwoStoriedFolkHouseTypeC23x13_Path);
								break;
						}
						return;
					}
					else
					{
						break;
					}
			}
		}

		// 本体
		CreateBoxRoom(startX, startY, endX, endY, TileID.DynastyWood, WallID.Ebonwood, true);

		// 黄色的屋檐
		PlaceRectangleAreaOfBlock(startX - 1, startY, startX - 1, startY, ModContent.TileType<YellowDynastyShingles>());
		PlaceRectangleAreaOfBlock(endX + 1, startY, endX + 1, startY, ModContent.TileType<YellowDynastyShingles>());
		PlaceRectangleAreaOfBlock(startX - 2, startY + 1, startX - 1, startY + 1, ModContent.TileType<YellowDynastyShingles>());
		PlaceRectangleAreaOfBlock(endX + 1, startY + 1, endX + 2, startY + 1, ModContent.TileType<YellowDynastyShingles>());
		if (GenRand.NextBool(2))
		{
			PlaceRectangleAreaOfBlock(startX - 3, startY + 2, startX - 1, startY + 2, ModContent.TileType<YellowDynastyShingles>());
			PlaceRectangleAreaOfBlock(endX + 1, startY + 2, endX + 3, startY + 2, ModContent.TileType<YellowDynastyShingles>());
		}

		// 50%的概率侧面有灯笼
		if (GenRand.NextBool(2))
		{
			int type = ModContent.TileType<SideHangingLantern_Red>();
			if (GenRand.NextBool(3))
			{
				type = ModContent.TileType<SideHangingLantern_White>();
			}
			int placeY = startY;

			// 左侧
			bool canPlaceLeft = true;
			while (SafeGetTile(startX - 1, placeY).HasTile)
			{
				placeY++;
				if (placeY - startY > 5)
				{
					canPlaceLeft = false;
					break;
				}
			}
			for (int x = startX - 2; x <= startX - 1; x++)
			{
				for (int y = placeY; y <= placeY + 2; y++)
				{
					Tile check = SafeGetTile(x, y);
					if (check.HasTile)
					{
						canPlaceLeft = false;
					}
				}
			}
			if (canPlaceLeft)
			{
				PlaceFrameImportantTiles(startX - 2, placeY, 2, 3, type);
			}

			// 右侧
			bool canPlaceRight = true;
			placeY = startY;
			while (SafeGetTile(endX + 1, placeY).HasTile)
			{
				placeY++;
				if (placeY - startY > 5)
				{
					canPlaceRight = false;
					break;
				}
			}
			for (int x = endX + 1; x <= endX + 2; x++)
			{
				for (int y = placeY; y <= placeY + 2; y++)
				{
					Tile check = SafeGetTile(x, y);
					if (check.HasTile)
					{
						canPlaceRight = false;
					}
				}
			}
			if (canPlaceRight)
			{
				PlaceFrameImportantTiles(endX + 1, placeY, 2, 3, type, 36);
			}
		}

		// 判定是否应该有门
		bool hasEndXDoor = true;
		for (int x = endX + 1; x < endX + 5; x++)
		{
			int y = endY;
			Tile tile = SafeGetTile(x, y);
			if (!tile.HasTile)
			{
				hasEndXDoor = false;
				break;
			}
		}
		if (hasEndXDoor)
		{
			PlaceFrameImportantTiles(endX, endY - 3, 1, 3, TileID.ClosedDoor);
		}
		bool hasStartXDoor = true;
		for (int x = startX - 1; x > startX - 5; x--)
		{
			int y = endY;
			Tile tile = SafeGetTile(x, y);
			if (!tile.HasTile)
			{
				hasStartXDoor = false;
				break;
			}
		}
		if (hasStartXDoor)
		{
			PlaceFrameImportantTiles(startX, endY - 3, 1, 3, TileID.ClosedDoor);
		}
		DistributeChineseStyleDecorations(startX, startY, endX, endY);
	}

	/// <summary>
	/// 装饰一个中式的房间
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="endX"></param>
	/// <param name="endY"></param>
	public static void DistributeChineseStyleDecorations(int startX, int startY, int endX, int endY)
	{
		int width = Math.Abs(endX - startX);
		int height = Math.Abs(endY - startY);
		int squire = width * height;

		// 面积大于100的情况拆分房间
		if (squire > 100)
		{
			if (height >= width * 0.84f)
			{
				int middleCutY = (int)(height / 2f + GenRand.NextFloat(-height * 0.02f, height * 0.02f)) + startY;
				PlaceRectangleAreaOfBlock(startX, middleCutY, endX, middleCutY, TileID.DynastyWood);
				DistributeChineseStyleDecorations(startX, startY, endX, middleCutY);
				DistributeChineseStyleDecorations(startX, middleCutY, endX, endY);
				return;
			}
			if (width >= 1.5f * height && height >= 3)
			{
				int middleCutX = (int)(width / 2f + GenRand.NextFloat(-width * 0.2f, width * 0.2f)) + startX;
				PlaceRectangleAreaOfBlock(middleCutX, startY, middleCutX, endY - 3, TileID.DynastyWood);

				// 横向分割需要加门
				PlaceFrameImportantTiles(middleCutX, endY - 3, 1, 3, TileID.ClosedDoor);

				DistributeChineseStyleDecorations(startX, startY, middleCutX, endY);
				DistributeChineseStyleDecorations(middleCutX, startY, endX, endY);
				return;
			}
		}
		List<int> emptyBottomX = new List<int>();
		for (int x = startX; x < endX; x++)
		{
			int y = endY + 1;
			Tile tile = SafeGetTile(x, y);
			Tile tileUp = SafeGetTile(x, y - 2);
			if (tile.WallType != WallID.None && !tile.HasTile && tileUp.wall != 0 && !tileUp.HasTile)
			{
				emptyBottomX.Add(x);
			}
		}

		// 随机获取一个放置平台的点位
		List<int[]> continuePlatforms = new List<int[]>();
		List<int> continuePlatform = new List<int>();
		for (int times = 1; times < emptyBottomX.Count; times++)
		{
			if (emptyBottomX[times] - emptyBottomX[times - 1] == 1)
			{
				if (continuePlatform.Count == 0)
				{
					continuePlatform.Add(emptyBottomX[times - 1]);
				}
				continuePlatform.Add(emptyBottomX[times]);
			}
			else
			{
				if (continuePlatform.Count >= 3)
				{
					continuePlatforms.Add(continuePlatform.ToArray());
				}
				continuePlatform = new List<int>();
			}
			if (continuePlatform.Count >= 3)
			{
				if (GenRand.NextBool(3))
				{
					continuePlatforms.Add(continuePlatform.ToArray());
					continuePlatform = new List<int>();
				}
			}
		}
		if (continuePlatform.Count > 0)
		{
			continuePlatforms.Add(continuePlatform.ToArray());
		}
		int createPlatformIndex = GenRand.Next(continuePlatforms.Count);
		if (continuePlatforms.Count > 0)
		{
			foreach (int x in continuePlatforms[createPlatformIndex])
			{
				int y = endY;
				Tile tile = SafeGetTile(x, y);
				tile.TileType = TileID.Platforms;
				tile.TileFrameY = 342;
				tile.TileFrameX = 0;
				if (continuePlatforms[createPlatformIndex][0] == x)
				{
					tile.TileFrameX = 252;
				}
				if (continuePlatforms[createPlatformIndex][continuePlatforms[createPlatformIndex].Length - 1] == x)
				{
					tile.TileFrameX = 216;
				}

				tile.WallType = SafeGetTile(x, y - 1).wall;
			}
		}

		// Hanging Tiles
		PlaceARowOfHangingItems(startX, endX, startY + 1);
	}

	/// <summary>
	/// 建造一个火柴盒
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="endX"></param>
	/// <param name="endY"></param>
	/// <param name="boundTileType"></param>
	/// <param name="contentWallType"></param>
	/// <param name="Forced"></param>
	public static void CreateBoxRoom(int startX, int startY, int endX, int endY, int boundTileType, int contentWallType, bool Forced = false)
	{
		if (endX < startX)
		{
			(startX, endX) = (endX, startX);
		}
		if (endY < startY)
		{
			(startY, endY) = (endY, startY);
		}
		for (int x = startX; x <= endX; x++)
		{
			for (int y = startY; y <= endY; y++)
			{
				Tile tile = SafeGetTile(x, y);
				if (!Forced)
				{
					if (x == startX || x == endX || y == startY || y == endY)
					{
						if (!tile.HasTile)
						{
							tile.TileType = (ushort)boundTileType;
							tile.HasTile = true;
						}
					}
					else
					{
						if (tile.WallType == WallID.None)
						{
							tile.WallType = (ushort)contentWallType;
						}
					}
				}
				else
				{
					if (x == startX || x == endX || y == startY || y == endY)
					{
						tile.TileType = (ushort)boundTileType;
						tile.HasTile = true;
					}
					else
					{
						tile.HasTile = false;
						tile.WallType = (ushort)contentWallType;
					}
				}
			}
		}
	}

	/// <summary>
	/// 造一条斜索
	/// </summary>
	public static void CreateSlantCable(int x, int y, int direction, int type, int density = 12, int maxStep = 65535)
	{
		int count = 0;
		while (!SafeGetTile(x, y).HasTile || count <= 2)
		{
			count++;
			Tile tile1 = SafeGetTile(x, y);
			Tile tile2 = SafeGetTile(x + direction, y);
			tile1.HasTile = true;
			tile1.TileType = (ushort)type;
			if (direction == -1)
			{
				tile1.slope((byte)SlopeType.SlopeUpLeft);
			}
			else
			{
				tile1.slope((byte)SlopeType.SlopeUpRight);
			}
			tile2.HasTile = true;
			tile2.TileType = (ushort)type;
			if (direction == -1)
			{
				tile2.slope((byte)SlopeType.SlopeDownRight);
			}
			else
			{
				tile2.slope((byte)SlopeType.SlopeDownLeft);
			}
			if (x < 20 || y < 20 || x > Main.maxTilesX - 20 || y > Main.maxTilesY - 20)
			{
				break;
			}
			if (count > maxStep)
			{
				break;
			}
			if (count % density == density - 1 && maxStep == 65535)
			{
				for (int y0 = 0; y0 < count; y0++)
				{
					Tile tile3 = SafeGetTile(x + direction, y - y0);
					tile3.wall = WallID.Shadewood;
					if (y0 % (density * 2) == density - 1 && !SafeGetTile(x + direction, y - y0).HasTile)
					{
						CreateSlantCable(x + direction, y - y0, direction, type, density, density - 1);
						CreateSlantCable(x + direction * 0, y - y0, -direction, type, density, density - 1);
					}
				}
			}
			x += direction;
			y += 1;
		}
	}

	/// <summary>
	/// 排布一行悬挂物
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="endX"></param>
	/// <param name="y"></param>
	public static void PlaceARowOfHangingItems(int startX, int endX, int y)
	{
		int x = startX + 1;
		while (x + 1 < endX)
		{
			float totalRare = 0;
			foreach (YggdrasilTownStreetElement element in InDoorChineseStyleHangingSheet)
			{
				if (element.Width <= endX - (x + 1))
				{
					if (element.Cooling == 0)
					{
						totalRare += 1 / element.Rare;
					}
				}
			}
			if (totalRare == 0)
			{
				foreach (YggdrasilTownStreetElement element in InDoorChineseStyleHangingSheet)
				{
					if (element.Cooling > 0)
					{
						element.Update();
					}
				}
				x++;
				continue;
			}
			float buildIndex = GenRand.NextFloat(totalRare);
			float rareIndex = 0;
			int width = 0;
			foreach (YggdrasilTownStreetElement element in InDoorChineseStyleHangingSheet)
			{
				if (element.Width <= endX - (x + 1))
				{
					if (element.Cooling == 0)
					{
						rareIndex += 1 / element.Rare;
					}
					if (rareIndex - 1 / element.Rare < buildIndex && rareIndex >= buildIndex)
					{
						element.Build(ref x, y);
						width = element.Width;
						break;
					}
				}
			}
			foreach (YggdrasilTownStreetElement element in InDoorChineseStyleHangingSheet)
			{
				if (element.Cooling == 0)
				{
					element.Update(width);
				}
			}
		}
	}

	/// <summary>
	/// 造一条街
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <param name="direction"></param>
	/// <param name="step"></param>
	/// <param name="thick"></param>
	public static void CreateStreet(int startX, int startY, int endX, int endY)
	{
		PlaceRectangleAreaOfBlock(startX, startY, endX, endY, TileID.GrayBrick);
		int x = startX + 1;
		int y = startY;
		while (x + 1 < endX)
		{
			float totalRare = 0;
			foreach (YggdrasilTownStreetElement element in StreetConstructorsSheet)
			{
				if (element.Width <= endX - (x + 1))
				{
					if (element.Cooling == 0)
					{
						totalRare += 1 / element.Rare;
					}
				}
			}
			if (totalRare == 0)
			{
				foreach (YggdrasilTownStreetElement element in StreetConstructorsSheet)
				{
					if (element.Cooling > 0)
					{
						element.Update();
					}
				}
				x++;
				continue;
			}
			float buildIndex = GenRand.NextFloat(totalRare);
			float rareIndex = 0;
			int width = 0;
			foreach (YggdrasilTownStreetElement element in StreetConstructorsSheet)
			{
				if (element.Width <= endX - (x + 1))
				{
					if (element.Cooling == 0)
					{
						rareIndex += 1 / element.Rare;
					}
					if (rareIndex - 1 / element.Rare < buildIndex && rareIndex >= buildIndex)
					{
						element.Build(ref x, y);
						width = element.Width;
						break;
					}
				}
			}
			foreach (YggdrasilTownStreetElement element in StreetConstructorsSheet)
			{
				if (element.Cooling == 0)
				{
					element.Update(width);
				}
			}
		}
	}

	/// <summary>
	/// 以ij为左上点放置青缎矿5x3
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	public static void PlaceLargeCyanVineOre(int i, int j)
	{
		switch (GenRand.Next(2))
		{
			case 0:
				for (int x = 0; x < 5; x++)
				{
					for (int y = 0; y < 3; y++)
					{
						var tile = SafeGetTile(i + x, j + y);
						if (x == 0 && y == 0)
						{
							continue;
						}

						if (x == 4 && y == 0)
						{
							continue;
						}

						if (x == 4 && y == 1)
						{
							continue;
						}

						if (x == 0 && y == 1)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.IsHalfBlock = true;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						if (x == 4 && y == 2)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.Slope = SlopeType.SlopeDownLeft;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						if (x == 1 && y == 2)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreLarge>();
							tile.TileFrameX = 36;
							tile.TileFrameY = 54;
							tile.HasTile = true;
							continue;
						}
						tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
						tile.TileFrameX = (short)(x * 18);
						tile.TileFrameY = (short)(y * 18);
						tile.HasTile = true;
						if (x == 0 && y == 2)
						{
							var tile2 = SafeGetTile(i + x - 1, j + y + 1);
							tile2.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
							tile2.HasTile = true;
						}
					}
				}
				break;
			case 1:
				for (int x = 1; x < 4; x++)
				{
					for (int y = 1; y < 3; y++)
					{
						var tile = SafeGetTile(i + x, j + y);
						if (x == 4 && y == 1)
						{
							continue;
						}

						if (x == 2 && y == 2)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreLarge>();
							tile.TileFrameX = 144;
							tile.TileFrameY = 54;
							tile.HasTile = true;
							continue;
						}
						if (x == 1 && y == 1)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.IsHalfBlock = true;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
						tile.TileFrameX = (short)(x * 18);
						tile.TileFrameY = (short)(y * 18);
						tile.HasTile = true;
					}
				}
				break;
		}
	}

	/// <summary>
	/// 以ij为左上点放置青缎矿4x3
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	public static void PlaceMiddleCyanVineOre(int i, int j)
	{
		switch (GenRand.Next(4))
		{
			case 0:
				for (int x = 0; x < 3; x++)
				{
					for (int y = 0; y < 3; y++)
					{
						var tile = SafeGetTile(i + x, j + y);
						if (x == 0 && y == 0)
						{
							continue;
						}

						if (x == 1 && y == 0)
						{
							continue;
						}

						if (x == 0 && y == 1)
						{
							continue;
						}

						if (x == 2 && y == 0)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.Slope = SlopeType.SlopeDownRight;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						if (x == 1 && y == 2)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreMiddle>();
							tile.TileFrameX = 18;
							tile.TileFrameY = 54;
							tile.HasTile = true;
							continue;
						}
						tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
						tile.TileFrameX = (short)(x * 18);
						tile.TileFrameY = (short)(y * 18);
						tile.HasTile = true;
					}
				}
				break;
			case 1:
				for (int x = 0; x < 3; x++)
				{
					for (int y = 1; y < 3; y++)
					{
						var tile = SafeGetTile(i + x, j + y);

						if (x == 0 && y == 1)
						{
							continue;
						}

						if (x == 2 && y == 1)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.Slope = SlopeType.SlopeDownLeft;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						if (x == 1 && y == 2)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreMiddle>();
							tile.TileFrameX = 90;
							tile.TileFrameY = 54;
							tile.HasTile = true;
							continue;
						}
						tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
						tile.TileFrameX = (short)(x * 18);
						tile.TileFrameY = (short)(y * 18);
						tile.HasTile = true;
					}
				}
				break;
			case 2:
				for (int x = 1; x < 4; x++)
				{
					for (int y = 1; y < 3; y++)
					{
						var tile = SafeGetTile(i + x, j + y);

						if (x == 3 && y == 1)
						{
							continue;
						}

						if (x == 1 && y == 2)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreMiddle>();
							tile.TileFrameX = 162;
							tile.TileFrameY = 54;
							tile.HasTile = true;
							continue;
						}
						tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
						tile.TileFrameX = (short)(x * 18);
						tile.TileFrameY = (short)(y * 18);
						tile.HasTile = true;
					}
				}
				break;
			case 3:
				for (int x = 0; x < 3; x++)
				{
					for (int y = 1; y < 3; y++)
					{
						var tile = SafeGetTile(i + x, j + y);

						if (x == 0 && y == 1)
						{
							continue;
						}

						if (x == 0 && y == 2)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.Slope = SlopeType.SlopeDownRight;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						if (x == 2 && y == 1)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.IsHalfBlock = true;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						if (x == 1 && y == 2)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreMiddle>();
							tile.TileFrameX = 234;
							tile.TileFrameY = 54;
							tile.HasTile = true;
							continue;
						}
						tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
						tile.TileFrameX = (short)(x * 18);
						tile.TileFrameY = (short)(y * 18);
						tile.HasTile = true;
					}
				}
				break;
		}
	}

	/// <summary>
	/// 以ij为左上点放置青缎矿3x2
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	public static void PlaceSmallCyanVineOre(int i, int j)
	{
		switch (GenRand.Next(3))
		{
			case 0:
				for (int x = 0; x < 2; x++)
				{
					for (int y = 0; y < 2; y++)
					{
						var tile = SafeGetTile(i + x, j + y);
						if (x == 0 && y == 0)
						{
							continue;
						}

						if (x == 0 && y == 1)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreSmall>();
							tile.TileFrameX = 0;
							tile.TileFrameY = 36;
							tile.HasTile = true;
							continue;
						}
						tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
						tile.TileFrameX = (short)(x * 18);
						tile.TileFrameY = (short)(y * 18);
						tile.HasTile = true;
					}
				}
				break;
			case 1:
				for (int x = 0; x < 2; x++)
				{
					for (int y = 0; y < 2; y++)
					{
						var tile = SafeGetTile(i + x, j + y);

						if (x == 0 && y == 0)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.Slope = SlopeType.SlopeDownRight;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						if (x == 1 && y == 0)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.Slope = SlopeType.SlopeDownLeft;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						if (x == 0 && y == 1)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreSmall>();
							tile.TileFrameX = 54;
							tile.TileFrameY = 36;
							tile.HasTile = true;
							continue;
						}
						tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
						tile.TileFrameX = (short)(x * 18);
						tile.TileFrameY = (short)(y * 18);
						tile.HasTile = true;
					}
				}
				break;
			case 2:
				for (int x = 0; x < 3; x++)
				{
					for (int y = 0; y < 2; y++)
					{
						var tile = SafeGetTile(i + x, j + y);

						if (x == 0 && y == 0)
						{
							continue;
						}

						if (x == 2 && y == 0)
						{
							continue;
						}

						if (x == 2 && y == 1)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
							tile.Slope = SlopeType.SlopeDownLeft;
							tile.TileFrameX = (short)(x * 18);
							tile.TileFrameY = (short)(y * 18);
							tile.HasTile = true;
							continue;
						}
						if (x == 0 && y == 1)
						{
							tile.TileType = (ushort)ModContent.TileType<CyanVineOreSmall>();
							tile.Slope = SlopeType.SlopeDownRight;
							tile.TileFrameX = 108;
							tile.TileFrameY = 36;
							tile.HasTile = true;
							continue;
						}
						tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
						tile.TileFrameX = (short)(x * 18);
						tile.TileFrameY = (short)(y * 18);
						tile.HasTile = true;
					}
				}
				break;
		}
	}

	/// <summary>
	/// 以ij为左上点放置青缎矿3x2
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	public static void PlaceSmallUpCyanVineOre(int i, int j)
	{
		switch (GenRand.Next(4))
		{
			case 0:
				{
					var tile = SafeGetTile(i + 1, j);
					tile.TileType = (ushort)ModContent.TileType<CyanVineOreSmallUp>();
					tile.TileFrameX = 18;
					tile.TileFrameY = 0;
					tile.HasTile = true;

					var tileII = SafeGetTile(i + 1, j + 1);
					tileII.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tileII.TileFrameX = 18;
					tileII.TileFrameY = 18;
					tileII.Slope = SlopeType.SlopeUpRight;
					tileII.HasTile = true;
				}

				break;
			case 1:
				{
					var tile = SafeGetTile(i + 1, j);
					tile.TileType = (ushort)ModContent.TileType<CyanVineOreSmallUp>();
					tile.TileFrameX = 72;
					tile.TileFrameY = 0;
					tile.HasTile = true;

					var tileII = SafeGetTile(i + 1, j + 1);
					tileII.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tileII.TileFrameX = 18;
					tileII.TileFrameY = 18;
					tileII.HasTile = true;
				}
				break;
			case 2:
				{
					var tile = SafeGetTile(i + 1, j);
					tile.TileType = (ushort)ModContent.TileType<CyanVineOreSmallUp>();
					tile.TileFrameX = 126;
					tile.TileFrameY = 0;
					tile.HasTile = true;

					var tileII = SafeGetTile(i + 1, j + 1);
					tileII.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tileII.TileFrameX = 18;
					tileII.TileFrameY = 18;
					tileII.HasTile = true;

					var tileIII = SafeGetTile(i, j);
					tileIII.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tileIII.TileFrameX = 0;
					tileIII.TileFrameY = 0;
					tileIII.Slope = SlopeType.SlopeUpRight;
					tileIII.HasTile = true;

					var tileIV = SafeGetTile(i + 2, j);
					tileIV.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tileIV.TileFrameX = 36;
					tileIV.TileFrameY = 0;
					tileIV.Slope = SlopeType.SlopeUpLeft;
					tileIV.HasTile = true;
				}
				break;
			case 3:
				{
					var tile = SafeGetTile(i + 1, j);
					tile.TileType = (ushort)ModContent.TileType<CyanVineOreSmallUp>();
					tile.TileFrameX = 180;
					tile.TileFrameY = 0;
					tile.HasTile = true;

					var tileII = SafeGetTile(i + 1, j + 1);
					tileII.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tileII.TileFrameX = 18;
					tileII.TileFrameY = 18;
					tileII.Slope = SlopeType.SlopeUpRight;
					tileII.HasTile = true;

					var tileIII = SafeGetTile(i, j);
					tileIII.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tileIII.TileFrameX = 0;
					tileIII.TileFrameY = 0;
					tileIII.HasTile = true;

					var tileIV = SafeGetTile(i + 2, j);
					tileIV.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tileIV.TileFrameX = 36;
					tileIV.TileFrameY = 0;
					tileIV.Slope = SlopeType.SlopeUpLeft;
					tileIV.HasTile = true;
				}
				break;
		}
	}

	/// <summary>
	/// 以ij为左上点放置青缎矿5x3
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	public static void PlaceLargeUpCyanVineOre(int i, int j)
	{
		for (int x = 1; x < 5; x++)
		{
			for (int y = 0; y < 3; y++)
			{
				var tile = SafeGetTile(i + x, j + y);
				if (x == 1 && y == 2)
				{
					continue;
				}

				if (x == 3 && y == 2)
				{
					continue;
				}

				if (x == 4 && y == 2)
				{
					continue;
				}

				if (x == 1 && y == 1)
				{
					tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tile.Slope = SlopeType.SlopeUpRight;
					tile.TileFrameX = (short)(x * 18);
					tile.TileFrameY = (short)(y * 18);
					tile.HasTile = true;
					continue;
				}
				if (x == 4 && y == 1)
				{
					tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
					tile.Slope = SlopeType.SlopeUpLeft;
					tile.TileFrameX = (short)(x * 18);
					tile.TileFrameY = (short)(y * 18);
					tile.HasTile = true;
					continue;
				}
				if (x == 2 && y == 0)
				{
					tile.TileType = (ushort)ModContent.TileType<CyanVineOreLargeUp>();
					tile.TileFrameX = 36;
					tile.TileFrameY = 0;
					tile.HasTile = true;
					continue;
				}
				tile.TileType = (ushort)ModContent.TileType<CyanVineOreTile>();
				tile.TileFrameX = (short)(x * 18);
				tile.TileFrameY = (short)(y * 18);
				tile.HasTile = true;
			}
		}
	}

	/// <summary>
	/// 生成一个灯塔木森林环境专属的宝箱
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void PlaceLampWoodBiomeChest(int x, int y)
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				Tile tile = SafeGetTile(i + x, y - j);
				if (tile.HasTile)
				{
					tile.ClearEverything();
				}
			}
		}

		int type = ModContent.TileType<LampWood_Chest>();
		WorldGenMisc.PlaceChest(x, y, (ushort)type, LampWoodChestContents());
	}

	public static List<Item> LampWoodChestContents()
	{
		List<Item> chestContents = new List<Item>();
		int mainItem = WorldGen.genRand.Next(7);

		// 尽可能出现不同奖励
		switch (mainItem)
		{
			case 0:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<AmberMagicOrb>(), 1));
				break;
			case 1:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<YggdrasilAmberLaser>(), 1));
				break;
			case 2:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<SevenShotGun>(), 1));
				break;
			case 3:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<YggdrasilStoneGyroscope>(), 1));
				break;
			case 4:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<LampWoodPollenBottle>(), 1));
				break;
			case 5:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<CelticSeal>(), 1));
				break;
			case 6:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<ConcealSpell>(), 1));
				break;
		}

		// 金币
		if (WorldGen.genRand.NextBool(5))
		{
			chestContents.Add(new Item(setDefaultsToType: ItemID.GoldCoin, WorldGen.genRand.Next(1, 3)));
		}

		// 绳子
		chestContents.Add(new Item(setDefaultsToType: ItemID.Rope, WorldGen.genRand.Next(70, 151)));

		// 药水
		int potionType = 1;
		switch (WorldGen.genRand.Next(5))
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
		}
		chestContents.Add(new Item(setDefaultsToType: potionType, WorldGen.genRand.Next(1, 4)));

		// 荧光棒
		if (WorldGen.genRand.NextBool(2))
		{
			if (WorldGen.genRand.NextBool(5))
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.StickyGlowstick, WorldGen.genRand.Next(20, 61)));
			}
			else
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.Glowstick, WorldGen.genRand.Next(20, 61)));
			}
		}
		return chestContents;
	}

	/// <summary>
	/// Generate a list of item to fill a twilight cell room chest.
	/// </summary>
	/// <returns></returns>
	public static List<Item> TwilightNormalChestContents()
	{
		List<Item> chestContents = new List<Item>();
		int mainItem = WorldGen.genRand.Next(8);

		// 尽可能出现不同奖励
		switch (mainItem)
		{
			case 0:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<LampWoodSeed>(), 1));
				break;
			case 1:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<RecoveryBand>(), 1));
				break;
			case 2:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<GlowstickLauncher>(), 1));
				break;
			case 3:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<NightfireStaff>(), 1));
				break;
			case 4:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<MidwinterNightmare>(), 1));
				break;
			case 5:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<HexaCrystalStaff>(), 1));
				break;
			case 6:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<TwilightCrystalMill_Item>(), 1));
				break;
			case 7:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<TwilightRod>(), 1));
				break;
		}
		chestContents.AddRange(NormalChestContents());
		return chestContents;
	}

	public static List<Item> NormalChestContents()
	{
		List<Item> contents = new List<Item>();

		// Gold coin
		if (WorldGen.genRand.NextBool(5))
		{
			contents.Add(new Item(setDefaultsToType: ItemID.GoldCoin, WorldGen.genRand.Next(1, 3)));
		}

		// Rope
		contents.Add(new Item(setDefaultsToType: ModContent.ItemType<NylonRope>(), WorldGen.genRand.Next(70, 151)));

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
		contents.Add(new Item(setDefaultsToType: potionType, WorldGen.genRand.Next(1, 4)));

		// glow stick
		if (WorldGen.genRand.NextBool(2))
		{
			if (WorldGen.genRand.NextBool(5))
			{
				contents.Add(new Item(setDefaultsToType: ItemID.StickyGlowstick, WorldGen.genRand.Next(20, 61)));
			}
			else
			{
				contents.Add(new Item(setDefaultsToType: ItemID.Glowstick, WorldGen.genRand.Next(20, 61)));
			}
		}

		// yggdrasil Amber
		if (WorldGen.genRand.NextBool(5))
		{
			contents.Add(new Item(setDefaultsToType: ModContent.ItemType<YggdrasilTown.Items.Materials.YggdrasilAmber>(), WorldGen.genRand.Next(5, 15)));
		}

		// cyan ore
		if (WorldGen.genRand.NextBool(5))
		{
			contents.Add(new Item(setDefaultsToType: ModContent.ItemType<CyanVineOre>(), WorldGen.genRand.Next(15, 75)));
		}
		return contents;
	}

	/// <summary>
	/// 生成一个暮光林地遗迹专属的宝箱
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void PlaceTwilightLegacyBiomeChest(int x, int y)
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				Tile tile = SafeGetTile(i + x, y - j);
				if (tile.HasTile)
				{
					tile.ClearEverything();
				}
			}
		}
		List<Item> chestContents = new List<Item>();
		if (TwilightBonusList.Count > 0)
		{
			int mainItem = WorldGen.genRand.Next(TwilightBonusList.Count);

			// 尽可能出现不同奖励
			chestContents.Add(new Item(setDefaultsToType: TwilightBonusList[mainItem], 1));
			TwilightBonusList.RemoveAt(mainItem);
		}

		// 金币
		if (WorldGen.genRand.NextBool(4))
		{
			chestContents.Add(new Item(setDefaultsToType: ItemID.GoldCoin, WorldGen.genRand.Next(3, 12)));
		}

		// 宝石
		for (int i = 0; i < 4; i++)
		{
			if (WorldGen.genRand.NextBool(12))
			{
				chestContents.Add(new Item(setDefaultsToType: VanillaJuniorGems[GenRand.Next(VanillaJuniorGems.Count)], WorldGen.genRand.Next(15, WorldGen.genRand.Next(17, WorldGen.genRand.Next(19, 75)))));
			}
		}

		// 绳子
		chestContents.Add(new Item(setDefaultsToType: ItemID.Rope, WorldGen.genRand.Next(70, 151)));

		// 药水
		int potionType = 1;
		switch (WorldGen.genRand.Next(5))
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
		}
		chestContents.Add(new Item(setDefaultsToType: potionType, WorldGen.genRand.Next(1, 4)));
		if (WorldGen.genRand.NextBool(2))
		{
			if (WorldGen.genRand.NextBool(5))
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.StickyGlowstick, WorldGen.genRand.Next(20, 61)));
			}
			else
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.Glowstick, WorldGen.genRand.Next(20, 61)));
			}
		}
		int type = 21;

		WorldGenMisc.PlaceChest(x, y, (ushort)type, chestContents, 0);
	}

	public static void AddTwilightLegacyBonusContain(List<int> bonus, int times = 1)
	{
		List<int> potentialList = new List<int>()
		{
			ItemID.CobaltShield,
			ItemID.LuckyHorseshoe,
			ItemID.Spear,
			ItemID.AnkletoftheWind,
			ItemID.Trident,
			ItemID.DivingHelmet,
			ItemID.FeralClaws,
			ItemID.Shackle,
			ItemID.Handgun,
			ItemID.AquaScepter,
			ItemID.Starfury,
			ItemID.Vilethorn,
			ItemID.CrimsonRod,
			ItemID.Harpoon,
			ItemID.SlimeStaff,
			ItemID.CloudinaBottle,
			ItemID.BandofRegeneration,
			ItemID.HermesBoots,
			ItemID.BandofStarpower,
			ItemID.Aglet,
			ItemID.Compass,
			ItemID.Flipper,
			ItemID.ShinyRedBalloon,
			ItemID.DepthMeter,
			ItemID.Umbrella,
		};

		for (int i = 0; i < times; i++)
		{
			bonus.Add(potentialList[GenRand.Next(potentialList.Count)]);
		}
	}
}
