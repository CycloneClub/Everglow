using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.Commons.Mechanics.Quest.UI;

/// <summary>
/// UI-only disclosure of complete quest snapshots.
/// </summary>
public static class QuestHintDisplay
{
	public static bool IsVisible(QuestView quest) => quest is not null
		&& (quest.Identity.Side, quest.State) is
			(QuestSide.Player, QuestViewState.Available) or (QuestSide.World, QuestViewState.Locked);

	public static string GetDisplayName(QuestView quest) => IsVisible(quest)
		? quest.HideMode switch
		{
			QuestHideMode.None => quest.DisplayName,
			QuestHideMode.Name or QuestHideMode.NameAndConditions => QuestHintText.Masked,
			_ => throw new ArgumentOutOfRangeException(nameof(quest.HideMode)),
		}
		: quest?.DisplayName ?? string.Empty;

	public static string GetHint(QuestView quest) => IsVisible(quest)
		? quest.HideMode switch
		{
			QuestHideMode.None or QuestHideMode.Name => quest.Hint,
			QuestHideMode.NameAndConditions => QuestHintText.Masked,
			_ => throw new ArgumentOutOfRangeException(nameof(quest.HideMode)),
		}
		: quest?.Hint ?? string.Empty;
}
