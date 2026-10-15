using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Terraria;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class PlayerObjectiveResetTest
{
	private sealed class StubBiome : IShoppingBiome
	{
		public string NameKey => nameof(StubBiome);

		public bool IsInBiome(Player player) => false;
	}

	[TestMethod]
	public void KillNPCObjective_ResetProgress_ClearsKilledCount()
	{
		var objective = new KillNPCObjective([NPCID.BlueSlime], 2, true);
		objective.LoadData(new TagCompound { [nameof(KillNPCObjective.KilledCount)] = 2 });

		objective.ResetProgress();

		Assert.AreEqual(0, objective.KilledCount);
		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void CollectItemObjective_ResetProgress_ClearsCollectedCount()
	{
		var objective = new CollectItemObjective([ItemID.DirtBlock], 3, true);
		objective.LoadData(new TagCompound { [nameof(CollectItemObjective.CollectedCount)] = 3 });

		objective.ResetProgress();

		Assert.AreEqual(0, objective.CollectedCount);
		Assert.AreEqual(0f, objective.CalculateProgress(new Player()));
	}

	[TestMethod]
	public void ConsumeItemObjective_ResetProgress_ClearsConsumedCount()
	{
		var objective = new ConsumeItemObjective([ItemID.HealingPotion], 4);
		objective.LoadData(new TagCompound { [nameof(ConsumeItemObjective.ConsumedCount)] = 4 });

		objective.ResetProgress();

		Assert.AreEqual(0, objective.ConsumedCount);
		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void ExploreObjective_ResetProgress_ClearsDistanceMoved()
	{
		var objective = new ExploreObjective(new StubBiome(), 100f);
		objective.LoadData(new TagCompound { ["distanceMoved"] = 100f });

		objective.ResetProgress();

		Assert.IsFalse(objective.CheckCompletion());
	}
}
