using Everglow.Commons.Mechanics.Quest.PlayerSide.Tests.ExampleNPC;
using Everglow.Commons.Mechanics.BiomesText;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Utilities;
using Everglow.Example.Quest;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

public class TestNewObj : ExampleWorldQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldCollectItemObjective(ItemID.Ichor, 10))
			.Add(new WorldGiveObjective(ModContent.NPCType<ExamplePerson>(), ItemID.Ichor, 10))
			.Add(new WorldReachObjective((p) => p.InVanillaBiome(VanillaBiomes.Desert)))
			.Add(new WorldTalkObjective(ModContent.NPCType<ExamplePerson>()));
	}
}
