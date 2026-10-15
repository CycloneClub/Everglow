using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.Minortopography.GiantPinetree.Items;
using Everglow.Minortopography.GiantPinetree.TilesAndWalls;
using Everglow.Minortopography.GiantPinetree.TilesAndWalls.Elevator;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Everglow.Minortopography.GiantPinetree;

public class GiantPinetree : ModSystem
{
	private class GiantPinetreeGenPass : GenPass
	{
		public GiantPinetreeGenPass()
			: base("GiantPinetree", 300)
		{
		}

		public override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			// TODO:翻译：建造巨大的雪松
			BuildGiantPinetree();
			HasBeenLootChest = new List<int>();
		}
	}

	public static List<int> HasBeenLootChest = new List<int>();

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) => tasks.Add(new GiantPinetreeGenPass());

	/// <summary>
	/// 在雪地表面随机获取一点
	/// </summary>
	/// <returns></returns>
	public static Point16 RandomPointInSurfaceSnow()
	{
		var aimPoint = new List<Point16>();
		int minYCoord = Main.maxTilesY - 100;
		for (int i = 33; i < Main.maxTilesX - 34; i += 33)
		{
			for (int buildCoordY = 12; buildCoordY < Main.maxTilesY - 100; buildCoordY += 6)
			{
				Tile tile = TileUtils.SafeGetTile(i, buildCoordY);
				if (tile.TileType == TileID.SnowBlock && tile.HasTile && tile.LiquidAmount == 0)
				{
					bool hasAirIsland = false;
					for (int j = 0; j < 100; j++)
					{
						Tile tileCheckCloud = TileUtils.SafeGetTile(i, buildCoordY - j);
						if (tileCheckCloud.TileType == TileID.Cloud)
						{
							hasAirIsland = true;
							break;
						}
					}
					if (hasAirIsland)
					{
						break;
					}
					aimPoint.Add(new Point16(i, buildCoordY));
					if (buildCoordY < minYCoord)
					{
						minYCoord = buildCoordY;
					}

					break;
				}
			}
		}
		var newAimPoint = new List<Point16>();
		foreach (Point16 point in aimPoint)
		{
			if (point.Y <= minYCoord + 30)
			{
				newAimPoint.Add(point);
			}
		}
		if (newAimPoint.Count == 0)
		{
			return new Point16(0, 0);
		}
		return newAimPoint[WorldGen.genRand.Next(newAimPoint.Count)];
	}

	/// <summary>
	/// 建造巨大的雪松
	/// </summary>
	public static void BuildGiantPinetree()
	{
		Point16 centerPoint = RandomPointInSurfaceSnow();
		if (centerPoint == new Point16(0, 0))
		{
			return;
		}

		// 避免小世界生成位置突兀
		int positonX = Main.maxTilesX == 4200 ? centerPoint.X + 2 : centerPoint.X;

		// 降低高度
		int positonY = centerPoint.Y - 8;

		float treeSize = WorldGen.genRand.NextFloat(16f, 20f);

		// PlacePineLeaves(positonX, positonY, 0, treeSize * 7.5f, new Vector2(0, -1));
		PlacePineLeavesStyleII(positonX, positonY - 16);
		if (Main.snowBG[2] == 260)// 在这种条件下，背景符合这段代码生成的松树
		{
		}
		float trunkWidth = treeSize; // 随机摇宽度
		for (int a = -3; a <= 3; a++)
		{
			GenerateRoots(new Point16(positonX, positonY), 0, a / 2f); // 随机发射树根
		}

		int killCoordY = -13;
		while (trunkWidth > 0)
		{
			killCoordY--;
			if (killCoordY + positonY >= Main.maxTilesY - 10 || killCoordY + positonY <= 10 || positonX <= 20 || positonX >= Main.maxTilesX - 20)// 防止超界
			{
				break;
			}

			for (int i = (int)-trunkWidth; i <= (int)trunkWidth; i++)
			{
				Tile tile = TileUtils.SafeGetTile(i + positonX, killCoordY + positonY);
				if (i > -trunkWidth + 4 || i < trunkWidth - 4 || trunkWidth < 4)
				{
					if (WorldGen.genRand.Next(Math.Abs(i)) < trunkWidth - 6)
					{
						tile.wall = (ushort)ModContent.WallType<PineLeavesWall>();
					}
					if (WorldGen.genRand.Next(Math.Abs(i)) < trunkWidth - 6)
					{
						tile.HasTile = false;
					}
				}
			}
			trunkWidth -= (float)(Math.Sin(killCoordY * 0.8) * 0.5 + 0.4);
		}
		GenerateTrunkWall(new Point16(positonX, positonY + 6), 7, 66);
		GenerateBranch(new Point16(positonX, positonY - 20), -1.4f, 26);
		GenerateBranch(new Point16(positonX, positonY - 36), 1.4f, 20);
		GeneratePineTreeHouse(new Point16(positonX - 15, positonY - 22), WorldGen.genRand.Next(11, 15), WorldGen.genRand.Next(11, 15), 13, new int[] { 0, WorldGen.genRand.Next(5, 7), WorldGen.genRand.Next(1620, 1980), 185, -1, 255, WorldGen.genRand.Next(5, 7), 0 });
		GeneratePineTreeHouse(new Point16(positonX + 12, positonY - 38), WorldGen.genRand.Next(11, 15), WorldGen.genRand.Next(11, 15), 13, new int[] { 1, WorldGen.genRand.Next(5, 7), WorldGen.genRand.Next(1620, 1980), 185, 1, -8, 0, 0 });
		GeneratePineTreeHouse(new Point16(positonX + WorldGen.genRand.Next(-3, 4), positonY - 68), 5, 4, 9, new int[] { 0, WorldGen.genRand.Next(3, 5), WorldGen.genRand.Next(810, 1000), 185, 0, 255, 0, 1 });

		// 清除被破坏的树木
		RemoveDestroyedTrees(positonX, positonY);

		// 平滑木头部分
		SmoothTile_XXYY(positonX - 60, positonY - 20, 120, 250, ModContent.TileType<PineWood>());
		DistributePineCone(new Point16(positonX, positonY - 10), 40, 40);
		DistributePineCone(new Point16(positonX, positonY - 60), 20, 20);
		DistributePineNeedle(new Point16(positonX, positonY), 120, 120);
		DistributePineCone(new Point16(positonX, positonY), 80, 80);
		DistributeLittlePineCone(700, new Point16(positonX, positonY - 90), 1800);
		DistributeMilkCap(new Point16(positonX - 40, positonY - 100), 80, 120);
	}

	/// <summary>
	/// 建造根系,起始角度=0时向下,根系会在起始位置以起始角度发射,并逐渐转向目标角度,如果天然卷曲,根系会在末尾处再发生一次拐弯
	/// </summary>
	/// <param name="startPoint"></param>
	/// <param name="startRotation"></param>
	/// <param name="trendRotation"></param>
	/// <param name="naturalCurve"></param>
	public static void GenerateRoots(Point16 startPoint, float startRotation = 0, float trendRotation = 0, bool naturalCurve = true)
	{
		int positonX = startPoint.X;
		int positonY = startPoint.Y;
		float trunkWidth = WorldGen.genRand.NextFloat(8f, 10f); // 随机摇宽度
		var rootPosition = new Vector2(0, 0);
		Vector2 rootVelocity = new Vector2(0, 1).RotatedBy(startRotation); // 根系当前速度
		Vector2 rootTrendVelocity = new Vector2(0, 1).RotatedBy(trendRotation); // 根系稳定趋势速度
		float omega = WorldGen.genRand.NextFloat(-0.2f, 0.2f); // 末端旋转的角速度
		if (!naturalCurve)// 如果禁止了自然旋转,角速度=0
		{
			omega = 0;
		}

		float startToRotatedByOmega = WorldGen.genRand.NextFloat(1.81f, 3.62f); // 算作末端的起始位置，这里用剩余宽度统计
		while (trunkWidth > 0)
		{
			for (int a = (int)-trunkWidth; a <= (int)trunkWidth; a++)
			{
				Vector2 tilePosition = rootPosition + a * rootVelocity.RotatedBy(MathHelper.PiOver2) * 0.6f;
				int i = (int)tilePosition.X;
				int buildCoordY = (int)tilePosition.Y;
				if (buildCoordY + positonY >= Main.maxTilesY - 10 || buildCoordY + positonY <= 10 || -10 + positonX <= 10 || 10 + positonX >= Main.maxTilesX + 10)// 防止超界
				{
					break;
				}

				Tile tile = TileUtils.SafeGetTile(i + positonX, buildCoordY + positonY);
				if (a <= -trunkWidth + 4 || a >= trunkWidth - 4)// 在靠边的部位为实木块
				{
					if (tile.WallType != ModContent.WallType<PineWoodWall>())// 防止松树块互相重合
					{
						tile.TileType = (ushort)ModContent.TileType<PineWood>();
						tile.HasTile = true;
					}
				}
				else// 空心根管
				{
					tile.ClearEverything();
				}
				if (a > -trunkWidth + 2 && a < trunkWidth - 2)// 铺上墙壁
				{
					tile.WallType = (ushort)ModContent.WallType<PineWoodWall>();
				}
			}
			rootPosition += rootVelocity;
			if (trunkWidth > startToRotatedByOmega)// 没有收束到末端
			{
				rootVelocity = rootVelocity * 0.95f + rootTrendVelocity * 0.05f;
			}
			else// 已经收束到末端
			{
				rootVelocity = rootVelocity.RotatedBy(omega * (startToRotatedByOmega - trunkWidth) / startToRotatedByOmega);
			}
			if (naturalCurve)// 只有自然卷曲才会导致以下现象
			{
				// 重力因素也会影响根系,下面判定根系悬空程度
				int surroundTileCount = 0; // 我们判定周围存在方块的数量来推断悬空程度，存在方块越少越悬空
				for (int b = 0; b < 12; b++)
				{
					Vector2 tilePosition = rootPosition + 3 * rootVelocity.RotatedBy(b / 6d * Math.PI);
					int i = (int)tilePosition.X;
					int buildCoordY = (int)tilePosition.Y;
					Tile tile = TileUtils.SafeGetTile(i + positonX, buildCoordY + positonY);
					if (tile.HasTile || tile.WallType == (ushort)ModContent.WallType<PineWoodWall>()/*这一项是为了防止自己干扰自己*/)
					{
						surroundTileCount++;
					}
				}
				if (surroundTileCount < 6)
				{
					rootVelocity += new Vector2(0, (6 - surroundTileCount) / 16f); // 重力自然下垂
					rootVelocity = Vector2.Normalize(rootVelocity); // 化作单位向量
					trunkWidth += (6 - surroundTileCount) / 50f; // 防止下降过程根系过分收束
				}
				else if (surroundTileCount > 9)
				{
					trunkWidth -= (surroundTileCount - 9) / 60f; // 周围物块太多，产生阻力，加快收束
				}
			}
			trunkWidth -= 0.1f;
			if (trunkWidth < 1.8f)// 太细了，准备破
			{
				if (trunkWidth < 1.8f)// 破掉吧
				{
					break;
				}
			}
		}
	}

	/// <summary>
	/// 根据分型生成树叶
	/// </summary>
	/// <param name="i"></param>
	/// <param name="buildCoordY"></param>
	/// <param name="iteration"></param>
	/// <param name="strength"></param>
	/// <param name="direction"></param>
	public static void PlacePineLeaves(int i, int buildCoordY, int iteration, float strength, Vector2 direction)
	{
		if (iteration > 50)// 万一发散就完了
		{
			return;
		}

		for (int x = 0; x < strength; x++)
		{
			int aBSXStr = Math.Min((int)((strength - x) * 0.16f), 8);

			for (int y = -aBSXStr; y < aBSXStr + 1; y++)
			{
				Vector2 normalizedDirection = Utils.SafeNormalize(direction, new Vector2(0, -1));
				Vector2 VnormalizedDirection = normalizedDirection.RotatedBy(Math.PI / 2d);
				int a = (int)(i + normalizedDirection.X * x + VnormalizedDirection.X * y);
				int b = (int)(buildCoordY + normalizedDirection.Y * x + VnormalizedDirection.Y * y);
				if (b >= Main.maxTilesY - 10 || b <= 10 || a <= 20 || a >= Main.maxTilesX - 20)// 防止超界
				{
					break;
				}

				var tile = TileUtils.SafeGetTile(a, b);
				tile.TileType = (ushort)ModContent.TileType<PineLeaves>();
				tile.HasTile = true;
				if (strength - x > 1)
				{
					tile.WallType = (ushort)ModContent.WallType<PineLeavesWall>();
				}

				if (y == 0)
				{
					if (x % 6 == 1)
					{
						PlacePineLeaves(a, b, iteration + 1, (strength - x) * 0.34f, normalizedDirection.RotatedBy(Math.PI * 0.3));
					}

					if (x % 6 == 4)
					{
						PlacePineLeaves(a, b, iteration + 1, (strength - x) * 0.34f, normalizedDirection.RotatedBy(-Math.PI * 0.3));
					}
				}
			}
		}
	}

	/// <summary>
	/// 第二种方式生成树叶
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void PlacePineLeavesStyleII(int x, int y)
	{
		void LineTileRunner(int type, Vector2 pos, Vector2 vel, int length)
		{
			Point p = pos.ToTileCoordinates();
			vel.Normalize();
			for (int i = 0; i < length; i++)
			{
				Tile tile = TileUtils.SafeGetTile(p.X + (int)(vel.X * i), p.Y + (int)(vel.Y * i));
				tile.TileType = (ushort)type;
				tile.HasTile = true;
				tile.wall = (ushort)ModContent.WallType<PineLeavesWall>();
				tile = TileUtils.SafeGetTile(p.X + (int)(vel.X * i) + WorldGen.genRand.Next(-1, 2), p.Y + (int)(vel.Y * i) + WorldGen.genRand.Next(-1, 2));
				tile.wall = (ushort)ModContent.WallType<PineLeavesWall>();
				if (i <= length - 1)
				{
					if (WorldGen.genRand.NextBool(2))
					{
						tile = TileUtils.SafeGetTile(p.X + (int)(vel.X * i), p.Y + (int)(vel.Y * i) + 1);
						tile.TileType = (ushort)type;
						tile.HasTile = true;
					}
					else
					{
						tile.wall = (ushort)ModContent.WallType<PineLeavesWall>();
					}
				}
				if (i <= length - 3)
				{
					if (WorldGen.genRand.NextBool(2))
					{
						tile = TileUtils.SafeGetTile(p.X + (int)(vel.X * i), p.Y + (int)(vel.Y * i) + 1);
						tile.TileType = (ushort)type;
						tile.HasTile = true;
					}
					else
					{
						tile.wall = (ushort)ModContent.WallType<PineLeavesWall>();
					}
				}
			}
		}

		void TileFrame(int sx, int sy, int w, int h)
		{
			for (int i = 0; i < w; i++)
			{
				for (int j = 0; j < h; j++)
				{
					WorldGen.TileFrame(sx + i, sy + j);
				}
			}
		}
		Vector2 bottomPosition = new Vector2(x, y) * 16;
		float ang = 0.15f; // 初始角度
		float mainWidth = 24; // 中间部分宽度

		float length = 60;
		for (int i = 0; i < 40; i++)
		{
			Vector2 cPos = bottomPosition + new Vector2(0, -i * 32);
			Vector2 vel = ang.ToRotationVector2();
			float factor = 1 - i / 40f;
			ang += 0.005f; // 角度变化
			length *= 0.75f; // 长度衰减

			if (WorldGen.genRand.Next(100) < 40 && length < mainWidth * factor)
			{
				length += 60 * factor;
			}

			float min = 3 * factor;
			if (length < min)
			{
				length = min;
			}

			LineTileRunner(ModContent.TileType<PineLeaves>(), cPos, vel, (int)length);
			vel.X *= -1;
			LineTileRunner(ModContent.TileType<PineLeaves>(), cPos, vel, (int)length);
		}

		Rectangle frameRec = new Rectangle(x - 75, y - 150, 150, 200);
		TileFrame(frameRec.X, frameRec.Y, frameRec.Width, frameRec.Height);
	}

	/// <summary>
	/// 生成枝干
	/// </summary>
	/// <param name="startPoint"></param>
	/// <param name="startRotation"></param>
	/// <param name="trendRotation"></param>
	/// <param name="naturalCurve"></param>
	public static void GenerateBranch(Point16 startPoint, float startRotation = 0, float expectLength = 40)
	{
		int positonX = startPoint.X;
		int positonY = startPoint.Y;
		float trunkWidth = WorldGen.genRand.NextFloat(2f, 3f); // 随机摇宽度
		var branchPosition = new Vector2(0, 0);
		Vector2 branchVelocity = new Vector2(0, -1).RotatedBy(startRotation); // 根系当前速度
		Vector2 branchTrendVelocity = branchVelocity; // 根系稳定趋势速度
		int iteration = 0;
		while (trunkWidth > 0)
		{
			iteration++;
			for (int a = (int)-trunkWidth; a <= (int)trunkWidth; a++)
			{
				Vector2 tilePosition = branchPosition + a * branchVelocity.RotatedBy(MathHelper.PiOver2) * 0.6f;
				int i = (int)tilePosition.X;
				int buildCoordY = (int)tilePosition.Y;
				if (buildCoordY + positonY >= Main.maxTilesY - 10 || buildCoordY + positonY <= 10 || -10 + positonX <= 10 || 10 + positonX >= Main.maxTilesX + 10)// 防止超界
				{
					break;
				}

				Tile tile = TileUtils.SafeGetTile(i + positonX, buildCoordY + positonY);
				if (a <= -trunkWidth + 4 || a >= trunkWidth - 4)// 在靠边的部位为实木块
				{
					tile.TileType = (ushort)ModContent.TileType<PineWood>();
					tile.HasTile = true;
				}
				else// 空心根管
				{
					tile.HasTile = false;
					tile.LiquidAmount = 0;
				}
				if (a > -trunkWidth + 2 && a < trunkWidth - 2)// 铺上墙壁
				{
					tile.WallType = (ushort)ModContent.WallType<PineWoodWall>();
				}
			}
			branchPosition += branchVelocity;
			if (iteration > expectLength)// 已经收束到末端
			{
				branchVelocity = branchVelocity * 0.75f + branchTrendVelocity * 0.25f;
				trunkWidth -= 0.4f;
			}
			else// 没有收束到末端
			{
				branchVelocity = branchVelocity * 0.8f + new Vector2(Math.Sign(branchVelocity.X), 0) * 0.2f;
			}

			if (trunkWidth < 0.8f)// 太细了，准备破
			{
				if (trunkWidth < 0.8f)// 破掉吧
				{
					break;
				}
			}
		}
	}

	/// <summary>
	/// 生成树屋,styleSeed是随机种子,复杂算法警告！
	/// style:
	/// [0] 外框架的样式 0盒子 1套筒
	/// [1] 尖顶超出外框架的长度
	/// [2] 尖顶高度的100分之一
	/// [3] 尖顶弧度>100内凹<100外凸
	/// [4] 门口位置 -1门在右 1门在左
	/// [5] 电梯位置 相较于中间的位置，电梯生成于房屋底部 绝对值大于等于255则不生成电梯
	/// [6] 窗户
	/// </summary>
	/// <param name="origin"></param>
	/// <param name="leftWidth"></param>
	/// <param name="rightWidth"></param>
	/// <param name="height"></param>
	/// <param name="styleSeed"></param>
	public static void GeneratePineTreeHouse(Point16 origin, int leftWidth, int rightWidth, int height, int[] styleSeed)
	{
		int positonX = origin.X;
		int positonY = origin.Y;
		int leftX = positonX - leftWidth;
		int rightX = positonX + rightWidth;
		int topY = origin.Y - height;

		// 生成火柴盒，有一定的逻辑
		void SummonBox(Point16 boxOrigin, int boxWidth, int boxHeight, int wallType = -1, int edgeThick = 1)
		{
			for (int i = boxOrigin.X; i <= boxOrigin.X + boxWidth; i++)
			{
				// 高度为-1的话就是一个套筒
				if (boxHeight == -1)
				{
					int j = boxOrigin.Y;
					while (TileUtils.SafeGetTile(i, j).TileType != ModContent.TileType<PineWood>())
					{
						Tile tile = TileUtils.SafeGetTile(i, j);
						if (i >= boxOrigin.X + 1 || i <= boxOrigin.X + boxWidth - 1 || j >= boxOrigin.Y + 1 || j <= boxOrigin.Y + 11 - edgeThick)
						{
							if (wallType != -1)
							{
								tile.wall = (ushort)wallType;
							}
							else
							{
								tile.ClearEverything();
							}
						}
						if (i <= boxOrigin.X - 1 + edgeThick || i >= boxOrigin.X + boxWidth + 1 - edgeThick || j <= boxOrigin.Y - 1 + edgeThick)
						{
							tile.TileType = (ushort)ModContent.TileType<PineWood>();
							tile.HasTile = true;
						}
						else
						{
							tile.HasTile = false;
						}
						j++;

						// 套筒也要检测不能套太深
						if (j > origin.Y + 4)
						{
							break;
						}
					}
				}
				else
				{
					for (int j = boxOrigin.Y; j <= boxOrigin.Y + boxHeight; j++)
					{
						Tile tile = TileUtils.SafeGetTile(i, j);
						if (i >= boxOrigin.X + 1 || i <= boxOrigin.X + boxWidth - 1 || j >= boxOrigin.Y + 1 || j <= boxOrigin.Y + boxHeight - 1)
						{
							if (wallType != -1)
							{
								tile.wall = (ushort)wallType;
							}
							else
							{
								tile.ClearEverything();
							}
						}
						if (i <= boxOrigin.X - 1 + edgeThick || i >= boxOrigin.X + boxWidth + 1 - edgeThick || j <= boxOrigin.Y - 1 + edgeThick || j >= boxOrigin.Y + height + 1 - edgeThick)
						{
							tile.TileType = (ushort)ModContent.TileType<PineWood>();
							tile.HasTile = true;
						}
						else
						{
							tile.HasTile = false;
						}
					}
				}
			}
		}
		if (styleSeed[0] == 0)
		{
			SummonBox(new Point16(leftX, topY), rightWidth + leftWidth, height, ModContent.WallType<PineWoodWall>(), 2);
		}
		if (styleSeed[0] == 1)
		{
			SummonBox(new Point16(leftX, topY), rightWidth + leftWidth, -1, ModContent.WallType<PineWoodWall>(), 2);
		}

		// 生成门
		if (styleSeed[4] == -1)
		{
			for (int i = rightX - 5; i <= rightX; i++)
			{
				int deltaHeight = Math.Min(i - (rightX - 5), 2);
				for (int j = positonY - deltaHeight; j <= positonY; j++)
				{
					Tile tile = TileUtils.SafeGetTile(i, j);
					tile.HasTile = false;
				}
			}
			TileUtils.PlaceFrameImportantTiles(rightX - 1, positonY - 2, 1, 3, ModContent.TileType<SnowPineDoorClosed>());
			for (int i = rightX - 8; i <= rightX - 6; i++)
			{
				int deltaHeight = i - (rightX - 8);
				for (int j = positonY - deltaHeight - 2; j <= positonY - 2; j++)
				{
					Tile tile = TileUtils.SafeGetTile(i, j);
					tile.HasTile = false;
				}
			}
		}
		if (styleSeed[4] == 1)
		{
			for (int i = leftX; i <= leftX + 5; i++)
			{
				int deltaHeight = Math.Min((leftX + 5) - i, 2);
				for (int j = positonY - deltaHeight; j <= positonY; j++)
				{
					Tile tile = TileUtils.SafeGetTile(i, j);
					tile.HasTile = false;
				}
			}
			TileUtils.PlaceFrameImportantTiles(leftX + 1, positonY - 2, 1, 3, ModContent.TileType<SnowPineDoorClosed>());
			for (int i = leftX + 6; i <= leftX + 8; i++)
			{
				int deltaHeight = i - (leftX + 6);
				for (int j = positonY - deltaHeight - 2; j <= positonY - 2; j++)
				{
					Tile tile = TileUtils.SafeGetTile(i, j);
					tile.HasTile = false;
				}
			}
		}

		// 生成尖顶
		for (int i = origin.X - leftWidth - styleSeed[1]; i <= origin.X + rightWidth + styleSeed[1]; i++)
		{
			int length = rightWidth + leftWidth + styleSeed[1] * 2;
			float maxHeight = styleSeed[2] / 100f;
			float baseHeight = (length / 2f - Math.Abs(i - origin.X)) / (length / 2f);
			if (baseHeight <= 0)
			{
				baseHeight = 0;
			}
			int domeHeight = (int)(Math.Pow(baseHeight, styleSeed[3] / 100f) * maxHeight);
			for (int j = origin.Y - height - domeHeight; j <= origin.Y - height; j++)
			{
				Tile tile = TileUtils.SafeGetTile(i, j);
				if (j <= origin.Y - height - domeHeight + 2)
				{
					tile.TileType = (ushort)ModContent.TileType<PineWood>();
					tile.HasTile = true;
				}
				else
				{
					tile.HasTile = false;
				}
				if (j > origin.Y - height - domeHeight + 1)
				{
					tile.wall = (ushort)ModContent.WallType<PineWoodWall>();
				}
			}
		}

		// 生成电梯
		if (Math.Abs(styleSeed[5]) < 255)
		{
			int addJ = 0;
			while (TileUtils.SafeGetTile(origin.X + styleSeed[5], origin.Y + 2 + addJ).TileType == ModContent.TileType<PineWood>())
			{
				addJ++;
				if (addJ > 20)
				{
					break;
				}
			}
			if (addJ != 21)
			{
				for (int addJJ = 0; addJJ < 12; addJJ++)
				{
					for (int addI = -4; addI < 5; addI++)
					{
						Tile tileKill = TileUtils.SafeGetTile(origin.X + styleSeed[5] + addI, origin.Y + 2 + addJ + addJJ);
						tileKill.HasTile = false;
					}
				}
				Tile tile = TileUtils.SafeGetTile(origin.X + styleSeed[5], origin.Y + 2 + addJ);
				tile.TileType = (ushort)ModContent.TileType<GiantPineTreeElevator_Winch>();
				tile.HasTile = true;
			}
		}

		// 生成窗户
		if (Math.Abs(styleSeed[6]) > 0)
		{
			int top = topY + 2;
			int left = leftX + styleSeed[6];
			for (int times = 0; times < 4; times++)
			{
				switch (times)
				{
					case 1:
						top += 4;
						break;
					case 2:
						left += 4;
						break;
					case 3:
						top -= 4;
						break;
					default:
						break;
				}

				for (int x = 0; x < 3; x++)
				{
					for (int y = 0; y < 3; y++)
					{
						Tile tile = TileUtils.SafeGetTile(left + x, top + y);
						tile.wall = WallID.Glass;
					}
				}
			}
		}

		// 生成巨大松塔
		if (Math.Abs(styleSeed[7]) > 0)
		{
			TileUtils.PlaceFrameImportantTiles(leftX + 2, topY + 2, 6, 6, (ushort)ModContent.TileType<GiantPineCone_0>());
		}
		int placeX = WorldGen.genRand.Next(leftX + 3, rightX - 3);
		int placeY = positonY - 8;
		for (int y = 0; y < 15; y++)
		{
			Tile checkTile = TileUtils.SafeGetTile(placeX, placeY + y);
			if (checkTile.HasTile)
			{
				Tile checkTile2 = TileUtils.SafeGetTile(placeX + 1, placeY + y);
				if (checkTile2.HasTile)
				{
					PlacePineChest(placeX, placeY + y - 1);
					break;
				}
				else
				{
					checkTile2.TileType = (ushort)ModContent.TileType<PineWood>();
					checkTile2.HasTile = true;
					PlacePineChest(placeX, placeY + y - 1);
					break;
				}
			}
		}
	}

	public static void PlacePineChest(int x, int y)
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				Tile tile = TileUtils.SafeGetTile(i + x, y - j);
				if (tile.HasTile)
				{
					tile.ClearEverything();
				}
			}
		}
		List<Item> chestContents = new List<Item>();
		int mainItem = WorldGen.genRand.Next(6);
		int counts = 0;

		// 尽可能出现不同奖励
		while (HasBeenLootChest.Contains(mainItem))
		{
			mainItem = WorldGen.genRand.Next(6);
			counts++;
			if (counts > 10)
			{
				break;
			}
		}
		HasBeenLootChest.Add(mainItem);
		switch (mainItem)
		{
			case 0:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<FrostBomb>(), 1));
				break;
			case 1:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<BurningFrozenHeart>(), 1));
				break;
			case 2:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<PearlOfCyan>(), 1));
				break;
			case 3:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<SnowPineLockBox>(), 1));
				break;
			case 4:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<HarvestingClaw>(), 1));
				break;
			case 5:
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<PineStab>(), 1));
				break;
		}

		// 生命针叶魔杖
		if (WorldGen.genRand.NextBool(3))
		{
			chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<SnowPineLeaveStaff>(), 1));
			chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<SnowPineWoodStaff>(), 1));
		}

		// 冰矛
		if (WorldGen.genRand.NextBool(2))
		{
			chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<IcedSpear>(), WorldGen.genRand.Next(80, 201)));
		}

		// 草药袋
		if (WorldGen.genRand.NextBool(7))
		{
			chestContents.Add(new Item(setDefaultsToType: ItemID.HerbBag, WorldGen.genRand.Next(1, 4)));
		}

		// 甜品袋
		if (WorldGen.genRand.NextBool(3))
		{
			chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<DessertBag_Snow>(), WorldGen.genRand.Next(2, 5)));
		}

		// 蠕虫桶
		if (WorldGen.genRand.NextBool(7))
		{
			chestContents.Add(new Item(setDefaultsToType: ItemID.CanOfWorms, WorldGen.genRand.Next(2, 5)));
		}

		// 针叶木
		if (WorldGen.genRand.NextBool(2))
		{
			chestContents.Add(new Item(setDefaultsToType: ItemID.BorealWood, WorldGen.genRand.Next(50, 151)));
		}

		// 冰雪飞鱼
		if (WorldGen.genRand.NextBool(7))
		{
			chestContents.Add(new Item(setDefaultsToType: ItemID.FrostDaggerfish, WorldGen.genRand.Next(80, 201)));
		}

		// 霜火箭
		if (WorldGen.genRand.NextBool(7))
		{
			chestContents.Add(new Item(setDefaultsToType: ItemID.FrostburnArrow, WorldGen.genRand.Next(80, 201)));
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

		// 冰雪火把
		if (WorldGen.genRand.NextBool(2))
		{
			chestContents.Add(new Item(setDefaultsToType: ItemID.IceTorch, WorldGen.genRand.Next(40, 91)));
		}

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
		int type = WorldGen.genRand.Next(3);
		switch (type)
		{
			case 0:
				type = ModContent.TileType<TilesAndWalls.SnowPineChest>();
				break;
			case 1:
				type = ModContent.TileType<TilesAndWalls.SnowPineChest_fresh>();
				break;
			case 2:
				type = ModContent.TileType<TilesAndWalls.SnowPineChest_gold>();
				break;
		}

		WorldGenMisc.PlaceChest(x, y, (ushort)type, chestContents);
	}

	/// <summary>
	/// 随机生成小松塔
	/// </summary>
	/// <param name="length"></param>
	/// <param name="position"></param>
	/// <param name="range"></param>
	public static void DistributePineCone(Point16 position, int leftWidth, int rightWidth)
	{
		for (int time = -leftWidth; time < rightWidth; time++)
		{
			int addJ = 0;
			Point16 newPos = position + new Point16(time, 0);
			while (!TileUtils.SafeGetTile(newPos.X, newPos.Y + addJ).HasTile)
			{
				addJ++;
			}
			newPos = position + new Point16(time, addJ - 2);
			int valid = 9;
			for (int i = 0; i < 3; i++)// 这里是0~2,因为下面手动-1
			{
				for (int j = 0; j < 3; j++)
				{
					Tile tile = TileUtils.SafeGetTile(newPos.X - 1 + i, newPos.Y + j);
					if (!tile.HasTile && j < 2)
					{
						valid--;
					}
					if (tile.HasTile && j == 2)
					{
						if (tile.blockType() == 0 && Main.tileSolid[tile.type])
						{
							valid--;
						}
					}
				}
			}
			if (valid == 0)
			{
				TileUtils.PlaceFrameImportantTiles(newPos.X - 1, newPos.Y, 3, 2, ModContent.TileType<TilesAndWalls.PineCone>(), 54 * WorldGen.genRand.Next(3));
				time += WorldGen.genRand.Next(4);
			}
		}
	}

	/// <summary>
	/// 随机生成松叶堆
	/// </summary>
	/// <param name="length"></param>
	/// <param name="position"></param>
	/// <param name="range"></param>
	public static void DistributePineNeedle(Point16 position, int leftWidth, int rightWidth)
	{
		for (int time = -leftWidth; time < rightWidth; time++)
		{
			int addJ = 0;
			Point16 newPos = position + new Point16(time, 0);
			while (!TileUtils.SafeGetTile(newPos.X, newPos.Y + addJ).HasTile)
			{
				addJ++;
			}
			newPos = position + new Point16(time, addJ - 1);
			int valid = 6;
			for (int i = 0; i < 3; i++)// 这里是0~2,因为下面手动-1
			{
				for (int j = 0; j < 2; j++)
				{
					Tile tile = TileUtils.SafeGetTile(newPos.X - 1 + i, newPos.Y + j);
					if (!tile.HasTile && j == 0)
					{
						valid--;
					}
					if (tile.HasTile && j == 1)
					{
						if (tile.blockType() == 0 && Main.tileSolid[tile.type])
						{
							valid--;
						}
					}
				}
			}
			if (valid == 0)
			{
				TileUtils.PlaceFrameImportantTiles(newPos.X - 1, newPos.Y, 3, 1, ModContent.TileType<TilesAndWalls.DryPineNeedles>(), 54 * WorldGen.genRand.Next(3));
				time += WorldGen.genRand.Next(4);
			}
		}
	}

	/// <summary>
	/// 随机生成小小松塔
	/// </summary>
	/// <param name="length"></param>
	/// <param name="position"></param>
	/// <param name="range"></param>
	public static void DistributeLittlePineCone(int length, Point16 position, int range)
	{
		for (int time = 0; time < length; time++)
		{
			Vector2 addPos = new Vector2(0, WorldGen.genRand.NextFloat(WorldGen.genRand.NextFloat(0, range), range)).RotatedBy(WorldGen.genRand.NextFloat(-1.57f, 1.57f)) / 16f;
			Point16 newPos = position + new Point16((int)addPos.X, (int)addPos.Y);
			Tile tile = TileUtils.SafeGetTile(newPos.X, newPos.Y);
			if (!tile.HasTile)
			{
				List<byte> direction = new List<byte>();
				if (TileUtils.SafeGetTile(newPos.X + 1, newPos.Y).TileType == ModContent.TileType<PineLeaves>() && TileUtils.SafeGetTile(newPos.X + 1, newPos.Y).HasTile)
				{
					direction.Add(1);
				}
				if (TileUtils.SafeGetTile(newPos.X, newPos.Y + 1).TileType == ModContent.TileType<PineLeaves>() && TileUtils.SafeGetTile(newPos.X, newPos.Y + 1).HasTile)
				{
					direction.Add(2);
				}
				if (TileUtils.SafeGetTile(newPos.X - 1, newPos.Y).TileType == ModContent.TileType<PineLeaves>() && TileUtils.SafeGetTile(newPos.X - 1, newPos.Y).HasTile)
				{
					direction.Add(3);
				}
				if (TileUtils.SafeGetTile(newPos.X, newPos.Y - 1).TileType == ModContent.TileType<PineLeaves>() && TileUtils.SafeGetTile(newPos.X, newPos.Y - 1).HasTile)
				{
					direction.Add(4);
				}
				if (TileUtils.SafeGetTile(newPos.X, newPos.Y).wall == ModContent.WallType<PineLeavesWall>() && TileUtils.SafeGetTile(newPos.X + 1, newPos.Y).HasTile)
				{
					direction.Add(0);
				}
				if (direction.Contains(1) && direction.Contains(2))
				{
					direction.Add(5);
				}
				if (direction.Contains(2) && direction.Contains(3))
				{
					direction.Add(6);
				}
				if (direction.Contains(3) && direction.Contains(4))
				{
					direction.Add(7);
				}
				if (direction.Contains(4) && direction.Contains(1))
				{
					direction.Add(8);
				}
				if (direction.Count > 0)
				{
					int frameType = direction[WorldGen.genRand.Next(direction.Count)];
					tile.TileType = (ushort)ModContent.TileType<PineCone_little>();
					tile.HasTile = true;
					switch (frameType)
					{
						case 0:
							tile.frameX = 18;
							tile.frameY = 18;
							break;
						case 1:
							tile.frameX = 0;
							tile.frameY = 18;
							break;
						case 2:
							tile.frameX = 18;
							tile.frameY = 0;
							break;
						case 3:
							tile.frameX = 36;
							tile.frameY = 18;
							break;
						case 4:
							tile.frameX = 18;
							tile.frameY = 36;
							break;
						case 5:
							tile.frameX = 0;
							tile.frameY = 0;
							break;
						case 6:
							tile.frameX = 36;
							tile.frameY = 0;
							break;
						case 7:
							tile.frameX = 36;
							tile.frameY = 36;
							break;
						case 8:
							tile.frameX = 0;
							tile.frameY = 36;
							break;
					}
				}
			}
		}
	}

	public static void DistributeMilkCap(Point16 position, int width, int height)
	{
		for (int j = 0; j < height; j++)
		{
			for (int i = 0; i < width; i++)
			{
				if (!WorldGen.genRand.NextBool(25))
				{
					continue;
				}
				Tile tile = TileUtils.SafeGetTile(i + position.X, j + position.Y);
				if (tile.HasTile)
				{
					if (tile.TileType == ModContent.TileType<TilesAndWalls.PineWood>())
					{
						if (tile.blockType() == 0)
						{
							Tile tileUp = TileUtils.SafeGetTile(i + position.X, j + position.Y - 1);
							if (!tileUp.HasTile)
							{
								tileUp.TileType = (ushort)ModContent.TileType<SaffronMilkCap>();
								tileUp.HasTile = true;
								tileUp.frameX = (short)(18 * WorldGen.genRand.Next(6));
								tileUp.frameY = 0;
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 生成树干的墙壁部分
	/// </summary>
	/// <param name="startPoint"></param>
	/// <param name="width"></param>
	/// <param name="expectLength"></param>
	public static void GenerateTrunkWall(Point16 startPoint, float width = 5, float expectLength = 40)
	{
		int positonX = startPoint.X;
		int positonY = startPoint.Y;
		Vector2 trunkVelocity = new Vector2(0, -1);
		Vector2 trunkPosition = Vector2.zeroVector;
		int iteration = 0;
		while (width > 0)
		{
			iteration++;
			for (int a = (int)-width; a <= (int)width; a++)
			{
				Vector2 tilePosition = trunkPosition + a * trunkVelocity.RotatedBy(MathHelper.PiOver2) * 0.6f;
				int i = (int)tilePosition.X;
				int buildCoordY = (int)tilePosition.Y;
				if (buildCoordY + positonY >= Main.maxTilesY - 10 || buildCoordY + positonY <= 10 || -10 + positonX <= 10 || 10 + positonX >= Main.maxTilesX + 10)// 防止超界
				{
					break;
				}

				Tile tile = TileUtils.SafeGetTile(i + positonX, buildCoordY + positonY);
				if (a > -width && a < width)// 铺上墙壁
				{
					tile.WallType = (ushort)ModContent.WallType<PineWoodWall>();
				}
			}
			trunkPosition += trunkVelocity;
			if (iteration > expectLength)// 已经收束到末端
			{
				width -= 0.4f;
			}
			if (width < 0.8f)// 太细了，准备破
			{
				if (width < 0.8f)// 破掉吧
				{
					break;
				}
			}
		}
	}

	public static void RemoveDestroyedTrees(int i, int j)
	{
		for (int x = -20; x < 21; x++)
		{
			for (int y = -40; y < 41; y++)
			{
				var tile = TileUtils.SafeGetTile(i + x, j + y);
				if (tile.HasTile && tile.TileType == TileID.Trees)
				{
					int maxTreeTile = 0;
					bool killTree = false;
					for (int addY = 0; addY < 80; addY++)
					{
						var treeTile = TileUtils.SafeGetTile(i + x, j + y + addY);
						if (treeTile.HasTile && treeTile.TileType == TileID.Trees)
						{
							maxTreeTile = addY;
						}
						else
						{
							if (treeTile.TileType != TileID.SnowBlock)
							{
								killTree = true;
							}
							break;
						}
					}
					if (killTree)
					{
						for (int addY = 0; addY <= maxTreeTile; addY++)
						{
							var treeTile = TileUtils.SafeGetTile(i + x, j + y + addY);
							treeTile.HasTile = false;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 平滑：参数分别是左上点世界坐标，宽，高，限定物块种类
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="tileType"></param>
	private static void SmoothTile_XXYY(int x = 0, int y = 0, int width = 0, int height = 0, int tileType = -1)
	{
		for (int i = 0; i < width; i += 1)
		{
			if (i + x > Main.maxTilesX - 20)
			{
				break;
			}
			if (i + x < 20)
			{
				break;
			}
			for (int j = 0; j < height; j += 1)
			{
				if (j + y > Main.maxTilesY - 20)
				{
					break;
				}
				if (j + y < 20)
				{
					break;
				}
				if (tileType == -1)
				{
					Tile.SmoothSlope(x + i, y + j, false);
					WorldGen.TileFrame(x + i, y + j, true, false);
					WorldGen.SquareWallFrame(x + i, y + j, true);
				}
				else
				{
					if (TileUtils.SafeGetTile(x + i, y + j).TileType == tileType)
					{
						Tile.SmoothSlope(x + i, y + j, false);
						WorldGen.TileFrame(x + i, y + j, true, false);
						WorldGen.SquareWallFrame(x + i, y + j, true);
					}
				}
			}
		}
	}
}
