using System.Collections;
using System.Reflection;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.UI;
using Everglow.Commons.Mechanics.Quest.UI.UIElements;
using Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;
using Everglow.Commons.UI;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class UIQuestObjectiveRetryTest
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
		var fonts = (IDictionary)typeof(FontManager)
			.GetField("_fonts", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(_fontManager)!;
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
	public void RetryableTimerHoverAndLeftClickOfferRetry()
	{
		var objective = new ObjectiveView
		{
			Id = 7,
			State = ObjectiveViewState.TimedOut,
			Timer = new TimerView { TimeLimit = 60, ElapsedTime = 60 },
			CanRetry = true,
		};
		var line = new ObjectiveLineView(objective, "Timed objective");
		int? receivedObjectiveId = null;
		var item = new UIQuestObjectiveItem(
			line,
			30f,
			400f,
			objectiveId => receivedObjectiveId = objectiveId);
		var timer = (UIQuestHourglassTimer)typeof(UIQuestObjectiveItem)
			.GetField("_timer", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(item)!;

		timer.Events.MouseHover(timer);
		timer.Events.LeftClick(timer);

		Assert.AreEqual("重试", _questContainer.MouseText);
		Assert.IsTrue(timer.OnSelect);
		Assert.AreEqual(7, receivedObjectiveId);
	}

	[TestMethod]
	public void NonRetryableTimerHoverAndLeftClickDoNotOfferRetry()
	{
		var objective = new ObjectiveView
		{
			Id = 7,
			State = ObjectiveViewState.TimedOut,
			Timer = new TimerView { TimeLimit = 60, ElapsedTime = 60 },
		};
		var line = new ObjectiveLineView(objective, "Timed objective");
		int retryCalls = 0;
		var item = new UIQuestObjectiveItem(
			line,
			30f,
			400f,
			_ => retryCalls++);
		var timer = (UIQuestHourglassTimer)typeof(UIQuestObjectiveItem)
			.GetField("_timer", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(item)!;

		timer.Events.MouseHover(timer);
		timer.Events.LeftClick(timer);

		Assert.AreEqual(string.Empty, _questContainer.MouseText);
		Assert.IsTrue(timer.OnSelect);
		Assert.AreEqual(0, retryCalls);
	}

	[TestMethod]
	public void ReusedItemLeftClickUsesLatestObjectiveId()
	{
		var originalLine = new ObjectiveLineView(
			new ObjectiveView
			{
				Id = 3,
				State = ObjectiveViewState.TimedOut,
				Timer = new TimerView { TimeLimit = 60, ElapsedTime = 60 },
				CanRetry = true,
			},
			"Original objective");
		var latestLine = new ObjectiveLineView(
			new ObjectiveView
			{
				Id = 9,
				State = ObjectiveViewState.TimedOut,
				Timer = new TimerView { TimeLimit = 60, ElapsedTime = 60 },
				CanRetry = true,
			},
			"Latest objective");
		int? receivedObjectiveId = null;
		var item = new UIQuestObjectiveItem(
			originalLine,
			30f,
			400f,
			objectiveId => receivedObjectiveId = objectiveId);
		item.SetLine(latestLine);
		var timer = (UIQuestHourglassTimer)typeof(UIQuestObjectiveItem)
			.GetField("_timer", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(item)!;

		timer.Events.LeftClick(timer);

		Assert.AreEqual(9, receivedObjectiveId);
	}

	[TestMethod]
	public void ReusedItemStopsOfferingRetryWhenLatestLineCannotRetry()
	{
		var originalLine = new ObjectiveLineView(
			new ObjectiveView
			{
				Id = 3,
				State = ObjectiveViewState.TimedOut,
				Timer = new TimerView { TimeLimit = 60, ElapsedTime = 60 },
				CanRetry = true,
			},
			"Original objective");
		var latestLine = new ObjectiveLineView(
			new ObjectiveView
			{
				Id = 9,
				State = ObjectiveViewState.TimedOut,
				Timer = new TimerView { TimeLimit = 60, ElapsedTime = 60 },
			},
			"Latest objective");
		int retryCalls = 0;
		var item = new UIQuestObjectiveItem(
			originalLine,
			30f,
			400f,
			_ => retryCalls++);
		item.SetLine(latestLine);
		var timer = (UIQuestHourglassTimer)typeof(UIQuestObjectiveItem)
			.GetField("_timer", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(item)!;

		timer.Events.MouseHover(timer);
		timer.Events.LeftClick(timer);

		Assert.AreEqual(string.Empty, _questContainer.MouseText);
		Assert.IsTrue(timer.OnSelect);
		Assert.AreEqual(0, retryCalls);
	}
}
