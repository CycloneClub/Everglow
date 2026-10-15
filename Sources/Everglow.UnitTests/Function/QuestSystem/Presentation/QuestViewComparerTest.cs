using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class QuestViewComparerTest
{
	[TestMethod]
	public void Compare_SortsQuestTypesByDocumentedDisplayOrder()
	{
		QuestView[] quests =
		[
			new QuestView { State = QuestViewState.Active, Type = QuestType.None },
			new QuestView { State = QuestViewState.Active, Type = QuestType.Legend },
			new QuestView { State = QuestViewState.Active, Type = QuestType.Daily },
			new QuestView { State = QuestViewState.Active, Type = QuestType.Challenge },
			new QuestView { State = QuestViewState.Active, Type = QuestType.Achievement },
			new QuestView { State = QuestViewState.Active, Type = QuestType.SideStory },
			new QuestView { State = QuestViewState.Active, Type = QuestType.MainStory },
		];

		QuestType[] actualOrder =
		[
			.. quests
				.OrderBy(quest => quest, QuestViewComparer.Instance)
				.Select(quest => quest.Type),
		];

		CollectionAssert.AreEqual(
			new[]
			{
				QuestType.MainStory,
				QuestType.SideStory,
				QuestType.Achievement,
				QuestType.Challenge,
				QuestType.Daily,
				QuestType.Legend,
				QuestType.None,
			},
			actualOrder);
	}

	[TestMethod]
	public void Compare_SortsNoneAfterUndefinedQuestType()
	{
		var undefinedType = (QuestType)int.MaxValue;
		QuestView[] quests =
		[
			new QuestView { State = QuestViewState.Active, Type = QuestType.None },
			new QuestView { State = QuestViewState.Active, Type = undefinedType },
		];

		QuestType[] actualOrder =
		[
			.. quests
				.OrderBy(quest => quest, QuestViewComparer.Instance)
				.Select(quest => quest.Type),
		];

		CollectionAssert.AreEqual(new[] { undefinedType, QuestType.None }, actualOrder);
	}
}
