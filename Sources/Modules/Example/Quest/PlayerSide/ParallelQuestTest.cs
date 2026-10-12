using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class ParallelQuestTest : PlayerQuestBase
{
	public ParallelQuestTest()
	{
		var objective1_1 = new KillNPCObjective(
			[NPCID.BlueArmoredBonesMace, NPCID.HellArmoredBonesMace], 10, true);
		var objective1_2 = new ConsumeItemObjective([ItemID.LifeCrystal], 2);
		var objective1_3 = new KillNPCObjective([NPCID.DemonEye,], 3, true);

		Objectives.AddParallel(objective1_1, objective1_2, objective1_3);

		var objective2 = new KillNPCObjective([NPCID.ChaosBallTim], 3, true);
		Objectives.Add(objective2);
	}

	public override string DisplayName => nameof(ParallelQuestTest);

	public override QuestType Type => QuestType.Legend;
}
