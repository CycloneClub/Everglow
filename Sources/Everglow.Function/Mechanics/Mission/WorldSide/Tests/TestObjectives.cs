using Everglow.Commons.Mechanics.BiomesText;
using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.WorldSide.Objectives;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Tests;

public class TestCollectItem : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldCollectItemObjective(ItemID.BoneArrow, 100));
		Objectives.Add(new WorldCollectItemObjective(ItemID.Bomb, 100));
	}
}

public class TestConsumeItem : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldConsumeItemObjective(ItemID.WoodenArrow, 10));
		Objectives.Add(new WorldConsumeItemObjective(ItemID.FeatherfallPotion, 5));
	}
}

public class TestExplore : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldExploreObjective(500, p => p.InVanillaBiome(VanillaBiomes.Jungle)));
	}
}

public class TestGive : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldGiveObjective(NPCID.DyeTrader, ItemID.DirtBlock, 10));
	}
}

public class TestKillNPC : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldKillNPCObjective(NPCID.DemonEye, 10))
			.Add(new WorldKillNPCObjective(NPCID.Zombie, 10));
	}
}

public class TestReach : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldReachObjective((p) => p.InVanillaBiome(VanillaBiomes.Desert)));
	}
}

public class TestTalkNPC : WorldMissionBase
{
	public override void Initialize()
	{
		Objectives.Add(new WorldTalkObjective(NPCID.Guide));
		Objectives.Add(new WorldTalkObjective(NPCID.Nurse));
	}
}
