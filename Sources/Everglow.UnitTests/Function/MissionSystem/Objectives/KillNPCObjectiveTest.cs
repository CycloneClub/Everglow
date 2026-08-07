using Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.MissionSystem.ObjectiveTests;

[TestClass]
public class KillNPCObjectiveTest
{
	[TestMethod]
	public void IndividualCounter_Should_BeCappedAtRequirement()
	{
		for (int reqCount = 1; reqCount < 30; reqCount++)
		{
			var objective = new KillNPCObjective(
				[
					NPCID.BlueSlime,
					NPCID.IceSlime,
					NPCID.SpikedJungleSlime,
					NPCID.MotherSlime,
				], reqCount, true);

			for (int i = 0; i < reqCount * 10; i++)
			{
				var npc = new NPC();
				npc.type = NPCID.BlueSlime;
				objective.CountKill(npc);
			}
			Assert.IsTrue(objective.KilledCount == reqCount);
		}
	}

	[TestMethod]
	public void IndividualProgress_Should_CalculateProperly()
	{
		for (int reqCount = 1; reqCount < 30; reqCount++)
		{
			var killNPCMission = new KillNPCObjective(
				[
					NPCID.BlueSlime,
					NPCID.IceSlime,
					NPCID.SpikedJungleSlime,
					NPCID.MotherSlime,
				], reqCount, true);

			for (int count = 0; count <= reqCount; count++)
			{
				Assert.IsTrue(killNPCMission.Progress == count / (float)reqCount);
				var npc = new NPC();
				npc.type = NPCID.BlueSlime;
				killNPCMission.CountKill(npc);
			}

			Assert.IsTrue(killNPCMission.Progress == 1f);
		}
	}

	[TestMethod]
	public void IndividualCounter_Should_NotCount_When_NPCTypeIsInvalid()
	{
		int reqCount = 10;
		var objective = new KillNPCObjective(
			[
				NPCID.BlueSlime,
				NPCID.IceSlime,
				NPCID.SpikedJungleSlime,
				NPCID.MotherSlime,
			], reqCount, true);

		var npc = new NPC
		{
			type = NPCID.Zombie,
		};
		objective.CountKill(npc);

		npc = new NPC
		{
			type = NPCID.MoonLordCore,
		};
		objective.CountKill(npc);
		Assert.IsTrue(objective.KilledCount == 0);
	}

	[TestMethod]
	public void IndividualCounter_Should_NotThrowInvalidOperationException_When_CountWithoutBeingEnabled()
	{
		int reqCount = 10;
		var killNPCMission = new KillNPCObjective(
				[
					NPCID.BlueSlime,
					NPCID.IceSlime,
					NPCID.SpikedJungleSlime,
					NPCID.MotherSlime,
				], reqCount, false);
		var npc = new NPC();
		npc.type = NPCID.BlueSlime;
		killNPCMission.CountKill(npc);
		Assert.IsTrue(true);
	}

	[TestMethod]
	public void Constructor_Should_ThrowInvalidDataException_When_RequirementIsInvalid()
	{
		Assert.ThrowsExactly<InvalidDataException>(() =>
		{
			new KillNPCObjective([], 1);
		});

		Assert.ThrowsExactly<InvalidDataException>(() =>
		{
			new KillNPCObjective([NPCID.Worm], 0);
		});
	}

	[TestMethod]
	public void LoadData_Should_ReadLegacyRequirementShape()
	{
		var objective = new KillNPCObjective([NPCID.Worm], 10, true);
		var legacyTag = new TagCompound()
		{
			["DemandNPC"] = new TagCompound()
			{
				["Counter"] = 7,
			},
		};

		objective.LoadData(legacyTag);

		Assert.AreEqual(7, objective.KilledCount);
	}
}
