namespace Everglow.Yggdrasil.Common.Fish;

public class FishSystem : ModSystem
{
	public static Dictionary<ModBiome, List<FishableItem>> FishMap = [];
	public static HashSet<int> LiquidList = [];

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

	public List<FishableItem> ShouldSpawnFish(Player player)
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

	public bool CheckSpawn(Player player, Point point, List<FishableItem> toSpawn)
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

	public void SpawnInRect(Rectangle rect, Player player, List<FishableItem> toSpawn)
	{
		int right = rect.X + rect.Width;
		int bottom = rect.Y + rect.Height;
		for (int x = rect.X; x <= right; x++)
		{
			for (int y = rect.Y; y <= bottom; y++)
			{
				Point point = new Point(x, y);
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
				if (canSpawn)
				{
					if (CheckSpawn(player, point, toSpawn))
					{
						break;
					}
				}
			}
			if (toSpawn.Count == 0)
			{
				break;
			}
		}
	}

	public void SpawnAroundPlayer(Player player)
	{
		var toSpawn = ShouldSpawnFish(player);

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

		base.PostUpdateTime();
	}
}
