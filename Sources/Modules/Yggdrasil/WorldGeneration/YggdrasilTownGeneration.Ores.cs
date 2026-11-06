using Everglow.Yggdrasil.Common.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.CyanVine;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Walls;
using Terraria.Utilities;
using static Everglow.Commons.Utilities.TileUtils;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.WorldGeneration;

public partial class YggdrasilTownGeneration
{
	private const int AmberVeinCount = 60;
	private const float AmberMinVeinSpacing = 40f;
	private const int AmberBuriedCoreRadius = 6;
	private const int AmberMinRetainedTilePercent = 85;
	private const int AmberRoofClearance = 8;

	/// <summary>Scatter buried deposits inside the lower-left tunneled mine.</summary>
	private static void GenerateAmberVeins()
	{
		int hostTileType = ModContent.TileType<StoneScaleWood>();
		ushort amberTileType = (ushort)ModContent.TileType<YggdrasilTown.Tiles.YggdrasilAmber>();
		int naturalWallType = ModContent.WallType<StoneDragonScaleWoodWall>();
		var townBounds = new Rectangle(218, Main.maxTilesY - 612, 730, 299);
		Rectangle generationBounds = GetMineBounds(Main.maxTilesX, Main.maxTilesY);
		int placedVeinCount = ScatterAmberVeins(generationBounds, townBounds, AmberVeinCount, AmberMinVeinSpacing, WorldGen.genRand, CanPlaceAmber, TryPlaceVein);
		if (placedVeinCount == 0)
		{
			Ins.Logger.Warn("No amber deposits could be placed in the lower-left town mine.");
		}

		bool CanPlaceAmber(Point point)
		{
			if (!IsInAmberGenerationArea(point, Main.maxTilesX, Main.maxTilesY) || !WorldGen.InWorld(point.X, point.Y, 1) || townBounds.Contains(point)
				|| !CanLegallySmooth(point.X, point.Y) || !ChestSafe(point.X, point.Y))
			{
				return false;
			}
			Tile tile = Main.tile[point.X, point.Y];
			return tile.HasTile && tile.TileType == hostTileType && (tile.WallType == WallID.None || tile.WallType == naturalWallType);
		}

		bool TryPlaceVein(Point center)
		{
			if (!CanPlaceAmber(center))
			{
				return false;
			}
			var veinTiles = CollectAmberVeinTiles(center, CreateAmberVeinShape(WorldGen.genRand), CanPlaceAmber);
			foreach (Point point in veinTiles)
			{
				Tile tile = Main.tile[point.X, point.Y];
				tile.TileType = amberTileType;
				tile.TileFrameX = 0;
				tile.TileFrameY = 0;
			}
			return veinTiles.Count > 0;
		}
	}

	private static bool IsInAmberGenerationArea(Point point, int worldWidth, int worldHeight)
	{
		int localX = point.X - MineRoofStartX;
		if (!GetMineBounds(worldWidth, worldHeight).Contains(point) || localX < 0 || localX >= MineRoofWidth)
		{
			return false;
		}
		// The terrain's roof noise adds at most 7.5 tiles. Stay below that entire band.
		float roofY = worldHeight - MineRoofBottomOffset + GetMineRoofOffset(localX) + AmberRoofClearance;
		return point.Y >= roofY;
	}

