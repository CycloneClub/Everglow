using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.Common.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCSpawn;

/// <summary>内容加载后调用；规则按区域维护，由 SpawnChance 计算本次权重。</summary>
public static class YggdrasilTownSpawnTable
{
	public static void AddRules(List<SubworldSpawnRule> rules)
	{
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<BarkSpicyCaterpillar>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 8f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CarapaceCaterpillar>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 8f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CrimsonSpell>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) &&
				!NPC.AnyNPCs(ModContent.NPCType<CrimsonSpell>())
				? 0.05f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CyanOreBeetle>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<DarkGlimmeringRods>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GiantJellyBall>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<JellyBall>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LampFruitBorer>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 8f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LeafcutterAnt>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LightBulb>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<RockElemental>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) &&
				!NPC.AnyNPCs(ModContent.NPCType<RockElemental>())
				? 0.05f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<ShadowstrikeCaterpillar>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 8f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<WhisperingGhost>(),
			SpawnChance = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<UmbrellaTailThief>(),
			SpawnChance = spawnInfo => ModContent.GetInstance<LampWoodForest>().IsBiomeActive(spawnInfo.Player)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<PurpleBombCaterpillar>(),
			SpawnChance = spawnInfo => ModContent.GetInstance<LampWoodForest>().IsBiomeActive(spawnInfo.Player)
				? 3f : 0f,
		});
	}

	private static bool InYggdrasilTown(NPCSpawnInfo info) =>
		YggdrasilSpawnRegistration.InYggdrasilTown(info.Player.Center)
		&& YggdrasilSpawnRegistration.InYggdrasilTown(new Vector2(info.SpawnTileX * 16 + 8, info.SpawnTileY * 16))
		&& SpawnConditions.OutsideHousing(info);
}
