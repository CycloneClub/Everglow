using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.Commons.Mechanics.Quest.Presentation;

public class QuestViewComparer : IComparer<QuestView>
{
	public static readonly QuestViewComparer Instance = new QuestViewComparer();

	public int Compare(QuestView x, QuestView y)
	{
		if (x.State != y.State)
		{
			return x.State.CompareTo(y.State);
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
