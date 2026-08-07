using Everglow.Commons.Mechanics.BiomesText;
using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.WorldSide.Objectives;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Tests;

public class TestStructure : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives
			.AddOptional(
				new WorldReachObjective(p => p.InVanillaBiome(VanillaBiomes.Dungeon)),
				new WorldKillNPCObjective(NPCID.BlueSlime, 5))
			.AddParallel(
				new WorldGiveObjective(NPCID.Merchant, ItemID.DirtBlock, 100),
				new WorldKillNPCObjective(NPCID.ZombieDoctor, 5))
			.AddBranch(
				[new WorldKillNPCObjective(NPCID.BloodZombie, 10)],
				[new WorldTalkObjective(NPCID.Angler), new WorldReachObjective(p => p.InVanillaBiome(VanillaBiomes.Jungle))],
				[new WorldReachObjective(p => p.InVanillaBiome(VanillaBiomes.Cavern))]);
	}
}
