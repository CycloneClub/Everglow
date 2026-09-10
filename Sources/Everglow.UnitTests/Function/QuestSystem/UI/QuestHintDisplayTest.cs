using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.UI;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class QuestHintDisplayTest
{
	[TestMethod]
	[DataRow(QuestSide.Player, QuestViewState.Available)]
	[DataRow(QuestSide.World, QuestViewState.Locked)]
	public void AwaitingAcceptanceOrUnlock_ShowsMaskEvenWithEmptyHint(QuestSide side, QuestViewState state)
	{
		var view = CreateView(side, state, QuestHideMode.None, string.Empty);

		Assert.IsTrue(QuestHintDisplay.IsVisible(view));
		Assert.AreEqual("Real quest", QuestHintDisplay.GetDisplayName(view));
		Assert.AreEqual(string.Empty, QuestHintDisplay.GetHint(view));
	}

	[TestMethod]
	[DataRow(QuestHideMode.None, "Real quest", "Talk to the guide")]
	[DataRow(QuestHideMode.Name, "???", "Talk to the guide")]
	[DataRow(QuestHideMode.NameAndConditions, "???", "???")]
	public void MaskedModes_FormatOnlyUIStringsAndPreserveTheView(QuestHideMode mode, string name, string hint)
	{
		foreach (var (side, state) in new[]
		{
			(QuestSide.Player, QuestViewState.Available),
			(QuestSide.World, QuestViewState.Locked),
		})
		{
			var view = CreateView(side, state, mode, "Talk to the guide");

			Assert.IsTrue(QuestHintDisplay.IsVisible(view));
			Assert.AreEqual(name, QuestHintDisplay.GetDisplayName(view));
			Assert.AreEqual(hint, QuestHintDisplay.GetHint(view));
			Assert.AreEqual("Real quest", view.DisplayName);
			Assert.AreEqual("Talk to the guide", view.Hint);
			Assert.AreEqual("Real description", view.Description);
		}
	}

	[TestMethod]
	[DataRow(QuestSide.Player, QuestViewState.Active)]
	[DataRow(QuestSide.Player, QuestViewState.Completed)]
	[DataRow(QuestSide.Player, QuestViewState.Failed)]
	[DataRow(QuestSide.World, QuestViewState.Active)]
	[DataRow(QuestSide.World, QuestViewState.Completed)]
	[DataRow(QuestSide.World, QuestViewState.Failed)]
	[DataRow(QuestSide.World, QuestViewState.Available)]
	[DataRow(QuestSide.Player, QuestViewState.Locked)]
	public void OtherStates_RestoreRealNameDespiteHiddenModeAndHint(QuestSide side, QuestViewState state)
	{
		var view = CreateView(side, state, QuestHideMode.NameAndConditions, "Secret condition");

		Assert.IsFalse(QuestHintDisplay.IsVisible(view));
		Assert.AreEqual("Real quest", QuestHintDisplay.GetDisplayName(view));
	}

	[TestMethod]
	[DataRow("")]
	[DataRow(" \t")]
	[DataRow("???")]
	[DataRow("[TextDrawer,Text='Condition']")]
	public void HintContents_DoNotChooseTheDisclosureMode(string hint)
	{
		var view = CreateView(QuestSide.Player, QuestViewState.Available, QuestHideMode.None, hint);

		Assert.IsTrue(QuestHintDisplay.IsVisible(view));
		Assert.AreEqual("Real quest", QuestHintDisplay.GetDisplayName(view));
		Assert.AreEqual(hint, QuestHintDisplay.GetHint(view));
	}

	[TestMethod]
	public void NoSelection_HasNoMaskOrText()
	{
		Assert.IsFalse(QuestHintDisplay.IsVisible(null!));
		Assert.AreEqual(string.Empty, QuestHintDisplay.GetDisplayName(null!));
		Assert.AreEqual(string.Empty, QuestHintDisplay.GetHint(null!));
	}

	private static QuestView CreateView(QuestSide side, QuestViewState state, QuestHideMode mode, string hint) => new()
	{
		Identity = new QuestIdentity(side, "definition", "instance"),
		State = state,
		HideMode = mode,
		DisplayName = "Real quest",
		Hint = hint,
		Description = "Real description",
	};
}
