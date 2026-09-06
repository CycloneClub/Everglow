using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.Commons.Mechanics.Quest.Presentation;

public class QuestViewComparer : IComparer<QuestView>
{
	public static readonly QuestViewComparer Instance = new QuestViewComparer();

	public int Compare(QuestView x, QuestView y)
	{
		int stateOrderComparison = GetStateSortOrder(x.State).CompareTo(GetStateSortOrder(y.State));
		if (stateOrderComparison != 0)
		{
			return stateOrderComparison;
		}
		else if (x.Type != y.Type)
		{
			int typeOrderComparison = GetTypeSortOrder(x.Type).CompareTo(GetTypeSortOrder(y.Type));
			return typeOrderComparison != 0 ? typeOrderComparison : x.Type.CompareTo(y.Type);
		}
		else
		{
			return string.Compare(x.DisplayName, y.DisplayName);
		}
	}

	private static int GetStateSortOrder(QuestViewState state) => state switch
	{
		QuestViewState.Active => 0,
		QuestViewState.Available => 1,
		QuestViewState.Completed => 2,
		QuestViewState.Failed => 3,
		QuestViewState.Locked => 4,
		_ => 5,
	};

	private static int GetTypeSortOrder(QuestType type) => type switch
	{
		QuestType.MainStory => 0,
		QuestType.SideStory => 1,
		QuestType.Achievement => 2,
		QuestType.Challenge => 3,
		QuestType.Daily => 4,
		QuestType.Legend => 5,
		QuestType.None => 7,
		_ => 6,
	};
}
