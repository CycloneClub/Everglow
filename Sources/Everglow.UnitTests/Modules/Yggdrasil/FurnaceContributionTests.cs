using System.Reflection;
using Everglow.Yggdrasil.YggdrasilTown;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class FurnaceContributionTests
{
	private static readonly Func<int, int, int, int> Calculate = typeof(YggdrasilTownFurnaceSystem)
		.GetMethod("CalculateContribution", BindingFlags.Static | BindingFlags.NonPublic)!
		.CreateDelegate<Func<int, int, int, int>>();
	private static readonly Func<int, int, int> Add = typeof(YggdrasilTownFurnaceSystem)
		.GetMethod("AddContribution", BindingFlags.Static | BindingFlags.NonPublic)!
		.CreateDelegate<Func<int, int, int>>();

	[TestMethod]
	[DataRow(0, -1, 100)]
	[DataRow(10000, -11, 1)]
	[DataRow(-1, 0, 1)]
	[DataRow(100, 0, -1)]
	public void InvalidOrNonValuableInputsNeverSubtractContribution(int value, int rarity, int stack)
	{
		Assert.AreEqual(0, Calculate(value, rarity, stack));
	}

	[TestMethod]
	public void OrdinaryValuesKeepExistingFormula()
	{
		Assert.AreEqual(100, Calculate(0, 0, 100));
		Assert.AreEqual(125, Calculate(10000, 0, 5));
		Assert.AreEqual(600, Calculate(0, 10, 100));
		Assert.AreEqual(600, Calculate(0, 100, 100));
	}

	[TestMethod]
	public void LargeStacksSaturateInsteadOfWrappingNegative()
	{
		Assert.AreEqual(878630242, Calculate(int.MaxValue, 10, 9999));
		Assert.AreEqual(int.MaxValue, Calculate(int.MaxValue, 10, int.MaxValue));
		Assert.AreEqual(int.MaxValue, Add(int.MaxValue - 1, 10));
	}

	[TestMethod]
	public void AddingContributionPreservesTheBalanceWithinRange()
	{
		Assert.AreEqual(125, Add(100, 25));
		Assert.AreEqual(100, Add(100, 0));
	}
}
