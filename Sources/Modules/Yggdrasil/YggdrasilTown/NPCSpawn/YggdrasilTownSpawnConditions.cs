using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.Common.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Walls;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCSpawn;

/// <summary>按本次原生生成位置判定环境，不依赖客户端屏幕或全局场景图块计数。</summary>
public static class YggdrasilTownSpawnConditions
{
	public static bool InForest(NPCSpawnInfo info) => CanSpawn(info) && IsForestGround(info) && !IsJellyBallHotbed(info);

	public static bool InLampWoodForest(NPCSpawnInfo info) => CanSpawn(info) &&
		info.SpawnTileType == ModContent.TileType<DarkForestGrass>() && !IsJellyBallHotbed(info);

	public static bool InTwilightForest(NPCSpawnInfo info) => CanSpawn(info) &&
		info.SpawnTileType == ModContent.TileType<TwilightGrassBlock>() && !IsJellyBallHotbed(info);

	public static bool InShallowMine(NPCSpawnInfo info) => CanSpawn(info) && IsMine(info) && info.SpawnTileY < Main.maxTilesY - 520;

	public static bool InDeepMine(NPCSpawnInfo info) => CanSpawn(info) && IsMine(info) && info.SpawnTileY >= Main.maxTilesY - 520;

	public static bool InJellyBallHotbed(NPCSpawnInfo info) => CanSpawn(info) && IsJellyBallHotbed(info);

	private static bool CanSpawn(NPCSpawnInfo info) =>
		YggdrasilSpawnRegistration.InYggdrasilTown(info.Player.Center) &&
		YggdrasilSpawnRegistration.InYggdrasilTown(new Vector2(info.SpawnTileX * 16 + 8, info.SpawnTileY * 16)) &&
		SpawnConditions.OutsideHousing(info) && SpawnConditions.IsDry(info);

	// 两种森林的底土相同，使用候选地面的草块区分，不把共享底土直接认作灯木森林。
	private static bool IsForestGround(NPCSpawnInfo info) =>
		info.SpawnTileType == ModContent.TileType<DarkForestGrass>() ||
		info.SpawnTileType == ModContent.TileType<TwilightGrassBlock>();

	private static bool IsJellyBallHotbed(NPCSpawnInfo info) =>
		info.SpawnTileType == ModContent.TileType<JellyBallSecretion>() ||
		Main.tile[info.SpawnTileX, info.SpawnTileY - 1].WallType == ModContent.WallType<JellyBallSecretionWall>();

	private static bool IsMine(NPCSpawnInfo info)
	{
		// 沿用 BuildTangledSubmine 的矿化范围；当前地图向下延伸，前 300 格为浅层，余下为深层。
		// 这是当前地图适配，不照搬旧生物策划“矿洞在上方、两层各 300 格”的坐标描述。
		return info.SpawnTileX >= 40 && info.SpawnTileX < Main.maxTilesX / 2 + 50 &&
			info.SpawnTileY >= Main.maxTilesY - 820 && info.SpawnTileY < Main.maxTilesY - 50 &&
			!IsForestGround(info) && !IsJellyBallHotbed(info);
	}
}
