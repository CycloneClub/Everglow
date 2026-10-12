using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class WorldKillNPCObjectiveTest
{
	[TestMethod]
	public void Constructor_Should_ThrowInvalidDataException_When_RequirementIsInvalid()
	{
		Assert.ThrowsExactly<InvalidDataException>(() =>
		{
			new WorldKillNPCObjective([], 1);
		});

		Assert.ThrowsExactly<InvalidDataException>(() =>
		{
			new WorldKillNPCObjective([NPCID.BlueSlime], 0);
		});
	}

	[TestMethod]
	public void SingleTypeConstructor_ExposesTypeInNpcTypesList()
	{
		var objective = new WorldKillNPCObjective(NPCID.BlueSlime, 5);

		Assert.HasCount(1, objective.NPCTypes);
		Assert.AreEqual(NPCID.BlueSlime, objective.NPCTypes[0]);
		Assert.AreEqual(5, objective.NPCCount);
	}

	[TestMethod]
	public void CountKill_Should_CountAnyListedNpcTypeTowardSharedRequirement()
	{
		var objective = new WorldKillNPCObjective([NPCID.BlueSlime, NPCID.Zombie], 3);

		objective.CountKill(new NPC { netID = NPCID.BlueSlime });
		objective.CountKill(new NPC { netID = NPCID.Zombie });
		objective.CountKill(new NPC { netID = NPCID.Guide });

		Assert.AreEqual(2, objective.KilledCount);
		Assert.AreEqual(2 / 3f, objective.Progress);
		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void CountKill_Should_BeCappedAtRequirement()
	{
		var objective = new WorldKillNPCObjective([NPCID.BlueSlime, NPCID.Zombie], 2);

		objective.CountKill(new NPC { netID = NPCID.BlueSlime });
		objective.CountKill(new NPC { netID = NPCID.Zombie });
		objective.CountKill(new NPC { netID = NPCID.BlueSlime });

		Assert.AreEqual(2, objective.KilledCount);
		Assert.AreEqual(1f, objective.Progress);
		Assert.IsTrue(objective.CheckCompletion());
	}
}
