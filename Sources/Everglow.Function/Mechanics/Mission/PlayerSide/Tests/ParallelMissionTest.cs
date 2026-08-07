using Everglow.Commons.Mechanics.Mission.Core;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Tests;

public class ParallelMissionTest : PlayerMissionBase
{
	public ParallelMissionTest()
	{
		var objective1_1 = new KillNPCObjective(
			[NPCID.BlueArmoredBonesMace, NPCID.HellArmoredBonesMace], 10, true);
		var objective1_2 = new ConsumeItemObjective([ItemID.LifeCrystal], 2);
		var objective1_3 = new KillNPCObjective([NPCID.DemonEye,], 3, true);

		Objectives.AddParallel(objective1_1, objective1_2, objective1_3);

		var objective2 = new KillNPCObjective([NPCID.ChaosBallTim], 3, true);
		Objectives.Add(objective2);

		Icon.AddRange(objective1_1.NPCTypes.Select(i => NPCMissionIcon.Create(i)));
		Icon.AddRange(objective1_2.ItemTypes.Select(i => ItemMissionIcon.Create(i)));
		Icon.AddRange(objective2.NPCTypes.Select(i => NPCMissionIcon.Create(i)));
	}

	public override string DisplayName => nameof(ParallelMissionTest);

	public override MissionType Type => MissionType.Legendary;
}
