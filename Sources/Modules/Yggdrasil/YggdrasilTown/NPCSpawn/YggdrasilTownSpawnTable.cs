using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.Common.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCSpawn;

/// <summary>内容加载后调用；普通规则只声明类型、权重和条件。</summary>
public static class YggdrasilTownSpawnTable
{
	public static void AddRules(List<SubworldSpawnRule> rules)
	{
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<BarkSpicyCaterpillar>(),
			Weight = 8f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CarapaceCaterpillar>(),
			Weight = 8f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CrimsonSpell>(),
			Weight = 0.05f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) &&
				!NPC.AnyNPCs(ModContent.NPCType<CrimsonSpell>()),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CyanOreBeetle>(),
			Weight = 3f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<DarkGlimmeringRods>(),
			Weight = 3f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GiantJellyBall>(),
			Weight = 3f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<JellyBall>(),
			Weight = 3f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LampFruitBorer>(),
			Weight = 8f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LeafcutterAnt>(),
			Weight = 3f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LightBulb>(),
			Weight = 3f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<RockElemental>(),
			Weight = 0.05f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) &&
				!NPC.AnyNPCs(ModContent.NPCType<RockElemental>()),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<ShadowstrikeCaterpillar>(),
			Weight = 8f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<WhisperingGhost>(),
			Weight = 3f,
			Condition = spawnInfo => InYggdrasilTown(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<UmbrellaTailThief>(),
			Weight = 3f,
			Condition = spawnInfo => ModContent.GetInstance<LampWoodForest>().IsBiomeActive(spawnInfo.Player),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<PurpleBombCaterpillar>(),
			Weight = 3f,
			Condition = spawnInfo => ModContent.GetInstance<LampWoodForest>().IsBiomeActive(spawnInfo.Player),
		});
	}

	private static bool InYggdrasilTown(NPCSpawnInfo info) =>
		YggdrasilSpawnRegistration.InYggdrasilTown(info.Player.Center)
		&& YggdrasilSpawnRegistration.InYggdrasilTown(new Vector2(info.SpawnTileX * 16 + 8, info.SpawnTileY * 16))
		&& SpawnConditions.OutsideHousing(info);
}
