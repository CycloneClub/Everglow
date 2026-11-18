using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.Common.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
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

	internal static int GetEucaLeafyMaxLift(int tileX, int floorY)
	{
		const int treeHeight = 7;
		NPC sample = ModContent.GetInstance<EucaLeafy>().NPC;
		int left = tileX * 16 + 8 - sample.width / 2;
		int right = left + sample.width - 1;
		if (!WorldGen.InWorld(left / 16, floorY - treeHeight, 1) || !WorldGen.InWorld(right / 16, floorY, 1))
		{
			return 0;
		}
		Tile floor = Main.tile[tileX, floorY];
		if (!floor.HasUnactuatedTile || floor.TileType != ModContent.TileType<TwilightGrassBlock>())
		{
			return 0;
		}

		int bodyRows = (sample.height + 15) / 16;
		int maxLift = 0;
		for (int row = 1; row <= treeHeight; row++)
		{
			int y = floorY - row;
			for (int x = left / 16; x <= right / 16; x++)
			{
				Tile tile = Main.tile[x, y];
				if ((tile.HasUnactuatedTile && Main.tileSolid[tile.TileType]) || tile.LiquidAmount > 0 ||
					Main.wallHouse[tile.WallType] || !YggdrasilSpawnRegistration.InYggdrasilTown(new Vector2(x * 16, y * 16)))
				{
					return maxLift;
				}
			}
			var strip = new Rectangle(left, y * 16, sample.width, 16);
			foreach (Player player in Main.ActivePlayers)
			{
				var safe = new Rectangle(
					(int)player.Center.X - NPC.sWidth / 2 - NPC.safeRangeX,
					(int)player.Center.Y - NPC.sHeight / 2 - NPC.safeRangeY,
					NPC.sWidth + NPC.safeRangeX * 2,
					NPC.sHeight + NPC.safeRangeY * 2);
				if (strip.Intersects(safe))
				{
					return maxLift;
				}
			}
			maxLift = Math.Max(0, row - bodyRows);
		}
		return maxLift;
	}

	public static bool InShallowMine(NPCSpawnInfo info) => CanSpawn(info) && IsMine(info) && info.SpawnTileY < Main.maxTilesY - 520;

	public static bool InDeepMine(NPCSpawnInfo info) => CanSpawn(info) && IsMine(info) && info.SpawnTileY >= Main.maxTilesY - 520;

	public static bool InJellyBallHotbed(NPCSpawnInfo info) => CanSpawn(info) && IsJellyBallHotbed(info);

	private static bool CanSpawn(NPCSpawnInfo info) =>
		YggdrasilSpawnRegistration.InYggdrasilTown(info.Player.Center) &&
		YggdrasilSpawnRegistration.InYggdrasilTown(new Vector2(info.SpawnTileX * 16 + 8, info.SpawnTileY * 16)) &&
		!Main.wallHouse[Main.tile[info.SpawnTileX, info.SpawnTileY - 1].WallType] && SpawnConditions.IsDry(info);

	// 两种森林的底土相同，使用候选地面的草块区分，不把共享底土直接认作灯木森林。
	private static bool IsForestGround(NPCSpawnInfo info) =>
		info.SpawnTileType == ModContent.TileType<DarkForestGrass>() ||
		info.SpawnTileType == ModContent.TileType<TwilightGrassBlock>();

	private static bool IsJellyBallHotbed(NPCSpawnInfo info) =>
		info.SpawnTileType == ModContent.TileType<JellyBallSecretion>() ||
		Main.tile[info.SpawnTileX, info.SpawnTileY - 1].WallType == ModContent.WallType<JellyBallSecretionWall>() ||
		(info.SpawnTileType == ModContent.TileType<StoneScaleWood>() && InHotbedContour(info.SpawnTileX, info.SpawnTileY));

	private static bool InHotbedContour(int x, int y)
	{
		// 与 BuildJellyBallHotbed 的填土/墙体外轮廓一致。温床内也有无分泌物的石鳞木地面，
		// 不能仅凭落点的单个地面/墙类型识别群系；轮廓外仍需实际分泌块或分泌墙。
		if (x < Main.maxTilesX - 350 || x >= Main.maxTilesX ||
			y < Main.maxTilesY - 500 || y > Main.maxTilesY - 40)
		{
			return false;
		}
		var offset = new Vector2(Main.maxTilesX - 175 - x, Main.maxTilesY - 390 - y);
		float radius = 150 - 83 * MathF.Sin(offset.ToRotation());
		offset.Y /= 1.2f;
		return offset.LengthSquared() <= radius * radius;
	}

	private static bool IsMine(NPCSpawnInfo info)
	{
		// 沿用 BuildTangledSubmine 的矿化范围；当前地图向下延伸，前 300 格为浅层，余下为深层。
		// 这是当前地图适配，不照搬旧生物策划“矿洞在上方、两层各 300 格”的坐标描述。
		return info.SpawnTileX >= 40 && info.SpawnTileX < Main.maxTilesX / 2 + 50 &&
			info.SpawnTileY >= Main.maxTilesY - 820 && info.SpawnTileY < Main.maxTilesY - 50 &&
			!IsForestGround(info) && !IsJellyBallHotbed(info);
	}
}
