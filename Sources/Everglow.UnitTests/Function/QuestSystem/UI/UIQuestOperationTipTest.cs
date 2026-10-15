using System.Collections;
using System.Reflection;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.UI;
using Everglow.Commons.Mechanics.Quest.UI.UIElements;
using Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;
using Everglow.Commons.UI;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class UIQuestOperationTipTest
{
	private static readonly FieldInfo UISystemInstanceField = typeof(UISystem).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic)!;

	private bool _originalDedServ;
	private UISystem _originalUISystem;
	private QuestContainer _questContainer;
	private FontManager _fontManager;

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalDedServ = Terraria.Main.dedServ;
		Terraria.Main.dedServ = true;

		_originalUISystem = UISystem.Instance;
		_ = new UISystem();
		_questContainer = new QuestContainer();
		UISystem.EverglowUISystem.Elements.Add(typeof(QuestContainer).FullName!, _questContainer);

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
	}

	[TestCleanup]
	public void Cleanup()
	{
		_questContainer?.Unload();
		_fontManager?.Unload();
		UISystemInstanceField.SetValue(null, _originalUISystem);
		Terraria.Main.dedServ = _originalDedServ;
	}

	[TestMethod]
	public void ConfirmationPassesPresentationEntryToCallback()
	{
		var identity = new QuestIdentity(QuestSide.Player, "TestQuest", "instance");
		var entry = new QuestPresentationEntry(
			new QuestView { Identity = identity },
			[new QuestAction(identity, QuestActionType.Cancel)]);
		QuestPresentationEntry receivedEntry = null;
		var tip = new UIQuestOperationTip(
			entry,
			UIQuestOperationTip.TipType.Confirmation,
			"Confirm",
			value => receivedEntry = value);
		tip.OnInitialization();

		var yesButton = (UIQuestButton)typeof(UIQuestOperationTip)
			.GetField("_yes", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(tip)!;
		yesButton.Events.LeftClick(yesButton);

		Assert.AreSame(entry, receivedEntry);
	}
}
