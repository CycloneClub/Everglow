using Everglow.Commons.FeatureFlags;
using Everglow.Yggdrasil.Common.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Liquids;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using Everglow.Yggdrasil.YggdrasilTown.Walls;
using ModLiquidLib.ModLoader;
using static Everglow.Commons.Utilities.TileUtils;

namespace Everglow.Yggdrasil.WorldGeneration;

public partial class YggdrasilTownGeneration
{
	private static Rectangle GetJellyBallHotbedBounds(int worldWidth, int worldHeight)
	{
		// 与 SafeGetTile 保持相同的 20 格边界，禁止写入原木生成未覆盖的预留区。
		return new Rectangle(worldWidth - 350, worldHeight - 510, 330, 486);
	}

	private static void ExtendJellyBallHotbed()
	{
		var bounds = GetJellyBallHotbedBounds(Main.maxTilesX, Main.maxTilesY);
		var observatory = new Rectangle(Main.maxTilesX - 104, Main.maxTilesY - 394, 76, 52);
		ushort jelly = (ushort)ModContent.TileType<JellyBallSecretion>();
		ushort wood = (ushort)ModContent.TileType<StoneScaleWood>();
		ushort soil = (ushort)ModContent.TileType<DarkForestSoil>();
		ushort grass = (ushort)ModContent.TileType<DarkForestGrass>();
		var source = new byte[bounds.Width, bounds.Height];
		for (int x = 0; x < bounds.Width; x++)
		{
			for (int y = 0; y < bounds.Height; y++)
			{
				Tile tile = Main.tile[bounds.X + x, bounds.Y + y];
				source[x, y] = tile.HasTile ? (tile.TileType == jelly ? JellyBallHotbedLayout.Jelly : JellyBallHotbedLayout.Terrain) : JellyBallHotbedLayout.Air;
				if (observatory.Contains(bounds.X + x, bounds.Y + y) || (tile.HasTile && tile.TileType != jelly && tile.TileType != wood && tile.TileType != soil && tile.TileType != grass))
				{
					source[x, y] = JellyBallHotbedLayout.Protected;
				}
			}
		}

		byte[,] layout = JellyBallHotbedLayout.Generate(source, GenRand.Next());
		int nutritionType = LiquidLoader.LiquidType<JellyBallNutritionLiquid>();
		for (int x = 0; x < bounds.Width; x++)
		{
			for (int y = 0; y < bounds.Height; y++)
			{
				byte material = layout[x, y];
				if (material == source[x, y] || material == JellyBallHotbedLayout.Protected)
				{
					continue;
				}
				Tile tile = Main.tile[bounds.X + x, bounds.Y + y];
				tile.ClearTile();
				tile.LiquidAmount = 0;
				tile.TileColor = PaintID.None;
				if (source[x, y] is JellyBallHotbedLayout.Terrain or JellyBallHotbedLayout.Air)
				{
					tile.WallType = (ushort)ModContent.WallType<JellyBallSecretionWall>();
				}
				if (material is JellyBallHotbedLayout.Air or JellyBallHotbedLayout.CaveAir or JellyBallHotbedLayout.Nutrition)
				{
					tile.WallType = (ushort)ModContent.WallType<JellyBallSecretionWall>();
					if (material == JellyBallHotbedLayout.Nutrition)
					{
						tile.LiquidType = nutritionType;
						tile.LiquidAmount = byte.MaxValue;
					}
					continue;
				}
				tile.HasTile = true;
				tile.TileType = material switch
				{
					JellyBallHotbedLayout.Jelly => jelly,
					JellyBallHotbedLayout.Crystal => TileID.DiamondGemspark,
					JellyBallHotbedLayout.Fresh => (ushort)ModContent.TileType<JellyBallSecretion_Fresh>(),
					JellyBallHotbedLayout.Membrane => (ushort)ModContent.TileType<JellyBallStemCells>(),
					_ => jelly,
				};
				tile.TileFrameX = 0;
				tile.TileFrameY = 0;
			}
		}

		// 固定曲线作为独立总 pass，只替换已有球冻物块，空洞不放置神经或背景墙。
		byte[,] nerves = JellyBallHotbedLayout.TraceReferenceNerves(layout);
		for (int x = 4; x < bounds.Width - 4; x++)
		{
			for (int y = 4; y < bounds.Height - 4; y++)
			{
				if (nerves[x, y] == 0)
				{
					continue;
				}
				Tile tile = Main.tile[bounds.X + x, bounds.Y + y];
				if (tile.HasTile && tile.TileType == jelly)
				{
					if (nerves[x, y] == 1)
					{
						tile.TileType = (ushort)ModContent.TileType<JellyBallNerve>();
					}
					else
					{
						tile.TileType = (ushort)ModContent.TileType<JellyBallNerve_Thin>();
					}
				}
			}
		}

		BuildJellyBallNerveCables(bounds);

		// 子世界没有原版的最终 framing pass，待全部材料写完再更新接缝。
		for (int x = 1; x < bounds.Width - 1; x++)
		{
			for (int y = 1; y < bounds.Height - 1; y++)
			{
				if (layout[x, y] != source[x, y] || nerves[x, y] != 0)
				{
					WorldGen.SquareTileFrame(bounds.X + x, bounds.Y + y);
					WorldGen.SquareWallFrame(bounds.X + x, bounds.Y + y);
				}
			}
		}
	}