	private static int ScatterAmberVeins(Rectangle generationBounds, Rectangle townBounds, int targetVeinCount, float minVeinSpacing, UnifiedRandom random, Func<Point, bool> canPlacePatchCenter, Func<Point, bool> tryPlaceVein)
	{
		if (generationBounds.Width <= 0 || generationBounds.Height <= 0 || targetVeinCount <= 0)
		{
			return 0;
		}
		// Unequal, overlapping density patches leave both rich pockets and sparse stretches.
		var patches = new List<(Vector2 Center, Vector2 Spread)>();
		int patchCount = random.Next(3, 6);
		for (int i = 0; i < patchCount; i++)
		{
			for (int retry = 0; retry < 256; retry++)
			{
				var patchCenter = new Point(random.Next(generationBounds.Left, generationBounds.Right), random.Next(generationBounds.Top, generationBounds.Bottom));
				if (townBounds.Contains(patchCenter) || !canPlacePatchCenter(patchCenter))
				{
					continue;
				}
				var spread = new Vector2(generationBounds.Width * random.NextFloat(0.04f, 0.09f), generationBounds.Height * random.NextFloat(0.04f, 0.09f));
				patches.Add((patchCenter.ToVector2(), spread));
				break;
			}
		}
		if (patches.Count == 0)
		{
			return 0;
		}

		var placedCenters = new List<Vector2>();
		for (int attempt = 0; attempt < targetVeinCount * 100 && placedCenters.Count < targetVeinCount; attempt++)
		{
			Point veinCenter;
			if (random.NextBool(10))
			{
				veinCenter = new Point(random.Next(generationBounds.Left, generationBounds.Right), random.Next(generationBounds.Top, generationBounds.Bottom));
			}
			else
			{
				var patch = patches[random.Next(patches.Count)];
				float distance = MathF.Sqrt(-2 * MathF.Log(Math.Max(0.0001f, random.NextFloat())));
				Vector2 offset = random.NextFloat(MathHelper.TwoPi).ToRotationVector2() * distance;
				Vector2 position = patch.Center + offset * patch.Spread;
				veinCenter = new Point((int)MathF.Round(position.X), (int)MathF.Round(position.Y));
			}
			if (!generationBounds.Contains(veinCenter) || townBounds.Contains(veinCenter)
				|| placedCenters.Any(other => Vector2.DistanceSquared(veinCenter.ToVector2(), other) < minVeinSpacing * minVeinSpacing))
			{
				continue;
			}
			if (tryPlaceVein(veinCenter))
			{
				placedCenters.Add(veinCenter.ToVector2());
			}
		}
		return placedCenters.Count;
	}

	private static List<Point> CollectAmberVeinTiles(Point center, HashSet<Point> tileOffsets, Func<Point, bool> canPlaceTile)
	{
		// Require a solid core so thin walls and exposed edges do not become an ore trail.
		for (int dx = -AmberBuriedCoreRadius; dx <= AmberBuriedCoreRadius; dx++)
		{
			for (int dy = -AmberBuriedCoreRadius; dy <= AmberBuriedCoreRadius; dy++)
			{
				if (!canPlaceTile(center + new Point(dx, dy)))
				{
					return new List<Point>();
				}
			}
		}
		var validTiles = tileOffsets.Select(offset => center + offset).Where(canPlaceTile).ToList();
		return validTiles.Count * 100 >= tileOffsets.Count * AmberMinRetainedTilePercent ? validTiles : new List<Point>();
	}

	private static HashSet<Point> CreateAmberVeinShape(UnifiedRandom random)
	{
		// All coordinates are relative to the fixed group center C = (0, 0).
		var circleCenters = new Vector2[random.Next(7, 15)];
		float aspectRatio = random.NextFloat(0.55f, 1.85f);
		float initialSpread = random.NextFloat(1.5f, 3f);
		float rotation = random.NextFloat(MathHelper.TwoPi);
		for (int i = 0; i < circleCenters.Length; i++)
		{
			Vector2 point = random.NextFloat(MathHelper.TwoPi).ToRotationVector2() * MathF.Sqrt(random.NextFloat()) * initialSpread;
			point.X *= MathF.Sqrt(aspectRatio);
			point.Y /= MathF.Sqrt(aspectRatio);
			circleCenters[i] = point.RotatedBy(rotation);
		}
		RelaxAmberPoints(circleCenters, random.NextFloat(4.6f, 6.4f), random.Next(14, 29));

		float baseRadius = random.NextFloat(5.4f, 7.2f);
		float radiusDecay = random.NextFloat(0.16f, 0.26f);
		var circleRadii = new float[circleCenters.Length];
		for (int i = 0; i < circleCenters.Length; i++)
		{
			circleRadii[i] = Math.Max(1.1f, baseRadius * MathF.Exp(-circleCenters[i].Length() * radiusDecay) + random.NextFloat(-0.25f, 0.25f));
		}
		// Rasterized linear size averages about 8.24 tiles, with variance about 11.41.
		float scale = random.NextFloat(0.19f, 0.815f);
		for (int i = 0; i < circleCenters.Length; i++)
		{
			circleCenters[i] *= scale;
			circleRadii[i] *= scale;
		}
		var tileOffsets = RasterizeAmberCircles(circleCenters, circleRadii, 1.25f * scale);
		if (tileOffsets.Count == 0)
		{
			tileOffsets.Add(Point.Zero);
		}
		return tileOffsets;
	}

