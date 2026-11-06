using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Yggdrasil.KelpCurtain.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown;
using Everglow.Yggdrasil.YggdrasilTown.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.Common.NPCSpawn;

/// <summary>向统一入口提供天穹树世界配置和区域规则表。</summary>
public sealed class YggdrasilSpawnRegistration : ModSystem
{
	public const string WorldId = "Everglow.Yggdrasil";

	public override void PostSetupContent()
	{
		SubworldSpawnSystem.RegisterWorld(new(WorldId, () => YggdrasilWorld.InYggdrasil, tilePosition => !InSafeArea(tilePosition * 16f)));
		var kelp = new List<SubworldSpawnRule>();
		KelpCurtainSpawnTable.AddRules(kelp);
		SubworldSpawnSystem.RegisterRules(WorldId, nameof(KelpCurtainSpawnTable), kelp, InKelpCurtain);

		var town = new List<SubworldSpawnRule>();
		YggdrasilTownSpawnTable.AddRules(town);
		SubworldSpawnSystem.RegisterRules(WorldId, nameof(YggdrasilTownSpawnTable), town, InYggdrasilTown);
	}

	public static bool InYggdrasilTown(Vector2 position) => YggdrasilTownBiome.IsTownLayer(position) && !InSafeArea(position);

	public static bool InKelpCurtain(Vector2 position) => YggdrasilWorld.InYggdrasil && !InSafeArea(position) &&
		KelpCurtainBiome.StratumBoundCurve.Count > 0 &&
		position.Y > Main.maxTilesY * 0.72f * 16 && position.Y < Main.maxTilesY * 0.9f * 16 &&
		position.X >= KelpCurtainBiome.FindClosestStratumBoundPointX(position.Y / 16f) * 16;

	// 自然生成落点、群体追加和玩家位置禁刷共用城镇及交通区域的禁刷范围。
	private static bool InSafeArea(Vector2 position)
	{
		if (YggdrasilTownBiome.BiomeCenter == Vector2.Zero)
		{
			return false;
		}
		Rectangle area = YggdrasilTownCentralSystem.TownArea;
		Point tile = position.ToTileCoordinates();
		// 与 InYggdrasilTown 一致，右侧和下侧边界格也属于镇内。
		if (tile.X >= area.Left && tile.X <= area.Right && tile.Y >= area.Top && tile.Y <= area.Bottom)
		{
			return true;
		}
		// 对应 YggdrasilTownGeneration 的 120 段 3×16 轨道，以及末端 230×58 晶塔车站。
		var railway = new Rectangle(area.Right, area.Top + 189, 360, 16);
		var pylonStation = new Rectangle(railway.Right, railway.Top - 43, 230, 58);
		return railway.Contains(tile) || pylonStation.Contains(tile);
	}
}
