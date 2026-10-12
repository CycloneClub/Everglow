namespace Everglow.Commons.Mechanics.NPCSpawn;

/// <summary>针对 tML 原生候选地面的条件；SpawnTileY 指向地面，身体所在液体取上方格子。坐标由原生选点或扩群位置检查保证有效。</summary>
public static class SpawnConditions
{
	public static bool IsDry(NPCSpawnInfo info) =>
		Main.tile[info.SpawnTileX, info.SpawnTileY - 1].LiquidAmount == 0;

	public static bool IsWater(NPCSpawnInfo info) => info.Water &&
		Main.tile[info.SpawnTileX, info.SpawnTileY - 1].LiquidType == LiquidID.Water;

	public static bool OutsideHousing(NPCSpawnInfo info) =>
		!Main.wallHouse[Main.tile[info.SpawnTileX, info.SpawnTileY - 1].WallType];

	public static bool OnGround(NPCSpawnInfo info)
	{
		Tile tile = Main.tile[info.SpawnTileX, info.SpawnTileY];
		return tile.HasUnactuatedTile && Main.tileSolid[tile.TileType];
	}
}
