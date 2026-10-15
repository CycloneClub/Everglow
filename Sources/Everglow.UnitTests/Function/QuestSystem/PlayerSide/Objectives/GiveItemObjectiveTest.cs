using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class GiveItemObjectiveTest
{
	[TestMethod]
	public void RemoveItem_Should_TurnFullyConsumedSlotsToAir()
	{
		var objective = new GiveItemObjective([ItemID.DirtBlock], 10, NPCID.Guide);
		Item[] inventory =
		[
			new Item()
			{
				type = ItemID.DirtBlock,
				stack = 4,
			},
			new Item()
			{
				type = ItemID.DirtBlock,
				stack = 6,
			},
			new Item()
			{
				type = ItemID.Wood,
				stack = 5,
			},
		];

		objective.RemoveItem(inventory);

		Assert.AreEqual(ItemID.None, inventory[0].type);
		Assert.AreEqual(0, inventory[0].stack);
		Assert.AreEqual(ItemID.None, inventory[1].type);
		Assert.AreEqual(0, inventory[1].stack);
		Assert.AreEqual(ItemID.Wood, inventory[2].type);
		Assert.AreEqual(5, inventory[2].stack);
	}

	[TestMethod]
	public void RemoveItem_Should_KeepRemainingStack_When_SlotIsOnlyPartiallyConsumed()
	{
		var objective = new GiveItemObjective([ItemID.DirtBlock], 10, NPCID.Guide);
		Item[] inventory =
		[
			new Item()
			{
				type = ItemID.DirtBlock,
				stack = 15,
			},
		];

		objective.RemoveItem(inventory);

		Assert.AreEqual(ItemID.DirtBlock, inventory[0].type);
		Assert.AreEqual(5, inventory[0].stack);
	}
}
