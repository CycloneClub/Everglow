using System.Reflection;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.TextDrawers;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function.UI;

[TestClass]
public class TextDrawerTest
{
	[TestInitialize]
	public void Initialize()
	{
		string fontAssemblyPath = Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..", "Everglow", "lib", "FontStashSharp.FNA.dll"));
		Assembly.LoadFrom(fontAssemblyPath);
	}

	[TestMethod]
	public void WordWrap_KeepsFittingChineseSuffixTogether()
	{
		CollectionAssert.AreEqual(
			new[] { "和Guard_of", "_Yggdrasil对话" },
			Wrap("和Guard_of_Yggdrasil对话", 14.5f));
	}

	[TestMethod]
	public void WordWrap_FitsCharactersExactlyAtWidthLimit()
	{
		CollectionAssert.AreEqual(new[] { "abcd", "ef" }, Wrap("abcdef", 4f));
	}

	[TestMethod]
	public void WordWrap_DoesNotCarrySeparatorSpaceOntoNextLine()
	{
		CollectionAssert.AreEqual(new[] { "hello", "world" }, Wrap("hello world", 8f));
	}

	[TestMethod]
	[DataRow("hello    world", 5f, "hello|world")]
	[DataRow("  hello world", 9f, "  hello|world")]
	[DataRow("hello,world", 8f, "hello|,world")]
	public void WordWrap_SkipsOnlyAutomaticLineSeparatorWhitespace(string text, float width, string expected)
	{
		Assert.AreEqual(expected, string.Join("|", Wrap(text, width)));
	}

	private static string[] Wrap(string text, float width)
	{
		var drawer = new FixedWidthTextDrawer { Text = text };
		List<DrawerItem> items = [drawer];
		int index = 0;
		int line = 0;

		drawer.WordWrap(ref index, items, ref line, width, width);

		return items.Cast<TextDrawer>().Select(item => item.Text).ToArray();
	}

	private sealed class FixedWidthTextDrawer : TextDrawer
	{
		public FixedWidthTextDrawer()
		{
		}

		protected override Vector2 GetTextSize(string text) => new(text.Length, 1f);
	}
}
