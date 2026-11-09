using Everglow.Commons.Mechanics.Quest.Core.Dialogue;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class QuestDialogueSessionTest
{
	[TestMethod]
	public void ReadingDoesNotSubmitAndSuccessSurvivesObjectiveDeactivation()
	{
		int submissions = 0;
		bool complete = false;
		bool active = true;
		var dialogue = new ObjectiveDialogue().WithText("First", "Second").WithConfirmButtonText("Deliver").WithCompletionText("Thanks", "Goodbye");
		var session = new QuestDialogueSession(
			dialogue,
			() =>
			{
				submissions++;
				return true;
			},
			() => complete,
			() => active);
		Assert.AreEqual("First", session.ChatTextKey);
		session.Continue();
		Assert.AreEqual("Second", session.ChatTextKey);
		Assert.AreEqual(0, submissions);
		session.Continue();
		Assert.AreEqual(QuestDialogueState.WaitingForCompletion, session.State);
		session.Continue();
		Assert.AreEqual(1, submissions);
		complete = true;
		active = false;
		session.Update();
		Assert.AreEqual("Thanks", session.ChatTextKey);
		session.Continue();
		Assert.AreEqual("Goodbye", session.ChatTextKey);
		session.Continue();
		Assert.AreEqual(QuestDialogueState.Closed, session.State);
	}

	[TestMethod]
	public void UnavailableCanRetryButCancellationCannotSubmit()
	{
		bool ready = false;
		int submissions = 0;
		var dialogue = new ObjectiveDialogue().WithText("Request").WithUnavailableText("Missing");
		var session = new QuestDialogueSession(
			dialogue,
			() =>
			{
				submissions++;
				return ready;
			},
			() => ready,
			() => true);
		session.Continue();
		Assert.AreEqual("Missing", session.ChatTextKey);
		ready = true;
		session.Continue();
		Assert.AreEqual(QuestDialogueState.Closed, session.State);
		Assert.AreEqual(2, submissions);
		session.Continue();
		Assert.AreEqual(2, submissions);
	}

	[TestMethod]
	public void ExpiredOrRemotelyCompletedObjectiveCancelsUnsubmittedConversation()
	{
		bool active = true;
		int submissions = 0;
		var session = new QuestDialogueSession(
			new ObjectiveDialogue().WithText("Request"),
			() =>
			{
				submissions++;
				return true;
			},
			() => true,
			() => active);
		active = false;
		session.Continue();
		Assert.AreEqual(QuestDialogueState.Closed, session.State);
		Assert.AreEqual(0, submissions);
	}

	[TestMethod]
	public void ClickAtAcknowledgementBoundaryDoesNotSkipFirstSuccessLine()
	{
		bool complete = false;
		var session = new QuestDialogueSession(new ObjectiveDialogue().WithText("Request").WithCompletionText("Thanks", "Goodbye"), () => true, () => complete, () => true);
		session.Continue();
		complete = true;
		session.Continue();
		Assert.AreEqual("Thanks", session.ChatTextKey);
	}

	[TestMethod]
	public void DefaultDialogueStillRequiresOneExplicitConfirmation()
	{
		int submissions = 0;
		var session = new QuestDialogueSession(
			new ObjectiveDialogue(),
			() =>
			{
				submissions++;
				return true;
			},
			() => true,
			() => true);
		Assert.IsNull(session.ChatTextKey, "The caller supplies the objective's text when no dialogue is configured.");
		Assert.IsTrue(session.IsConfirmationRequired);
		Assert.AreEqual(0, submissions);
		session.Continue();
		Assert.AreEqual(1, submissions);
		Assert.AreEqual(QuestDialogueState.Closed, session.State);
	}
}
