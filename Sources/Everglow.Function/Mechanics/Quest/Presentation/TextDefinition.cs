using System.Text;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.Commons.Mechanics.Quest.Presentation;

public static class TextDefinition
{
	private const string TimedOutObjectiveColor = "210,90,70,255";

	public static string GetQuestTypeText(QuestType? type) =>
		QuestText.Get(type is null ? "Common.All" : Enum.IsDefined(type.Value) ? $"Types.{type}" : "Common.Unknown");

	public static string GetQuestNotificationText(QuestView quest, QuestNotification notification)
	{
		ArgumentNullException.ThrowIfNull(quest);

		return notification.Type switch
		{
			QuestNotificationType.Unlocked => QuestText.Get("Notifications.Unlocked", quest.DisplayName),
			QuestNotificationType.Restored => QuestText.Get("Notifications.Restored", quest.DisplayName),
			QuestNotificationType.Failed => QuestText.Get("Notifications.Failed", quest.DisplayName),
			QuestNotificationType.Completed => QuestText.Get("Notifications.Completed", quest.DisplayName),
			QuestNotificationType.Restarted => QuestText.Get("Notifications.Restarted", quest.DisplayName),
			QuestNotificationType.ObjectiveCompleted => QuestText.Get("Notifications.ObjectiveCompleted", quest.DisplayName, notification.Detail),
			_ => throw new ArgumentOutOfRangeException(nameof(notification)),
		};
	}

	public static string GetQuestStateText(QuestViewState? state) =>
		QuestText.Get(state is null ? "Common.All" : Enum.IsDefined(state.Value) ? $"States.{state}" : "Common.Unknown");

	public static string GetQuestDetailText(QuestView quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

		var text = new StringBuilder();
		if (quest.TimeLimit.HasValue)
		{
			string timer = $"[TimerStringDrawer,QuestName='{quest.Identity.DefinitionId}']";
			text.Append($"[TimerIconDrawer,QuestName='{quest.Identity.DefinitionId}'] {QuestText.Get("UI.RemainingTime", timer)}\n\n");
		}

		text.Append(QuestText.Get("UI.Description")).Append('\n');
		text.Append((string.IsNullOrWhiteSpace(quest.Description) ? QuestText.Get("Common.None") : quest.Description) + "\n");
		return text.ToString();
	}

	public static string GetQuestObjectivesText(QuestView quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

		var text = new StringBuilder(QuestText.Get("UI.Objectives") + "\n");
		foreach (ObjectiveLineView line in GetQuestObjectiveLines(quest))
		{
			text.Append(line.Text);
			if (!line.Text.EndsWith('\n'))
			{
				text.Append('\n');
			}
		}

		return text.ToString();
	}

	public static IReadOnlyList<ObjectiveLineView> GetQuestObjectiveLines(QuestView quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

		List<ObjectiveLineView> lines = [];
		foreach (ObjectiveView objective in QuestObjectiveDisplay.GetObjectives(quest.State, quest.ObjectiveNodes))
		{
			string text = FormatObjective(objective, objective.ObjectiveText);
			if (!string.IsNullOrWhiteSpace(objective.Description))
			{
				text = objective.Description + "\n" + text;
			}
			lines.Add(new ObjectiveLineView(objective, text));
		}

		return lines.ToArray();
	}

	public static string GetQuestActionText(QuestPresentationEntry entry, string color)
	{
		if (entry is null)
		{
			return GetColoredText(string.Empty, color);
		}

		string text = entry.Actions.Count > 0
			? entry.Actions[0].Type switch
			{
				QuestActionType.Accept => QuestText.Get("Actions.Accept"),
				QuestActionType.Cancel => QuestText.Get("Actions.Cancel"),
				QuestActionType.Retry => QuestText.Get("Actions.Retry"),
				QuestActionType.ClaimReward => QuestText.Get("Actions.ClaimReward"),
				QuestActionType.Submit => QuestText.Get("Actions.Submit"),
				_ => QuestText.Get("Common.Unknown"),
			}
			: entry.View.State switch
			{
				QuestViewState.Available => QuestText.Get("Actions.Accept"),
				QuestViewState.Active => QuestText.Get("States.Active"),
				QuestViewState.Completed => QuestText.Get("States.Completed"),
				QuestViewState.Failed => QuestText.Get("States.Failed"),
				QuestViewState.Locked => QuestText.Get("States.Locked"),
				_ => QuestText.Get("Common.Unknown"),
			};
		return GetColoredText(text, color);
	}

	public static string GetRemainingTimeText(int? remainingTime)
	{
		if (!remainingTime.HasValue)
		{
			return QuestText.Get("Time.Unlimited");
		}

		var time = new TimeSpan(0, 0, remainingTime.Value / 60);
		return QuestText.Get("Time.Remaining", (int)time.TotalMinutes, time.Seconds);
	}

	public static string GetObjectiveTimerText(int remainingTime)
	{
		int totalSeconds = Math.Max(0, remainingTime) / 60;
		int hours = totalSeconds / 3600;
		int minutes = totalSeconds % 3600 / 60;
		int seconds = totalSeconds % 60;

		var text = new StringBuilder();
		if (hours > 0)
		{
			text.Append(QuestText.Get("Time.Hours", hours));
		}

		if (hours > 0 || minutes > 0)
		{
			text.Append(QuestText.Get("Time.Minutes", minutes));
		}

		if (seconds > 0 || text.Length == 0)
		{
			text.Append(QuestText.Get("Time.Seconds", seconds));
		}

		return text.ToString();
	}

	public static string GetObjectiveTimerTooltip() => QuestText.Get("Actions.Retry");

	public static string GetObjectiveDurationTooltip(float currentDuration, float maxDuration) => QuestText.Get("UI.Duration", (int)currentDuration, (int)maxDuration);

	public static string GetQuestLevelTooltip(int stars) => QuestText.Get("UI.Level", stars);

	public static string GetColoredText(string text, string color) => $"[TextDrawer,Text='{text}',Color='{color}']";

	private static string FormatObjective(ObjectiveView objective, string text)
	{
		if (objective.State == ObjectiveViewState.TimedOut)
		{
			return $"{GetColoredText(QuestText.Get("Time.TimedOut"), TimedOutObjectiveColor)} {text}";
		}

		return text;
	}
}
