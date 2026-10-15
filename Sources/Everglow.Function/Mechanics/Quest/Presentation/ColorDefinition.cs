using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.Commons.Mechanics.Quest.Presentation;

public static class ColorDefinition
{
	public static Color GetQuestNotificationColor(QuestNotificationType type) => type switch
	{
		QuestNotificationType.Unlocked or QuestNotificationType.Restored => new Color(150, 150, 250),
		QuestNotificationType.Failed => new Color(250, 150, 150),
		QuestNotificationType.Completed or QuestNotificationType.Restarted => new Color(150, 250, 150),
		QuestNotificationType.ObjectiveCompleted => new Color(250, 250, 150),
		_ => throw new ArgumentOutOfRangeException(nameof(type)),
	};

	public static Rectangle GetGemFrame(QuestType? questType) => questType switch
	{
		QuestType.None => new Rectangle(231, 0, 33, 33),
		QuestType.MainStory => new Rectangle(198, 0, 33, 33),
		QuestType.SideStory => new Rectangle(165, 0, 33, 33),
		QuestType.Achievement => new Rectangle(99, 0, 33, 33),
		QuestType.Challenge => new Rectangle(33, 0, 33, 33),
		QuestType.Daily => new Rectangle(66, 0, 33, 33),
		QuestType.Legend => new Rectangle(132, 0, 33, 33),
		_ => new Rectangle(0, 0, 33, 33),
	};

	public static Rectangle GetQuestStateFrame(QuestViewState? questState) => questState switch
	{
		QuestViewState.Active => new Rectangle(139, 36, 17, 67),
		QuestViewState.Available => new Rectangle(121, 36, 17, 67),
		QuestViewState.Failed => new Rectangle(103, 36, 17, 67),
		QuestViewState.Locked => new Rectangle(85, 36, 17, 67),
		QuestViewState.Completed => new Rectangle(67, 36, 17, 67),
		null => new Rectangle(157, 36, 17, 67),
		_ => new Rectangle(157, 36, 17, 67),
	};

	public static Rectangle GetQuestStateGemFrame(QuestViewState? questState)
	{
		const int FrameSize = 26;
		const int LockedFrameIndex = 5;
		int frameIndex = questState switch
		{
			null => 0,
			QuestViewState.Locked => LockedFrameIndex,
			_ => (int)questState.Value,
		};
		return new Rectangle(FrameSize * frameIndex, FrameSize, FrameSize, FrameSize);
	}
}
