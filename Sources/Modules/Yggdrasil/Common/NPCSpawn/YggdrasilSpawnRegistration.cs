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
		SubworldSpawnSystem.RegisterWorld(new(WorldId, () => YggdrasilWorld.InYggdrasil, SpawnDensity));
		var kelp = new List<SubworldSpawnRule>();
		KelpCurtainSpawnTable.AddRules(kelp);
		SubworldSpawnSystem.RegisterRules(WorldId, nameof(KelpCurtainSpawnTable), kelp, InKelpCurtain);

		var town = new List<SubworldSpawnRule>();
		YggdrasilTownSpawnTable.AddRules(town);
		SubworldSpawnSystem.RegisterRules(WorldId, nameof(YggdrasilTownSpawnTable), town, InTown);
	}

	public static bool InTown(Vector2 position) => YggdrasilTownBiome.IsTownLayer(position) && !InTownArea(position);

	public static bool InKelpCurtain(Vector2 position) => YggdrasilWorld.InYggdrasil && !InTownArea(position) &&
		KelpCurtainBiome.StratumBoundCurve.Count > 0 &&
		position.Y > Main.maxTilesY * 0.72f * 16 && position.Y < Main.maxTilesY * 0.9f * 16 &&
		position.X >= KelpCurtainBiome.FindClosestStratumBoundPointX(position.Y / 16f) * 16;

	// 在登记规则表的区域检查中排除镇内落点，群体追加成员也会复用该检查。
	private static bool InTownArea(Vector2 position)
	{
		if (YggdrasilTownBiome.BiomeCenter == Vector2.Zero)
		{
			return false;
		}
		Rectangle area = YggdrasilTownCentralSystem.TownArea;
		Point tile = position.ToTileCoordinates();
		// 与 InYggdrasilTown 一致，右侧和下侧边界格也属于镇内。
		return tile.X >= area.Left && tile.X <= area.Right && tile.Y >= area.Top && tile.Y <= area.Bottom;
	}

	private static float SpawnDensity(Vector2 tilePosition) =>
		InTownArea(tilePosition * 16f) ? 0f : EllipseDensity(tilePosition);

	public static float EllipseDensity(Vector2 tilePosition)
	{
		float x = (tilePosition.X - 600f) / 300f;
		float y = (tilePosition.Y - 11630f) / 150f;
		return Math.Clamp(x * x + y * y, 0f, 1f);
	}
}
