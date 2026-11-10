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
	public void SpecialSuppliesAreRareAndUniform()
	{
		var random = new UnifiedRandom(831);
		int[] counts = new int[2];
		const int Samples = 70000;
		for (int i = 0; i < Samples; i++)
		{
			var reward = Roll(random);
			if (reward is { } drop)
			{
				Assert.IsTrue(drop.RewardIndex is >= 0 and < 2);
				counts[drop.RewardIndex]++;
			}
		}

		int total = counts[0] + counts[1];
		Assert.IsTrue(total is > 0 and < Samples);
		Assert.IsTrue(Math.Abs(counts[0] - counts[1]) < counts[0] * 0.15);
	}

	[TestMethod]
	public void SupplyQuantitiesVary()
	{
		var random = new UnifiedRandom(217);
		HashSet<int> amounts = [];
		for (int i = 0; i < 10000; i++)
		{
			if (Roll(random) is { } drop)
			{
				Assert.IsTrue(drop.Stack > 0);
				amounts.Add(drop.Stack);
			}
		}

		Assert.IsTrue(amounts.Count > 1);
	}
}
