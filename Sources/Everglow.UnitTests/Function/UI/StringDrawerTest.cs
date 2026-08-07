namespace Everglow.UnitTests.Function.UI;

[TestClass]
public class StringDrawerTest
{
	[TestMethod]
	public void TestStringIndexPick()
	{
		var text = "abcd";

		Assert.AreEqual('a', text[0]);
		Assert.AreEqual('b', text[1]);
		Assert.AreEqual('c', text[2]);
		Assert.AreEqual('d', text[3]);
	}

	[TestMethod]
	public void TestStringCut()
	{
		var text = "abc";

		var fullLength = text[0..text.Length];
		var minusOne = text[0..(text.Length - 1)];

		Assert.AreEqual(text, fullLength);

		Assert.AreNotEqual(text, minusOne);
		Assert.AreEqual("ab", minusOne);
	}

	[TestMethod]
	public void TestStringRemove()
	{
		var text = "abc";
		text = text.Remove(1, 1);

		Assert.AreEqual(text, "ac");
	}
}