	private readonly record struct JellyBallNerveCableAnchor(Point Nerve, Point Cable);

	private static void BuildJellyBallNerveCables(Rectangle bounds)
	{
		int nutritionType = LiquidLoader.LiquidType<JellyBallNutritionLiquid>();
		int nerveType = ModContent.TileType<JellyBallNerve>();
		int poolLeft = bounds.Right;
		int poolTop = bounds.Bottom;
		int poolRight = bounds.Left;
		int poolBottom = bounds.Top;
		for (int x = bounds.Left; x < bounds.Right; x++)
		{
			for (int y = bounds.Top; y < bounds.Bottom; y++)
			{
				Tile tile = Main.tile[x, y];
				if (tile.LiquidAmount == 0 || tile.LiquidType != nutritionType)
				{
					continue;
				}

				poolLeft = Math.Min(poolLeft, x);
				poolTop = Math.Min(poolTop, y);
				poolRight = Math.Max(poolRight, x + 1);
				poolBottom = Math.Max(poolBottom, y + 1);
			}
		}

		if (poolLeft >= poolRight || poolTop >= poolBottom)
		{
			return;
		}

		var poolBounds = Rectangle.Intersect(bounds, new Rectangle(poolLeft - 24, poolTop - 16, poolRight - poolLeft + 48, poolBottom - poolTop + 32));
		var anchors = new List<JellyBallNerveCableAnchor>();
		var seenCablePoints = new HashSet<Point>();
		Point[] directions = [new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1)];
		for (int x = poolBounds.Left; x < poolBounds.Right; x++)
		{
			for (int y = poolBounds.Top; y < poolBounds.Bottom; y++)
			{
				Tile tile = Main.tile[x, y];
				if (!tile.HasUnactuatedTile || tile.TileType != nerveType)
				{
					continue;
				}

				foreach (Point direction in directions)
				{
					var cablePoint = new Point(x + direction.X, y + direction.Y);
					if (poolBounds.Contains(cablePoint) && !Main.tile[cablePoint].HasUnactuatedTile && seenCablePoints.Add(cablePoint))
					{
						anchors.Add(new JellyBallNerveCableAnchor(new Point(x, y), cablePoint));
					}
				}
			}
		}

		var pairs = new List<(JellyBallNerveCableAnchor Start, JellyBallNerveCableAnchor End)>();
		for (int i = 0; i < anchors.Count; i++)
		{
			for (int j = i + 1; j < anchors.Count; j++)
			{
				var start = anchors[i];
				var end = anchors[j];
				int deltaX = Math.Abs(start.Cable.X - end.Cable.X);
				int deltaY = Math.Abs(start.Cable.Y - end.Cable.Y);
				if (start.Nerve == end.Nerve || deltaX < 18 || deltaX > 50 || deltaY > 24
					|| !IsClearCablePath(start.Cable, end.Cable, poolBounds))
				{
					continue;
				}

				pairs.Add((start, end));
			}
		}

		var cable = ModContent.GetInstance<JellyBallNerveCable>();
		var usedAnchors = new List<Point>();
		int placed = 0;
		foreach (var pair in pairs.OrderBy(_ => GenRand.Next()))
		{
			if (usedAnchors.Any(point => Vector2.DistanceSquared(point.ToVector2(), pair.Start.Cable.ToVector2()) < 144
				|| Vector2.DistanceSquared(point.ToVector2(), pair.End.Cable.ToVector2()) < 144))
			{
				continue;
			}

			cable.CreateACableTile(pair.Start.Cable.X, pair.Start.Cable.Y, pair.End.Cable.X, pair.End.Cable.Y);
			WorldGen.SquareTileFrame(pair.Start.Cable.X, pair.Start.Cable.Y);
			WorldGen.SquareTileFrame(pair.End.Cable.X, pair.End.Cable.Y);
			usedAnchors.Add(pair.Start.Cable);
			usedAnchors.Add(pair.End.Cable);
			if (++placed >= 5)
			{
				break;
			}
		}
	}

	private static bool IsClearCablePath(Point start, Point end, Rectangle bounds)
	{
		int steps = Math.Max(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) * 2;
		for (int step = 1; step < steps; step++)
		{
			float progress = step / (float)steps;
			int x = (int)MathF.Round(start.X + (end.X - start.X) * progress);
			int y = (int)MathF.Round(start.Y + (end.Y - start.Y) * progress);
			if (!bounds.Contains(x, y) || Main.tile[x, y].HasUnactuatedTile)
			{
				return false;
			}
		}

		return true;
	}
}
