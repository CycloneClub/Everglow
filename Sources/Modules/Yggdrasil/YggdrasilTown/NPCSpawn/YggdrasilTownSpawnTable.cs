using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using static Everglow.Yggdrasil.YggdrasilTown.NPCSpawn.YggdrasilTownSpawnConditions;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCSpawn;

/// <summary>按环境和昼夜组织名单；权重为初始调参值，不是策划指定的百分比。</summary>
public static class YggdrasilTownSpawnTable
{
	public static void AddRules(List<SubworldSpawnRule> rules)
	{
		// 灯木白昼
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<BarkSpicyCaterpillar>(),
			SpawnChance = spawnInfo => Main.dayTime && InLampWoodForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 2f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LeafcutterAnt>(),
			SpawnChance = spawnInfo => Main.dayTime && InLampWoodForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<PurpleBombCaterpillar>(),
			SpawnChance = spawnInfo => Main.dayTime && InLampWoodForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 6f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<UmbrellaTailThief>(),
			SpawnChance = spawnInfo => Main.dayTime && InLampWoodForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 6f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LightBulb>(),
			SpawnChance = spawnInfo => Main.dayTime && InLampWoodForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		// 灯木夜晚：夜行毛虫最常见，幽灵比飞棍常见，血月教徒稀有。
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LampFruitBorer>(),
			SpawnChance = spawnInfo => Main.dayTime && InLampWoodForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 2f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<ShadowstrikeCaterpillar>(),
			SpawnChance = spawnInfo => !Main.dayTime && InLampWoodForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 2f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<WhisperingGhost>(),
			SpawnChance = spawnInfo => !Main.dayTime && InLampWoodForest(spawnInfo)
				? 1.5f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<DarkGlimmeringRods>(),
			SpawnChance = spawnInfo => !Main.dayTime && InLampWoodForest(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CrimsonSpell>(),
			SpawnChance = spawnInfo => !Main.dayTime && InLampWoodForest(spawnInfo) && !NPC.AnyNPCs(ModContent.NPCType<CrimsonSpell>())
				? 0.05f : 0f,
		});

		// 暮光林地：现有四种旧时幻影昼夜均可生成，暂用相同权重。
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AncientPhantom_Sheild>(),
			SpawnChance = spawnInfo => InTwilightForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AncientPhantom_Spear>(),
			SpawnChance = spawnInfo => InTwilightForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AncientPhantom_Bow>(),
			SpawnChance = spawnInfo => InTwilightForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AncientPhantom_Magic>(),
			SpawnChance = spawnInfo => InTwilightForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		// TODO：晶胞虫

		// 原版无幸运修正时，普通史莱姆有 1/180 的机会变为粉史莱姆。
		// 此处没有普通史莱姆候选，将同一基础比例用于灯木、暮光森林的白昼地面生成；不额外抽取随机数。
		// W / 179 与其余候选 W 一起加权，最终占比为 1/180。
		var forestRules = rules.ToArray();
		rules.Add(new SubworldSpawnRule
		{
			Type = NPCID.Pinky,
			SpawnChance = spawnInfo => Main.dayTime && InForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? forestRules.Sum(rule => rule.SpawnChance(spawnInfo)) / 179f : 0f,
		});

		// 浅层矿洞：不分昼夜，球冻最常见。
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CyanOreBeetle>(),
			SpawnChance = spawnInfo => InShallowMine(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<JellyBall>(),
			SpawnChance = spawnInfo => InShallowMine(spawnInfo)
				? 8f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<RockElemental>(),
			SpawnChance = spawnInfo => InShallowMine(spawnInfo) && !NPC.AnyNPCs(ModContent.NPCType<RockElemental>())
				? 0.05f : 0f,
		});

		// 深层矿洞：不分昼夜，大型球冻占比提高，加入石甲壳毛虫。
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CyanOreBeetle>(),
			SpawnChance = spawnInfo => InDeepMine(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<JellyBall>(),
			SpawnChance = spawnInfo => InDeepMine(spawnInfo)
				? 3f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GiantJellyBall>(),
			SpawnChance = spawnInfo => InDeepMine(spawnInfo)
				? 8f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CarapaceCaterpillar>(),
			SpawnChance = spawnInfo => InDeepMine(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 2f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<RockElemental>(),
			SpawnChance = spawnInfo => InDeepMine(spawnInfo) && !NPC.AnyNPCs(ModContent.NPCType<RockElemental>())
				? 0.05f : 0f,
		});

		// 球冻温床：不混入森林和矿洞的其他候选，以大小球冻为主。
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<JellyBall>(),
			SpawnChance = spawnInfo => InJellyBallHotbed(spawnInfo)
				? 12f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GiantJellyBall>(),
			SpawnChance = spawnInfo => InJellyBallHotbed(spawnInfo)
				? 6f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CyanOreBeetle>(),
			SpawnChance = spawnInfo => InJellyBallHotbed(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CarapaceCaterpillar>(),
			SpawnChance = spawnInfo => InJellyBallHotbed(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});
	}
}
