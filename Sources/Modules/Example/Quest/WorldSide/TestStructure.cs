using Everglow.Commons.Mechanics.BiomesText;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

public class TestStructure : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives
			.AddOptional(
				new WorldReachObjective(p => p.InVanillaBiome(VanillaBiomes.Dungeon), "到达地牢"),
				new WorldKillNPCObjective(NPCID.BlueSlime, 5))
			.AddParallel(
				new WorldGiveObjective(NPCID.Merchant, ItemID.DirtBlock, 100),
				new WorldKillNPCObjective(NPCID.ZombieDoctor, 5))
			.AddBranch(
				[new WorldKillNPCObjective(NPCID.BloodZombie, 10)],
				[new WorldTalkObjective(NPCID.Angler), new WorldReachObjective(p => p.InVanillaBiome(VanillaBiomes.Jungle), "到达丛林")],
				[new WorldReachObjective(p => p.InVanillaBiome(VanillaBiomes.Cavern), "到达洞穴层")]);
	}
}
