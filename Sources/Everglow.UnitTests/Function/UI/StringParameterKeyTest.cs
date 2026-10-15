using Everglow.Commons.UI.StringDrawerSystem;

namespace Everglow.UnitTests.Function.UI;

[TestClass]
public class StringParameterKeyTest
{
	[TestMethod]
	public void SameValuesAreEqual()
	{
		var key = "key";
		var keyObj1 = new StringParameterKey(key);
		var keyObj2 = new StringParameterKey(key);

		Assert.AreEqual(keyObj1, keyObj2);
	}

	[TestMethod]
	public void DifferentValuesAreUnequal()
	{
		var key1 = "key1";
		var keyObj1 = new StringParameterKey(key1);

		var key2 = "key2";
		var keyObj2 = new StringParameterKey(key2);

		Assert.AreNotEqual(keyObj1, keyObj2);
	}

	[TestMethod]
	public void OperatorsWorkCorrectly()
	{
		var key1 = "Test1";
		var same1 = new StringParameterKey(key1);
		var same2 = new StringParameterKey(key1);

		var key2 = "Test2";
		var different = new StringParameterKey(key2);

		Assert.IsTrue(same1 == same2);
		Assert.IsTrue(same1 != different);
		Assert.IsFalse(same1 == different);
		Assert.IsFalse(same1 != same2);
	}
}