	private static void RelaxAmberPoints(Vector2[] circleCenters, float repulsionDistance, int iterationCount)
	{
		var displacements = new Vector2[circleCenters.Length];
		for (int iteration = 0; iteration < iterationCount; iteration++)
		{
			for (int i = 0; i < circleCenters.Length; i++)
			{
				displacements[i] = -circleCenters[i] * 0.025f;
			}
			for (int i = 0; i < circleCenters.Length; i++)
			{
				for (int j = i + 1; j < circleCenters.Length; j++)
				{
					Vector2 separation = circleCenters[j] - circleCenters[i];
					float distance = separation.Length();
					if (distance >= repulsionDistance)
					{
						continue;
					}
					Vector2 direction = distance > 0.0001f ? separation / distance : Vector2.UnitX;
					Vector2 push = direction * (repulsionDistance - distance) * 0.3f;
					displacements[i] -= push;
					displacements[j] += push;
				}
			}

			// Apply the accumulated forces together, with a step cap for dense initial clouds.
			float maxDisplacement = repulsionDistance * 0.35f;
			for (int i = 0; i < circleCenters.Length; i++)
			{
				float displacementLength = displacements[i].Length();
				circleCenters[i] += displacementLength > maxDisplacement ? displacements[i] * (maxDisplacement / displacementLength) : displacements[i];
			}
		}
	}

	private static HashSet<Point> RasterizeAmberCircles(Vector2[] circleCenters, float[] circleRadii, float rockGap)
	{
		var tileOffsets = new HashSet<Point>();
		for (int i = 0; i < circleCenters.Length; i++)
		{
			var clipPlanes = new List<(Vector2 Normal, float Distance)>();
			for (int j = 0; j < circleCenters.Length; j++)
			{
				if (i == j)
				{
					continue;
				}
				Vector2 separation = circleCenters[j] - circleCenters[i];
				float distance = separation.Length();
				if (distance >= circleRadii[i] + circleRadii[j])
				{
					continue;
				}
				Vector2 normal = distance > 0.0001f ? separation / distance : (i < j ? Vector2.UnitX : -Vector2.UnitX);
				// Offset both sides of the Voronoi bisector by half a rock gap.
				clipPlanes.Add((normal, (distance - rockGap) * 0.5f));
			}
			int left = (int)MathF.Floor(circleCenters[i].X - circleRadii[i]);
			int right = (int)MathF.Ceiling(circleCenters[i].X + circleRadii[i]);
			int top = (int)MathF.Floor(circleCenters[i].Y - circleRadii[i]);
			int bottom = (int)MathF.Ceiling(circleCenters[i].Y + circleRadii[i]);
			for (int x = left; x <= right; x++)
			{
				for (int y = top; y <= bottom; y++)
				{
					Vector2 offset = new Vector2(x, y) - circleCenters[i];
					if (offset.LengthSquared() > circleRadii[i] * circleRadii[i])
					{
						continue;
					}
					bool isClipped = false;
					foreach (var clipPlane in clipPlanes)
					{
						if (Vector2.Dot(offset, clipPlane.Normal) > clipPlane.Distance)
						{
							isClipped = true;
							break;
						}
					}
					if (!isClipped)
					{
						tileOffsets.Add(new Point(x, y));
					}
				}
			}
		}
		return tileOffsets;
	}

