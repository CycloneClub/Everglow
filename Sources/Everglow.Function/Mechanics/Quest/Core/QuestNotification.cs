namespace Everglow.Commons.Mechanics.Quest.Core;

public enum QuestNotificationType
{
	Unlocked,
	Restored,
	Failed,
	Completed,
	Restarted,
	ObjectiveCompleted,
}

public readonly record struct QuestNotification(
	QuestIdentity Quest,
	QuestNotificationType Type,
	string Detail = null);
