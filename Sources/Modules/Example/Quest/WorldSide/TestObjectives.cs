using Everglow.Commons.Mechanics.Quest.PlayerSide.Tests.ExampleNPC;
using Everglow.Commons.Mechanics.BiomesText;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Utilities;
using Everglow.Example.Quest;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

public class TestCollectItem : ExampleWorldQuest
{
	public override void Initialize()
	{
		Objectives.Add(new WorldCollectItemObjective(ItemID.BoneArrow, 100));
		Objectives.Add(new WorldCollectItemObjective(ItemID.Bomb, 100));
	}
}

public class TestConsumeItem : ExampleWorldQuest
{
	public override void Initialize()
	{
		Objectives.Add(new WorldConsumeItemObjective(ItemID.WoodenArrow, 10));
		Objectives.Add(new WorldConsumeItemObjective(ItemID.FeatherfallPotion, 5));
	}
}

public class TestExplore : ExampleWorldQuest
{
	public override void Initialize()
	{
		Objectives.Add(new WorldExploreObjective(500, p => p.InVanillaBiome(VanillaBiomes.Jungle)));
	}
}

public class TestGive : ExampleWorldQuest
{
	public override void Initialize()
	{
		Objectives.Add(new WorldGiveObjective(ModContent.NPCType<ExamplePerson>(), ItemID.DirtBlock, 10));
	}
}

public class TestKillNPC : ExampleWorldQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldKillNPCObjective(NPCID.DemonEye, 10))
			.Add(new WorldKillNPCObjective(NPCID.Zombie, 10));
	}
}

public class TestReach : ExampleWorldQuest
{
	public override void Initialize()
	{
		Objectives.Add(new WorldReachObjective((p) => p.InVanillaBiome(VanillaBiomes.Desert)));
	}
}

public class TestTalkNPC : ExampleWorldQuest
{
	public override void Initialize()
	{
		Objectives.Add(new WorldTalkObjective(ModContent.NPCType<ExamplePerson>()));
		Objectives.Add(new WorldTalkObjective(ModContent.NPCType<ExamplePerson>()));
	}
}
