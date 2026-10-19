using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.Common.NPCSpawn;
using Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;

namespace Everglow.Yggdrasil.KelpCurtain.NPCSpawn;

/// <summary>原生 SpawnTileY 指向地面，水体采样从地面上一格开始。</summary>
public static class KelpCurtainSpawnConditions
{
	public static bool InKelpCurtain(NPCSpawnInfo info) =>
		KelpCurtainBiome.IsKelpCurtainLayer(info.Player) &&
		YggdrasilSpawnRegistration.InKelpCurtain(new Vector2(info.SpawnTileX * 16 + 8, info.SpawnTileY * 16)) &&
		SpawnConditions.OutsideHousing(info);

	public static bool NearWaterBottom(NPCSpawnInfo info)
	{
		if (!info.Water)
		{
			return false;
		}
		Tile water = Main.tile[info.SpawnTileX, info.SpawnTileY - 1];
		Tile floor = Main.tile[info.SpawnTileX, info.SpawnTileY];
		return water.LiquidAmount > 0 && water.LiquidType == LiquidID.Water &&
			floor.HasTile && !floor.IsActuated && Main.tileSolid[floor.TileType];
	}

	public static bool NearWaterSurface(NPCSpawnInfo info) =>
		info.Water && WaterDepth(info, 17) is > 0 and <= 16;

	public static bool InShallowWater(NPCSpawnInfo info) =>
		info.Water && WaterDepth(info, 7) is > 0 and <= 6;

	public static bool InDeepWater(NPCSpawnInfo info) =>
		info.Water && WaterDepth(info, 7) > 6;

	public static bool CanSpawnOnWaterSurface(NPCSpawnInfo info)
	{
		if (!info.Water)
		{
			return false;
		}
		NPC sample = ModContent.GetInstance<WaterStrider>().NPC;
		return TryFindWaterSurface(info.SpawnTileX, info.SpawnTileY, sample.width, sample.height, out _);
	}

	/// <summary>水黾的中心位于最上层液体格，与它的水面 AI 判定一致；不缓存候选。</summary>
	public static bool TryFindWaterSurface(int tileX, int floorY, int width, int height, out Vector2 feet)
	{
		feet = default;
		int depth = CountWaterAbove(floorY, 128, y => IsWater(tileX, y));
		if (depth == 0 || depth == 128)
		{
			return false;
		}
		int surfaceY = floorY - depth;
		if (!WorldGen.InWorld(tileX, surfaceY - 1, 2) || Main.tile[tileX, surfaceY - 1].LiquidAmount != 0)
		{
			return false;
		}
		feet = new Vector2(tileX * 16 + 8, surfaceY * 16 + 8 + height / 2);
		return KelpCurtainNaturalSpawn.CanPlace(feet, width, height);
	}

	private static int WaterDepth(NPCSpawnInfo info, int limit) =>
		CountWaterAbove(info.SpawnTileY, limit, y => IsWater(info.SpawnTileX, y));

	private static int CountWaterAbove(int floorY, int limit, Func<int, bool> isWater)
	{
		int depth = 0;
		while (depth < limit && isWater(floorY - depth - 1))
		{
			depth++;
		}
		return depth;
	}

	private static bool IsWater(int x, int y) => WorldGen.InWorld(x, y, 2) &&
		Main.tile[x, y].LiquidAmount > 0 && Main.tile[x, y].LiquidType == LiquidID.Water;
}
