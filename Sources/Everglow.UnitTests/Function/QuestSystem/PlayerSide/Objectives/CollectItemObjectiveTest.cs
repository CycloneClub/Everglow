using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class CollectItemObjectiveTest
{
	[TestMethod]
	public void Progress_Should_CalculateCorrectly_When_TypeIsSingle()
	{
		var random = new Random();
		for (int i = 0; i < 100; i++)
		{
			int testStack = random.Next(30, 120);
			var objective = new CollectItemObjective([ItemID.DirtBlock], testStack, false);

			var player = new Player();
			player.inventory = [];

			Assert.IsTrue(objective.CalculateProgress(player) == 0f);

			for (int stack = 0; stack <= testStack; stack++)
			{
				player.inventory =
				[
					new Item()
					{
						type = ItemID.DirtBlock,
						stack = stack,
					}
				];

				var targetProgress = stack / (float)testStack;
				var actualProgress = objective.CalculateProgress(player);
				Assert.IsTrue(actualProgress == targetProgress);
			}
		}
	}

	[TestMethod]
	public void Progress_Should_CalculateCorrectly_When_TypeIsMultiple()
	{
		for (int i = 0; i < 30; i++)
		{
			var random = new Random();

			int type1 = ItemID.DirtBlock;
			int testStack1 = random.Next(23, 48);

			int type2 = ItemID.Wood;

			int type3 = ItemID.IronOre;

			var quest = new CollectItemObjective([type1, type2, type3], testStack1, false);

			var player = new Player();
			player.inventory = [];

			Assert.IsTrue(quest.CalculateProgress(player) == 0f);

			for (int stack1 = 0; stack1 <= testStack1; stack1++)
			{
				for (int stack2 = 0; stack2 <= testStack1; stack2++)
				{
					for (int stack3 = 0; stack3 <= testStack1; stack3++)
					{
						player.inventory =
						[
							new Item()
							{
								type = type1,
								stack = stack1,
							},
							new Item()
							{
								type = type2,
								stack = stack2,
							},
							new Item()
							{
								type = type3,
								stack = stack3,
							},
						];
						var targetProgress = Math.Clamp((stack1 + stack2 + stack3) / (float)testStack1, 0f, 1f);
						var acturalProgress = quest.CalculateProgress(player);
						Assert.IsTrue(acturalProgress == targetProgress);
					}
				}
			}
		}
	}

	[TestMethod]
	public void Progress_Should_CappedAtOne()
	{
		for (int i = 0; i < 100; i++)
		{
			int testStack = (int)new Random().NextInt64(50, 200);

			var objective = new CollectItemObjective([ItemID.DirtBlock], testStack, false);

			var player = new Player();
			player.inventory = [];

			Assert.IsTrue(objective.CalculateProgress(player) == 0f);

			for (int stack = testStack; stack <= 2 * testStack; stack++)
			{
				player.inventory =
				[
					new Item()
					{
						type = ItemID.DirtBlock,
						stack = stack,
					},
				];

				Assert.IsTrue(objective.CalculateProgress(player) == 1f);
			}
		}
	}

	[TestMethod]
	public void Constructor_Should_ThrowInvalidDataException_When_RequirementIsInvalid()
	{
		Assert.ThrowsExactly<InvalidDataException>(() =>
		{
			new CollectItemObjective([], 1);
		});

		Assert.ThrowsExactly<InvalidDataException>(() =>
		{
			new CollectItemObjective([ItemID.DirtBlock], 0);
		});
	}

	[TestMethod]
	public void LoadData_Should_ReadLegacyCounterShape()
	{
		var objective = new CollectItemObjective([ItemID.DirtBlock], 10);
		var legacyTag = new TagCompound()
		{
			["Counter"] = new TagCompound()
			{
				["Value"] = 7,
			},
		};

		objective.LoadData(legacyTag);

		Assert.AreEqual(7, objective.CollectedCount);
	}

}
