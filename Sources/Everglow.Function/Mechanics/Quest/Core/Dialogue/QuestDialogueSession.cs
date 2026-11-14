namespace Everglow.Commons.Mechanics.Quest.Core.Dialogue;

public enum QuestDialogueState
{
	Reading,
	Unavailable,
	WaitingForCompletion,
	Completed,
	Closed,
}

/// <summary>单个本地交谈的阅读状态；不持久化，不自行完成目标或发奖。</summary>
public sealed class QuestDialogueSession
{
	private readonly ObjectiveDialogue dialogue;
	private readonly Func<bool> trySubmit;
	private readonly Func<bool> isCompleted;
	private readonly Func<bool> isAvailable;
	private int lineIndex;

	public QuestDialogueSession(ObjectiveDialogue dialogue, Func<bool> trySubmit, Func<bool> isCompleted, Func<bool> isAvailable)
	{
		ArgumentNullException.ThrowIfNull(dialogue);
		this.dialogue = dialogue;
		this.trySubmit = trySubmit;
		this.isCompleted = isCompleted;
		this.isAvailable = isAvailable;
	}

	public QuestDialogueState State { get; private set; }

	public string ChatTextKey => State switch
	{
		QuestDialogueState.Reading or QuestDialogueState.WaitingForCompletion => dialogue.TextKeys.Count == 0 ? null : dialogue.TextKeys[lineIndex],
		QuestDialogueState.Unavailable => dialogue.UnavailableTextKey,
		QuestDialogueState.Completed => dialogue.CompletionTextKeys[lineIndex],
		_ => null,
	};

	public string ConfirmButtonTextKey => dialogue.ConfirmButtonTextKey;

	public bool IsConfirmationRequired => State == QuestDialogueState.Unavailable
		|| (State == QuestDialogueState.Reading && lineIndex >= dialogue.TextKeys.Count - 1);

	public void Update()
	{
		if (State is QuestDialogueState.Closed or QuestDialogueState.Completed)
		{
			return;
		}
		if (State == QuestDialogueState.WaitingForCompletion && isCompleted())
		{
			lineIndex = 0;
			State = dialogue.CompletionTextKeys.Count == 0 ? QuestDialogueState.Closed : QuestDialogueState.Completed;
		}
		else if (!isAvailable())
		{
			Close();
		}
	}

	public void Continue()
	{
		if (State == QuestDialogueState.WaitingForCompletion)
		{
			Update();
			return;
		}
		Update();
		switch (State)
		{
			case QuestDialogueState.Reading when lineIndex < dialogue.TextKeys.Count - 1:
				lineIndex++;
				break;
			case QuestDialogueState.Reading:
			case QuestDialogueState.Unavailable:
				// 重试时等待页仍指向提交前的最后一句。
				State = trySubmit() ? QuestDialogueState.WaitingForCompletion : QuestDialogueState.Unavailable;
				Update();
				break;
			case QuestDialogueState.Completed:
				if (++lineIndex >= dialogue.CompletionTextKeys.Count)
				{
					Close();
				}
				break;
		}
	}

	public void Close() => State = QuestDialogueState.Closed;
}
