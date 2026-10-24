using Everglow.Yggdrasil.YggdrasilTown.Tiles;
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
}
