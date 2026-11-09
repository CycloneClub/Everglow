using Everglow.Commons.Mechanics.Quest.PlayerSide.Tests.ExampleNPC;
using Everglow.Commons.Mechanics.BiomesText;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Utilities;
using Everglow.Example.Quest;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

public class TestStructure : ExampleWorldQuest
{
	public override void Initialize()
	{
		Objectives
			.AddOptional(
				new WorldReachObjective(p => p.InVanillaBiome(VanillaBiomes.Dungeon)),
				new WorldKillNPCObjective(NPCID.BlueSlime, 5))
			.AddParallel(
				new WorldGiveObjective(ModContent.NPCType<ExamplePerson>(), ItemID.DirtBlock, 100),
				new WorldKillNPCObjective(NPCID.ZombieDoctor, 5))
			.AddBranch(
				[new WorldKillNPCObjective(NPCID.BloodZombie, 10)],
				[new WorldTalkObjective(ModContent.NPCType<ExamplePerson>()), new WorldReachObjective(p => p.InVanillaBiome(VanillaBiomes.Jungle))],
				[new WorldReachObjective(p => p.InVanillaBiome(VanillaBiomes.Cavern))]);
	}
}
