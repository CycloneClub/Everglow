using Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;
using Everglow.Yggdrasil.KelpCurtain.Walls;
using static Everglow.Commons.Utilities.TileUtils;
using static Everglow.Yggdrasil.WorldGeneration.KelpCurtainGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Tools.Developer;

public class ResetIsleOfBloom : ModItem
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
		IsleOfBloom(Main.MouseWorld);
		return false;
	}

	public static void IsleOfBloom(Vector2 worldPos)
	{
		Point tilePos = worldPos.ToTileCoordinates();
		TileUtils.PlaceRectangleAreaOfBlock(tilePos.X - 150, tilePos.Y - 60, tilePos.X + 150, tilePos.Y + 150, -2);

		List<Point> area = new List<Point>();
		area.Add(tilePos + new Point(-130, 0));
		area.Add(tilePos + new Point(130, 0));
		area.Add(tilePos + new Point(150, 120));
		area.Add(tilePos + new Point(-150, 120));
		area = GetPolygonAreaOfTilePos(area);
		foreach (var pos in area)
		{
			var checkPoint = pos;
			var tile = SafeGetTile(checkPoint);
			float value0 = GetPerlinPixelG(pos.X, pos.Y) * 12;
			if (pos.Y - tilePos.Y < 12 + value0)
			{
				tile.TileType = (ushort)ModContent.TileType<Tiles.OldMoss>();
			}
			else
			{
				tile.TileType = (ushort)ModContent.TileType<Tiles.MossProneSandSoil>();
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
			if (pos.Y - tilePos.Y > value2 * 3 + value0 + 2)
			{
				if (pos.Y - tilePos.Y > value2 * 4 + value0 * 1.2f + 3)
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
			PlaceCircleAreaOfBlockWithRandomNoise(tilePos + new Point(0, y), radius, ModContent.TileType<Tiles.OldMoss>(), 3, (int)TileChangeState.HasTile);
			PlaceCircleAreaOfBlockWithRandomNoise(tilePos + new Point(0, y), radius - 3, -1, 3, (int)TileChangeState.Forceful);
			PlaceCircleAreaOfWallWithRandomNoise(tilePos + new Point(0, y), radius - 1, ModContent.WallType<OldMossWall>(), 3, (int)TileChangeState.HasWall);
		}

		// SubFloor Cave
		List<Vector2> Cave0_Bound = new List<Vector2>();
		List<Vector2> Cave0 = new List<Vector2>();
		int cave0Y = 75;
		int caveHeight = 128;
		for (int x = -130; x <= 130; x++)
		{
			float height = 130 - MathF.Abs(x);
			height = Math.Clamp(height, 0, caveHeight + 64);
			float value2 = GetPerlinPixelR(x * 2, cave0Y) * 64;
			height += value2;
			Cave0_Bound.Add(new Vector2(x * 16, cave0Y * 16 - height));
			height -= 64;
			height = Math.Clamp(height, 0, caveHeight);
			value2 = GetPerlinPixelB(x * 2, cave0Y + 40) * 64;
			height += value2;
			Cave0.Add(new Vector2(x * 16, cave0Y * 16 - height));
		}
		for (int x = 130; x >= -130; x--)
		{
			float height = 130 - MathF.Abs(x);
			height = Math.Clamp(height, 0, caveHeight + 64);
			float value2 = GetPerlinPixelB(x * 2, cave0Y + 30) * 64;
			height += value2;
			Cave0_Bound.Add(new Vector2(x * 16, cave0Y * 16 + height));
			height -= 64;
			height = Math.Clamp(height, 0, caveHeight);
			value2 = GetPerlinPixelB(x * 2, cave0Y + 70) * 64;
			height += value2;
			Cave0.Add(new Vector2(x * 16, cave0Y * 16 + height));
		}
		PlacePolygonAreaOfBlockWithOffset(Cave0_Bound, tilePos.ToWorldCoordinates(), ModContent.TileType<Tiles.OldMoss>(), (int)TileChangeState.HasTile);
		PlacePolygonAreaOfBlockWithOffset(Cave0, tilePos.ToWorldCoordinates(), -1, (int)TileChangeState.Forceful);
		SmoothTile_XXYY(tilePos.X - 150, tilePos.Y - 60, tilePos.X + 150, tilePos.Y + 150);

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
	}
}
