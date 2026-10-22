using System.Reflection;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class StoneBridgePotLootTests
{
	private static readonly Func<UnifiedRandom, (int RewardIndex, int Stack)?> Roll = typeof(StoneBridge_Pot)
		.GetMethod("RollSpecialSupplies", BindingFlags.Static | BindingFlags.NonPublic)!
		.CreateDelegate<Func<UnifiedRandom, (int RewardIndex, int Stack)?>>();

	[TestMethod]
	public void OneInSevenPotsHasOneOfTwoUniformSpecialSupplies()
	{
		var random = new UnifiedRandom(831);
		int[] counts = new int[2];
		for (int i = 0; i < 70000; i++)
		{
			var reward = Roll(random);
			if (reward is { } drop)
			{
				Assert.IsTrue(drop.RewardIndex is >= 0 and < 2);
				counts[drop.RewardIndex]++;
			}
		}
		foreach (int count in counts)
		{
			Assert.IsTrue(count is > 4600 and < 5400, $"Expected about 1/14, observed {count}/70000");
		}
	}

	[TestMethod]
	public void SupplyQuantitiesIncludeBothSixteenAndThirtyTwo()
	{
		var random = new UnifiedRandom(217);
		HashSet<int> amounts = [];
		for (int i = 0; i < 10000; i++)
		{
			if (Roll(random) is { } drop)
			{
				Assert.IsTrue(drop.Stack is >= 16 and <= 32);
				amounts.Add(drop.Stack);
			}
		}
		Assert.IsTrue(amounts.Contains(16));
		Assert.IsTrue(amounts.Contains(32));
	}
}
