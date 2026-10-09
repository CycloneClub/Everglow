namespace Everglow.Commons.TileHelper;

public class WorldGenMisc
{
	/// <summary>
	/// 以给定的坐标为左下角，放置内含物品的箱子
	/// </summary>
	public static void PlaceChest(int x, int y, int type, List<Item> itemList, int style = 0)
	{
		if (x >= Main.maxTilesX - 2 || x <= 2 || y >= Main.maxTilesY - 2 || y <= 2)
		{
			throw new Exception("the chest stand out of the world!");
		}
		SmoothTileOfAreaXYWH(x - 1, y + 1, 4, 2);
		WorldGen.PlaceChest(x, y, (ushort)type, false, style);
		foreach (Chest chest in Main.chest)
		{
			if (chest != null)
			{
				if (chest.x == x && chest.y + 1 == y)
				{
					for (int t = 0; t < itemList.Count; t++)
					{
						chest.item[t] = itemList[t];
					}
				}
			}
		}
	}

	/// <summary>
	/// Fill chest by given coord, make sure this point is the left bottom tile of a chest.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="itemList"></param>
	/// <exception cref="Exception"></exception>
	public static void TryFillChest(int x, int y, List<Item> itemList)
	{
		if (x >= Main.maxTilesX - 2 || x <= 2 || y >= Main.maxTilesY - 2 || y <= 2)
		{
			throw new Exception("the chest stand out of the world!");
		}
		foreach (Chest chest in Main.chest)
		{
			if (chest != null)
			{
				if (chest.x == x && chest.y + 1 == y)
				{
					for (int t = 0; t < itemList.Count; t++)
					{
						chest.item[t] = itemList[t];
					}
				}
			}
		}

		// int cIndex = Chest.FindChest(x, y);
		// if (cIndex >= 0)
		// {
		// Chest chest = Main.chest[cIndex];
		// if (chest != null)
		// {
		// if (chest.x == x && chest.y + 1 == y)
		// {
		// for (int t = 0; t < itemList.Count; t++)
		// {
		// chest.item[t] = itemList[t];
		// }
		// }
		// }
		// }
	}

	/// <summary>
	/// 以[x0,y0]为左上顶点,[x1,y1]为右下顶点平滑物块
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	public static void SmoothTileOfAreaFourPoint(int x0, int y0, int x1, int y1)
	{
		x0 = Math.Clamp(x0, 20, Main.maxTilesX - 20);
		x1 = Math.Clamp(x1, 20, Main.maxTilesX - 20);
		y0 = Math.Clamp(y0, 20, Main.maxTilesY - 20);
		y1 = Math.Clamp(y1, 20, Main.maxTilesY - 20);
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = Main.tile[x, y];
				Tile tileUp = Main.tile[x, y - 1];
				if (tile.TileType == TileID.Containers)
				{
					continue;
				}
				if (tileUp.TileType == TileID.Containers)
				{
					continue;
				}
				Tile.SmoothSlope(x, y, false);
				WorldGen.TileFrame(x, y, true, false);
				WorldGen.SquareWallFrame(x, y, true);
			}
		}
	}

	/// <summary>
	/// 以x, y, width, height的格式确定区域平滑物块
	/// </summary>
	/// <param name="x">待平滑区域左上角的物块横坐标，会根据世界边界调整。</param>
	/// <param name="y">待平滑区域左上角的物块纵坐标，会根据世界边界调整。</param>
	/// <param name="width">待平滑区域的宽度，以物块为单位。</param>
	/// <param name="height">待平滑区域的高度，以物块为单位。</param>
	public static void SmoothTileOfAreaXYWH(int x, int y, int width, int height)
	{
		x = Math.Clamp(x, 20, Main.maxTilesX - 20);
		y = Math.Clamp(y, 20, Main.maxTilesY - 20);
		if (x + width > Main.maxTilesX - 20)
		{
			x += Main.maxTilesX - 20 - (x + width);
		}
		if (y + height > Main.maxTilesY - 20)
		{
			y += Main.maxTilesY - 20 - (y + height);
		}
		for (int xi = x; xi < x + width; xi += 1)
		{
			for (int yi = y; yi < y + height; yi += 1)
			{
				Tile.SmoothSlope(xi, yi, false);
				WorldGen.TileFrame(xi, yi, true, false);
				WorldGen.SquareWallFrame(xi, yi, true);
			}
		}
	}

	/// <summary>
	/// Connect a cable tile by 2 given points.
	/// </summary>
	/// <param name="i0"></param>
	/// <param name="j0"></param>
	/// <param name="i1"></param>
	/// <param name="j1"></param>
	/// <param name="type"></param>
	/// <param name="style"></param>
	/// <returns></returns>
	public static bool PlaceRope(int i0, int j0, int i1, int j1, int type, int style = 0)
	{
		if ((i0, j0) == (i1, j1))
		{
			return false;
		}
		i0 = Math.Clamp(i0, 20, Main.maxTilesX - 20);
		j0 = Math.Clamp(j0, 20, Main.maxTilesY - 20);
		i1 = Math.Clamp(i1, 20, Main.maxTilesX - 20);
		j1 = Math.Clamp(j1, 20, Main.maxTilesY - 20);
		if ((Main.tile[i0, j0].HasTile && Main.tile[i0, j0].TileType != type) || Main.tile[i1, j1].HasTile)
		{
			return false;
		}
		if (TileLoader.GetTile(type) is CableTile)
		{
			CableTile cableTile = TileLoader.GetTile(type) as CableTile;
			cableTile.CreateACableTile(i0, j0, i1, j1, style);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Do damage to a certain tile.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="damage"></param>
	/// <param name="player"></param>
	public static void DamageTile(int x, int y, int damage, Player player = null)
	{
		if (player == null)
		{
			player = Main.LocalPlayer;
		}
		if (Main.LocalPlayer == null)
		{
			return;
		}
		HitTile hitTile = player.hitTile;
		int tileId = hitTile.HitObject(x, y, 1);
		bool killed = hitTile.AddDamage(tileId, damage, true) >= 100;
		if (killed)
		{
			WorldGen.KillTile(x, y);
		}
	}

	/// <summary>
	/// Do damage to a certain tile.
	/// </summary>
	/// <param name="point">Tile coordinates of the tile receiving damage.</param>
	/// <param name="damage"></param>
	/// <param name="player"></param>
	public static void DamageTile(Point point, int damage, Player player = null)
	{
		DamageTile(point.X, point.Y, damage, player);
	}

	public static Tile SafeGetTile(int i, int j)
	{
		return Main.tile[Math.Clamp(i, 20, Main.maxTilesX - 20), Math.Clamp(j, 20, Main.maxTilesY - 20)];
	}

	public static Tile SafeGetTile(Point point)
	{
		return SafeGetTile(point.X, point.Y);
	}

	public static Tile SafeGetTile_WorldCoord(Vector2 position)
	{
		return SafeGetTile(position.ToTileCoordinates());
	}
}
