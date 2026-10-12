using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.UI;
using Everglow.Commons.Mechanics.Quest.UI.UIElements;
using Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;
using Everglow.Commons.UI;
using Everglow.Commons.UI.UIElements;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.TextDrawers;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class UIQuestItemHintTest
{
	private static readonly FieldInfo UISystemInstanceField = typeof(UISystem).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic)!;
	private static readonly FieldInfo DrawerLoaderInstanceField = typeof(DrawerItemLoader).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic)!;
	private object? _originalDrawerLoader;
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

		string fontAssemblyPath = Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..", "Everglow", "lib", "FontStashSharp.FNA.dll"));
		Assembly fontAssembly = Assembly.LoadFrom(fontAssemblyPath);
		_fontManager = new FontManager();
		Type fontSystemType = fontAssembly.GetType("FontStashSharp.FontSystem")!;
		object fontSystem = Activator.CreateInstance(fontSystemType)!;
		string fontPath = Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..", "Everglow.Function", "UI", "Fonts", "FusionPixel12.ttf"));
		fontSystemType.GetMethod("AddFont", [typeof(byte[])])!.Invoke(fontSystem, [File.ReadAllBytes(fontPath)]);
		var fonts = (IDictionary)typeof(FontManager).GetField("_fonts", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(_fontManager)!;
		fonts.Add("FusionPixel12", fontSystem);

		// Avoid scanning unrelated tML-dependent drawer types in this text-only UI test.
		_originalDrawerLoader = DrawerLoaderInstanceField.GetValue(null);
		var loader = (DrawerItemLoader)RuntimeHelpers.GetUninitializedObject(typeof(DrawerItemLoader));
		typeof(DrawerItemLoader).GetField("drawers", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(loader, new List<DrawerItem> { new TextDrawer() });
		DrawerLoaderInstanceField.SetValue(null, loader);
	}

	[TestCleanup]
	public void Cleanup()
	{
		DrawerLoaderInstanceField.SetValue(null, _originalDrawerLoader);
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

	[TestMethod]
	[DataRow(QuestSide.Player, QuestViewState.Available)]
	[DataRow(QuestSide.World, QuestViewState.Locked)]
	public void AcceptanceOrUnlock_RevealsListNameAndRemovesHintRoot(QuestSide side, QuestViewState initialState)
	{
		var identity = new QuestIdentity(side, "quest", "instance");
		var initial = new QuestView
		{
			Identity = identity,
			State = initialState,
			DisplayName = "Revealed name",
			Hint = string.Empty,
			HideMode = QuestHideMode.NameAndConditions,
		};
		var item = new UIQuestItem(new QuestPresentationEntry(initial, []));
		var hint = new UIQuestHint();
		hint.SetEntry(new QuestPresentationEntry(initial, []));

		Assert.AreEqual("???", GetName(item).Text);
		Assert.IsTrue(hint.IsVisible);

		var active = new QuestView
		{
			Identity = identity,
			State = QuestViewState.Active,
			DisplayName = "Revealed name",
			Hint = "Still retains condition text",
			HideMode = QuestHideMode.NameAndConditions,
		};
		item.UpdateEntry(new QuestPresentationEntry(active, []));
		hint.SetEntry(new QuestPresentationEntry(active, []));

		Assert.AreEqual("Revealed name", GetName(item).Text);
		Assert.IsFalse(hint.IsVisible);
		Assert.AreEqual(identity, item.View.Identity);
	}

	[TestMethod]
	public void Hint_RefreshesAcceptAvailabilityWithoutChangingQuestState()
	{
		var identity = new QuestIdentity(QuestSide.Player, "quest", "instance");
		var view = new QuestView
		{
			Identity = identity,
			State = QuestViewState.Available,
			Description = "Quest description",
		};
		var hint = new UIQuestHint();
		hint.Info.Width.SetValue(800);
		hint.Info.Height.SetValue(600);
		hint.OnInitialization();
		hint.SetEntry(new QuestPresentationEntry(view, [new QuestAction(identity, QuestActionType.Accept)]));
		var button = hint.ChildrenElements.OfType<UIQuestActionButton>().Single();

		Assert.IsTrue(hint.IsVisible);
		Assert.IsTrue(button.IsVisible);
		Assert.IsTrue(button.Info.CanBeInteract);
		Assert.IsFalse(hint.ChildrenElements.OfType<UITextPlus>().Any(text => text.Text == "Quest description"));

		hint.SetEntry(new QuestPresentationEntry(view, []));

		Assert.IsTrue(hint.IsVisible);
		Assert.IsFalse(button.IsVisible);
		Assert.IsFalse(button.Info.CanBeInteract);
	}

	[TestMethod]
	public void ActionButton_UsesLatestEntryAndStopsWhenActionIsRemoved()
	{
		var first = new QuestIdentity(QuestSide.Player, "first", "one");
		var second = new QuestIdentity(QuestSide.Player, "second", "two");
		List<QuestAction> executed = [];
		var button = new UIQuestActionButton(executed.Add);
		button.SetEntry(new QuestPresentationEntry(new QuestView { Identity = first }, [new QuestAction(first, QuestActionType.Accept)]));
		button.SetEntry(new QuestPresentationEntry(new QuestView { Identity = second }, [new QuestAction(second, QuestActionType.Submit)]));
		button.Events.LeftDown(button);
		button.SetEntry(new QuestPresentationEntry(new QuestView { Identity = second }, []));
		button.Events.LeftDown(button);

		CollectionAssert.AreEqual(new[] { new QuestAction(second, QuestActionType.Submit) }, executed);
	}

	private static UITextPlus GetName(UIQuestItem item) => (UITextPlus)typeof(UIQuestItem)
		.GetField("name", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(item)!;
}
