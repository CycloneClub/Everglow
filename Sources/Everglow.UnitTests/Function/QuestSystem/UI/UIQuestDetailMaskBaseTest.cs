using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class UIQuestDetailMaskBaseTest
{
	private bool _originalDedServ;

	private sealed class TestMask : UIQuestDetailMaskBase<TestMask>
	{
	}

	private sealed class TestContent : UIQuestDetailMaskContentBase<TestMask>
	{
	}

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalDedServ = Terraria.Main.dedServ;
		Terraria.Main.dedServ = true;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Terraria.Main.dedServ = _originalDedServ;
	}

	[TestMethod]
	public void HideCurrent_RemovesContentWithoutThrowing()
	{
		var mask = CreateMask();
		var content = new TestContent();
		content.SetQuest(new QuestView());
		mask.Show(content);

		mask.HideCurrent();

		Assert.IsFalse(mask.Info.IsVisible);
		Assert.IsNull(content.ParentElement);
	}

	[TestMethod]
	public void Calculation_DoesNotRemoveShownContent()
	{
		var mask = CreateMask();
		var content = new TestContent();
		content.SetQuest(new QuestView());
		mask.Show(content);

		mask.Calculation();

		Assert.IsTrue(mask.Info.IsVisible);
		Assert.IsNotNull(content.ParentElement);
	}

	[TestMethod]
	public void Show_ReplacesExistingContentWithoutThrowing()
	{
		var quest = new QuestView();
		var mask = CreateMask();
		var first = new TestContent();
		var second = new TestContent();
		first.SetQuest(quest);
		second.SetQuest(quest);

		mask.Show(first);
		mask.Show(second);

		Assert.IsNull(first.ParentElement);
		Assert.IsNotNull(second.ParentElement);
		Assert.IsTrue(mask.Info.IsVisible);
	}

	private static TestMask CreateMask()
	{
		var mask = new TestMask();
		mask.OnInitialization();
		return mask;
	}
}
