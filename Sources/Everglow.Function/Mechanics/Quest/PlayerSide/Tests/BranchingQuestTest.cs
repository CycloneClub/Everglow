using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class BranchingQuestTest : PlayerQuestBase
{
	public BranchingQuestTest()
	{
		var objective1 = new KillNPCObjective(
				[
					NPCID.BlueSlime,
					NPCID.IceSlime,
					NPCID.SpikedJungleSlime,
					NPCID.MotherSlime,
				], 10, true);

		var objective2_1 = new ConsumeItemObjective([ItemID.WoodenArrow], 2);
		var objective2_2 = new KillNPCObjective([NPCID.BlueSlime, NPCID.IceSlime, NPCID.SpikedJungleSlime, NPCID.MotherSlime], 5, true);

		var objective3 = new KillNPCObjective(
				[
					NPCID.EyeofCthulhu,
				], 2, true);

		Objectives.Add(objective1).AddBranch([objective2_1, objective2_2], [objective3]);
	}

	public override string DisplayName => nameof(BranchingQuestTest);

	public override QuestType Type => QuestType.Daily;
}
