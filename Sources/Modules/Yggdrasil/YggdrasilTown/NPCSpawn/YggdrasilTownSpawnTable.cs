using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using static Everglow.Yggdrasil.YggdrasilTown.NPCSpawn.YggdrasilTownSpawnConditions;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCSpawn;

/// <summary>按环境和昼夜组织名单；权重为初始调参值，不是策划指定的百分比。</summary>
public static class YggdrasilTownSpawnTable
{
	public static void AddRules(List<SubworldSpawnRule> rules)
	{
		// 森林白昼：两片森林共享普通生物。紫色炮弹虫和伞尾贼按策划所在的白昼章节配置。
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<BarkSpicyCaterpillar>(),
			SpawnChance = spawnInfo => Main.dayTime && InForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 2f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LeafcutterAnt>(),
			SpawnChance = spawnInfo => Main.dayTime && InForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<PurpleBombCaterpillar>(),
			SpawnChance = spawnInfo => Main.dayTime && InForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 6f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<UmbrellaTailThief>(),
			SpawnChance = spawnInfo => Main.dayTime && InForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 6f : 0f,
		});

		// 灯木白昼：暮光以拟桉叶虫替代灯泡花苞。
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LightBulb>(),
			SpawnChance = spawnInfo => Main.dayTime && InLampWoodForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 3f : 0f,
		});

		// 灯果螟为现有内容，策划未单列；暂保留在灯木白昼，不扩散到矿洞。
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LampFruitBorer>(),
			SpawnChance = spawnInfo => Main.dayTime && InLampWoodForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 2f : 0f,
		});

		// TODO：暮光白昼的拟桉叶虫 Euca Leafy，替代灯泡花苞，初始权重 3；当前分支尚无实现。
		// TODO：旧时幻影（盾、矛、弓、铳、法师），待内容及晶体显化条件接入后添加暮光规则。
		// TODO：晶胞虫与粉色史莱姆，待内容和生成条件明确；不以 NPCID 0 作为占位。

		// 森林夜晚：夜行毛虫最常见，幽灵比飞棍常见，血月教徒无需血月事件。
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<ShadowstrikeCaterpillar>(),
			SpawnChance = spawnInfo => !Main.dayTime && InForest(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 2f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<WhisperingGhost>(),
			SpawnChance = spawnInfo => !Main.dayTime && InForest(spawnInfo)
				? 1.5f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<DarkGlimmeringRods>(),
			SpawnChance = spawnInfo => !Main.dayTime && InForest(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CrimsonSpell>(),
			SpawnChance = spawnInfo => !Main.dayTime && InForest(spawnInfo) && !NPC.AnyNPCs(ModContent.NPCType<CrimsonSpell>())
				? 0.05f : 0f,
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
