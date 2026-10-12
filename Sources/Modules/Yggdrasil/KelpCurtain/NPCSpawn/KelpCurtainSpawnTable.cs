using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Yggdrasil.Common.NPCSpawn;
using Everglow.Yggdrasil.KelpCurtain.NPCs;
using Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;
using Everglow.Yggdrasil.KelpCurtain.NPCs.SpinyMossCourt;
using Everglow.Yggdrasil.KelpCurtain.NPCs.ValleyOfLushAndMoist;
using static Everglow.Yggdrasil.KelpCurtain.NPCSpawn.KelpCurtainSpawnConditions;

namespace Everglow.Yggdrasil.KelpCurtain.NPCSpawn;

/// <summary>内容加载后调用；普通规则只声明类型、权重和条件。</summary>
public static class KelpCurtainSpawnTable
{
	public static void AddRules(List<SubworldSpawnRule> rules)
	{
		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AlgaeOctopus>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<ArmoredShrimp>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo),
			MinGroupSize = 2,
			MaxGroupSize = 5,
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<BombJellyfish>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				InShallowWater(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CannonBarnacle>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterBottom(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<FluorescentHydra>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterBottom(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GiantTigerShrimp>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GlowSalamander>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<JadeSpiritAnglerfish>(),
			Weight = 0.35f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterBottom(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LargeAlgaeOctopus>(),
			Weight = 0.35f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterBottom(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LargeBombJellyfish>(),
			Weight = 0.5f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				InDeepWater(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<Radiolarian>(),
			Weight = 0.35f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				NearWaterSurface(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<SailfinSnakehead>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsWater(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<ToxicToad>(),
			Weight = 1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<WaterStrider>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				CanSpawnOnWaterSurface(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GiantDandelion>(),
			Weight = 0.25f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<GuppyConch>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<MossyThornTurtle>(),
			Weight = 1.5f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AnimatedWitherbarkHound>(),
			Weight = 1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AnimatedWitherbarkSoldier>(),
			Weight = 1.5f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AnimatedWitherbarkSoldierRanged>(),
			Weight = 1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AnimatedWitherbarkSoldierSpell>(),
			Weight = 0.75f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<BrodieFlydragon>(),
			Weight = 1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<CourtCommander>(),
			Weight = 0.5f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<SmallBrodieFlydragon>(),
			Weight = 0.5f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<AssassinRaspberry>(),
			Weight = 1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<LargeMossyThornTurtle>(),
			Weight = 0.1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo) &&
				!NPC.AnyNPCs(ModContent.NPCType<LargeMossyThornTurtle>()),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<RedNeedleCaterpillar>(),
			Weight = 1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<SerpentMoss>(),
			Weight = 1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<SmallGuppyConch>(),
			Weight = 1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo) && SpawnConditions.OnGround(spawnInfo),
		});

		rules.Add(new SubworldSpawnRule
		{
			Type = ModContent.NPCType<VerdantRods>(),
			Weight = 1f,
			Condition = spawnInfo => InKelpCurtain(spawnInfo) &&
				SpawnConditions.IsDry(spawnInfo),
		});
	}
}
