using Everglow.Commons.Mechanics.BiomesText;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

public class TestCollectItem : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldCollectItemObjective(ItemID.BoneArrow, 100));
		Objectives.Add(new WorldCollectItemObjective(ItemID.Bomb, 100));
	}
}

public class TestConsumeItem : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldConsumeItemObjective(ItemID.WoodenArrow, 10));
		Objectives.Add(new WorldConsumeItemObjective(ItemID.FeatherfallPotion, 5));
	}
}

public class TestExplore : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldExploreObjective(500, p => p.InVanillaBiome(VanillaBiomes.Jungle), "在丛林中探索"));
	}
}

public class TestGive : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldGiveObjective(NPCID.DyeTrader, ItemID.DirtBlock, 10));
	}
}

public class TestKillNPC : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldKillNPCObjective(NPCID.DemonEye, 10))
			.Add(new WorldKillNPCObjective(NPCID.Zombie, 10));
	}
}

public class TestReach : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldReachObjective((p) => p.InVanillaBiome(VanillaBiomes.Desert), "到达沙漠"));
	}
}

public class TestTalkNPC : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldTalkObjective(NPCID.Guide));
		Objectives.Add(new WorldTalkObjective(NPCID.Nurse));
	}
}