	/// <summary>
	/// 青缎矿化
	/// </summary>
	/// <param name="leftX"></param>
	/// <param name="upY"></param>
	/// <param name="rightX"></param>
	/// <param name="downY"></param>
	public static void GenerateCyanVineOre(int leftX, int upY, int rightX, int downY)
	{
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
						if (GenRand.NextBool(12))
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
						if (GenRand.NextBool(12))
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

	/// <summary>Mineralize the union of a pillar's circles once, regardless of its radius or path overlap.</summary>
	private static void GeneratePillarCyanVineOre(IReadOnlyList<Vector2> centers, float range)
	{
		var positions = new List<Point>();
		var visited = new HashSet<Point>();
		foreach (Vector2 center in centers)
		{
			for (int x = (int)(center.X - range); x < (int)(center.X + range); x++)
			{
				for (int y = (int)(center.Y - range); y < (int)(center.Y + range); y++)
				{
					var position = new Point(x, y);
					if (Vector2.DistanceSquared(center, new Vector2(x, y)) <= range * range && visited.Add(position))
					{
						positions.Add(position);
					}
				}
			}
		}

		foreach (Point position in positions)
		{
			int x = position.X;
			int y = position.Y;
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
					if (GenRand.NextBool(12))
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
					if (GenRand.NextBool(12))
					{
						if (CanPlaceMultiBenethTowardsDownRight(x - 4, y - 1, 5, 3))
						{
							PlaceLargeUpCyanVineOre(x - 4, y);
						}
					}
				}
			}
		}
		foreach (Point position in positions)
		{
			int x = position.X;
			int y = position.Y;
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

	/// <summary>Scatter small ore deposits during the first-stratum mineralization stage.</summary>
	private static void GenerateTownManaSpringDeposits()
	{
		const int TargetOreCount = 480;
		var random = WorldGen.genRand;
		Point[] neighbors = [new(-1, 0), new(1, 0), new(0, -1), new(0, 1)];
		Main.statusText = "Scattering Mana Spring Ore...";
		var bounds = new Rectangle(20, (int)(Main.maxTilesY * 0.9f), Main.maxTilesX - 40, Main.maxTilesY - (int)(Main.maxTilesY * 0.9f) - 20);
		var protectedAreas = new List<Rectangle>();
		foreach (Rectangle area in ShouldNotSmoothAreas)
		{
			var padded = area;
			padded.Inflate(2, 2);
			protectedAreas.Add(padded);
		}
		var town = new Rectangle(YggdrasilTownTopLeft.X, YggdrasilTownTopLeft.Y, 706, 275);
		town.Inflate(12, 12);
		protectedAreas.Add(town);
		HashSet<ushort> hosts =
		[
			(ushort)ModContent.TileType<StoneScaleWood>(),
			(ushort)ModContent.TileType<DarkForestSoil>(),
			(ushort)ModContent.TileType<DarkForestGrass>(),
			(ushort)ModContent.TileType<TwilightGrassBlock>(),
			(ushort)ModContent.TileType<YggdrasilGrayRock>(),
		];

		// These natural walls also count as housing walls; the flag alone cannot identify buildings.
		HashSet<ushort> naturalWalls =
		[
			(ushort)ModContent.WallType<StoneDragonScaleWoodWall>(),
			(ushort)ModContent.WallType<DarkForestSoilWall>(),
			(ushort)ModContent.WallType<FemaleLampWoodWall>(),
		];

		bool CanReplace(Point point)
		{
			Tile tile = Main.tile[point.X, point.Y];
			Tile above = Main.tile[point.X, point.Y - 1];
			return tile.HasTile && !tile.IsActuated && hosts.Contains(tile.TileType)
				&& (!Main.wallHouse[tile.WallType] || naturalWalls.Contains(tile.WallType))
				&& !(above.HasTile && TileID.Sets.BasicChest[above.TileType]);
		}

		bool IsExposed(Point point)
		{
			foreach (Point offset in neighbors)
			{
				if (!Main.tile[point.X + offset.X, point.Y + offset.Y].HasTile)
				{
					return true;
				}
			}
			return false;
		}

		var deposits = CreateDeposits();
		ushort oreType = (ushort)ModContent.TileType<YggdrasilTown.Tiles.ManaSpringOre_Tile>();
		foreach (Point point in deposits.SelectMany(cluster => cluster))
		{
			Tile tile = Main.tile[point.X, point.Y];
			tile.TileType = oreType;
			tile.TileFrameX = 0;
			tile.TileFrameY = 0;
			tile.Slope = SlopeType.Solid;
			tile.IsHalfBlock = false;
		}
		foreach (Point point in deposits.SelectMany(cluster => cluster))
		{
			WorldGen.SquareTileFrame(point.X, point.Y);
		}
		int placed = deposits.Sum(cluster => cluster.Length);
		Ins.Logger.Info($"Generated {placed} mana spring ore tiles in {deposits.Count} deposits.");
		if (placed < TargetOreCount)
		{
			Ins.Logger.Warn($"Mana spring ore generation found insufficient suitable terrain ({placed}/{TargetOreCount} tiles).");
		}

		/// <summary>
		/// Plan connected deposits without modifying terrain. Shuffled sectors spread the budget;
		/// bounded retries redistribute it when sectors contain only air or protected structures.
		/// </summary>
		List<Point[]> CreateDeposits()
		{
			const int SectorCount = 8;
			const int MinimumClusterSize = 3;
			const int MaximumClusterSize = 6;
			const int MinimumCenterDistance = 32;
			var deposits = new List<Point[]>();
			if (bounds.Width < SectorCount || bounds.Height < SectorCount)
			{
				return deposits;
			}

			var sectors = Enumerable.Range(0, SectorCount * SectorCount).ToArray();
			for (int i = sectors.Length - 1; i > 0; i--)
			{
				int other = random.Next(i + 1);
				(sectors[i], sectors[other]) = (sectors[other], sectors[i]);
			}

			bool IsEligible(Point point) => bounds.Contains(point) && CanReplace(point)
				&& !protectedAreas.Any(area => area.Contains(point));

			int remaining = TargetOreCount;
			for (int round = 0; round < 12 && remaining >= MinimumClusterSize; round++)
			{
				foreach (int sector in sectors)
				{
					if (remaining < MinimumClusterSize)
					{
						break;
					}
					int column = sector % SectorCount;
					int row = sector / SectorCount;
					int left = bounds.Left + bounds.Width * column / SectorCount;
					int right = bounds.Left + bounds.Width * (column + 1) / SectorCount;
					int top = bounds.Top + bounds.Height * row / SectorCount;
					int bottom = bounds.Top + bounds.Height * (row + 1) / SectorCount;
					int size = remaining <= MaximumClusterSize ? remaining : random.Next(MinimumClusterSize, MaximumClusterSize + 1);
					// Do not leave an unusable final budget of one or two tiles.
					if (remaining > size && remaining - size < MinimumClusterSize)
					{
						size = remaining - MinimumClusterSize;
					}

					for (int attempt = 0; attempt < 256; attempt++)
					{
						var center = new Point(random.Next(left, right), random.Next(top, bottom));
						if (!IsEligible(center))
						{
							continue;
						}
						// Prefer an exposed seed for half the deposits, but still allow buried ore.
						if (deposits.Count % 2 == 0 && attempt < 128 && !IsExposed(center))
						{
							continue;
						}
						if (deposits.Any(cluster => DistanceSquared(center, cluster[0]) < MinimumCenterDistance * MinimumCenterDistance))
						{
							continue;
						}
						var cluster = GrowCluster(center, size, IsEligible);
						if (cluster.Length != size)
						{
							continue;
						}
						deposits.Add(cluster);
						remaining -= cluster.Length;
						break;
					}
				}
			}
			return deposits;
		}

		Point[] GrowCluster(Point center, int size, Func<Point, bool> isEligible)
		{
			const int ClusterRadius = 2;
			var cluster = new List<Point> { center };
			var frontier = new List<Point>();
			var visited = new HashSet<Point> { center };
			Point last = center;
			while (cluster.Count < size)
			{
				foreach (Point offset in neighbors)
				{
					Point next = last + offset;
					if (Math.Abs(next.X - center.X) <= ClusterRadius && Math.Abs(next.Y - center.Y) <= ClusterRadius
						&& visited.Add(next) && isEligible(next))
					{
						frontier.Add(next);
					}
				}
				if (frontier.Count == 0)
				{
					break;
				}
				int index = random.Next(frontier.Count);
				last = frontier[index];
				frontier.RemoveAt(index);
				cluster.Add(last);
			}
			return cluster.ToArray();
		}

		static int DistanceSquared(Point first, Point second)
		{
			int dx = first.X - second.X;
			int dy = first.Y - second.Y;
			return dx * dx + dy * dy;
		}
	}
}
