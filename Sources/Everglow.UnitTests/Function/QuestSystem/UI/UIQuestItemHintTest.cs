using System.Collections;
using System.Reflection;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.UI;
using Everglow.Commons.Mechanics.Quest.UI.UIElements;
using Everglow.Commons.UI;
using Everglow.Commons.UI.UIElements;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class UIQuestItemHintTest
{
	private static readonly FieldInfo UISystemInstanceField = typeof(UISystem).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic)!;
	private bool _originalDedServ;
	private UISystem _originalUISystem = null!;
	private QuestContainer _container = null!;
	private FontManager _fontManager = null!;

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalDedServ = Terraria.Main.dedServ;
		Terraria.Main.dedServ = true;
		_originalUISystem = UISystem.Instance;
		_ = new UISystem();
		_container = new QuestContainer();
		UISystem.EverglowUISystem.Elements.Add(typeof(QuestContainer).FullName!, _container);

		string fontAssemblyPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
			"..", "..", "..", "..", "Everglow", "lib", "FontStashSharp.FNA.dll"));
		Assembly fontAssembly = Assembly.LoadFrom(fontAssemblyPath);
		_fontManager = new FontManager();
		Type fontSystemType = fontAssembly.GetType("FontStashSharp.FontSystem")!;
		object fontSystem = Activator.CreateInstance(fontSystemType)!;
		string fontPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
			"..", "..", "..", "..", "Everglow.Function", "UI", "Fonts", "FusionPixel12.ttf"));
		fontSystemType.GetMethod("AddFont", [typeof(byte[])])!.Invoke(fontSystem, [File.ReadAllBytes(fontPath)]);
		var fonts = (IDictionary)typeof(FontManager).GetField("_fonts", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(_fontManager)!;
		fonts.Add("FusionPixel12", fontSystem);
	}

	[TestCleanup]
	public void Cleanup()
	{
		_container?.Unload();
		_fontManager?.Unload();
		UISystemInstanceField.SetValue(null, _originalUISystem);
		Terraria.Main.dedServ = _originalDedServ;
	}

	[TestMethod]
	public void UpdateEntry_RefreshesDisplayedNameWithoutResize()
	{
		var identity = new QuestIdentity(QuestSide.Player, "quest", "instance");
		var item = new UIQuestItem(new QuestPresentationEntry(
			new QuestView { Identity = identity, State = QuestViewState.Active, DisplayName = "Original title" }, []));

		item.UpdateEntry(new QuestPresentationEntry(
			new QuestView { Identity = identity, State = QuestViewState.Active, DisplayName = "Updated title" }, []));

		Assert.AreEqual("Updated title", GetName(item).Text);
		Assert.AreEqual(identity, item.View.Identity);
	}

	private static UITextPlus GetName(UIQuestItem item) => (UITextPlus)typeof(UIQuestItem)
		.GetField("name", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(item)!;
}
