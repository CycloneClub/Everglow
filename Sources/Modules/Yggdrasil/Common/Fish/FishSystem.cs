namespace Everglow.Yggdrasil.Common.Fish;

public class FishSystem : ModSystem
{
	/// <summary>
	/// 生成点周围上下左右一定范围内不得有方块，包括含液体的方块。
	/// </summary>
	private const int SpawnTileClearance = 10;

	public static Dictionary<ModBiome, List<FishableItem>> FishMap = [];
	public static HashSet<int> LiquidList = [];

	public override void Load()
	{
		FishMap = [];
		LiquidList = [];
	}

	public override void Unload()
	{
		FishMap.Clear();
		FishMap = null;

		LiquidList.Clear();
		LiquidList = null;
	}

	/// <summary>
	/// 注册一种可以被钩取的渔获，会自然生成在指定生态群系的水面上
	/// </summary>
	/// <param name="biome">会自然生成渔获的mod群系</param>
	/// <param name="item">会自然生成的物品</param>
	public static void RegisterFish(ModBiome biome, FishableItem item)
	{
		if (!FishMap.TryGetValue(biome, out List<FishableItem> value))
		{
			value = [];
			FishMap[biome] = value;
		}

		value.Add(item);
	}

	/// <summary>
	/// 注册一种液体，在钓鱼时将会模拟原版液体的行为
	/// </summary>
	/// <param name="liquid">图块的 ID</param>
	public static void RegisterLiquid(int liquid)
	{
		LiquidList.Add(liquid);
	}

	private List<FishableItem> ShouldSpawnFish(Player player)
	{
		List<FishableItem> toSpawn = [];
		foreach (KeyValuePair<ModBiome, List<FishableItem>> kvp in FishMap)
		{
			if (player.InModBiome(kvp.Key))
			{
				foreach (var item in kvp.Value)
				{
					float chance = Main.rand.NextFloat(1);
					if (chance < item.Chance)
					{
						toSpawn.Add(item);
					}
				}
			}
		}
		return toSpawn;
	}

	private bool CheckSpawn(Player player, Point point, List<FishableItem> toSpawn)
	{
		Tile tile = TileUtils.SafeGetTile(point);
		List<FishableItem> spawned = [];
		foreach (var item in toSpawn)
		{
			if (tile.LiquidType != item.Liquid || tile.LiquidAmount < 26)
			{
				continue;
			}
			spawned.Add(item);
			var itemIndex = Item.NewItem(player.GetSource_FromThis(), new Vector2(point.X * 16f, point.Y * 16f), item.Item);
			var itemGen = Main.item[itemIndex];
			if (itemGen != null)
			{
				if (itemGen.TryGetGlobalItem(out FishGlobalItem globalItem))
				{
					globalItem.Fishable = true;
					globalItem.FloatSpeed = Main.rand.NextFloat(.5f);
				}
			}
		}
		foreach (var item in spawned)
		{
			toSpawn.Remove(item);
		}
		return toSpawn.Count == 0;
	}

	private static bool IsSpawnAreaClear(Point point)
	{
		int left = point.X - SpawnTileClearance;
		int right = point.X + SpawnTileClearance;
		int top = point.Y - SpawnTileClearance;
		int bottom = point.Y + SpawnTileClearance;
		if (left < 0 || top < 0 || right >= Main.maxTilesX || bottom >= Main.maxTilesY)
		{
			return false;
		}

		for (int x = left; x <= right; x++)
		{
			for (int y = top; y <= bottom; y++)
			{
				if (Main.tile[x, y].HasTile)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void SpawnInRect(Rectangle rect, Player player, List<FishableItem> toSpawn)
	{
		if (toSpawn.Count == 0)
		{
			return;
		}

		var liquids = toSpawn.Select(item => item.Liquid).ToHashSet();

		int left = Math.Max(rect.X, SpawnTileClearance);
		int top = Math.Max(rect.Y, SpawnTileClearance);
		int right = Math.Min(rect.X + rect.Width, Main.maxTilesX - SpawnTileClearance - 1);
		int bottom = Math.Min(rect.Y + rect.Height, Main.maxTilesY - SpawnTileClearance - 1);
		for (int x = left; x <= right; x++)
		{
			for (int y = top; y <= bottom; y++)
			{
				Point point = new Point(x, y);
				Tile cTile = Main.tile[point];
				int liquidType = cTile.LiquidType;
				if (cTile.LiquidAmount <= 26 || !liquids.Contains(liquidType))
				{
					continue;
				}

				bool canSpawn = true;
				for (int nx = -3; nx <= 3; nx++)
				{
					Point np = point + new Point(nx, 0);
					Tile tile = TileUtils.SafeGetTile(np);
					if (tile.LiquidAmount <= 26)
					{
						canSpawn = false;
						break;
					}
				}
				if (canSpawn && IsSpawnAreaClear(point))
				{
					if (CheckSpawn(player, point, toSpawn))
					{
						return;
					}

					liquids.Remove(liquidType);
				}
			}
		}
	}

	private void SpawnAroundPlayer(Player player)
	{
		var toSpawn = ShouldSpawnFish(player);
		if (toSpawn.Count == 0)
		{
			return;
		}

		// 在刷怪区域的左右两侧生成，上下不生成
		Point playerPoint = player.Center.ToTileCoordinates();
		Rectangle rect1 = new Rectangle(playerPoint.X - 84, playerPoint.Y - 47, 20, 94);
		Rectangle rect2 = new Rectangle(playerPoint.X + 64, playerPoint.Y - 47, 20, 94);

		float first = Main.rand.NextFloat(1);
		if (first < .5f)
		{
			SpawnInRect(rect1, player, toSpawn);
			SpawnInRect(rect2, player, toSpawn);
		}
		else
		{
			SpawnInRect(rect2, player, toSpawn);
			SpawnInRect(rect1, player, toSpawn);
		}
	}

	public override void PostUpdateTime()
	{
		foreach (var player in Main.player)
		{
			if (player.active)
			{
				SpawnAroundPlayer(player);
			}
		}
	}
}
