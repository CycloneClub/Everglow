using System.Text;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.Commons.Mechanics.Quest.Presentation;

public static class TextDefinition
{
	private const string TimedOutObjectiveColor = "210,90,70,255";

	public static string GetQuestTypeText(QuestType? type) => type?.ToString() ?? "All";

	public static string GetQuestNotificationText(QuestView quest, QuestNotification notification)
	{
		ArgumentNullException.ThrowIfNull(quest);

		return notification.Type switch
		{
			QuestNotificationType.Unlocked => $"[{quest.DisplayName}]任务已解锁",
			QuestNotificationType.Restored => $"[{quest.DisplayName}]任务已恢复",
			QuestNotificationType.Failed => $"[{quest.DisplayName}]任务已失败",
			QuestNotificationType.Completed => $"[{quest.DisplayName}]任务已完成",
			QuestNotificationType.Restarted => $"[{quest.DisplayName}]任务已重启",
			QuestNotificationType.ObjectiveCompleted => $"[{quest.DisplayName}]任务当前节点[{notification.Detail}]中目标已完成",
			_ => throw new ArgumentOutOfRangeException(nameof(notification)),
		};
	}

	public static string GetQuestStateText(QuestViewState? state) => state switch
	{
		QuestViewState.Active => "Accepted",
		null => "All",
		_ => state.ToString(),
	};

	public static string GetQuestDetailText(QuestView quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

		var text = new StringBuilder();
		if (quest.TimeLimit.HasValue)
		{
			text.Append($"[TimerIconDrawer,QuestName='{quest.Identity.DefinitionId}'] 剩余时间:[TimerStringDrawer,QuestName='{quest.Identity.DefinitionId}']\n\n");
		}

		text.Append("描述：\n");
		text.Append(string.IsNullOrWhiteSpace(quest.Description) ? "无\n" : quest.Description + "\n");
		return text.ToString();
	}

	public static string GetQuestObjectivesText(QuestView quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

		var text = new StringBuilder("目标：\n");
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
		int mainIndex = 1;
		foreach (ObjectiveNodeView node in quest.ObjectiveNodes)
		{
			int subIndex = 1;
			bool completed = IsCompleted(node);
			foreach ((ObjectiveView objective, string objectiveText) in GetObjectiveLines(node))
			{
				string formattedObjective = FormatObjective(objective, objectiveText);
				string line = completed
					? $"[TextDrawer,Text='(已完成)',Color='100,100,100,255'] {formattedObjective}"
					: formattedObjective;
				lines.Add(new ObjectiveLineView(objective, $"{mainIndex}.{subIndex++} {line}"));
			}
			mainIndex++;
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
				QuestActionType.Accept => "接取",
				QuestActionType.Cancel => "放弃",
				QuestActionType.Retry => "重试",
				QuestActionType.ClaimReward => "领取奖励",
				QuestActionType.Submit => "提交",
				_ => "未知",
			}
			: entry.View.State switch
			{
				QuestViewState.Available => "接取",
				QuestViewState.Active => "进行中",
				QuestViewState.Completed => "完成",
				QuestViewState.Failed => "失败",
				QuestViewState.Locked => "锁定",
				_ => "未知",
			};
		return GetColoredText(text, color);
	}

	public static string GetRemainingTimeText(int? remainingTime)
	{
		if (!remainingTime.HasValue)
		{
			return "Indefinitely";
		}

		var time = new TimeSpan(0, 0, remainingTime.Value / 60);
		return $"{(int)time.TotalMinutes}Min {time.Seconds}s";
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
			text.Append(hours).Append('h');
		}

		if (hours > 0 || minutes > 0)
		{
			text.Append(minutes).Append('m');
		}

		if (seconds > 0 || text.Length == 0)
		{
			text.Append(seconds).Append('s');
		}

		return text.ToString();
	}

	public static string GetObjectiveTimerTooltip() => "重试";

	public static string GetObjectiveDurationTooltip(float currentDuration, float maxDuration) => $"Duration: {(int)currentDuration}/{(int)maxDuration}";

	public static string GetQuestLevelTooltip(int stars) => $"Quest Level: {stars}";

	public static string GetColoredText(string text, string color) => $"[TextDrawer,Text='{text}',Color='{color}']";

	private static IEnumerable<(ObjectiveView Objective, string Text)> GetObjectiveLines(ObjectiveNodeView node)
	{
		return node switch
		{
			LeafObjectiveNodeView leaf => [(leaf.Objective, leaf.Objective.ObjectiveText)],
			ParallelObjectiveNodeView parallel => parallel.Objectives.Select(objective => (objective, objective.ObjectiveText)),
			AnyOfObjectiveNodeView anyOf => anyOf.Objectives.Select(objective => (objective, objective.ObjectiveText)),
			BranchObjectiveNodeView branch => branch.Branches.SelectMany((branchView, branchIndex) =>
				branchView.Objectives.Select(objective =>
					(objective, $"[TextDrawer,Text='(Branch {branchIndex + 1})',Color='{GetBranchColor(branchView.State)}'] {objective.ObjectiveText}"))),
			_ => [],
		};
	}

	private static string FormatObjective(ObjectiveView objective, string text)
	{
		if (objective.State == ObjectiveViewState.TimedOut)
		{
			return $"[TextDrawer,Text='(已超时)',Color='{TimedOutObjectiveColor}'] {text}";
		}

		return text;
	}

	private static bool IsCompleted(ObjectiveNodeView node)
	{
		return node switch
		{
			LeafObjectiveNodeView leaf => leaf.Objective.State == ObjectiveViewState.Completed,
			ParallelObjectiveNodeView parallel => parallel.Objectives.All(objective => objective.State == ObjectiveViewState.Completed),
			AnyOfObjectiveNodeView anyOf => anyOf.Objectives.Any(objective => objective.State == ObjectiveViewState.Completed),
			BranchObjectiveNodeView branch => branch.Branches.Any(branchView =>
				branchView.State == ObjectiveBranchState.Selected
				&& branchView.Objectives.All(objective => objective.State == ObjectiveViewState.Completed)),
			_ => false,
		};
	}

	private static string GetBranchColor(ObjectiveBranchState state) => state switch
	{
		ObjectiveBranchState.Candidate => "100,180,120,255",
		ObjectiveBranchState.Selected => "100,255,100,255",
		ObjectiveBranchState.Skipped => "100,100,100,255",
		_ => "100,100,100,255",
	};
}
