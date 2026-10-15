using Everglow.Commons.Mechanics.BiomesText;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Tests;

public class TestNewObj : WorldQuestBase
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldCollectItemObjective(ItemID.Ichor, 10))
			.Add(new WorldGiveObjective(NPCID.Guide, ItemID.Ichor, 10))
			.Add(new WorldReachObjective((p) => p.InVanillaBiome(VanillaBiomes.Desert), "到达沙漠"))
			.Add(new WorldTalkObjective(NPCID.Nurse));
	}
}
