using System.Reflection;
using Everglow.Commons.Mechanics;
using Everglow.Commons.Mechanics.Miscs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function.Mechanics;

[TestClass]
[DoNotParallelize]
public class NPCDifficultyMigrationTest
{
	[NoGameModeScale]
	private sealed class FixedStatsNPC : ModNPC
	{
	}

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
	}

	[TestMethod]
	[DataRow(0.5f)]
	[DataRow(1.5f)]
	[DataRow(2f)]
	[DataRow(3f)]
	public void DifficultyExemption_PreservesBaseStatsAcrossJourneyAndExpert(float difficulty)
	{
		bool oldHardmodeExemption = NPCID.Sets.DontDoHardmodeScaling[NPCID.BlueSlime];
		try
		{
			var npc = new NPC { type = NPCID.BlueSlime, difficulty = difficulty, lifeMax = 1000, damage = 120, value = 100, knockBackResist = 1f };
			typeof(NPC).GetProperty(nameof(NPC.ModNPC))!.SetValue(npc, new FixedStatsNPC());
			On_NPC.orig_ScaleStats_ByDifficulty original = subject => subject.ScaleStats_ByDifficulty();
			typeof(EverglowGlobalNPC).GetMethod("NPC_ScaleStats_ByDifficulty", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [original, npc]);
			Assert.AreEqual(1000, npc.lifeMax);
			Assert.AreEqual(120, npc.damage);
			Assert.AreEqual(100f, npc.value);
			Assert.AreEqual(1f, npc.knockBackResist, 0.0001f);
			Assert.AreEqual(difficulty, npc.difficulty);
		}
		finally
		{
			NPCID.Sets.DontDoHardmodeScaling[NPCID.BlueSlime] = oldHardmodeExemption;
		}
	}
}
