using System.Reflection;
using Everglow.Commons.UI.StringDrawerSystem;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function.UI;

[TestClass]
public class StringDrawerTest
{
	[TestMethod]
	public void InitEmptyStringClearsCachedContentAndLayout()
	{
		var stringDrawer = CreateWithCachedState();

		stringDrawer.Init(string.Empty);

		AssertEmpty(stringDrawer);
	}

	[TestMethod]
	public void InitNullClearsCachedContentAndLayout()
	{
		var stringDrawer = CreateWithCachedState();

		stringDrawer.Init(null!);

		AssertEmpty(stringDrawer);
	}

	[TestMethod]
	public void ConsecutiveEmptyInitializationsRemainEmpty()
	{
		var stringDrawer = CreateWithCachedState();

		stringDrawer.Init(string.Empty);
		stringDrawer.Init(string.Empty);

		AssertEmpty(stringDrawer);
	}

	[TestMethod]
	public void EmptyInitializationPreservesDefaultParameters()
	{
		var stringDrawer = CreateWithCachedState();
		StringParameters defaultParameters = stringDrawer.DefaultParameters;
		defaultParameters["Color"] = "1,2,3,4";

		stringDrawer.Init(string.Empty);

		Assert.AreSame(defaultParameters, stringDrawer.DefaultParameters);
		Assert.AreEqual("1,2,3,4", stringDrawer.DefaultParameters["Color"]);
	}

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

	private static StringDrawer CreateWithCachedState()
	{
		var stringDrawer = new StringDrawer();
		GetPrivateField<List<DrawerItem>>(stringDrawer, "drawerItems").Add(null!);
		GetPrivateField<List<Vector2>>(stringDrawer, "lineSize").Add(new Vector2(20f, 10f));
		GetPrivateFieldInfo("size").SetValue(stringDrawer, new Vector2(20f, 10f));
		return stringDrawer;
	}

	private static T GetPrivateField<T>(StringDrawer stringDrawer, string name)
	{
		return (T)GetPrivateFieldInfo(name).GetValue(stringDrawer)!;
	}

	private static FieldInfo GetPrivateFieldInfo(string name)
	{
		return typeof(StringDrawer).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
	}

	private static void AssertEmpty(StringDrawer stringDrawer)
	{
		Assert.AreEqual(0, stringDrawer.DrawerItems.Count);
		Assert.AreEqual(0, stringDrawer.Line);
		Assert.AreEqual(Vector2.Zero, stringDrawer.Size);
	}
}
