using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.Common.NPCSpawn;
using Everglow.Yggdrasil.KelpCurtain.NPCs;
using Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;
using Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt;
using Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist;
using static Everglow.Yggdrasil.KelpCurtain.NPCSpawn.KelpCurtainSpawnConditions;

namespace Everglow.Yggdrasil.KelpCurtain.NPCSpawn;

/// <summary>内容加载后调用；规则按区域维护，由 SpawnChance 计算本次权重。</summary>
public static class KelpCurtainSpawnTable
{
	public static void AddRules(List<SubworldSpawnRule> rules)
	{
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AlgaeOctopus>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<ArmoredShrimp>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo)
				? 0.75f : 0f,
			MinGroupSize = 2,
			MaxGroupSize = 5,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<BombJellyfish>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				InShallowWater(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CannonBarnacle>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterBottom(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<FluorescentHydra>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterBottom(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GiantTigerShrimp>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GlowSalamander>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<JadeSpiritAnglerfish>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterBottom(spawnInfo)
				? 0.35f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LargeAlgaeOctopus>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterBottom(spawnInfo)
				? 0.35f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LargeBombJellyfish>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				InDeepWater(spawnInfo)
				? 0.5f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<Radiolarian>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterSurface(spawnInfo)
				? 0.35f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<SailfinSnakehead>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<ToxicToad>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<WaterStrider>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				CanSpawnOnWaterSurface(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GiantDandelion>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 0.25f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GuppyConch>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<MossyThornTurtle>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1.5f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AnimatedWitherbarkHound>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AnimatedWitherbarkSoldier>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1.5f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AnimatedWitherbarkSoldierRanged>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AnimatedWitherbarkSoldierSpell>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 0.75f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<BrodieFlydragon>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CourtCommander>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 0.5f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<SmallBrodieFlydragon>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo)
				? 0.5f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AssassinRaspberry>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LargeMossyThornTurtle>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo) &&
				!NPC.AnyNPCs(ModContent.NPCType<LargeMossyThornTurtle>())
				? 0.1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<RedNeedleCaterpillar>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<SerpentMoss>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<SmallGuppyConch>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo)
				? 1f : 0f,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<VerdantRods>(),
			SpawnChance = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo)
				? 1f : 0f,
		});
	}
}
