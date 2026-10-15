using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.UI;
using Everglow.Commons.Mechanics.Quest.WorldSide;

namespace Everglow.Commons.Mechanics.Quest.Presentation;

public sealed class QuestPresentationSystem : ModSystem
{
	private enum QuestEventType
	{
		Added,
		Removed,
		StatusUpdated,
		ObjectiveUpdated,
	}

	private readonly List<(QuestEventType Type, QuestIdentity Identity)> _pendingEvents = [];
	private readonly List<QuestNotification> _pendingNotifications = [];

	public event Action<QuestIdentity> QuestAdded;
	public event Action<QuestIdentity> QuestRemoved;
	public event Action<QuestIdentity> QuestStatusUpdated;
	public event Action<QuestIdentity> QuestObjectiveUpdated;

	public QuestPresentationService Service { get; private set; }

	public override void PostSetupContent()
	{
		PlayerQuestSystem playerSystem = ModContent.GetInstance<PlayerQuestSystem>();
		WorldQuestSystem worldSystem = ModContent.GetInstance<WorldQuestSystem>();
		Service = new QuestPresentationService(
			playerSystem.Manager,
			playerSystem.Actions,
			worldSystem.Manager,
			worldSystem.Actions);

		if (!Main.dedServ)
		{
			playerSystem.Manager.QuestAdded += identity => QueueEvent(QuestEventType.Added, identity);
			playerSystem.Manager.QuestRemoved += identity => QueueEvent(QuestEventType.Removed, identity);
			playerSystem.Manager.QuestStatusUpdated += identity => QueueEvent(QuestEventType.StatusUpdated, identity);
			playerSystem.Manager.QuestObjectiveUpdated += identity => QueueEvent(QuestEventType.ObjectiveUpdated, identity);
			worldSystem.Manager.QuestStatusUpdated += identity => QueueEvent(QuestEventType.StatusUpdated, identity);
			worldSystem.Manager.QuestObjectiveUpdated += identity => QueueEvent(QuestEventType.ObjectiveUpdated, identity);
			WorldQuestManager.NotificationRequested += QueueNotification;
			QuestContainer.Instance.SubscribePresentationEvents(this);
		}
	}

	public override void PostUpdateEverything()
	{
		(QuestEventType Type, QuestIdentity Identity)[] pendingEvents = _pendingEvents.Distinct().ToArray();
		_pendingEvents.Clear();

		foreach ((QuestEventType type, QuestIdentity identity) in pendingEvents)
		{
			switch (type)
			{
				case QuestEventType.Added:
					QuestAdded?.Invoke(identity);
					break;
				case QuestEventType.Removed:
					QuestRemoved?.Invoke(identity);
					break;
				case QuestEventType.StatusUpdated:
					QuestStatusUpdated?.Invoke(identity);
					break;
				case QuestEventType.ObjectiveUpdated:
					QuestObjectiveUpdated?.Invoke(identity);
					break;
			}
		}

		QuestNotification[] pendingNotifications = _pendingNotifications.Distinct().ToArray();
		_pendingNotifications.Clear();

		foreach (QuestNotification notification in pendingNotifications)
		{
			if (Service.TryGet(notification.Quest, out QuestPresentationEntry entry))
			{
				Main.NewText(
					TextDefinition.GetQuestNotificationText(entry.View, notification),
					ColorDefinition.GetQuestNotificationColor(notification.Type));
			}
		}
	}

	public override void Unload()
	{
		if (!Main.dedServ)
		{
			WorldQuestManager.NotificationRequested -= QueueNotification;
			QuestContainer.Instance.Unload();
		}

		_pendingEvents.Clear();
		_pendingNotifications.Clear();
		QuestAdded = null;
		QuestRemoved = null;
		QuestStatusUpdated = null;
		QuestObjectiveUpdated = null;
		Service = null;
	}

	private void QueueEvent(QuestEventType type, QuestIdentity identity) => _pendingEvents.Add((type, identity));

	private void QueueNotification(QuestNotification notification) => _pendingNotifications.Add(notification);
}
