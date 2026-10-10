using Everglow.Myth.TheFirefly.WorldGeneration;
using System.Collections.Generic;
using Terraria;

namespace Everglow.Myth.TheFirefly.GlobalNPCs;

public class SpawnControl : GlobalNPC
{
	// 编辑 NPC 生成池子。
	public override void EditSpawnPool(IDictionary<int, float> pool, NPC.Spawner spawner)
	{
		foreach (var kv in pool)
		{
			// 生成位置在流萤地形内的原版 NPC 禁止生成。
			if (!CanSpawnToFirefly(kv.Key, spawner))
			{
				pool.Remove(kv.Key);
			}
		}
	}

	// 能否生成与流萤地形。如果给流萤地生物创建好集合了，还需要在这个地方修改下。
	public static bool CanSpawnToFirefly(int type, NPC.Spawner spawnInfo)
	{
		// 0 代表是原版 NPC，不会在这里给出要生成的怪的 Type，ModNPC 会直接给出来。
		if (type == 0 && true &&
			(InFirefly(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY) || // 在没在流萤范围
			spawnInfo.SpawnTileType == ModContent.TileType<Tiles.DarkCocoon>())) // 在没在环境砖块上，不判断是否自然生成会导致这个物块放到哪里哪里不生成怪。
		{
			return false;
		}
		return true;
	}

	// 判断坐标是否在流萤地形内。我觉的可以给ModPlayer加上这个，判断玩家是不是在地形。
	public static bool InFirefly(int tileX, int tileY)
	{
		MothLand mothLand = ModContent.GetInstance<MothLand>(); // 联机应该没问题。
		var BiomeCenter = new Vector2(mothLand.fireflyCenterX * 16, (mothLand.fireflyCenterY - 20) * 16);
		Vector2 distance = new Vector2(tileX * 16, tileY * 16) - BiomeCenter;
		distance.Y *= 1.35f;
		distance.X *= 0.9f;
		return distance.Length() < 2000;
	}
}
