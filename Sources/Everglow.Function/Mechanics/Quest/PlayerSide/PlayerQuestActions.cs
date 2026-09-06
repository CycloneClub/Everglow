using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide;

public sealed class PlayerQuestActions
{
	private readonly PlayerQuestManager _manager;

	public PlayerQuestActions(PlayerQuestManager manager)
	{
		_manager = manager ?? throw new ArgumentNullException(nameof(manager));
	}

	public static IReadOnlyList<QuestActionType> GetAvailableTypes(PlayerQuestBase quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

		if (quest.State == PlayerQuestState.Available)
		{
			return [QuestActionType.Accept];
		}

		if (quest.State == PlayerQuestState.Accepted
			&& quest.CheckComplete())
		{
			return [QuestActionType.Submit];
		}

		if (quest.State == PlayerQuestState.Accepted
			&& quest.Cancellable)
		{
			return [QuestActionType.Cancel];
		}

		return [];
	}

	public bool TryExecute(QuestAction action)
	{
		QuestIdentity identity = action.Quest;
		if (identity.Side != QuestSide.Player)
		{
			return false;
		}

		var quest = _manager.GetQuest(identity.DefinitionId);
		if (quest is null
			|| !string.Equals(quest.InstanceId, identity.InstanceId, StringComparison.Ordinal)
			|| QuestHintRules.HasContent(quest.Hint))
		{
			return false;
		}

		if (action is { Type: QuestActionType.Retry, Args: int objectiveId })
		{
			return _manager.TryRetryObjective(quest.Name, objectiveId);
		}

		if (action.Args is not null || !GetAvailableTypes(quest).Contains(action.Type))
		{
			return false;
		}

		switch (action.Type)
		{
			case QuestActionType.Accept:
				_manager.ChangeQuestState(quest, PlayerQuestState.Available, PlayerQuestState.Accepted);
				break;
			case QuestActionType.Cancel:
				_manager.ChangeQuestState(quest, PlayerQuestState.Accepted, PlayerQuestState.Failed);
				break;
			case QuestActionType.Submit:
				quest.OnComplete();
				return quest.State == PlayerQuestState.Completed;
			default:
				return false;
		}

		return true;
	}
}
