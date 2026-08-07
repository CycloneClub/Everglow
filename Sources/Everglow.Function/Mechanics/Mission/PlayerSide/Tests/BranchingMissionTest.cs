using Everglow.Commons.Mechanics.Mission.Core;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Tests;

public class BranchingMissionTest : PlayerMissionBase
{
	public BranchingMissionTest()
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

		Icon.AddRange(objective1.NPCTypes.Select(i => NPCMissionIcon.Create(i)));
		Icon.AddRange(objective2_1.ItemTypes.Select(i => ItemMissionIcon.Create(i)));
	}

	public override string DisplayName => nameof(BranchingMissionTest);

	public override MissionType Type => MissionType.Daily;
}
