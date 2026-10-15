using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;
using static Everglow.Commons.Utilities.TileUtils;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

internal class TreePlacer : ModItem
{
	// TODO:这是一个代码测试物品，使用后可能会引起程序崩坏
	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.useTime = 40;
		Item.useAnimation = 40;
	}

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			int i = (int)(Main.MouseWorld.X / 16);
			int j = (int)(Main.MouseWorld.Y / 16);
			WorldGen.PlaceTile(i, j, ModContent.TileType<TwilightGrassBlock>());
			BuildTwilightTree(i, j, 44);
		}
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			int i = (int)(Main.MouseWorld.X / 16);
			int j = (int)(Main.MouseWorld.Y / 16);
			Main.tile[i, j].TileType = (ushort)ModContent.TileType<TwilightGrassBlock>();
		}
	}

	public static void BuildTwilightTree(int i, int j, int height = 20)
	{
		if (j < 45)
		{
			return;
		}
		if (!ChestSafe(i, j) || !ChestSafe(i + 1, j))
		{
			return;
		}
		int Height = height;

		for (int g = 0; g < Height; g++)
		{
			Tile tile = TileUtils.SafeGetTile(i, j - g);
			Tile tileRight = TileUtils.SafeGetTile(i + 1, j - g);
			if (!ChestSafe(i, j - g - 1) || !ChestSafe(i + 1, j - g - 1))
			{
				Height = g + 1;
			}
			if (g == 0)
			{
				tile.TileType = (ushort)ModContent.TileType<TwilightTree>();
				tile.TileFrameY = 0;
				tile.TileFrameX = 0;
				tile.HasTile = true;

				tileRight.TileType = (ushort)ModContent.TileType<TwilightTree>();
				tileRight.TileFrameY = -1;
				tileRight.TileFrameX = 0;
				tileRight.HasTile = true;
				continue;
			}
			if (g == 1)
			{
				tile.TileType = (ushort)ModContent.TileType<TwilightTree>();
				tile.TileFrameY = -1;
				tile.TileFrameX = 0;
				tile.HasTile = true;

				tileRight.TileType = (ushort)ModContent.TileType<TwilightTree>();
				tileRight.TileFrameY = -1;
				tileRight.TileFrameX = 0;
				tileRight.HasTile = true;
				continue;
			}

			if (g == Height - 1)
			{
				tile.TileType = (ushort)ModContent.TileType<TwilightTree>();
				tile.TileFrameY = 3;
				tile.TileFrameX = 0;
				tile.HasTile = true;

				tileRight.TileType = (ushort)ModContent.TileType<TwilightTree>();
				tileRight.TileFrameY = -1;
				tileRight.TileFrameX = 0;
				tileRight.HasTile = true;
				continue;
			}
			tile.TileType = (ushort)ModContent.TileType<TwilightTree>();
			tile.TileFrameY = 1;
			tile.TileFrameX = (short)Main.rand.Next(8);
			tile.HasTile = true;

			tileRight.TileType = (ushort)ModContent.TileType<TwilightTree>();
			tileRight.TileFrameY = 2;
			tileRight.TileFrameX = (short)Main.rand.Next(8);
			tileRight.HasTile = true;
		}
	}
}
