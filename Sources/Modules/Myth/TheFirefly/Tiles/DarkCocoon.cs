using Everglow.Commons.TileHelper;

namespace Everglow.Myth.TheFirefly.Tiles;

public class DarkCocoon : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkCocoonMoss>()] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkCocoonSpecial>()] = true;
		MinPick = 175;
		DustType = DustID.SpookyWood;
		AddMapEntry(new Color(17, 16, 17));
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		var thisTile = Main.tile[i, j];
		bool slope = thisTile.Slope != SlopeType.Solid;
		if (slope)
		{
			return;
		}

		// 种树
		if (Main.rand.NextBool(6))
		{
			if (Main.tile[i + 1, j].Slope == SlopeType.Solid && Main.tile[i - 1, j].Slope == SlopeType.Solid && Main.tile[i + 2, j].Slope == SlopeType.Solid && Main.tile[i - 2, j].Slope == SlopeType.Solid &&
				Main.tile[i, j + 1].Slope == SlopeType.Solid && Main.tile[i + 1, j + 1].Slope == SlopeType.Solid && Main.tile[i - 1, j + 1].Slope == SlopeType.Solid && Main.tile[i + 2, j + 1].Slope == SlopeType.Solid && Main.tile[i - 2, j + 1].Slope == SlopeType.Solid)// 树木
			{
				int MaxHeight = 0;
				for (int x = -2; x < 3; x++)
				{
					for (int y = -1; y > -30; y--)
					{
						if (j + y > 20)
						{
							if (Main.tile[i + x, j + y].HasTile || Main.tile[i + x, j + y].LiquidAmount > 3)
							{
								return;
							}
						}
						MaxHeight = -y;
					}
				}
				if (MaxHeight > 7)
				{
					BuildFluorescentTree(i, j - 1, MaxHeight);
				}
			}
		}

		// 种植灯莲
		if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j - 1].LiquidAmount > 0)
		{
			Tile tile = Main.tile[i, j - 1];
			tile.TileType = (ushort)ModContent.TileType<LampLotus>();
			tile.HasTile = true;
			tile.TileFrameX = (short)(28 * Main.rand.Next(8));
		}

		// 黑萤藤蔓
		if (Main.rand.NextBool(6))
		{
			Tile t2 = Main.tile[i, j + 1];
			if (!t2.HasTile)
			{
				t2.TileType = (ushort)ModContent.TileType<BlackVine>();
				t2.HasTile = true;
			}
		}
		bool canPlaceShrub = true;
		for (int x = -1; x < 2; x++)
		{
			for (int y = -3; y < 0; y++)
			{
				if (Main.tile[i + x, j + y].HasTile)
				{
					canPlaceShrub = false;
				}
			}
		}
		bool nearWater = false;
		for (int x = -8; x < 9; x++)
		{
			for (int y = -3; y < 4; y++)
			{
				if (Main.tile[i + x, j + y].LiquidAmount > 3)
				{
					nearWater = true;
				}
			}
		}
		bool canPlace1x1 = true;
		for (int x = 0; x < 1; x++)
		{
			for (int y = -1; y < 1; y++)
			{
				Tile checkTile = Main.tile[i + x, j + y];
				if (checkTile.HasTile && y <= -1)
				{
					canPlace1x1 = false;
				}
				if (y == 0)
				{
					if (!checkTile.HasTile || checkTile.Slope != SlopeType.Solid)
					{
						canPlace1x1 = false;
					}
				}
			}
		}
		if (Main.rand.NextBool(12))// 巨型萤火吊
		{
			int count = 0;
			float length = 0;
			for (int x = 0; x <= 1; x++)
			{
				for (int y = 0; y <= 4; y++)
				{
					Tile t0 = Main.tile[i + x, j + y];
					if (y == 0)
					{
						if (!t0.HasTile || t0.TileType != (ushort)ModContent.TileType<DarkCocoon>() || t0.IsHalfBlock)
						{
							count++;
						}
					}
					else
					{
						if (t0.HasTile)
						{
							count++;
						}
					}
				}
			}
			if (count == 0)
			{
				for (int y = 4; y <= 80; y++)
				{
					for (int x = 0; x <= 1; x++)
					{
						Tile t0 = Main.tile[i + x, j + y];
						if (!t0.HasTile)
						{
							length += 1 / 8f;
						}
						else
						{
							y = 81;
							break;
						}
					}
				}
				if (Main.netMode != NetmodeID.Server)
				{
					if (length > 3)
					{
						LargeFireBulb.PlaceMe(i, j + 1, (ushort)Main.rand.Next(4));
					}
				}
			}
		}
		bool canPlace2x1 = true;
		for (int x = 0; x < 2; x++)
		{
			for (int y = -1; y < 1; y++)
			{
				Tile checkTile = Main.tile[i + x, j + y];
				if (checkTile.HasTile && y <= -1)
				{
					canPlace2x1 = false;
				}
				if (y == 0)
				{
					if (!checkTile.HasTile || checkTile.Slope != SlopeType.Solid)
					{
						canPlace2x1 = false;
					}
				}
			}
		}
		bool canPlace2x2 = true;
		for (int x = 0; x < 2; x++)
		{
			for (int y = -2; y < 1; y++)
			{
				Tile checkTile = Main.tile[i + x, j + y];
				if (checkTile.HasTile && y <= -1)
				{
					canPlace2x2 = false;
				}
				if (y == 0)
				{
					if (!checkTile.HasTile || checkTile.Slope != SlopeType.Solid)
					{
						canPlace2x2 = false;
					}
				}
			}
		}
		bool canPlace3x2 = true;
		for (int x = 0; x < 3; x++)
		{
			for (int y = -2; y < 1; y++)
			{
				Tile checkTile = Main.tile[i + x, j + y];
				if (checkTile.HasTile && y <= -1)
				{
					canPlace3x2 = false;
				}
				if (y == 0)
				{
					if (!checkTile.HasTile || checkTile.Slope != SlopeType.Solid)
					{
						canPlace3x2 = false;
					}
				}
			}
		}

		// 黑萤苣
		if (canPlaceShrub && !nearWater && Main.rand.NextBool(3))
		{
			Tile t1 = Main.tile[i, j - 1];
			Tile t2 = Main.tile[i, j - 2];
			Tile t3 = Main.tile[i, j - 3];
			switch (Main.rand.Next(5))
			{
				case 0:
					TileUtils.PlaceFrameImportantTiles(i, j - 3, 1, 3, ModContent.TileType<BluishGiantGentian>(), 120 * Main.rand.Next(12));
					break;
				case 1:
					t1.TileType = (ushort)ModContent.TileType<BlackStarShrubSmall>();
					t2.TileType = (ushort)ModContent.TileType<BlackStarShrubSmall>();
					t1.HasTile = true;
					t2.HasTile = true;
					short numa = (short)(Main.rand.Next(0, 6) * 48);
					t1.TileFrameX = numa;
					t2.TileFrameX = numa;
					t1.TileFrameY = 16;
					t2.TileFrameY = 0;
					break;

				case 2:
					t1.TileType = (ushort)ModContent.TileType<BlackStarShrubSmall>();
					t2.TileType = (ushort)ModContent.TileType<BlackStarShrubSmall>();
					t1.HasTile = true;
					t2.HasTile = true;
					short num = (short)(Main.rand.Next(0, 6) * 48);
					t2.TileFrameX = num;
					t1.TileFrameX = num;
					t1.TileFrameY = 16;
					t2.TileFrameY = 0;
					break;

				case 3:
					t1.TileType = (ushort)ModContent.TileType<BlackStarShrub>();
					t2.TileType = (ushort)ModContent.TileType<BlackStarShrub>();
					t3.TileType = (ushort)ModContent.TileType<BlackStarShrub>();
					t1.HasTile = true;
					t2.HasTile = true;
					t3.HasTile = true;
					short num1 = (short)(Main.rand.Next(0, 6) * 72);
					t3.TileFrameX = num1;
					t2.TileFrameX = num1;
					t1.TileFrameX = num1;
					t1.TileFrameY = 32;
					t2.TileFrameY = 16;
					t3.TileFrameY = 0;
					break;
				case 4:
					TileUtils.PlaceFrameImportantTiles(i, j - 2, 1, 2, (ushort)ModContent.TileType<BluishGiantGentian_small>(), 48 * Main.rand.Next(6));
					break;
			}
			return;
		}

		// 蕨和岩石
		if (Main.rand.NextBool(2) && !nearWater)
		{
			if (Main.rand.NextBool(2))
			{
				switch (Main.rand.Next(6))
				{
					case 0:
						if (canPlace3x2)
						{
							TileUtils.PlaceFrameImportantTiles(i, j - 2, 3, 2, ModContent.TileType<BlackFrenLarge>(), 54 * Main.rand.Next(3));
						}

						break;

					case 1:
						if (canPlace2x2)
						{
							TileUtils.PlaceFrameImportantTiles(i, j - 2, 2, 2, ModContent.TileType<BlackFren>(), 36 * Main.rand.Next(3));
						}

						break;

					case 2:
						if (canPlace3x2)
						{
							TileUtils.PlaceFrameImportantTiles(i, j - 2, 3, 2, ModContent.TileType<BlackFrenLarge>(), 54 * Main.rand.Next(3));
						}

						break;

					case 3:
						if (canPlace2x2)
						{
							TileUtils.PlaceFrameImportantTiles(i, j - 2, 2, 2, ModContent.TileType<BlackFren>(), 36 * Main.rand.Next(3));
						}

						break;

					case 4:
						if (canPlace2x1)
						{
							TileUtils.PlaceFrameImportantTiles(i, j - 1, 2, 1, ModContent.TileType<CocoonRock>(), 36 * Main.rand.Next(3));
						}

						break;

					case 5:
						if (canPlace2x1)
						{
							TileUtils.PlaceFrameImportantTiles(i, j - 1, 2, 1, ModContent.TileType<CocoonRock>(), 36 * Main.rand.Next(3));
						}

						break;
				}
			}
			return;
		}
		if (canPlace1x1)
		{
			if (nearWater)
			{
				TileUtils.PlaceFrameImportantTiles(i, j - 1, 1, 1, ModContent.TileType<GlowingReed>(), 18 * Main.rand.Next(5));
			}
			else
			{
				switch (Main.rand.Next(2))
				{
					case 0:
						TileUtils.PlaceFrameImportantTiles(i, j - 1, 1, 1, ModContent.TileType<DarkCocoonGrass>(), 18 * Main.rand.Next(6));
						break;

					case 1:
						WorldGen.PlaceTile(i, j - 1, ModContent.TileType<Tiles.PurpleThorns>());
						break;
				}
			}
		}
	}

	public static void BuildFluorescentTree(int i, int j, int height = 0)
	{
		if (j < 30)
		{
			return;
		}

		int Height = Main.rand.Next(7, height);

		for (int g = 0; g < Height; g++)
		{
			Tile tile = Main.tile[i, j - g];
			if (g > 3)
			{
				if (Main.rand.NextBool(5))
				{
					Tile tileLeft = Main.tile[i - 1, j - g];
					tileLeft.TileType = (ushort)ModContent.TileType<FluorescentTree>();
					tileLeft.TileFrameY = 4;
					tileLeft.TileFrameX = (short)Main.rand.Next(4);
					tileLeft.HasTile = true;
				}
				if (Main.rand.NextBool(5))
				{
					Tile tileRight = Main.tile[i + 1, j - g];
					tileRight.TileType = (ushort)ModContent.TileType<FluorescentTree>();
					tileRight.TileFrameY = 5;
					tileRight.TileFrameX = (short)Main.rand.Next(4);
					tileRight.HasTile = true;
				}
			}
			if (g == 0)
			{
				tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
				tile.TileFrameY = 0;
				tile.TileFrameX = 0;
				tile.HasTile = true;
				continue;
			}
			if (g == 1)
			{
				tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
				tile.TileFrameY = -1;
				tile.TileFrameX = 0;
				tile.HasTile = true;
				continue;
			}
			if (g == 2)
			{
				tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
				tile.TileFrameY = 3;
				tile.TileFrameX = (short)Main.rand.Next(4);
				tile.HasTile = true;
				continue;
			}
			if (g == Height - 1)
			{
				tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
				tile.TileFrameY = 2;
				tile.TileFrameX = (short)Main.rand.Next(2);
				tile.HasTile = true;
				continue;
			}
			tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
			tile.TileFrameY = 1;
			tile.TileFrameX = (short)Main.rand.Next(12);
			tile.HasTile = true;
		}
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}
}
