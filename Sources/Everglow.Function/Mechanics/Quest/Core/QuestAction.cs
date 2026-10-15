namespace Everglow.Commons.Mechanics.Quest.Core;

public enum QuestActionType
{
	Accept,
	Cancel,
	Retry,
	ClaimReward,
	Submit,
}

public readonly record struct QuestAction(
	QuestIdentity Quest,
	QuestActionType Type,
	object? Args = null);
