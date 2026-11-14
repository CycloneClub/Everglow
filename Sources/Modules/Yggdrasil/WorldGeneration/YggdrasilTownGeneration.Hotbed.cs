using Everglow.Commons.FeatureFlags;
using Everglow.Yggdrasil.Common.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using Everglow.Yggdrasil.YggdrasilTown.Walls;
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
		// 占位材料的试验地形仅在调试模式下生成，沿用现有配置而不增加临时内容类型。
		if (!EverglowConfig.DebugMode)
		{
			return;
		}
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
				if (material is JellyBallHotbedLayout.Air or JellyBallHotbedLayout.CaveAir or JellyBallHotbedLayout.Honey)
				{
					tile.WallType = (ushort)ModContent.WallType<JellyBallSecretionWall>();
					if (material == JellyBallHotbedLayout.Honey)
					{
						tile.LiquidType = LiquidID.Honey;
						tile.LiquidAmount = byte.MaxValue;
					}
					continue;
				}
				tile.HasTile = true;
				tile.TileType = material switch
				{
					JellyBallHotbedLayout.Jelly => jelly,
					JellyBallHotbedLayout.Crystal => TileID.DiamondGemspark,
					_ => TileID.Glass,
				};
				tile.TileColor = material switch
				{
					JellyBallHotbedLayout.Membrane => PaintID.PinkPaint,
					JellyBallHotbedLayout.Fresh => PaintID.DeepSkyBluePaint,
					_ => PaintID.None,
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
				if (tile.HasTile && layout[x, y] is JellyBallHotbedLayout.Jelly or JellyBallHotbedLayout.Fresh)
				{
					tile.TileType = TileID.Glass;
					tile.TileColor = nerves[x, y] == 1 ? PaintID.WhitePaint : PaintID.BlackPaint;
				}
			}
		}

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
}
