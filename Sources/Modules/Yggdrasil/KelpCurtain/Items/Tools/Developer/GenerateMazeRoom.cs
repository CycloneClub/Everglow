using Everglow.Commons.Physics.DataStructures;
using Everglow.Yggdrasil.Common.Tiles;
using Everglow.Yggdrasil.Common.Walls;
using Everglow.Yggdrasil.KelpCurtain.Tiles;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.WaterDeliveryHoles;
using Everglow.Yggdrasil.KelpCurtain.Walls;
using Everglow.Yggdrasil.WorldGeneration;
using Spine;
using static Everglow.Commons.Utilities.TileUtils;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Tools.Developer;

public class GenerateMazeRoom : ModItem
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

	public List<Point> SeedMap = new List<Point>();

	public override void HoldItem(Player player) => base.HoldItem(player);

	public override bool CanUseItem(Player player)
	{
		BuildMazeRoom(Main.MouseWorld);
		//ClearMazeRooms(Main.MouseWorld);
		//BuildEntireMaze(Main.MouseWorld);

		// ConnectDeliveryHole(player.MountedCenter, Main.MouseWorld);
		//List<Vector2> polygon = new List<Vector2>();
		//for(int h = 0;h < 5;h++)
		//{
		//	polygon.Add(Main.MouseWorld + new Vector2(0, 720).RotatedBy(h / 5f * MathHelper.TwoPi));
		//}
		//PlacePolygonBoundOfBlock(polygon, TileID.Dirt, 16);
		return false;
	}

	public void BuildMazeRoom(Vector2 worldPos)
	{
		Point tilePos = worldPos.ToTileCoordinates();
		Point buildPos = tilePos;
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
		int randomAddPosX = GenRand.Next(0, 4);
		buildPos.X += randomAddPosX * dir;
		if (dir == 1)
		{
			if (GetUniformTile(buildPos.X, buildPos.Y - 9, 10, 10) == -1)
			{
				YggdrasilWorldGeneration.QuickBuild(buildPos.X, buildPos.Y - 9, ModAsset.MazeUnderLake_ChestRoom_withLamps10x10_Path, false);
				PlaceLineBlock(buildPos, buildPos + new Point(-randomAddPosX, 0), 1, ModContent.TileType<YggdrasilBlackRock>());
			}
		}
		else
		{
			if (GetUniformTile(buildPos.X - 10, buildPos.Y - 9, 10, 10) == -1)
			{
				YggdrasilWorldGeneration.QuickBuild(buildPos.X - 10, buildPos.Y - 9, ModAsset.MazeUnderLake_ChestRoom_withLamps10x10_Path, false);
				PlaceLineBlock(buildPos, buildPos + new Point(randomAddPosX, 0), 1, ModContent.TileType<YggdrasilBlackRock>());
			}
		}
	}

	public void BuildEntireMaze(Vector2 worldPos)
	{
		Point tilePos = worldPos.ToTileCoordinates();
		int xBoundLeft = tilePos.X - 200;
		int xBoundRight = tilePos.X + 200;
		int yBoundTop = tilePos.Y - 100;
		int yBoundBottom = tilePos.Y + 100;

		// Random seed Points
		List<Point> seeds = GenerateRandomSeeds(xBoundLeft - 30, xBoundRight + 30, yBoundTop - 30, yBoundBottom + 30, 180, 25);

		for (int x = xBoundLeft; x < xBoundRight; x++)
		{
			for (int y = yBoundTop; y < yBoundBottom; y++)
			{
				// Exist a projection. SeedMap is not TileMap.
				var tile = SafeGetTile(x, y);
				tile.ClearEverything();
				if (KelpCurtainGeneration.MazeUnderLake_IsEdgePoint(x, y, seeds))
				{
					if (!tile.HasTile)
					{
						tile.TileType = (ushort)ModContent.TileType<YggdrasilBlackRock>();
						tile.WallType = (ushort)ModContent.WallType<YggdrasilBlackRockWall>();
						tile.HasTile = true;
					}
				}
			}
		}
		Dictionary<Point, List<Point>> holes = new Dictionary<Point, List<Point>>();
		Point mediumSeedPos = default;
		float minDisToCenter = new Vector2(xBoundRight - xBoundLeft, yBoundBottom - yBoundTop).Length();
		foreach (var pos in seeds)
		{
			float boundRange = 8;
			Vector2 center = new Vector2(xBoundLeft + xBoundRight, yBoundTop + yBoundBottom) * 0.5f;
			float toCenterDis = new Vector2(pos.X - center.X, pos.Y - center.Y).Length();
			if (toCenterDis < minDisToCenter)
			{
				minDisToCenter = toCenterDis;
				mediumSeedPos = pos;
			}
			if (pos.X >= xBoundLeft + boundRange && pos.X <= xBoundRight - boundRange && pos.Y <= yBoundBottom - boundRange && pos.Y >= yBoundTop + boundRange)
			{
				KelpCurtainGeneration.MazeUnderLake_AddNewConnection(pos, holes, seeds);
			}
		}
		if (mediumSeedPos != default)
		{
			int maxStep = 6;
			for (int t = 0; t < maxStep; t++)
			{
				List<Point> connectedWithMediumSeeds = KelpCurtainGeneration.MazeUnderLake_GetAllConnectedPoints(holes, seeds, new List<Point>(), mediumSeedPos, 0);
				foreach (var pos in connectedWithMediumSeeds)
				{
					KelpCurtainGeneration.MazeUnderLake_AddNewConnectionWithMediumNet(pos, holes, seeds, connectedWithMediumSeeds);
				}
			}
		}
	}
}
